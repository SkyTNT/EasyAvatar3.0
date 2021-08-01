using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace EasyAvatar
{
    public class EasyLocomotionManager : MonoBehaviour
    {
        public bool useAnimatorController;
        public AnimatorController controller;

        public bool useStandBlendTree;
        public BlendTree standBlendTree;
        public EasyLocomotion standStill;
        public EasyLocomotion walkForward;
        public EasyLocomotion walkBackward;
        public EasyLocomotion walkLeft;
        public EasyLocomotion walkRight;
        public EasyLocomotion walkForwardLeft;
        public EasyLocomotion walkForwardRight;
        public EasyLocomotion walkBackwardLeft;
        public EasyLocomotion walkBackwardRight;
        public EasyLocomotion runForward;
        public EasyLocomotion runBackward;
        public EasyLocomotion runLeft;
        public EasyLocomotion runRight;
        public EasyLocomotion runForwardLeft;
        public EasyLocomotion runForwardRight;
        public EasyLocomotion runBackwardLeft;
        public EasyLocomotion runBackwardRight;
        public EasyLocomotion sprintForward;

        public bool useCrouchBlendTree;
        public BlendTree crouchBlendTree;
        public EasyLocomotion crouchStill;
        public EasyLocomotion crouchForward;
        public EasyLocomotion crouchBackward;
        public EasyLocomotion crouchLeft;
        public EasyLocomotion crouchRight;
        public EasyLocomotion crouchForwardLeft;
        public EasyLocomotion crouchForwardRight;
        public EasyLocomotion crouchBackwardLeft;
        public EasyLocomotion crouchBackwardRight;

        public bool useProneBlendTree;
        public BlendTree proneBlendTree;
        public EasyLocomotion proneStill;
        public EasyLocomotion proneForward;
        public EasyLocomotion proneBackward;
        public EasyLocomotion proneLeft;
        public EasyLocomotion proneRight;

        public EasyLocomotion shortFall;
        public EasyLocomotion longFall;
        public EasyLocomotion quickLand;
        public EasyLocomotion land;

        public EasyLocomotion afk;
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


