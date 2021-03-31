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
            CreateObject<EasyGestureManager>(Lang.GestureManager);
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
        public static void CreateObject<T>(string name) where T : Component
        {
            GameObject gameObject = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create "+ name);
            gameObject.AddComponent<T>();
            if (Selection.activeGameObject)
                gameObject.transform.parent = Selection.activeGameObject.transform;
            Selection.activeGameObject = gameObject;
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

        class AnimatorControllerBuilder
        {
            AnimatorController controller;
            AnimatorControllerLayer baseLayer;

            Dictionary<int, Motion> drivedMotions;
            Dictionary<int, StateMachineBehaviour> drivedStateBehaviour;
            Dictionary<int, string> stateNames;
            Dictionary<string, AnimatorControllerLayer> layers;

            public AnimatorControllerBuilder(string path)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(path); ;
                baseLayer = controller.layers[0];
                drivedMotions = new Dictionary<int, Motion>();
                stateNames = new Dictionary<int, string>();
                layers = new Dictionary<string, AnimatorControllerLayer>();
            }

            public void AddDrivedState(int driverId, string name, Motion motion)
            {
                if (drivedMotions.ContainsKey(driverId))
                {
                    if (motion is AnimationClip)
                    {
                        AnimationClip clip = drivedMotions[driverId] as AnimationClip;
                        drivedMotions[driverId] = clip == null ? motion : Utility.MergeAnimClip(clip, motion as AnimationClip);
                    }
                    else
                        drivedMotions[driverId] = motion;
                }
                else
                    drivedMotions.Add(driverId, motion);
                stateNames.Add(driverId, name);
            }

            public void AddLayer(string name)
            {
                AnimatorControllerLayer newLayer = new AnimatorControllerLayer();
                newLayer.name = name;
                newLayer.defaultWeight = 1;
                AnimatorStateMachine stateMachine = new AnimatorStateMachine();
                stateMachine.name = name;
                stateMachine.hideFlags = HideFlags.HideInHierarchy;
                //不加这个unity重启后新加的层里的内容会消失
                AssetDatabase.AddObjectToAsset(stateMachine, AssetDatabase.GetAssetPath(controller));
                newLayer.stateMachine = stateMachine;
                //必须在设置stateMachine再添加层
                controller.AddLayer(newLayer);
                layers.Add(name, newLayer);
            }

            public void AddParameterBool(string name)
            {
                controller.AddParameter(name, AnimatorControllerParameterType.Bool);
            }

            public void AddParameterInt(string name)
            {
                controller.AddParameter(name, AnimatorControllerParameterType.Int);
            }

            public void AddParameterFloat(string name)
            {
                controller.AddParameter(name, AnimatorControllerParameterType.Float);
            }

            public static bool CheckControllerParameter(AnimatorController controller, string paramaName)
            {
                if (controller.parameters == null)
                    return false;
                foreach (AnimatorControllerParameter parameter in controller.parameters)
                {
                    if (parameter.name == paramaName)
                        return true;
                }
                return false;
            }

            public static AnimatorState FindState(AnimatorStateMachine stateMachine, string name)
            {
                foreach (var childState in stateMachine.states)
                {
                    if (name == childState.state.name)
                        return childState.state;
                }
                return null;
            }

            public AnimatorControllerLayer FindLayer(string name)
            {
                AnimatorControllerLayer layer = null;
                layers.TryGetValue(name, out layer);
                return layer;
            }

            public virtual  void BuildDrivedState(int driverId)
            {

            }

            public void Build()
            {
                foreach (int driverId in drivedMotions.Keys)
                    BuildDrivedState(driverId);
            }
        }

        public class Builder
        {
            static int controlCount;
            static string rootBuildDir, menuBuildDir, animBuildDir;
            static GameObject avatar;
            static AnimatorController controllerFx, controllerAction;
            static AnimationClip fxInitClip;
            static Dictionary<KeyValuePair<string, int>, int> drivers;
            

            /// <summary>
            /// 构建所有
            /// </summary>
            /// <param name="helper">AvatarHelper</param>
            public static void Build(EasyAvatarHelper helper)
            {
                controlCount = 0;
                avatar = helper.avatar;
                rootBuildDir = workingDirectory + "Build/" + Utility.GetGoodFileName(avatar.name);
                menuBuildDir = rootBuildDir + "/Menu/";
                animBuildDir = rootBuildDir + "/Anim/";
                drivers = new Dictionary<KeyValuePair<string, int>, int>();
                fxInitClip = new AnimationClip();

                VRCAvatarDescriptor avatarDescriptor = avatar.GetComponent<VRCAvatarDescriptor>();
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

                //初始化AnimatorController
                controllerFx = AnimatorController.CreateAnimatorControllerAtPath(animBuildDir + "FXLayer.controller");
                controllerFx.AddParameter("driver", AnimatorControllerParameterType.Int);
                controllerFx.AddParameter("float1", AnimatorControllerParameterType.Float);
                controllerFx.AddParameter("float2", AnimatorControllerParameterType.Float);
                AssetDatabase.CopyAsset(workingDirectory + "Res/TemplateActionLayer.controller", animBuildDir + "ActionLayer.controller");
                controllerAction = AssetDatabase.LoadAssetAtPath<AnimatorController>(animBuildDir + "ActionLayer.controller");
                controllerAction.AddParameter("driver", AnimatorControllerParameterType.Int);
                controllerAction.AddParameter("float1", AnimatorControllerParameterType.Float);
                controllerAction.AddParameter("float2", AnimatorControllerParameterType.Float);

                //构建手势
                if (gestureManager)
                    BuildGestures(gestureManager);

                if (mainMenu)
                {
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

                //构建FX的初始状态
                BuildFxInitState(fxInitClip);

                //设置CustomizeAnimationLayers
                avatarDescriptor.customizeAnimationLayers = true;
                avatarDescriptor.baseAnimationLayers = new CustomAnimLayer[]{
                    new CustomAnimLayer(){type = AnimLayerType.Base ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Additive ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Gesture ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Action ,isDefault = false , animatorController = controllerAction },
                    new CustomAnimLayer(){type = AnimLayerType.FX ,isDefault = false,animatorController = controllerFx},
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
            private static VRCExpressionsMenu BuildMenu(EasyMenu menu, string prefix)
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
            private static void BuildGestures( EasyGestureManager gestures)
            {
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
            private static void BuildGesture(string name, EasyGesture gesture)
            {
                AnimationClip animationClip = Utility.GenerateAnimClip(gesture.behaviors);
                if (gesture.useAnimClip)
                {
                    animationClip = Utility.MergeAnimClip(Utility.MergeAnimClip(gesture.animations.ToArray()), animationClip);
                }

                AnimationClip[] separatedClips = Utility.SeparateHumanAnimation(animationClip);
                AnimationClip actionAnim = separatedClips[0];
                AnimationClip fxAnim = separatedClips[1];
                bool hasActionAnim = AnimationUtility.GetCurveBindings(actionAnim).Length > 0;
                AssetDatabase.CreateAsset(fxAnim, animBuildDir + name + "_fx.anim");
                if (hasActionAnim)
                    AssetDatabase.CreateAsset(actionAnim, animBuildDir + name + "_action.anim");

                if (gesture.handType == EasyGesture.HandType.Left || gesture.handType == EasyGesture.HandType.Any)
                {
                    int driverId = BuildDriver("GestureLeft", (int)gesture.gestureType);
                    BuildFxState(name + "_L", driverId, fxAnim);
                    if (hasActionAnim)
                        BuildActionState(name + "_L", driverId, actionAnim);
                }

                if (gesture.handType == EasyGesture.HandType.Right || gesture.handType == EasyGesture.HandType.Any)
                {
                    int driverId = BuildDriver("GestureRight", (int)gesture.gestureType);
                    BuildFxState(name + "_R", driverId, fxAnim);
                    if (hasActionAnim)
                        BuildActionState(name + "_R", driverId, actionAnim);
                }
                if (gesture.gestureType == EasyGesture.GestureType.Neutral)
                {
                    AddFxInitClip(fxAnim);
                }
            }

            /// <summary>
            /// 构建开关控件
            /// </summary>
            /// <param name="name">名字</param>
            /// <param name="control">控件</param>
            private static void BuildToggle(string name, EasyControl control)
            {
                //EasyBehaviors生成动画
                AnimationClip offClip = Utility.GenerateAnimClip(control.behaviors1);
                AnimationClip onClip = Utility.GenerateAnimClip(control.behaviors2);
                //使用动画文件
                if (control.useAnimClip)
                {
                    //先将动画文件合并，在与Behaviors生成动画合并
                    offClip = Utility.MergeAnimClip(Utility.MergeAnimClip(control.anims1.ToArray()), offClip);
                    onClip = Utility.MergeAnimClip(Utility.MergeAnimClip(control.anims2.ToArray()), onClip);
                }
                //分离action动画
                AnimationClip[] offClips = Utility.SeparateHumanAnimation(offClip);
                AnimationClip[] onClips = Utility.SeparateHumanAnimation(onClip);
                AnimationClip offFx = offClips[1];
                AnimationClip onAction = onClips[0];
                AnimationClip onFx = onClips[1];
                AddFxInitClip(offFx);

                AssetDatabase.CreateAsset(offFx, animBuildDir + name + "_off_fx.anim");
                AssetDatabase.CreateAsset(onFx, animBuildDir + name + "_on_fx.anim");
                //生成驱动id
                int driverId = BuildDriver("control" + controlCount);
                //通过驱动id到对应状态
                BuildFxState(name + "_on", driverId, onFx);
                BuildFxState(name + "_off", driverId + 1, offFx);

                //有action动画才加进去
                if (AnimationUtility.GetCurveBindings(onAction).Length > 0)
                {
                    AssetDatabase.CreateAsset(onAction, animBuildDir + name + "_on_action.anim");
                    BuildActionState(name,driverId, onAction);
                }
            }
            
            /// <summary>
            /// 构建旋钮控件
            /// </summary>
            /// <param name="name">名字</param>
            /// <param name="control">控件</param>
            private static void BuildRadialPuppet(string name, EasyControl control)
            {

                //EasyBehaviors生成动画
                AnimationClip offClip = Utility.GenerateAnimClip(control.behaviors1);
                AnimationClip onClip = Utility.GenerateAnimClip(control.behaviors2);
                //使用动画文件
                if (control.useAnimClip)
                {
                    //先将动画文件合并，在与Behaviors生成动画合并
                    offClip = Utility.MergeAnimClip(Utility.MergeAnimClip(control.anims1.ToArray()), offClip);
                    onClip = Utility.MergeAnimClip(Utility.MergeAnimClip(control.anims2.ToArray()), onClip);
                }
                //分离action动画
                AnimationClip[] offClips = Utility.SeparateHumanAnimation(offClip);
                AnimationClip[] onClips = Utility.SeparateHumanAnimation(onClip);
                AnimationClip offFx = offClips[1];
                AnimationClip onFx = onClips[1];
                AnimationClip offAction = offClips[0];
                AnimationClip onAction = onClips[0];

                BlendTree fxBlendTree = Build1DBlendTree(name + "_on_fx", "float1", offFx, onFx);
                AssetDatabase.AddObjectToAsset(fxBlendTree, AssetDatabase.GetAssetPath(controllerFx));
                AssetDatabase.CreateAsset(offFx, animBuildDir + name + "_off_fx.anim");
                AssetDatabase.CreateAsset(onFx, animBuildDir + name + "_on_fx.anim");
                int driverId = BuildDriver("control" + controlCount);
                BuildFxState(name, driverId, fxBlendTree);

                if (AnimationUtility.GetCurveBindings(offAction).Length > 0 || AnimationUtility.GetCurveBindings(onAction).Length > 0)
                {
                    BlendTree actionBlendTree = Build1DBlendTree(name + "_on_action", "float1", offAction, onAction);
                    AssetDatabase.AddObjectToAsset(actionBlendTree, AssetDatabase.GetAssetPath(controllerAction));
                    AssetDatabase.CreateAsset(offAction, animBuildDir + name + "_off_action.anim");
                    AssetDatabase.CreateAsset(onAction, animBuildDir + name + "_on_action.anim");
                    BuildActionState(name, driverId, actionBlendTree);
                }
            }

            /// <summary>
            /// 为bool参数创建驱动
            /// </summary>
            /// <param name="paramName">参数名</param>
            /// <returns>bool参数为true时的驱动id，fales时为id+1</returns>
            private static int BuildDriver(string paramName)
            {
                //查询drivers里是否已经有driver
                KeyValuePair<string, int> driverKey = new KeyValuePair<string, int>(paramName, 1);
                if (drivers.ContainsKey(driverKey))
                    return drivers[driverKey];
                //每次id是成对的，所以乘2
                int driverCount = drivers.Count;
                int driverId = driverCount*2 +1;
                //添加新的层
                AnimatorControllerLayer fxLayer = new AnimatorControllerLayer();
                AnimatorStateMachine stateMachine = new AnimatorStateMachine();
                fxLayer.name = paramName;
                fxLayer.stateMachine = stateMachine;
                fxLayer.defaultWeight = 1;
                stateMachine.name = paramName;
                stateMachine.hideFlags = HideFlags.HideInHierarchy;
                //不加这个unity重启后新的层里的内容会消失
                AssetDatabase.AddObjectToAsset(fxLayer.stateMachine, AssetDatabase.GetAssetPath(controllerFx));
                controllerFx.AddLayer(fxLayer);
                //开关状态
                AnimatorState stateOff = stateMachine.AddState("off");
                AnimatorState stateOn = stateMachine.AddState("on");
                stateMachine.defaultState = stateOff;
                stateOff.writeDefaultValues = false;
                stateOn.writeDefaultValues = false;
                //VRC参数驱动，paramName的参数为true时driver设为driverId，paramName的参数为false是driver设为driverId+1
                VRCAvatarParameterDriver offDriver = stateOff.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                offDriver.parameters.Add(new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter() { name = "driver", value = driverId + 1, type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set });
                VRCAvatarParameterDriver onDriver = stateOn.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                onDriver.parameters.Add(new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter() { name = "driver", value = driverId, type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set });
                
                if (!Utility.CheckControllerParameter(controllerFx, paramName))
                    controllerFx.AddParameter(paramName,AnimatorControllerParameterType.Bool);
                //状态连线
                AnimatorStateTransition off_on = stateOff.AddTransition(stateOn);
                off_on.duration = 0;
                AnimatorStateTransition on_off = stateOn.AddTransition(stateOff);
                on_off.duration = 0;
                off_on.AddCondition(AnimatorConditionMode.If, 0, paramName);
                on_off.AddCondition(AnimatorConditionMode.IfNot, 0, paramName);
                //加入词典方便查询
                drivers.Add(driverKey, driverId);
                return driverId;
            }

            /// <summary>
            /// 为int参数创建驱动
            /// </summary>
            /// <param name="paramName">参数名</param>
            /// <param name="threshold">参数为何值时的驱动</param>
            /// <returns>驱动id，当参数设置为该值时的驱动id，当参数从该值设置为其他值时驱动id+1</returns>
            private static int BuildDriver(string paramName,int threshold)
            {
                //查询drivers里是否已经有driver
                KeyValuePair<string, int> driverKey = new KeyValuePair<string, int>(paramName, threshold);
                if (drivers.ContainsKey(driverKey))
                    return drivers[driverKey];
                
                int driverCount = drivers.Count;
                int driverId = driverCount * 2 + 1;

                AnimatorControllerLayer fxLayer = Utility.FindLayer(controllerFx,paramName);
                AnimatorStateMachine stateMachine;
                if (fxLayer == null)
                {
                    fxLayer = new AnimatorControllerLayer();
                    fxLayer.name = paramName;
                    fxLayer.defaultWeight = 1;
                    stateMachine = new AnimatorStateMachine();
                    stateMachine.name = paramName;
                    stateMachine.hideFlags = HideFlags.HideInHierarchy;
                    //不加这个unity重启后新加的层里的内容会消失
                    AssetDatabase.AddObjectToAsset(stateMachine, AssetDatabase.GetAssetPath(controllerFx));
                    fxLayer.stateMachine = stateMachine;
                    //必须在设置stateMachine再添加层
                    controllerFx.AddLayer(fxLayer);
                }
                stateMachine = fxLayer.stateMachine;
                
                AnimatorState defaultState = Utility.FindState(stateMachine, "default");
                if(defaultState == null)
                {
                    defaultState = stateMachine.AddState("default");
                    defaultState.writeDefaultValues = false;
                    stateMachine.defaultState = defaultState;
                }

                AnimatorState stateOff = stateMachine.AddState(threshold + "_off");
                AnimatorState stateOn = stateMachine.AddState(threshold + "_on");
                stateOff.writeDefaultValues = false;
                stateOn.writeDefaultValues = false;
                VRCAvatarParameterDriver offDriver = stateOff.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                offDriver.parameters.Add(new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter() { name = "driver", value = driverId + 1, type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set });
                VRCAvatarParameterDriver onDriver = stateOn.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                onDriver.parameters.Add(new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter() { name = "driver", value = driverId, type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set });

                if (!Utility.CheckControllerParameter(controllerFx, paramName))
                    controllerFx.AddParameter(paramName, AnimatorControllerParameterType.Int);
                AnimatorStateTransition default_on = defaultState.AddTransition(stateOn);
                default_on.duration = 0;
                AnimatorStateTransition on_off = stateOn.AddTransition(stateOff);
                on_off.duration = 0;
                AnimatorStateTransition off_default = stateOff.AddTransition(defaultState);
                off_default.hasExitTime = true;
                off_default.exitTime = 0.01f;
                off_default.duration = 0;
                default_on.AddCondition(AnimatorConditionMode.Equals, threshold, paramName);
                on_off.AddCondition(AnimatorConditionMode.NotEqual, threshold, paramName);
                drivers.Add(driverKey, driverId);

                return driverId;
            }

            /// <summary>
            /// 构建fx层的状态
            /// </summary>
            /// <param name="name">名字</param>
            /// <param name="driverId">驱动id</param>
            /// <param name="motion">动画</param>
            private static void BuildFxState(string name, int driverId, Motion motion)
            {
                AnimatorStateMachine fxStateMachine = controllerFx.layers[0].stateMachine;
                AnimatorState state = fxStateMachine.AddState(name);
                state.writeDefaultValues = false;
                state.motion = motion;
                //通过驱动id从anystate进入对应状态
                AnimatorStateTransition transition = fxStateMachine.AddAnyStateTransition(state);
                transition.AddCondition(AnimatorConditionMode.Equals, driverId, "driver");
                transition.duration = 0.05f;
            }
            
            /// <summary>
            /// 构建action层的动画
            /// </summary>
            /// <param name="name">名字</param>
            /// <param name="driverId">驱动id</param>
            /// <param name="motion">动画</param>
            private static void BuildActionState(string name, int driverId, Motion motion)
            {
                AnimatorControllerLayer actionLayer = controllerAction.layers[0];
                AnimatorStateMachine actionStateMachine = actionLayer.stateMachine;
                AnimatorState waitForActionOrAFK = Utility.FindState(actionStateMachine, "WaitForActionOrAFK");
                AnimatorState templatePre = Utility.FindState(actionStateMachine, "easy_avatar_pre");
                AnimatorState templateOut = Utility.FindState(actionStateMachine, "easy_avatar_out");

                AnimatorState statePre = actionStateMachine.AddState(name + "_pre");
                statePre.motion = templatePre.motion;
                statePre.behaviours = templatePre.behaviours;
                statePre.writeDefaultValues = false;
                AnimatorState stateOn = actionStateMachine.AddState(name + "_on");
                stateOn.motion = motion;
                stateOn.writeDefaultValues = false;
                AnimatorState stateOut = actionStateMachine.AddState(name + "_out");
                stateOut.motion = templateOut.motion;
                stateOut.behaviours = templateOut.behaviours;
                stateOut.writeDefaultValues = false;

                AnimatorStateTransition off_pre = waitForActionOrAFK.AddTransition(statePre);
                off_pre.AddCondition(AnimatorConditionMode.Equals, driverId, "driver");
                off_pre.duration = 0;
                AnimatorStateTransition pre_on = statePre.AddTransition(stateOn);
                pre_on.hasExitTime = true;
                pre_on.duration = 0;
                AnimatorStateTransition on_out = stateOn.AddTransition(stateOut);
                on_out.AddCondition(AnimatorConditionMode.Equals, driverId+1, "driver");
                on_out.duration = 0;
                AnimatorStateTransition out_off = stateOut.AddTransition(waitForActionOrAFK);
                out_off.hasExitTime = true;
                out_off.duration = 0;
            }
            
            /// <summary>
            /// 构建一维blend tree
            /// </summary>
            /// <param name="name">名字</param>
            /// <param name="paramaName">参数名</param>
            /// <param name="motion1">参数为0时的动画</param>
            /// <param name="motion2">参数为1时的动画</param>
            /// <returns></returns>
            private static BlendTree Build1DBlendTree(string name,string paramaName, Motion motion1, Motion motion2)
            {
                BlendTree blendTree = new BlendTree();
                blendTree.blendType = BlendTreeType.Simple1D;
                blendTree.blendParameter = paramaName;
                blendTree.name = name;
                blendTree.AddChild(motion1);
                blendTree.AddChild(motion2);
                return blendTree;
            }

            /// <summary>
            /// 将clip添加到fx层的初始动画
            /// </summary>
            /// <param name="clip"></param>
            private static void AddFxInitClip(AnimationClip clip)
            {
                fxInitClip = Utility.MergeAnimClip(fxInitClip, clip);
            }

            /// <summary>
            /// 构建fx层初始状态
            /// </summary>
            /// <param name="motion">初始动画</param>
            private static void BuildFxInitState(Motion motion)
            {
                AssetDatabase.CreateAsset(motion, animBuildDir + "FxInit.anim");
                AnimatorStateMachine fxState = controllerFx.layers[0].stateMachine;
                fxState.defaultState = fxState.AddState("Init");
                fxState.defaultState.writeDefaultValues = false;
                fxState.defaultState.motion = motion;
            }



        }
        #endregion

        #region Utility
        public class Utility
        {
            /// <summary>
            /// 去除文件名的非法字符
            /// </summary>
            /// <param name="fileName">文件名</param>
            /// <returns></returns>
            public static string GetGoodFileName(string fileName)
            {
                string result = fileName;
                foreach (char ichar in Path.GetInvalidFileNameChars())
                    result.Replace(ichar.ToString(), "");
                return result;
            }

            /// <summary>
            /// 通过behaviors生成动画
            /// </summary>
            /// <param name="behaviors">序列化的EasyBehvior列表</param>
            /// <returns>生成的动画</returns>
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

            /// <summary>
            /// 通过behaviors生成动画
            /// </summary>
            /// <param name="behaviors">behaviors</param>
            /// <returns>动画</returns>
            public static AnimationClip GenerateAnimClip(List<EasyBehavior> behaviors)
            {
                AnimationClip clip = new AnimationClip();
                clip.frameRate = 60;
                
                int behaviorsCount = behaviors.Count;
                for (int i = 0; i < behaviorsCount; i++)
                {
                    EasyBehavior behavior = behaviors[i];
                    List<EasyProperty> propertyGroup = behavior.propertyGroup;
                    int propertyCount = propertyGroup.Count;
                    for (int j = 0; j < propertyCount; j++)
                    {
                        EasyProperty property = propertyGroup[j];
                        if (property.targetProperty == "")
                            continue;

                        EditorCurveBinding binding = GetBinding(property);

                        if (!binding.isPPtrCurve)
                        {
                            AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(0, property.floatValue, 1.0f / 60, property.floatValue));
                        }
                        else
                        {

                            ObjectReferenceKeyframe[] objectReferenceKeyframes = {
                                new ObjectReferenceKeyframe() { time = 0, value = property.objectValue },
                                new ObjectReferenceKeyframe() { time = 1.0f / 60, value = property.objectValue }
                            };
                            AnimationUtility.SetObjectReferenceCurve(clip, binding, objectReferenceKeyframes);
                        }
                    }
                }
                return clip;
            }

            /// <summary>
            /// 生成恢复到默认状态的动画
            /// </summary>
            /// <param name="avatar">avatar</param>
            /// <param name="clip">clip</param>
            /// <returns></returns>
            public static AnimationClip GenerateRestoreAnimClip(GameObject avatar, AnimationClip clip)
            {
                AnimationClip result = new AnimationClip();
                result.frameRate = 60;
                foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
                {
                    float value;
                    AnimationUtility.GetFloatValue(avatar, binding, out value);
                    AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(0, value, 1.0f / 60, value));
                }
                foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                {
                    UnityEngine.Object value;
                    AnimationUtility.GetObjectReferenceValue(avatar, binding, out value);
                    ObjectReferenceKeyframe[] objectReferenceKeyframes = {
                                new ObjectReferenceKeyframe() { time = 0, value = value },
                                new ObjectReferenceKeyframe() { time = 1.0f / 60, value = value }
                            };
                    AnimationUtility.SetObjectReferenceCurve(clip, binding, objectReferenceKeyframes);
                }
                return clip;
            }

            /// <summary>
            /// 合并动画
            /// </summary>
            /// <param name="clips">序列化的动画列表</param>
            /// <returns>动画</returns>
            public static AnimationClip MergeAnimClip(SerializedProperty clips)
            {
                AnimationClip result = new AnimationClip();
                result.frameRate = 60;

                for (int i = 0; i < clips.arraySize; i++)
                {
                    AnimationClip clip = (AnimationClip)clips.GetArrayElementAtIndex(i).objectReferenceValue;
                    if (!clip)
                        continue;
                    foreach(EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
                        AnimationUtility.SetEditorCurve(result, binding, AnimationUtility.GetEditorCurve(clip, binding));
                    foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                        AnimationUtility.SetObjectReferenceCurve(result, binding, AnimationUtility.GetObjectReferenceCurve(clip, binding));
                }
                return result;
            }

            /// <summary>
            /// 合并动画
            /// </summary>
            /// <param name="clips">多个动画</param>
            /// <returns>合并的动画</returns>
            public static AnimationClip MergeAnimClip(params AnimationClip[] clips)
            {
                AnimationClip result = new AnimationClip();
                result.frameRate = 60;

                foreach (AnimationClip clip in clips)
                {
                    if (!clip)
                        continue;
                    foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
                        AnimationUtility.SetEditorCurve(result, binding, AnimationUtility.GetEditorCurve(clip, binding));
                    foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                        AnimationUtility.SetObjectReferenceCurve(result, binding, AnimationUtility.GetObjectReferenceCurve(clip, binding));
                }
                return result;
            }

            /// <summary>
            /// 分离人体动画
            /// </summary>
            /// <param name="clip">动画</param>
            /// <returns>[0]人体动画，[1]非人体动画</returns>
            public static AnimationClip[] SeparateHumanAnimation(AnimationClip clip)
            {
                AnimationClip humanAnim = new AnimationClip();
                humanAnim.frameRate = 60;
                AnimationClip fxAnim = new AnimationClip();
                fxAnim.frameRate = 60;
                foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
                {
                    //一般在根物体也就是avatar上只用了人体动画
                    if (binding.path == "")
                        AnimationUtility.SetEditorCurve(humanAnim, binding, AnimationUtility.GetEditorCurve(clip, binding));
                    else
                        AnimationUtility.SetEditorCurve(fxAnim, binding, AnimationUtility.GetEditorCurve(clip, binding));
                }
                //Object类型的曲线一定不是人体动画
                foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                    AnimationUtility.SetObjectReferenceCurve(fxAnim, binding, AnimationUtility.GetObjectReferenceCurve(clip, binding));

                return new AnimationClip[] {humanAnim,fxAnim };
            }

            /// <summary>
            /// 通过名字寻找动画状态机中的状态
            /// </summary>
            /// <param name="stateMachine">动画状态机</param>
            /// <param name="name">名字</param>
            /// <returns>找到的状态</returns>
            public static AnimatorState FindState(AnimatorStateMachine stateMachine, string name)
            {
                foreach(var childState in stateMachine.states)
                {
                    if (name == childState.state.name)
                        return childState.state;
                }
                return null;
            }

            /// <summary>
            /// 通过名字寻找controller中的层
            /// </summary>
            /// <param name="controller">controller</param>
            /// <param name="name">名字</param>
            /// <returns>层</returns>
            public static AnimatorControllerLayer FindLayer(AnimatorController controller, string name)
            {
                foreach (var layer in controller.layers)
                {
                    if (name == layer.name)
                        return layer;
                }
                return null;
            }

            /// <summary>
            /// 检测controller中是否有某个参数
            /// </summary>
            /// <param name="controller"></param>
            /// <param name="paramaName"></param>
            /// <returns>有则返回true</returns>
            public static bool CheckControllerParameter(AnimatorController controller, string paramaName)
            {
                if (controller.parameters == null)
                    return false;
                foreach (AnimatorControllerParameter parameter in controller.parameters)
                {
                    if (parameter.name == paramaName)
                        return true;
                }

                return false;
            }
            
            /// <summary>
            /// 复制序列化的EasyBehavior
            /// </summary>
            /// <param name="dest">目标Behavior</param>
            /// <param name="src">源Behavior</param>
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

            /// <summary>
            /// 复制序列化的EasyProperty
            /// </summary>
            /// <param name="dest">目标Property</param>
            /// <param name="src">源Property</param>
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
            
            /// <summary>
            /// 获取binding
            /// </summary>
            /// <param name="property">序列化的EasyProperty</param>
            /// <returns>对应binding</returns>
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

            /// <summary>
            /// 获取binding
            /// </summary>
            /// <param name="property"></param>
            /// <returns></returns>
            public static EditorCurveBinding GetBinding(EasyProperty property)
            {
                if (property.isPPtr)
                    return EditorCurveBinding.PPtrCurve(property.targetPath, EasyReflection.FindType(property.targetPropertyType), property.targetProperty);
                else if (property.isDiscrete)
                    return EditorCurveBinding.DiscreteCurve(property.targetPath, EasyReflection.FindType(property.targetPropertyType), property.targetProperty);
                return EditorCurveBinding.FloatCurve(property.targetPath, EasyReflection.FindType(property.targetPropertyType), property.targetProperty);
            }

            /// <summary>
            /// 获取欧拉角
            /// </summary>
            /// <param name="propertyGroup">序列化的EasyProperty列表</param>
            /// <returns>欧拉角</returns>
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

            /// <summary>
            /// 获取EasyProperty的值类型
            /// </summary>
            /// <param name="property">序列化的EasyProperty</param>
            /// <returns>值类型</returns>
            public static Type GetValueType(SerializedProperty property)
            {
                string valueTypeName = property.FindPropertyRelative("valueType").stringValue;
                return valueTypeName == "" ? null : EasyReflection.FindType(valueTypeName);
            }
            
            /// <summary>
            /// 检测物体是否具有该属性
            /// </summary>
            /// <param name="avatar">根物体</param>
            /// <param name="property">序列化的EasyProperty</param>
            /// <returns></returns>
            public static bool CheckProperty(GameObject avatar, SerializedProperty property)
            {

                if (!avatar || property.FindPropertyRelative("valueType").stringValue == "" || property.FindPropertyRelative("targetProperty").stringValue == "")
                    return false;
                return AnimationUtility.GetEditorCurveValueType(avatar, GetBinding(property)) != null;
            }

            /// <summary>
            /// 清除PropertyGroup只保留一个空的Property
            /// </summary>
            /// <param name="propertyGroup">序列化的EasyProperty列表</param>
            public static void ClearPropertyGroup(SerializedProperty propertyGroup)
            {
                string targetPath = propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("targetPath").stringValue;
                propertyGroup.ClearArray();
                propertyGroup.arraySize++;
                propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("targetPath").stringValue = targetPath;


            }

            /// <summary>
            /// 修改PropertyGroup中每一个Property
            /// </summary>
            /// <param name="propertyGroup">序列化的EasyProperty列表</param>
            /// <param name="relativePath">要修改的字段名</param>
            /// <param name="value">修改的值</param>
            public static void PropertyGroupEdit(SerializedProperty propertyGroup, string relativePath, string value)
            {
                for (int i = 0; i < propertyGroup.arraySize; i++)
                {
                    SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i).FindPropertyRelative(relativePath);
                    property.stringValue = value;
                }
            }

            /// <summary>
            /// 修改PropertyGroup中每一个Property
            /// </summary>
            /// <param name="propertyGroup">序列化的EasyProperty列表</param>
            /// <param name="relativePath">要修改的字段名</param>
            /// <param name="value">修改的值</param>
            public static void PropertyGroupEdit(SerializedProperty propertyGroup, string relativePath, bool value)
            {
                for (int i = 0; i < propertyGroup.arraySize; i++)
                {
                    SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i).FindPropertyRelative(relativePath);
                    property.boolValue = value;
                }
            }
            
            /// <summary>
            /// 美化PropertyGroup名
            /// </summary>
            /// <param name="animatableObjectType">Property的类型</param>
            /// <param name="propertyGroupName">原名</param>
            /// <returns>美化名</returns>
            public static string NicifyPropertyGroupName(Type animatableObjectType, string propertyGroupName)
            {

                return (string)EasyReflection.internalNicifyPropertyGroupName.Invoke(null, new object[] { animatableObjectType, propertyGroupName });
            }

            /// <summary>
            /// 获取Property在PropertyGroup中的后缀名
            /// </summary>
            /// <param name="propertyGroup">序列化的EasyProperty列表</param>
            /// <param name="index">Property的索引</param>
            /// <returns>后缀名</returns>
            public static string GetPropertyGroupSubname(SerializedProperty propertyGroup, int index)
            {
                return GetPropertyGroupSubname(propertyGroup.GetArrayElementAtIndex(index));
            }

            /// <summary>
            /// 获取Property在PropertyGroup中的后缀名
            /// </summary>
            /// <param name="property">序列化的EasyProperty</param>
            /// <returns>后缀名</returns>
            public static string GetPropertyGroupSubname(SerializedProperty property)
            {
                string targetProperty = property.FindPropertyRelative("targetProperty").stringValue;
                return targetProperty.Substring(targetProperty.Length - 1);
            }

            /// <summary>
            /// 检测PropertyGroup是否为颜色
            /// </summary>
            /// <param name="propertyGroup">序列化的EasyProperty列表</param>
            /// <returns></returns>
            public static bool PropertyGroupIsColor(SerializedProperty propertyGroup)
            {
                string targetProperty = propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("targetProperty").stringValue;
                //简单地判断一下
                return targetProperty.ToLower().Contains("color") && propertyGroup.arraySize == 4;
            }

            /// <summary>
            /// 检测EasyProperty是否为形态键
            /// </summary>
            /// <param name="propertyGroup">序列化的EasyProperty列表</param>
            /// <returns></returns>
            public static bool PropertyGroupIsBlendShape(SerializedProperty propertyGroup)
            {
                SerializedProperty property = propertyGroup.GetArrayElementAtIndex(0);
                string targetProperty = property.FindPropertyRelative("targetProperty").stringValue;
                string targetPropertyType = property.FindPropertyRelative("targetPropertyType").stringValue;
                return targetPropertyType.Contains("SkinnedMeshRenderer") && targetProperty.Contains("blendShape") && propertyGroup.arraySize == 1;
            }

            /// <summary>
            /// 获取target的avatar
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public static GameObject GetAvatar(GameObject target)
            {
                EasyAvatarHelper avatarHelper = target.GetComponentInParent<EasyAvatarHelper>();
                //检测是否在Avatar Helper中
                if (!avatarHelper)
                    return null;
                GameObject avatar = avatarHelper.avatar;
                //检测是否在Avatar Helper中设置了avatar
                if (!avatar)
                    return null;
                return avatar;
            }
        }
        #endregion
    }

}

