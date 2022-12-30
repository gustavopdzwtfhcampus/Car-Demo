using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ///////////////////////////////////////PLEASE READ THIS///////////////////////////////////////////////////////
/// 
/// this script is not optimized and still beta, it is a super performance hog, so don't use it in your projects, 
/// it's here for educational purposes only, use the simpleblendshapes deformer for that purpose only
/// you can edit this script as you please to make it perform well for your project
/// 
/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>

public class BETABlendShapeDeformer : MonoBehaviour {
	public bool isStatic = false;
	public GameObject objectToDeform;
	public GameObject [] targetMeshObjects;
	[Range(0.0f,1.0f)]
	public float[] Weight; 
	Mesh deformingMesh;
	Mesh[] targetsMesh;
	float new_x,new_y,new_z,newnorx,newnory,newnorz;
	float distx = 0, disty = 0, distz = 0, norx = 0,nory = 0,norz = 0;
	float divider = 1;
	float averageWeights =0;
	Vector3[] originalVertices, displacedVertices,originalnormals,diplacednormals;
	bool execute = false;

	void Start () 
	{
		deformingMesh = objectToDeform.GetComponent<MeshFilter>().mesh;
		originalVertices = deformingMesh.vertices;
		originalnormals = deformingMesh.normals;
		displacedVertices = new Vector3[originalVertices.Length];
		diplacednormals = new Vector3[originalVertices.Length];
		targetsMesh = new Mesh[targetMeshObjects.Length];
	
		if (targetMeshObjects.Length != Weight.Length) 
		{
			Weight = new float[targetMeshObjects.Length];
		}
		for (int m =0; m<targetMeshObjects.Length;m++)
		{
			targetsMesh[m] = targetMeshObjects[m].GetComponent<MeshFilter>().mesh;
		}

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
		if(!isStatic && execute == false)
		{
			execute = true;
			StartCoroutine(orchestrate());
		}
	}

	IEnumerator orchestrate()
	{
		yield return new WaitForSeconds (0.02f);
		blendshapes();
	}
	void blendshapes()
	{
		for (int i = 0; i < originalVertices.Length; i++) 
		{
			for (int m = 0; m < targetMeshObjects.Length; m++) 
			{
				distx += (targetsMesh[m].vertices[i].x - originalVertices [i].x)*Weight[m];
				disty += (targetsMesh[m].vertices[i].y - originalVertices [i].y)*Weight[m];
				distz += (targetsMesh[m].vertices[i].z - originalVertices [i].z)*Weight[m];
				norx += targetsMesh [m].normals [i].x * Weight [m];
				nory += targetsMesh [m].normals [i].y * Weight [m];
				norz += targetsMesh [m].normals [i].z * Weight [m];
				if (Weight [m] != 0) {
					divider++;
					averageWeights += Weight [m];
				}
			}

			new_x = originalVertices [i].x + distx;
			new_y = originalVertices [i].y + disty;
			new_z = originalVertices [i].z + distz;
			Vector3 newvertPos = new Vector3 (new_x, new_y, new_z);
			Vector3 newnorms = new Vector3(norx, nory, norz);

			averageWeights = averageWeights / divider;
			displacedVertices [i] = newvertPos;
			diplacednormals [i] = newnorms + originalnormals[i]*(1-averageWeights); 
			distx = 0;
			disty = 0;
			distz = 0;
			norx = 0;
			nory = 0;
			norz = 0;
			divider = 1;
		}
		deformingMesh.vertices = displacedVertices;
		deformingMesh.normals = diplacednormals;
		execute = false;
	}
}

