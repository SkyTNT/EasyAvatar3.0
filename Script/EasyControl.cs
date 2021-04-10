using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace EasyAvatar
{
    public class EasyControl : MonoBehaviour
    {
        public enum Type
        {
            Toggle,
            RadialPuppet
        }

        [SerializeField]
        public Type type;
        [SerializeField]
        public Texture2D icon;
        [SerializeField]
        public bool autoRestore = true;
        [SerializeField]
        public bool autoTrackingControl = true;
        [SerializeField]
        public VRCAnimatorTrackingControl offTrackingControl, onTrackingControl;
        [SerializeField]
        public List<EasyBehaviors> behaviors;//Toggle型的为2个，RadialPuppet型的为3个

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
