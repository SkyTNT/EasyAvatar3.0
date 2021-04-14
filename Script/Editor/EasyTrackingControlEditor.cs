using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EasyAvatar 
{
    [CustomPropertyDrawer(typeof(EasyTrackingControl))]
    public class EasyTrackingControlEditor : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            SerializedProperty head = property.FindPropertyRelative("head");
            SerializedProperty mouth = property.FindPropertyRelative("mouth");
            SerializedProperty eyes = property.FindPropertyRelative("eyes");
            SerializedProperty hip = property.FindPropertyRelative("hip");
            SerializedProperty rightHand = property.FindPropertyRelative("rightHand");
            SerializedProperty leftHand = property.FindPropertyRelative("leftHand");
            SerializedProperty rightFingers = property.FindPropertyRelative("rightFingers");
            SerializedProperty leftFingers = property.FindPropertyRelative("leftFingers");
            SerializedProperty rightFoot = property.FindPropertyRelative("rightFoot");
            SerializedProperty leftFoot = property.FindPropertyRelative("leftFoot");

            int linecCount = 0;
            DrawSingleProperty(position, ref linecCount, head);
            DrawSingleProperty(position, ref linecCount, mouth);
            DrawSingleProperty(position, ref linecCount, eyes);
            DrawSingleProperty(position, ref linecCount, hip);
            DrawSingleProperty(position, ref linecCount, rightHand);
            DrawSingleProperty(position, ref linecCount, leftHand);
            DrawSingleProperty(position, ref linecCount, rightFingers);
            DrawSingleProperty(position, ref linecCount, leftFingers);
            DrawSingleProperty(position, ref linecCount, rightFoot);
            DrawSingleProperty(position, ref linecCount, leftFoot);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 10 * EditorGUIUtility.singleLineHeight;
        }

        private void DrawSingleProperty(Rect position,ref int linecCount, SerializedProperty property)
        {
            EditorGUI.PropertyField(new Rect(position) { y = position.y + linecCount * EditorGUIUtility.singleLineHeight, height = EditorGUIUtility.singleLineHeight }, property);
            linecCount++;
        }
    }
}

