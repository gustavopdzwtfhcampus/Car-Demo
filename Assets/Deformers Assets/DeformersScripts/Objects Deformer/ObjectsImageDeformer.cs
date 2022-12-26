using UnityEngine;
using System.Collections;

public class ObjectsImageDeformer : MonoBehaviour {

	[Tooltip("The image used for the deformation. Must be set to Advanced, and Read/Write Enabled must be true in the texture import settings")]
	public Texture2D heightmap;
	public enum Axis {X,Y,Z};
	[Tooltip("Choose the Axis you want the deformer to work on, set this value before hitting play")]
	public Axis DeformAxis = Axis.Y;
	[Tooltip("calculate the deformation once at Start or keep it dynamic")]
	public bool IsStaic = false;
	[Tooltip("Height Multiplier of the deformer")]
	public float Height = 5.0f;
	[Tooltip("offset the image texture (kind of like repeat UV")]
	public float StretchX= 1.01f,StretchXB= 1.01f,StretchZ= 1.01f,StretchZB = 1.01f;
	[Tooltip("An Array of textures that the deformer can use as an animation. Must be set to Advanced, and Read/Write Enabled must be true in the texture import settings")]
	public Texture2D[] AnimatableMaps;
	[Tooltip("enable the Animation through the array of images")]
	public bool AnimateThroughMaps = false;
	[Tooltip("Loop Through the animation")]
	public bool	AnimLoop = false;
	[Tooltip("time to change the image")]
	public float AnimFrameChangeInSec = 1.0f;

	private float tempAnimVal = 1.0f;
	private float startTime = 0.0f;
	private int startval = 0;
	private float[] OriginalY,OriginalX,OriginalZ;
	Transform[] originalObjects, movedObjects;
	Vector3 [] displacedObjects, originalPositions;
	float smallestX,largestX,smallestZ,largestZ,smallestY,largestY = 0;

	void Start () 
	{
		tempAnimVal = AnimFrameChangeInSec;
		originalObjects = new Transform[transform.childCount];
		movedObjects = new Transform[originalObjects.Length];
		OriginalY = new float[originalObjects.Length];
		OriginalX = new float[originalObjects.Length];
		OriginalZ = new float[originalObjects.Length];
		for (int i = 0; i < transform.childCount; i++) 
		{
			originalObjects [i] = transform.GetChild (i);
		}
		movedObjects = originalObjects;

		if (originalObjects.Length != 0) //check if the array is empty
		{
			
			smallestX = originalObjects [0].transform.position.x;//set an initial relevant value
			largestX = smallestX;//set an initial relevant value
			smallestZ = originalObjects [0].transform.position.z;//set an initial relevant value
			largestZ = smallestZ;//set an initial relevant value

			for (int i = 0; i < originalObjects.Length; i++) //find the smallest and largest values of x and z to be able to normalize later on
			{
				OriginalY [i] = originalObjects [i].transform.position.y;
				OriginalX [i] = originalObjects [i].transform.position.x;
				OriginalZ [i] = originalObjects [i].transform.position.z;

				if (originalObjects [i].transform.position.x < smallestX) {
					smallestX = originalObjects [i].transform.position.x;
				}

				if (originalObjects [i].transform.position.x > largestX) {
					largestX = originalObjects [i].transform.position.x;
				}

				if (originalObjects [i].transform.position.y < smallestY) {
					smallestY = originalObjects [i].transform.position.y;
				}

				if (originalObjects [i].transform.position.y > largestY) {
					largestY = originalObjects [i].transform.position.y;
				}

				if (originalObjects [i].transform.position.z < smallestZ) {
					smallestZ = originalObjects [i].transform.position.z;
				}

				if (originalObjects [i].transform.position.z > largestZ) {
					largestZ = originalObjects [i].transform.position.z;
				}
			}
		}
		else 
		{
			Debug.Log ("Add children to the object with this script");
		}
		if (IsStaic) 
		{
			moveObjects ();
		}
	}

	void FixedUpdate() 
	{
		if (!IsStaic) 
		{
			if (AnimateThroughMaps == true && AnimatableMaps.Length != 0) {
				heightmap = AnimatableMaps [startval];
				startTime += Time.deltaTime;
				if(startTime >= tempAnimVal)
				{
					startval++;
					if (startval >= AnimatableMaps.Length&& AnimLoop == true) {
						startval = 0;
					}
					else if(startval >= AnimatableMaps.Length && AnimLoop == false)
					{
						startval = AnimatableMaps.Length - 1;
					}
					tempAnimVal += AnimFrameChangeInSec;
				}
			}
			moveObjects ();
		}
	}


	void moveObjects()
	{

		switch (DeformAxis) 
		{
		case Axis.X:
			DeformX ();
			break;
		case Axis.Y:
			DeformY ();
			break;
		case Axis.Z:
			DeformZ ();
			break;
		}

	}

	void DeformX()
	{
		for (int i = 0; i < originalObjects.Length; i++) 
		{
			float normalizedY = (OriginalY [i] - smallestY*StretchX) / (largestY*StretchXB - smallestY*StretchX);
			float normalizedZ = (OriginalZ [i]- smallestZ*StretchZ) / (largestZ*StretchZB - smallestZ*StretchZ);

			int y = Mathf.FloorToInt(normalizedY * heightmap.width);
			int z = Mathf.FloorToInt(normalizedZ * heightmap.height);

			Vector3 pos = originalObjects [i].position;
			float HeightValue = heightmap.GetPixel (y, z).grayscale * Height;
			pos.x = OriginalX[i] + HeightValue;
			movedObjects[i].position = pos;
		}
	}

	void DeformY()
	{
		for (int i = 0; i < originalObjects.Length; i++) 
		{
			float normalizedX = (OriginalX [i] - smallestX*StretchX) / (largestX*StretchXB - smallestX*StretchX);
			float normalizedZ = (OriginalZ [i] - smallestZ*StretchZ) / (largestZ*StretchZB - smallestZ*StretchZ);

			int x = Mathf.FloorToInt(normalizedX * heightmap.width);
			int z = Mathf.FloorToInt(normalizedZ * heightmap.height);

			Vector3 pos = originalObjects [i].position;
			float HeightValue = heightmap.GetPixel (x, z).grayscale * Height;
			pos.y = OriginalY[i] + HeightValue;
			movedObjects[i].position = pos;
		}
	}

	void DeformZ()
	{
		for (int i = 0; i < originalObjects.Length; i++) 
		{
			float normalizedX = (OriginalX [i] - smallestX*StretchX) / (largestX*StretchXB - smallestX*StretchX);
			float normalizedY = (OriginalY [i] - smallestY*StretchZ) / (largestY*StretchZB - smallestY*StretchZ);

			int x = Mathf.FloorToInt(normalizedX * heightmap.width);
			int y = Mathf.FloorToInt(normalizedY * heightmap.height);

			Vector3 pos = originalObjects [i].position;
			float HeightValue = heightmap.GetPixel (x, y).grayscale * Height;
			pos.z = OriginalZ[i] + HeightValue;
			movedObjects[i].position = pos;
		}
	}
}
