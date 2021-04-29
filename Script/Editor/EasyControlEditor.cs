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
        GameObject avatar;
        SerializedProperty controlType, icon, autoRestore, autoTrackingControl, offTrackingControl, onTrackingControl, behaviorGroupList;
        List<SerializedProperty> behaviorGroups;
        List<EasyBehaviorsEditor> editors;
        int[] typeIndex = { 0, 1, 2, 3 };
        private void OnEnable()
        {

            serializedObject.Update();
            controlType = serializedObject.FindProperty("type");
            icon = serializedObject.FindProperty("icon");
            autoRestore = serializedObject.FindProperty("autoRestore");
            autoTrackingControl = serializedObject.FindProperty("autoTrackingControl");
            offTrackingControl = serializedObject.FindProperty("offTrackingControl");
            onTrackingControl = serializedObject.FindProperty("onTrackingControl");
            behaviorGroupList = serializedObject.FindProperty("behaviors");
            behaviorGroups = new List<SerializedProperty>();
            editors = new List<EasyBehaviorsEditor>();
            for (int i = 0; i < behaviorGroupList.arraySize; i++)
            {
                SerializedProperty behaviourGroup = behaviorGroupList.GetArrayElementAtIndex(i);
                behaviorGroups.Add(behaviourGroup);
                editors.Add(new EasyBehaviorsEditor(behaviourGroup));
            }
            

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {

        }

        private void ResizeBehaviorGroupList(int size)
        {
            behaviorGroupList.arraySize = size;
            behaviorGroups.Clear();
            editors.Clear();
            for (int i = 0; i < size; i++)
            {
                SerializedProperty behaviourGroup = behaviorGroupList.GetArrayElementAtIndex(i);
                behaviorGroups.Add(behaviourGroup);
                editors.Add(new EasyBehaviorsEditor(behaviourGroup));
            }
        }

        private void ChangeType(EasyControl.Type type)
        {
            switch (type)
            {
                case EasyControl.Type.Toggle:
                case EasyControl.Type.Button:
                    ResizeBehaviorGroupList(2);
                    break;
                case EasyControl.Type.RadialPuppet:
                    ResizeBehaviorGroupList(3);
                    break;
                case EasyControl.Type.TwoAxisPuppet:
                    ResizeBehaviorGroupList(5);
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
            controlType.enumValueIndex = EditorGUILayout.IntPopup(Lang.ControlType, controlType.enumValueIndex, new string[] { Lang.Toggle, Lang.Button, Lang.RadialPuppet, Lang.TwoAxisPuppet }, typeIndex);
            if (EditorGUI.EndChangeCheck())
                ChangeType((EasyControl.Type)controlType.enumValueIndex);

            //是否自动恢复
            autoRestore.boolValue = EditorGUILayout.ToggleLeft(Lang.AutoRestore, autoRestore.boolValue);
            //是否自动设置追踪
            autoTrackingControl.boolValue = EditorGUILayout.ToggleLeft(Lang.autoTrackingControl, autoTrackingControl.boolValue);

            if (!autoTrackingControl.boolValue)
            {
                GUILayout.Label(Lang.OnSwitchOn, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(offTrackingControl);
                GUILayout.Label(Lang.OnSwitchOff, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(onTrackingControl);
            }

            string[] labels;
            switch ((EasyControl.Type)controlType.enumValueIndex)
            {
                case EasyControl.Type.Toggle:
                    labels = new string[] { Lang.OnSwitchOff, Lang.OnSwitchOn };
                    break;
                case EasyControl.Type.Button:
                    labels = new string[] { Lang.OnRelease, Lang.OnPress };
                    break;
                case EasyControl.Type.RadialPuppet:
                    labels = new string[] { Lang.OnRadialPuppetOff, Lang.OnRadialPuppet0, Lang.OnRadialPuppet1 };
                    break;
                case EasyControl.Type.TwoAxisPuppet:
                    labels = new string[] { Lang.OnTwoAxisPuppetOff, Lang.OnTwoAxisPuppetPosition };
                    break;
                default:
                    labels = new string[] { "" };
                    break;
            }


            //TwoAxisPuppet是可以自己添加多个BehaviorGroup的
            if ((EasyControl.Type)controlType.enumValueIndex == EasyControl.Type.TwoAxisPuppet)
            {
                int editorCount = editors.Count;
                int removeIndex,changeIndex1, changeIndex2;
                removeIndex = changeIndex1 = changeIndex2 = -1;

                for (int i = 0; i < editorCount; i++)
                {
                    EasyBehaviorsEditor editor = editors[i];
                    editor.avatar = avatar;
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(i == 0 ? labels[0] : labels[1], EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    //0处为关闭，位置固定为0，不能删除
                    if (i > 0)
                    {
                        //上移
                        if (i>1&&GUILayout.Button(Lang.Up))
                        {
                            changeIndex1 = i - 1;
                            changeIndex2 = i;
                        }
                        //下移
                        if (i<editorCount-1&&GUILayout.Button(Lang.Down))
                        {
                            changeIndex1 = i;
                            changeIndex2 = i + 1;
                        }
                        //删除
                        if (GUILayout.Button(Lang.Delete))
                        {
                            removeIndex = i;
                        }
                    }
                    
                    GUILayout.EndHorizontal();
                    editor.DoLayout();
                    GUILayout.EndVertical();
                }
                
                if (removeIndex != -1)
                {
                    editors.RemoveAt(removeIndex);
                    behaviorGroupList.DeleteArrayElementAtIndex(removeIndex);
                }
                if (changeIndex1 != -1 && changeIndex2 != -1)
                {

                }
                
                if (GUILayout.Button(Lang.Add))
                {
                    int index = behaviorGroupList.arraySize++;
                    SerializedProperty behaviorGroup = behaviorGroupList.GetArrayElementAtIndex(index);
                    behaviorGroups.Add(behaviorGroup);
                    editors.Add(new EasyBehaviorsEditor(behaviorGroup));
                }
            }
            else
            {
                for (int i = 0; i < editors.Count; i++)
                {
                    EasyBehaviorsEditor editor = editors[i];
                    editor.avatar = avatar;
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.Label(labels[i], EditorStyles.boldLabel);
                    editor.DoLayout();
                    GUILayout.EndVertical();
                }
            }

            serializedObject.ApplyModifiedProperties();

        }

    }
}
