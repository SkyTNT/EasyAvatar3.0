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

        public class AnimatorControllerBuilder
        {
            public AnimatorController controller;
            AnimatorControllerLayer baseLayer;
            string path;
            Dictionary<int, Motion> drivedMotions;
            Dictionary<int, List<StateMachineBehaviour>> drivedStateBehaviour;
            Dictionary<int, string> stateNames;
            Dictionary<string, AnimatorControllerLayer> layers;
            AnimationClip initAnimation;

            public AnimatorControllerBuilder(string path)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(path); ;
                baseLayer = controller.layers[0];
                this.path = path;
                drivedMotions = new Dictionary<int, Motion>();
                drivedStateBehaviour = new Dictionary<int, List<StateMachineBehaviour>>();
                stateNames = new Dictionary<int, string>();
                layers = new Dictionary<string, AnimatorControllerLayer>();

                AddParameterInt("driver");
                AddParameterFloat("float1");
                AddParameterFloat("float2");
            }

            public void AddDrivedState(int driverId, string name, Motion motion = null)
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

                if (stateNames.ContainsKey(driverId))
                    stateNames[driverId] += "_" + name;
                else
                    stateNames.Add(driverId, name);
            }

            public void AddDrivedStateBehaviour(int driverId, params StateMachineBehaviour[] behaviours)
            {
                foreach (StateMachineBehaviour behaviour in behaviours)
                {
                    behaviour.hideFlags = HideFlags.HideInHierarchy;
                    //不加这个unity重启后新加的层里的内容会消失
                    AssetDatabase.AddObjectToAsset(behaviour, AssetDatabase.GetAssetPath(controller));

                    if (drivedStateBehaviour.ContainsKey(driverId))
                    {
                        List<StateMachineBehaviour> behaviourList = drivedStateBehaviour[driverId];
                        //先去除同类型的
                        StateMachineBehaviour sameType = null;
                        foreach (StateMachineBehaviour b in behaviourList)
                        {
                            if (b.GetType() == behaviour.GetType())
                            {
                                sameType = b;
                                break;
                            }
                        }
                        if (sameType) behaviourList.Remove(sameType);

                        behaviourList.Add(behaviour);
                    }
                    else
                    {
                        List<StateMachineBehaviour> behaviourList = new List<StateMachineBehaviour>();
                        behaviourList.Add(behaviour);
                        drivedStateBehaviour.Add(driverId, behaviourList);
                    }
                }
            }

            public void AddToInitState(AnimationClip clip)
            {
                if (!initAnimation)
                    initAnimation = clip;
                else
                    initAnimation = Utility.MergeAnimClip(initAnimation, clip);

            }

            public AnimatorControllerLayer AddLayer(string name)
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
                return newLayer;
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

            public bool CheckControllerParameter(string paramaName)
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
            
            public AnimatorControllerLayer FindLayer(string name)
            {
                AnimatorControllerLayer layer = null;
                layers.TryGetValue(name, out layer);
                return layer;
            }

            public AnimatorState FindState(AnimatorStateMachine stateMachine, string name)
            {
                foreach (var childState in stateMachine.states)
                {
                    if (name == childState.state.name)
                        return childState.state;
                }
                return null;
            }

            public  void BuildDrivedState(int driverId)
            {
                AnimatorStateMachine fxStateMachine = baseLayer.stateMachine;
                AnimatorState state = fxStateMachine.AddState(stateNames[driverId]);
                state.writeDefaultValues = false;
                state.motion = drivedMotions[driverId];
                List<StateMachineBehaviour> stateMachineBehaviours = null;
                drivedStateBehaviour.TryGetValue(driverId, out stateMachineBehaviours);
                state.behaviours = stateMachineBehaviours == null ? null : stateMachineBehaviours.ToArray();
                //通过驱动id从anystate进入对应状态
                AnimatorStateTransition transition = fxStateMachine.AddAnyStateTransition(state);
                transition.AddCondition(AnimatorConditionMode.Equals, driverId, "driver");
                transition.duration = 0.05f;
            }

            public void Build()
            {
                AnimatorState initState = baseLayer.stateMachine.AddState("init");
                initState.writeDefaultValues = false;
                initState.motion = initAnimation;
                baseLayer.stateMachine.defaultState = initState;
                if(initAnimation)
                    AssetDatabase.CreateAsset(initAnimation, Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_init.anim");
                foreach (int driverId in drivedMotions.Keys)
                    BuildDrivedState(driverId);

            }
        }

        public class AnimatorDriver
        {
            Dictionary<KeyValuePair<string, int>, int> drivers;
            AnimatorControllerBuilder builder;
            public AnimatorDriver(AnimatorControllerBuilder builder)
            {
                drivers = new Dictionary<KeyValuePair<string, int>, int>();
                this.builder = builder;
            }

            public int GetDriverId(string parameterName)
            {
                //查询drivers里是否已经有driver
                KeyValuePair<string, int> driverKey = new KeyValuePair<string, int>(parameterName, 1);
                if (drivers.ContainsKey(driverKey))
                    return drivers[driverKey];
                //每次id是成对的，所以乘2
                int driverCount = drivers.Count;
                int driverId = driverCount * 2 + 1;
                //添加新的层
                AnimatorControllerLayer layer = builder.AddLayer(parameterName);
                AnimatorStateMachine stateMachine = layer.stateMachine;
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

                if (!builder.CheckControllerParameter(parameterName))
                    builder.AddParameterBool(parameterName);
                //状态连线
                AnimatorStateTransition off_on = stateOff.AddTransition(stateOn);
                off_on.duration = 0;
                AnimatorStateTransition on_off = stateOn.AddTransition(stateOff);
                on_off.duration = 0;
                off_on.AddCondition(AnimatorConditionMode.If, 0, parameterName);
                on_off.AddCondition(AnimatorConditionMode.IfNot, 0, parameterName);
                //加入词典方便查询
                drivers.Add(driverKey, driverId);
                return driverId;
            }

            public int GetDriverId(string parameterName, int threshold)
            {
                //查询drivers里是否已经有driver
                KeyValuePair<string, int> driverKey = new KeyValuePair<string, int>(parameterName, threshold);
                if (drivers.ContainsKey(driverKey))
                    return drivers[driverKey];

                int driverCount = drivers.Count;
                int driverId = driverCount * 2 + 1;

                AnimatorControllerLayer layer = builder.FindLayer(parameterName);
                if (layer == null)
                    layer = builder.AddLayer(parameterName);
                AnimatorStateMachine stateMachine = layer.stateMachine;

                AnimatorState defaultState = builder.FindState(stateMachine, "default");
                if (defaultState == null)
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

                if (!builder.CheckControllerParameter(parameterName))
                    builder.AddParameterInt(parameterName);
                AnimatorStateTransition default_on = defaultState.AddTransition(stateOn);
                default_on.duration = 0;
                AnimatorStateTransition on_off = stateOn.AddTransition(stateOff);
                on_off.duration = 0;
                AnimatorStateTransition off_default = stateOff.AddTransition(defaultState);
                off_default.hasExitTime = true;
                off_default.exitTime = 0.05f;
                off_default.duration = 0;
                default_on.AddCondition(AnimatorConditionMode.Equals, threshold, parameterName);
                on_off.AddCondition(AnimatorConditionMode.NotEqual, threshold, parameterName);
                drivers.Add(driverKey, driverId);

                return driverId;
            }
        }

        public class VRCStateMachineBehaviour
        {
            public static VRCAnimatorTrackingControl FullTracking()
            {
                VRCAnimatorTrackingControl trackingControl = ScriptableObject.CreateInstance<VRCAnimatorTrackingControl>();
                VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType tracking = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Tracking;
                trackingControl.trackingEyes = tracking;
                trackingControl.trackingHead = tracking;
                trackingControl.trackingHip = tracking;
                trackingControl.trackingLeftFingers = tracking;
                trackingControl.trackingLeftFoot = tracking;
                trackingControl.trackingLeftHand = tracking;
                trackingControl.trackingMouth = tracking;
                trackingControl.trackingRightFingers = tracking;
                trackingControl.trackingRightFoot = tracking;
                trackingControl.trackingRightHand = tracking;
                return trackingControl;
            }

            public static VRCAnimatorTrackingControl FullAnimation()
            {
                VRCAnimatorTrackingControl trackingControl = ScriptableObject.CreateInstance<VRCAnimatorTrackingControl>();
                VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType animation = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Animation;
                trackingControl.trackingEyes = animation;
                trackingControl.trackingHead = animation;
                trackingControl.trackingHip = animation;
                trackingControl.trackingLeftFingers = animation;
                trackingControl.trackingLeftFoot = animation;
                trackingControl.trackingLeftHand = animation;
                trackingControl.trackingMouth = animation;
                trackingControl.trackingRightFingers = animation;
                trackingControl.trackingRightFoot = animation;
                trackingControl.trackingRightHand = animation;
                return trackingControl;
            }

            public static VRCAnimatorTrackingControl ReverseTrackingControl(VRCAnimatorTrackingControl src)
            {
                VRCAnimatorTrackingControl trackingControl = ScriptableObject.CreateInstance<VRCAnimatorTrackingControl>();
                trackingControl.trackingEyes = ReverseSingleTrackingControl(src.trackingEyes);
                trackingControl.trackingHip = ReverseSingleTrackingControl(src.trackingHip);
                trackingControl.trackingHead = ReverseSingleTrackingControl(src.trackingHead);
                trackingControl.trackingLeftFingers = ReverseSingleTrackingControl(src.trackingLeftFingers);
                trackingControl.trackingLeftFoot = ReverseSingleTrackingControl(src.trackingLeftFoot);
                trackingControl.trackingLeftHand = ReverseSingleTrackingControl(src.trackingLeftHand);
                trackingControl.trackingMouth = ReverseSingleTrackingControl(src.trackingMouth);
                trackingControl.trackingRightFingers = ReverseSingleTrackingControl(src.trackingRightFingers);
                trackingControl.trackingRightFoot = ReverseSingleTrackingControl(src.trackingRightFoot);
                trackingControl.trackingRightHand = ReverseSingleTrackingControl(src.trackingRightHand);
                return trackingControl;
            }

            private static VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType ReverseSingleTrackingControl(VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType src)
            {
                switch (src)
                {
                    case VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Tracking:
                        return VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Animation;
                    case VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Animation:
                        return VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Tracking;
                    default:
                        return VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.NoChange;
                }
            }

            public static VRCAnimatorTrackingControl CalculateTrackingControl(Motion motion)
            {
                VRCAnimatorTrackingControl trackingControl = ScriptableObject.CreateInstance<VRCAnimatorTrackingControl>();
                CalculateTrackingControl(trackingControl, motion);
                return trackingControl;
            }

            public static void CalculateTrackingControl(VRCAnimatorTrackingControl trackingControl, Motion motion)
            {
                VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType animation = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Animation;

                AnimationClip animationClip = motion as AnimationClip;
                if (animationClip)
                {
                    foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(animationClip))
                    {
                        if (binding.type == typeof(Animator) && binding.path == "")
                        {
                            string name = binding.propertyName;
                            if (name.Contains("Head"))
                                trackingControl.trackingHead = animation;
                            else if (name.Contains("Root"))
                                trackingControl.trackingHip = animation;
                            else if (name.Contains("Eye"))
                                trackingControl.trackingEyes = animation;
                            else if (name.Contains("Jaw"))
                                trackingControl.trackingMouth = animation;
                            else if (name.Contains("Left Hand"))
                                trackingControl.trackingLeftHand = animation;
                            else if (name.Contains("LeftHand"))
                                trackingControl.trackingLeftFingers = animation;
                            else if (name.Contains("Right Hand"))
                                trackingControl.trackingRightHand = animation;
                            else if (name.Contains("RightHand"))
                                trackingControl.trackingRightFingers = animation;
                            else if (name.Contains("Left Foot"))
                                trackingControl.trackingLeftFoot = animation;
                            else if (name.Contains("Right Foot"))
                                trackingControl.trackingRightFoot = animation;
                        }
                    }
                }
                BlendTree blendTree = motion as BlendTree;
                if (blendTree)
                {
                    foreach (var child in blendTree.children)
                    {
                        CalculateTrackingControl(trackingControl, child.motion);
                    }
                }
            }

            public static VRCPlayableLayerControl PlayableLayerControl(VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer layer,float weight)
            {
                VRCPlayableLayerControl playableLayerControl = ScriptableObject.CreateInstance<VRCPlayableLayerControl>();
                playableLayerControl.blendDuration = 1;
                playableLayerControl.layer = layer;
                playableLayerControl.goalWeight = weight;
                return playableLayerControl;
            }

            public static VRCPlayableLayerControl ActionLayerControl(float weight)
            {
                return PlayableLayerControl(VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer.Action, weight);
            }

        }

        public class ProxyAnim
        {
            /// <summary>
            /// 代理站立
            /// </summary>
            public static AnimationClip stand_still;
            public static AnimationClip afk;
            static ProxyAnim()
            {
                stand_still = GetProxyAnim("proxy_stand_still");
                afk = GetProxyAnim("proxy_afk");
            }

            /// <summary>
            /// 获取vrc的代理动画
            /// </summary>
            /// <param name="name">名字，不包含后缀</param>
            /// <returns></returns>
            public static AnimationClip GetProxyAnim(string name)
            {
                AnimationClip animation = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/VRCSDK/Examples3/Animation/ProxyAnim/" + name + ".anim");
                return animation;
            }
        }

        public class Builder
        {
            string rootBuildDir, menuBuildDir, animBuildDir;
            EasyAvatarHelper helper;
            AnimatorControllerBuilder fxLayerBuilder, actionLayerBuilder;
            AnimatorDriver driver;
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

                //初始化AnimatorController
                fxLayerBuilder = new AnimatorControllerBuilder(animBuildDir + "FXLayer.controller");
                actionLayerBuilder = new AnimatorControllerBuilder(animBuildDir + "ActionLayer.controller");
                driver = new AnimatorDriver(fxLayerBuilder);

                //afk
                {
                    int afkDriverId = driver.GetDriverId("AFK");
                    actionLayerBuilder.AddDrivedState(afkDriverId, "afk", ProxyAnim.afk);
                    actionLayerBuilder.AddDrivedStateBehaviour(afkDriverId, VRCStateMachineBehaviour.FullAnimation(), VRCStateMachineBehaviour.ActionLayerControl(1));
                    actionLayerBuilder.AddDrivedState(afkDriverId + 1, "afk_off", ProxyAnim.stand_still);
                    actionLayerBuilder.AddDrivedStateBehaviour(afkDriverId + 1, VRCStateMachineBehaviour.FullTracking(), VRCStateMachineBehaviour.ActionLayerControl(0));
                }

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

                //构建AnimatorController
                fxLayerBuilder.Build();
                actionLayerBuilder.Build();

                //设置CustomizeAnimationLayers
                avatarDescriptor.customizeAnimationLayers = true;
                avatarDescriptor.baseAnimationLayers = new CustomAnimLayer[]{
                    new CustomAnimLayer(){type = AnimLayerType.Base ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Additive ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Gesture ,isDefault = true},
                    new CustomAnimLayer(){type = AnimLayerType.Action ,isDefault = false , animatorController = actionLayerBuilder.controller },
                    new CustomAnimLayer(){type = AnimLayerType.FX ,isDefault = false,animatorController = fxLayerBuilder.controller},
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
                    int driverId = driver.GetDriverId("GestureLeft", (int)gesture.gestureType);
                    fxLayerBuilder.AddDrivedState(driverId, name + "_L", fxAnim);
                    if (hasActionAnim)
                    {
                        actionLayerBuilder.AddDrivedState(driverId, name + "_L_on", actionAnim);
                        actionLayerBuilder.AddDrivedStateBehaviour(driverId, VRCStateMachineBehaviour.FullAnimation(), VRCStateMachineBehaviour.ActionLayerControl(1));
                        actionLayerBuilder.AddDrivedState(driverId + 1, name + "_L_off", ProxyAnim.stand_still);
                        actionLayerBuilder.AddDrivedStateBehaviour(driverId + 1, VRCStateMachineBehaviour.FullTracking(), VRCStateMachineBehaviour.ActionLayerControl(0));
                    }
                }

                if (gesture.handType == EasyGesture.HandType.Right || gesture.handType == EasyGesture.HandType.Any)
                {
                    int driverId = driver.GetDriverId("GestureRight", (int)gesture.gestureType);
                    fxLayerBuilder.AddDrivedState(driverId, name + "_R", fxAnim);
                    if (hasActionAnim)
                    { 
                        actionLayerBuilder.AddDrivedState(driverId, name + "_R_on", actionAnim);
                        actionLayerBuilder.AddDrivedStateBehaviour(driverId, VRCStateMachineBehaviour.FullAnimation(), VRCStateMachineBehaviour.ActionLayerControl(1));
                        actionLayerBuilder.AddDrivedState(driverId + 1, name + "_R_off", ProxyAnim.stand_still);
                        actionLayerBuilder.AddDrivedStateBehaviour(driverId + 1, VRCStateMachineBehaviour.FullTracking(), VRCStateMachineBehaviour.ActionLayerControl(0));
                    }
                }
                if (gesture.gestureType == EasyGesture.GestureType.Neutral)
                {
                    fxLayerBuilder.AddToInitState(fxAnim);
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

                AssetDatabase.CreateAsset(offFx, animBuildDir + name + "_off_fx.anim");
                AssetDatabase.CreateAsset(onFx, animBuildDir + name + "_on_fx.anim");
                //生成驱动id
                int driverId = driver.GetDriverId("control" + controlCount);
                //通过驱动id到对应状态
                fxLayerBuilder.AddDrivedState(driverId, name + "_on", onFx);
                fxLayerBuilder.AddDrivedState(driverId + 1, name + "_off", offFx);
                fxLayerBuilder.AddToInitState(offFx);

                //有action动画才加进去
                if (AnimationUtility.GetCurveBindings(onAction).Length > 0)
                {
                    AssetDatabase.CreateAsset(onAction, animBuildDir + name + "_on_action.anim");
                    actionLayerBuilder.AddDrivedState(driverId, name + "_on", onFx);
                    actionLayerBuilder.AddDrivedState(driverId + 1, name + "_off",ProxyAnim.stand_still);
                    actionLayerBuilder.AddDrivedStateBehaviour(driverId, VRCStateMachineBehaviour.FullAnimation(), VRCStateMachineBehaviour.ActionLayerControl(1));
                    actionLayerBuilder.AddDrivedStateBehaviour(driverId + 1, VRCStateMachineBehaviour.FullTracking(), VRCStateMachineBehaviour.ActionLayerControl(0));
                }
            }
            
            /// <summary>
            /// 构建旋钮控件
            /// </summary>
            /// <param name="name">名字</param>
            /// <param name="control">控件</param>
            private void BuildRadialPuppet(string name, EasyControl control)
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

                BlendTree fxBlendTree = Utility.Generate1DBlendTree(name + "_on_fx", "float1", offFx, onFx);
                AssetDatabase.AddObjectToAsset(fxBlendTree, AssetDatabase.GetAssetPath(fxLayerBuilder.controller));
                AssetDatabase.CreateAsset(offFx, animBuildDir + name + "_off_fx.anim");
                AssetDatabase.CreateAsset(onFx, animBuildDir + name + "_on_fx.anim");
                int driverId = driver.GetDriverId("control" + controlCount);
                fxLayerBuilder.AddDrivedState(driverId, name + "_on", fxBlendTree);
                fxLayerBuilder.AddToInitState(offFx);

                if (AnimationUtility.GetCurveBindings(offAction).Length > 0 || AnimationUtility.GetCurveBindings(onAction).Length > 0)
                {
                    BlendTree actionBlendTree = Utility.Generate1DBlendTree(name + "_on_action", "float1", offAction, onAction);
                    AssetDatabase.AddObjectToAsset(actionBlendTree, AssetDatabase.GetAssetPath(actionLayerBuilder.controller));
                    AssetDatabase.CreateAsset(offAction, animBuildDir + name + "_off_action.anim");
                    AssetDatabase.CreateAsset(onAction, animBuildDir + name + "_on_action.anim");
                    actionLayerBuilder.AddDrivedState(driverId, name + "_on", actionBlendTree);
                    actionLayerBuilder.AddDrivedState(driverId + 1, name + "_off", ProxyAnim.stand_still);
                    actionLayerBuilder.AddDrivedStateBehaviour(driverId, VRCStateMachineBehaviour.FullAnimation(), VRCStateMachineBehaviour.ActionLayerControl(1));
                    actionLayerBuilder.AddDrivedStateBehaviour(driverId + 1, VRCStateMachineBehaviour.FullTracking(), VRCStateMachineBehaviour.ActionLayerControl(0));
                }
            }

        }
        #endregion
        
    }

}

