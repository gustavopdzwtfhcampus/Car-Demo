using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MeshFilter))]
public class TwistDeformer : MonoBehaviour {

	public enum Axis {X,Y,Z};
	[Tooltip("Choose the Axis you want the deformer to work on")]
	public Axis DeformAxis = Axis.Y;
	[Tooltip("The Angle of twist")]
	public float angleOfTwist = 30.0f;
	[Range(0.1f,100.0f)]
	public float TwistSize =1.0f;
	[Tooltip("area where the deformer has no effect can only be changed after the execution of the script")]
	public float offsetMin = 0; 
	[Tooltip("area where the deformer has no effect can only be changed after the execution of the script")]
	public float offsetMax = 0;
	[Tooltip("Use this curve to further refine the deformation")]
	public AnimationCurve Refinecurve ;
	[Tooltip("Deformation Multiplier")]
	public float multiplier = 1.0f;
	[Tooltip("calculte every frame or just at start")]
	public bool IsStatic = false;
	[Tooltip("Enable/Disable the use of the effector")]
	public bool UseEffector = false;
	[Tooltip("The effector Object (must have the effector script attached to it)")]
	public GameObject Effector;

	private EffectorVal theEffector;
	private float EffectorDistance = 3.0f;
	private bool InvertedEffector = false;
	private AnimationCurve ERefinecurve;
	private float EnormalizedCurve,EcurveValue;

