using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using System.IO;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyAvatarHelper))]
    public class EasyAvatarHelperEditor : Editor
    {
        SerializedProperty avatar;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            avatar = serializedObject.FindProperty("avatar");
            EditorGUI.BeginChangeCheck();
            avatar.objectReferenceValue = EditorGUILayout.ObjectField(Lang.Avatar, avatar.objectReferenceValue, typeof(GameObject), true);
            GameObject avatarObj = (GameObject)avatar.objectReferenceValue;
            if (EditorGUI.EndChangeCheck())
            {
                //防止设置为自己
                if(avatarObj.transform == ((EasyAvatarHelper)target).transform)
                {
                    avatar.objectReferenceValue = avatarObj = null;
                }
                //检测Avatar Helper是否包含在Avatar中，vrchat是不允许avatar包含非白名单内的脚本的。
                if (avatar.objectReferenceValue&&((EasyAvatarHelper)target).transform.IsChildOf(avatarObj.transform))
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarHelperInAvatar, "ok");
            }
            
            if (avatarObj)
            {
                target.name = Lang.AvatarHelper + avatar.objectReferenceValue.name;
                if (!avatarObj.GetComponent<VRCAvatarDescriptor>())
                {
                    if (GUILayout.Button(Lang.AvataNoDescriptor))
                    {
                        avatarObj.AddComponent<VRCAvatarDescriptor>();
                    }
                }
                else
                {
                    if (GUILayout.Button(Lang.AvatarApply))
                    {
                        new EasyAvatarBuilder((EasyAvatarHelper)target).Build();
                    }
                    EditorGUILayout.HelpBox(Lang.AvatarApplyHelpBox, MessageType.Warning);

                    EasyMissingTargetReplacer.Field((EasyAvatarHelper)target);
                    EditorGUILayout.HelpBox(Lang.OneClickReplaceMissingTargetNote, MessageType.Info);
                }
                //检测Avatar Helper是否包含在Avatar中，vrchat是不允许avatar包含非白名单内的脚本的。
                if (((EasyAvatarHelper)target).transform.IsChildOf(avatarObj.transform))
                    EditorGUILayout.HelpBox(Lang.ErrAvatarHelperInAvatar, MessageType.Error);
            }

            serializedObject.ApplyModifiedProperties();
        }
        
    }
}

