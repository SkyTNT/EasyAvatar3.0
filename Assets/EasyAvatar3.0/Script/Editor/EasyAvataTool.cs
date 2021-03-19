using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace EasyAvatar
{
    
    public class EasyAvatarTool
    {

        [MenuItem("GameObject/EasyAvatar3.0/Avatar Helper", priority = 0)]
        public static bool CreateAvatarInfo()
        {
            GameObject gameObject = new GameObject(Lang.AvatarHelper);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create Avatar Helper");
            gameObject.AddComponent<EasyAvatarHelper>();

            if (Selection.activeGameObject)
                gameObject.transform.parent = Selection.activeGameObject.transform;
            Selection.activeGameObject = gameObject;

            return true;
        }

        [MenuItem("GameObject/EasyAvatar3.0/Expression Menu", priority = 0)]
        public static bool CreateExpressionMenu()
        {
            bool isSubMenu = false;

            if (Selection.activeGameObject)
            {
                //检查控件是否超过8
                if (GetMenuItemCount(Selection.activeGameObject.transform) >= 8)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuItemLen8, "ok");
                    return false;
                }
                //检查是否在控件中添加菜单
                if (Selection.activeGameObject.transform.GetComponent<EasyControl>())
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuInControl, "ok");
                    return false;
                }
                //检查是否Avatar已有菜单
                if (Selection.activeGameObject.transform.GetComponent<EasyAvatarHelper>() && GetMenuCount(Selection.activeGameObject.transform) >= 1)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarMenuLen1, "ok");
                    return false;
                }
                //检查是否为子菜单
                if (Selection.activeGameObject.transform.GetComponent<EasyMenu>())
                {
                    isSubMenu = true;
                }
            }

            //检查是否直接创建
            if (!Selection.activeGameObject || (!Selection.activeGameObject.transform.GetComponent<EasyAvatarHelper>() && !Selection.activeGameObject.transform.GetComponent<EasyMenu>()))
                if (!CreateAvatarInfo())
                    return false;


            GameObject gameObject = new GameObject(isSubMenu?Lang.SubMenu:Lang.MainMenu);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create Expression Menu");
            gameObject.AddComponent<EasyMenu>();

            if (Selection.activeGameObject)
                gameObject.transform.parent = Selection.activeGameObject.transform;
            Selection.activeGameObject = gameObject;

            return true;
        }

        [MenuItem("GameObject/EasyAvatar3.0/Expression Menu Control", priority = 0)]
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
                //检查是否在控件中添加控件
                if (Selection.activeGameObject.transform.GetComponent<EasyControl>())
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrControlInControl, "ok");
                    return false;
                }
                
            }
            //没有菜单则创建菜单
            if (!Selection.activeGameObject || !Selection.activeGameObject.transform.GetComponent<EasyMenu>())
                if (!CreateExpressionMenu())
                    return false;

            GameObject gameObject = new GameObject(Lang.Control);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create Menu Control");
            gameObject.AddComponent<EasyControl>();
            if (Selection.activeGameObject)
                gameObject.transform.parent = Selection.activeGameObject.transform;
            Selection.activeGameObject = gameObject;

            return true;
        }

        public static int GetMenuItemCount(Transform transform)
        {
            int count = 0;
            Transform child;
            for(int i = 0;i< transform.childCount; i++)
            {
                child = transform.GetChild(i);
                if (child.GetComponent<EasyMenu>() || child.GetComponent<EasyControl>())
                    count++;
            }
            return count;
        }

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


        public class Builder
        {
            static int buttonCount = 0;
            public static void BuildExpressionMenu(EasyAvatarHelper helper)
            {
                GameObject avatar = helper.avatar;
                if (!avatar)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarNotSet, "ok");
                    return;
                }
                EasyMenu mainMenu = null;
                foreach (Transform child in helper.gameObject.transform)
                {
                    EasyMenu temp = child.GetComponent<EasyMenu>();
                    if (temp)
                    {
                        if (mainMenu)//检测是否有多个主菜单
                        {
                            EditorUtility.DisplayDialog("Error", Lang.ErrAvatarMenuLen1, "ok");
                            return;
                        }
                        mainMenu = temp;
                    }
                }
                if (!mainMenu)//检测是否有主菜单
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarMenuLen0, "ok");
                    return;
                }

                if (Directory.Exists("Assets/EasyAvatar3.0/Build/Anim/"))
                    Directory.Delete("Assets/EasyAvatar3.0/Build/Anim/", true);
                if (Directory.Exists("Assets/EasyAvatar3.0/Build/Menu/"))
                    Directory.Delete("Assets/EasyAvatar3.0/Build/Menu/", true);
                Directory.CreateDirectory("Assets/EasyAvatar3.0/Build/Anim/");
                Directory.CreateDirectory("Assets/EasyAvatar3.0/Build/Menu/");
                List<VRCExpressionParameters.Parameter> parameters = new List<VRCExpressionParameters.Parameter>();
                VRCExpressionParameters expressionParameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                parameters.Add(new VRCExpressionParameters.Parameter() {name = "VRCEmote",valueType = VRCExpressionParameters.ValueType.Int });
                parameters.Add(new VRCExpressionParameters.Parameter() { name = "button", valueType = VRCExpressionParameters.ValueType.Int });
                expressionParameters.parameters = parameters.ToArray();
                AssetDatabase.CreateAsset(expressionParameters, "Assets/EasyAvatar3.0/Build/Menu/Parameters.asset");
                AssetDatabase.SaveAssets();
                buttonCount = 0;
                BuildExpressionMenu(avatar, mainMenu, "M");
            }

            private static VRCExpressionsMenu BuildExpressionMenu(GameObject avatar, EasyMenu menu, string prefix)
            {
                if (GetMenuItemCount(menu.gameObject.transform) > 8)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrMenuItemLen8, "ok");
                    return null;
                }

                VRCExpressionsMenu expressionsMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                AssetDatabase.CreateAsset(expressionsMenu, "Assets/EasyAvatar3.0/Build/Menu/" + prefix + ".asset");

                int count = 0;
                foreach (Transform child in menu.gameObject.transform)
                {
                    count++;
                    EasyMenu subMenu = child.GetComponent<EasyMenu>();
                    EasyControl control = child.GetComponent<EasyControl>();
                    if (control)
                    {
                        buttonCount++;
                        SerializedObject serializedObject = new SerializedObject(control);
                        serializedObject.Update();
                        //加_count_避免重名
                        EasyBehavior.GenerateAnimClip("Assets/EasyAvatar3.0/Build/Anim/" + prefix + "_" + count + "_" + control.name + "_off.anim", serializedObject.FindProperty("offBehaviors"));
                        EasyBehavior.GenerateAnimClip("Assets/EasyAvatar3.0/Build/Anim/" + prefix + "_" + count + "_" + control.name + "_on.anim", serializedObject.FindProperty("onBehaviors"));

                        VRCExpressionsMenu.Control vrcControl = new VRCExpressionsMenu.Control();
                        vrcControl.name = control.name;
                        vrcControl.type = VRCExpressionsMenu.Control.ControlType.Button;
                        vrcControl.icon = (Texture2D)serializedObject.FindProperty("icon").objectReferenceValue;
                        
                        expressionsMenu.controls.Add(vrcControl);
                    }

                    if (subMenu)
                    {

                        VRCExpressionsMenu.Control vrcControl = new VRCExpressionsMenu.Control();
                        vrcControl.name = subMenu.name;
                        vrcControl.type = VRCExpressionsMenu.Control.ControlType.SubMenu;
                        vrcControl.subMenu = BuildExpressionMenu(avatar, subMenu, prefix + "_" + count + "_" + subMenu.name);
                        expressionsMenu.controls.Add(vrcControl);
                    }
                }

                AssetDatabase.SaveAssets();
                return expressionsMenu;
            }
        }

    }

}

