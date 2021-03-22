# EasyAvatar3.0 [[中文]](#easyavatar30-english "[中文]")

Making vrchat avatar3.0 without animatior controllers and parameters.

##  Background
Generally, the production process of avatar 3.0 includes menu creation, parameter table setting and animation state machine setting, so the production threshold of avatar 3.0 is too high. Easyavatar3.0 omits the parameter table setting and animation state machine setting to let the menu control the animation directly. So as to greatly reduce the production threshold. It's very friendly to new hand.

## Downloads

1. [VRChatAvatar3.0SDK](https://vrchat.com/download/sdk3-avatars/ "下载")

2. [EasyAvatar3.0(this project)](https://github.com/SkyTNT/EasyAvatar3.0/releases/latest "下载")

## Installation

1. Import VRCAvatar3.0SDK to unity

2. Import EasyAvatar3.0 to unity

## Usage

1. In the top toolbar, you can click `EasyAvatar3.0/Language` to change the language.
[![Language](https://fs.fab.moe/?explorer/share/file&hash=132bs__W5vh5OaE3str_2JdQZuJa6A6azsrKw-G8UTmakOPka-upuRAF&name=1e.png "Language")](https://fs.fab.moe/?explorer/share/file&hash=132bs__W5vh5OaE3str_2JdQZuJa6A6azsrKw-G8UTmakOPka-upuRAF&name=1e.png "Language")

2. Right click in the hierarchy view and click `EasyAvatar3.0/Expression Menu Control` to add a control. The new control will be automatically included in a menu in a Avatar Helper.
[![Create Control](https://fs.fab.moe/?explorer/share/file&hash=e456BQDRcDAGz-u_rdxKvUz1nWZ8ce1aMFvWRjnprNALen3PezPkADvB&name=2e.png "Create Control")](https://fs.fab.moe/?explorer/share/file&hash=e456BQDRcDAGz-u_rdxKvUz1nWZ8ce1aMFvWRjnprNALen3PezPkADvB&name=2e.png "Create Control")

3. Drag the avatar from Hierarchy to the `Avatar` field in Avatar Helper ( the object whose name starts with `[Easy Avatar]`).
[![Bind Avatar](https://fs.fab.moe/?explorer/share/file&hash=74625LO_bMcWY28I7upPuWj03BKx23FzUdrQEC-BX6f9p_fV9wfh7PSJ&name=4e.png "Bind Avatar")](https://fs.fab.moe/?explorer/share/file&hash=74625LO_bMcWY28I7upPuWj03BKx23FzUdrQEC-BX6f9p_fV9wfh7PSJ&name=4e.png "Bind Avatar")

4. Select the control you just added.

5. In the Inspector, you can set the name of the control in the `Name` field, of course, can also directly set the name of the object. You can set the icon of the control in the the `Icon` field. Under `On Control Switch Off`, there is a list of behaviors when a control is closed. Click `+` to add a behavior.
[![Add Behavior](https://fs.fab.moe/?explorer/share/file&hash=19a6hEwE7K6YGKnmh7QuT0IrHIYdouejpgj7H4u1ZDk4xpZJI5Dl0Pd9&name=5e.png "Add Behavior")](https://fs.fab.moe/?explorer/share/file&hash=19a6hEwE7K6YGKnmh7QuT0IrHIYdouejpgj7H4u1ZDk4xpZJI5Dl0Pd9&name=5e.png "Add Behavior")

6. Drag the object you want to modify to the `Target` field.
[![Set Target](https://fs.fab.moe/?explorer/share/file&hash=64977oXpfztTn7rMox0SenSk2Ojw-yZ1G10LukFkXwSbdG51LR0x7g-8&name=5e.png "Set Target")](https://fs.fab.moe/?explorer/share/file&hash=64977oXpfztTn7rMox0SenSk2Ojw-yZ1G10LukFkXwSbdG51LR0x7g-8&name=5e.png "Set Target")

7. Click the `Property` field to select the target's property you want to modify, such as `Is Active`.
[![Set Property](https://fs.fab.moe/?explorer/share/file&hash=23ce5wjPWTb3wKDm_oo76uJ6PSfM9SnIQJTkEx_AVJ9S0J47l_J0LBI1&name=7e.png "Set Property")](https://fs.fab.moe/?explorer/share/file&hash=23ce5wjPWTb3wKDm_oo76uJ6PSfM9SnIQJTkEx_AVJ9S0J47l_J0LBI1&name=7e.png "Set Property")

8. `Set Valued` field will display as different type according to the selected property. Set the value you want here. For example, it will be displayed as a check box here. If you uncheck it, the object will be hidden when the control is closed, otherwise, the object will show.
[![Set Value](https://fs.fab.moe/?explorer/share/file&hash=a614VZhKqJHAxq8BVRH4Xm0vnLy5k2om3oWq_1IytVyin3eab-G4yo-t&name=8e.png "Set Value")](https://fs.fab.moe/?explorer/share/file&hash=a614VZhKqJHAxq8BVRH4Xm0vnLy5k2om3oWq_1IytVyin3eab-G4yo-t&name=8e.png "Set Value")

9. Similarly, set the behavior list under `On Control Switch On`
[![On behaviors](https://fs.fab.moe/?explorer/share/file&hash=1f6fBbnL-0yERvtIpGPocQcpBsZOEy1aYgl0X71XJOI4F1tJoBzvRYHX&name=10e.png "On behaviors")](https://fs.fab.moe/?explorer/share/file&hash=1f6fBbnL-0yERvtIpGPocQcpBsZOEy1aYgl0X71XJOI4F1tJoBzvRYHX&name=10e.png "On behaviors")
This does not mean that the targets and properties of the two behavior lists should be consistent. They can be completely different. Properties that are not in the behavior list will remain the default. If a property is set by a control in closing behavior, the property will keep setting when the control is closed. In this case, you can not change the value of the property in another control. So if it's not what you want, it's best not to set the closing behaviors.

10. At the same time, you can selectively check `Use Animation Clip`. You can attach multiple animation clips. When the control is opened or closed, the corresponding behaviors and animations can be played at the same time.
[![Add Animation Clips](https://fs.fab.moe/?explorer/share/file&hash=6fb8Gj8ZuLs9zZSCL27huseD7bGl0WNHfXoJd04W-ER3UJ3CFZF0J6d7&name=11e.png "Add Animation Clips")](https://fs.fab.moe/?explorer/share/file&hash=6fb8Gj8ZuLs9zZSCL27huseD7bGl0WNHfXoJd04W-ER3UJ3CFZF0J6d7&name=11e.png "Add Animation Clips")

11. When one control is set up, you can add and set up another control. But the total number of items in a menu cannot exceed 8. So you can add submenu. Right click a menu and select `EasyAvatar3.0/Expression Menu` to add one.
[![Add Submenu](https://fs.fab.moe/?explorer/share/file&hash=0e9bisEp6JWGvdtFBiZiHHvNEeyqo-Wv0l9NaM0WnmZr6rmD5B4S6ZBc&name=12e.png "Add Submenu")](https://fs.fab.moe/?explorer/share/file&hash=0e9bisEp6JWGvdtFBiZiHHvNEeyqo-Wv0l9NaM0WnmZr6rmD5B4S6ZBc&name=12e.png "Add Submenu")

12. When the menu is ready, select Avatar Helper, and in the Inspector, click `Build expression menu and apply to avatar`. You have to click this button every time after modifying the menu and before uploading the avatar.
[![Build](https://fs.fab.moe/?explorer/share/file&hash=dd92nmz87BJYtGYmvOisUb0Olu6dA7gn83xR0Toa85J-prW_e4xxN-lu&name=13e.png "Build")](https://fs.fab.moe/?explorer/share/file&hash=dd92nmz87BJYtGYmvOisUb0Olu6dA7gn83xR0Toa85J-prW_e4xxN-lu&name=13e.png "Build")
It will automatically generate menus and animations and set them in VRC Avatar Descriptor. The generated files are stored in `Assets\EasyAvatar3.0\Build`. You don't need to care about them. Just go ahead.
[![Builded](https://fs.fab.moe/?explorer/share/file&hash=fc2dZ26mdN7GB0u_F5kloG3tbjll1peRPcKjWi1CJSki2V3obqMahHo4&name=14e.png "Builded")](https://fs.fab.moe/?explorer/share/file&hash=fc2dZ26mdN7GB0u_F5kloG3tbjll1peRPcKjWi1CJSki2V3obqMahHo4&name=14e.png "Builded")

13. Finally, open the VRC control panel to upload the avatar.

## Demo

[Demo video](https://www.bilibili.com/video/BV1uZ4y1w76G/)

## Todo

1. support radial puppet and axis puppet control

2. support chage locomotion animations.

3. support chage gesture animations.

# EasyAvatar3.0 [[English]](#easyavatar30-中文 "[English]")
不使用AnimatorController和参数来制作vrchat avatar3.0。

## 背景
通常avatar3.0制作流程包含了菜单创建，参数表设置，动画状态机的设置，这样avatar3.0的制作门槛就太高了。
而EasyAvatar3.0,省略参数表设置，动画状态机的设置这两步，让菜单直接控制动画。从而大大降低制作门槛。
对萌新来说很友好。

## 下载

1. [下载最新VRChatAvatarSDK](https://vrchat.com/download/sdk3-avatars/ "下载")

2. [下载最新EasyAvatar3.0](https://github.com/SkyTNT/EasyAvatar3.0/releases/latest "下载")

## 安装

1. 导入VRCAvatarSDK

2. 导入EasyAvatar3.0

## 使用

1. 在最上方的工具栏中，点击`EasyAvatar3.0/Language`可以选择语言
[![Language](https://fs.fab.moe/?explorer/share/file&hash=e686XaRLEC2MJF6SUsu7cY9uaIZSDxUnG318F0TJMzY7HVkR6xT6qgu7&name=1.png "Language")](https://fs.fab.moe/?explorer/share/file&hash=e686XaRLEC2MJF6SUsu7cY9uaIZSDxUnG318F0TJMzY7HVkR6xT6qgu7&name=1.png "Language")

2. 在Hierarchy视图中右键，点击`EasyAvatar3.0/Expression Menu Control`添加一个控件，它会自动包含在Avatar Helper和Menu中
[![Control](https://fs.fab.moe/?explorer/share/file&hash=6428snJditR6Q-hEp26phyv2UZvEYKGDZ6b5ZdE5OFMyeE5Bv9iluQwE&name=2.png "Control")](https://fs.fab.moe/?explorer/share/file&hash=6428snJditR6Q-hEp26phyv2UZvEYKGDZ6b5ZdE5OFMyeE5Bv9iluQwE&name=2.png "Control")

3. 将模型从Hierarchy中拖动到Avatar Helper中绑定模型
[![Bind Avatar](https://fs.fab.moe/?explorer/share/file&hash=7288BlBteLu65cPPBe1R1p12NXI6B-9rjPuFfR-68MI2O1xtf5iJGZvi&name=4.png "Bind Avatar")](https://fs.fab.moe/?explorer/share/file&hash=7288BlBteLu65cPPBe1R1p12NXI6B-9rjPuFfR-68MI2O1xtf5iJGZvi&name=4.png "Bind Avatar")

4. 选中刚才添加的控件，在Inspector视图中对它进行设置
[![Inspector Control](https://fs.fab.moe/?explorer/share/file&hash=fcaaagVgNDf8Cl0mJfuT2iZeVwwUvY_bqDls_HIW0x4XxrljWadhSe2l&name=3.png "Inspector Control")](https://fs.fab.moe/?explorer/share/file&hash=fcaaagVgNDf8Cl0mJfuT2iZeVwwUvY_bqDls_HIW0x4XxrljWadhSe2l&name=3.png "Inspector Control")

5. `名称`处可以设置控件显示的名称，当然也可以直接修改该控件物体的名字。
`图标`处设置控件显示的图标。
`控件关闭时的行为`指当控件在关闭状态下做的事。
这里可以添加多个行为，点击`+`以添加一个行为。
[![Add Behavior](https://fs.fab.moe/?explorer/share/file&hash=7283bi_QsTdVY6j_lkT9JmU8EoMomEV4nDqaMZh8NYnMGIaGn36VWxGO&name=6.png "Add Behavior")](https://fs.fab.moe/?explorer/share/file&hash=7283bi_QsTdVY6j_lkT9JmU8EoMomEV4nDqaMZh8NYnMGIaGn36VWxGO&name=6.png "Add Behavior")

6. 将你想要控制的物体拖动到行为的`目标`中
[![Set Target](https://fs.fab.moe/?explorer/share/file&hash=31f4jgtKJOi2ja6GDAp6kZc3_sf5bzT2Doe2P_hS3y-Dt5ZD9WY1MMyZ&name=5.png "Set Target")](https://fs.fab.moe/?explorer/share/file&hash=31f4jgtKJOi2ja6GDAp6kZc3_sf5bzT2Doe2P_hS3y-Dt5ZD9WY1MMyZ&name=5.png "Set Target")

7. 点击`属性`的输入框选择你想要修改的物体属性，例如`Is Active`
[![Set Property](https://fs.fab.moe/?explorer/share/file&hash=d8d9hFDVkvZAicT070q4zNQuF5t7FXNOevTCzNl7Eyc8DxMyraF8QqQ7&name=7.png "Set Property")](https://fs.fab.moe/?explorer/share/file&hash=d8d9hFDVkvZAicT070q4zNQuF5t7FXNOevTCzNl7Eyc8DxMyraF8QqQ7&name=7.png "Set Property")

8. `设置为`会根据选择的属性显示为不同的种类, 在这里设置你想要的值，例如这里就显示为复选框，取消勾选，那么在控件关闭时物体就会被隐藏反之物体就会显示
[![Set Value](https://fs.fab.moe/?explorer/share/file&hash=edfftVBTwr4_kpMrcGxbx1IhIWfrvq6gvIQG1U2x2P4BLGASHiFnGIhM&name=8.png "Set Value")](https://fs.fab.moe/?explorer/share/file&hash=edfftVBTwr4_kpMrcGxbx1IhIWfrvq6gvIQG1U2x2P4BLGASHiFnGIhM&name=8.png "Set Value")

9. 同理对控件打开时的行为进行设置
[![Set On Behaviors](https://fs.fab.moe/?explorer/share/file&hash=a410JvE5XxZhPAxls-SjIIubGT-UZEtmbqn4Zser9ddIwQJ1CmG3oNrT&name=10.png "Set On Behaviors")](https://fs.fab.moe/?explorer/share/file&hash=a410JvE5XxZhPAxls-SjIIubGT-UZEtmbqn4Zser9ddIwQJ1CmG3oNrT&name=10.png "Set On Behaviors")
这里并不是意味着打开行为和关闭行为的目标和属性要保持一致，他们可以完全不同。当一个属性被包含着某个控件的关闭行为中时。这个属性会默认情况下（控件关闭）一直保持设置状态。这样其他控件在打开时就设置不了该属性了。因此除非必要，最好不要设置关闭行为。

10. 同时你可以选择性勾选`使用动画文件`，你可以附加多个动画文件，控件打开或关闭时，对应行为和动画可以同时进行
[![Add Animation Clip](https://fs.fab.moe/?explorer/share/file&hash=97b3m1XN59ZiZQe3wKfgFdL2Iv3K-S68KLRlrO8Ak6qgrBGcTDmvv6sJ&name=11.png "Add Animation Clip")](https://fs.fab.moe/?explorer/share/file&hash=97b3m1XN59ZiZQe3wKfgFdL2Iv3K-S68KLRlrO8Ak6qgrBGcTDmvv6sJ&name=11.png "Add Animation Clip")

11. 一个控件设置完成后，可以添加和设置另一个控件。但一个菜单中的项目总数不能超过8。因此你可以添加子菜单。对着一个菜单右键选择`EasyAvatar3.0/Expression Menu`
[![Add Submenu](https://fs.fab.moe/?explorer/share/file&hash=afe06N1WBEfBSNOTzzRxeISXF8jwZMmyDeHB8hM_1ffv1ysDQ-rdLEs9&name=12.png "Add Submenu")](https://fs.fab.moe/?explorer/share/file&hash=afe06N1WBEfBSNOTzzRxeISXF8jwZMmyDeHB8hM_1ffv1ysDQ-rdLEs9&name=12.png "Add Submenu")

12. 当菜单做好之后，选中Avatar Helper(以`[模型]`开头的那个物体)，在Inspecor视图中点击`生成菜单并应用到模型`。每次修改完菜单后，上传模型之前你都必须点这个按钮。
[![Build](https://fs.fab.moe/?explorer/share/file&hash=f61aQ2-tvYbGmoTCVJyaQATxD46t1aEwLE_soTSxpcabIBYNssKFLyoY&name=13.png "Build")](https://fs.fab.moe/?explorer/share/file&hash=f61aQ2-tvYbGmoTCVJyaQATxD46t1aEwLE_soTSxpcabIBYNssKFLyoY&name=13.png "Build")
它会自动生成菜单，并设置在VRC Avatar Descriptor中，生成的文件存放在`Assets\EasyAvatar3.0\Build`中，你不需要管他，如图所示
[![Builded](https://fs.fab.moe/?explorer/share/file&hash=2718aumKQksGWLpd1W4JPo-bZaG-H7DzGQYDOkShPqz8VPb1LRiudXYR&name=14.png "Builded")](https://fs.fab.moe/?explorer/share/file&hash=2718aumKQksGWLpd1W4JPo-bZaG-H7DzGQYDOkShPqz8VPb1LRiudXYR&name=14.png "Builded")

13. 最后打开VRC控制面板上传模型

## 演示

[演示视频](https://www.bilibili.com/video/BV1uZ4y1w76G/)

## 待实现功能

1. 支持圆盘和轴圆盘控件

2. 支持修改走路动画

3. 支持修改手势动画

