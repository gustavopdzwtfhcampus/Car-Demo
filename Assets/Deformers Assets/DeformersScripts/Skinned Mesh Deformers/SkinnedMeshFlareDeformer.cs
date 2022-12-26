using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshFlareDeformer : MonoBehaviour {

	[Tooltip("Defines whether the Deformer is static or Dynamic, if true, the deformer will only be calculated once at Start, Iportant if you want to create a script to stack deformers")]
	public bool isStatic = false;
	public enum Axis {X,Y,Z};
	[Tooltip("Choose the Axis you want the deformer to work on")]
	public Axis DeformAxis = Axis.Y;
	[Tooltip("Use the curve to determin the influence of the deformer over the chosen Axis")]
	public AnimationCurve Refinecurve ;
	[Tooltip("Multiply the over-all effect of the deformer")]
	public float Multiplier = 1.0f;

	// private Vars
	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices;
	Vector3 []normalVerts;
	float smallestY,largestY,smallestX,largestX,smallestZ,largestZ,normalized,curveValue =0;
	private bool ISkinned = true;

	void Start () 
	{if (GetComponent<SkinnedMeshRenderer> () == null) {
			Debug.Log ("Please assign this script to a Skinned Mesh, you can find out which mesh is skinned by checking if it has a Skinned Mesh Renderer Attached to it");
			ISkinned = false;
		} else {
			if (Refinecurve.length == 0) {
				Refinecurve = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));
			}
			deformingMesh = GetComponent<SkinnedMeshRenderer> ().sharedMesh;
			originalVertices = deformingMesh.vertices;
			normalVerts = new Vector3[originalVertices.Length];

			displacedVertices = new Vector3[originalVertices.Length];
			for (int i = 0; i < originalVertices.Length; i++) {
				normalVerts [i] = Vector3.Normalize (deformingMesh.normals [i]);

				displacedVertices [i] = originalVertices [i];

				if (displacedVertices [i].y < smallestY) {
					smallestY = displacedVertices [i].y;
				}
				if (displacedVertices [i].y > largestY) {
					largestY = displacedVertices [i].y;
				}
				if (displacedVertices [i].x < smallestX) {
					smallestX = displacedVertices [i].x;
				}
				if (displacedVertices [i].x > largestX) {
					largestX = displacedVertices [i].x;
				}
				if (displacedVertices [i].z < smallestZ) {
					smallestZ = displacedVertices [i].z;
				}
				if (displacedVertices [i].z > largestZ) {
					largestZ = displacedVertices [i].z;
				}
			}
			flareUP ();
		}
	}

	void FixedUpdate()
	{
		if (!isStatic && ISkinned) {
			flareUP ();
		}
	}

	void flareUP ()
	{
		for (int i = 0; i < originalVertices.Length; i++)
		{
			float x, y, z;
			x = originalVertices [i].x;
			y = originalVertices [i].y;
			z = originalVertices [i].z;

			float new_x = x;
			float new_y = y;
			float new_z = z;
			switch (DeformAxis)
			{
			case Axis.X:
				normalized = (x - smallestX) / (largestX - smallestX);
				curveValue = Refinecurve.Evaluate (normalized);

				new_y = (y + normalVerts [i].y/100000.0f)* Multiplier * curveValue;
				new_z = (z + normalVerts[i].z/100000.0f)* Multiplier* curveValue;
				break;

			case Axis.Y:
				normalized = (y - smallestY) / (largestY - smallestY);
				curveValue = Refinecurve.Evaluate (normalized);
				new_x = (x + normalVerts [i].x/100000.0f)* Multiplier * curveValue;
				new_z = (z + normalVerts[i].z/100000.0f)* Multiplier* curveValue;
				break;

			case Axis.Z:
				normalized = (z - smallestZ) / (largestZ - smallestZ);
				curveValue = Refinecurve.Evaluate (normalized);
				new_x = (x + normalVerts [i].x/100000.0f)* Multiplier * curveValue;
				new_y = (y + normalVerts[i].y/100000.0f)* Multiplier* curveValue;
				break;
			}

			Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
			displacedVertices [i] = newvertPos;
		}

		deformingMesh.vertices = displacedVertices;
	}

	void OnApplicationQuit()
	{
		Multiplier = 1;
		Refinecurve = new AnimationCurve (new Keyframe (0, 1), new Keyframe (1, 1));
		if (deformingMesh != null) {
			flareUP ();
		}
	}
}
