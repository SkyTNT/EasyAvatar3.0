using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyAvatar
{
    [Serializable]
    public class EasyBehavior
    {
        public List<EasyProperty> propertyGroup;
        
    }

    [Serializable]
    public class EasyProperty
    {
        public string targetPath, targetProperty, targetPropertyType, valueType;
        public bool isDiscrete, isPPtr;

        public UnityEngine.Object objectValue;
        public float floatValue;

    }


}

