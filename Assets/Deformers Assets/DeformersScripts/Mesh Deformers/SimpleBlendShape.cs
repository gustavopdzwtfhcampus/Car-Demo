using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBlendShape : MonoBehaviour {

	[Tooltip("calculte every frame or just at start")]
	public bool isStatic = false;
	[Tooltip("the shape you want this mesh to transform into (must be a variation of the original mesh)")]
	public GameObject targetMeshObject;
	[Tooltip("how much do u want to blent (0 no blend, 1 full blend, < 0 under blend, > 1 over blend")]
	[Range(-1.0f,2.0f)]
	public float Weight; 

	Mesh deformingMesh;
	Mesh targetsMesh;
	float new_x,new_y,new_z,newnorx,newnory,newnorz;
	float distx = 0, disty = 0, distz = 0, norx = 0,nory = 0,norz = 0;
	Vector3[] originalVertices, displacedVertices,originalnormals,diplacednormals,targetVetricies,targetNormals;
	Vector3 newvertPos=Vector3.zero;
	Vector3 newnorms=Vector3.zero;

	void Start () 
	{
		deformingMesh = GetComponent<MeshFilter>().mesh;
		originalVertices = deformingMesh.vertices;
		originalnormals = deformingMesh.normals;
		displacedVertices = new Vector3[originalVertices.Length];
		diplacednormals = new Vector3[originalVertices.Length];
		targetsMesh = targetMeshObject.GetComponent<MeshFilter> ().mesh;
		targetVetricies = targetsMesh.vertices;
		targetNormals = targetsMesh.normals;
		for (int i = 0; i < originalVertices.Length; i++)
		{
			displacedVertices[i] = originalVertices[i];
		}
		if(isStatic)
		{
			blendshapes();
		}
	}

	void Update ()
	{
		if(!isStatic )
		{
			blendshapes();
		}
	}

	void blendshapes()
	{
		for (int i = 0; i < originalVertices.Length; i++) 
		{
			distx = (targetVetricies[i].x - originalVertices [i].x)*Weight;
			disty = (targetVetricies[i].y - originalVertices [i].y)*Weight;
			distz = (targetVetricies[i].z - originalVertices [i].z)*Weight;
			norx = targetNormals [i].x * Weight;
			nory = targetNormals [i].y * Weight;
			norz= targetNormals [i].z * Weight;
			new_x = originalVertices [i].x + distx;
			new_y = originalVertices [i].y + disty;
			new_z = originalVertices [i].z + distz;
			newvertPos.Set(new_x, new_y, new_z);
			newnorms.Set(norx, nory, norz);
			displacedVertices [i] = newvertPos;
			diplacednormals [i] = newnorms + originalnormals[i]*(1-Weight); 
		}
		deformingMesh.vertices = displacedVertices;
		deformingMesh.normals = diplacednormals;
	}
}
