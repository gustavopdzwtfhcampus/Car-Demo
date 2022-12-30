using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skinnedMeshRipple : MonoBehaviour {


	public enum Axis {X,Y,Z};
	[Tooltip("Choose the Axis you want the deformer to work on")]
	public Axis DeformAxis = Axis.Y;
	[Tooltip("increase or decrease Ripples")]
	public float Frequency =2.0f;
	[Tooltip("Cycle through the Ripples")]
	public float Phaze = 1.0f;
	[Tooltip("Height of the Ripples")]
	public float PeakMultiplier=1.0f;
	[Tooltip("Enable the animation of the Ripples, PS. animates the phase value")]
	public bool AnimatePhaze = false;
	[Tooltip("The speed for animating the ripples")]
	public float AnimationSpeed = 10.0f;
	[Tooltip("Offset the Ripple center")]
	public float OffsetA,OffsetB = 0.0f;
	[Tooltip("Defines whether the Deformer is static or Dynamic, if true, the deformer will only be calculated once at Start")]
	public bool isStatic = false;
	[Tooltip("Enable/Disable the use of the effector")]
	public bool UseEffector = false;
	[Tooltip("The effector Object (must have the effector script attached to it)")]
	public GameObject Effector;

	float new_y, new_x, new_z,xsquared,ysquared,zsquared = 0;
	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices;
	float normalized,curveValue,normalized2,curveValue2;
	private bool ISkinned = true;
	private EffectorVal theEffector;
	private float EffectorDistance = 3.0f;
	private bool InvertedEffector = false;
	private AnimationCurve Refinecurve;
	private float normalizedCurve;

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
			deformingMesh = GetComponent<SkinnedMeshRenderer> ().sharedMesh;

			originalVertices = deformingMesh.vertices;
			displacedVertices = new Vector3[originalVertices.Length];
			if (isStatic) {
				if (!UseEffector) {
					Ripple ();
				} else {
					RippleEffector ();
				}
			}
		}
	}

	void FixedUpdate()
	{
		if (ISkinned) {
			if (!isStatic) {
				if (!UseEffector) {
					Ripple ();
				} else {
					RippleEffector ();
				}
				if (AnimatePhaze) {
					animateRipple ();
				}
			}
		}
	}

	void Ripple()
	{
		for (int i = 0; i < originalVertices.Length; i++) {
			float x, y, z;
			x = originalVertices [i].x;
			y = originalVertices [i].y;
			z = originalVertices [i].z;
			switch (DeformAxis)
			{
			case Axis.X:
				new_y = y;
				ysquared = Mathf.Pow (y+OffsetA, 2);
				zsquared = Mathf.Pow (z+OffsetB, 2);
				new_x = x + Mathf.Sin (Frequency * Mathf.Sqrt (ysquared +zsquared) + Phaze)*PeakMultiplier;
				new_z = z;
				break;

			case Axis.Y:
				new_x = x;
				xsquared = Mathf.Pow (x+OffsetA,2);
				zsquared =  Mathf.Pow (z+OffsetB,2);
				new_y =y + Mathf.Sin (Frequency * Mathf.Sqrt (xsquared +zsquared) + Phaze)*PeakMultiplier;
				new_z = z;
				break;

			case Axis.Z:
				new_x = x;
				xsquared = Mathf.Pow (x+OffsetA, 2);
				ysquared = Mathf.Pow (y+OffsetB, 2);
				new_z = z + Mathf.Sin (Frequency * Mathf.Sqrt (xsquared +ysquared) + Phaze)*PeakMultiplier;
				new_y = y;
				break;
			}

			Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
			displacedVertices [i] = newvertPos;
		}
		deformingMesh.vertices = displacedVertices;
	}	

	public void animateRipple()
	{
		Phaze += Time.deltaTime * AnimationSpeed;
	}

	void OnApplicationQuit()
	{
		PeakMultiplier = 0;
		if (deformingMesh != null) {
			Ripple ();
		}
	}

	void RippleEffector()
	{
		InvertedEffector = theEffector.Inverted;
		Refinecurve = theEffector.FallOffCurve;
		EffectorDistance = theEffector.EffectorDistance;

		if (!InvertedEffector) {
			for (int i = 0; i < originalVertices.Length; i++) {
				if (Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position) <= EffectorDistance) { 
					float dist = Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position);
					normalizedCurve = dist / EffectorDistance;
					curveValue = Refinecurve.Evaluate (normalizedCurve);
					float x, y, z;
					x = originalVertices [i].x;
					y = originalVertices [i].y;
					z = originalVertices [i].z;
					switch (DeformAxis) {
					case Axis.X:
						new_y = y;
						ysquared = Mathf.Pow (y + OffsetA, 2);
						zsquared = Mathf.Pow (z + OffsetB, 2);
						new_x = x + Mathf.Sin (Frequency * Mathf.Sqrt (ysquared + zsquared) + Phaze) * PeakMultiplier * curveValue;
						new_z = z;
						break;

					case Axis.Y:
						new_x = x;
						xsquared = Mathf.Pow (x + OffsetA, 2);
						zsquared = Mathf.Pow (z + OffsetB, 2);
						new_y = y + Mathf.Sin (Frequency * Mathf.Sqrt (xsquared + zsquared) + Phaze) * PeakMultiplier * curveValue;
						new_z = z;
						break;

					case Axis.Z:
						new_x = x;
						xsquared = Mathf.Pow (x + OffsetA, 2);
						ysquared = Mathf.Pow (y + OffsetB, 2);
						new_z = z + Mathf.Sin (Frequency * Mathf.Sqrt (xsquared + ysquared) + Phaze) * PeakMultiplier * curveValue;
						new_y = y;
						break;
					}

					Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
					displacedVertices [i] = newvertPos;
				} else {
					displacedVertices [i] = originalVertices [i];
				}
			}
		} else {
			for (int i = 0; i < originalVertices.Length; i++) {
				if (Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position) >= EffectorDistance) { 
					float dist = Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position);
					normalizedCurve = (dist - EffectorDistance) / EffectorDistance;
					curveValue = Refinecurve.Evaluate (normalizedCurve);
					float x, y, z;
					x = originalVertices [i].x;
					y = originalVertices [i].y;
					z = originalVertices [i].z;
					switch (DeformAxis) {
					case Axis.X:
						new_y = y;
						ysquared = Mathf.Pow (y + OffsetA, 2);
						zsquared = Mathf.Pow (z + OffsetB, 2);
						new_x = x + Mathf.Sin (Frequency * Mathf.Sqrt (ysquared + zsquared) + Phaze) * PeakMultiplier * curveValue;
						new_z = z;
						break;

					case Axis.Y:
						new_x = x;
						xsquared = Mathf.Pow (x + OffsetA, 2);
						zsquared = Mathf.Pow (z + OffsetB, 2);
						new_y = y + Mathf.Sin (Frequency * Mathf.Sqrt (xsquared + zsquared) + Phaze) * PeakMultiplier * curveValue;
						new_z = z;
						break;

					case Axis.Z:
						new_x = x;
						xsquared = Mathf.Pow (x + OffsetA, 2);
						ysquared = Mathf.Pow (y + OffsetB, 2);
						new_z = z + Mathf.Sin (Frequency * Mathf.Sqrt (xsquared + ysquared) + Phaze) * PeakMultiplier * curveValue;
						new_y = y;
						break;
					}

					Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
					displacedVertices [i] = newvertPos;
				} else {
					displacedVertices [i] = originalVertices [i];
				}
			}
		}

		deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
	}	
}
