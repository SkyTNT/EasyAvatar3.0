using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyGesture))]
    public class EasyGestureEditor : Editor
    {
        GameObject avatar;
        SerializedProperty behaviors1, behaviors2, gestureType, handType, autoRestore, autoTrackingControl, offTrackingControl, onTrackingControl;
        EasyBehaviorGroupEditor editor1, editor2;

        int[] handTypeIndex = { 0, 1, 2 };
        int[] gestureTypeIndex = { 0, 1, 2, 3, 4, 5, 6, 7 };

        private void OnEnable()
        {
            serializedObject.Update();
            behaviors1 = serializedObject.FindProperty("behaviors1");
            behaviors2 = serializedObject.FindProperty("behaviors2");
            gestureType = serializedObject.FindProperty("gestureType");
            handType = serializedObject.FindProperty("handType");
            autoRestore = serializedObject.FindProperty("autoRestore");
            autoTrackingControl = serializedObject.FindProperty("autoTrackingControl");
            offTrackingControl = serializedObject.FindProperty("offTrackingControl");
            onTrackingControl = serializedObject.FindProperty("onTrackingControl");
            editor1 = new EasyBehaviorGroupEditor(behaviors1);
            editor2 = new EasyBehaviorGroupEditor(behaviors2);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            avatar = Utility.GetAvatar(((EasyGesture)target).gameObject);
            handType.enumValueIndex = EditorGUILayout.IntPopup(Lang.HandType, handType.enumValueIndex, new string[] { Lang.LeftHand, Lang.RightHand, Lang.AnyHand }, handTypeIndex);
            gestureType.enumValueIndex = EditorGUILayout.IntPopup(Lang.GestureType, gestureType.enumValueIndex, new string[] { Lang.GestureNeutral, Lang.GestureFist, Lang.GestureHandOpen, Lang.GestureFingerPoint, Lang.GestureVictory, Lang.GestureRockNRoll, Lang.GestureHandGun, Lang.GestureThumbsUp }, gestureTypeIndex);
            autoRestore.boolValue = EditorGUILayout.ToggleLeft(Lang.AutoRestore, autoRestore.boolValue);
            autoTrackingControl.boolValue = EditorGUILayout.ToggleLeft(Lang.autoTrackingControl, autoTrackingControl.boolValue);
            if (!autoTrackingControl.boolValue)
            {
                EditorGUILayout.LabelField(Lang.OnGestureOut, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(offTrackingControl);
                EditorGUILayout.LabelField(Lang.OnGesture, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(onTrackingControl);
            }
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField(Lang.OnGestureOut, EditorStyles.boldLabel);
            editor1.DoLayout(avatar);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField(Lang.OnGesture, EditorStyles.boldLabel);
            editor2.DoLayout(avatar);
            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
