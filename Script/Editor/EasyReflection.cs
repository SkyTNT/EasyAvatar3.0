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
        /// SerializedProperty获取对应实例
        /// </summary>
        /// <typeparam name="T">对应类型</typeparam>
        /// <param name="serializedProperty">SerializedProperty</param>
        /// <returns></returns>
        public static T GetObject<T>(this SerializedProperty serializedProperty)
        {
            object target = serializedProperty.serializedObject.targetObject;
            Type type = target.GetType();
            bool isArray = false;
            //按照路径寻找字段
            foreach (var name in serializedProperty.propertyPath.Split('.'))
            {
                if(name == "Array")//数组类型
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
        /// Uinty内部美化PropertyGroup名
        /// </summary>
        public static MethodInfo internalNicifyPropertyGroupName;

        static EasyReflection()
        {
            
            internalNicifyPropertyGroupName = FindType("UnityEditorInternal.AnimationWindowUtility").GetMethod("NicifyPropertyGroupName", BindingFlags.Static | BindingFlags.Public);
        }
            
    }
}