	private Axis TempAxis = Axis.Y;
	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices;
	float smallestY,largestY,smallestX,largestX,smallestZ,largestZ =0;
	float OffsetTwist = 0.5f;
	float normalized,curveValue = 0;

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
			Refinecurve = new AnimationCurve (new Keyframe (0 ,1), new Keyframe (1,1));
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
				offsetMin = smallestY; //setting a default value

			}

			if(displacedVertices[i].y > largestY)
			{
				largestY = displacedVertices[i].y ;
				offsetMax = largestY; //setting a default value
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
		if (IsStatic) {
			
			if (!UseEffector) {
				twist ();
			} else {
				twistEffector ();
			}
		}
	}
		
	void FixedUpdate()
	{
		if (!UseEffector) {
			twist ();
		} else {
			twistEffector ();
		}
	}

	void twist()
	{
		if (DeformAxis != TempAxis) {
			TempAxis = DeformAxis;
			switch (DeformAxis) 
			{
			case Axis.X:
				offsetMin = smallestX;
				offsetMax = largestX;
				break;

			case Axis.Y:
				offsetMin = smallestY;
				offsetMax = largestY;
				break;

			case Axis.Z:
				offsetMin = smallestZ;
				offsetMax = largestZ;
				break;
			}
		}

		for (int i = 0; i < originalVertices.Length; i++)
		{ 
			float x, y, z;
			x = originalVertices [i].x;
			y = originalVertices [i].y;
			z = originalVertices [i].z;
			switch (DeformAxis)
			{
			case Axis.X:
				normalized = (x - smallestX) / (largestX - smallestX);
				curveValue = Refinecurve.Evaluate (normalized);
				if (x >= offsetMin && x <= offsetMax ) {
					float ang = ((TwistSize *OffsetTwist + x) / TwistSize * angleOfTwist)*multiplier*curveValue;
					float new_x = x;
					float new_y = y * Mathf.Cos (ang) - z * Mathf.Sin (ang)*curveValue;
					float new_z = y * Mathf.Sin (ang) + z * Mathf.Cos (ang)*curveValue;
					Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
					displacedVertices [i] = newvertPos;
				} else 
				{
					displacedVertices [i] = originalVertices [i];
				}
				break;

			case Axis.Y:
				normalized = (y - smallestY) / (largestY - smallestY);
				curveValue = Refinecurve.Evaluate (normalized);
				if (y >= offsetMin && y <= offsetMax ) {
					float ang = ((TwistSize * OffsetTwist + y) / TwistSize * angleOfTwist)*multiplier;
					float new_x = (x * Mathf.Cos (ang) - z * Mathf.Sin (ang))*curveValue;
					float new_y = y;
					float new_z = (x * Mathf.Sin (ang) + z * Mathf.Cos (ang))*curveValue;
					Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
					displacedVertices [i] = newvertPos;
				} else 
				{
					displacedVertices [i] = originalVertices [i];
				}
				break;

			case Axis.Z:
				normalized = (z - smallestZ) / (largestZ - smallestZ);
				curveValue = Refinecurve.Evaluate (normalized);
				if (z >= offsetMin && z <= offsetMax ) {
					float ang = ((TwistSize *OffsetTwist + z) / TwistSize * angleOfTwist)*multiplier*curveValue;
					float new_x = y * Mathf.Sin (ang) + x * Mathf.Cos (ang)*curveValue;
					float new_y = y * Mathf.Cos (ang) - x * Mathf.Sin (ang)*curveValue;
					float new_z = z;
					Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
					displacedVertices [i] = newvertPos;
				} else 
				{
					displacedVertices [i] = originalVertices [i];
				}
				break;
			}
		}
		deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
	}

	void twistEffector ()
	{
		InvertedEffector = theEffector.Inverted;
		ERefinecurve = theEffector.FallOffCurve;
		EffectorDistance = theEffector.EffectorDistance;

			if (DeformAxis != TempAxis) {
				TempAxis = DeformAxis;
				switch (DeformAxis) {
				case Axis.X:
					offsetMin = smallestX;
					offsetMax = largestX;
					break;

				case Axis.Y:
					offsetMin = smallestY;
					offsetMax = largestY;
					break;

				case Axis.Z:
					offsetMin = smallestZ;
					offsetMax = largestZ;
					break;
				}
			}
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
					switch (DeformAxis) {
					case Axis.X:
						normalized = (x - smallestX) / (largestX - smallestX);
						curveValue = Refinecurve.Evaluate (normalized);
						if (x >= offsetMin && x <= offsetMax) {
							float ang = ((TwistSize * OffsetTwist + x) / TwistSize * angleOfTwist) * multiplier * curveValue * EcurveValue;
							float new_x = x;
							float new_y = y * Mathf.Cos (ang) - z * Mathf.Sin (ang);
							float new_z = y * Mathf.Sin (ang) + z * Mathf.Cos (ang);
							Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
							displacedVertices [i] = newvertPos;
						} else {
							displacedVertices [i] = originalVertices [i];
						}
						break;

					case Axis.Y:
						normalized = (y - smallestY) / (largestY - smallestY);
						curveValue = Refinecurve.Evaluate (normalized);
						if (y >= offsetMin && y <= offsetMax) {
							float ang = ((TwistSize * OffsetTwist + y) / TwistSize * angleOfTwist) * multiplier * curveValue * EcurveValue;
							float new_x = x * Mathf.Cos (ang) - z * Mathf.Sin (ang);
							float new_y = y;
							float new_z = x * Mathf.Sin (ang) + z * Mathf.Cos (ang);
							Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
							displacedVertices [i] = newvertPos;
						} else {
							displacedVertices [i] = originalVertices [i];
						}
						break;

					case Axis.Z:
						normalized = (z - smallestZ) / (largestZ - smallestZ);
						curveValue = Refinecurve.Evaluate (normalized);
						if (z >= offsetMin && z <= offsetMax) {
							float ang = ((TwistSize * OffsetTwist + z) / TwistSize * angleOfTwist) * multiplier * curveValue * EcurveValue;
							float new_x = y * Mathf.Sin (ang) + x * Mathf.Cos (ang);
							float new_y = y * Mathf.Cos (ang) - x * Mathf.Sin (ang);
							float new_z = z;
							Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
							displacedVertices [i] = newvertPos;
						} else {
							displacedVertices [i] = originalVertices [i];
						}
						break;
					}
				} else {
					displacedVertices [i] = originalVertices [i];
				}
			}
			deformingMesh.vertices = displacedVertices;
			deformingMesh.RecalculateNormals ();
		} else 
		{
			for (int i = 0; i < originalVertices.Length; i++) { 
				if (Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position) >= EffectorDistance) { 
					float dist = Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position);
					EnormalizedCurve = (dist - EffectorDistance) / EffectorDistance;
					EcurveValue = ERefinecurve.Evaluate (EnormalizedCurve);
					float x, y, z;
					x = originalVertices [i].x;
					y = originalVertices [i].y;
					z = originalVertices [i].z;
					switch (DeformAxis) {
					case Axis.X:
						normalized = (x - smallestX) / (largestX - smallestX);
						curveValue = Refinecurve.Evaluate (normalized);
						if (x >= offsetMin && x <= offsetMax) {
							float ang = ((TwistSize * OffsetTwist + x) / TwistSize * angleOfTwist) * multiplier * curveValue * EcurveValue;
							float new_x = x;
							float new_y = y * Mathf.Cos (ang) - z * Mathf.Sin (ang);
							float new_z = y * Mathf.Sin (ang) + z * Mathf.Cos (ang);
							Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
							displacedVertices [i] = newvertPos;
						} else {
							displacedVertices [i] = originalVertices [i];
						}
						break;

					case Axis.Y:
						normalized = (y - smallestY) / (largestY - smallestY);
						curveValue = Refinecurve.Evaluate (normalized);
						if (y >= offsetMin && y <= offsetMax) {
							float ang = ((TwistSize * OffsetTwist + y) / TwistSize * angleOfTwist) * multiplier * curveValue * EcurveValue;
							float new_x = x * Mathf.Cos (ang) - z * Mathf.Sin (ang);
							float new_y = y;
							float new_z = x * Mathf.Sin (ang) + z * Mathf.Cos (ang);
							Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
							displacedVertices [i] = newvertPos;
						} else {
							displacedVertices [i] = originalVertices [i];
						}
						break;

					case Axis.Z:
						normalized = (z - smallestZ) / (largestZ - smallestZ);
						curveValue = Refinecurve.Evaluate (normalized);
						if (z >= offsetMin && z <= offsetMax) {
							float ang = ((TwistSize * OffsetTwist + z) / TwistSize * angleOfTwist) * multiplier * curveValue * EcurveValue;
							float new_x = y * Mathf.Sin (ang) + x * Mathf.Cos (ang);
							float new_y = y * Mathf.Cos (ang) - x * Mathf.Sin (ang);
							float new_z = z;
							Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
							displacedVertices [i] = newvertPos;
						} else {
							displacedVertices [i] = originalVertices [i];
						}
						break;
					}
				} else {
					displacedVertices [i] = originalVertices [i];
				}
			}
			deformingMesh.vertices = displacedVertices;
			deformingMesh.RecalculateNormals ();
		}
	}
}
