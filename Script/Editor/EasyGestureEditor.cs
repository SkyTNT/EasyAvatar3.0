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
        SerializedProperty behaviors, animations, gestureType, handType, useAnimClip, autoRestore;
        EasyBehaviorAndAnimEditor behaviorAndAnimEditor;

        int[] handTypeIndex = { 0, 1, 2 };
        int[] gestureTypeIndex = { 0, 1, 2, 3, 4, 5, 6, 7 };
        string[] handTypeLabels;
        string[] gestureTypeLabels;

        private void OnEnable()
        {
            serializedObject.Update();
            behaviors = serializedObject.FindProperty("behaviors");
            animations = serializedObject.FindProperty("animations");
            gestureType = serializedObject.FindProperty("gestureType");
            handType = serializedObject.FindProperty("handType");
            useAnimClip = serializedObject.FindProperty("useAnimClip");
            autoRestore = serializedObject.FindProperty("autoRestore");
            behaviorAndAnimEditor = new EasyBehaviorAndAnimEditor(behaviors, animations);
            handTypeLabels = new string[] { Lang.LeftHand, Lang.RightHand, Lang.AnyHand };
            gestureTypeLabels = new string[] {Lang.GestureNeutral, Lang.GestureFist, Lang.GestureHandOpen, Lang.GestureFingerPoint, Lang.GestureVictory, Lang.GestureRockNRoll, Lang.GestureHandGun, Lang.GestureThumbsUp };
            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            avatar = behaviorAndAnimEditor.avatar = Utility.GetAvatar(((EasyGesture)target).gameObject);
            handType.enumValueIndex = EditorGUILayout.IntPopup(Lang.HandType, handType.enumValueIndex, handTypeLabels, handTypeIndex);
            gestureType.enumValueIndex = EditorGUILayout.IntPopup(Lang.GestureType, gestureType.enumValueIndex, gestureTypeLabels, gestureTypeIndex);
            behaviorAndAnimEditor.useAnimClip = useAnimClip.boolValue = EditorGUILayout.ToggleLeft(Lang.UseAnimClip, useAnimClip.boolValue);
            autoRestore.boolValue = EditorGUILayout.ToggleLeft(Lang.AutoRestore, autoRestore.boolValue);
            EditorGUILayout.LabelField(Lang.OnGesture,EditorStyles.boldLabel);
            behaviorAndAnimEditor.LayoutGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}

