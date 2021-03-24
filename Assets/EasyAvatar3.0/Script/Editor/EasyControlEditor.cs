using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using System.IO;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyControl))]
    public class EasyControlEditor : Editor
    {
        SerializedProperty icon, offBehaviors, onBehaviors, useAnimClip, offAnims, onAnims;
        GameObject avatar;

        EasyBehaviorAndAnimEditor offEditor, onEditor;

        private void OnEnable()
        {
            
            serializedObject.Update();
            icon = serializedObject.FindProperty("icon");
            offBehaviors = serializedObject.FindProperty("offBehaviors");
            onBehaviors = serializedObject.FindProperty("onBehaviors");
            useAnimClip = serializedObject.FindProperty("useAnimClip");
            offAnims = serializedObject.FindProperty("offAnims");
            onAnims = serializedObject.FindProperty("onAnims");
            offEditor = new EasyBehaviorAndAnimEditor(offBehaviors, offAnims);
            onEditor = new EasyBehaviorAndAnimEditor(onBehaviors, onAnims);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            offEditor.StopPreview();
            onEditor.StopPreview();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            avatar = GetAvatar();
            offEditor.avatar = avatar;
            onEditor.avatar = avatar;
            //名字设置
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Lang.Name);
            target.name = EditorGUILayout.TextField(target.name);
            if (target.name == "")
                target.name = Lang.Control;
            EditorGUILayout.EndHorizontal();
            //图标设置
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Lang.Icon);
            EditorGUILayout.PropertyField(icon, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            useAnimClip.boolValue = EditorGUILayout.ToggleLeft(Lang.UseAnimClip, useAnimClip.boolValue);
            offEditor.useAnimClip = onEditor.useAnimClip = useAnimClip.boolValue;

            
            GUILayout.Label(Lang.OnSwitchOff, EditorStyles.boldLabel);
            offEditor.LayoutGUI();

            GUILayout.Label(Lang.OnSwitchOn, EditorStyles.boldLabel);
            onEditor.LayoutGUI();

            //一个预览的时候把另一个关了
            if (offEditor.previewStarted)
                onEditor.previewing = false;
            if (onEditor.previewStarted)
                offEditor.previewing = false;


            serializedObject.ApplyModifiedProperties();
            
        }
        

        /// <summary>
        /// 获取avatar
        /// </summary>
        /// <returns></returns>
        public GameObject GetAvatar()
        {
            EasyAvatarHelper avatarHelper = ((EasyControl)target).GetComponentInParent<EasyAvatarHelper>();
            //检测是否本控件在是否在Avatar Helper中
            if (!avatarHelper)
                return null;
            GameObject avatar = avatarHelper.avatar;
            //检测是否在Avatar Helper中设置了avatar
            if (!avatar)
                return null;
            return avatar;
        }
        
    }
}
