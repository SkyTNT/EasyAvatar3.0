# EasyAvatar3.0--A easy way to make vrchat avatar3.0

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

5. `名称`处可以设置控件显示的名称，当然也可以直接该控件物体的名字。
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
这里并不是意味着打开行为和关闭行为的目标和属性要保持一致，他们可以完全不同，一个控件只会执行其中的一个行为。

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
