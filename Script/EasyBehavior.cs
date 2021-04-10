using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyAvatar
{

    [Serializable]
    public class EasyBehaviors
    {
        public List<EasyBehavior> list;
    }
    [Serializable]
    public class EasyBehavior
    {
        public enum Type
        {
            Property,
            AnimationClip
        }

        public Type type;
        public List<EasyProperty> propertyGroup;
        public AnimationClip anim;
        public EasyAnimationMask mask;
    }

    [Serializable]
    public class EasyProperty
    {
        public string targetPath, targetProperty, targetPropertyType, valueType;
        public bool isDiscrete, isPPtr;

        public UnityEngine.Object objectValue;
        public float floatValue;

    }

    [Serializable]
    public class EasyAnimationMask
    {
        public bool head;
        public bool mouth;
        public bool eyes;
        public bool hip;
        public bool rightHand;
        public bool leftHand;
        public bool rightFingers;
        public bool leftFingers;
        public bool rightFoot;
        public bool leftFoot;
        public bool fx;
    }

    [Serializable]
    class EasyTrackingControl
    {
       
    }


}

