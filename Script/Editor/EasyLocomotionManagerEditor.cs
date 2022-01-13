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
            SerializedProperty defaultLocomotionGroup = serializedObject.FindProperty("defaultLocomotionGroup");
            EditorGUILayout.PropertyField(useController, new GUIContent(Lang.UseController));
            if (useController.boolValue)
            {
                GUILayout.BeginVertical();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("controller"), new GUIContent(Lang.AnimatorController));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("afk"), new GUIContent(Lang.AFK));
                if (GUILayout.Button(Lang.Default))
                {
                    serializedObject.FindProperty("controller").objectReferenceValue = VRCAssets.locomotionController;
                    serializedObject.FindProperty("afk").objectReferenceValue = VRCAssets.proxy_afk;
                }
                GUILayout.EndVertical();
                EditorGUILayout.HelpBox(Lang.LocomotionControllerNote, MessageType.Info);

            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(defaultLocomotionGroup, new GUIContent(Lang.DefaultLocomotionGroup));
                if (EditorGUI.EndChangeCheck())
                {
                    EasyLocomotionManager locomotionManager = (EasyLocomotionManager)target;
                    EasyLocomotionGroup locomotion = (EasyLocomotionGroup)defaultLocomotionGroup.objectReferenceValue;
                    if (locomotion && !locomotion.transform.IsChildOf(locomotionManager.transform))
                    {
                        defaultLocomotionGroup.objectReferenceValue = null;
                        EditorUtility.DisplayDialog("Error", Lang.ErrSetLocomotion, "OK");
                    }
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }

}

