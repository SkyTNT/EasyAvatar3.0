using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace EasyAvatar
{
    
    public class EasyAnimator
    {
        string directoryPath;
        GameObject avatar;
        AnimatorControllerBuilder fxBuilder,actionBuilder,gestureBuilder;
        AnimatorControllerLayer gestureLeftLayer, gestureRightLayer;
        AnimatorDriver driver;

        public AnimatorController fxController
        {
            get
            {
                return fxBuilder.controller;
            }
        }

        public AnimatorController actionController
        {
            get
            {
                return actionBuilder.controller;
            }
        }

        public AnimatorController gestureController
        {
            get
            {
                return gestureBuilder.controller;
            }
        }

        public EasyAnimator(string directoryPath, GameObject avatar)
        {
            this.directoryPath = directoryPath;
            this.avatar = avatar;
            fxBuilder = new AnimatorControllerBuilder(directoryPath + "FXLayer.controller");
            actionBuilder = new AnimatorControllerBuilder(directoryPath + "ActionLayer.controller");
            gestureBuilder = new AnimatorControllerBuilder(directoryPath + "GestureLayer.controller");
            gestureLeftLayer = gestureBuilder.AddLayer("gesture Left");
            gestureRightLayer = gestureBuilder.AddLayer("gesture Right");
            gestureBuilder.baseLayer.avatarMask = VRCAssets.hands_only;
            gestureLeftLayer.avatarMask = VRCAssets.hand_left;
            gestureRightLayer.avatarMask = VRCAssets.hand_right;
            driver = new AnimatorDriver(fxBuilder);
        }

        /// <summary>
        /// 设置手势动画
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="animation">动画</param>
        /// <param name="rightHand">是否为右手</param>
        /// <param name="threshold">手势id</param>
        public void SetGestureAnimation(string name, AnimationClip animation, bool rightHand, int threshold)
        {
            int driverId = driver.GetDriverId(rightHand ? "GestureRight" : "GestureLeft", threshold);
            name += rightHand ? "_R" : "_L";
            gestureBuilder.AddDrivedState(driverId, name , animation);
            gestureBuilder.SetDrivedStateTimeParameter(driverId, rightHand ? "GestureRightWeight" : "GestureLeftWeight");
            gestureBuilder.SetDrivedStateLayer(driverId, rightHand ? gestureRightLayer : gestureLeftLayer);
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="offAnim">关闭的动画</param>
        /// <param name="blendTree0">打开时混合树左端的动画</param>
        /// <param name="blendTree1">打开时混合树右端的动画</param>
        /// <param name="autoRestore">是否自动恢复</param>
        /// <param name="parameterName">控制开关的参数名</param>
        /// <param name="threshold">参数值</param>
        public void AddState(string name, AnimationClip offAnim, Motion onMotion, bool autoRestore, string parameterName, int threshold = -999)
        {
            AddState(name, offAnim, onMotion, null, null, autoRestore, parameterName, threshold);
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="offAnim">关闭的动画</param>
        /// <param name="onMotion">打开时的混合树</param>
        /// <param name="offTracking">关闭时设置的追踪状态，null时自动生成</param>
        /// <param name="onTracking">打开时设置的追踪状态，null时自动生成</param>
        /// <param name="autoRestore">是否自动恢复</param>
        /// <param name="parameterName">控制开关的参数名</param>
        /// <param name="threshold">参数值</param>
        public void AddState(string name, AnimationClip offAnim, Motion onMotion, EasyTrackingControl offTracking, EasyTrackingControl onTracking, bool autoRestore, string parameterName, int threshold = -999)
        {
            Utility.SeparateAnimation(offAnim, out AnimationClip off_action, out AnimationClip off_fx);
            Utility.SeparateMotion(onMotion, out Motion on_action, out Motion on_fx);
            off_action = VRCAssets.proxy_stand_still;
            if (autoRestore)
            {
                off_fx = Utility.MergeAnimClip(Utility.GenerateRestoreAnimClip(avatar, on_fx), off_fx);
            }

            int driverId;
            if (threshold != -999)
                driverId = driver.GetDriverId(parameterName, threshold);
            else
                driverId = driver.GetDriverId(parameterName);
            
            fxBuilder.AddDrivedState(driverId, name + "_on", on_fx);
            fxBuilder.AddDrivedState(driverId + 1, name + "_off", off_fx);
            fxBuilder.AddToInitState(off_fx);

            if(!Utility.MotionIsEmpty(on_action))
            {
                actionBuilder.AddDrivedState(driverId, name + "_on", on_action);
                actionBuilder.AddDrivedState(driverId + 1, name + "_off", off_action);
                if (offTracking != null && onTracking != null)
                {
                    actionBuilder.AddDrivedStateBehaviour(driverId, VRCStateMachineBehaviourUtility.GetTrackingControl(onTracking), VRCStateMachineBehaviourUtility.ActionLayerControl(1, 0));
                    actionBuilder.AddDrivedStateBehaviour(driverId + 1, VRCStateMachineBehaviourUtility.GetTrackingControl(offTracking), VRCStateMachineBehaviourUtility.ActionLayerControl(0, 0));

                }
                else
                {
                    var trackingControl = VRCStateMachineBehaviourUtility.CalculateTrackingControl(on_action);
                    actionBuilder.AddDrivedStateBehaviour(driverId, trackingControl, VRCStateMachineBehaviourUtility.ActionLayerControl(1, 0));
                    actionBuilder.AddDrivedStateBehaviour(driverId + 1, VRCStateMachineBehaviourUtility.ReverseTrackingControl(trackingControl), VRCStateMachineBehaviourUtility.ActionLayerControl(0, 0));
                }
            }
        }
        
        /// <summary>
        /// 生成所有动画控制器
        /// </summary>
        public void Build()
        {
            {
                int afkDriverId = driver.GetDriverId("AFK");
                actionBuilder.AddDrivedState(afkDriverId, "afk", VRCAssets.proxy_afk);
                actionBuilder.AddDrivedStateBehaviour(afkDriverId, VRCStateMachineBehaviourUtility.FullAnimation(), VRCStateMachineBehaviourUtility.ActionLayerControl(1));
                actionBuilder.AddDrivedState(afkDriverId + 1, "afk_off", VRCAssets.proxy_stand_still);
                actionBuilder.AddDrivedStateBehaviour(afkDriverId + 1, VRCStateMachineBehaviourUtility.FullTracking(), VRCStateMachineBehaviourUtility.ActionLayerControl(0));
            }
            fxBuilder.Build();
            actionBuilder.Build();
            gestureBuilder.Build();
        }
        
    }

    
    public class AnimatorControllerBuilder
    {
        public AnimatorController controller;
        public AnimatorControllerLayer baseLayer;
        string saveDir;
        Dictionary<int, Motion> drivedMotions;
        Dictionary<int, List<StateMachineBehaviour>> drivedStateBehaviour;
        Dictionary<int, string> stateNames;
        Dictionary<int, string> timeParameters;
        Dictionary<int, AnimatorControllerLayer> drivedStateLayer;
        Dictionary<string, AnimatorControllerLayer> layers;
        AnimationClip initAnimation;

        public AnimatorControllerBuilder(string path)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            //controller.layers是复制的
            baseLayer = controller.layers[0];
            drivedMotions = new Dictionary<int, Motion>();
            drivedStateBehaviour = new Dictionary<int, List<StateMachineBehaviour>>();
            stateNames = new Dictionary<int, string>();
            timeParameters = new Dictionary<int, string>();
            drivedStateLayer = new Dictionary<int, AnimatorControllerLayer>();
            layers = new Dictionary<string, AnimatorControllerLayer>();

            layers.Add("Base Layer",baseLayer);

            AddParameterInt("driver");
            AddParameterFloat("float1");
            AddParameterFloat("float2");

            saveDir = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "/";
            Directory.CreateDirectory(saveDir);
        }

        /// <summary>
        /// 添加驱动了的状态
        /// </summary>
        /// <param name="driverId">驱动id</param>
        /// <param name="name">状态名字</param>
        /// <param name="motion">动画或者混合树</param>
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

        /// <summary>
        /// 添加状态行为
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="behaviours"></param>
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
        /// <summary>
        /// 设置状态的时间参数
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="parameterName"></param>
        public void SetDrivedStateTimeParameter(int driverId, string parameterName)
        {
            if (!CheckControllerParameter(parameterName))
            {
                AddParameterFloat(parameterName);
            }

            if (timeParameters.ContainsKey(driverId))
                timeParameters[driverId] = parameterName;
            else
                timeParameters.Add(driverId, parameterName);
        }

        /// <summary>
        /// 设置状态所在的层，默认为基础层
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="layer"></param>
        public void SetDrivedStateLayer(int driverId, AnimatorControllerLayer layer)
        {
            if (drivedStateLayer.ContainsKey(driverId))
                drivedStateLayer[driverId] = layer;
            else
                drivedStateLayer.Add(driverId, layer);
        }

        /// <summary>
        /// 添加到初始化动画
        /// </summary>
        /// <param name="clip"></param>
        public void AddToInitState(AnimationClip clip)
        {
            if (!initAnimation)
                initAnimation = clip;
            else
                initAnimation = Utility.MergeAnimClip(initAnimation, clip);

        }

        /// <summary>
        /// 添加层
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 添加bool型参数
        /// </summary>
        /// <param name="name"></param>
        public void AddParameterBool(string name)
        {
            controller.AddParameter(name, AnimatorControllerParameterType.Bool);
        }

        /// <summary>
        /// 添加int型参数
        /// </summary>
        /// <param name="name"></param>
        public void AddParameterInt(string name)
        {
            controller.AddParameter(name, AnimatorControllerParameterType.Int);
        }

        /// <summary>
        /// 添加float型参数
        /// </summary>
        /// <param name="name"></param>
        public void AddParameterFloat(string name)
        {
            controller.AddParameter(name, AnimatorControllerParameterType.Float);
        }

        /// <summary>
        /// 检查参数是否存在
        /// </summary>
        /// <param name="paramaName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 寻找层
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AnimatorControllerLayer FindLayer(string name)
        {
            AnimatorControllerLayer layer = null;
            layers.TryGetValue(name, out layer);
            return layer;
        }

        /// <summary>
        /// 寻找状态
        /// </summary>
        /// <param name="stateMachine">从这个状态机中寻找</param>
        /// <param name="name">状态名</param>
        /// <returns></returns>
        public AnimatorState FindState(AnimatorStateMachine stateMachine, string name)
        {
            foreach (var childState in stateMachine.states)
            {
                if (name == childState.state.name)
                    return childState.state;
            }
            return null;
        }

        /// <summary>
        /// 检查动画或者混合树是否为空，不为空就保存
        /// </summary>
        /// <param name="motion">动画或者混合树</param>
        /// <param name="name">保存名字</param>
        public void CheckMotionAndSave(ref Motion motion, string name)
        {

            AnimationClip animationClip = motion as AnimationClip;
            if (animationClip)
            {
                //如果animationClip为空，就舍弃这个animationClip
                if (animationClip.empty)
                    motion = null;
                else if (!AssetDatabase.Contains(animationClip))
                    AssetDatabase.CreateAsset(animationClip, saveDir + name + ".anim");
            }

            BlendTree blendTree = motion as BlendTree;
            if (blendTree)
            {
                bool childNotNull = false;

                //坑，这里是复制的
                ChildMotion[] children = blendTree.children;
                for (int i = 0; i < children.Length; i++)
                {
                    Motion m = children[i].motion;
                    CheckMotionAndSave(ref m, name + "_" + i);
                    children[i].motion = m;
                    if (m)
                        childNotNull = true;
                }
                //复制回去
                blendTree.children = children;

                if (childNotNull)
                    AssetDatabase.AddObjectToAsset(blendTree, controller);
                else
                    motion = null;
            }
        }

        /// <summary>
        /// 构建状态
        /// </summary>
        /// <param name="driverId"></param>
        public void BuildDrivedState(int driverId)
        {
            string name = stateNames[driverId];
            Motion motion = drivedMotions[driverId];
            CheckMotionAndSave(ref motion, name);
            drivedStateBehaviour.TryGetValue(driverId, out List < StateMachineBehaviour > stateMachineBehaviours);
            drivedStateLayer.TryGetValue(driverId, out AnimatorControllerLayer layer);
            if (layer == null)
                layer = baseLayer;

            AnimatorStateMachine fxStateMachine = layer.stateMachine;
            AnimatorState state = fxStateMachine.AddState(name);
            state.writeDefaultValues = false;
            state.motion = motion;
            state.behaviours = stateMachineBehaviours == null ? null : stateMachineBehaviours.ToArray();
            timeParameters.TryGetValue(driverId, out string timeParameter);
            if (timeParameter != null && timeParameter != "")
            {
                state.timeParameterActive = true;
                state.timeParameter = timeParameter;
            }

            //通过驱动id从anystate进入对应状态
            AnimatorStateTransition transition = fxStateMachine.AddAnyStateTransition(state);
            transition.AddCondition(AnimatorConditionMode.Equals, driverId, "driver");
            transition.duration = 0.05f;
            transition.canTransitionToSelf = false;
        }

        /// <summary>
        /// 构建动画控制器
        /// </summary>
        public void Build()
        {
            List<AnimatorControllerLayer> layerList=new List<AnimatorControllerLayer>();
            foreach(var layer in layers)
            {
                layerList.Add(layer.Value);
            }
            //把修改后的层复制回去
            controller.layers = layerList.ToArray();

            AnimatorState initState = baseLayer.stateMachine.AddState("init");
            initState.writeDefaultValues = false;
            initState.motion = initAnimation;
            baseLayer.stateMachine.defaultState = initState;
            if (initAnimation)
                AssetDatabase.CreateAsset(initAnimation, saveDir + "init.anim");
            foreach (int driverId in drivedMotions.Keys)
                BuildDrivedState(driverId);

        }
    }

    /// <summary>
    /// 驱动器
    /// </summary>
    public class AnimatorDriver
    {
        Dictionary<KeyValuePair<string, int>, int> drivers;
        AnimatorControllerBuilder builder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">作为驱动的动画控制器构建器</param>
        public AnimatorDriver(AnimatorControllerBuilder builder)
        {
            drivers = new Dictionary<KeyValuePair<string, int>, int>();
            this.builder = builder;
        }

        /// <summary>
        /// 生成一个驱动id
        /// </summary>
        /// <param name="parameterName">bool型参数名</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取驱动id
        /// </summary>
        /// <param name="parameterName">int型参数名字</param>
        /// <param name="threshold">参数值</param>
        /// <returns></returns>
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

    public class VRCStateMachineBehaviourUtility
    {
        /// <summary>
        /// EasyTrackingControl转为VRCAnimatorTrackingControl
        /// </summary>
        /// <param name="easyTrackingControl"></param>
        /// <returns></returns>
        public static VRCAnimatorTrackingControl GetTrackingControl(EasyTrackingControl easyTrackingControl)
        {
            VRCAnimatorTrackingControl trackingControl = ScriptableObject.CreateInstance<VRCAnimatorTrackingControl>();
            trackingControl.trackingHead = (VRCAnimatorTrackingControl.TrackingType)easyTrackingControl.head;
            trackingControl.trackingEyes = (VRCAnimatorTrackingControl.TrackingType)easyTrackingControl.eyes;
            trackingControl.trackingMouth = (VRCAnimatorTrackingControl.TrackingType)easyTrackingControl.mouth;
            trackingControl.trackingHip = (VRCAnimatorTrackingControl.TrackingType)easyTrackingControl.hip;
            trackingControl.trackingRightHand = (VRCAnimatorTrackingControl.TrackingType)easyTrackingControl.rightHand ;
            trackingControl.trackingLeftHand = (VRCAnimatorTrackingControl.TrackingType)easyTrackingControl.leftHand;
            trackingControl.trackingRightFingers = (VRCAnimatorTrackingControl.TrackingType)easyTrackingControl.rightFingers;
            trackingControl.trackingLeftFingers  = (VRCAnimatorTrackingControl.TrackingType)easyTrackingControl.leftFingers;
            trackingControl.trackingRightFoot  = (VRCAnimatorTrackingControl.TrackingType)easyTrackingControl.rightFoot;
            trackingControl.trackingLeftFoot = (VRCAnimatorTrackingControl.TrackingType)easyTrackingControl.leftFoot;
            return trackingControl;
        }

        /// <summary>
        /// 全部追踪
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// 全部动画
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 反转追踪状态
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 反转单个追踪状态
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 计算追踪
        /// </summary>
        /// <param name="motion"></param>
        /// <returns></returns>
        public static VRCAnimatorTrackingControl CalculateTrackingControl(Motion motion)
        {
            VRCAnimatorTrackingControl trackingControl = ScriptableObject.CreateInstance<VRCAnimatorTrackingControl>();
            CalculateTrackingControl(trackingControl, motion);
            return trackingControl;
        }

        /// <summary>
        /// 计算追踪
        /// </summary>
        /// <param name="trackingControl"></param>
        /// <param name="motion"></param>
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
        /// <summary>
        /// PlayableLayerControl设置
        /// </summary>
        /// <param name="layer">层</param>
        /// <param name="weight">混合权重</param>
        /// <param name="blendDuration">混合时间</param>
        /// <returns></returns>
        public static VRCPlayableLayerControl PlayableLayerControl(VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer layer, float weight, float blendDuration)
        {
            VRCPlayableLayerControl playableLayerControl = ScriptableObject.CreateInstance<VRCPlayableLayerControl>();
            playableLayerControl.blendDuration = blendDuration;
            playableLayerControl.layer = layer;
            playableLayerControl.goalWeight = weight;
            return playableLayerControl;
        }
        /// <summary>
        /// Action层控制
        /// </summary>
        /// <param name="weight">混合权重</param>
        /// <param name="blendDuration">混合时间</param>
        /// <returns></returns>
        public static VRCPlayableLayerControl ActionLayerControl(float weight, float blendDuration = 1)
        {
            return PlayableLayerControl(VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer.Action, weight, blendDuration);
        }

    }


}
