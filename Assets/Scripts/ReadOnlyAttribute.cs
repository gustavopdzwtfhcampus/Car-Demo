using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Can be put before a variable so that it cannot be modified in the inspector
//Useful in development only for cases where you want to view the variables information in the inspector
//for debugging purposes, but dont want to give anybody the option to modify the information
public class ReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
