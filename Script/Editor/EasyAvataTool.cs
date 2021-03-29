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

        [MenuItem("EasyAvatar3.0/About", priority = 0)]
        public static void showAbout()
        {
            EditorUtility.DisplayDialog("About", Lang.About, "ok");
            Debug.Log(Lang.About);
        }

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
        public class Builder
        {
            static int controlCount, driverCount;
            static AnimatorController controllerFx, controllerAction;
            static string rootBuildDir, menuBuildDir, animBuildDir;
            static AnimationClip fxInitClip;

            /// <summary>
            /// 构建所有
            /// </summary>
            /// <param name="helper">AvatarHelper</param>
            public static void Build(EasyAvatarHelper helper)
            {
                controlCount = driverCount = 0;
                
                GameObject avatar = helper.avatar;
                if (!avatar)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarNotSet, "ok");
                    return;
                }
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

                rootBuildDir = workingDirectory + "Build/" + Utility.GetGoodFileName(avatar.name);
                menuBuildDir = rootBuildDir + "/Menu/";
                animBuildDir = rootBuildDir + "/Anim/";

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
                fxInitClip = new AnimationClip();

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

                Utility.SaveAnimClip(animBuildDir + "FxInit.anim", fxInitClip);
                AnimatorStateMachine fxState = controllerFx.layers[0].stateMachine;
                fxState.defaultState = fxState.AddState("Init");
                fxState.defaultState.writeDefaultValues = false;
                fxState.defaultState.motion = fxInitClip;

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
                        SerializedObject serializedObject = new SerializedObject(control);
                        serializedObject.Update();
                        EasyControl.Type controlType =(EasyControl.Type)serializedObject.FindProperty("type").enumValueIndex;
                        
                        VRCExpressionsMenu.Control vrcControl = new VRCExpressionsMenu.Control();
                        vrcControl.name = control.name;
                        vrcControl.icon = (Texture2D)serializedObject.FindProperty("icon").objectReferenceValue;
                        vrcControl.parameter = new VRCExpressionsMenu.Control.Parameter() { name = "control" + controlCount };
                        if (controlType == EasyControl.Type.Toggle)
                        {
                            vrcControl.type = VRCExpressionsMenu.Control.ControlType.Toggle;
                            BuildToggle(prefix + "_" + count + "_" + control.name, serializedObject);
                        }
                        else if(controlType == EasyControl.Type.RadialPuppet)
                        {
                            vrcControl.type = VRCExpressionsMenu.Control.ControlType.RadialPuppet;
                            vrcControl.subParameters = new VRCExpressionsMenu.Control.Parameter[] { new VRCExpressionsMenu.Control.Parameter() { name = "float1"} };
                            BuildRadialPuppet(prefix + "_" + count + "_" + control.name, serializedObject);
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

                    SerializedObject serializedObject = new SerializedObject(gesture);
                    serializedObject.Update();
                    SerializedProperty behaviors = serializedObject.FindProperty("behaviors");
                    SerializedProperty animations = serializedObject.FindProperty("animations");
                    SerializedProperty gestureType = serializedObject.FindProperty("gestureType");
                    SerializedProperty handType = serializedObject.FindProperty("handType");
                    SerializedProperty useAnimClip = serializedObject.FindProperty("useAnimClip");
                    AnimationClip animationClip = Utility.GenerateAnimClip(behaviors);
                    if (useAnimClip.boolValue)
                    {
                        animationClip = Utility.MergeAnimClip(Utility.MergeAnimClip(animations), animationClip);
                    }

                    AnimationClip[] separatedClips = Utility.SeparateActionAnimation(animationClip);
                    AnimationClip actionAnim = separatedClips[0];
                    AnimationClip nonActionAnim = separatedClips[1];
                    bool hasActionAnim = AnimationUtility.GetCurveBindings(actionAnim).Length > 0;
                    Utility.SaveAnimClip(animBuildDir +name + "_fx.anim", nonActionAnim);
                    if(hasActionAnim)
                        Utility.SaveAnimClip(animBuildDir + name + "_action.anim", actionAnim);
                    
                    if (handType.enumValueIndex == (int)EasyGesture.HandType.Left || handType.enumValueIndex == (int)EasyGesture.HandType.Any)
                    {
                        int driverId = BuildDriver(name + "_L", "GestureLeft", gestureType.enumValueIndex);
                        BuildFxState(name + "_L", driverId, nonActionAnim);
                        if (hasActionAnim)
                            BuildActionState(name + "_L", driverId, actionAnim);
                    }
                        
                    if (handType.enumValueIndex == (int)EasyGesture.HandType.Right || handType.enumValueIndex == (int)EasyGesture.HandType.Any)
                    {
                        int driverId = BuildDriver(name + "_R", "GestureRight", gestureType.enumValueIndex);
                        BuildFxState(name + "_R", driverId, nonActionAnim);
                        if (hasActionAnim)
                            BuildActionState(name + "_R", driverId, actionAnim);
                    }
                    if (gestureType.enumValueIndex == (int)EasyGesture.GestureType.Neutral)
                    {
                        fxInitClip = Utility.MergeAnimClip(fxInitClip, nonActionAnim);
                    }
                }
            }

            /// <summary>
            /// 构建按钮
            /// </summary>
            /// <param name="name">名字</param>
            /// <param name="control">序列化的控件</param>
            private static void BuildToggle(string name, SerializedObject control)
            {
                //EasyBehaviors生成动画
                AnimationClip offClip = Utility.GenerateAnimClip(control.FindProperty("behaviors1"));
                AnimationClip onClip = Utility.GenerateAnimClip(control.FindProperty("behaviors2"));
                //使用动画文件
                if (control.FindProperty("useAnimClip").boolValue)
                {
                    //先将动画文件合并，在与Behaviors生成动画合并
                    offClip = Utility.MergeAnimClip(Utility.MergeAnimClip(control.FindProperty("anims1")), offClip);
                    onClip = Utility.MergeAnimClip(Utility.MergeAnimClip(control.FindProperty("anims2")), onClip);
                }
                //分离action动画
                AnimationClip[] offClips = Utility.SeparateActionAnimation(offClip);
                AnimationClip[] onClips = Utility.SeparateActionAnimation(onClip);
                AnimationClip offClipNonAction = offClips[1];
                AnimationClip onClipAction = onClips[0];
                AnimationClip onClipNonAction = onClips[1];
                fxInitClip = Utility.MergeAnimClip(fxInitClip, offClipNonAction);

                Utility.SaveAnimClip(animBuildDir + name + "_off_fx.anim", offClipNonAction);
                Utility.SaveAnimClip(animBuildDir + name + "_on_fx.anim", onClipNonAction);
                //生成驱动id
                int driverId = BuildDriver(name, "control" + controlCount);
                //通过驱动id到对应状态
                BuildFxState(name + "_on", driverId, onClipNonAction);
                BuildFxState(name + "_off", driverId + 1, offClipNonAction);

                //有action动画才加进去
                if (AnimationUtility.GetCurveBindings(onClipAction).Length > 0)
                {
                    Utility.SaveAnimClip(animBuildDir + name + "_on_action.anim", onClipAction);
                    BuildActionState(name,driverId, onClipAction);
                }
            }

            private static void BuildRadialPuppet(string name, SerializedObject control)
            {

                //EasyBehaviors生成动画
                AnimationClip offClip = Utility.GenerateAnimClip(control.FindProperty("behaviors1"));
                AnimationClip onClip = Utility.GenerateAnimClip(control.FindProperty("behaviors2"));
                //使用动画文件
                if (control.FindProperty("useAnimClip").boolValue)
                {
                    //先将动画文件合并，在与Behaviors生成动画合并
                    offClip = Utility.MergeAnimClip(Utility.MergeAnimClip(control.FindProperty("anims1")), offClip);
                    onClip = Utility.MergeAnimClip(Utility.MergeAnimClip(control.FindProperty("anims2")), onClip);
                }
                //分离action动画
                AnimationClip[] offClips = Utility.SeparateActionAnimation(offClip);
                AnimationClip[] onClips = Utility.SeparateActionAnimation(onClip);
                AnimationClip offClipNonAction = offClips[1];
                AnimationClip onClipNonAction = onClips[1];

                BlendTree fxBlendTree = Build1DBlendTree(name + "_on", "float1", offClipNonAction, onClipNonAction);
                AssetDatabase.AddObjectToAsset(fxBlendTree, AssetDatabase.GetAssetPath(controllerFx));
                Utility.SaveAnimClip(animBuildDir + name + "_off_fx.anim", offClipNonAction);
                Utility.SaveAnimClip(animBuildDir + name + "_on_fx.anim", onClipNonAction);
                int driverId = BuildDriver(name, "control" + controlCount);
                BuildFxState(name, driverId, fxBlendTree);

            }

            private static bool checkControllerParameter(AnimatorController controller,string paramaName)
            {
                if (controller.parameters == null)
                    return false;
                foreach(AnimatorControllerParameter parameter in controller.parameters)
                {
                    if (parameter.name == paramaName)
                        return true;
                }

                return false;
            }

            private static int BuildDriver(string name, string paramName,int threshold = -200)
            {
                //调用时不指定threshold参数，就认为是bool类型的Parameter
                int driverId = driverCount*2 +1;
                driverCount++;
                AnimatorControllerLayer fxLayer = new AnimatorControllerLayer();
                AnimatorStateMachine stateMachine = new AnimatorStateMachine();
                fxLayer.name = name;
                fxLayer.stateMachine = stateMachine;
                fxLayer.defaultWeight = 1;
                stateMachine.name = name;
                stateMachine.hideFlags = HideFlags.HideInHierarchy;
                //不加这个unity重启后FxLayer里的内容会消失
                AssetDatabase.AddObjectToAsset(fxLayer.stateMachine, AssetDatabase.GetAssetPath(controllerFx));
                controllerFx.AddLayer(fxLayer);
                AnimatorState stateOff = stateMachine.AddState("off");
                AnimatorState statePre = stateMachine.AddState("pre");
                AnimatorState stateOn = stateMachine.AddState("on");
                AnimatorState stateOut = stateMachine.AddState("out");
                stateMachine.defaultState = stateOff;
                stateOff.writeDefaultValues = false;
                statePre.writeDefaultValues = false;
                stateOn.writeDefaultValues = false;
                stateOut.writeDefaultValues = false;
                VRCAvatarParameterDriver offDriver = stateOff.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                offDriver.parameters.Add(new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter() { name = "driver", value = 0, type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set });
                VRCAvatarParameterDriver preDriver = statePre.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                preDriver.parameters.Add(new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter() { name = "driver", value = driverId, type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set });
                VRCAvatarParameterDriver onDriver = stateOn.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                onDriver.parameters.Add(new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter() { name = "driver", value = 0, type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set });
                VRCAvatarParameterDriver outDriver = stateOut.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                outDriver.parameters.Add(new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter() { name = "driver", value = driverId+1, type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set });

                if (!checkControllerParameter(controllerFx, paramName))
                    controllerFx.AddParameter(paramName,threshold== -200 ? AnimatorControllerParameterType.Bool: AnimatorControllerParameterType.Int);
                AnimatorStateTransition off_pre = stateOff.AddTransition(statePre);
                off_pre.hasExitTime = true;
                off_pre.duration = 0.01f;
                off_pre.exitTime = 0.01f;
                AnimatorStateTransition pre_on = statePre.AddTransition(stateOn);
                pre_on.hasExitTime = true;
                pre_on.duration = 0.01f;
                pre_on.exitTime = 0.05f;
                AnimatorStateTransition on_out = stateOn.AddTransition(stateOut);
                on_out.hasExitTime = true;
                on_out.duration = 0.01f;
                on_out.exitTime = 0.01f;
                AnimatorStateTransition out_off = stateOut.AddTransition(stateOff);
                out_off.hasExitTime = true;
                out_off.duration = 0.01f;
                out_off.exitTime = 0.05f;
                if(threshold == -200)
                {
                    off_pre.AddCondition(AnimatorConditionMode.If, 0, paramName);
                    on_out.AddCondition(AnimatorConditionMode.IfNot, 0, paramName);
                }
                else
                {
                    off_pre.AddCondition(AnimatorConditionMode.Equals, threshold, paramName);
                    on_out.AddCondition(AnimatorConditionMode.NotEqual, threshold, paramName);
                }
                return driverId;
            }

            private static void BuildFxState(string name, int driverId, Motion motion)
            {
                AnimatorStateMachine fxStateMachine = controllerFx.layers[0].stateMachine;
                AnimatorState state = fxStateMachine.AddState(name);
                state.writeDefaultValues = false;
                state.motion = motion;
                AnimatorStateTransition transition = fxStateMachine.AddAnyStateTransition(state);
                transition.AddCondition(AnimatorConditionMode.Equals, driverId, "driver");
                transition.duration = 0.05f;
            }
            
            private static void BuildActionState(string name, int driverId, Motion motion)
            {
                AnimatorControllerLayer actionLayer = controllerAction.layers[0];
                AnimatorStateMachine actionStateMachine = actionLayer.stateMachine;
                AnimatorState waitForActionOrAFK = Utility.findState(actionStateMachine, "WaitForActionOrAFK");
                AnimatorState templatePre = Utility.findState(actionStateMachine, "easy_avatar_pre");
                AnimatorState templateOut = Utility.findState(actionStateMachine, "easy_avatar_out");

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
            /// 保存动画文件
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="clip">动画</param>
            /// <returns>原动画</returns>
            public static AnimationClip SaveAnimClip(string path, AnimationClip clip)
            {
                AssetDatabase.CreateAsset(clip, path);
                AssetDatabase.SaveAssets();
                return clip;
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
            public static AnimationClip[] SeparateActionAnimation(AnimationClip clip)
            {
                AnimationClip action = new AnimationClip();
                action.frameRate = 60;
                AnimationClip nonAction = new AnimationClip();
                nonAction.frameRate = 60;
                foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
                {
                    //一般在根物体也就是avatar上只用了animator
                    if (binding.path == "")
                        AnimationUtility.SetEditorCurve(action, binding, AnimationUtility.GetEditorCurve(clip, binding));
                    else
                        AnimationUtility.SetEditorCurve(nonAction, binding, AnimationUtility.GetEditorCurve(clip, binding));
                }
                //Object类型的曲线一定不是animator动画
                foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                    AnimationUtility.SetObjectReferenceCurve(nonAction, binding, AnimationUtility.GetObjectReferenceCurve(clip, binding));

                return new AnimationClip[] {action,nonAction };
            }

            /// <summary>
            /// 通过名字寻找动画状态机中的状态
            /// </summary>
            /// <param name="stateMachine">动画状态机</param>
            /// <param name="name">名字</param>
            /// <returns>找到的状态</returns>
            public static AnimatorState findState(AnimatorStateMachine stateMachine, string name)
            {
                foreach(var childState in stateMachine.states)
                {
                    if (name == childState.state.name)
                        return childState.state;
                }
                return null;
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

