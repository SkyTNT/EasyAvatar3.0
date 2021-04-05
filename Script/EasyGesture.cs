using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public List<EasyBehavior> behaviors1, behaviors2;
        [SerializeField]
        public List<AnimationClip> animations1, animations2;
        [SerializeField]
        public GestureType gestureType;
        [SerializeField]
        public HandType handType;
        [SerializeField]
        public bool useAnimClip;
        [SerializeField]
        public bool autoRestore = true;

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
