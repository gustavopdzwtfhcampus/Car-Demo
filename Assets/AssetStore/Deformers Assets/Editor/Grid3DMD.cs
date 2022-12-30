using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Grid3DMD : EditorWindow {

	int Columns = 10;
	int Rows = 10;
	int stacks = 10;

	float SeperationX=0.0f;
	float SeperationZ=0.0f;
	float SeperationY=1.0f;

	public Object theNewObject;

	float tempx = 0;
	float tempz = 0;
	float tempy = 0;

	public enum GridItemsType 
	{ 
		Cube = 0, 
		Sphere = 1, 
		Capsule = 2,
		Cylinder = 3,
		Plane=4,
		Quad=5,
		Custom=6
	}

	public GridItemsType chosenPrimitive;
	[MenuItem ("Mesh Deformer/Create 3D Grid")]
	static void Init () 
	{
		Grid3DMD window = (Grid3DMD)EditorWindow.GetWindow (typeof (Grid3DMD));
		window.Show();
	}

	void OnGUI () {
		GUILayout.Label ("Grid Options", EditorStyles.boldLabel);
		Columns = EditorGUILayout.IntField("Number Of Columns", Columns);
		Rows = EditorGUILayout.IntField("Number Of Rows", Rows);
		stacks = EditorGUILayout.IntField("Number Of Stacks", stacks);
		SeperationX = EditorGUILayout.FloatField ("Seperation in X", SeperationX);
		SeperationY = EditorGUILayout.FloatField ("Seperation in Y", SeperationY);
		SeperationZ = EditorGUILayout.FloatField ("Seperation in Z", SeperationZ);
		chosenPrimitive =(GridItemsType)EditorGUILayout.EnumPopup ("Grid Item", chosenPrimitive);
		if (chosenPrimitive == GridItemsType.Custom)
		{
			theNewObject = EditorGUILayout.ObjectField ("Custom Prefab", theNewObject, typeof(Object), true);
		}
		if (GUILayout.Button ("Create Grid"))
		{
			createTheGridItems (chosenPrimitive);
		}
	}

	void OnInspectorUpdate() {
		Repaint();
	}

	void createTheGridItems(GridItemsType theType )
	{
		GameObject GridHandle = new GameObject ("3D Grid Handler");
		GridHandle.transform.position = Vector3.zero;
		GridHandle.transform.rotation = Quaternion.identity;
		switch (theType) {
		case GridItemsType.Cube:
			for (int k = 0; k < stacks; k++) {
				for (int i = 0; i < Columns; i++) {
					for (int j = 0; j < Rows; j++) {
						GameObject OBJ = GameObject.CreatePrimitive (PrimitiveType.Cube);
						OBJ.transform.position = new Vector3 (i + tempx, tempy, j + tempz);
						OBJ.transform.SetParent (GridHandle.transform);
						tempz += SeperationZ;
					}
					tempx += SeperationX;
					tempz = 0;
				}
				tempy += SeperationY;
				tempx = 0;
			}
			break;
		case GridItemsType.Sphere:
			for (int k = 0; k < stacks; k++) {
				for (int i = 0; i < Columns; i++) {
					for (int j = 0; j < Rows; j++) {
						GameObject OBJ = GameObject.CreatePrimitive (PrimitiveType.Sphere);
						OBJ.transform.position = new Vector3 (i + tempx, tempy, j + tempz);
						OBJ.transform.SetParent (GridHandle.transform);
						tempz += SeperationZ;
					}
					tempx += SeperationX;
					tempz = 0;
				}
				tempy += SeperationY;
				tempx = 0;
			}
			break;
		case GridItemsType.Capsule:
			for (int k = 0; k < stacks; k++) {
				for (int i = 0; i < Columns; i++) {
					for (int j = 0; j < Rows; j++) {
						GameObject OBJ = GameObject.CreatePrimitive (PrimitiveType.Capsule);
						OBJ.transform.position = new Vector3 (i + tempx, tempy, j + tempz);
						OBJ.transform.SetParent (GridHandle.transform);
						tempz += SeperationZ;
					}
					tempx += SeperationX;
					tempz = 0;
				}
				tempy += SeperationY;
				tempx = 0;
			}
			break;
		case GridItemsType.Cylinder:
			for (int k = 0; k < stacks; k++) {
				for (int i = 0; i < Columns; i++) {
					for (int j = 0; j < Rows; j++) {
						GameObject OBJ = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
						OBJ.transform.position = new Vector3 (i + tempx, tempy, j + tempz);
						OBJ.transform.SetParent (GridHandle.transform);
						tempz += SeperationZ;
					}
					tempx += SeperationX;
					tempz = 0;
				}
				tempy += SeperationY;
				tempx = 0;
			}
			break;
		case GridItemsType.Plane:
			for (int k = 0; k < stacks; k++) {
				for (int i = 0; i < Columns; i++) {
					for (int j = 0; j < Rows; j++) {
						GameObject OBJ = GameObject.CreatePrimitive (PrimitiveType.Plane);
						OBJ.transform.position = new Vector3 (i + tempx, tempy, j + tempz);
						OBJ.transform.SetParent (GridHandle.transform);
						tempz += SeperationZ;
					}
					tempx += SeperationX;
					tempz = 0;
				}
				tempy += SeperationY;
				tempx = 0;
			}
			break;
		case GridItemsType.Quad:
			for (int k = 0; k < stacks; k++) {
				for (int i = 0; i < Columns; i++) {
					for (int j = 0; j < Rows; j++) {
						GameObject Quad = GameObject.CreatePrimitive (PrimitiveType.Quad);
						Quad.transform.position = new Vector3 (i + tempx, tempy, j + tempz);
						Quad.transform.eulerAngles = new Vector3 (90.0f, 0, 0);
						Quad.transform.SetParent (GridHandle.transform);
						tempz += SeperationZ;
					}
					tempx += SeperationX;
					tempz = 0;
				}
				tempy += SeperationY;
				tempx = 0;
			}
			break;
		case GridItemsType.Custom:
			if (theNewObject != null) {
				for (int k = 0; k < stacks; k++) {
					for (int i = 0; i < Columns; i++) {
						for (int j = 0; j < Rows; j++) {
							GameObject OBJ = Instantiate (theNewObject, new Vector3 (i + tempx, tempy, j + tempz), Quaternion.identity) as GameObject;
							OBJ.transform.SetParent (GridHandle.transform);
							tempz += SeperationZ;
						}
						tempx += SeperationX;
						tempz = 0;
					}
					tempy += SeperationY;
					tempx = 0;
				}
			} else {
				Debug.LogError ("Kindly Assign a Prefab to the custom Prefab field");
			}

			break;
		}
		tempx = 0;
		tempz = 0;
		tempy = 0;
	}
}