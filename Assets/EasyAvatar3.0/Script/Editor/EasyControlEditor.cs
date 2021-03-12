using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyControlEditor))]
    public class EasyControlEditor : Editor
    {
        SerializedProperty behaviors;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            behaviors = serializedObject.FindProperty("behaviors");
            EditorGUILayout.PropertyField(behaviors);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

