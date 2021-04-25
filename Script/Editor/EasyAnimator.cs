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

        public void SetGestureAnimation(string name, AnimationClip animation, bool rightHand, int threshold)
        {
            int driverId = driver.GetDriverId(rightHand ? "GestureRight" : "GestureLeft", threshold);
            name += rightHand ? "_R" : "_L";
            gestureBuilder.AddDrivedState(driverId, name , animation);
            gestureBuilder.SetDrivedStateTimeParameter(driverId, rightHand ? "GestureRightWeight" : "GestureLeftWeight");
            gestureBuilder.SetDrivedStateLayer(driverId, rightHand ? gestureRightLayer : gestureLeftLayer);
        }

        public void AddState(string name, AnimationClip offAnim, AnimationClip onAnim, bool autoRestore, string parameterName, int threshold = -999)
        {
            AddState(name, offAnim, onAnim, null, null, autoRestore, parameterName, threshold);
        }

        public void AddState(string name, AnimationClip offAnim, AnimationClip onAnim, EasyTrackingControl offTracking, EasyTrackingControl onTracking, bool autoRestore, string parameterName, int threshold = -999)
        {
            SeparateAnimation(offAnim, out AnimationClip off_action, out AnimationClip off_fx);
            SeparateAnimation(onAnim, out AnimationClip on_action, out AnimationClip on_fx);
            //对于人体动画，关闭的动画只能是待机动画
            off_action = VRCAssets.proxy_stand_still;
            if (autoRestore)
            {
                off_fx = Utility.MergeAnimClip(Utility.GenerateRestoreAnimClip(avatar, on_fx), off_fx);
            }

            int driverId;

            //设-999为bool类型的参数
            if (threshold != -999)
                driverId = driver.GetDriverId(parameterName, threshold);
            else
                driverId = driver.GetDriverId(parameterName);

            fxBuilder.AddDrivedState(driverId, name + "_on", on_fx);
            fxBuilder.AddDrivedState(driverId + 1, name + "_off", off_fx);
            fxBuilder.AddToInitState(off_fx);
            if (!Utility.AnimationIsEmpty(on_action))
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

        public void AddState(string name, AnimationClip offAnim, AnimationClip blendTree0, AnimationClip blendTree1, bool autoRestore, string parameterName, int threshold = -999)
        {
            AddState(name, offAnim, blendTree0, blendTree1, null, null, autoRestore, parameterName, threshold);
        }

        public void AddState(string name, AnimationClip offAnim, AnimationClip blendTree0, AnimationClip blendTree1, EasyTrackingControl offTracking, EasyTrackingControl onTracking, bool autoRestore, string parameterName, int threshold = -999)
        {
            SeparateAnimation(offAnim, out AnimationClip off_action, out AnimationClip off_fx);
            SeparateAnimation(blendTree0, out AnimationClip blend0_action, out AnimationClip blend0_fx);
            SeparateAnimation(blendTree1, out AnimationClip blend1_action, out AnimationClip blend1_fx);
            off_action = VRCAssets.proxy_stand_still;
            if (autoRestore)
            {
                off_fx = Utility.MergeAnimClip(Utility.GenerateRestoreAnimClip(avatar, Utility.MergeAnimClip(blend0_fx, blend1_fx)), off_fx);
            }

            int driverId;
            if (threshold != -999)
                driverId = driver.GetDriverId(parameterName, threshold);
            else
                driverId = driver.GetDriverId(parameterName);

            BlendTree blendTree_fx, blendTree_action;
            blendTree_fx = Utility.Generate1DBlendTree(name + "_on", "float1", blend0_fx, blend1_fx);
            fxBuilder.AddDrivedState(driverId, name + "_on", blendTree_fx);
            fxBuilder.AddDrivedState(driverId + 1, name + "_off", off_fx);
            fxBuilder.AddToInitState(off_fx);

            if(!Utility.AnimationIsEmpty(blend0_action) || !Utility.AnimationIsEmpty(blend1_action))
            {
                blendTree_action = Utility.Generate1DBlendTree(name + "_on", "float1", blend0_action, blend1_action);
                actionBuilder.AddDrivedState(driverId, name + "_on", blendTree_action);
                actionBuilder.AddDrivedState(driverId + 1, name + "_off", off_action);
                if (offTracking != null && onTracking != null)
                {
                    actionBuilder.AddDrivedStateBehaviour(driverId, VRCStateMachineBehaviourUtility.GetTrackingControl(onTracking), VRCStateMachineBehaviourUtility.ActionLayerControl(1, 0));
                    actionBuilder.AddDrivedStateBehaviour(driverId + 1, VRCStateMachineBehaviourUtility.GetTrackingControl(offTracking), VRCStateMachineBehaviourUtility.ActionLayerControl(0, 0));

                }
                else
                {
                    var trackingControl = VRCStateMachineBehaviourUtility.CalculateTrackingControl(blendTree_action);
                    actionBuilder.AddDrivedStateBehaviour(driverId, trackingControl, VRCStateMachineBehaviourUtility.ActionLayerControl(1, 0));
                    actionBuilder.AddDrivedStateBehaviour(driverId + 1, VRCStateMachineBehaviourUtility.ReverseTrackingControl(trackingControl), VRCStateMachineBehaviourUtility.ActionLayerControl(0, 0));
                }
            }
        }
        
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
        
        public static void SeparateAnimation(AnimationClip clip, out AnimationClip action, out AnimationClip fx)
        {
            action = new AnimationClip();
            fx = new AnimationClip();
            action.frameRate = 60;
            fx.frameRate = 60;
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (binding.type == typeof(Animator) && binding.path == "")
                    AnimationUtility.SetEditorCurve(action, binding, AnimationUtility.GetEditorCurve(clip, binding));
                else
                    AnimationUtility.SetEditorCurve(fx, binding, AnimationUtility.GetEditorCurve(clip, binding));

            }
            foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                AnimationUtility.SetObjectReferenceCurve(fx, binding, AnimationUtility.GetObjectReferenceCurve(clip, binding));
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

        public void SetDrivedStateLayer(int driverId, AnimatorControllerLayer layer)
        {
            if (drivedStateLayer.ContainsKey(driverId))
                drivedStateLayer[driverId] = layer;
            else
                drivedStateLayer.Add(driverId, layer);
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

        public void CheckMotionAndSave(ref Motion motion, string name)
        {

            AnimationClip animationClip = motion as AnimationClip;
            if (animationClip)
            {
                //如果animationClip为空，就舍弃这个animationClip
                if (Utility.AnimationIsEmpty(animationClip))
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

    public class VRCStateMachineBehaviourUtility
    {
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

        public static VRCPlayableLayerControl PlayableLayerControl(VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer layer, float weight, float blendDuration)
        {
            VRCPlayableLayerControl playableLayerControl = ScriptableObject.CreateInstance<VRCPlayableLayerControl>();
            playableLayerControl.blendDuration = blendDuration;
            playableLayerControl.layer = layer;
            playableLayerControl.goalWeight = weight;
            return playableLayerControl;
        }

        public static VRCPlayableLayerControl ActionLayerControl(float weight, float blendDuration = 1)
        {
            return PlayableLayerControl(VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer.Action, weight, blendDuration);
        }

    }


}
