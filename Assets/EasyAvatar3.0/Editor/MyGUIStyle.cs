using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    public class MyGUIStyle
    {
        public static GUIStyle centerBoldLable = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            fontStyle = FontStyle.Bold
        };
    }
}
