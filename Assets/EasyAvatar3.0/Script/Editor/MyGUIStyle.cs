using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    public class MyGUIStyle
    {
        public static GUIStyle centerBoldLable;
        public static GUIStyle yellowLabel;

        static MyGUIStyle()
        {
            centerBoldLable = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontStyle = FontStyle.Bold
            };
            yellowLabel = new GUIStyle(EditorStyles.label);
            yellowLabel.normal.textColor = Color.yellow;
        }

    }
}
