using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyGestureManager))]
    public class EasyGestureManagerEditor : Editor
    {
        bool leftFold = true, rightFold = true;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.Label(Lang.GestureBaseAnimation,EditorStyles.boldLabel);
            leftFold = EditorGUILayout.Foldout(leftFold, Lang.LeftHand);
            if (leftFold)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftNeutral"), new GUIContent(Lang.GestureNeutral));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftFist"), new GUIContent(Lang.GestureFist));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftHandOpen"), new GUIContent(Lang.GestureHandOpen));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftFingerPoint"), new GUIContent(Lang.GestureFingerPoint));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftVictory"), new GUIContent(Lang.GestureVictory));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftRockNRoll"), new GUIContent(Lang.GestureRockNRoll));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftHandGun"), new GUIContent(Lang.GestureHandGun));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftThumbsUp"), new GUIContent(Lang.GestureThumbsUp));
                GUILayout.EndVertical();
            }

            rightFold = EditorGUILayout.Foldout(rightFold, Lang.RightHand);
            if (rightFold)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightNeutral"), new GUIContent(Lang.GestureNeutral));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightFist"), new GUIContent(Lang.GestureFist));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightHandOpen"), new GUIContent(Lang.GestureHandOpen));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightFingerPoint"), new GUIContent(Lang.GestureFingerPoint));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightVictory"), new GUIContent(Lang.GestureVictory));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightRockNRoll"), new GUIContent(Lang.GestureRockNRoll));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightHandGun"), new GUIContent(Lang.GestureHandGun));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightThumbsUp"), new GUIContent(Lang.GestureThumbsUp));
                GUILayout.EndVertical();
            }

            if (GUILayout.Button(Lang.DefaultValue))
            {
                SetDefaultGesture(serializedObject);
            }
            EditorGUILayout.HelpBox(Lang.GestureManagerHelpBox, MessageType.Warning);


            serializedObject.ApplyModifiedProperties();
        }

        public static void SetDefaultGesture(SerializedObject serializedObject)
        {
            serializedObject.Update();
            serializedObject.FindProperty("leftNeutral").objectReferenceValue = serializedObject.FindProperty("rightNeutral").objectReferenceValue = VRCAssets.proxy_hands_idle;
            serializedObject.FindProperty("leftFist").objectReferenceValue = serializedObject.FindProperty("rightFist").objectReferenceValue = VRCAssets.proxy_hands_fist;
            serializedObject.FindProperty("leftHandOpen").objectReferenceValue = serializedObject.FindProperty("rightHandOpen").objectReferenceValue = VRCAssets.proxy_hands_open;
            serializedObject.FindProperty("leftFingerPoint").objectReferenceValue = serializedObject.FindProperty("rightFingerPoint").objectReferenceValue = VRCAssets.proxy_hands_point;
            serializedObject.FindProperty("leftVictory").objectReferenceValue = serializedObject.FindProperty("rightVictory").objectReferenceValue = VRCAssets.proxy_hands_peace;
            serializedObject.FindProperty("leftRockNRoll").objectReferenceValue = serializedObject.FindProperty("rightRockNRoll").objectReferenceValue = VRCAssets.proxy_hands_rock;
            serializedObject.FindProperty("leftHandGun").objectReferenceValue = serializedObject.FindProperty("rightHandGun").objectReferenceValue = VRCAssets.proxy_hands_gun;
            serializedObject.FindProperty("leftThumbsUp").objectReferenceValue = serializedObject.FindProperty("rightThumbsUp").objectReferenceValue = VRCAssets.proxy_hands_thumbs_up;
            serializedObject.ApplyModifiedProperties();
        }
    }
}

