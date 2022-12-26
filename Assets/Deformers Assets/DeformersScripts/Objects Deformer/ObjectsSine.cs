using UnityEngine;
using System.Collections;

public class ObjectsSine : MonoBehaviour {
	//Public Vars
	public enum Axis {X,Y,Z};
	[Tooltip("Choose the Axis you want the deformer to work on")]
	public Axis DeformAxis = Axis.Y;
	[Tooltip("Use this curve to further refine the deformation")]
	public AnimationCurve Refinecurve ;
	[Tooltip("Choose the Axis the curve will act on")]
	public Axis CurveInfluence = Axis.X;
	[Tooltip("the amount of sine waves")]
	public float Frequency =0.5f;
	[Tooltip("cycle throught the waves")]
	public float Phaze = 1.0f;
	[Tooltip("Waves Height multiplier")]
	public float PeakMultiplier=1.0f;
	[Tooltip("Animate the waves")]
	public bool AnimatePhaze = false;	
	[Tooltip("the speed of the waves animation")]
	public float AnimationSpeed = 10.0f;
	[Tooltip("Defines whether the Deformer is static or Dynamic, if true, the deformer will only be calculated once at Start")]
	public bool IsStaic = false;
	[Tooltip("move through the corresponding axis")]
	public float moveX, moveY, moveZ = 0;

	//private Vars
	Transform[] originalObjects, movedObjects;
	Vector3 [] displacedObjects, originalPositions;
	float normalized , curveValue;
	float smallestY,largestY,smallestX,largestX,smallestZ,largestZ =0;
	float offsetMin,offsetMax = 0; 
	Vector3 newObjectPos;

	private float[] OriginalY,OriginalX,OriginalZ;
	private Vector3 originalsLerper;

	void Start () 
	{
		if (Refinecurve.length == 0) {
			Refinecurve = new AnimationCurve (new Keyframe (0, 1), new Keyframe (1, 1));
		}
			originalObjects = new Transform[transform.childCount];
			movedObjects = new Transform[originalObjects.Length];
			displacedObjects = new Vector3[originalObjects.Length];
			OriginalY = new float[originalObjects.Length];
			OriginalX = new float[originalObjects.Length];
			OriginalZ = new float[originalObjects.Length];
			originalPositions = new Vector3[originalObjects.Length];
			for (int i = 0; i < transform.childCount; i++) 
			{
				originalObjects [i] = transform.GetChild (i);
				displacedObjects [i] = originalObjects [i].position;
				originalPositions[i] = originalObjects [i].position;
			}
			movedObjects = originalObjects;
		if(originalObjects.Length !=0)//if the array is not empty fill variables with relevant values
		{
			smallestY=originalObjects [0].position.y;
			largestY=originalObjects [0].position.y;
			smallestX=originalObjects [0].position.x;
			largestX=originalObjects [0].position.x;
			smallestZ=originalObjects [0].position.z;
			largestZ =originalObjects [0].position.z;
		}
		for (int i = 0; i <originalObjects.Length; i++)
		{
			OriginalY [i] = originalObjects [i].transform.position.y;
			OriginalX [i] = originalObjects [i].transform.position.x;
			OriginalZ [i] = originalObjects [i].transform.position.z;
			if(originalObjects[i].position.y < smallestY)
			{
				smallestY = originalObjects[i].position.y;
			}

			if(originalObjects[i].position.y > largestY)
			{
				largestY = originalObjects[i].position.y ;
			}
			if(originalObjects[i].position.x < smallestX)
			{
				smallestX =originalObjects[i].position.x;
			}

			if(originalObjects[i].position.x > largestX)
			{
				largestX = originalObjects[i].position.x ;
			}

			if(originalObjects[i].position.z < smallestZ)
			{
				smallestZ = originalObjects[i].position.z;
			}

			if(originalObjects[i].position.z > largestZ)
			{
				largestZ = originalObjects[i].position.z ;
			}
		}
		if (IsStaic) 
		{
			SineWave ();
		}
	}

	void FixedUpdate()
	{
		if (!IsStaic) {
			SineWave ();
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

		if (offsetMin == offsetMax) 
		{
			offsetMin = 0;
			offsetMax = 1;
		}
		for (int i = 0; i < originalObjects.Length; i++)
		{
			float x, y, z;
			x = originalPositions [i].x+ moveX;
			y = originalPositions [i].y+ moveY;
			z = originalPositions [i].z+ moveZ;

			float new_x = x;
			float new_y = y;
			float new_z = z;

			switch (DeformAxis) 
			{
			case Axis.X:
				switch (CurveInfluence) 
				{
				case Axis.X:
					normalized = (x - offsetMin) / (offsetMax -  offsetMin);
					break;

				case Axis.Y:
					normalized = (y - offsetMin) / (offsetMax - offsetMin);
					break;

				case Axis.Z:
					normalized = (z - offsetMin) / (offsetMax - offsetMin);
					break;
				} 
				curveValue = Refinecurve.Evaluate (normalized);
				new_x = x + Mathf.Sin (Frequency * z + Phaze)*PeakMultiplier*curveValue;
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
				new_y = y + Mathf.Sin (Frequency * x + Phaze) * PeakMultiplier*curveValue; 
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
				new_z = z + Mathf.Sin (Frequency * x  + Phaze) * PeakMultiplier*curveValue; 
				break;
			}


			newObjectPos = new Vector3 (new_x, new_y, new_z);
			displacedObjects [i] = newObjectPos;
		}

		for (int x = 0; x < originalObjects.Length; x++) 
		{
						movedObjects [x].position = displacedObjects [x];
		} 

	}

	public void animateSine()
	{
		Phaze += Time.deltaTime * AnimationSpeed;
	}

}
