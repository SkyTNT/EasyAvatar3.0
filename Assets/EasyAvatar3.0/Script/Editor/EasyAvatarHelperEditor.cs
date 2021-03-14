using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            if(avatar.objectReferenceValue) target.name =Lang.AvatarHelper + avatar.objectReferenceValue.name;
            if (GUILayout.Button(Lang.AvatarApply)){

            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

