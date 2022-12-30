using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class FlareDeformer : MonoBehaviour {
	
	[Tooltip("Defines whether the Deformer is static or Dynamic, if true, the deformer will only be calculated once at Start, Iportant if you want to create a script to stack deformers")]
	public bool isStatic = false;
	public enum Axis {X,Y,Z};
	[Tooltip("Choose the Axis you want the deformer to work on")]
	public Axis DeformAxis = Axis.Y;
	[Tooltip("Use the curve to determin the influence of the deformer over the chosen Axis")]
	public AnimationCurve Refinecurve ;
	[Tooltip("Multiply the over-all effect of the deformer")]
	public float Multiplier = 1.0f;
	[Tooltip("Enable/Disable the use of the effector")]
	public bool UseEffector = false;
	[Tooltip("The effector Object (must have the effector script attached to it)")]
	public GameObject Effector;

	private EffectorVal theEffector;
	private float EffectorDistance = 3.0f;
	private bool InvertedEffector = false;
	private AnimationCurve ERefinecurve;
	private float EnormalizedCurve,EcurveValue;

	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices;
	Vector3 []normalVerts;
	float smallestY,largestY,smallestX,largestX,smallestZ,largestZ,normalized,curveValue =0;

	void Start () 
	{
		if (UseEffector != false) {
			if (Effector != null) {
				theEffector = Effector.GetComponent<EffectorVal> ();
			} else {
				Debug.LogWarning ("Please assign an effector to the effector Value, to create an effector go to: Mesh Deformer -> createEffector");
			}
		}
		if (Refinecurve.length == 0) {
			Refinecurve = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));
		}
		deformingMesh = GetComponent<MeshFilter>().mesh;
		originalVertices = deformingMesh.vertices;
		normalVerts = new Vector3[originalVertices.Length];

		displacedVertices = new Vector3[originalVertices.Length];
		for (int i = 0; i < originalVertices.Length; i++)
		{
			normalVerts [i] = Vector3.Normalize( deformingMesh.normals[i]);

			displacedVertices[i] = originalVertices[i];

			if(displacedVertices[i].y < smallestY)
			{
				smallestY = displacedVertices [i].y;
			}
			if(displacedVertices[i].y > largestY)
			{
				largestY = displacedVertices[i].y ;
			}
			if(displacedVertices[i].x < smallestX)
			{
				smallestX = displacedVertices [i].x;
			}
			if(displacedVertices[i].x > largestX)
			{
				largestX = displacedVertices[i].x ;
			}
			if(displacedVertices[i].z < smallestZ)
			{
				smallestZ = displacedVertices [i].z;
			}
			if(displacedVertices[i].z > largestZ)
			{
				largestZ = displacedVertices[i].z ;
			}
		}

		if (!UseEffector) {
			flareUP ();
		} else {
			flareUPEffector ();
		}
	}

	void FixedUpdate()
	{
		if (!isStatic) {

			if (!UseEffector) {
				flareUP ();
			} else {
				flareUPEffector ();
			}
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
		//deformingMesh.RecalculateNormals();
	}

	void flareUPEffector ()
	{
		InvertedEffector = theEffector.Inverted;
		ERefinecurve = theEffector.FallOffCurve;
		EffectorDistance = theEffector.EffectorDistance;

		if (!InvertedEffector) {

			for (int i = 0; i < originalVertices.Length; i++) {			
				if (Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position) <= EffectorDistance) { 
					float dist = Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position);
					EnormalizedCurve = dist / EffectorDistance;
					EcurveValue = ERefinecurve.Evaluate (EnormalizedCurve);
					float x, y, z;
					x = originalVertices [i].x;
					y = originalVertices [i].y;
					z = originalVertices [i].z;
					float new_x = x;
					float new_y = y;
					float new_z = z;
					switch (DeformAxis) {

					case Axis.X:
						normalized = (x - smallestX) / (largestX - smallestX);
						curveValue = Refinecurve.Evaluate (normalized);
						new_y = ((y + normalVerts [i].y / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						new_z = ((z + normalVerts [i].z / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						break;

					case Axis.Y:
						normalized = (y - smallestY) / (largestY - smallestY);
						curveValue = Refinecurve.Evaluate (normalized);
						new_x = ((x + normalVerts [i].x / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						new_z = ((z + normalVerts [i].z / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						break;

					case Axis.Z:
						normalized = (z - smallestZ) / (largestZ - smallestZ);
						curveValue = Refinecurve.Evaluate (normalized);
						new_x = ((x + normalVerts [i].x / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						new_y = ((y + normalVerts [i].y / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						break;
					}

					Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
					displacedVertices [i] = newvertPos;
				} else {
					displacedVertices [i] = originalVertices [i];
				}
			} 

			deformingMesh.vertices = displacedVertices;
		} else 
		{
			for (int i = 0; i < originalVertices.Length; i++) {			
				if (Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position) <= EffectorDistance) { 
					float dist = Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position);
					EnormalizedCurve = (dist - EffectorDistance) / EffectorDistance;
					EcurveValue = ERefinecurve.Evaluate (EnormalizedCurve);
					float x, y, z;
					x = originalVertices [i].x;
					y = originalVertices [i].y;
					z = originalVertices [i].z;

					float new_x = x;
					float new_y = y;
					float new_z = z;
					switch (DeformAxis) {

					case Axis.X:
						normalized = (x - smallestX) / (largestX - smallestX);
						curveValue = Refinecurve.Evaluate (normalized);
						new_y = ((y + normalVerts [i].y / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						new_z = ((z + normalVerts [i].z / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						break;

					case Axis.Y:
						normalized = (y - smallestY) / (largestY - smallestY);
						curveValue = Refinecurve.Evaluate (normalized);
						new_x = ((x + normalVerts [i].x / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						new_z = ((z + normalVerts [i].z / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						break;

					case Axis.Z:
						normalized = (z - smallestZ) / (largestZ - smallestZ);
						curveValue = Refinecurve.Evaluate (normalized);
						new_x = ((x + normalVerts [i].x / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						new_y = ((y + normalVerts [i].y / 100000.0f) * Multiplier * curveValue) * EcurveValue;
						break;
					}

					Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
					displacedVertices [i] = newvertPos;
				} else {
					displacedVertices [i] = originalVertices [i];
				}
			} 

			deformingMesh.vertices = displacedVertices;
		}
	}
}
