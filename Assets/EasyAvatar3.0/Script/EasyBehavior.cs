using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    [Serializable]
    public class EasyBehavior
    {
        //public List<EasyProperty> propertyGroup;
        public EasyProperty property;
        public UnityEngine.Object objectValue;
        public bool boolValue;
        public float floatValue;
        public int intValue;
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
        
    }

    [Serializable]
    public class EasyProperty
    {
        public string targetPath, targetProperty, targetPropertyType, valueType;
        public bool isDiscrete, isPPtr;

        public static EditorCurveBinding GetBinding(SerializedProperty property)
        {
            SerializedProperty targetPath = property.FindPropertyRelative("targetPath");
            SerializedProperty targetProperty = property.FindPropertyRelative("targetProperty");
            SerializedProperty targetPropertyType = property.FindPropertyRelative("targetPropertyType");
            SerializedProperty isDiscrete = property.FindPropertyRelative("isDiscrete");
            SerializedProperty isPPtr = property.FindPropertyRelative("isPPtr");
            if (isPPtr.boolValue)//如果isPPtr为true那么isDiscrete一定为true,参考：https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/Animation/EditorCurveBinding.bindings.cs
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

        public static void ClearProperty(SerializedProperty property)
        {
            //不能清除targetPath
            property.FindPropertyRelative("targetProperty").stringValue ="";
            property.FindPropertyRelative("targetPropertyType").stringValue = "";
            property.FindPropertyRelative("valueType").stringValue = "";
            property.FindPropertyRelative("isDiscrete").boolValue = false;
            property.FindPropertyRelative("isPPtr").boolValue = false;
        }

        public static void NicifyPropertyGroupName()
        {

        }
    }


}

