﻿# EasyAvatar3.0
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

### 基础

1. #### 设置语言
在unity上方工具栏选择`EasyAvatar3.0/Language`即可切换语言

2. #### 添加物体
在Hierarchy窗口右键，选择`EasyAvatar3.0`可以添加相关物体。

3. #### 使用模板
在Hierarchy窗口右键，选择`EasyAvatar3.0/Template`可以添加模板。

### 模型助手
`模型`：绑定的模型。从Hierarchy窗口拖入到这个方框。可以随时换绑。
`生成并应用到模型`：生成动画，菜单，手势。并应用到模型。注意：每次修改完菜单或手势在上传之前都必须按下次按钮。此按钮只有绑定了模型才可见。

### 制作菜单

1. #### 菜单
菜单为容器，可以容纳子菜单和控件。其层次关系和vrc中菜单层次关系对应。
物体的名称即为在vrc中菜单显示的名称。

2. #### 控件
控件不是容器，不可以容纳任何物体。
`名称`：物体的名称即为在vrc菜单中控件显示的名称。
`图标`：vrc菜单中控件显示的图标。
`控件类型`：开关，按钮，旋钮，两轴操纵杆，改变姿态。
`保存开关状态`：是否在下次加载模型时使用上次模型开关的打开状态。
`开关默认打开`：是否重置模型后开关是默认打开的状态。
`关闭时自动恢复修改的内容到默认状态`：勾选的话可以不用设置关闭时触发的行为，也能将修改的内容恢复。
`自动设置追踪状态`：按照动画自动设置vrc的追踪状态。取消勾选，则手动设置。

 - ##### 开关
 包含关闭时触发的行为和打开触发时的行为

 - ##### 按钮
 包含松开按钮时触发的行为和按下按钮时触发的行为

 - ##### 旋钮
 包含关闭时触发的行为，和旋钮在任一值时的行为。
 值的范围为[0,1]

 - ##### 两轴操纵杆
 包含关闭时触发的行为和许多操纵杆在某一位置时的行为。
 可以添加和删除在操纵杆在某一位置时的行为。
 位置指操纵杆横轴和纵轴坐标，范围都为[-1,1]
 横轴：-1为左，1为右
 纵轴：-1为下，1为上

 - ##### 改变姿态
 设置要替换的姿态组后,在游戏里打开控件该就可以切换到对应姿态。

### 制作手势

1. #### 手势管理
手势管理为容器，可以容纳手势。
在手势管理中可以设置基础手势动画。注意，基础手势动画不能包含非人体动画。否则会出现问题。基础手势动画只能控制人体对应手部的手势。

2. #### 手势
包含当切换到其他手势时触发的行为和当做手势时触发的行为。
`手类型`：做手势的是左手、右手还是两只手都可以。
`手势类型`：普通（默认状态）、握拳、张手、指人、剪刀手、摇滚、手枪、大拇指。
`关闭时自动恢复修改的内容到默认状态`：同上。
`自动设置追踪状态`：同上。


### 行为列表

行为列表包含多个行为。可以预览所有行为。
行为分为：修改物体属性、播放动画

1. #### 修改物体属性行为
`目标`：模型中要修改的物体。将物体从Hierarchy窗口拖入其中。
`属性`：要修改的属性。单击方框选择属性。
`设置为`：将属性的值设置为你想要的值

2. #### 播放动画行为
`动画文件`：要播放的动画。
`动画有效部分`：控制动画中哪些部分可以动。也就是动画的遮罩。

3. #### 播放音乐开关行为
`音乐`：要播放的音频文件。
`开关`：控制音乐的打开。

4. #### 音乐音量调整
`音乐`：要调整音乐的音频文件。
`音量`：调整的音量。

5. #### 物体开关
`目标`：要开关的物体。
`开关`：物体打开还是关闭。

### 制作姿态

1. #### 姿态管理
姿态管理为容器，可以容纳姿态组。
`是否使用动画状态机`：是否使用动画状态机来控制姿态。
`默认使用的姿态`：默认使用的是哪个姿态组。

2. #### 姿态组
姿态组有五个部分：站立、蹲下、趴下、跳、其他
`是否使用混合树`：勾选的话就自己做一个混合树来控制那个姿态。
`速度`：动画播放的速度倍率
`镜像`：动画是否镜像播放
`追踪设置`：当切换到这个姿势时改变追踪状态
注意：目前只有默认使用的姿态组的AFK有效

### 上传模型

1. 在模型助手中点击`生成并应用到模型`

2. 打开VRC控制面板上传模型

## 演示

[演示视频](https://www.bilibili.com/video/BV1uZ4y1w76G/)

## 待实现功能

1. 从vrchat文件导入成EasyAvatar的结构


