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
            ControlType,
            Toggle,
            RadialPuppet,
            Icon,
            Target,
            Missing,
            MissingTarget,
            Property,
            SetTo,
            Preview,
            Copy,
            Paste,
            OnSwitchOff,
            OnSwitchOn,
            OnRadialPuppet0,
            OnRadialPuppet1,
            BehaviorListLabel,
            AnimClipListLabel,
            UseAnimClip,
            AnimClip,
            AnimClipOffNote,
            GestureManager,
            Gesture,
            GestureType,
            HandType,
            OnGesture,
            GestureNeutral,
            GestureFist,
            GestureHandOpen,
            GestureFingerPoint,
            GestureVictory,
            GestureRockNRoll,
            GestureHandGun,
            GestureThumbsUp,
            LeftHand,
            RightHand,
            AnyHand,
            BuildSucceed,
            AutoRestore,
            About,
            ErrAvatarMenuLen1,
            ErrAvatarGestureMenuLen1,
            ErrMenuItemLen8,
            ErrMenuInControl,
            ErrControlInControl,
            ErrGestureMenuNotInHelper,
            ErrGestureNotInGestureManager,
            ErrAvatarNotSet,
            ErrAvatarHelperInAvatar;



        static Lang()
        {
            if (IsChinese())
                UseChinese();
            else
                UseEnglish();
        }

        /// <summary>
        /// 判断是否为中文环境
        /// </summary>
        /// <returns></returns>
        private static bool IsChinese()
        {
            string lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            return  lang== "zh-CN"||lang== "zh-TW";
        }

        /// <summary>
        /// 切换中文
        /// </summary>
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
            ControlType = "控件类型";
            Toggle = "开关";
            RadialPuppet = "旋钮";
            Icon = "图标";
            Target = "目标";
            Missing = "缺失";
            MissingTarget = "缺失目标物体";
            Property = "属性";
            SetTo = "设置为";
            Preview = "预览";
            Copy = "复制";
            Paste = "粘贴";
            OnSwitchOff = "开关关闭时";
            OnSwitchOn = "开关打开时";
            OnRadialPuppet0 = "旋钮值为零时";
            OnRadialPuppet1 = "旋钮旋满时";
            BehaviorListLabel = "进行的修改";
            AnimClipListLabel = "播放的动画";
            UseAnimClip = "使用动画文件";
            AnimClip = "动画文件";
            AnimClipOffNote = "控件关闭时的动画只有非人体动画部分会播放";
            GestureManager = "手势管理";
            Gesture = "手势";
            GestureType = "手势类型";
            HandType = "手类型";
            OnGesture = "当做手势时";
            GestureNeutral = "普通";
            GestureFist = "握拳";
            GestureHandOpen = "张手";
            GestureFingerPoint = "指人";
            GestureVictory = "剪刀手";
            GestureRockNRoll = "摇滚🤟";
            GestureHandGun = "手枪";
            GestureThumbsUp = "大拇指";
            LeftHand = "左手";
            RightHand = "右手";
            AnyHand = "任何手";
            BuildSucceed = "成功";
            AutoRestore = "关闭时自动恢复修改的内容到默认状态";
            About = "由SkyTNT制作\n项目地址：https://github.com/SkyTNT/EasyAvatar3.0/";
            ErrAvatarMenuLen1 = "一个模型中只能有一个主菜单，请考虑创建子菜单";
            ErrAvatarGestureMenuLen1 = "一个模型中只能有一个手势菜单";
            ErrMenuItemLen8 = "菜单中的项目数量不能超过8，请考虑创建子菜单";
            ErrMenuInControl = "控件中不能加菜单";
            ErrControlInControl = "控件中不能加控件";
            ErrGestureMenuNotInHelper = "手势管理只能添加在模型助手下";
            ErrGestureNotInGestureManager = "手势只能添加在手势管理中";
            ErrAvatarNotSet = "请确保模型助手绑定了模型";
            ErrAvatarHelperInAvatar = "模型助手不能包含在模型里，请放在模型的外边";
        }

        /// <summary>
        /// 切换英文
        /// </summary>
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
            ControlType = "Control Type";
            Toggle = "Toggle";
            RadialPuppet = "Radial Puppet";
            Icon = "Icon";
            Target = "Target";
            Missing = "Missing";
            MissingTarget = "Missing Target";
            Property = "Property";
            SetTo = "Set Value";
            Preview = "Preview";
            Copy = "Copy";
            Paste = "Paste";
            OnSwitchOff = "On Control Switch Off";
            OnSwitchOn = "On Control Switch On";
            OnRadialPuppet0 = "On Value Is 0%";
            OnRadialPuppet1 = "On Value Is 100%";
            BehaviorListLabel = "Behaviors";
            AnimClipListLabel = "Animation Clips";
            UseAnimClip = "Use Animation Clip";
            AnimClip = "Animation Clip";
            AnimClipOffNote = "When the control is switched off, only the non human animation is played";
            GestureManager = "Gesture Manager";
            Gesture = "Gesture";
            GestureType = "Gesture Type";
            HandType = "Hand Type";
            OnGesture = "On Gesture";
            GestureNeutral = "Neutral";
            GestureFist = "Fist";
            GestureHandOpen = "HandOpen";
            GestureFingerPoint = "FingerPoint";
            GestureVictory = "Victory";
            GestureRockNRoll = "RockNRoll";
            GestureHandGun = "HandGun";
            GestureThumbsUp = "ThumbsUp";
            LeftHand = "LeftHand";
            RightHand = "RightHand";
            AnyHand = "AnyHand";
            BuildSucceed = "Succeed";
            AutoRestore = "Auto restore to default state when off";
            About = " Made by SkyTNT\nProject:https://github.com/SkyTNT/EasyAvatar3.0/";
            ErrAvatarMenuLen1 = "There should only be one main menu in a avatar. Consider creating a submenu.";
            ErrAvatarGestureMenuLen1 = "There should only be one gesture menu in a avatar.";
            ErrMenuItemLen8 = "The number of items in the menu can not exceed 8. Consider creating a submenu.";
            ErrMenuInControl = "Cannot add menu to control";
            ErrControlInControl = "Cannot add control to control";
            ErrGestureMenuNotInHelper = "Gesture Manager can only be added to the Avatar Helper";
            ErrGestureNotInGestureManager = "Gesture can only be added to the Gesture Manager";
            ErrAvatarNotSet = "Make sure your avatar is setted in the Avatar Helper";
            ErrAvatarHelperInAvatar = "Avatar Helper cannot be included in a avatar, Please put it outside the avatar.";
        }

    }
}