using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotateEffectorDeformer : MonoBehaviour {

	public float RotateX = 0.0f,RotateY = 60.0f,RotateZ = 0.0f;

	Transform[] originalObjects, movedObjects;
	Vector3 [] displacedObjects, originalPositions;
	Vector3 newObjectPos;
	public GameObject Effector;
	private float EffectorDistance = 3.0f;
	private float EffectorRecoverySpeed = 10.0f;
	private bool InvertedEffector = false;
	private float[] OriginalY,OriginalX,OriginalZ;
	private float[] oriRotX, oriRotY, oriRotZ;
	private Vector3 originalsLerper,OriginalsPos;
	private Vector3 RotationVector = Vector3.zero;
	private EffectorVal theEffector;
	private float normalizedCurve, curveValue = 1f;
	private AnimationCurve Refinecurve;
	private float rangeSmallest = 0f;
	void Start () 
	{
		if (Effector == null) {
			Debug.LogWarning ("Please assign an effector to the effector Value, to create an effector go to: Mesh Deformer -> createEffector");
		} else 
		{
			theEffector = Effector.GetComponent<EffectorVal> ();
		}

		originalObjects = new Transform[transform.childCount];
		movedObjects = new Transform[originalObjects.Length];
		displacedObjects = new Vector3[originalObjects.Length];
		OriginalY = new float[originalObjects.Length];
		OriginalX = new float[originalObjects.Length];
		OriginalZ = new float[originalObjects.Length];

		oriRotX = new float[originalObjects.Length];
		oriRotY = new float[originalObjects.Length];
		oriRotZ = new float[originalObjects.Length];

		originalPositions = new Vector3[originalObjects.Length];
		for (int i = 0; i < transform.childCount; i++) 
		{
			originalObjects [i] = transform.GetChild (i);
			displacedObjects [i] = originalObjects [i].position;
			originalPositions[i] = originalObjects [i].position;
		}
		movedObjects = originalObjects;

		for (int i = 0; i < originalObjects.Length; i++) 
		{
			OriginalY [i] = originalObjects [i].transform.position.y;
			OriginalX [i] = originalObjects [i].transform.position.x;
			OriginalZ [i] = originalObjects [i].transform.position.z;

			oriRotY [i] = originalObjects [i].transform.eulerAngles.y;
			oriRotX [i] = originalObjects [i].transform.eulerAngles.x;
			oriRotZ [i] = originalObjects [i].transform.eulerAngles.z;
		}
	}

	void FixedUpdate()
	{
		if (Effector != null) {
			TransformChanger ();
		}
	}

	void TransformChanger ()
	{
		InvertedEffector = theEffector.Inverted;
		Refinecurve = theEffector.FallOffCurve;
		EffectorDistance = theEffector.EffectorDistance;
		EffectorRecoverySpeed = theEffector.EffectorRecoverySpeed;

		for (int i = 0; i < originalObjects.Length; i++) {
			float x, y, z;
			Vector3 posstatic = new Vector3 (OriginalX [i], OriginalY [i], OriginalZ [i]);
			float dist = Vector3.Distance (posstatic, Effector.transform.position);
			normalizedCurve = (dist - rangeSmallest) / (EffectorDistance - rangeSmallest);
			curveValue = Refinecurve.Evaluate (normalizedCurve);

			x = oriRotX [i] + RotateX*curveValue;
			y = oriRotY [i] + RotateY*curveValue;
			z = oriRotZ [i] + RotateZ*curveValue;

			float new_x = x;
			float new_y = y;
			float new_z = z;

			newObjectPos = new Vector3 (new_x, new_y, new_z);
			displacedObjects [i] = newObjectPos;
		}
		if (!InvertedEffector) {
			for (int x = 0; x < originalObjects.Length; x++) {
				OriginalsPos.Set (OriginalX [x], OriginalY [x], OriginalZ [x]);
				if (Vector3.Distance (OriginalsPos, Effector.transform.position) <= EffectorDistance) {
					float xLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.x, displacedObjects [x].x,  EffectorRecoverySpeed * Time.deltaTime);
					float yLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.y, displacedObjects [x].y,  EffectorRecoverySpeed * Time.deltaTime);
					float zLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.z, displacedObjects [x].z,  EffectorRecoverySpeed * Time.deltaTime);
					RotationVector.Set (xLerp, yLerp, zLerp);
					movedObjects [x].eulerAngles = RotationVector;
				} else {
					originalsLerper.Set (oriRotX [x], oriRotY [x], oriRotZ [x]);
					float xLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.x, originalsLerper.x,  EffectorRecoverySpeed * Time.deltaTime);
					float yLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.y, originalsLerper.y,  EffectorRecoverySpeed * Time.deltaTime);
					float zLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.z, originalsLerper.z,  EffectorRecoverySpeed * Time.deltaTime);
					RotationVector.Set (xLerp, yLerp, zLerp);
					movedObjects [x].eulerAngles = RotationVector;
				
				}
			}
		} else {
			for (int x = 0; x < originalObjects.Length; x++) {
				OriginalsPos.Set (OriginalX [x], OriginalY [x], OriginalZ [x]);
				if (Vector3.Distance (OriginalsPos, Effector.transform.position) > EffectorDistance) {
					float xLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.x, displacedObjects [x].x,  EffectorRecoverySpeed * Time.deltaTime);
					float yLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.y, displacedObjects [x].y,  EffectorRecoverySpeed * Time.deltaTime);
					float zLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.z, displacedObjects [x].z,  EffectorRecoverySpeed * Time.deltaTime);
					RotationVector.Set (xLerp, yLerp, zLerp);
					movedObjects [x].eulerAngles = RotationVector;
				} else {
					originalsLerper.Set (oriRotX [x], oriRotY [x], oriRotZ [x]);
					float xLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.x, originalsLerper.x,  EffectorRecoverySpeed * Time.deltaTime);
					float yLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.y, originalsLerper.y,  EffectorRecoverySpeed * Time.deltaTime);
					float zLerp = Mathf.LerpAngle(movedObjects [x].transform.eulerAngles.z, originalsLerper.z,  EffectorRecoverySpeed * Time.deltaTime);
					RotationVector.Set (xLerp, yLerp, zLerp);
					movedObjects [x].eulerAngles = RotationVector;
				}
			}
		}
	}
}
