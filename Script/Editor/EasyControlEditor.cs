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
        static bool offTrackingFold=true, onTrackingFold = true;
        SerializedProperty controlType, icon, save, toggleDefault, autoRestore, autoTrackingControl, offTrackingControl, onTrackingControl, behaviorGroupList;
        List<EasyBehaviorGroupEditor> editors;
        bool needReLoad;
        int[] typeIndex = { 0, 1, 2, 3 };
        private void OnEnable()
        {
            ReLoad();
            //初始behaviorGroupList为空，设置大小
            if (behaviorGroupList.arraySize == 0)
            {

                ChangeType((EasyControl.Type)controlType.enumValueIndex, (EasyControl.Type)(-1));
                serializedObject.ApplyModifiedProperties();
            }
                

        }

        private void ReLoad()
        {
            serializedObject.Update();
            controlType = serializedObject.FindProperty("type");
            icon = serializedObject.FindProperty("icon");
            save = serializedObject.FindProperty("save");
            toggleDefault = serializedObject.FindProperty("toggleDefault");
            autoRestore = serializedObject.FindProperty("autoRestore");
            autoTrackingControl = serializedObject.FindProperty("autoTrackingControl");
            offTrackingControl = serializedObject.FindProperty("offTrackingControl");
            onTrackingControl = serializedObject.FindProperty("onTrackingControl");
            behaviorGroupList = serializedObject.FindProperty("behaviors");
            editors = new List<EasyBehaviorGroupEditor>();
            for (int i = 0; i < behaviorGroupList.arraySize; i++)
            {
                SerializedProperty behaviourGroup = behaviorGroupList.GetArrayElementAtIndex(i);
                editors.Add(new EasyBehaviorGroupEditor(behaviourGroup));
            }
            serializedObject.ApplyModifiedProperties();
        }
        

        private void ResizeBehaviorGroupList(int size)
        {
            behaviorGroupList.arraySize = size;
            editors.Clear();
            for (int i = 0; i < size; i++)
            {
                SerializedProperty behaviourGroup = behaviorGroupList.GetArrayElementAtIndex(i);
                editors.Add(new EasyBehaviorGroupEditor(behaviourGroup));
            }
        }

        private void ChangeType(EasyControl.Type type, EasyControl.Type pre)
        {
            switch (type)
            {
                case EasyControl.Type.Toggle:
                case EasyControl.Type.Button:
                    ResizeBehaviorGroupList(2);
                    break;
                case EasyControl.Type.RadialPuppet:
                    if (pre == EasyControl.Type.TwoAxisPuppet)//不进行修改
                        break;
                    ResizeBehaviorGroupList(3);
                    for (int i = 1; i < 3; i++)
                    {
                        SerializedProperty behaviourGroup = behaviorGroupList.GetArrayElementAtIndex(i);
                        SerializedProperty position = behaviourGroup.FindPropertyRelative("position");
                        //设置初始位置
                        switch (i)
                        {
                            case 1:
                                position.vector2Value = new Vector2(0, 0);//0
                                break;
                            case 2:
                                position.vector2Value = new Vector2(1, 0);//1
                                break;
                            default:
                                break;
                        }
                        //清除,不清除的话会设置为上一个behaviourGroup的值
                        //behaviourGroup.FindPropertyRelative("list").ClearArray();
                    }
                    break;
                case EasyControl.Type.TwoAxisPuppet:
                    if (pre == EasyControl.Type.RadialPuppet)//不进行修改
                        break;
                    ResizeBehaviorGroupList(6);
                    
                    for (int i = 1; i < 6; i++)
                    {
                        SerializedProperty behaviourGroup = behaviorGroupList.GetArrayElementAtIndex(i);
                        SerializedProperty position = behaviourGroup.FindPropertyRelative("position");
                        //设置初始位置
                        switch (i)
                        {
                            case 1:
                                position.vector2Value = new Vector2(0, 0);//中
                                break;
                            case 2:
                                position.vector2Value = new Vector2(-1, 0);//左
                                break;
                            case 3:
                                position.vector2Value = new Vector2(1, 0);//右
                                break;
                            case 4:
                                position.vector2Value = new Vector2(0, -1);//下
                                break;
                            case 5:
                                position.vector2Value = new Vector2(0, 1);//上
                                break;
                            default:
                                break;
                        }
                        //清除,不清除的话会设置为上一个behaviourGroup的值
                        //behaviourGroup.FindPropertyRelative("list").ClearArray();
                    }
                        
                    break;
                default:
                    break;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //behaviorGroupList.arraySize!=editors.Count判断撤销或重做是引起的behaviorGroupList.arraySize变化
            if (needReLoad||behaviorGroupList.arraySize!=editors.Count)
            {
                needReLoad = false;
                ReLoad();
            }

            avatar = Utility.GetAvatar(((EasyControl)target).gameObject);

            //名字设置
            target.name = EditorGUILayout.TextField(Lang.Name, target.name);
            if (target.name == "")
                target.name = Lang.Control;
            //图标设置
            EditorGUILayout.PropertyField(icon, new GUIContent(Lang.Icon));
            //控件类型
            EasyControl.Type tempType = (EasyControl.Type)controlType.enumValueIndex;
            EditorGUI.BeginChangeCheck();
            controlType.enumValueIndex = EditorGUILayout.IntPopup(Lang.ControlType,(int) tempType, new string[] { Lang.Toggle, Lang.Button, Lang.RadialPuppet, Lang.TwoAxisPuppet }, typeIndex);
            if (EditorGUI.EndChangeCheck())
                ChangeType((EasyControl.Type)controlType.enumValueIndex, tempType);

            if (controlType.enumValueIndex == (int)EasyControl.Type.Toggle)
            {
                //是否保存状态
                save.boolValue = EditorGUILayout.ToggleLeft(Lang.Save, save.boolValue);
                //是否默认打开
                toggleDefault.boolValue = EditorGUILayout.ToggleLeft(Lang.ToggleDefault, toggleDefault.boolValue);
            }
            //是否自动恢复
            autoRestore.boolValue = EditorGUILayout.ToggleLeft(Lang.AutoRestore, autoRestore.boolValue);
            //是否自动设置追踪
            autoTrackingControl.boolValue = EditorGUILayout.ToggleLeft(Lang.AutoTrackingControl, autoTrackingControl.boolValue);

            if (!autoTrackingControl.boolValue)
            {
                offTrackingFold = EditorGUILayout.Foldout(offTrackingFold, Lang.OnClose);
                if(offTrackingFold)
                    EditorGUILayout.PropertyField(offTrackingControl);
                onTrackingFold = EditorGUILayout.Foldout(onTrackingFold, Lang.OnOpen);
                if (onTrackingFold)
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
                    labels = new string[] { Lang.OnRadialPuppetOff, Lang.OnRadialPuppet };
                    break;
                case EasyControl.Type.TwoAxisPuppet:
                    labels = new string[] { Lang.OnTwoAxisPuppetOff, Lang.OnTwoAxisPuppetPosition };
                    break;
                default:
                    labels = new string[] { "" };
                    break;
            }


            //TwoAxisPuppet是可以自己添加多个BehaviorGroup的
            if ((EasyControl.Type)controlType.enumValueIndex == EasyControl.Type.TwoAxisPuppet || (EasyControl.Type)controlType.enumValueIndex == EasyControl.Type.RadialPuppet)
            {
                int editorCount = editors.Count;
                int removeIndex, changeIndex1, changeIndex2;
                removeIndex = changeIndex1 = changeIndex2 = -1;

                for (int i = 0; i < editorCount; i++)
                {
                    SerializedProperty position = behaviorGroupList.GetArrayElementAtIndex(i).FindPropertyRelative("position");
                    Vector2 positionVal = position.vector2Value;
                    EasyBehaviorGroupEditor editor = editors[i];
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.BeginHorizontal();
                    if((EasyControl.Type)controlType.enumValueIndex == EasyControl.Type.TwoAxisPuppet)//TwoAxisPuppet有Y
                        GUILayout.Label(i == 0 ? labels[0] : string.Format(labels[1], positionVal.x, positionVal.y), EditorStyles.boldLabel);
                    else
                        GUILayout.Label(i == 0 ? labels[0] : string.Format(labels[1], positionVal.x), EditorStyles.boldLabel);

                    GUILayout.FlexibleSpace();
                    //0处为关闭，位置固定为0，不能删除
                    if (i > 0)
                    {
                        //上移
                        if (i > 1 && GUILayout.Button(Lang.Up))
                        {
                            changeIndex1 = i - 1;
                            changeIndex2 = i;
                        }
                        //下移
                        if (i < editorCount - 1 && GUILayout.Button(Lang.Down))
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
                    if (i > 0)
                    {
                        
                        if ((EasyControl.Type)controlType.enumValueIndex == EasyControl.Type.TwoAxisPuppet)//TwoAxisPuppet有Y
                        {
                            positionVal.x = EditorGUILayout.Slider(Lang.OnTwoAxisPuppetH, positionVal.x, -1, 1);
                            positionVal.y = EditorGUILayout.Slider(Lang.OnTwoAxisPuppetV, positionVal.y, -1, 1);

                        }
                        else
                        {
                            positionVal.x = EditorGUILayout.Slider(Lang.OnRadialPuppetX, positionVal.x, 0, 1);
                        }
                            
                    }
                    editor.DoLayout(avatar);
                    GUILayout.EndVertical();
                    position.vector2Value = positionVal;
                }

                //只能在遍历外进行操作
                if (removeIndex != -1)
                {
                    behaviorGroupList.DeleteArrayElementAtIndex(removeIndex);
                    needReLoad = true;
                }
                if (changeIndex1 != -1 && changeIndex2 != -1)
                {
                    behaviorGroupList.MoveArrayElement(changeIndex1, changeIndex2);
                    needReLoad = true;
                }

                if (GUILayout.Button(Lang.Add))
                {
                    behaviorGroupList.arraySize++;
                    //默认情况新的元素会填充上一个元素的值，这里进行清除
                    SerializedProperty behaviorGroup = behaviorGroupList.GetArrayElementAtIndex(behaviorGroupList.arraySize - 1);
                    behaviorGroup.FindPropertyRelative("list").ClearArray();
                    behaviorGroup.FindPropertyRelative("position").vector2Value = new Vector2();
                    needReLoad = true;
                }
            }
            else
            {
                for (int i = 0; i < editors.Count; i++)
                {
                    EasyBehaviorGroupEditor editor = editors[i];
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.Label(labels[i], EditorStyles.boldLabel);
                    editor.DoLayout(avatar);
                    GUILayout.EndVertical();
                }
            }

            serializedObject.ApplyModifiedProperties();

        }

    }
}