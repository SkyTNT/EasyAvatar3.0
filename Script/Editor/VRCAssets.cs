using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace EasyAvatar
{
    public class VRCAssets
    {
        private static string animPath = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";
        private static string maskPath = "Assets/VRCSDK/Examples3/Animation/Masks/";
        private static string controllerPath = "Assets/VRCSDK/Examples3/Animation/Controllers/";
        private static string blendTreePath = "Assets/VRCSDK/Examples3/Animation/BlendTrees/";

        public static AnimationClip
            proxy_hands_idle,
            proxy_hands_fist,
            proxy_hands_gun,
            proxy_hands_open,
            proxy_hands_peace,
            proxy_hands_point,
            proxy_hands_rock,
            proxy_hands_thumbs_up,
            proxy_stand_still,
            proxy_walk_forward,
            proxy_walk_backward,
            proxy_strafe_right,
            proxy_strafe_right_45,
            proxy_strafe_right_135,
            proxy_run_forward,
            proxy_run_backward,
            proxy_run_strafe_right,
            proxy_run_strafe_right_45,
            proxy_run_strafe_right_135,
            proxy_sprint_forward,
            proxy_crouch_still,
            proxy_crouch_walk_forward,
            proxy_crouch_walk_right,
            proxy_crouch_walk_right_45,
            proxy_crouch_walk_right_135,
            proxy_low_crawl_still,
            proxy_low_crawl_forward,
            proxy_low_crawl_right,
            proxy_fall_short,
            proxy_fall_long,
            proxy_land_quick,
            proxy_landing,
            proxy_afk,
            proxy_tpose;

        public static AvatarMask hands_only, hand_left, hand_right;

        public static AnimatorController locomotionController;
        public static BlendTree standBlendTree, crouchBlendTree, proneBlendTree;

        static VRCAssets()
        {
            proxy_hands_idle = GetProxyAnim("proxy_hands_idle");
            proxy_hands_fist = GetProxyAnim("proxy_hands_fist");
            proxy_hands_gun = GetProxyAnim("proxy_hands_gun");
            proxy_hands_open = GetProxyAnim("proxy_hands_open");
            proxy_hands_peace = GetProxyAnim("proxy_hands_peace");
            proxy_hands_point = GetProxyAnim("proxy_hands_point");
            proxy_hands_rock = GetProxyAnim("proxy_hands_rock");
            proxy_hands_thumbs_up = GetProxyAnim("proxy_hands_thumbs_up");
            proxy_stand_still = GetProxyAnim("proxy_stand_still");
            proxy_walk_forward = GetProxyAnim("proxy_walk_forward");
            proxy_walk_backward = GetProxyAnim("proxy_walk_backward");
            proxy_strafe_right = GetProxyAnim("proxy_strafe_right");
            proxy_strafe_right_45 = GetProxyAnim("proxy_strafe_right_45");
            proxy_strafe_right_135 = GetProxyAnim("proxy_strafe_right_135");
            proxy_run_forward = GetProxyAnim("proxy_run_forward");
            proxy_run_backward = GetProxyAnim("proxy_run_backward");
            proxy_run_strafe_right = GetProxyAnim("proxy_run_strafe_right");
            proxy_run_strafe_right_45 = GetProxyAnim("proxy_run_strafe_right_45");
            proxy_run_strafe_right_135 = GetProxyAnim("proxy_run_strafe_right_135");
            proxy_sprint_forward = GetProxyAnim("proxy_sprint_forward");
            proxy_crouch_still = GetProxyAnim("proxy_crouch_still");
            proxy_crouch_walk_forward = GetProxyAnim("proxy_crouch_walk_forward");
            proxy_crouch_walk_right = GetProxyAnim("proxy_crouch_walk_right");
            proxy_crouch_walk_right_45 = GetProxyAnim("proxy_crouch_walk_right_45");
            proxy_crouch_walk_right_135 = GetProxyAnim("proxy_crouch_walk_right_135");
            proxy_low_crawl_still = GetProxyAnim("proxy_low_crawl_still");
            proxy_low_crawl_forward = GetProxyAnim("proxy_low_crawl_forward");
            proxy_low_crawl_right = GetProxyAnim("proxy_low_crawl_right");
            proxy_fall_short = GetProxyAnim("proxy_fall_short");
            proxy_fall_long = GetProxyAnim("proxy_fall_long");
            proxy_land_quick = GetProxyAnim("proxy_land_quick");
            proxy_landing = GetProxyAnim("proxy_landing");
            proxy_afk = GetProxyAnim("proxy_afk");
            proxy_tpose = GetProxyAnim("proxy_tpose");

            hands_only = GetAvatarMask("vrc_HandsOnly");
            hand_right = GetAvatarMask("vrc_Hand Right");
            hand_left = GetAvatarMask("vrc_Hand Left");

            locomotionController = GetController("vrc_AvatarV3LocomotionLayer");
            standBlendTree = GetBlendTree("vrc_StandingLocomotion");
            crouchBlendTree = GetBlendTree("vrc_CrouchingLocomotion");
            proneBlendTree = GetBlendTree("vrc_ProneLocomotion");
        }

        /// <summary>
        /// 获取vrc的代理动画
        /// </summary>
        /// <param name="name">名字，不包含后缀</param>
        /// <returns></returns>
        public static AnimationClip GetProxyAnim(string name)
        {
            AnimationClip animation = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath + name + ".anim");
            return animation;
        }

        public static AvatarMask GetAvatarMask(string name)
        {
            AvatarMask mask = AssetDatabase.LoadAssetAtPath<AvatarMask>(maskPath + name + ".mask");
            return mask;
        }

        public static AnimatorController GetController(string name)
        {
            AnimatorController mask = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + name + ".controller");
            return mask;
        }

        public static BlendTree GetBlendTree(string name)
        {
            BlendTree mask = AssetDatabase.LoadAssetAtPath<BlendTree>(blendTreePath + name + ".asset");
            return mask;
        }
    }
}


