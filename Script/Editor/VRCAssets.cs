using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    public class VRCAssets
    {
        public static string animPath = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";
        public static AnimationClip proxy_stand_still;
        public static AnimationClip proxy_afk;
        static VRCAssets()
        {
            proxy_stand_still = GetProxyAnim("proxy_stand_still");
            proxy_afk = GetProxyAnim("proxy_afk");
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
    }
}


