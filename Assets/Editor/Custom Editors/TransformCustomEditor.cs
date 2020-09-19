using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformCustomEditor : Editor
{
    Transform _transform;
    bool didPositionChange;
    bool didRotationChange;
    bool didScaleChange;
    Vector3 initialLocalPosition;
    Vector3 initialLocalEuler;
    Vector3 initialLocalScale;
    Vector3 localPosition;
    Vector3 localEulerAngles;
    Vector3 localScale;

    private void OnEnable()
    {
        _transform = target as Transform;
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = 85;
        EditorGUIUtility.fieldWidth = 165;

        didPositionChange = false;
        didRotationChange = false;
        didScaleChange = false;

        initialLocalPosition = _transform.localPosition;
        initialLocalEuler = _transform.localEulerAngles;
        initialLocalScale = _transform.localScale;

        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                localPosition = EditorGUILayout.Vector3Field("Local Position", _transform.localPosition);
                if (EditorGUI.EndChangeCheck())
                    didPositionChange = true;

                GUILayout.Space(20);

                EditorGUI.BeginChangeCheck();
                localEulerAngles = EditorGUILayout.Vector3Field("Local Rotation", _transform.localEulerAngles);
                if (EditorGUI.EndChangeCheck())
                    didRotationChange = true;

                GUILayout.Space(20);

                EditorGUI.BeginChangeCheck();
                localScale = EditorGUILayout.Vector3Field("Local Scale", _transform.localScale);
                if (EditorGUI.EndChangeCheck())
                    didScaleChange = true;

                if (didPositionChange || didRotationChange || didScaleChange)
                {
                    Undo.RecordObject(_transform, _transform.name);

                    if (didPositionChange)
                        _transform.localPosition = localPosition;

                    if (didRotationChange)
                        _transform.localEulerAngles = localEulerAngles;

                    if (didScaleChange)
                        _transform.localScale = localScale;

                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUI.enabled = false;
                EditorGUILayout.Vector3Field("World Position", _transform.position);
                GUILayout.Space(20);
                EditorGUILayout.Vector3Field("World Rotation", _transform.eulerAngles);
                GUILayout.Space(20);
                EditorGUILayout.Vector3Field("World Scale", _transform.lossyScale);
                GUILayout.FlexibleSpace();
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        Transform[] selectedTransforms = Selection.transforms;
        if (selectedTransforms.Length > 1)
        {
            foreach (var item in selectedTransforms)
            {
                if (didPositionChange || didRotationChange || didScaleChange)
                    Undo.RecordObject(item, item.name);

                if (didPositionChange)
                {
                    item.localPosition = ApplyChangesOnly(
                        item.localPosition, initialLocalPosition, _transform.localPosition);
                }

                if (didRotationChange)
                {
                    item.localEulerAngles = ApplyChangesOnly(
                        item.localEulerAngles, initialLocalEuler, _transform.localEulerAngles);
                }

                if (didScaleChange)
                {
                    item.localScale = ApplyChangesOnly(
                        item.localScale, initialLocalScale, _transform.localScale);
                }

            }
        }
    }

    private Vector3 ApplyChangesOnly(Vector3 toApply, Vector3 initial, Vector3 changed)
    {
        if (!Mathf.Approximately(initial.x, changed.x))
            toApply.x = _transform.localPosition.x;

        if (!Mathf.Approximately(initial.y, changed.y))
            toApply.y = _transform.localPosition.y;

        if (!Mathf.Approximately(initial.z, changed.z))
            toApply.z = _transform.localPosition.z;

        return toApply;
    }
}