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
                foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
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
        /// 判断一个动画是否为空
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static bool AnimationIsEmpty(AnimationClip clip)
        {
            return !clip || (AnimationUtility.GetCurveBindings(clip).Length == 0 && AnimationUtility.GetCurveBindings(clip).Length == 0);
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

            return new AnimationClip[] { humanAnim, fxAnim };
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
}
