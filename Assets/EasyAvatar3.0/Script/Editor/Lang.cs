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
            Name,
            Add,
            Clear,
            Delete,
            MainMenu,
            SubMenu,
            Control,
            Icon,
            Target,
            BehaviorOff,
            BehaviorOn,
            ErrAvatarMenuLen1,
            ErrMenuItemLen8,
            ErrMenuInControl,
            ErrControlInControl,
            ErrNotInAvatarHelper,
            ErrAvatarNotSet;



        static Lang()
        {
            if (IsChinese())
                UseChinese();
            else
                UseEnglish();
        }

        private static bool IsChinese()
        {
            string lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            return  lang== "zh-CN"||lang== "zh-TW";
        }

        [MenuItem("EasyAvatar3.0/Language/Chinese", priority = 0)]
        public static void UseChinese()
        {
            Avatar = "模型";
            AvatarHelper = "[模型]";
            AvatarApply = "应用到模型";
            Name = "名称";
            Add = "添加";
            Clear = "清空";
            Delete = "删除";
            MainMenu = "[菜单]";
            SubMenu = "子菜单";
            Control = "控件";
            Icon = "图标";
            Target = "目标";
            BehaviorOff = "控件关闭时的行为";
            BehaviorOn = "控件打开时的行为";

            ErrAvatarMenuLen1 = "一个模型中只能有一个主菜单，请考虑创建子菜单";
            ErrMenuItemLen8 = "菜单中的项目数量不能超过8，请考虑创建子菜单";
            ErrMenuInControl = "控件中不能加菜单";
            ErrControlInControl = "控件中不能加控件";
            ErrNotInAvatarHelper = "请确保控件或菜单在模型助手中";
            ErrAvatarNotSet = "请确保模型助手绑定了模型";
        }

        [MenuItem("EasyAvatar3.0/Language/English", priority = 0)]
        public static void UseEnglish()
        {
            Avatar = "Avatar";
            AvatarHelper = "[Easy Avatar]";
            AvatarApply = "Apply to avatar";
            Add = "Add";
            Name = "Name";
            Clear = "Clear";
            Delete = "Delete";
            MainMenu = "[Expression Menu]";
            SubMenu = "Sub Menu";
            Control = "Control";
            Icon = "Icon";
            Target = "Target";
            BehaviorOff = "On Control Switch Off";
            BehaviorOn = "On Control Switch On";
            ErrAvatarMenuLen1 = "There can only be one main menu in a avatar. Consider creating a submenu.";
            ErrMenuItemLen8 = "The number of items in the menu can not exceed 8. Consider creating a submenu.";
            ErrMenuInControl = "Cannot add menu to control";
            ErrControlInControl = "Cannot add control to control";
            ErrNotInAvatarHelper = "Make sure that the control or menu is in a Avatar Helper";
            ErrAvatarNotSet = "Make sure your avatar is setted in the Avatar Helper";
        }

    }
}