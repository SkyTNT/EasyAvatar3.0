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
        SerializedProperty controlType, icon, autoRestore, autoTrackingControl, offTrackingControl, onTrackingControl, behaviorsList, behaviors1, behaviors2, behaviors3, behaviors4;
        GameObject avatar;
        EasyBehaviorsEditor editor1, editor2, editor3, editor4, editor5;
        int[] typeIndex = { 0, 1 };
        string[] typeLabels;
        private void OnEnable()
        {

            serializedObject.Update();
            controlType = serializedObject.FindProperty("type");
            icon = serializedObject.FindProperty("icon");
            autoRestore = serializedObject.FindProperty("autoRestore");
            autoTrackingControl = serializedObject.FindProperty("autoTrackingControl");
            offTrackingControl = serializedObject.FindProperty("offTrackingControl");
            onTrackingControl = serializedObject.FindProperty("onTrackingControl");
            behaviorsList = serializedObject.FindProperty("behaviors");
            ChangeType((EasyControl.Type)controlType.enumValueIndex);

            typeLabels = new string[] { Lang.Toggle, Lang.RadialPuppet };

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {

        }

        private void ChangeType(EasyControl.Type type)
        {
            switch (type)
            {
                case EasyControl.Type.Toggle:
                    behaviorsList.arraySize = 2;
                    behaviors1 = behaviorsList.GetArrayElementAtIndex(0);
                    behaviors2 = behaviorsList.GetArrayElementAtIndex(1);
                    editor1 = new EasyBehaviorsEditor(behaviors1);
                    editor2 = new EasyBehaviorsEditor(behaviors2);
                    break;
                case EasyControl.Type.RadialPuppet:
                    behaviorsList.arraySize = 3;
                    behaviors1 = behaviorsList.GetArrayElementAtIndex(0);
                    behaviors2 = behaviorsList.GetArrayElementAtIndex(1);
                    behaviors3 = behaviorsList.GetArrayElementAtIndex(2);
                    editor1 = new EasyBehaviorsEditor(behaviors1);
                    editor2 = new EasyBehaviorsEditor(behaviors2);
                    editor3 = new EasyBehaviorsEditor(behaviors3);
                    break;
                default:
                    break;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            avatar = Utility.GetAvatar(((EasyControl)target).gameObject);

            //名字设置
            target.name = EditorGUILayout.TextField(Lang.Name, target.name);
            if (target.name == "")
                target.name = Lang.Control;
            //图标设置
            EditorGUILayout.PropertyField(icon, new GUIContent(Lang.Icon));
            //控件类型
            EditorGUI.BeginChangeCheck();
            controlType.enumValueIndex = EditorGUILayout.IntPopup(Lang.ControlType, controlType.enumValueIndex, typeLabels, typeIndex);
            if (EditorGUI.EndChangeCheck())
                ChangeType((EasyControl.Type)controlType.enumValueIndex);
            
            //是否自动设置追踪
            autoTrackingControl.boolValue = EditorGUILayout.ToggleLeft(Lang.autoTrackingControl, autoTrackingControl.boolValue);

            if (!autoTrackingControl.boolValue)
            {
                GUILayout.Label(Lang.OnSwitchOn, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(offTrackingControl);
                GUILayout.Label(Lang.OnSwitchOff, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(onTrackingControl);
            }

            if (controlType.enumValueIndex == (int)EasyControl.Type.Toggle)
            {
                editor1.avatar = editor2.avatar = avatar;
                GUILayout.Label(Lang.OnSwitchOn, EditorStyles.boldLabel);
                editor1.DoLayout();

                //是否自动恢复
                autoRestore.boolValue = EditorGUILayout.ToggleLeft(Lang.AutoRestore, autoRestore.boolValue);

                GUILayout.Label(Lang.OnSwitchOff, EditorStyles.boldLabel);
                editor2.DoLayout();
            }
            else if (controlType.enumValueIndex == (int)EasyControl.Type.RadialPuppet)
            {
                editor1.avatar = editor2.avatar = editor3.avatar = avatar;

                GUILayout.Label(Lang.OnRadialPuppet0, EditorStyles.boldLabel);
                editor1.DoLayout();

                GUILayout.Label(Lang.OnRadialPuppet1, EditorStyles.boldLabel);
                editor2.DoLayout();

                //是否自动恢复
                autoRestore.boolValue = EditorGUILayout.ToggleLeft(Lang.AutoRestore, autoRestore.boolValue);

                GUILayout.Label(Lang.OnRadialPuppetOff, EditorStyles.boldLabel);
                editor3.DoLayout();
            }

            serializedObject.ApplyModifiedProperties();

        }

    }
}
