using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace EasyAvatar
{
    
    public class EasyAvatarBuilder
    {
        public static string workingDirectory = "Assets/EasyAvatar3.0/";
        string rootBuildDir, menuBuildDir, animBuildDir, templateDir;
        EasyAvatarHelper helper;
        EasyAnimator easyAnimator;
        int controlCount;

        public EasyAvatarBuilder(EasyAvatarHelper helper)
        {
            this.helper = helper;
            rootBuildDir = workingDirectory + "Build/" + Utility.GetGoodFileName(helper.avatar.name);
            templateDir = workingDirectory + "Template/";
            menuBuildDir = rootBuildDir + "/Menu/";
            animBuildDir = rootBuildDir + "/Anim/";
            controlCount = 0;
        }


        /// <summary>
        /// 构建所有
        /// </summary>
        public void Build()
        {
            VRCAvatarDescriptor avatarDescriptor = helper.avatar.GetComponent<VRCAvatarDescriptor>();
            EasyMenu mainMenu = null;
            EasyGestureManager gestureManager = null;
            EasyLocomotionManager locomotionManager = null;

            foreach (Transform child in helper.gameObject.transform)
            {
                EasyMenu tempMenu = child.GetComponent<EasyMenu>();
                EasyGestureManager tempGestureManager = child.GetComponent<EasyGestureManager>();
                EasyLocomotionManager tempLocomotionManager = child.GetComponent<EasyLocomotionManager>();
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
                    if (gestureManager)//检测是否有多个手势管理
                    {
                        EditorUtility.DisplayDialog("Error", Lang.ErrAvatarGestureManagerLen1, "ok");
                        return;
                    }
                    gestureManager = tempGestureManager;
                }

                if (tempLocomotionManager)
                {
                    if (locomotionManager)//检测是否有多个姿态管理
                    {
                        EditorUtility.DisplayDialog("Error", Lang.ErrAvatarLocomotionManagerLen1, "ok");
                        return;
                    }
                    locomotionManager = tempLocomotionManager;
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
            easyAnimator = new EasyAnimator(animBuildDir, helper.avatar ,AssetDatabase.LoadAssetAtPath<AnimatorController>(templateDir+ "LocomotionLayer.controller"));


            //构建手势
            if (gestureManager)
                BuildGestures(gestureManager);

            //设置姿态
            if (locomotionManager)
                easyAnimator.SetLocomotion(locomotionManager);

            if (mainMenu)
            {
                controlCount = 0;
                //构建菜单
                VRCExpressionsMenu VRCMenu = BuildMenu(mainMenu, "Menu");

                //构建VRCExpressionParameters
                List<VRCExpressionParameters.Parameter> parameters = new List<VRCExpressionParameters.Parameter>();
                VRCExpressionParameters expressionParameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                parameters.Add(new VRCExpressionParameters.Parameter() { name = "driver", valueType = VRCExpressionParameters.ValueType.Int, saved = false });
                parameters.Add(new VRCExpressionParameters.Parameter() { name = "float1", valueType = VRCExpressionParameters.ValueType.Float, saved = false });
                parameters.Add(new VRCExpressionParameters.Parameter() { name = "float2", valueType = VRCExpressionParameters.ValueType.Float, saved = false });
                for (int i = 0; i < controlCount; i++)
                    parameters.Add(new VRCExpressionParameters.Parameter() { name = "control" + (i + 1), valueType = VRCExpressionParameters.ValueType.Bool, saved = false });
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
                    new CustomAnimLayer(){type = AnimLayerType.Base ,isDefault = false, animatorController = easyAnimator.locomotionController},
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
            if (EasyAvatarMenuItem.GetMenuItemCount(menu.gameObject.transform) > 8)
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

                    switch (control.type)
                    {
                        case EasyControl.Type.Toggle:
                            vrcControl.type = VRCExpressionsMenu.Control.ControlType.Toggle;
                            BuildToggle(prefix + "_" + count + "_" + control.name, control);
                            break;
                        case EasyControl.Type.Button:
                            vrcControl.type = VRCExpressionsMenu.Control.ControlType.Button;
                            BuildToggle(prefix + "_" + count + "_" + control.name, control);
                            break;
                        case EasyControl.Type.RadialPuppet:
                            vrcControl.type = VRCExpressionsMenu.Control.ControlType.RadialPuppet;
                            vrcControl.subParameters = new VRCExpressionsMenu.Control.Parameter[] { new VRCExpressionsMenu.Control.Parameter() { name = "float1" } };
                            BuildRadialPuppet(prefix + "_" + count + "_" + control.name, control);
                            break;
                        case EasyControl.Type.TwoAxisPuppet:
                            vrcControl.type = VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet;
                            vrcControl.subParameters = new VRCExpressionsMenu.Control.Parameter[] { new VRCExpressionsMenu.Control.Parameter() { name = "float1" }, new VRCExpressionsMenu.Control.Parameter() { name = "float2" } };
                            BuildTwoAxisPuppet(prefix + "_" + count + "_" + control.name, control);
                            break;
                        default:
                            break;
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
        private void BuildGestures(EasyGestureManager gestures)
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
            AnimationClip outClip = Utility.GenerateAnimClip(gesture.behaviors1.list);
            AnimationClip onClip = Utility.GenerateAnimClip(gesture.behaviors2.list);
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
            if (control.behaviors.Count < 2)
                return;
            //EasyBehaviors生成动画
            AnimationClip offClip = Utility.GenerateAnimClip(control.behaviors[0].list);
            AnimationClip onClip = Utility.GenerateAnimClip(control.behaviors[1].list);
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
            if (control.behaviors.Count < 3)
                return;
            //EasyBehaviors生成动画
            AnimationClip offClip = Utility.GenerateAnimClip(control.behaviors[0].list);
            AnimationClip blend0 = Utility.GenerateAnimClip(control.behaviors[1].list);
            AnimationClip blend1 = Utility.GenerateAnimClip(control.behaviors[2].list);
            BlendTree blendTree = Utility.Generate1DBlendTree(name, "float1", blend0, blend1);
            if (control.autoTrackingControl)
                easyAnimator.AddState(name, offClip, blendTree, control.autoRestore, "control" + controlCount);
            else
                easyAnimator.AddState(name, offClip, blendTree, control.offTrackingControl, control.onTrackingControl, control.autoRestore, "control" + controlCount);
        }
        /// <summary>
        /// 构建操纵杆控件
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="control">控件</param>
        private void BuildTwoAxisPuppet(string name, EasyControl control)
        {
            if (control.behaviors.Count < 1)
                return;
            List<EasyBehaviorGroup> behaviorGroups = control.behaviors;
            AnimationClip offClip = Utility.GenerateAnimClip(behaviorGroups[0].list);
            List<Vector2> positions = new List<Vector2>();
            List<Motion> motions = new List<Motion>();
            for (int i = 1; i < behaviorGroups.Count; i++)
            {
                EasyBehaviorGroup behaviorGroup = behaviorGroups[i];
                positions.Add(behaviorGroup.position);
                motions.Add(Utility.GenerateAnimClip(behaviorGroup.list));
            }
            BlendTree blendTree = Utility.Generate2DBlendTree(name, "float1", "float2", positions, motions);
            if (control.autoTrackingControl)
                easyAnimator.AddState(name, offClip, blendTree, control.autoRestore, "control" + controlCount);
            else
                easyAnimator.AddState(name, offClip, blendTree, control.offTrackingControl, control.onTrackingControl, control.autoRestore, "control" + controlCount);
        }

    }

}
