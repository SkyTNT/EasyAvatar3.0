using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;

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
            EditorGUILayout.PropertyField(avatar, new GUIContent(Lang.Avatar));
            GameObject avatarObj = (GameObject)avatar.objectReferenceValue;
            if(avatar.objectReferenceValue) target.name =Lang.AvatarHelper + avatar.objectReferenceValue.name;
            if (avatarObj)
            {
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

