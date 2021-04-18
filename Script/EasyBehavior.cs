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
        public bool body;
        public bool rightArm;
        public bool leftArm;
        public bool rightFingers;
        public bool leftFingers;
        public bool rightLeg;
        public bool leftLeg;
        public bool fx;
    }

    [Serializable]
    public class EasyTrackingControl
    {
        public enum Type
        {
            NoChange,
            Tracking,
            Animation
        }
        public Type head;
        public Type mouth;
        public Type eyes;
        public Type hip;
        public Type rightHand;
        public Type leftHand;
        public Type rightFingers;
        public Type leftFingers;
        public Type rightFoot;
        public Type leftFoot;
    }


}
