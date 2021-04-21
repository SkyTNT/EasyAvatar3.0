using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    public static class EasyReflection
    {
        static Dictionary<string, Type> types = new Dictionary<string, Type>();

        /// <summary>
        /// 通过类型名获取类型
        /// </summary>
        /// <param name="typeName">类型名</param>
        /// <returns>类型</returns>
        public static Type FindType(string typeName)
        {
            Type _type = null;
            //通过字典提高效率
            if (types.ContainsKey(typeName))
                return types[typeName];
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                _type = assembly.GetType(typeName);
                if (_type != null)
                    break;
            }
            types.Add(typeName, _type);
            return _type;
        }

        /// <summary>
        /// Uinty内部美化PropertyGroup名
        /// </summary>
        public static MethodInfo internalNicifyPropertyGroupName;

        static EasyReflection()
        {
            
            internalNicifyPropertyGroupName = FindType("UnityEditorInternal.AnimationWindowUtility").GetMethod("NicifyPropertyGroupName", BindingFlags.Static | BindingFlags.Public);
        }
            
    }

    public static class SerializedPropertyExtension
    {

        /// <summary>
        /// SerializedProperty获取对应实例
        /// </summary>
        /// <typeparam name="T">对应类型</typeparam>
        /// <param name="serializedProperty">SerializedProperty</param>
        /// <returns></returns>
        public static T GetObject<T>(this SerializedProperty serializedProperty)
        {
            //如果修改了没有应用修改的话会读取不到修改后的值
            serializedProperty.serializedObject.ApplyModifiedProperties();
            object target = serializedProperty.serializedObject.targetObject;
            Type type = target.GetType();
            bool isArray = false;
            //按照路径寻找字段
            foreach (var name in serializedProperty.propertyPath.Split('.'))
            {
                if (name == "Array")//数组类型
                {
                    isArray = true;
                    continue;
                }
                if (isArray)//name为data[index]形式
                {
                    int startIndex = name.IndexOf('[') + 1;
                    int endIndex = name.IndexOf(']');
                    string indexStr = name.Substring(startIndex, endIndex - startIndex);
                    target = (target as IList)[Convert.ToInt32(indexStr)];
                    type = target.GetType();
                    isArray = false;
                    continue;
                }
                target = type.GetField(name).GetValue(target);
                type = target.GetType();
            }
            return (T)target;
        }

        /// <summary>
        /// 获取序列化属性的相对父级
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static SerializedProperty Parent(this SerializedProperty property)
        {
            string path = property.propertyPath;
            return property.serializedObject.FindProperty(path.Substring(0, path.LastIndexOf('.')));
        }

        /// <summary>
        /// 复制序列化属性
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        public static void CopyFrom(this SerializedProperty dest, SerializedProperty src)
        {
            //避免影响原来的迭代
            SerializedProperty src_c = src.Copy();
            string prefix = src_c.propertyPath;
            while (true)
            {
                //找到对应属性
                SerializedProperty property = dest;
                if (prefix != src_c.propertyPath)
                    property = property.FindPropertyRelative(src_c.propertyPath.Substring(prefix.Length + 1));
                if (property == null)
                    continue;

                switch (src_c.propertyType)
                {
                    case SerializedPropertyType.Generic:
                        break;
                    case SerializedPropertyType.Integer:
                        property.intValue = src_c.intValue;
                        break;
                    case SerializedPropertyType.Boolean:
                        property.boolValue = src_c.boolValue;
                        break;
                    case SerializedPropertyType.Float:
                        property.floatValue = src_c.floatValue;
                        break;
                    case SerializedPropertyType.String:
                        property.stringValue = src_c.stringValue;
                        break;
                    case SerializedPropertyType.Color:
                        property.colorValue = src_c.colorValue;
                        break;
                    case SerializedPropertyType.ObjectReference:
                        property.objectReferenceValue = src_c.objectReferenceValue;
                        break;
                    case SerializedPropertyType.LayerMask:
                        break;
                    case SerializedPropertyType.Enum:
                        property.enumValueIndex = src_c.enumValueIndex;
                        break;
                    case SerializedPropertyType.Vector2:
                        property.vector2Value = src_c.vector2Value;
                        break;
                    case SerializedPropertyType.Vector3:
                        property.vector3Value = src_c.vector3Value;
                        break;
                    case SerializedPropertyType.Vector4:
                        property.vector4Value = src_c.vector4Value;
                        break;
                    case SerializedPropertyType.Rect:
                        property.rectValue = src_c.rectValue;
                        break;
                    case SerializedPropertyType.ArraySize:
                        //去除.Array.size得到数组属性
                        property.Parent().Parent().arraySize = src_c.Parent().Parent().arraySize;
                        break;
                    case SerializedPropertyType.Character:
                        break;
                    case SerializedPropertyType.AnimationCurve:
                        property.animationCurveValue = src_c.animationCurveValue;
                        break;
                    case SerializedPropertyType.Bounds:
                        property.boundsValue = src_c.boundsValue;
                        break;
                    case SerializedPropertyType.Gradient:
                        break;
                    case SerializedPropertyType.Quaternion:
                        property.quaternionValue = src_c.quaternionValue;
                        break;
                    case SerializedPropertyType.ExposedReference:
                        property.exposedReferenceValue = src_c.exposedReferenceValue;
                        break;
                    case SerializedPropertyType.FixedBufferSize:
                        break;
                    case SerializedPropertyType.Vector2Int:
                        property.vector2IntValue = src_c.vector2IntValue;
                        break;
                    case SerializedPropertyType.Vector3Int:
                        property.vector3IntValue = src_c.vector3IntValue;
                        break;
                    case SerializedPropertyType.RectInt:
                        property.rectIntValue = src_c.rectIntValue;
                        break;
                    case SerializedPropertyType.BoundsInt:
                        property.boundsIntValue = src_c.boundsIntValue;
                        break;
                    default:
                        break;
                }
                if (!src_c.Next(true) || !src_c.propertyPath.StartsWith(prefix))
                {
                    break;
                }
            }
        }
    }

}
