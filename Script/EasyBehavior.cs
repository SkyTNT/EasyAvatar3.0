using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyAvatar
{

    [Serializable]
    public class EasyBehaviorGroup
    {
        public List<EasyBehavior> list = new List<EasyBehavior>();
        public Vector2 position =new Vector2();//EasyBehaviorGroup放在TwoAxisPuppet,RadialPuppet才会用到
        public bool hide;//是否再inspector中隐藏
    }
    [Serializable]
    public class EasyBehavior
    {
        public enum Type
        {
            Property,
            AnimationClip,
            ToggleMusic,
            MusicVolume,
            ToggleObject,
        }

        public Type type;
        public EasyPropertyGroup propertyGroup;
        public AnimationClip anim;
        public EasyAnimationMask mask;
        public bool mirror;
        public bool isActive;
        public float volume;
        public AudioClip audio;
    }

    [Serializable]
    public class EasyPropertyGroup
    {
        public string targetPath;//targetPath在替换avatar时找到对应GameObject，方便移植
        public List<EasyProperty> properties = new List<EasyProperty>();
        public GameObject tempTarget;//tempTarget在Build时转换为targetPath
    }

    [Serializable]
    public class EasyProperty
    {
        public string targetProperty, targetPropertyType, valueType;
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
