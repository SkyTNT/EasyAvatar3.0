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
            GameObject avatarObj = (GameObject)avatar.objectReferenceValue;
            GameObject temp = (GameObject)EditorGUILayout.ObjectField(Lang.Avatar, avatarObj, typeof(GameObject), true);

            if(temp!= avatarObj)
            {
                //检测Avatar Helper是否包含在Avatar中，vrchat是不允许avatar包含非白名单内的脚本的。
                if (temp && ((EasyAvatarHelper)target).transform.IsChildOf(temp.transform))
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarHelperInAvatar, "ok");
                else
                    avatar.objectReferenceValue = avatarObj = temp;
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
                        EasyAvatarTool.Builder.Build((EasyAvatarHelper)target);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        
    }
}

