using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static EasyAvatar.EasyAvatarTool;

namespace EasyAvatar
{
    public class EasyAnimator
    {
        string directoryPath;
        GameObject avatar;
        AnimatorControllerBuilder fxBuilder,actionBuilder,gestureBuilder;
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
            gestureBuilder.SetMask(VRCAssets.hands_only);
            driver = new AnimatorDriver(fxBuilder);
        }

        public void AddState(string name, AnimationClip offAnim, AnimationClip onAnim, bool autoRestore, string parameterName,int threshold = -999)
        {
            AnimationClip off_fx, off_action, off_gesture;
            AnimationClip on_fx, on_action, on_gesture;
            off_fx = off_action = off_gesture = null;
            on_fx = on_action = on_gesture = null;
            if (offAnim)
                SeparateAnimation(offAnim, out off_action, out off_gesture, out off_fx);
            if (onAnim)
                SeparateAnimation(onAnim, out on_action, out on_gesture, out on_fx);
            if (autoRestore)
            {
                off_fx = Utility.MergeAnimClip(Utility.GenerateRestoreAnimClip(avatar, on_fx), off_fx);
                off_gesture = Utility.MergeAnimClip(Utility.GenerateRestoreAnimClip(avatar, on_gesture), off_gesture);
            }
            off_action = VRCAssets.proxy_stand_still;
            AssetDatabase.CreateAsset(off_fx, directoryPath + name + "_off_fx.anim");
            AssetDatabase.CreateAsset(on_fx, directoryPath + name + "_on_fx.anim");
            CheckAnimationAndSave(ref off_gesture, name + "_off_gesture");
            CheckAnimationAndSave(ref on_action, name + "_on_action");
            CheckAnimationAndSave(ref on_gesture, name + "_on_gesture");
            
            
            int driverId;
            //int参数或者bool参数
            if (threshold != -999)
                driverId = driver.GetDriverId(parameterName,threshold);
            else
                driverId = driver.GetDriverId(parameterName);

            fxBuilder.AddDrivedState(driverId, name + "_on", on_fx);
            fxBuilder.AddDrivedState(driverId + 1, name + "_off", off_fx);
            fxBuilder.AddToInitState(off_fx);
            if (on_action)
            {
                var trackingControl = VRCStateMachineBehaviour.CalculateTrackingControl(on_action);
                actionBuilder.AddDrivedState(driverId, name + "_on", on_action);
                actionBuilder.AddDrivedStateBehaviour(driverId, trackingControl, VRCStateMachineBehaviour.ActionLayerControl(1));
                actionBuilder.AddDrivedState(driverId + 1, name + "_off", off_action);
                actionBuilder.AddDrivedStateBehaviour(driverId, VRCStateMachineBehaviour.ReverseTrackingControl(trackingControl), VRCStateMachineBehaviour.ActionLayerControl(0));
            }
            if (on_gesture)
            {
                gestureBuilder.AddDrivedState(driverId, name + "_on", on_gesture);
                gestureBuilder.AddDrivedState(driverId + 1, name + "_off", off_gesture);
            }
        }

