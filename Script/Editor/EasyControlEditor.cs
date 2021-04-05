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
        SerializedProperty icon, behaviors1, behaviors2, behaviors3, behaviors4, useAnimClip, autoRestore, anims1, anims2, anims3, anims4, controlType;
        GameObject avatar;

        EasyBehaviorAndAnimEditor editor1, editor2, editor3, editor4;

        int[] typeIndex = { 0, 1 };
        string[] typeLabels;

        private void OnEnable()
        {
            
            serializedObject.Update();
            icon = serializedObject.FindProperty("icon");
            behaviors1 = serializedObject.FindProperty("behaviors1");
            behaviors2 = serializedObject.FindProperty("behaviors2");
            behaviors3 = serializedObject.FindProperty("behaviors3");
            behaviors4 = serializedObject.FindProperty("behaviors4");
            anims1 = serializedObject.FindProperty("anims1");
            anims2 = serializedObject.FindProperty("anims2");
            anims3 = serializedObject.FindProperty("anims3");
            anims4 = serializedObject.FindProperty("anims4");
            controlType = serializedObject.FindProperty("type");
            useAnimClip = serializedObject.FindProperty("useAnimClip");
            autoRestore = serializedObject.FindProperty("autoRestore");

            editor1 = new EasyBehaviorAndAnimEditor(behaviors1, anims1);
            editor2 = new EasyBehaviorAndAnimEditor(behaviors2, anims2);
            editor3 = new EasyBehaviorAndAnimEditor(behaviors3, anims3);
            editor4 = new EasyBehaviorAndAnimEditor(behaviors4, anims4);

            typeLabels = new string[] { Lang.Toggle, Lang.RadialPuppet };
            
            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            editor1.StopPreview();
            editor2.StopPreview();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            avatar = Utility.GetAvatar(((EasyControl)target).gameObject);
            editor1.avatar = editor2.avatar = editor3.avatar = editor4.avatar = avatar;
            //名字设置
            target.name = EditorGUILayout.TextField(Lang.Name,target.name);
            if (target.name == "")
                target.name = Lang.Control;
            //图标设置
            EditorGUILayout.PropertyField(icon, new GUIContent(Lang.Icon));
            //控件类型
            controlType.enumValueIndex = EditorGUILayout.IntPopup(Lang.ControlType, controlType.enumValueIndex, typeLabels, typeIndex);
            //是否使用动画
            editor1.useAnimClip = editor2.useAnimClip = editor3.useAnimClip = editor4.useAnimClip = useAnimClip.boolValue = EditorGUILayout.ToggleLeft(Lang.UseAnimClip, useAnimClip.boolValue);
            //是否自动恢复
            autoRestore.boolValue = EditorGUILayout.ToggleLeft(Lang.AutoRestore, autoRestore.boolValue);

            if (controlType.enumValueIndex == (int)EasyControl.Type.Toggle)
            {
                GUILayout.Label(Lang.OnSwitchOn, EditorStyles.boldLabel);
                editor1.LayoutGUI();

                GUILayout.Label(Lang.OnSwitchOff, EditorStyles.boldLabel);
                editor2.LayoutGUI();
            }
            else if(controlType.enumValueIndex == (int)EasyControl.Type.RadialPuppet)
            {
                GUILayout.Label(Lang.OnRadialPuppet0, EditorStyles.boldLabel);
                editor1.LayoutGUI();

                GUILayout.Label(Lang.OnRadialPuppet1, EditorStyles.boldLabel);
                editor2.LayoutGUI();

                GUILayout.Label(Lang.OnRadialPuppetOff, EditorStyles.boldLabel);
                editor3.LayoutGUI();
            }
            

            //一个预览的时候把另一个关了
            if (editor1.previewStarted)
            {
                editor2.previewing = false;
                editor3.previewing = false;
                editor4.previewing = false;
            }
            if (editor2.previewStarted)
            {
                editor1.previewing = false;
                editor3.previewing = false;
                editor4.previewing = false;
            }
            if (editor3.previewStarted)
            {
                editor1.previewing = false;
                editor2.previewing = false;
                editor4.previewing = false;
            }
            if (editor4.previewStarted)
            {
                editor1.previewing = false;
                editor2.previewing = false;
                editor3.previewing = false;
            }

            serializedObject.ApplyModifiedProperties();
            
        }
        
    }
}
