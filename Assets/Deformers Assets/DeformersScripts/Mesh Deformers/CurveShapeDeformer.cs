using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class CurveShapeDeformer : MonoBehaviour {
	
	public enum Axis {X,Y,Z};
	[Tooltip("Choose the Axis you want the deformer to work on")]
	public Axis DeformAxis = Axis.Y;
	[Tooltip("Use this curve to define the deformation")]
	public AnimationCurve Refinecurve ;
	[Tooltip("Deformation Multiplier")]
	public float Multiplier = 1.0f;
	[Tooltip("Allow unity to re-calculate the normals, sometimes its needed, others no")]
	public bool RecalculateNormals = false;
	[Tooltip("Defines whether the Deformer is static or Dynamic, if true, the deformer will only be calculated once at Start")]
	public bool isStatic = false;

	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices;
	float smallestY =0;
	float largestY = 0;


	void Start () 
	{
		if (Refinecurve.length == 0) {
			Refinecurve = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0.5f));
		}
		deformingMesh = GetComponent<MeshFilter>().mesh;
		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
		for (int i = 0; i < originalVertices.Length; i++)
		{
			displacedVertices[i] = originalVertices[i];
			if(displacedVertices[i].y < smallestY)
			{
				smallestY = displacedVertices [i].y;
			}

			if(displacedVertices[i].y > largestY)
			{
				largestY = displacedVertices[i].y ;
			}
		}
		CurveUP ();
	}

	void FixedUpdate()
	{
		if (!isStatic) {
			CurveUP ();
		}
	}

	void CurveUP ()
	{
		for (int i = 0; i < originalVertices.Length; i++)
		{

			float x, y, z;
			x = originalVertices [i].x;
			y = originalVertices [i].y;
			z = originalVertices [i].z;
			float normalized = (y - smallestY) / (largestY - smallestY);
			float curveValue = Refinecurve.Evaluate (normalized);

			float new_x = x;
			float new_y = y;
			float new_z = z;

			switch (DeformAxis) 
			{
			case Axis.X:
				new_x = x + curveValue * Multiplier;
				break;
			case Axis.Y:
				new_y = y + curveValue * Multiplier;
				break;
			case Axis.Z:
				new_z = z + curveValue * Multiplier;
				break;
			}

			Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
			displacedVertices [i] = newvertPos;
		}

		deformingMesh.vertices = displacedVertices;
		if(RecalculateNormals){
		deformingMesh.RecalculateNormals();
		}
	}

}