        public void AddState(string name, AnimationClip offAnim, AnimationClip blendTree0, AnimationClip blendTree1, bool autoRestore, string parameterName, int threshold = -999)
        {
            AnimationClip off_fx, off_action, off_gesture;
            AnimationClip blend0_fx, blend0_action, blend0_gesture;
            AnimationClip blend1_fx, blend1_action, blend1_gesture;
            off_fx = off_action = off_gesture = null;
            blend0_fx = blend0_action = blend0_gesture = null;
            blend1_fx = blend1_action = blend1_gesture = null;
            if (offAnim)
                SeparateAnimation(offAnim, out off_action, out off_gesture, out off_fx);
            if (blendTree0)
                SeparateAnimation(blendTree0, out blend0_action, out blend0_gesture, out blend0_fx);
            if (blendTree1)
                SeparateAnimation(blendTree1, out blend1_action, out blend1_gesture, out blend1_fx);

            if (autoRestore)
            {
                off_fx = Utility.MergeAnimClip(Utility.GenerateRestoreAnimClip(avatar, Utility.MergeAnimClip(blend0_fx, blend1_fx)), off_fx);
                off_gesture = Utility.MergeAnimClip(Utility.GenerateRestoreAnimClip(avatar, Utility.MergeAnimClip(blend0_gesture, blend1_gesture)), off_gesture);
            }
            off_action = VRCAssets.proxy_stand_still;

            AssetDatabase.CreateAsset(off_fx, directoryPath + name + "_off_fx.anim");
            AssetDatabase.CreateAsset(blend0_fx, directoryPath + name + "_on_fx0.anim");
            AssetDatabase.CreateAsset(blend1_fx, directoryPath + name + "_on_fx1.anim");
            CheckAnimationAndSave(ref off_gesture, name + "_off_gesture");
            CheckAnimationAndSave(ref blend0_action, name + "_on_action0");
            CheckAnimationAndSave(ref blend0_gesture, name + "_on_gesture0");
            CheckAnimationAndSave(ref blend1_action, name + "_on_action1");
            CheckAnimationAndSave(ref blend1_gesture, name + "_on_gesture1");

            int driverId;
            if (threshold != -999)
                driverId = driver.GetDriverId(parameterName, threshold);
            else
                driverId = driver.GetDriverId(parameterName);

            BlendTree blendTree_fx, blendTree_action, blendTree_gesture;
            blendTree_fx = Utility.Generate1DBlendTree(name+"_on", "float1", blend0_fx, blend1_fx);
            AssetDatabase.AddObjectToAsset(blendTree_fx, fxBuilder.controller);
            fxBuilder.AddDrivedState(driverId, name + "_on", blendTree_fx);
            fxBuilder.AddDrivedState(driverId + 1, name + "_off", off_fx);
            fxBuilder.AddToInitState(off_fx);

            if (blend0_action || blend1_action)
            {
                blendTree_action = Utility.Generate1DBlendTree(name + "_on", "float1", blend0_action, blend1_action);
                AssetDatabase.AddObjectToAsset(blendTree_action, actionBuilder.controller);
                var trackingControl = VRCStateMachineBehaviour.CalculateTrackingControl(blendTree_action);
                actionBuilder.AddDrivedState(driverId, name + "_on", blendTree_action);
                actionBuilder.AddDrivedStateBehaviour(driverId, trackingControl, VRCStateMachineBehaviour.ActionLayerControl(1));
                actionBuilder.AddDrivedState(driverId + 1, name + "_off", off_action);
                actionBuilder.AddDrivedStateBehaviour(driverId, VRCStateMachineBehaviour.ReverseTrackingControl(trackingControl), VRCStateMachineBehaviour.ActionLayerControl(0));
            }

            if (blend0_gesture || blend1_gesture)
            {
                blendTree_gesture = Utility.Generate1DBlendTree(name + "_on", "float1", blend0_gesture, blend1_gesture);
                AssetDatabase.AddObjectToAsset(blendTree_gesture, gestureBuilder.controller);
                actionBuilder.AddDrivedState(driverId, name + "_on", blendTree_gesture);
                actionBuilder.AddDrivedState(driverId + 1, name + "_off", off_gesture);
            }
        }

        public void CheckAnimationAndSave(ref AnimationClip animation, string name)
        {
            if (AnimationUtility.GetCurveBindings(animation).Length == 0)
                animation = null;
            else
                AssetDatabase.CreateAsset(animation, directoryPath + name + ".anim");
        }

        public void Build()
        {
            {
                int afkDriverId = driver.GetDriverId("AFK");
                actionBuilder.AddDrivedState(afkDriverId, "afk", VRCAssets.proxy_afk);
                actionBuilder.AddDrivedStateBehaviour(afkDriverId, VRCStateMachineBehaviour.FullAnimation(), VRCStateMachineBehaviour.ActionLayerControl(1));
                actionBuilder.AddDrivedState(afkDriverId + 1, "afk_off", VRCAssets.proxy_stand_still);
                actionBuilder.AddDrivedStateBehaviour(afkDriverId + 1, VRCStateMachineBehaviour.FullTracking(), VRCStateMachineBehaviour.ActionLayerControl(0));
            }
            fxBuilder.Build();
            actionBuilder.Build();
            gestureBuilder.Build();
        }
        

        public static void SeparateAnimation(AnimationClip clip, out AnimationClip action, out AnimationClip gesture,out AnimationClip fx)
        {
            action = new AnimationClip();
            gesture = new AnimationClip();
            fx = new AnimationClip();
            action.frameRate = 60;
            gesture.frameRate = 60;
            fx.frameRate = 60;
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (binding.type == typeof(Animator) && binding.path == "")
                {
                    if (binding.propertyName.Contains("LeftHand") || binding.propertyName.Contains("RightHand"))
                        AnimationUtility.SetEditorCurve(gesture, binding, AnimationUtility.GetEditorCurve(clip, binding));
                    else
                        AnimationUtility.SetEditorCurve(action, binding, AnimationUtility.GetEditorCurve(clip, binding));
                }
                else
                    AnimationUtility.SetEditorCurve(fx, binding, AnimationUtility.GetEditorCurve(clip, binding));

            }
            foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                AnimationUtility.SetObjectReferenceCurve(fx, binding, AnimationUtility.GetObjectReferenceCurve(clip, binding));
            
        }
    }

}
