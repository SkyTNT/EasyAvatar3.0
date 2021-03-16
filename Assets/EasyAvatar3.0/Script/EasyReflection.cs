using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EasyAvatar
{
    public class EasyReflection
    {
        static Dictionary<string, Type> types = new Dictionary<string, Type>();
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

        public static MethodInfo internalNicifyPropertyGroupName;

        static EasyReflection()
        {
            internalNicifyPropertyGroupName = FindType("UnityEditorInternal.AnimationWindowUtility").GetMethod("NicifyPropertyGroupName", BindingFlags.Static | BindingFlags.Public);
        }
            
    }
}
