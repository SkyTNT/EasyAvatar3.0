using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

        public EasyAnimator(string directoryPath, GameObject avatar)
        {
            this.directoryPath = directoryPath;
            this.avatar = avatar;
            fxBuilder = new AnimatorControllerBuilder(directoryPath + "FXLayer.controller");
            actionBuilder = new AnimatorControllerBuilder(directoryPath + "ActionLayer.controller");
            gestureBuilder = new AnimatorControllerBuilder(directoryPath + "GestureLayer.controller");
            driver = new AnimatorDriver(fxBuilder);
        }

        public void AddState(string name, AnimationClip offAnim, AnimationClip onAnim, bool autoRestore, string parameterName)
        {
            AnimationClip off_fx, off_action, off_gesture;
            AnimationClip on_fx, on_action, on_gesture;
            ProcessAnimation(name, offAnim, onAnim, autoRestore, out off_fx, out off_action, out off_gesture, out on_fx, out on_action, out on_gesture);
            
            int driverId = driver.GetDriverId(parameterName);

            fxBuilder.AddDrivedState(driverId, name + "_on", on_fx);
            fxBuilder.AddDrivedState(driverId + 1, name + "_off", off_fx);
            if (on_action)
            {
                actionBuilder.AddDrivedState(driverId, name + "_on", on_action);
                actionBuilder.AddDrivedState(driverId + 1, name + "_off", VRCAssets.proxy_stand_still);
            }
            if (on_gesture)
            {
                gestureBuilder.AddDrivedState(driverId, name + "_on", on_gesture);
                gestureBuilder.AddDrivedState(driverId + 1, name + "_off", off_gesture);
            }
        }

        private void ProcessAnimation(string name, AnimationClip offAnim, AnimationClip onAnim, bool autoRestore, out AnimationClip off_fx, out AnimationClip off_action, out AnimationClip off_gesture, out AnimationClip on_fx, out AnimationClip on_action, out AnimationClip on_gesture)
        {
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

            if (AnimationUtility.GetCurveBindings(off_action).Length == 0)
                off_action = null;
            else
                AssetDatabase.CreateAsset(off_action, directoryPath + name + "_off_action.anim");
            if (AnimationUtility.GetCurveBindings(off_gesture).Length == 0)
                off_gesture = null;
            else
                AssetDatabase.CreateAsset(off_gesture, directoryPath + name + "_off_gesture.anim");
            AssetDatabase.CreateAsset(off_fx, directoryPath + name + "_off_fx.anim");

            if (AnimationUtility.GetCurveBindings(on_action).Length == 0)
                on_action = null;
            else
                AssetDatabase.CreateAsset(on_action, directoryPath + name + "_on_action.anim");
            if (AnimationUtility.GetCurveBindings(on_gesture).Length == 0)
                on_gesture = null;
            else
                AssetDatabase.CreateAsset(on_gesture, directoryPath + name + "_on_gesture.anim");
            AssetDatabase.CreateAsset(on_fx, directoryPath + name + "_on_fx.anim");

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
