using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EasyAvatar
{
    public class EasyAvatarEditor : EditorWindow
    {
        private string version = "1.0.0";
        private bool useEnglise = false;
        private bool fristGUIFrame = true;

        private GameObject avatarObject = null;
        private static ArrayList menuList = new ArrayList();
        private static ArrayList menuGUIList = new ArrayList();
        private static ArrayList menuGUIDeleteList = new ArrayList();
        public static AvatarMenu editingMenu = null;
        

        [MenuItem("Window/EasyAvatar3.0/Main")]
        private static void openWindows()
        {
            EasyAvatarEditor easyAvatarEditor = GetWindow<EasyAvatarEditor>();
            easyAvatarEditor.titleContent = new GUIContent("EasyAvatar3.0");
            easyAvatarEditor.Show();
        }


        private void OnGUI()
        {
            if (fristGUIFrame)
            {
                fristGUIFrame = false;
                Lang.Change(useEnglise ? Lang.Type.En : Lang.Type.Zh);
            }
            GUILayout.BeginVertical();
            GUILayout.Label("EasyAvatar3.0 v" + version, MyGUIStyle.centerBoldLable);
            GUILayout.BeginHorizontal();
            GUILayout.Label("language:", EditorStyles.boldLabel);
            if (GUILayout.Button(useEnglise ? "English":"Chinese"))
            {
                useEnglise = !useEnglise;
                Lang.Change(useEnglise ? Lang.Type.En : Lang.Type.Zh);
            }
            GUILayout.EndHorizontal();
            GUILayout.Label(Lang.ExpressionMenu,EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope("box"))
            {
                foreach(MenuGUI menuGUI in menuGUIList)
                {
                    menuGUI.OnGUI();
                }
                //不能在上面那个for中执行删除操作
                foreach (MenuGUI menuGUI in menuGUIDeleteList)
                {
                    menuGUIList.Remove(menuGUI);
                    menuList.Remove(menuGUI.menu);
                }
                menuGUIDeleteList.Clear();

                if (GUILayout.Button(Lang.ExpressionMenuAdd))
                {
                    addMenu();
                }
            }
            GUILayout.EndVertical();
        }

        public static void addMenu()
        {
            AvatarMenu menu = new AvatarMenu(Lang.ExpressionMenuNew);
            MenuGUI menuGUI = new MenuGUI(menu);
            menuList.Add(menu);
            menuGUIList.Add(menuGUI);
        }

        public static void deleteMenu(MenuGUI menuGUI)
        {
            menuGUIDeleteList.Add(menuGUI);
            if (editingMenu == menuGUI.menu)
                editingMenu = null;
        }
    }

    public class MenuGUI
    {
        public AvatarMenu menu;
        private bool show = true;
        public MenuGUI(AvatarMenu menu)
        {
            this.menu = menu;
        }
        public void OnGUI()
        {
            show = EditorGUILayout.Foldout(show, menu.name);
            if (show)
            {
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    menu.name = GUILayout.TextField(menu.name);
                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button(Lang.ExpressionMenuEdit))
                    {
                        EasyAvatarEditor.editingMenu = menu;
                        AvatarMenuEditor.openWindows();
                    }
                    if (GUILayout.Button(Lang.ExpressionMenuDel))
                    {
                        EasyAvatarEditor.deleteMenu(this);
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }

    
}

