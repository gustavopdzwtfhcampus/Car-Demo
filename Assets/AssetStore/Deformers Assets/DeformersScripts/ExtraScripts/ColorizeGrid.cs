using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorizeGrid : MonoBehaviour {
	[Tooltip("The image used for coloring. Must be set toRead/Write Enabled must be true in the advanced section of the texture import settings")]
	public Texture2D ColorMap;
	[Tooltip("An Array of textures that the colorize script can use as an animation. Must be set toRead/Write Enabled must be true in the advanced section of the texture import settings")]
	public Texture2D[] AnimatableMaps;
	public enum Axis {X,Y};
	[Tooltip("Choose the Axis you want the colored image to be applied to")]
	public Axis DeformAxis = Axis.X;
	[Tooltip("offset the image texture (kind of like repeat UV")]
	public float StretchXB= 1.01f,StretchZB = 1.01f;
	public bool AnimateThroughMaps = false;
	[Tooltip("Loop Through the animation")]
	public bool	AnimLoop = false;
	[Tooltip("time to change the image")]
	public float AnimFrameChangeInSec = 1.0f;
	[Tooltip("calculte every frame or just at start")]
	public bool IsStatic = false;

	private float[] OriginalY,OriginalX,OriginalZ;
	float smallestX,largestX,smallestZ,largestZ,smallestY,largestY = 0;
	private float tempAnimVal = 1.0f;
	private float startTime = 0.0f;
	private int startval = 0;
	Transform[] originalObjects;
	float StretchX= 1.0f,StretchZ= 1.0f;

	void Start () 
	{
		tempAnimVal = AnimFrameChangeInSec;
		originalObjects = new Transform[transform.childCount];
		OriginalY = new float[originalObjects.Length];
		OriginalX = new float[originalObjects.Length];
		OriginalZ = new float[originalObjects.Length];
		for (int i = 0; i < transform.childCount; i++) 
		{
			originalObjects [i] = transform.GetChild (i);
		}

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
		if (IsStatic) {
			ColorObjects ();
		}

	}

	void FixedUpdate() 
	{
		if(!IsStatic){
			if (AnimateThroughMaps == true && AnimatableMaps.Length != 0) {
				ColorMap = AnimatableMaps [startval];
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
			ColorObjects ();
	}
}

	void ColorObjects()
	{

		switch (DeformAxis) 
		{
		case Axis.X:
			ColorX ();
			break;
		case Axis.Y:
			ColorY ();
			break;
		}
	}

	void ColorX()
	{
		for (int i = 0; i < originalObjects.Length; i++) 
		{
			float normalizedX = (OriginalX[i] - smallestX*StretchX) / (largestX*StretchXB - smallestX*StretchX);
			float normalizedZ = (OriginalZ [i] - smallestZ*StretchZ) / (largestZ*StretchZB - smallestZ*StretchZ);

			int x = Mathf.FloorToInt(normalizedX * ColorMap.width);
			int z = Mathf.FloorToInt(normalizedZ * ColorMap.height);

			float red = ColorMap.GetPixel (x, z).r;
			float green = ColorMap.GetPixel (x, z).g;
			float blue = ColorMap.GetPixel (x, z).b;

			Color color = new Color (red, green, blue, 1.0f);
			originalObjects [i].GetComponent<Renderer> ().material.color = color;
		}
	}

	void ColorY()
	{
		for (int i = 0; i < originalObjects.Length; i++) 
		{
			float normalizedY = (OriginalY [i] - smallestY*StretchX) / (largestY*StretchXB - smallestY*StretchX);
			float normalizedZ = (OriginalZ [i]- smallestZ*StretchZ) / (largestZ*StretchZB - smallestZ*StretchZ);

			int y = Mathf.FloorToInt(normalizedY * ColorMap.width);
			int z = Mathf.FloorToInt(normalizedZ * ColorMap.height);

			float red = ColorMap.GetPixel (y, z).r;
			float green = ColorMap.GetPixel (y, z).g;
			float blue = ColorMap.GetPixel (y, z).b;

			Color color = new Color (red, green, blue, 1.0f);
			originalObjects [i].GetComponent<Renderer> ().material.color = color;
		}
	}


}
