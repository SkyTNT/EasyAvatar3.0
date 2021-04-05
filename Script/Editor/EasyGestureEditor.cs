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
        SerializedProperty behaviors1, animations1, behaviors2, animations2, gestureType, handType, useAnimClip, autoRestore;
        EasyBehaviorAndAnimEditor behaviorAndAnimEditor1, behaviorAndAnimEditor2;

        int[] handTypeIndex = { 0, 1, 2 };
        int[] gestureTypeIndex = { 0, 1, 2, 3, 4, 5, 6, 7 };
        string[] handTypeLabels;
        string[] gestureTypeLabels;

        private void OnEnable()
        {
            serializedObject.Update();
            behaviors1 = serializedObject.FindProperty("behaviors1");
            animations1 = serializedObject.FindProperty("animations1");
            behaviors2 = serializedObject.FindProperty("behaviors2");
            animations2 = serializedObject.FindProperty("animations2");
            gestureType = serializedObject.FindProperty("gestureType");
            handType = serializedObject.FindProperty("handType");
            useAnimClip = serializedObject.FindProperty("useAnimClip");
            autoRestore = serializedObject.FindProperty("autoRestore");
            behaviorAndAnimEditor1 = new EasyBehaviorAndAnimEditor(behaviors1, animations1);
            behaviorAndAnimEditor2 = new EasyBehaviorAndAnimEditor(behaviors2, animations2);
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
            avatar = behaviorAndAnimEditor1.avatar = behaviorAndAnimEditor2.avatar = Utility.GetAvatar(((EasyGesture)target).gameObject);
            handType.enumValueIndex = EditorGUILayout.IntPopup(Lang.HandType, handType.enumValueIndex, handTypeLabels, handTypeIndex);
            gestureType.enumValueIndex = EditorGUILayout.IntPopup(Lang.GestureType, gestureType.enumValueIndex, gestureTypeLabels, gestureTypeIndex);
            behaviorAndAnimEditor1.useAnimClip = behaviorAndAnimEditor2.useAnimClip = useAnimClip.boolValue = EditorGUILayout.ToggleLeft(Lang.UseAnimClip, useAnimClip.boolValue);
            autoRestore.boolValue = EditorGUILayout.ToggleLeft(Lang.AutoRestore, autoRestore.boolValue);
            EditorGUILayout.LabelField(Lang.OnGesture,EditorStyles.boldLabel);
            behaviorAndAnimEditor1.LayoutGUI();
            EditorGUILayout.LabelField(Lang.OnGestureOut, EditorStyles.boldLabel);
            behaviorAndAnimEditor2.LayoutGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}

