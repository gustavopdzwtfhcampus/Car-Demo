using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LinkComponents : MonoBehaviour {

	[MenuItem ("Mesh Deformer/Create Effector")]
	static void AddEffector () {
		GameObject OBJ = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		DestroyImmediate (OBJ.GetComponent<SphereCollider>());
		OBJ.AddComponent<EffectorVal> ();
		OBJ.name ="Effector";
	}

	[MenuItem ("Mesh Deformer/Mesh Deformer/Image Deformer")]
	static void AddImageDef () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <ImageDeformer> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one mesh Object");
		}
	}

	[MenuItem ("Mesh Deformer/Mesh Deformer/Ripple Deformer")]
	static void AddRipple () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <RippleDeformer> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one mesh Object");
		}
	}

	[MenuItem ("Mesh Deformer/Mesh Deformer/Simple Blend Shape Deformer")]
	static void AddBlendShape () {
		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <SimpleBlendShape> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one mesh Object");
		}
	}

	[MenuItem ("Mesh Deformer/Mesh Deformer/Twist Deformer")]
	static void AddTwist () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <TwistDeformer> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one mesh Object");
		}
	}
	[MenuItem ("Mesh Deformer/Mesh Deformer/Flare Deformer")]
	static void AddFlare () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <FlareDeformer> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one mesh Object");
		}
	}

	[MenuItem ("Mesh Deformer/Mesh Deformer/Sine Deformer")]
	static void AddSine () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <SineDeformer> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one mesh Object");
		}
	}

	[MenuItem ("Mesh Deformer/Mesh Deformer/Curve Shape Deformer")]
	static void AddCurve () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <CurveShapeDeformer> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one mesh Object");
		}
	}

	[MenuItem ("Mesh Deformer/Objects Deformer/Object Image Deformer")]
	static void AddObjImage () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <ObjectsImageDeformer> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one parent Object");
		}
	}

	[MenuItem ("Mesh Deformer/Objects Deformer/Object Sine Deformer")]
	static void AddObjSine () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <ObjectsSine> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one parent Object");
		}
	}

	[MenuItem ("Mesh Deformer/Objects Deformer/Object Ripple Deformer")]
	static void AddObjRipple () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <ObjectsRipple> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one parent Object");
		}
	}

	[MenuItem ("Mesh Deformer/Objects Deformer/Object Translate Deformer")]
	static void AddObjTDeformer () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <ObjectTransformEffectorDeformer> ();
				GameObject OBJ = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				DestroyImmediate (OBJ.GetComponent<SphereCollider>());
				OBJ.AddComponent<EffectorVal> ();
				OBJ.name ="Translation Effector";
				Selection.gameObjects [i].GetComponent<ObjectTransformEffectorDeformer> ().Effector = OBJ;
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one parent Object");
		}
	}

	[MenuItem ("Mesh Deformer/Objects Deformer/Object Rotate Deformer")]
	static void AddObjRDeformer () {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <ObjectRotateEffectorDeformer> ();
				GameObject OBJ = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				DestroyImmediate (OBJ.GetComponent<SphereCollider>());
				OBJ.AddComponent<EffectorVal> ();
				OBJ.name ="Rotation Effector";
				Selection.gameObjects [i].GetComponent<ObjectRotateEffectorDeformer> ().Effector = OBJ;
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one parent Object");
		}
	}

	[MenuItem ("Mesh Deformer/Objects Deformer/Object Scale Deformer")]
	static void AddObjSDeformer () {
		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <ObjectScaleEffectorDeformer> ();
				GameObject OBJ = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				DestroyImmediate (OBJ.GetComponent<SphereCollider>());
				OBJ.AddComponent<EffectorVal> ();
				OBJ.name ="Scale Effector";
				Selection.gameObjects [i].GetComponent<ObjectScaleEffectorDeformer> ().Effector = OBJ;
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one parent Object");
		}
	}

	[MenuItem ("Mesh Deformer/Help")]
	static void OpenMyWeb () {
		Application.OpenURL("https://wixarexperience.com/mesh-and-object-deformer-support/");
	}

	[MenuItem ("Mesh Deformer/Skinned Mesh Deformer/Skinned Mesh Ripple")]
	static void AddSkinnedRipple() {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <skinnedMeshRipple> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one Skinned mesh Object");
		}
	}

	[MenuItem ("Mesh Deformer/Skinned Mesh Deformer/Skinned Mesh Flare")]
	static void AddSkinnedFlare() {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <SkinnedMeshFlareDeformer> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one Skinned mesh Object");
		}
	}

	[MenuItem ("Mesh Deformer/Skinned Mesh Deformer/Skinned Mesh Sine")]
	static void AddSkinnedSine() {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <SkinnedMeshSine> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one Skinned mesh Object");
		}
	}

	[MenuItem ("Mesh Deformer/Skinned Mesh Deformer/Skinned Mesh Anim Curve Shape")]
	static void AddSkinnedAnimCurve() {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <SkinnedMeshAnimCurveDeformer> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one Skinned mesh Object");
		}
	}

	[MenuItem ("Mesh Deformer/Colorize/Colorize Grid")]
	static void AddColorize() {

		if (Selection.activeGameObject != null) {
			for (int i = 0; i < Selection.gameObjects.Length; i++) {
				Selection.gameObjects [i].AddComponent <ColorizeGrid> ();
			}
		} else 
		{
			Debug.LogWarning ("You Must Select at least one parent Object");
		}
	}
	[MenuItem ("Mesh Deformer/Save Mesh")]
	static void SaveMeshtoAsset() {

		if (Selection.activeGameObject != null) {
			string saveName = Selection.gameObjects [0].name + "Saved";
			Transform selectedGameObject = Selection.gameObjects [0].transform;
			var SavedMesh = selectedGameObject.GetComponent<MeshFilter>();
			if (SavedMesh) {
				var savePath = "Assets/" + saveName + ".asset";
				Debug.Log ("Saved Mesh to:" + savePath);
				AssetDatabase.CreateAsset (SavedMesh.mesh, savePath);
			} else {
				Debug.LogWarning ("your selected object must contain a meshfilter");
			}
		} else 
		{
			Debug.LogWarning ("You Must Select One Mesh");
		}
	}


}
