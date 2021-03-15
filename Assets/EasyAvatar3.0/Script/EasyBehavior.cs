using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
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

        public static void GenerateAnimClip(string path, SerializedProperty behaviors)
        {
            AnimationClip clip = new AnimationClip();
            clip.frameRate = 60;
            

            int count = behaviors.arraySize;
            for (int i = 0; i < count; i++)
            {
                SerializedProperty behavior = behaviors.GetArrayElementAtIndex(i);
                SerializedProperty property = behavior.FindPropertyRelative("property");
                EditorCurveBinding binding = EasyProperty.GetBinding(property);
                Type valueType = EasyProperty.GetValueType(property);
                if (valueType == null)
                    continue;
                
                float value = 0;
                if (valueType == typeof(bool))
                {
                    value =Convert.ToSingle(behavior.FindPropertyRelative("boolValue").boolValue);
                    AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(1.0f / 60, value, 2.0f / 60, value));
                }
                else if (valueType == typeof(float))
                {
                    value = behavior.FindPropertyRelative("floatValue").floatValue;
                    AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(1.0f / 60, value, 2.0f / 60, value));
                }
                else if (valueType == typeof(int))
                {
                    value = Convert.ToSingle(behavior.FindPropertyRelative("intValue").intValue);
                    AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(1.0f / 60, value, 2.0f / 60, value));
                }
                else//UnityEngine.Object
                {
                    UnityEngine.Object obj = behavior.FindPropertyRelative("objectValue").objectReferenceValue;
                    ObjectReferenceKeyframe[] objectReferenceKeyframes = {
                        new ObjectReferenceKeyframe() { time = 1.0f / 60, value = obj },
                        new ObjectReferenceKeyframe() { time = 2.0f / 60, value = obj }
                    };
                    AnimationUtility.SetObjectReferenceCurve(clip, binding, objectReferenceKeyframes);
                }
            }
            AssetDatabase.CreateAsset(clip, path);
            AssetDatabase.SaveAssets();
        }

        public static void PropertyGroupEdit(SerializedProperty propertyGroup ,string relativePath,string value)
        {
            for(int i = 0; i < propertyGroup.arraySize; i++)
            {
                SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i).FindPropertyRelative(relativePath);
                property.stringValue =value;
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

    }

    [Serializable]
    public class EasyProperty
    {
        public string targetPath, targetProperty, targetPropertyType, valueType;
        public bool isDiscrete, isPPtr;

        public UnityEngine.Object objectValue;
        public bool boolValue;
        public float floatValue;
        public int intValue;

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
        
    }


}

