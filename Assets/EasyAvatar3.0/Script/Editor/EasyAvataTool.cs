using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace EasyAvatar
{
    
    public class EasyAvatarTool
    {

        [MenuItem("GameObject/EasyAvatar3.0/Avatar Helper", priority = 0)]
        public static bool CreateAvatarInfo()
        {
            GameObject gameObject = new GameObject(Lang.AvatarHelper);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create Avatar Helper");
            gameObject.AddComponent<EasyAvatarHelper>();

            if (Selection.activeGameObject)
                gameObject.transform.parent = Selection.activeGameObject.transform;
            Selection.activeGameObject = gameObject;

            return true;
        }

        [MenuItem("GameObject/EasyAvatar3.0/Expression Menu", priority = 0)]
        public static bool CreateExpressionMenu()
        {
            bool isSubMenu = false;

            if (Selection.activeGameObject)
            {
                //检查控件是否超过8
                if (GetMenuItemCount(Selection.activeGameObject.transform) >= 8)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuItemLen8, "ok");
                    return false;
                }
                //检查是否在控件中添加菜单
                if (Selection.activeGameObject.transform.GetComponent<EasyControl>())
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuInControl, "ok");
                    return false;
                }
                //检查是否Avatar已有菜单
                if (Selection.activeGameObject.transform.GetComponent<EasyAvatarHelper>() && GetMenuCount(Selection.activeGameObject.transform) >= 1)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarMenuLen1, "ok");
                    return false;
                }
                //检查是否为子菜单
                if (Selection.activeGameObject.transform.GetComponent<EasyMenu>())
                {
                    isSubMenu = true;
                }
            }

            //检查是否直接创建
            if (!Selection.activeGameObject || (!Selection.activeGameObject.transform.GetComponent<EasyAvatarHelper>() && !Selection.activeGameObject.transform.GetComponent<EasyMenu>()))
                if (!CreateAvatarInfo())
                    return false;


            GameObject gameObject = new GameObject(isSubMenu?Lang.SubMenu:Lang.MainMenu);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create Expression Menu");
            gameObject.AddComponent<EasyMenu>();

            if (Selection.activeGameObject)
                gameObject.transform.parent = Selection.activeGameObject.transform;
            Selection.activeGameObject = gameObject;

            return true;
        }

        [MenuItem("GameObject/EasyAvatar3.0/Expression Menu Control", priority = 0)]
        public static bool CreateExpressionMenuControl()
        {
            if (Selection.activeGameObject)
            {
                //检查控件是否超过8
                if (GetMenuItemCount(Selection.activeGameObject.transform) >= 8)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuItemLen8, "ok");
                    return false;
                }
                //检查是否在控件中添加控件
                if (Selection.activeGameObject.transform.GetComponent<EasyControl>())
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrControlInControl, "ok");
                    return false;
                }
                
            }
            //没有菜单则创建菜单
            if (!Selection.activeGameObject || !Selection.activeGameObject.transform.GetComponent<EasyMenu>())
                if (!CreateExpressionMenu())
                    return false;

            GameObject gameObject = new GameObject(Lang.Control);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create Menu Control");
            gameObject.AddComponent<EasyControl>();
            if (Selection.activeGameObject)
                gameObject.transform.parent = Selection.activeGameObject.transform;
            Selection.activeGameObject = gameObject;

            return true;
        }

        public static int GetMenuItemCount(Transform transform)
        {
            int count = 0;
            Transform child;
            for(int i = 0;i< transform.childCount; i++)
            {
                child = transform.GetChild(i);
                if (child.GetComponent<EasyMenu>() || child.GetComponent<EasyControl>())
                    count++;
            }
            return count;
        }

        public static int GetMenuCount(Transform transform)
        {
            int count = 0;
            Transform child;
            for (int i = 0; i < transform.childCount; i++)
            {
                child = transform.GetChild(i);
                if (child.GetComponent<EasyMenu>())
                    count++;
            }
            return count;
        }

        #region Builder
        public class Builder
        {
            static int buttonCount = 0;
            static AnimatorController controller;
            public static void Build(EasyAvatarHelper helper)
            {
                buttonCount = 0;
                GameObject avatar = helper.avatar;
                if (!avatar)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarNotSet, "ok");
                    return;
                }
                EasyMenu mainMenu = null;
                foreach (Transform child in helper.gameObject.transform)
                {
                    EasyMenu temp = child.GetComponent<EasyMenu>();
                    if (temp)
                    {
                        if (mainMenu)//检测是否有多个主菜单
                        {
                            EditorUtility.DisplayDialog("Error", Lang.ErrAvatarMenuLen1, "ok");
                            return;
                        }
                        mainMenu = temp;
                    }
                }
                if (!mainMenu)//检测是否有主菜单
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarMenuLen0, "ok");
                    return;
                }

                //清除目录
                if (Directory.Exists("Assets/EasyAvatar3.0/Build/Anim/"))
                {
                    Directory.Delete("Assets/EasyAvatar3.0/Build/Anim/", true);
                    File.Delete("Assets/EasyAvatar3.0/Build/Anim.meta");
                }
                    
                if (Directory.Exists("Assets/EasyAvatar3.0/Build/Menu/"))
                {
                    Directory.Delete("Assets/EasyAvatar3.0/Build/Menu/", true);
                    File.Delete("Assets/EasyAvatar3.0/Build/Menu.meta");
                }
                    
                Directory.CreateDirectory("Assets/EasyAvatar3.0/Build/Anim/");
                Directory.CreateDirectory("Assets/EasyAvatar3.0/Build/Menu/");

                //初始化AnimatorController
                controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/EasyAvatar3.0/Build/Anim/FXLayer.controller");
                controller.AddParameter("button", AnimatorControllerParameterType.Int);
                //初始化VRCExpressionParameters
                List<VRCExpressionParameters.Parameter> parameters = new List<VRCExpressionParameters.Parameter>();
                VRCExpressionParameters expressionParameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                parameters.Add(new VRCExpressionParameters.Parameter() {name = "VRCEmote",valueType = VRCExpressionParameters.ValueType.Int });
                parameters.Add(new VRCExpressionParameters.Parameter() { name = "button", valueType = VRCExpressionParameters.ValueType.Int ,saved = false});
                expressionParameters.parameters = parameters.ToArray();
                AssetDatabase.CreateAsset(expressionParameters, "Assets/EasyAvatar3.0/Build/Menu/Parameters.asset");
                //构建菜单
                VRCExpressionsMenu VRCMenu = BuildMenu(avatar, mainMenu, "Menu");
                //设置VRCAvatarDescriptor
                VRCAvatarDescriptor avatarDescriptor = avatar.GetComponent<VRCAvatarDescriptor>();
                avatarDescriptor.customExpressions = true;
                avatarDescriptor.expressionParameters = expressionParameters;
                avatarDescriptor.expressionsMenu = VRCMenu;
                avatarDescriptor.customizeAnimationLayers = true;
                avatarDescriptor.baseAnimationLayers = new CustomAnimLayer[]{
                    new CustomAnimLayer(){type = AnimLayerType.Base ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Additive ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Gesture ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Action ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.FX ,isDefault = false,animatorController = controller},
                };
                //保存
                AssetDatabase.SaveAssets();
            }

            private static VRCExpressionsMenu BuildMenu(GameObject avatar, EasyMenu menu, string prefix)
            {
                if (GetMenuItemCount(menu.gameObject.transform) > 8)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuItemLen8, "ok");
                    return null;
                }

                VRCExpressionsMenu expressionsMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                AssetDatabase.CreateAsset(expressionsMenu, "Assets/EasyAvatar3.0/Build/Menu/" + prefix + ".asset");

                int count = 0;
                foreach (Transform child in menu.gameObject.transform)
                {
                    count++;
                    EasyMenu subMenu = child.GetComponent<EasyMenu>();
                    EasyControl control = child.GetComponent<EasyControl>();
                    if (control)
                    {
                        buttonCount++;
                        SerializedObject serializedObject = new SerializedObject(control);
                        serializedObject.Update();
                        //加_count_避免重名
                        BuildButtonLayer(prefix + "_" + count + "_" + control.name, serializedObject);

                        VRCExpressionsMenu.Control vrcControl = new VRCExpressionsMenu.Control();
                        vrcControl.name = control.name;
                        vrcControl.type = VRCExpressionsMenu.Control.ControlType.Button;
                        vrcControl.icon = (Texture2D)serializedObject.FindProperty("icon").objectReferenceValue;
                        vrcControl.parameter = new VRCExpressionsMenu.Control.Parameter() { name = "button" };
                        vrcControl.value = buttonCount;
                        expressionsMenu.controls.Add(vrcControl);
                    }

                    if (subMenu)
                    {

                        VRCExpressionsMenu.Control vrcControl = new VRCExpressionsMenu.Control();
                        vrcControl.name = subMenu.name;
                        vrcControl.type = VRCExpressionsMenu.Control.ControlType.SubMenu;
                        vrcControl.subMenu = BuildMenu(avatar, subMenu, prefix + "_" + count + "_" + subMenu.name);
                        expressionsMenu.controls.Add(vrcControl);
                    }
                }
                
                return expressionsMenu;
            }

            private static void BuildButtonLayer(string name, SerializedObject control)
            {
                AnimationClip offClip = Utility.GenerateAnimClip("Assets/EasyAvatar3.0/Build/Anim/" + name + "_off.anim", control.FindProperty("offBehaviors"));
                AnimationClip onClip = Utility.GenerateAnimClip("Assets/EasyAvatar3.0/Build/Anim/" + name + "_on.anim", control.FindProperty("onBehaviors"));
                AnimatorControllerLayer layer = new AnimatorControllerLayer() { name = name, stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
                controller.AddLayer(layer);
                AnimatorStateMachine stateMachine = layer.stateMachine;
                AnimatorState stateOff = stateMachine.AddState("off");
                AnimatorState statePre = stateMachine.AddState("pre");
                AnimatorState stateOn = stateMachine.AddState("on");
                AnimatorState stateOut = stateMachine.AddState("out");
                stateOff.motion = offClip;
                stateOn.motion = onClip;
                stateMachine.defaultState = stateOff;
                AnimatorStateTransition off_pre = stateOff.AddTransition(statePre);
                off_pre.AddCondition(AnimatorConditionMode.Equals, buttonCount, "button");
                off_pre.duration = 0;
                AnimatorStateTransition pre_on = statePre.AddTransition(stateOn);
                pre_on.AddCondition(AnimatorConditionMode.Equals, 0, "button");
                pre_on.duration = 0;
                AnimatorStateTransition on_out = stateOn.AddTransition(stateOut);
                on_out.AddCondition(AnimatorConditionMode.Equals, buttonCount, "button");
                on_out.duration = 0;
                AnimatorStateTransition out_off = stateOut.AddTransition(stateOff);
                out_off.AddCondition(AnimatorConditionMode.Equals, 0, "button");
                out_off.duration = 0;
            }
            
        }
        #endregion

        #region Utility
        public class Utility
        {
            public static AnimationClip GenerateAnimClip(string path, SerializedProperty behaviors)
            {
                AnimationClip clip = GenerateAnimClip(behaviors);
                AssetDatabase.CreateAsset(clip, path);
                AssetDatabase.SaveAssets();
                return clip;
            }

            public static AnimationClip GenerateAnimClip(SerializedProperty behaviors)
            {
                AnimationClip clip = new AnimationClip();
                clip.frameRate = 60;


                int behaviorsCount = behaviors.arraySize;
                for (int i = 0; i < behaviorsCount; i++)
                {
                    SerializedProperty behavior = behaviors.GetArrayElementAtIndex(i);
                    SerializedProperty propertyGroup = behavior.FindPropertyRelative("propertyGroup");
                    int propertyCount = propertyGroup.arraySize;
                    for (int j = 0; j < propertyCount; j++)
                    {
                        SerializedProperty property = propertyGroup.GetArrayElementAtIndex(j);
                        if (property.FindPropertyRelative("targetProperty").stringValue == "")
                            continue;

                        EditorCurveBinding binding = GetBinding(property);

                        if (!binding.isPPtrCurve)
                        {
                            float value = property.FindPropertyRelative("floatValue").floatValue;
                            AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(0, value, 1.0f / 60, value));
                        }
                        else
                        {
                            UnityEngine.Object obj = property.FindPropertyRelative("objectValue").objectReferenceValue;
                            ObjectReferenceKeyframe[] objectReferenceKeyframes = {
                        new ObjectReferenceKeyframe() { time = 0, value = obj },
                        new ObjectReferenceKeyframe() { time = 1.0f / 60, value = obj }
                        };
                            AnimationUtility.SetObjectReferenceCurve(clip, binding, objectReferenceKeyframes);
                        }
                    }
                }
                return clip;
            }

            public static void CopyBehavior(SerializedProperty dest, SerializedProperty src)
            {
                SerializedProperty destPropertyGroup = dest.FindPropertyRelative("propertyGroup");
                SerializedProperty srcPropertyGroup = src.FindPropertyRelative("propertyGroup");
                destPropertyGroup.arraySize = srcPropertyGroup.arraySize;

                for (int i = 0; i < destPropertyGroup.arraySize; i++)
                {
                    CopyProperty(destPropertyGroup.GetArrayElementAtIndex(i), srcPropertyGroup.GetArrayElementAtIndex(i));
                }
            }


            public static void CopyProperty(SerializedProperty dest, SerializedProperty src)
            {
                dest.FindPropertyRelative("targetPath").stringValue = src.FindPropertyRelative("targetPath").stringValue;
                dest.FindPropertyRelative("targetProperty").stringValue = src.FindPropertyRelative("targetProperty").stringValue;
                dest.FindPropertyRelative("targetPropertyType").stringValue = src.FindPropertyRelative("targetPropertyType").stringValue;
                dest.FindPropertyRelative("valueType").stringValue = src.FindPropertyRelative("valueType").stringValue;
                dest.FindPropertyRelative("isDiscrete").boolValue = src.FindPropertyRelative("isDiscrete").boolValue;
                dest.FindPropertyRelative("isPPtr").boolValue = src.FindPropertyRelative("isPPtr").boolValue;
                dest.FindPropertyRelative("objectValue").objectReferenceValue = src.FindPropertyRelative("objectValue").objectReferenceValue;
                dest.FindPropertyRelative("floatValue").floatValue = src.FindPropertyRelative("floatValue").floatValue;
            }

            public static EditorCurveBinding GetBinding(SerializedProperty property)
            {
                SerializedProperty targetPath = property.FindPropertyRelative("targetPath");
                SerializedProperty targetProperty = property.FindPropertyRelative("targetProperty");
                SerializedProperty targetPropertyType = property.FindPropertyRelative("targetPropertyType");
                SerializedProperty isDiscrete = property.FindPropertyRelative("isDiscrete");
                SerializedProperty isPPtr = property.FindPropertyRelative("isPPtr");
                if (isPPtr.boolValue)
                    return EditorCurveBinding.PPtrCurve(targetPath.stringValue, EasyReflection.FindType(targetPropertyType.stringValue), targetProperty.stringValue);
                else if (isDiscrete.boolValue)
                    return EditorCurveBinding.DiscreteCurve(targetPath.stringValue, EasyReflection.FindType(targetPropertyType.stringValue), targetProperty.stringValue);
                return EditorCurveBinding.FloatCurve(targetPath.stringValue, EasyReflection.FindType(targetPropertyType.stringValue), targetProperty.stringValue);
            }


            public static Vector3 GetEulerAngles(SerializedProperty propertyGroup)
            {
                Dictionary<string, SerializedProperty> rotationMap = new Dictionary<string, SerializedProperty>();
                for (int i = 0; i < propertyGroup.arraySize; i++)
                {
                    SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i);
                    rotationMap.Add(GetPropertyGroupSubname(property), property.FindPropertyRelative("floatValue"));
                }

                return new Vector3(rotationMap["x"].floatValue, rotationMap["y"].floatValue, rotationMap["z"].floatValue);
            }

            public static Type GetValueType(SerializedProperty property)
            {
                string valueTypeName = property.FindPropertyRelative("valueType").stringValue;
                return valueTypeName == "" ? null : EasyReflection.FindType(valueTypeName);
            }

            public static bool CheckProperty(GameObject avatar, SerializedProperty property)
            {

                if (!avatar || property.FindPropertyRelative("valueType").stringValue == "" || property.FindPropertyRelative("targetProperty").stringValue == "")
                    return false;
                return AnimationUtility.GetEditorCurveValueType(avatar, GetBinding(property)) != null;
            }

            public static void ClearPropertyGroup(SerializedProperty propertyGroup)
            {
                string targetPath = propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("targetPath").stringValue;
                propertyGroup.ClearArray();
                propertyGroup.arraySize++;
                propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("targetPath").stringValue = targetPath;


            }
            public static void PropertyGroupEdit(SerializedProperty propertyGroup, string relativePath, string value)
            {
                for (int i = 0; i < propertyGroup.arraySize; i++)
                {
                    SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i).FindPropertyRelative(relativePath);
                    property.stringValue = value;
                }
            }

            public static void PropertyGroupEdit(SerializedProperty propertyGroup, string relativePath, bool value)
            {
                for (int i = 0; i < propertyGroup.arraySize; i++)
                {
                    SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i).FindPropertyRelative(relativePath);
                    property.boolValue = value;
                }
            }

            public static string NicifyPropertyGroupName(Type animatableObjectType, string propertyGroupName)
            {

                return (string)EasyReflection.internalNicifyPropertyGroupName.Invoke(null, new object[] { animatableObjectType, propertyGroupName });
            }

            public static string GetPropertyGroupSubname(SerializedProperty propertyGroup, int index)
            {
                return GetPropertyGroupSubname(propertyGroup.GetArrayElementAtIndex(index));
            }

            public static string GetPropertyGroupSubname(SerializedProperty property)
            {
                string targetProperty = property.FindPropertyRelative("targetProperty").stringValue;
                return targetProperty.Substring(targetProperty.Length - 1);
            }

            public static bool PropertyGroupIsColor(SerializedProperty propertyGroup)
            {
                string targetProperty = propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("targetProperty").stringValue;

                return targetProperty.ToLower().Contains("color") && propertyGroup.arraySize == 4;
            }

            public static bool PropertyGroupIsBlendShape(SerializedProperty propertyGroup)
            {
                SerializedProperty property = propertyGroup.GetArrayElementAtIndex(0);
                string targetProperty = property.FindPropertyRelative("targetProperty").stringValue;
                string targetPropertyType = property.FindPropertyRelative("targetPropertyType").stringValue;
                return targetPropertyType.Contains("SkinnedMeshRenderer") && targetProperty.Contains("blendShape") && propertyGroup.arraySize == 1;
            }
        }
        #endregion
    }

}

