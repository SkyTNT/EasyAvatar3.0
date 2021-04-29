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
            Up,
            Down,
            MainMenu,
            SubMenu,
            Control,
            ControlType,
            Toggle,
            Button,
            RadialPuppet,
            TwoAxisPuppet,
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
            OnPress,
            OnRelease,
            OnRadialPuppet0,
            OnRadialPuppet1,
            OnRadialPuppetOff,
            OnTwoAxisPuppetPosition,
            OnTwoAxisPuppetOff,
            Behavior,
            BehaviorType,
            BehaviorTypeProperty,
            BehaviorTypeAnim,
            AnimClip,
            AnimMask,
            AnimMaskHead,
            AnimMaskBody,
            AnimMaskRightArm,
            AnimMaskLeftArm,
            AnimMaskRightFingers,
            AnimMaskLeftFingers,
            AnimMaskRightLeg,
            AnimMaskLeftLeg,
            AnimMaskFx,
            TrackingAll,
            TrackingHead,
            TrackingMouth,
            TrackingEyes,
            TrackingHip,
            TrackingRightHand,
            TrackingLeftHand,
            TrackingRightFingers,
            TrackingLeftFingers,
            TrackingRightFoot,
            TrackingLeftFoot,
            TrackingTypeNoChange,
            TrackingTypeTracking,
            TrackingTypeAnimation,
            GestureManager,
            GestureBaseAnimation,
            Gesture,
            GestureType,
            HandType,
            OnGesture,
            OnGestureOut,
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
            DefaultValue,
            BuildSucceed,
            AutoRestore,
            autoTrackingControl,
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
            Up = "上移";
            Down = "下移";
            MainMenu = "主菜单";
            SubMenu = "子菜单";
            Control = "控件";
            ControlType = "控件类型";
            Toggle = "开关";
            Button = "按钮";
            RadialPuppet = "旋钮";
            TwoAxisPuppet = "两轴操纵杆";
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
            OnPress = "按钮按下时";
            OnRelease = "按钮松开时";
            OnRadialPuppet0 = "旋钮值为零时";
            OnRadialPuppet1 = "旋钮旋满时";
            OnRadialPuppetOff = "旋钮关闭时";
            OnTwoAxisPuppetPosition = "操纵杆在{0},{1}处时";
            OnTwoAxisPuppetOff = "操纵杆关闭时";
            Behavior = "行为";
            BehaviorType = "行为类型";
            BehaviorTypeProperty = "修改物体属性";
            BehaviorTypeAnim = "播放动画";
            AnimClip = "动画文件";
            AnimMask = "动画有效部分";
            AnimMaskHead = "头";
            AnimMaskBody = "主体";
            AnimMaskRightArm  = "右手";
            AnimMaskLeftArm  = "左手";
            AnimMaskRightFingers   = "右手指";
            AnimMaskLeftFingers  = "左手指";
            AnimMaskRightLeg  = "右脚";
            AnimMaskLeftLeg  = "左脚";
            AnimMaskFx = "非人体";
            TrackingAll = "全部";
            TrackingHead = "头";
            TrackingMouth = "嘴";
            TrackingEyes = "眼睛";
            TrackingHip = "臀部";
            TrackingRightHand = "右手";
            TrackingLeftHand = "左手";
            TrackingRightFingers = "右手指";
            TrackingLeftFingers = "左手指";
            TrackingRightFoot = "右脚";
            TrackingLeftFoot = "左脚";
            TrackingTypeNoChange = "不变";
            TrackingTypeTracking = "追踪中";
            TrackingTypeAnimation = "动画中";
            GestureManager = "手势管理";
            GestureBaseAnimation = "基本手势动画";
            Gesture = "手势";
            GestureType = "手势类型";
            HandType = "手类型";
            OnGesture = "当做手势时";
            OnGestureOut = "当切换到其他手势时";
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
            DefaultValue = "默认值";
            BuildSucceed = "成功";
            AutoRestore = "关闭时自动恢复修改的内容到默认状态";
            autoTrackingControl = "自动设置追踪状态";
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
            Up = "Up";
            Down = "Down";
            MainMenu = "Expression Menu";
            SubMenu = "Sub Menu";
            Control = "Control";
            ControlType = "Control Type";
            Toggle = "Toggle";
            Button = "Button";
            RadialPuppet = "Radial Puppet";
            TwoAxisPuppet = "Two Axis Puppet";
            Icon = "Icon";
            Target = "Target";
            Missing = "Missing";
            MissingTarget = "Missing Target";
            Property = "Property";
            SetTo = "Set Value";
            Preview = "Preview";
            Copy = "Copy";
            Paste = "Paste";
            OnSwitchOff = "On Switch Off";
            OnSwitchOn = "On Switch On";
            OnPress = "On Press Button";
            OnRelease = "On Release Button";
            OnRadialPuppet0 = "On Value Is 0%";
            OnRadialPuppet1 = "On Value Is 100%";
            OnRadialPuppetOff = "On Close";
            OnTwoAxisPuppetPosition = "On Position {0},{1}";
            OnTwoAxisPuppetOff = "On Close";
            Behavior = "Behaviors";
            BehaviorType = "Behavior Type";
            BehaviorTypeProperty = "Change Property";
            BehaviorTypeAnim = "Play Animation";
            AnimClip = "Animation Clip";
            AnimMask = "Animation Mask";
            AnimMaskHead = "Head";
            AnimMaskBody = "Body";
            AnimMaskRightArm = "Right Arm";
            AnimMaskLeftArm = "Left Arm";
            AnimMaskRightFingers = "Right Fingers";
            AnimMaskLeftFingers = "Left Fingers";
            AnimMaskRightLeg = "Right Leg";
            AnimMaskLeftLeg = "Left Leg";
            AnimMaskFx = "FX";
            TrackingAll = "All";
            TrackingHead = "Head";
            TrackingMouth = "Mouth";
            TrackingEyes = "Eyes";
            TrackingHip = "Hip";
            TrackingRightHand = "Right Hand";
            TrackingLeftHand = "Left Hand";
            TrackingRightFingers = "Right Fingers";
            TrackingLeftFingers = "Left Fingers";
            TrackingRightFoot = "Right Foot";
            TrackingLeftFoot = "Left Foot";
            TrackingTypeNoChange = "No Change";
            TrackingTypeTracking = "Tracking";
            TrackingTypeAnimation = "Animation";
            GestureManager = "Gesture Manager";
            GestureBaseAnimation = "Basic Gesture Animations";
            Gesture = "Gesture";
            GestureType = "Gesture Type";
            HandType = "Hand Type";
            OnGesture = "On Gesture";
            OnGestureOut = "On Switch to Other Gesture";
            GestureNeutral = "Neutral";
            GestureFist = "Fist";
            GestureHandOpen = "Hand Open";
            GestureFingerPoint = "Finger Point";
            GestureVictory = "Victory";
            GestureRockNRoll = "RockNRoll";
            GestureHandGun = "Hand Gun";
            GestureThumbsUp = "Thumbs Up";
            LeftHand = "Left Hand";
            RightHand = "Right Hand";
            AnyHand = "Any Hand";
            DefaultValue = "Default Value";
            BuildSucceed = "Succeed";
            AutoRestore = "Auto restore to default state when off";
            autoTrackingControl = "Auto set tracking state";
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