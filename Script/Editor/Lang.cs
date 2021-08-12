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
            AvatarApplyHelpBox,
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
            Replacement,
            MissingTarget,
            NoMissingTarget,
            Property,
            SetTo,
            Preview,
            Copy,
            Paste,
            Fold,
            OnOpen,
            OnClose,
            OnSwitchOff,
            OnSwitchOn,
            OnPress,
            OnRelease,
            OnRadialPuppet,
            OnRadialPuppetX,
            OnRadialPuppetOff,
            OnTwoAxisPuppetPosition,
            OnTwoAxisPuppetOff,
            OnTwoAxisPuppetH,
            OnTwoAxisPuppetV,
            Behavior,
            BehaviorType,
            BehaviorTypeProperty,
            BehaviorTypeAnim,
            AnimClip,
            Motion,
            Speed,
            Mirror,
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
            TrackingControl,
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
            ToggleMusic,
            MusicVolume,
            Music,
            Volume,
            ToggleObject,
            GestureManager,
            GestureManagerHelpBox,
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
            Default,
            AllDefault,
            BuildSucceed,
            AutoRestore,
            AutoTrackingControl,
            LocomotionManager,
            LocomotionGroup,
            LocomotionStand,
            LocomotionProne,
            LocomotionCrouch,
            LocomotionJump,
            LocomotionOther,
            LocomotionTrackingNote,
            LocomotionVersionNote,
            LocomotionControllerNote,
            UseController,
            UseBlendTree,
            AnimatorController,
            BlendTree,
            StandStill,
            WalkForward,
            WalkBackward,
            WalkLeft,
            WalkRight,
            WalkForwardLeft,
            WalkForwardRight,
            WalkBackwardLeft,
            WalkBackwardRight,
            RunForward,
            RunBackward,
            RunLeft,
            RunRight,
            RunForwardLeft,
            RunForwardRight,
            RunBackwardLeft,
            RunBackwardRight,
            SprintForward,
            CrouchStill,
            CrouchForward,
            CrouchBackward,
            CrouchLeft,
            CrouchRight,
            CrouchForwardLeft,
            CrouchForwardRight,
            CrouchBackwardLeft,
            CrouchBackwardRight,
            ProneStill,
            ProneForward,
            ProneBackward,
            ProneLeft,
            ProneRight,
            ShortFall,
            LongFall,
            QuickLand,
            Land,
            AFK,
            OneClickReplaceMissingTarget,
            OneClickReplaceMissingTargetNote,
            About,
            ErrAvatarMenuLen1,
            ErrAvatarGestureManagerLen1,
            ErrAvatarLocomotionManagerLen1,
            ErrMenuItemLen8,
            ErrMenuPath,
            ErrControlPath,
            ErrGestureManagerPath,
            ErrLocomotionManagerPath,
            ErrLocomotionGroupPath,
            ErrGesturePath,
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
            AvatarApply = "生成并应用到模型";
            AvatarApplyHelpBox = "每次完成修改后，你都必须点击此按钮，修改的内容才会生效";
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
            NoMissingTarget = "没有缺失目标物体";
            Replacement = "替换";
            Property = "属性";
            SetTo = "设置为";
            Preview = "预览";
            Copy = "复制";
            Paste = "粘贴";
            Fold = "折叠";
            OnOpen = "打开时";
            OnClose = "关闭时";
            OnSwitchOff = "开关关闭时";
            OnSwitchOn = "开关打开时";
            OnPress = "按钮按下时";
            OnRelease = "按钮松开时";
            OnRadialPuppet = "旋钮值为{0}时";
            OnRadialPuppetX = "值";
            OnRadialPuppetOff = "旋钮关闭时";
            OnTwoAxisPuppetPosition = "操纵杆在({0},{1})处时";
            OnTwoAxisPuppetOff = "操纵杆关闭时";
            OnTwoAxisPuppetH = "左/右";
            OnTwoAxisPuppetV = "下/上";
            Behavior = "行为";
            BehaviorType = "行为类型";
            BehaviorTypeProperty = "修改物体属性";
            BehaviorTypeAnim = "播放动画";
            AnimClip = "动画文件";
            Motion = "动画";
            Speed = "速度";
            Mirror = "镜像";
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
            TrackingControl = "追踪设置";
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
            ToggleMusic = "播放音乐开关";
            MusicVolume = "音乐音量调整";
            Music = "音乐";
            Volume = "音量";
            ToggleObject = "物体开关";
            GestureManager = "手势管理";
            GestureManagerHelpBox = "仅支持基本的手部动画，不支持其他部位动画以及表情。如果需要，请在手势管理下添加手势。";
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
            Default = "默认值";
            AllDefault = "全部默认";
            BuildSucceed = "成功";
            AutoRestore = "关闭时自动恢复修改的内容到默认状态";
            AutoTrackingControl = "自动设置追踪状态";
            LocomotionManager = "姿态管理";
            LocomotionGroup = "姿态组";
            LocomotionStand = "站立姿势";
            LocomotionCrouch = "蹲下姿势";
            LocomotionProne = "趴下姿势";
            LocomotionJump = "跳姿势";
            LocomotionOther = "其他姿势";
            LocomotionTrackingNote = "如果出现模型陷入地下等高度位置不正常的情况，需要在对应‘追踪设置’中把头部和臀部追踪设置为‘动画中’";
            LocomotionVersionNote = "当前版本只支持一套姿势";
            LocomotionControllerNote = "如果使用动画状态机，姿态管理下的姿势都不会起作用";
            UseController = "是否使用动画控制器";
            UseBlendTree = "是否使用混合树";
            AnimatorController = "动画控制器";
            BlendTree = "混合树";
            StandStill = "站立静止";
            WalkForward = "走向前";
            WalkBackward = "走向后";
            WalkLeft = "走向左";
            WalkRight = "走向右";
            WalkForwardLeft = "走向前左";
            WalkForwardRight = "走向前右";
            WalkBackwardLeft = "走向后左";
            WalkBackwardRight = "走向后右";
            RunForward = "跑向前";
            RunBackward = "跑向后";
            RunLeft = "跑向左";
            RunRight = "跑向右";
            RunForwardLeft = "跑向前左";
            RunForwardRight = "跑向前右";
            RunBackwardLeft = "跑向后左";
            RunBackwardRight = "跑向后右";
            SprintForward = "疾跑向前";
            CrouchStill = "蹲下静止";
            CrouchForward = "蹲下前进";
            CrouchBackward = "蹲下后退";
            CrouchLeft = "蹲下向左";
            CrouchRight = "蹲下向右";
            CrouchForwardLeft = "蹲下向前左";
            CrouchForwardRight = "蹲下向前右";
            CrouchBackwardLeft = "蹲下向后左";
            CrouchBackwardRight = "蹲下向后右";
            ProneStill = "趴下静止";
            ProneForward = "趴下向前";
            ProneBackward = "趴下向后";
            ProneLeft = "趴下向左";
            ProneRight = "趴下向右";
            ShortFall = "短时间下落";
            LongFall = "长时间下落";
            QuickLand = "快速着陆";
            Land = "着陆";
            AFK = "挂机(AFK)";
            OneClickReplaceMissingTarget = "一键替换缺失目标";
            OneClickReplaceMissingTargetNote = "当你替换模型后，可能会出现缺失目标的情况，用这个可以一键替换缺失的目标";
            About = "由SkyTNT制作\n项目地址：https://github.com/SkyTNT/EasyAvatar3.0/";
            ErrAvatarMenuLen1 = "一个模型中只能有一个主菜单，请考虑创建子菜单";
            ErrAvatarGestureManagerLen1 = "一个模型中只能有一个手势管理";
            ErrAvatarLocomotionManagerLen1 = "一个模型中只能有一个姿态管理";
            ErrMenuItemLen8 = "菜单中的项目数量不能超过8，请考虑创建子菜单";
            ErrMenuPath = "菜单只能放在模型助手或菜单中";
            ErrControlPath = "控件只能放在菜单中";
            ErrGestureManagerPath = "手势管理只能添加在模型助手下";
            ErrGesturePath = "手势只能添加在手势管理中";
            ErrLocomotionManagerPath = "姿态管理只能添加在模型助手下";
            ErrLocomotionGroupPath = "姿态组只能放在姿态管理中";
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
            AvatarApply = "Build and apply to avatar";
            AvatarApplyHelpBox = "After finished modifications, you must click this button for the modified contents to take effect";
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
            NoMissingTarget = "No missing target objects";
            Replacement = "Replacement";
            Property = "Property";
            SetTo = "Set Value";
            Preview = "Preview";
            Copy = "Copy";
            Paste = "Paste";
            Fold = "Fold";
            OnOpen = "On Open";
            OnClose = "On Close";
            OnSwitchOff = "On Switch Off";
            OnSwitchOn = "On Switch On";
            OnPress = "On Press Button";
            OnRelease = "On Release Button";
            OnRadialPuppet = "On Value Is {0}";
            OnRadialPuppetX = "Value";
            OnRadialPuppetOff = "On Close";
            OnTwoAxisPuppetPosition = "On Position ({0},{1})";
            OnTwoAxisPuppetOff = "On Close";
            OnTwoAxisPuppetH = "X Axis";
            OnTwoAxisPuppetV = "Y Axis";
            Behavior = "Behaviors";
            BehaviorType = "Behavior Type";
            BehaviorTypeProperty = "Change Property";
            BehaviorTypeAnim = "Play Animation";
            AnimClip = "Animation Clip";
            Motion = "Motion";
            Speed = "Speed";
            Mirror = "Mirror";
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
            TrackingControl = "Tracking Control";
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
            ToggleMusic = "Toggle Music";
            MusicVolume = "Set Music Volume";
            Music = "Music";
            Volume = "Volume";
            ToggleObject = "Toggle Object";
            GestureManager = "Gesture Manager";
            GestureManagerHelpBox = "Only basic hand animations are supported. Animations of other body parts and facial expressions are not supported. If necessary, please add gestures under the GestureManager.";
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
            Default = "Default";
            AllDefault = "All Default";
            BuildSucceed = "Succeed";
            AutoRestore = "Auto restore to default state when off";
            AutoTrackingControl = "Auto set tracking state";
            LocomotionManager = "Locomotion Manager";
            LocomotionGroup = "Locomotion Group";
            LocomotionStand = "Stand";
            LocomotionCrouch = "Crouch";
            LocomotionProne = "Crawl";
            LocomotionJump = "Jump";
            LocomotionOther = "Other";
            LocomotionTrackingNote = "If the model falls into the ground , it is necessary to set the hip tracking and head tracking to \"animation\" in the corresponding \"tracking control\"";
            LocomotionVersionNote = "The current version only supports one set of locomotions.";
            LocomotionControllerNote = "If you use the animatior controller, the locomotions under Locomotion Manager will not work.";
            UseController = "Use AnimatorController";
            UseBlendTree = "Use BlendTree";
            AnimatorController = "AnimatorController";
            BlendTree = "BlendTree";
            StandStill = "Stand Still";
            WalkForward = "Walk Forward";
            WalkBackward = "Walk Backward";
            WalkLeft = "Walk Left";
            WalkRight = "Walk Right";
            WalkForwardLeft = "Walk Forward Left";
            WalkForwardRight = "Walk Forward Right";
            WalkBackwardLeft = "Walk Backward Left";
            WalkBackwardRight = "Walk Backward Right";
            RunForward = "Run Forward";
            RunBackward = "Run Backward";
            RunLeft = "Run Left";
            RunRight = "Run Right";
            RunForwardLeft = "Run Forward Left";
            RunForwardRight = "Run Forward Right";
            RunBackwardLeft = "Run Backward Left";
            RunBackwardRight = "Run Backward Right";
            SprintForward = "Sprint Forward";
            CrouchStill = "Crouch Still";
            CrouchForward = "Crouch Forward";
            CrouchBackward = "Crouch Backward";
            CrouchLeft = "Crouch Left";
            CrouchRight = "Crouch Right";
            CrouchForwardLeft = "Crouch Forward Left";
            CrouchForwardRight = "Crouch Forward Right";
            CrouchBackwardLeft = "Crouch Backward Left";
            CrouchBackwardRight = "Crouch Backward Right";
            ProneStill = "Crawl Still";
            ProneForward = "Crawl Forward";
            ProneBackward = "Crawl Backward";
            ProneLeft = "Crawl Left";
            ProneRight = "Crawl Right";
            ShortFall = "Short Fall";
            LongFall = "Long Fall";
            QuickLand = "Quick Land";
            Land = "Land";
            AFK = "AFK";
            OneClickReplaceMissingTarget = "One click replacement of missing targets";
            OneClickReplaceMissingTargetNote = "After you replace the avatar, there may be some missing targets. You can use this button to replace the missing targets";
            About = " Made by SkyTNT\nProject:https://github.com/SkyTNT/EasyAvatar3.0/";
            ErrAvatarMenuLen1 = "There should only be one main menu in a avatar. Consider creating a submenu.";
            ErrAvatarGestureManagerLen1 = "There should only be one gesture manager in a avatar.";
            ErrAvatarLocomotionManagerLen1 = "There should only be one locomotion manager in a avatar.";
            ErrMenuItemLen8 = "The number of items in the menu can not exceed 8. Consider creating a submenu.";
            ErrMenuPath = "Menu can only be added to the Avatar Helper or a parent menu";
            ErrControlPath = "Control can only be added to a menu";
            ErrGestureManagerPath = "Gesture Manager can only be added to the Avatar Helper";
            ErrGesturePath = "Gesture can only be added to the Gesture Manager";
            ErrLocomotionManagerPath = "Locomotion Manager can only be added to the Avatar Helper";
            ErrLocomotionGroupPath = "Locomotion Group can only be added to the Locomotion Manager";
            ErrAvatarNotSet = "Avatar is not set in the Avatar Helper";
            ErrAvatarHelperInAvatar = "Avatar Helper cannot be included in a avatar, Please put it outside the avatar.";
        }

    }
}