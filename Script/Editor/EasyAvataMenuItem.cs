using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using VRC.SDKBase.Editor;

namespace EasyAvatar
{

    public class EasyAvatarMenuItem
    {

        [MenuItem("GameObject/EasyAvatar3.0/Template(模板)", priority = 0)]
        public static bool CerateTemplate()
        {
            GameObject helper = CreateObject<EasyAvatarHelper>(Lang.AvatarHelper);
            GameObject menu = CreateObject<EasyMenu>(Lang.MainMenu);
            CreateObject<EasyControl>(Lang.Control);
            Selection.activeGameObject = helper;
            GameObject gestureManager = CreateObject<EasyGestureManager>(Lang.GestureManager);
            EasyGestureManagerEditor.SetDefaultGesture(new SerializedObject(gestureManager.GetComponent<EasyGestureManager>()));
            CreateObject<EasyGesture>(Lang.Gesture);
            Selection.activeGameObject = helper;
            return true;
        }
        /// <summary>
        /// 创建AvatarHelper
        /// </summary>
        /// <returns>是否成功</returns>
        [MenuItem("GameObject/EasyAvatar3.0/Avatar Helper(模型助手)", priority = 0)]
        public static bool CreateAvatarHelper()
        {
            CreateObject<EasyAvatarHelper>(Lang.AvatarHelper);
            return true;
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <returns>是否成功</returns>
        [MenuItem("GameObject/EasyAvatar3.0/Expression Menu(菜单)", priority = 0)]
        public static bool CreateExpressionMenu()
        {
            bool isSubMenu = false;

            if (Selection.activeGameObject)
            {
                //检查添加菜单位置
                if (!Selection.activeGameObject.transform.GetComponent<EasyMenu>() && !Selection.activeGameObject.transform.GetComponent<EasyAvatarHelper>())
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuPath, "ok");
                    return false;
                }
                //检查是否Avatar已有菜单
                if (Selection.activeGameObject.transform.GetComponent<EasyAvatarHelper>() && GetMenuCount(Selection.activeGameObject.transform) >= 1)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarMenuLen1, "ok");
                    return false;
                }
                //检查控件是否超过8
                if (GetMenuItemCount(Selection.activeGameObject.transform) >= 8)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuItemLen8, "ok");
                    return false;
                }
                //检查是否为子菜单
                if (Selection.activeGameObject.transform.GetComponent<EasyMenu>())
                {
                    isSubMenu = true;
                }
                CreateObject<EasyMenu>(isSubMenu ? Lang.SubMenu : Lang.MainMenu);
                return true;
            }

            EditorUtility.DisplayDialog("Error", Lang.ErrMenuPath, "ok");
            return false;
        }

        /// <summary>
        /// 创建控件
        /// </summary>
        /// <returns>是否成功</returns>
        [MenuItem("GameObject/EasyAvatar3.0/Expression Menu Control(控件)", priority = 0)]
        public static bool CreateExpressionMenuControl()
        {
            if (Selection.activeGameObject)
            {
                //检查控件是否超过8
                if (GetMenuItemCount(Selection.activeGameObject.transform) >= 8)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuItemLen8, "ok");
                    return false;
                }
                //检查是否在菜单控件
                if (!Selection.activeGameObject.transform.GetComponent<EasyMenu>())
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrControlPath, "ok");
                    return false;
                }
                CreateObject<EasyControl>(Lang.Control);
                return true;
            }
            EditorUtility.DisplayDialog("Error", Lang.ErrControlPath, "ok");
            return false;
        }

        /// <summary>
        /// 创建手势管理
        /// </summary>
        /// <returns></returns>
        [MenuItem("GameObject/EasyAvatar3.0/Gesture Manager(手势管理)", priority = 0)]
        public static bool CreateGestureManager()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.transform.GetComponent<EasyAvatarHelper>())
            {
                //EasyGestureManager已经添加了
                if (Selection.activeGameObject.transform.GetComponentInChildren<EasyGestureManager>())
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarGestureManagerLen1, "ok");
                    return false;
                }
                GameObject gameObject = CreateObject<EasyGestureManager>(Lang.GestureManager);
                EasyGestureManagerEditor.SetDefaultGesture(new SerializedObject(gameObject.GetComponent<EasyGestureManager>()));
                return true;
            }
            EditorUtility.DisplayDialog("Error", Lang.ErrGestureManagerPath, "ok");
            return false;
        }

        /// <summary>
        /// 创建手势
        /// </summary>
        /// <returns></returns>
        [MenuItem("GameObject/EasyAvatar3.0/Gesture(手势)", priority = 0)]
        public static bool CreateGesture()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.transform.GetComponent<EasyGestureManager>())
            {
                CreateObject<EasyGesture>(Lang.Gesture);
                return true;
            }
            EditorUtility.DisplayDialog("Error", Lang.ErrGesturePath, "ok");
            return false;
        }

        /// <summary>
        /// 创建姿态管理
        /// </summary>
        /// <returns></returns>
        [MenuItem("GameObject/EasyAvatar3.0/Locomotion Manager(姿态管理)", priority = 0)]
        public static bool CreateLocomotionManager()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.transform.GetComponent<EasyAvatarHelper>())
            {
                //LocomotionManager已经添加了
                if (Selection.activeGameObject.transform.GetComponentInChildren<EasyLocomotionManager>())
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarLocomotionManagerLen1, "ok");
                    return false;
                }
                GameObject gameObject = CreateObject<EasyLocomotionManager>(Lang.LocomotionManager);
                EasyLocomotionManagerEditor.SetAllDefaultLocomotion(new SerializedObject(gameObject.GetComponent<EasyLocomotionManager>()));
                return true;
            }
            EditorUtility.DisplayDialog("Error", Lang.ErrLocomotionManagerPath, "ok");
            return false;
        }

        /// <summary>
        /// 显示关于对话框
        /// </summary>
        [MenuItem("EasyAvatar3.0/About", priority = 0)]
        public static void showAbout()
        {
            EditorUtility.DisplayDialog("About", Lang.About, "ok");
            Debug.Log(Lang.About);
        }

        /// <summary>
        /// 通过组件创建GameObject
        /// </summary>
        /// <typeparam name="T">组件</typeparam>
        /// <param name="name">新建物体的名字</param>
        public static GameObject CreateObject<T>(string name) where T : Component
        {
            GameObject gameObject = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + name);
            gameObject.AddComponent<T>();
            if (Selection.activeGameObject)
                gameObject.transform.parent = Selection.activeGameObject.transform;
            Selection.activeGameObject = gameObject;
            return gameObject;
        }

        /// <summary>
        /// 获取菜单下项目的数量
        /// </summary>
        /// <param name="transform">查询的菜单</param>
        /// <returns>数量</returns>
        public static int GetMenuItemCount(Transform transform)
        {
            int count = 0;
            Transform child;
            for (int i = 0; i < transform.childCount; i++)
            {
                child = transform.GetChild(i);
                if (child.GetComponent<EasyMenu>() || child.GetComponent<EasyControl>())
                    count++;
            }
            return count;
        }

        /// <summary>
        /// transform下获取菜单的数量
        /// </summary>
        /// <param name="transform">要查询的transform</param>
        /// <returns>数量</returns>
        public static int GetMenuCount(Transform transform)
        {
            int count = 0;
            Transform child;
            for (int i = 0; i < transform.childCount; i++)
            {
                child = transform.GetChild(i);
                if (child.GetComponent<EasyMenu>())
                    count++;
            }
            return count;
        }

    }

}