using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshSine : MonoBehaviour {

	//Public Vars
	public enum Axis {X,Y,Z};
	[Tooltip("Choose the Axis you want the deformer to work on")]
	public Axis DeformAxis = Axis.Y;
	[Tooltip("Use this curve to further refine the deformation")]
	public AnimationCurve Refinecurve ;
	[Tooltip("Choose the Axis the curve will act on")]
	public Axis CurveInfluence = Axis.X;
	[Tooltip("changes the formula of the deformer slightly, only works withthe Y-Axis")]
	public bool predictable = true;
	[Tooltip("the amount of sine waves")]
	public float Frequency =1.75f;
	[Tooltip("cycle throught the waves")]
	public float Phaze = 1.0f;
	[Tooltip("Waves Height multiplier")]
	public float PeakMultiplier=1.0f;
	[Tooltip("Animate the waves")]
	public bool AnimatePhaze = false;
	[Tooltip("the speed of the waves animation")]
	public float AnimationSpeed = 10.0f;
	[Tooltip("Defines whether the Deformer is static or Dynamic, if true, the deformer will only be calculated once at Start")]
	public bool IsStatic = false;
	[Tooltip("Enable/Disable the use of the effector")]
	public bool UseEffector = false;
	[Tooltip("The effector Object (must have the effector script attached to it)")]
	public GameObject Effector;


	//private Vars
	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices;
	float normalized , curveValue;
	float smallestY,largestY,smallestX,largestX,smallestZ,largestZ =0;
	float offsetMin,offsetMax = 0; 
	private bool ISkinned = true;

	private EffectorVal theEffector;
	private float EffectorDistance = 3.0f;
	private bool InvertedEffector = false;
	private AnimationCurve ERefinecurve;
	private float EnormalizedCurve,EcurveValue;

	void Start () 
	{	
		if (UseEffector != false) {
			if (Effector != null) {
				theEffector = Effector.GetComponent<EffectorVal> ();
			} else {
				Debug.LogWarning ("Please assign an effector to the effector Value, to create an effector go to: Mesh Deformer -> createEffector");
			}
		}
	
		if (GetComponent<SkinnedMeshRenderer> () == null) {
			Debug.Log ("Please assign this script to a Skinned Mesh, you can find out which mesh is skinned by checking if it has a Skinned Mesh Renderer Attached to it");
			ISkinned = false;
		} else {
			if (Refinecurve.length == 0) {
				Refinecurve = new AnimationCurve (new Keyframe (0, 1), new Keyframe (1, 1));
			}
		deformingMesh =  GetComponent<SkinnedMeshRenderer> ().sharedMesh;
			originalVertices = deformingMesh.vertices;
			displacedVertices = new Vector3[originalVertices.Length];
			for (int i = 0; i < originalVertices.Length; i++) {
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
			if (IsStatic) 
			{
				if (!UseEffector) {
					SineWave ();
				} else {
					SineWaveEffector ();
				}
			}
		}
	}

	void FixedUpdate()
	{
		if (!IsStatic) {
			if (!UseEffector) {
				SineWave ();
			} else {
				SineWaveEffector ();
			}
			if (AnimatePhaze) {
				animateSine ();
			}
		}
	}

	void SineWave ()
	{
		switch (CurveInfluence) 
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
		for (int i = 0; i < originalVertices.Length; i++)
		{


			float x = originalVertices [i].x;
			float y = originalVertices [i].y;
			float z = originalVertices [i].z;

			float new_x = x;
			float new_y = y;
			float new_z = z;

			switch (DeformAxis) 
			{
			case Axis.X:
				switch (CurveInfluence) 
				{
				case Axis.X:
					normalized = (x - offsetMin) / (offsetMax - offsetMin);
					break;

				case Axis.Y:
					normalized = (y - offsetMin) / (offsetMax - offsetMin);
					break;

				case Axis.Z:
					normalized = (z - offsetMin) / (offsetMax - offsetMin);
					break;
				} 

				curveValue = Refinecurve.Evaluate (normalized);
				new_x = (x + Mathf.Sin (Frequency*(y+z)+ Phaze)*PeakMultiplier * curveValue);
				break;

			case Axis.Y:
				switch (CurveInfluence) 
				{
				case Axis.X:
					normalized = (x - offsetMin) / (offsetMax - offsetMin);
					break;

				case Axis.Y:
					normalized = (y - offsetMin) / (offsetMax - offsetMin);
					break;

				case Axis.Z:
					normalized = (z - offsetMin) / (offsetMax - offsetMin);
					break;
				} 
				curveValue = Refinecurve.Evaluate (normalized);
				if (!predictable) {
					new_y = y+ PeakMultiplier * Mathf.Sin ((Frequency * x + Phaze) * Mathf.PI * curveValue);
				}
				else{
					new_y = y+ PeakMultiplier * Mathf.Sin(Frequency * x + Phaze) *curveValue;

				}
				break;
			case Axis.Z:
				switch (CurveInfluence) 
				{
				case Axis.X:
					normalized = (x - offsetMin) / (offsetMax - offsetMin);
					break;

				case Axis.Y:
					normalized = (y - offsetMin) / (offsetMax - offsetMin);
					break;

				case Axis.Z:
					normalized = (z - offsetMin) / (offsetMax - offsetMin);
					break;
				} 
				curveValue = Refinecurve.Evaluate (normalized);
				new_z = (z + Mathf.Sin (Frequency*(y+x) + Phaze)*PeakMultiplier * curveValue);
				break;
			}

			Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
			displacedVertices [i] = newvertPos;
		}

		deformingMesh.vertices = displacedVertices;
	}

	public void animateSine()
	{
		Phaze += Time.deltaTime * AnimationSpeed;
	}

	void OnApplicationQuit()
	{
		PeakMultiplier = 0;
		if (deformingMesh != null) {
			SineWave ();
		}
	}

	void SineWaveEffector ()
	{
		InvertedEffector = theEffector.Inverted;
		ERefinecurve = theEffector.FallOffCurve;
		EffectorDistance = theEffector.EffectorDistance;

		switch (CurveInfluence) 
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
		if (!InvertedEffector) {
			for (int i = 0; i < originalVertices.Length; i++) {
				if (Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position) <= EffectorDistance) { 
					float dist = Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position);
					EnormalizedCurve = dist / EffectorDistance;
					EcurveValue = ERefinecurve.Evaluate (EnormalizedCurve);
					float x = originalVertices [i].x;
					float y = originalVertices [i].y;
					float z = originalVertices [i].z;

					float new_x = x;
					float new_y = y;
					float new_z = z;

					switch (DeformAxis) {
					case Axis.X:
						switch (CurveInfluence) {
						case Axis.X:
							normalized = (x - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Y:
							normalized = (y - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Z:
							normalized = (z - offsetMin) / (offsetMax - offsetMin);
							break;
						} 

						curveValue = Refinecurve.Evaluate (normalized);
						new_x = (x + Mathf.Sin (Frequency * (y + z) + Phaze) * PeakMultiplier * curveValue * EcurveValue);
						break;

					case Axis.Y:
						switch (CurveInfluence) {
						case Axis.X:
							normalized = (x - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Y:
							normalized = (y - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Z:
							normalized = (z - offsetMin) / (offsetMax - offsetMin);
							break;
						} 
						curveValue = Refinecurve.Evaluate (normalized);
						if (!predictable) {
							new_y = y + PeakMultiplier * Mathf.Sin ((Frequency * x + Phaze) * Mathf.PI * curveValue * EcurveValue);
						} else {
							new_y = y + PeakMultiplier * Mathf.Sin (Frequency * x + Phaze) * curveValue * EcurveValue;
						}
						break;
					case Axis.Z:
						switch (CurveInfluence) {
						case Axis.X:
							normalized = (x - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Y:
							normalized = (y - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Z:
							normalized = (z - offsetMin) / (offsetMax - offsetMin);
							break;
						} 
						curveValue = Refinecurve.Evaluate (normalized);
						new_z = (z + Mathf.Sin (Frequency * (y + x) + Phaze) * PeakMultiplier * curveValue * EcurveValue);
						break;
					}

					Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
					displacedVertices [i] = newvertPos;
				} else {
					displacedVertices [i] = originalVertices [i];
				}
			}

			deformingMesh.vertices = displacedVertices;
			deformingMesh.RecalculateNormals ();
		} else {
			for (int i = 0; i < originalVertices.Length; i++) {
				if (Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position) >= EffectorDistance) { 
					float dist = Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position);
					EnormalizedCurve = (dist - EffectorDistance) / EffectorDistance;
					EcurveValue = ERefinecurve.Evaluate (EnormalizedCurve);
					float x = originalVertices [i].x;
					float y = originalVertices [i].y;
					float z = originalVertices [i].z;
					float new_x = x;
					float new_y = y;
					float new_z = z;

					switch (DeformAxis) {
					case Axis.X:
						switch (CurveInfluence) {
						case Axis.X:
							normalized = (x - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Y:
							normalized = (y - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Z:
							normalized = (z - offsetMin) / (offsetMax - offsetMin);
							break;
						} 

						curveValue = Refinecurve.Evaluate (normalized);
						new_x = (x + Mathf.Sin (Frequency * (y + z) + Phaze) * PeakMultiplier * curveValue * EcurveValue);
						break;

					case Axis.Y:
						switch (CurveInfluence) {
						case Axis.X:
							normalized = (x - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Y:
							normalized = (y - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Z:
							normalized = (z - offsetMin) / (offsetMax - offsetMin);
							break;
						} 
						curveValue = Refinecurve.Evaluate (normalized);
						if (!predictable) {
							new_y = y + PeakMultiplier * Mathf.Sin ((Frequency * x + Phaze) * Mathf.PI * curveValue * EcurveValue);
						} else {
							new_y = y + PeakMultiplier * Mathf.Sin (Frequency * x + Phaze) * curveValue * EcurveValue;
						}

						break;
					case Axis.Z:
						switch (CurveInfluence) {
						case Axis.X:
							normalized = (x - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Y:
							normalized = (y - offsetMin) / (offsetMax - offsetMin);
							break;

						case Axis.Z:
							normalized = (z - offsetMin) / (offsetMax - offsetMin);
							break;
						} 
						curveValue = Refinecurve.Evaluate (normalized);
						new_z = (z + Mathf.Sin (Frequency * (y + x) + Phaze) * PeakMultiplier * curveValue * EcurveValue);
						break;
					}

					Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
					displacedVertices [i] = newvertPos;
				} else {
					displacedVertices [i] = originalVertices [i];
				}
			}

			deformingMesh.vertices = displacedVertices;
			deformingMesh.RecalculateNormals ();
		}
	}

}
