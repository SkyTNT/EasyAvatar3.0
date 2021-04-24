using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    public class VRCAssets
    {
        private static string animPath = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";
        private static string maskPath = "Assets/VRCSDK/Examples3/Animation/Masks/";
        public static AnimationClip 
            proxy_stand_still,
            proxy_afk,
            proxy_hands_idle,
            proxy_hands_fist,
            proxy_hands_gun,
            proxy_hands_open,
            proxy_hands_peace,
            proxy_hands_point,
            proxy_hands_rock,
            proxy_hands_thumbs_up;

        public static AvatarMask hands_only, hand_left, hand_right;
        static VRCAssets()
        {
            proxy_stand_still = GetProxyAnim("proxy_stand_still");
            proxy_afk = GetProxyAnim("proxy_afk");

            proxy_hands_idle = GetProxyAnim("proxy_hands_idle");
            proxy_hands_fist = GetProxyAnim("proxy_hands_fist");
            proxy_hands_gun = GetProxyAnim("proxy_hands_gun");
            proxy_hands_open = GetProxyAnim("proxy_hands_open");
            proxy_hands_peace = GetProxyAnim("proxy_hands_peace");
            proxy_hands_point = GetProxyAnim("proxy_hands_point");
            proxy_hands_rock = GetProxyAnim("proxy_hands_rock");
            proxy_hands_thumbs_up = GetProxyAnim("proxy_hands_thumbs_up");

            hands_only = GetAvatarMask("vrc_HandsOnly");
            hand_right = GetAvatarMask("vrc_Hand Right");
            hand_left = GetAvatarMask("vrc_Hand Left");
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
    }
}


