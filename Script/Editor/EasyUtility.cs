using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace EasyAvatar
{

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

        public static bool CheckCurveBinding(EditorCurveBinding binding, EasyAnimationMask mask)
        {
            if (binding.type == typeof(Animator) && binding.path == "")
            {
                string name = binding.propertyName;
                if (name.Contains("Head") || name.Contains("Neck") || name.Contains("Eye") || name.Contains("Jaw"))
                    return mask.head;
                else if (name.Contains("Root") || name.Contains("Spine") || name.Contains("Chest"))
                    return mask.body;
                else if (name.Contains("Left Hand") || name.Contains("Left Arm") || name.Contains("Left Forearm"))
                    return mask.leftArm;
                else if (name.Contains("LeftHand"))
                    return mask.leftFingers;
                else if (name.Contains("Right Hand") || name.Contains("Right Arm") || name.Contains("Right Forearm"))
                    return mask.rightArm;
                else if (name.Contains("RightHand"))
                    return mask.rightFingers;
                else if (name.Contains("Left Upper Leg") || name.Contains("Left Lower Leg") || name.Contains("Left Foot") || name.Contains("Left Toes"))
                    return mask.leftLeg;
                else if (name.Contains("Right Upper Leg") || name.Contains("Right Lower Leg") || name.Contains("Right Foot") || name.Contains("Right Toes"))
                    return mask.rightLeg;
                else
                    return true;
            }
            else
                return mask.fx;
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
                if (behavior.type == EasyBehavior.Type.Property)
                {
                    EasyPropertyGroup propertyGroup = behavior.propertyGroup;
                    List<EasyProperty> properties = propertyGroup.properties;
                    for (int j = 0; j < properties.Count; j++)
                    {
                        EasyProperty property = properties[j];
                        if (property.targetProperty == "")
                            continue;

                        EditorCurveBinding binding = GetBinding(propertyGroup.targetPath, property);

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
                else if (behavior.type == EasyBehavior.Type.AnimationClip)
                {
                    if (behavior.anim)
                    {
                        foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(behavior.anim))
                            if (CheckCurveBinding(binding, behavior.mask))
                                AnimationUtility.SetEditorCurve(clip, binding, AnimationUtility.GetEditorCurve(behavior.anim, binding));
                        foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(behavior.anim))
                            if (CheckCurveBinding(binding, behavior.mask))
                                AnimationUtility.SetObjectReferenceCurve(clip, binding, AnimationUtility.GetObjectReferenceCurve(behavior.anim, binding));
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
            if (!clip)
                return result;

            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                float value;
                AnimationUtility.GetFloatValue(avatar, binding, out value);
                if (binding.type == typeof(Animator) && binding.path == "")
                    AnimationUtility.SetEditorCurve(result, binding, AnimationCurve.Linear(0, 0, 1.0f / 60, 0));
                else
                    AnimationUtility.SetEditorCurve(result, binding, AnimationCurve.Linear(0, value, 1.0f / 60, value));
            }
            foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                UnityEngine.Object value;
                AnimationUtility.GetObjectReferenceValue(avatar, binding, out value);
                ObjectReferenceKeyframe[] objectReferenceKeyframes = {
                                new ObjectReferenceKeyframe() { time = 0, value = value },
                                new ObjectReferenceKeyframe() { time = 1.0f / 60, value = value }
                            };
                AnimationUtility.SetObjectReferenceCurve(result, binding, objectReferenceKeyframes);
            }
            return result;
        }

        public static BlendTree Generate1DBlendTree(string name, string paramaName, Motion motion1, Motion motion2)
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
        /// 判断一个动画是否为空
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static bool AnimationIsEmpty(AnimationClip clip)
        {
            return !clip || (AnimationUtility.GetCurveBindings(clip).Length == 0 && AnimationUtility.GetCurveBindings(clip).Length == 0);
        }
        
        /// <summary>
        /// 获取binding
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static EditorCurveBinding GetBinding(string targetPath, EasyProperty property)
        {
            if (property.isPPtr)
                return EditorCurveBinding.PPtrCurve(targetPath, EasyReflection.FindType(property.targetPropertyType), property.targetProperty);
            else if (property.isDiscrete)
                return EditorCurveBinding.DiscreteCurve(targetPath, EasyReflection.FindType(property.targetPropertyType), property.targetProperty);
            return EditorCurveBinding.FloatCurve(targetPath, EasyReflection.FindType(property.targetPropertyType), property.targetProperty);
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
}
