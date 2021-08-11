using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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


        /// <summary>
        /// 检查曲线是否在遮罩中开启
        /// </summary>
        /// <param name="binding">曲线</param>
        /// <param name="mask">遮罩</param>
        /// <returns></returns>
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
        public static AnimationClip GenerateAnimClip(List<EasyBehavior> behaviors, bool preview = false)
        {
            AnimationClip clip = new AnimationClip();
            clip.frameRate = 60;
            bool isProxy = false;

            if (behaviors == null)
                return clip;

            int behaviorsCount = behaviors.Count;
            for (int i = 0; i < behaviorsCount; i++)
            {
                EasyBehavior behavior = behaviors[i];
                EasyPropertyGroup propertyGroup = behavior.propertyGroup;
                if (isProxy)
                {
                    clip = MergeAnimClip(clip);//复制，防止修改proxy动画
                    isProxy = false;
                }
                if (behavior.type == EasyBehavior.Type.Property)
                {
                    
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

                        if (clip.empty&& Path.GetFileName(AssetDatabase.GetAssetPath(behavior.anim)).StartsWith("proxy"))//proxy动画
                        {
                            clip = behavior.anim;
                            isProxy = true;
                            continue;
                        }
                            

                        //不能用foreach，foreach不能修改变量成员
                        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(behavior.anim);
                        for (int j = 0; j < curveBindings.Length; j++)
                        {
                            EditorCurveBinding binding = curveBindings[j];
                            if (CheckCurveBinding(binding, behavior.mask))
                            {
                                /*暂时还为完善
                                if (behavior.mirror)
                                {
                                    //有同时出现LeftRight的情况
                                    binding.propertyName = binding.propertyName.Replace("Left", "R1i1g1h1t");
                                    binding.propertyName = binding.propertyName.Replace("Right", "Left");
                                    binding.propertyName = binding.propertyName.Replace("R1i1g1h1t", "Right");
                                }*/
                                AnimationUtility.SetEditorCurve(clip, binding, AnimationUtility.GetEditorCurve(behavior.anim, binding));
                            }
                        }
                           
                                
                        foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(behavior.anim))
                            if (CheckCurveBinding(binding, behavior.mask))
                                AnimationUtility.SetObjectReferenceCurve(clip, binding, AnimationUtility.GetObjectReferenceCurve(behavior.anim, binding));
                    }
                }
                else if (behavior.type == EasyBehavior.Type.ToggleObject)
                {
                    if (propertyGroup.targetPath == "")
                        continue;

                    EditorCurveBinding binding = EditorCurveBinding.FloatCurve(propertyGroup.targetPath, typeof(GameObject), "m_IsActive");
                    float value = behavior.isActive ? 1 : 0;
                    AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(0, value, 1.0f / 60, value));
                }
                else if (behavior.type == EasyBehavior.Type.ToggleMusic)
                {
                    if (!behavior.audio||preview)
                        continue;
                    string path = EasyAvatarAsset.GetPathRelateToAvatar(behavior.audio);
                    EditorCurveBinding binding = EditorCurveBinding.FloatCurve(path, typeof(GameObject), "m_IsActive");
                    float value = behavior.isActive ? 1 : 0;
                    AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(0, value, 1.0f / 60, value));
                }
                else if (behavior.type == EasyBehavior.Type.MusicVolume)
                {
                    if (!behavior.audio || preview)
                        continue;
                    string path = EasyAvatarAsset.GetPathRelateToAvatar(behavior.audio);
                    EditorCurveBinding binding = EditorCurveBinding.FloatCurve(path, typeof(AudioSource), "m_Volume");
                    AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(0, behavior.volume, 1.0f / 60, behavior.volume));
                }

            }

            return clip;
        }

        /// <summary>
        /// 获取当前模型的动作动画
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public static AnimationClip GenerateCurrentAnimClip(GameObject avatar)
        {
            AnimationClip clip = new AnimationClip();
            HumanPose humanPose = new HumanPose();
            HumanPoseHandler humanPoseHandler = new HumanPoseHandler(avatar.GetComponent<Animator>().avatar, avatar.transform);
            humanPoseHandler.GetHumanPose(ref humanPose);
            string[] names = HumanTrait.MuscleName;
            for (int i=0;i< HumanTrait.MuscleCount; i++)
            {
                string name = names[i];
                if (Regex.IsMatch(name, "Index|Thumb|Little|Middle|Ring"))
                {
                    name = name.Replace("Left ", "LeftHand.");
                    name = name.Replace("Right ", "RightHand.");
                    name = name.Replace("Index ", "Index.");
                    name = name.Replace("Thumb ", "Thumb.");
                    name = name.Replace("Little ", "Little.");
                    name = name.Replace("Middle ", "Middle.");
                    name = name.Replace("Ring ", "Ring.");
                }

                addToClip(name, humanPose.muscles[i]);
            }

            addToClip("RootT.x",humanPose.bodyPosition.x);
            addToClip("RootT.y", humanPose.bodyPosition.y);
            addToClip("RootT.z", humanPose.bodyPosition.z);
            addToClip("RootQ.x", humanPose.bodyRotation.x);
            addToClip("RootQ.y", humanPose.bodyRotation.y);
            addToClip("RootQ.z", humanPose.bodyRotation.z);
            addToClip("RootQ.w", humanPose.bodyRotation.w);

            void addToClip(string name , float value)
            {
                EditorCurveBinding binding = EditorCurveBinding.FloatCurve("", typeof(Animator), name);
                AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(0, value, 1.0f / 60, value));
            }
            return clip;
        }

        /// <summary>
        /// 生成恢复到默认状态的动画
        /// </summary>
        /// <param name="avatar">avatar</param>
        /// <param name="clip">clip</param>
        /// <returns></returns>
        public static AnimationClip GenerateRestoreAnimClip(GameObject avatar, Motion motion)
        {
            AnimationClip result = new AnimationClip();
            result.frameRate = 60;

            AnimationClip clip = motion as AnimationClip;
            if (clip)
            {
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
            }

            BlendTree blendTree = motion as BlendTree;
            if (blendTree)
            {
                foreach (var child in blendTree.children)
                    result = MergeAnimClip(result, GenerateRestoreAnimClip(avatar, child.motion));
            }

            return result;
        }

        public static BlendTree Generate1DBlendTree(string name, string xParamaName, List<float> positions, List<Motion> motions)
        {
            BlendTree blendTree = new BlendTree();
            blendTree.blendType = BlendTreeType.Simple1D;
            blendTree.blendParameter = xParamaName;
            blendTree.name = name;
            blendTree.useAutomaticThresholds = false;
            
            if (positions.Count != motions.Count)
            {
                Debug.LogError("count of positions is not equal to count of motions");
                return blendTree;
            }
            int count = positions.Count;
            for (int i = 0; i < count; i++)
            {
                blendTree.AddChild(motions[i], positions[i]);
            }
            
            return blendTree;
        }

        public static BlendTree Generate2DBlendTree(string name, string xParamaName, string yParamaName, List<Vector2> positions, List<Motion> motions)
        {
            BlendTree blendTree = new BlendTree();
            blendTree.blendType = BlendTreeType.SimpleDirectional2D;
            blendTree.blendParameter = xParamaName;
            blendTree.blendParameterY = yParamaName;
            blendTree.name = name;
            if (positions.Count != motions.Count)
            {
                Debug.LogError("count of positions is not equal to count of motions");
                return blendTree;
            }
            int count = positions.Count;
            for (int i = 0; i < count; i++)
            {
                blendTree.AddChild(motions[i], positions[i]);
            }
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
        /// 分离人体动画和非人体动画
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="action"></param>
        /// <param name="fx"></param>
        public static void SeparateAnimation(AnimationClip clip, out AnimationClip action, out AnimationClip fx)
        {
            action = new AnimationClip();
            fx = new AnimationClip();
            action.frameRate = 60;
            fx.frameRate = 60;

            if (Path.GetFileName(AssetDatabase.GetAssetPath(clip)).StartsWith("proxy"))
            {
                action = clip;
                return;
            }
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

        public static void SeparateBlendTree(BlendTree blendTree, out BlendTree action, out BlendTree fx)
        {
            action = new BlendTree();
            fx = new BlendTree();
            fx.blendType = action.blendType = blendTree.blendType;
            fx.blendParameter = action.blendParameter = blendTree.blendParameter;
            fx.blendParameterY = action.blendParameterY = blendTree.blendParameterY;
            fx.useAutomaticThresholds = action.useAutomaticThresholds = blendTree.useAutomaticThresholds;
            fx.minThreshold = action.minThreshold = blendTree.minThreshold;
            fx.maxThreshold = action.maxThreshold = blendTree.maxThreshold;
            action.name = blendTree.name + "_action";
            fx.name = blendTree.name + "fx";
            List<ChildMotion> actionChildren = new List<ChildMotion>();
            List<ChildMotion> fxChildren = new List<ChildMotion>();
            foreach (var child in blendTree.children)
            {
                AnimationClip clip = child.motion as AnimationClip;
                if (clip)
                {
                    
                    SeparateAnimation(clip, out AnimationClip actionAnim, out AnimationClip fxAnim);
                    actionChildren.Add(CopyChild(actionAnim));
                    fxChildren.Add(CopyChild(fxAnim));
                }
                BlendTree blendTree1 = child.motion as BlendTree;
                if (blendTree1)
                {
                    SeparateBlendTree(blendTree1, out BlendTree actionBlend, out BlendTree fxBlend);
                    actionChildren.Add(CopyChild(actionBlend));
                    fxChildren.Add(CopyChild(fxBlend));
                }

                ChildMotion CopyChild(Motion motion)
                {
                    return new ChildMotion { motion = motion, cycleOffset = child.cycleOffset, directBlendParameter = child.directBlendParameter, mirror = child.mirror, position = child.position, threshold = child.threshold, timeScale = child.timeScale };
                }
            }
            action.children = actionChildren.ToArray();
            fx.children = fxChildren.ToArray();
        }

        public static void SeparateMotion(Motion motion, out Motion action, out Motion fx)
        {
            action = null;
            fx = null;
            AnimationClip clip = motion as AnimationClip;
            if (clip)
            {
                SeparateAnimation(clip, out AnimationClip actionClip, out AnimationClip fxClip);
                action = actionClip;
                fx = fxClip;
            }
            BlendTree blendTree = motion as BlendTree;
            if(blendTree)
            {
                SeparateBlendTree(blendTree, out BlendTree actionBlend, out BlendTree fxBlend);
                action = actionBlend;
                fx = fxBlend;
            }
                
        }

        /// <summary>
        /// 判断一个动画是否为空
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static bool MotionIsEmpty(Motion motion)
        {

            AnimationClip clip = motion as AnimationClip;
            if (clip)
                return clip.empty;

            BlendTree blendTree = motion as BlendTree;
            if (blendTree)
            {
                foreach (var child in blendTree.children)
                    if (!MotionIsEmpty(child.motion))
                        return false;
            }

            return true;
        }

        public static T CopyAsset<T>(string path , T asset) where T : UnityEngine.Object
        {
            if (!AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(asset), path))
            {
                Debug.Log("复制失败" + path+"从"+AssetDatabase.GetAssetPath(asset));
                return null;
            }
                
            return AssetDatabase.LoadAssetAtPath<T>(path);
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