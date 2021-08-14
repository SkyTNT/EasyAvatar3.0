using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyMenu))]
    public class EasyMenuEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //名字设置
            string tempName = EditorGUILayout.TextField(Lang.Name, target.name);
            if (tempName == "")
                tempName = Lang.SubMenu;
            if (tempName!= target.name)
                Undo.RecordObject(((EasyMenu)target).gameObject,"Set Name");
            target.name = tempName;

            //图标设置
            EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"), new GUIContent(Lang.Icon));
            serializedObject.ApplyModifiedProperties();
        }
    }
}


