using UnityEngine;
using System.Collections;

public class ObjectsRipple : MonoBehaviour {

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
	public float OffsetRippleCenterA = 1.0f, OffsetRippleCenterB= 1.0f;
	[Tooltip("Move objects in a Set Axis")]
	public float moveX, moveY, moveZ = 0;
	[Tooltip("Defines whether the Deformer is static or Dynamic, if true, the deformer will only be calculated once at Start")]
	public bool IsStaic = false;

	float new_y, new_x, new_z,xsquared,ysquared,zsquared = 0;
	Mesh deformingMesh;
	Transform[] originalObjects, movedObjects;
	Vector3 [] displacedObjects, originalPositions;
	Vector3 newObjectPos = Vector3.zero;


	void Start () 
	{
		originalObjects = new Transform[transform.childCount];
		movedObjects = new Transform[originalObjects.Length];
		displacedObjects = new Vector3[originalObjects.Length];
		originalPositions = new Vector3[originalObjects.Length];
		for (int i = 0; i < transform.childCount; i++) 
		{
			originalObjects [i] = transform.GetChild (i);
			displacedObjects [i] = originalObjects [i].position;
			originalPositions[i] = originalObjects [i].position;
		}
		movedObjects = originalObjects;
		if (IsStaic) 
		{
			Ripple ();
		}
	}

	void FixedUpdate()
	{
		if (!IsStaic) {
			Ripple ();
			if (AnimatePhaze) {
				animateRipple ();
			}
		}
	}

	void Ripple()
	{
		for (int i = 0; i < originalObjects.Length; i++) {

		
			float x = originalPositions [i].x + moveX;
			float y = originalPositions [i].y + moveY;
			float z = originalPositions [i].z + moveZ;

			switch (DeformAxis)
			{
			case Axis.X:
				new_y = y;
				ysquared = Mathf.Pow (y+OffsetRippleCenterA, 2);
				zsquared = Mathf.Pow (z+OffsetRippleCenterB, 2);
				new_x = x + Mathf.Sin (Frequency * Mathf.Sqrt (ysquared +zsquared) + Phaze)*PeakMultiplier;
				new_z = z;
				break;

			case Axis.Y:
				new_x = x;
				new_z = z;
				xsquared = Mathf.Pow (x+OffsetRippleCenterA,2);
				zsquared =  Mathf.Pow (z+OffsetRippleCenterB,2);
				new_y = y + Mathf.Sin (Frequency * Mathf.Sqrt (xsquared +zsquared) + Phaze)*PeakMultiplier;

				break;

			case Axis.Z:
				new_x = x;
				xsquared = Mathf.Pow (x+OffsetRippleCenterA, 2);
				ysquared = Mathf.Pow (y+OffsetRippleCenterB, 2);
				new_z = z + Mathf.Sin (Frequency * Mathf.Sqrt (xsquared +ysquared) + Phaze)*PeakMultiplier;
				new_y = y;
				break;
			}

			newObjectPos.Set(new_x, new_y, new_z); //Vector3.set
			displacedObjects [i] = newObjectPos;
		}
		for (int x = 0; x < originalObjects.Length; x++) 
		{
			movedObjects [x].position = displacedObjects [x];		
		}
	}	

	public void animateRipple()
	{
		Phaze += Time.deltaTime * AnimationSpeed;
	}

}
