using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class ImageDeformer : MonoBehaviour {
	[Tooltip("The image used for the deformation. Must be set to Advanced, and Read/Write Enabled must be true in the texture import settings")]
	public Texture2D heightmap;
	[Tooltip("Height Multiplier of the deformer")]
	public float Height = 1.0f;
	[Tooltip("calculate the deformation once at Start or keep it dynamic")]
	public bool isStatic = false;
	[Tooltip("offset the image texture (kind of like repeat UV")]
	public float stretchX = 1.0f, stretchZ = 1.0f;
	[Tooltip("An Array of textures that the deformer cas use as an animation. Must be set to Advanced, and Read/Write Enabled must be true in the texture import settings")]
	public Texture2D[] AnimatableMaps;
	[Tooltip("enable the Animation through the array of images")]
	public bool AnimateThroughMaps = false;
	[Tooltip("Loop Through the animation")]
	public bool  AnimLoop = false;
	[Tooltip("time to change the image")]
	public float AnimFrameChangeInSec = 1.0f;
	[Tooltip("Allow unity to re-calculate the normals, sometimes its needed, others no, must be set before hitting play")]
	public bool RecalculateNormals = false;
	[Tooltip("Enable/Disable the use of the effector")]
	public bool UseEffector = false;
	[Tooltip("The effector Object (must have the effector script attached to it)")]
	public GameObject Effector;

	private EffectorVal theEffector;
	private float EffectorDistance = 3.0f;
	private bool InvertedEffector = false;
	private AnimationCurve Refinecurve;
	private float normalizedCurve,curveValue;
	private float tempAnimVal = 1.0f;
	private float startTime = 0.0f;
	private int startval = 0;
	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices;
	Vector3 []normalVerts;
	Vector2[] uvs;

	void Start () 
	{
		if (UseEffector != false) {
			if (Effector != null) {
				theEffector = Effector.GetComponent<EffectorVal> ();
			} else {
				Debug.LogWarning ("Please assign an effector to the effector Value, to create an effector go to: Mesh Deformer -> createEffector");
			}
		}
		tempAnimVal = AnimFrameChangeInSec;
		deformingMesh = GetComponent<MeshFilter>().mesh;
		uvs = deformingMesh.uv;
		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
		normalVerts = new Vector3[originalVertices.Length];

		for (int i = 0; i < originalVertices.Length; i++)
		{
			normalVerts [i] = Vector3.Normalize( deformingMesh.normals[i]);
		}

		if (isStatic)
		{
			if (!UseEffector) {
				MoveVerts ();
			} else {
				MoveVertsEffector ();
			}
		}
	}

	void FixedUpdate() 
	{
		if (!isStatic) {
			if (AnimateThroughMaps == true && AnimatableMaps.Length != 0) {
				heightmap = AnimatableMaps [startval];
				startTime += Time.deltaTime;
				if(startTime >= tempAnimVal)
				{
					startval++;
					if (startval >= AnimatableMaps.Length && AnimLoop == true) {
						startval = 0;
					} else if(startval >= AnimatableMaps.Length && AnimLoop == false)
					{
						startval = AnimatableMaps.Length - 1;
					}
					tempAnimVal += AnimFrameChangeInSec;
				}
			}
			if (!UseEffector) {
				MoveVerts ();
			} else {
				MoveVertsEffector ();
			}
		}
	}

	void MoveVerts()
	{
		for (int i = 0; i < originalVertices.Length; i++) {
			int u = Mathf.FloorToInt (uvs [i].x * heightmap.width * stretchX);
			int v = Mathf.FloorToInt (uvs [i].y * heightmap.height * stretchZ);

			float multiplier = heightmap.GetPixel (u, v).grayscale * Height;

			float newx = originalVertices [i].x + normalVerts [i].x * multiplier;
			float newy = originalVertices [i].y + normalVerts [i].y * multiplier;
			float newz = originalVertices [i].z + normalVerts [i].z * multiplier;

			Vector3 pos = new Vector3 (newx, newy, newz);
			displacedVertices [i] = pos;
		}
		deformingMesh.vertices = displacedVertices;
		if (RecalculateNormals) {
			deformingMesh.RecalculateNormals ();
		}
	}

	void MoveVertsEffector ()
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
					int u = Mathf.FloorToInt (uvs [i].x * heightmap.width * stretchX);
					int v = Mathf.FloorToInt (uvs [i].y * heightmap.height * stretchZ);

					float multiplier = heightmap.GetPixel (u, v).grayscale * Height;

					float newx = originalVertices [i].x + normalVerts [i].x * multiplier * curveValue;
					float newy = originalVertices [i].y + normalVerts [i].y * multiplier * curveValue;
					float newz = originalVertices [i].z + normalVerts [i].z * multiplier * curveValue;

					Vector3 pos = new Vector3 (newx, newy, newz);
					displacedVertices [i] = pos;
				} else {
					displacedVertices [i] = originalVertices [i];
				}
			}
			deformingMesh.vertices = displacedVertices;
			if (RecalculateNormals) {
				deformingMesh.RecalculateNormals ();
			}
		} else 
		{
			for (int i = 0; i < originalVertices.Length; i++) {
				if (Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position) >= EffectorDistance) { 
					float dist = Vector3.Distance (transform.TransformPoint (originalVertices [i]), Effector.transform.position);
					normalizedCurve = (dist - EffectorDistance) / EffectorDistance;
					curveValue = Refinecurve.Evaluate (normalizedCurve);
					int u = Mathf.FloorToInt (uvs [i].x * heightmap.width * stretchX);
					int v = Mathf.FloorToInt (uvs [i].y * heightmap.height * stretchZ);

					float multiplier = heightmap.GetPixel (u, v).grayscale * Height;

					float newx = originalVertices [i].x + normalVerts [i].x * multiplier * curveValue;
					float newy = originalVertices [i].y + normalVerts [i].y * multiplier * curveValue;
					float newz = originalVertices [i].z + normalVerts [i].z * multiplier * curveValue;

					Vector3 pos = new Vector3 (newx, newy, newz);
					displacedVertices [i] = pos;
				} else {
					displacedVertices [i] = originalVertices [i];
				}
			}
			deformingMesh.vertices = displacedVertices;
			if (RecalculateNormals) {
				deformingMesh.RecalculateNormals ();
			}
		}
	}
}
