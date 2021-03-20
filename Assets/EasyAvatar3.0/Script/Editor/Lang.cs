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
            AvataNoDescriptor,
            Name,
            Add,
            Clear,
            Delete,
            MainMenu,
            SubMenu,
            Control,
            Icon,
            Target,
            Missing,
            MissingTarget,
            Property,
            SetTo,
            Preview,
            Copy,
            Paste,
            BehaviorOff,
            BehaviorOn,
            UseAnimClip,
            AnimClip,
            AnimClipOff,
            AnimClipOn,
            AnimClipOffNote,
            ErrAvatarMenuLen1,
            ErrAvatarMenuLen0,
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
            AvatarApply = "生成菜单并应用到模型";
            AvataNoDescriptor = "模型没有检测到VRCAvatarDescriptor，点击添加";
            Name = "名称";
            Add = "添加";
            Clear = "清空";
            Delete = "删除";
            MainMenu = "主菜单";
            SubMenu = "子菜单";
            Control = "控件";
            Icon = "图标";
            Target = "目标";
            Missing = "缺失";
            MissingTarget = "缺失目标物体";
            Property = "属性";
            SetTo = "设置为";
            Preview = "预览";
            Copy = "复制";
            Paste = "粘贴";
            BehaviorOff = "控件关闭时的行为";
            BehaviorOn = "控件打开时的行为";
            UseAnimClip = "使用动画文件";
            AnimClip = "动画文件";
            AnimClipOff = "控件关闭时播放的动画";
            AnimClipOn = "控件打开时播放的动画";
            AnimClipOffNote = "控件关闭时的动画只有非人体动画部分会播放";
            ErrAvatarMenuLen1 = "一个模型中只能有一个主菜单，请考虑创建子菜单";
            ErrAvatarMenuLen0 = "没有主菜单，请先创建";
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
            AvatarApply = "Build expression menu and apply to avatar";
            AvataNoDescriptor = "VRCAvatarDescriptor is not detected in the avatar. Click to add";
            Add = "Add";
            Name = "Name";
            Clear = "Clear";
            Delete = "Delete";
            MainMenu = "Expression Menu";
            SubMenu = "Sub Menu";
            Control = "Control";
            Icon = "Icon";
            Target = "Target";
            Missing = "Missing";
            MissingTarget = "Missing Target";
            Property = "Property";
            SetTo = "Set Value";
            Preview = "Preview";
            Copy = "Copy";
            Paste = "Paste";
            BehaviorOff = "On Control Switch Off";
            BehaviorOn = "On Control Switch On";
            UseAnimClip = "Use Animation Clip";
            AnimClip = "Animation Clip";
            AnimClipOff = "On Control Switch Off";
            AnimClipOn = "On Control Switch On";
            AnimClipOffNote = "When the control is switched off, only the non human animation is played";
            ErrAvatarMenuLen1 = "There can only be one main menu in a avatar. Consider creating a submenu.";
            ErrAvatarMenuLen0 = "There is no main menu. Please create it first.";
            ErrMenuItemLen8 = "The number of items in the menu can not exceed 8. Consider creating a submenu.";
            ErrMenuInControl = "Cannot add menu to control";
            ErrControlInControl = "Cannot add control to control";
            ErrNotInAvatarHelper = "Make sure that the control or menu is in a Avatar Helper";
            ErrAvatarNotSet = "Make sure your avatar is setted in the Avatar Helper";
        }

    }
}