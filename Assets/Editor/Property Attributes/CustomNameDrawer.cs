using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CustomNameAttribute))]
public class CustomNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, new GUIContent((attribute as CustomNameAttribute).NewName));
    }
}