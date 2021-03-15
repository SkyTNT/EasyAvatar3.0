using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

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

        public static string NicifyPropertyGroupName(Type animatableObjectType, string propertyGroupName)
        {
            string result = GetPropertyGroupName(GetPropertyDisplayName(propertyGroupName));
            if (animatableObjectType == typeof(RectTransform) & result.Equals("Position"))
                result = "Position (Z)";

            return result;
        }


        //Reference: https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/Animation/AnimationWindow/AnimationWindowUtility.cs
        public static string GetPropertyGroupName(string propertyName)
        {
            if (GetComponentIndex(propertyName) != -1)
                return propertyName.Substring(0, propertyName.Length - 2);

            return propertyName;
        }
        //Reference: https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/Animation/AnimationWindow/AnimationWindowUtility.cs
        public static string GetPropertyDisplayName(string propertyName)
        {
            propertyName = propertyName.Replace("m_LocalPosition", "Position");
            propertyName = propertyName.Replace("m_LocalScale", "Scale");
            propertyName = propertyName.Replace("m_LocalRotation", "Rotation");
            propertyName = propertyName.Replace("localEulerAnglesBaked", "Rotation");
            propertyName = propertyName.Replace("localEulerAnglesRaw", "Rotation");
            propertyName = propertyName.Replace("localEulerAngles", "Rotation");
            propertyName = propertyName.Replace("m_Materials.Array.data", "Material Reference");

            propertyName = ObjectNames.NicifyVariableName(propertyName);
            propertyName = propertyName.Replace("m_", "");

            return propertyName;
        }
        //Reference: https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/Animation/AnimationWindow/AnimationWindowUtility.cs
        static public int GetComponentIndex(string name)
        {
            if (name == null || name.Length < 3 || name[name.Length - 2] != '.')
                return -1;
            char lastCharacter = name[name.Length - 1];
            switch (lastCharacter)
            {
                case 'r':
                    return 0;
                case 'g':
                    return 1;
                case 'b':
                    return 2;
                case 'a':
                    return 3;
                case 'x':
                    return 0;
                case 'y':
                    return 1;
                case 'z':
                    return 2;
                case 'w':
                    return 3;
                default:
                    return -1;
            }
        }
    }
}

