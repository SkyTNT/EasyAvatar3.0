using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    public class EasyMissingTargetReplacer : EditorWindow
    {
        public static void Field(EasyAvatarHelper helper)
        {
            if (GUILayout.Button(Lang.OneClickReplaceMissingTarget))
            {
                EasyMissingTargetReplacer easyMissingTargetReplacer = CreateInstance<EasyMissingTargetReplacer>();
                easyMissingTargetReplacer.Show();
            }
        }
        private void OnGUI()
        {
            
        }
    }
}


