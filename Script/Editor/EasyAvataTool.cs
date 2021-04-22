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

        public static string workingDirectory = "Assets/EasyAvatar3.0/";

        /// <summary>
        /// 创建AvatarHelper
        /// </summary>
        /// <returns>是否成功</returns>
        [MenuItem("GameObject/EasyAvatar3.0/Avatar Helper", priority = 0)]
        public static bool CreateAvatarHelper()
        {
            CreateObject<EasyAvatarHelper>(Lang.AvatarHelper);
            return true;
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <returns>是否成功</returns>
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
                if (!CreateAvatarHelper())
                    return false;
            CreateObject<EasyMenu>(isSubMenu ? Lang.SubMenu : Lang.MainMenu);
            return true;
        }

        /// <summary>
        /// 创建控件
        /// </summary>
        /// <returns>是否成功</returns>
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
            CreateObject<EasyControl>(Lang.Control);
            return true;
        }

        /// <summary>
        /// 创建手势管理
        /// </summary>
        /// <returns></returns>
        [MenuItem("GameObject/EasyAvatar3.0/Gesture Manager", priority = 0)]
        public static bool CreateGestureManager()
        {
            if (Selection.activeGameObject)
            {
                if (!Selection.activeGameObject.transform.GetComponent<EasyAvatarHelper>())
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrGestureMenuNotInHelper, "ok");
                    return false;
                }
            }
            else
            {
                if (!CreateAvatarHelper())
                    return false;
            }
            GameObject gameObject = CreateObject<EasyGestureManager>(Lang.GestureManager);
            EasyGestureManagerEditor.SetDefaultGesture(new SerializedObject(gameObject.GetComponent<EasyGestureManager>()));
            return true;
        }

        /// <summary>
        /// 创建手势
        /// </summary>
        /// <returns></returns>
        [MenuItem("GameObject/EasyAvatar3.0/Gesture", priority = 0)]
        public static bool CreateGesture()
        {
            if (Selection.activeGameObject)
            {
                if (!Selection.activeGameObject.transform.GetComponent<EasyGestureManager>())
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrGestureNotInGestureManager, "ok");
                    return false;
                }
            }
            else
            {
                if (!CreateGestureManager())
                    return false;
            }
            CreateObject<EasyGesture>(Lang.Gesture);
            return true;
        }

        /// <summary>
        /// 显示关于对话框
        /// </summary>
        [MenuItem("EasyAvatar3.0/About", priority = 0)]
        public static void showAbout()
        {
            EditorUtility.DisplayDialog("About", Lang.About, "ok");
            Debug.Log(Lang.About);
        }

        /// <summary>
        /// 通过组件创建GameObject
        /// </summary>
        /// <typeparam name="T">组件</typeparam>
        /// <param name="name">新建物体的名字</param>
        public static GameObject CreateObject<T>(string name) where T : Component
        {
            GameObject gameObject = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create "+ name);
            gameObject.AddComponent<T>();
            if (Selection.activeGameObject)
                gameObject.transform.parent = Selection.activeGameObject.transform;
            Selection.activeGameObject = gameObject;
            return gameObject;
        }

        /// <summary>
        /// 获取菜单下项目的数量
        /// </summary>
        /// <param name="transform">查询的菜单</param>
        /// <returns>数量</returns>
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

        /// <summary>
        /// transform下获取菜单的数量
        /// </summary>
        /// <param name="transform">要查询的transform</param>
        /// <returns>数量</returns>
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
            string rootBuildDir, menuBuildDir, animBuildDir;
            EasyAvatarHelper helper;
            EasyAnimator easyAnimator;
            int controlCount;

            public Builder(EasyAvatarHelper helper)
            {
                this.helper = helper;
                rootBuildDir = workingDirectory + "Build/" + Utility.GetGoodFileName(helper.avatar.name);
                menuBuildDir = rootBuildDir + "/Menu/";
                animBuildDir = rootBuildDir + "/Anim/";
                controlCount = 0;
            }
            

            /// <summary>
            /// 构建所有
            /// </summary>
            /// <param name="helper">AvatarHelper</param>
            public void Build()
            {
                VRCAvatarDescriptor avatarDescriptor = helper.avatar.GetComponent<VRCAvatarDescriptor>();
                EasyMenu mainMenu = null;
                EasyGestureManager gestureManager = null;
                
                foreach (Transform child in helper.gameObject.transform)
                {
                    EasyMenu tempMenu = child.GetComponent<EasyMenu>();
                    EasyGestureManager tempGestureManager = child.GetComponent<EasyGestureManager>();
                    if (tempMenu)
                    {
                        if (mainMenu)//检测是否有多个主菜单
                        {
                            EditorUtility.DisplayDialog("Error", Lang.ErrAvatarMenuLen1, "ok");
                            return;
                        }
                        mainMenu = tempMenu;
                    }

                    if (tempGestureManager)
                    {
                        if (gestureManager)//检测是否有多个手势菜单
                        {
                            EditorUtility.DisplayDialog("Error", Lang.ErrAvatarGestureMenuLen1, "ok");
                            return;
                        }
                        gestureManager = tempGestureManager;
                    }
                }
                
                //清除目录
                if (Directory.Exists(rootBuildDir))
                {
                    Directory.Delete(rootBuildDir, true);
                }
                Directory.CreateDirectory(rootBuildDir);
                Directory.CreateDirectory(menuBuildDir);
                Directory.CreateDirectory(animBuildDir);

                //初始化EasyAnimator
                easyAnimator = new EasyAnimator(animBuildDir, helper.avatar);
                

                //构建手势
                if (gestureManager)
                    BuildGestures(gestureManager);

                if (mainMenu)
                {
                    controlCount = 0;
                    //构建菜单
                    VRCExpressionsMenu VRCMenu = BuildMenu(mainMenu, "Menu");

                    //构建VRCExpressionParameters
                    List<VRCExpressionParameters.Parameter> parameters = new List<VRCExpressionParameters.Parameter>();
                    VRCExpressionParameters expressionParameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                    parameters.Add(new VRCExpressionParameters.Parameter() { name = "driver", valueType = VRCExpressionParameters.ValueType.Int });
                    parameters.Add(new VRCExpressionParameters.Parameter() { name = "float1", valueType = VRCExpressionParameters.ValueType.Float });
                    parameters.Add(new VRCExpressionParameters.Parameter() { name = "float2", valueType = VRCExpressionParameters.ValueType.Float });
                    for (int i = 0; i < controlCount; i++)
                        parameters.Add(new VRCExpressionParameters.Parameter() { name = "control" + (i + 1), valueType = VRCExpressionParameters.ValueType.Bool });
                    expressionParameters.parameters = parameters.ToArray();
                    AssetDatabase.CreateAsset(expressionParameters, menuBuildDir + "Parameters.asset");

                    avatarDescriptor.customExpressions = true;
                    avatarDescriptor.expressionParameters = expressionParameters;
                    avatarDescriptor.expressionsMenu = VRCMenu;
                }
                else
                {
                    avatarDescriptor.customExpressions = false;
                    avatarDescriptor.expressionParameters = null;
                    avatarDescriptor.expressionsMenu = null;
                }

                //构建EasyAnimator
                easyAnimator.Build();

                //设置CustomizeAnimationLayers
                avatarDescriptor.customizeAnimationLayers = true;
                avatarDescriptor.baseAnimationLayers = new CustomAnimLayer[]{
                    new CustomAnimLayer(){type = AnimLayerType.Base ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Additive ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Gesture ,isDefault = false,animatorController = easyAnimator.gestureController},
                    new CustomAnimLayer(){type = AnimLayerType.Action ,isDefault = false , animatorController = easyAnimator.actionController},
                    new CustomAnimLayer(){type = AnimLayerType.FX ,isDefault = false,animatorController = easyAnimator.fxController}
                };

                //保存
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Info", Lang.BuildSucceed, "ok");
            }

            /// <summary>
            /// 构建菜单
            /// </summary>
            /// <param name="menu">根菜单</param>
            /// <param name="prefix">菜单名字累加前缀</param>
            /// <returns>vrc菜单</returns>
            private VRCExpressionsMenu BuildMenu(EasyMenu menu, string prefix)
            {
                if (GetMenuItemCount(menu.gameObject.transform) > 8)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuItemLen8, "ok");
                    return null;
                }

                VRCExpressionsMenu expressionsMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                expressionsMenu.controls = new List<VRCExpressionsMenu.Control>();

                int count = 0;
                foreach (Transform child in menu.gameObject.transform)
                {
                    count++;
                    EasyMenu subMenu = child.GetComponent<EasyMenu>();
                    EasyControl control = child.GetComponent<EasyControl>();
                    if (control)
                    {
                        controlCount++;
                        VRCExpressionsMenu.Control vrcControl = new VRCExpressionsMenu.Control();
                        vrcControl.name = control.name;
                        vrcControl.icon = control.icon;
                        vrcControl.parameter = new VRCExpressionsMenu.Control.Parameter() { name = "control" + controlCount };
                        if (control.type == EasyControl.Type.Toggle)
                        {
                            vrcControl.type = VRCExpressionsMenu.Control.ControlType.Toggle;
                            BuildToggle(prefix + "_" + count + "_" + control.name, control);
                        }
                        else if(control.type == EasyControl.Type.RadialPuppet)
                        {
                            vrcControl.type = VRCExpressionsMenu.Control.ControlType.RadialPuppet;
                            vrcControl.subParameters = new VRCExpressionsMenu.Control.Parameter[] { new VRCExpressionsMenu.Control.Parameter() { name = "float1"} };
                            BuildRadialPuppet(prefix + "_" + count + "_" + control.name, control);
                        }

                        expressionsMenu.controls.Add(vrcControl);
                    }
                    else if (subMenu)
                    {

                        VRCExpressionsMenu.Control vrcControl = new VRCExpressionsMenu.Control();
                        vrcControl.name = subMenu.name;
                        vrcControl.type = VRCExpressionsMenu.Control.ControlType.SubMenu;
                        vrcControl.subMenu = BuildMenu(subMenu, prefix + "_" + count + "_" + subMenu.name);
                        expressionsMenu.controls.Add(vrcControl);
                    }
                }
                AssetDatabase.CreateAsset(expressionsMenu, menuBuildDir + prefix + ".asset");

                return expressionsMenu;
            }

            /// <summary>
            /// 构建所有手势
            /// </summary>
            /// <param name="gestures">手势管理</param>
            private void BuildGestures( EasyGestureManager gestures)
            {
                easyAnimator.SetGestureAnimation("Neutral", gestures.leftNeutral, false, 0);
                easyAnimator.SetGestureAnimation("Fist", gestures.leftFist, false, 1);
                easyAnimator.SetGestureAnimation("HandOpen", gestures.leftHandOpen, false, 2);
                easyAnimator.SetGestureAnimation("FingerPoint", gestures.leftFingerPoint, false, 3);
                easyAnimator.SetGestureAnimation("Victory", gestures.leftVictory, false, 4);
                easyAnimator.SetGestureAnimation("RockNRoll", gestures.leftRockNRoll, false, 5);
                easyAnimator.SetGestureAnimation("HandGun", gestures.leftHandGun, false, 6);
                easyAnimator.SetGestureAnimation("ThumbsUp", gestures.leftThumbsUp, false, 7);
                easyAnimator.SetGestureAnimation("Neutral", gestures.rightNeutral, true, 0);
                easyAnimator.SetGestureAnimation("Fist", gestures.rightFist, true, 1);
                easyAnimator.SetGestureAnimation("HandOpen", gestures.rightHandOpen, true, 2);
                easyAnimator.SetGestureAnimation("FingerPoint", gestures.rightFingerPoint, true, 3);
                easyAnimator.SetGestureAnimation("Victory", gestures.rightVictory, true, 4);
                easyAnimator.SetGestureAnimation("RockNRoll", gestures.rightRockNRoll, true, 5);
                easyAnimator.SetGestureAnimation("HandGun", gestures.rightHandGun, true, 6);
                easyAnimator.SetGestureAnimation("ThumbsUp", gestures.rightThumbsUp, true, 7);

                int count = 0;
                foreach (Transform child in gestures.gameObject.transform)
                {
                    EasyGesture gesture = child.GetComponent<EasyGesture>();
                    if (!gesture)
                        continue;
                    count++;
                    string name = "Gesture_" + count + "_" + gesture.name;
                    BuildGesture(name, gesture);
                }
            }

            /// <summary>
            /// 构建手势
            /// </summary>
            /// <param name="name">名字</param>
            /// <param name="gesture">手势</param>
            private void BuildGesture(string name, EasyGesture gesture)
            {
                AnimationClip onClip = Utility.GenerateAnimClip(gesture.behaviors1.list);
                AnimationClip outClip = Utility.GenerateAnimClip(gesture.behaviors2.list);
                if (gesture.handType == EasyGesture.HandType.Left || gesture.handType == EasyGesture.HandType.Any)
                {
                    if (gesture.autoTrackingControl)
                        easyAnimator.AddState(name + "L", outClip, onClip, gesture.autoRestore, "GestureLeft", (int)gesture.gestureType);
                    else
                        easyAnimator.AddState(name + "L", outClip, onClip, gesture.offTrackingControl, gesture.onTrackingControl, gesture.autoRestore, "GestureLeft", (int)gesture.gestureType);
                }

                if (gesture.handType == EasyGesture.HandType.Right || gesture.handType == EasyGesture.HandType.Any)
                {
                    if (gesture.autoTrackingControl)
                        easyAnimator.AddState(name + "R", outClip, onClip, gesture.autoRestore, "GestureRight", (int)gesture.gestureType);
                    else
                        easyAnimator.AddState(name + "R", outClip, onClip, gesture.offTrackingControl, gesture.onTrackingControl, gesture.autoRestore, "GestureRight", (int)gesture.gestureType);
                }
            }

            /// <summary>
            /// 构建开关控件
            /// </summary>
            /// <param name="name">名字</param>
            /// <param name="control">控件</param>
            private void BuildToggle(string name, EasyControl control)
            {
                //EasyBehaviors生成动画
                AnimationClip offClip = Utility.GenerateAnimClip(control.behaviors[1].list);
                AnimationClip onClip = Utility.GenerateAnimClip(control.behaviors[0].list);
                if (control.autoTrackingControl)
                    easyAnimator.AddState(name, offClip, onClip, control.autoRestore, "control" + controlCount);
                else
                    easyAnimator.AddState(name, offClip, onClip, control.offTrackingControl, control.onTrackingControl, control.autoRestore, "control" + controlCount);
            }
            
            /// <summary>
            /// 构建旋钮控件
            /// </summary>
            /// <param name="name">名字</param>
            /// <param name="control">控件</param>
            private void BuildRadialPuppet(string name, EasyControl control)
            {

                //EasyBehaviors生成动画
                AnimationClip offClip = Utility.GenerateAnimClip(control.behaviors[2].list);
                AnimationClip blend0 = Utility.GenerateAnimClip(control.behaviors[0].list);
                AnimationClip blend1 = Utility.GenerateAnimClip(control.behaviors[1].list);
                if (control.autoTrackingControl)
                    easyAnimator.AddState(name, offClip, blend0, blend1, control.autoRestore, "control" + controlCount);
                else
                    easyAnimator.AddState(name, offClip, blend0, blend1, control.offTrackingControl, control.onTrackingControl, control.autoRestore, "control" + controlCount);
            }

        }
        #endregion
        
    }

}

