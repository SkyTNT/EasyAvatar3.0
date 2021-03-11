using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EasyAvatar
{
    public class AvatarMenuEditor : EditorWindow
    {
        [MenuItem("Window/EasyAvatar3.0/MenuEditor")]
        public static void openWindows()
        {
            AvatarMenuEditor menuEdit = GetWindow<AvatarMenuEditor>();
            menuEdit.titleContent = new GUIContent("Menu Edit");
            menuEdit.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label(EasyAvatarEditor.editingMenu == null ? Lang.ExpressionMenuNotEditing: EasyAvatarEditor.editingMenu.name, MyGUIStyle.centerBoldLable);
            GUILayout.EndVertical();
        }
    }
}

