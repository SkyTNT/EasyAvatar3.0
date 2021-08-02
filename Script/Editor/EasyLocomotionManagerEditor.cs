using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyLocomotionManager))]
    public class EasyLocomotionManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SerializedProperty useController = serializedObject.FindProperty("useAnimatorController");
            EditorGUILayout.PropertyField(useController, new GUIContent(Lang.UseController));
            if (useController.boolValue)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("controller"), new GUIContent(Lang.AnimatorController));
                if (GUILayout.Button(Lang.Default))
                {
                    serializedObject.FindProperty("controller").objectReferenceValue = VRCAssets.locomotionController;
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.HelpBox(Lang.LocomotionControllerNote, MessageType.Info);
            serializedObject.ApplyModifiedProperties();
        }
    }

}

