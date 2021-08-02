using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyAvatar
{
    public class EasyAvatarAsset
    {
        public static string AssetRootName = "EasyAvatarAsset";
        private static List<Object> assets = new List<Object>();
        public static AddAssetToAvatarDelegate AddAssetToAvatar;

        public static void Init()
        {
            assets.Clear();
            
        }

        public static int GetId(Object asset)
        {
            int id = 0;
            foreach(var item in assets)
            {
                if (item == asset)
                    return id;
                id++;
            }
            assets.Add(asset);
            AddAssetToAvatar(asset);
            return id;
        }

        public static string GetPathRelateToAvatar(Object asset)
        {

            return AssetRootName + "/asset" + GetId(asset);
        }

        public static string GetNameInAvatar(Object asset)
        {

            return "asset" + GetId(asset);
        }

        public static List<Object> GetAssets()
        {
            return assets;
        }

        public delegate void AddAssetToAvatarDelegate(Object asset);
    }
}

