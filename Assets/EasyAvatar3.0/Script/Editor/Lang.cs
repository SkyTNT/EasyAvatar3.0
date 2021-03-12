using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace EasyAvatar
{
    
    public class Lang
    {
        public static string
            Avatar,
            AvatarHelper,
            AvatarApply,
            MainMenu,
            SubMenu,
            Control,
            Behavior,
            ErrAvatarMenuLen1,
            ErrMenuItemLen8,
            ErrMenuInControl,
            ErrControlInControl;



        static Lang()
        {
            UseChinese();
        }

        [MenuItem("EasyAvatar3.0/Language/Chinese", priority = 0)]
        public static void UseChinese()
        {
            Avatar = "模型";
            AvatarHelper = "[模型]";
            AvatarApply = "应用到模型";
            MainMenu = "[菜单]";
            SubMenu = "子菜单";
            Control = "控件";
            Behavior = "行为";
            ErrAvatarMenuLen1 = "一个模型中只能有一个主菜单，请考虑创建子菜单";
            ErrMenuItemLen8 = "菜单中的项目数量不能超过8，请考虑创建子菜单";
            ErrMenuInControl = "控件中不能加菜单";
            ErrControlInControl = "控件中不能加控件";
        }

        [MenuItem("EasyAvatar3.0/Language/English", priority = 0)]
        public static void UseEnglish()
        {
            Avatar = "Avatar";
            AvatarHelper = "[Easy Avatar]";
            AvatarApply = "Apply to avatar";
            MainMenu = "[Expression Menu]";
            SubMenu = "Sub Menu";
            Control = "Control";
            Behavior = "Behavior";
            ErrAvatarMenuLen1 = "There can only be one main menu in a avatar. Consider creating a submenu.";
            ErrMenuItemLen8 = "The number of items in the menu can not exceed 8. Consider creating a submenu.";
            ErrMenuInControl = "Cannot add menu to control";
            ErrControlInControl = "Cannot add control to control";
        }

    }
}