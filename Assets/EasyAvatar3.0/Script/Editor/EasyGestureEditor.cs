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
        SerializedProperty behaviors, animations, gestureType, handType, useAnimClip;
        EasyBehaviorAndAnimEditor behaviorAndAnimEditor;

        private void OnEnable()
        {
            serializedObject.Update();
            behaviors = serializedObject.FindProperty("behaviors");
            animations = serializedObject.FindProperty("animations");
            gestureType = serializedObject.FindProperty("gestureType");
            handType = serializedObject.FindProperty("handType");
            useAnimClip = serializedObject.FindProperty("useAnimClip");
            behaviorAndAnimEditor = new EasyBehaviorAndAnimEditor(behaviors, animations);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            avatar = behaviorAndAnimEditor.avatar = EasyAvatarTool.Utility.GetAvatar(((EasyGesture)target).gameObject);

            behaviorAndAnimEditor.useAnimClip = useAnimClip.boolValue = EditorGUILayout.ToggleLeft(Lang.UseAnimClip, useAnimClip.boolValue);
            EditorGUILayout.LabelField(Lang.OnGesture);
            behaviorAndAnimEditor.LayoutGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}

