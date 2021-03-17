using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace EasyAvatar
{
    [Serializable]
    public class EasyBehavior
    {
        public List<EasyProperty> propertyGroup;

        public EasyBehavior()
        {

        }

        public static AnimationClip GenerateAnimClip(string path, SerializedProperty behaviors)
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

                    EditorCurveBinding binding = EasyProperty.GetBinding(property);

                    if (! binding.isPPtrCurve)
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
            AssetDatabase.CreateAsset(clip, path);
            AssetDatabase.SaveAssets();
            return clip;
        }


        /*public static void Preview(GameObject root, SerializedProperty behaviors)
        {
            Animator animator = root.GetOrAddComponent<Animator>();
            AnimationClip clip = GenerateAnimClip("Assets/preview.anim", behaviors);

            AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPathWithClip("Assets/preview.controller", clip);
            animator.runtimeAnimatorController = animatorController;
            animator.Play("preview",0,1);
        }*/


        ///TODO : Render中的material不能预览，transform的欧拉角不能预览
        public static void Preview(GameObject root, SerializedProperty behaviors)
        {
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

                    EditorCurveBinding binding = EasyProperty.GetBinding(property);
                    UnityEngine.Object target = root.transform.Find(binding.path).gameObject;
                    if (binding.type != typeof(GameObject))
                        target = ((GameObject)target).GetComponent(binding.type);

                    SerializedObject serializedObject = new SerializedObject(target);
                    serializedObject.Update();
                    SerializedProperty targetValue;
                    if (true)//list情况
                    {
                        SerializedProperty temp = null;
                        foreach (string subpath in binding.propertyName.Split('.'))
                        {
                            if (temp == null)
                                temp = serializedObject.FindProperty(subpath);
                            else
                            {
                                if (subpath.Contains("["))
                                {
                                    int startInext = subpath.IndexOf('[') + 1;
                                    temp = temp.GetArrayElementAtIndex(Convert.ToInt32(subpath.Substring(startInext, subpath.Length -startInext - 1)));
                                }
                                else
                                    temp = temp.FindPropertyRelative(subpath);
                                
                            }
                            //Debug.Log(binding.propertyName);
                            //Debug.Log(subpath);
                            //Debug.Log(temp.type);
                        }
                        targetValue = temp;
                    }
                    //else
                        //targetValue = serializedObject.FindProperty(binding.propertyName);

                    SerializedProperty value;
                    Type valueType = EasyProperty.GetValueType(property);

                    if (!binding.isPPtrCurve)
                    {
                        value = property.FindPropertyRelative("floatValue");
                        if (valueType == typeof(bool))
                            targetValue.boolValue = Convert.ToBoolean(value.floatValue);
                        else if (valueType == typeof(float))
                            targetValue.floatValue = value.floatValue;
                        else if (valueType == typeof(int))
                            targetValue.intValue = Convert.ToInt32(value.floatValue);
                        else if (valueType == typeof(long))
                            targetValue.longValue = Convert.ToInt64(value.floatValue);
                    }
                    else
                    {
                        targetValue.objectReferenceValue = property.FindPropertyRelative("objectValue").objectReferenceValue;
                    }
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public static void Copy(SerializedProperty dest, SerializedProperty src)
        {
            SerializedProperty destPropertyGroup = dest.FindPropertyRelative("propertyGroup");
            SerializedProperty srcPropertyGroup = src.FindPropertyRelative("propertyGroup");
            destPropertyGroup.arraySize = srcPropertyGroup.arraySize;

            for (int i = 0; i < destPropertyGroup.arraySize; i++)
            {
                EasyProperty.Copy(destPropertyGroup.GetArrayElementAtIndex(i), srcPropertyGroup.GetArrayElementAtIndex(i));
            }
        }
    }

    [Serializable]
    public class EasyProperty
    {
        public string targetPath, targetProperty, targetPropertyType, valueType;
        public bool isDiscrete, isPPtr;

        public UnityEngine.Object objectValue;
        public float floatValue;

        public static void Copy(SerializedProperty dest, SerializedProperty src)
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

        public static EditorCurveBinding GetBinding(SerializedProperty property)
        {
            SerializedProperty targetPath = property.FindPropertyRelative("targetPath");
            SerializedProperty targetProperty = property.FindPropertyRelative("targetProperty");
            SerializedProperty targetPropertyType = property.FindPropertyRelative("targetPropertyType");
            SerializedProperty isDiscrete = property.FindPropertyRelative("isDiscrete");
            SerializedProperty isPPtr = property.FindPropertyRelative("isPPtr");
            if (isPPtr.boolValue)
                return EditorCurveBinding.PPtrCurve(targetPath.stringValue, EasyReflection.FindType(targetPropertyType.stringValue), targetProperty.stringValue);
            else if(isDiscrete.boolValue)
                return EditorCurveBinding.DiscreteCurve(targetPath.stringValue, EasyReflection.FindType(targetPropertyType.stringValue), targetProperty.stringValue);
            return EditorCurveBinding.FloatCurve(targetPath.stringValue, EasyReflection.FindType(targetPropertyType.stringValue), targetProperty.stringValue);
        }
        

        public static Type GetValueType(SerializedProperty property)
        {
            string valueTypeName = property.FindPropertyRelative("valueType").stringValue;
            return valueTypeName == "" ? null : EasyReflection.FindType(valueTypeName);
        }

        public static bool CheckProperty(GameObject avatar, SerializedProperty property)
        {
            
            if (!avatar || property.FindPropertyRelative("valueType").stringValue == "" || property.FindPropertyRelative("targetProperty").stringValue == "")
                return false;
            return AnimationUtility.GetEditorCurveValueType(avatar, GetBinding(property)) != null;
        }

        public static void ClearPropertyGroup(SerializedProperty propertyGroup)
        {
            string targetPath = propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("targetPath").stringValue;
            propertyGroup.ClearArray();
            propertyGroup.arraySize++;
            propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("targetPath").stringValue = targetPath;
            
            
        }
        public static void PropertyGroupEdit(SerializedProperty propertyGroup, string relativePath, string value)
        {
            for (int i = 0; i < propertyGroup.arraySize; i++)
            {
                SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i).FindPropertyRelative(relativePath);
                property.stringValue = value;
            }
        }

        public static void PropertyGroupEdit(SerializedProperty propertyGroup, string relativePath, bool value)
        {
            for (int i = 0; i < propertyGroup.arraySize; i++)
            {
                SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i).FindPropertyRelative(relativePath);
                property.boolValue = value;
            }
        }

        public static string NicifyPropertyGroupName(Type animatableObjectType, string propertyGroupName)
        {

            return (string)EasyReflection.internalNicifyPropertyGroupName.Invoke(null, new object[] { animatableObjectType, propertyGroupName });
        }

        public static string GetPropertyGroupSubname(SerializedProperty propertyGroup, int index)
        {
            return GetPropertyGroupSubname(propertyGroup.GetArrayElementAtIndex(index));
        }

        public static string GetPropertyGroupSubname(SerializedProperty property)
        {
            string targetProperty = property.FindPropertyRelative("targetProperty").stringValue;
            return targetProperty.Substring(targetProperty.Length - 1);
        }

        public static bool PropertyGroupIsColor(SerializedProperty propertyGroup)
        {
            string targetProperty = propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("targetProperty").stringValue;

            return targetProperty.ToLower().Contains("color") && propertyGroup.arraySize == 4;
        }

        public static bool PropertyGroupIsBlendShape(SerializedProperty propertyGroup)
        {
            SerializedProperty property = propertyGroup.GetArrayElementAtIndex(0);
            string targetProperty = property.FindPropertyRelative("targetProperty").stringValue;
            string targetPropertyType = property.FindPropertyRelative("targetPropertyType").stringValue;
            return targetPropertyType.Contains("SkinnedMeshRenderer") && targetProperty.ToLower().Contains("blendshape") && propertyGroup.arraySize == 1;
        }
    }


}

