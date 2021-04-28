using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace EasyAvatar
{
    public class EasyGesture : MonoBehaviour
    {
        public enum GestureType
        {
            Neutral,
            Fist,
            HandOpen,
            FingerPoint,
            Victory,
            RockNRoll,
            HandGun,
            ThumbsUp
        }

        public enum HandType
        {
            Left,
            Right,
            Any
        }

        
        [SerializeField]
        public GestureType gestureType;
        [SerializeField]
        public HandType handType;
        [SerializeField]
        public bool autoRestore = true;
        [SerializeField]
        public bool autoTrackingControl = true;
        [SerializeField]
        public EasyTrackingControl offTrackingControl, onTrackingControl;
        [SerializeField]
        public EasyBehaviorGroup behaviors1, behaviors2;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
