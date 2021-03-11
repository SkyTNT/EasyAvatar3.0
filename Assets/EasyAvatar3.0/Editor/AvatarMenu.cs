using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyAvatar
{
    public class AvatarMenu
    {
        public string name;
        private ArrayList items;

        public AvatarMenu(string name)
        {
            this.name = name;
            items = new ArrayList();
        }

        /// <summary>
        /// add a item to the menu
        /// </summary>
        /// <param name="item">the item</param>
        public void Add(AvatarMenuItem item)
        {
            if(items.Count>=8)
            {
                Debug.LogError("[EasyAvatar]菜单内容不能超过8");
                return;
            }
            items.Add(item);
        }
        
        /// <summary>
        /// remove a item from the menu
        /// </summary>
        /// <param name="index">index of item</param>
        public void Remove(int index)
        {
            items.RemoveAt(index);
        }

        /// <summary>
        /// remove a item from the menu
        /// </summary>
        /// <param name="item">the item</param>
        public void Remove(AvatarMenuItem item)
        {
            items.Remove(item);
        }

    }

    public class AvatarMenuItem
    {

        public string name;
        public AvatarMenuItem(string name)
        {
            this.name = name;
        }

    }
}

