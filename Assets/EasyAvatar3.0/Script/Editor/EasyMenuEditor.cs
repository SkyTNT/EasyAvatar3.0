using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyMenu))]
    public class EasyMenuEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
}


