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
        public static AnimationClip proxy_stand_still;
        public static AnimationClip proxy_afk;
        public static AvatarMask hands_only;
        static VRCAssets()
        {
            proxy_stand_still = GetProxyAnim("proxy_stand_still");
            proxy_afk = GetProxyAnim("proxy_afk");
            hands_only = GetAvatarMask("vrc_HandsOnly");
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


