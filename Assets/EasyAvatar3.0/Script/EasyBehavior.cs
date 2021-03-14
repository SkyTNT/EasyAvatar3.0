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
        public EasyProperty property;
        public UnityEngine.Object objectValue;
        public bool boolValue;
        public float floatValue;
        public float intValue;
        public EasyBehavior()
        {

        }

        public static void GenerateAnimClip(string path,List<EasyBehavior> behaviors)
        {
            
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
            return EasyReflection.FindType(property.FindPropertyRelative("valueType").stringValue);
        }
    }


}

