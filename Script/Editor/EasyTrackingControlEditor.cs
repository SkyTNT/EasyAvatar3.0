using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EasyAvatar 
{
    [CustomPropertyDrawer(typeof(EasyTrackingControl))]
    public class EasyTrackingControlEditor : PropertyDrawer
    {
        SerializedProperty head, mouth, eyes, hip, rightHand, leftHand, rightFingers, leftFingers, rightFoot, leftFoot;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            head = property.FindPropertyRelative("head");
            mouth = property.FindPropertyRelative("mouth");
            eyes = property.FindPropertyRelative("eyes");
            hip = property.FindPropertyRelative("hip");
            rightHand = property.FindPropertyRelative("rightHand");
            leftHand = property.FindPropertyRelative("leftHand");
            rightFingers = property.FindPropertyRelative("rightFingers");
            leftFingers = property.FindPropertyRelative("leftFingers");
            rightFoot = property.FindPropertyRelative("rightFoot");
            leftFoot = property.FindPropertyRelative("leftFoot");

            GUI.Box(position,GUIContent.none);
            position.y += 5;
            position.x += 5;
            position.width -= 10;
            position.height = EditorGUIUtility.singleLineHeight;
            //表头,第一行
            EditorGUI.LabelField(new Rect(position) { x = position.x + position.width * 3 / 6, width = position.width / 6 }, Lang.TrackingTypeNoChange);
            EditorGUI.LabelField(new Rect(position) { x = position.x + position.width * 4 / 6, width = position.width / 6 }, Lang.TrackingTypeTracking);
            EditorGUI.LabelField(new Rect(position) { x = position.x + position.width * 5 / 6, width = position.width / 6 }, Lang.TrackingTypeAnimation);

            //全选，第二行
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(new Rect(position) { width = position.width / 2 }, Lang.BodyPartAll);
            EasyTrackingControl.Type type = (EasyTrackingControl.Type)head.enumValueIndex;
            bool all = 
                (head.enumValueIndex == mouth.enumValueIndex) && 
                (head.enumValueIndex == eyes.enumValueIndex) && 
                (head.enumValueIndex == hip.enumValueIndex) && 
                (head.enumValueIndex == rightHand.enumValueIndex) && 
                (head.enumValueIndex == leftHand.enumValueIndex) && 
                (head.enumValueIndex == rightFingers.enumValueIndex) && 
                (head.enumValueIndex == leftFingers.enumValueIndex) && 
                (head.enumValueIndex == rightFoot.enumValueIndex) && 
                (head.enumValueIndex == leftFoot.enumValueIndex);
            bool temp = false;
            EditorGUI.BeginChangeCheck();
            temp = EditorGUI.Toggle(new Rect(position) { x = position.x + position.width * 3 / 6, width = position.width / 6 }, type == EasyTrackingControl.Type.NoChange && all);
            if (EditorGUI.EndChangeCheck() && temp)
                SetAll(EasyTrackingControl.Type.NoChange);
            EditorGUI.BeginChangeCheck();
            temp = EditorGUI.Toggle(new Rect(position) { x = position.x + position.width * 4 / 6, width = position.width / 6 }, type == EasyTrackingControl.Type.Tracking && all);
            if (EditorGUI.EndChangeCheck() && temp)
                SetAll(EasyTrackingControl.Type.Tracking);
            EditorGUI.BeginChangeCheck();
            temp = EditorGUI.Toggle(new Rect(position) { x = position.x + position.width * 5 / 6, width = position.width / 6 }, type == EasyTrackingControl.Type.Animation && all);
            if (EditorGUI.EndChangeCheck() && temp)
                SetAll(EasyTrackingControl.Type.Animation);

            int linecCount = 1;
            DrawSingleProperty(position, ref linecCount, head, Lang.BodyPartHead);
            DrawSingleProperty(position, ref linecCount, mouth, Lang.BodyPartMouth);
            DrawSingleProperty(position, ref linecCount, eyes, Lang.BodyPartEyes);
            DrawSingleProperty(position, ref linecCount, hip, Lang.BodyPartHip);
            DrawSingleProperty(position, ref linecCount, rightHand, Lang.BodyPartRightHand);
            DrawSingleProperty(position, ref linecCount, leftHand, Lang.BodyPartLeftHand);
            DrawSingleProperty(position, ref linecCount, rightFingers, Lang.BodyPartRightFingers);
            DrawSingleProperty(position, ref linecCount, leftFingers, Lang.BodyPartLeftFingers);
            DrawSingleProperty(position, ref linecCount, rightFoot, Lang.BodyPartRightFoot);
            DrawSingleProperty(position, ref linecCount, leftFoot, Lang.BodyPartLeftFoot);
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 12 * EditorGUIUtility.singleLineHeight + 10;
        }

        private void SetAll(EasyTrackingControl.Type type)
        {
            head.enumValueIndex = mouth.enumValueIndex = eyes.enumValueIndex = hip.enumValueIndex = rightHand.enumValueIndex = leftHand.enumValueIndex = rightFingers.enumValueIndex = leftFingers.enumValueIndex = rightFoot.enumValueIndex = leftFoot.enumValueIndex = (int)type;
        }
        
        private void DrawSingleProperty(Rect position,ref int linecCount, SerializedProperty property ,string lable)
        {
            position.y += linecCount * EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(new Rect(position) { width = position.width / 2 }, lable);
            EasyTrackingControl.Type type =(EasyTrackingControl.Type)property.enumValueIndex;
            if (EditorGUI.Toggle(new Rect(position) { x = position.x + position.width *3 /6 , width = position.width / 6 }, type == EasyTrackingControl.Type.NoChange))
                type =EasyTrackingControl.Type.NoChange;
            if (EditorGUI.Toggle(new Rect(position) { x = position.x + position.width * 4 / 6, width = position.width / 6 }, type == EasyTrackingControl.Type.Tracking))
                type = EasyTrackingControl.Type.Tracking;
            if (EditorGUI.Toggle(new Rect(position) { x = position.x + position.width * 5 / 6, width = position.width / 6 }, type == EasyTrackingControl.Type.Animation))
                type = EasyTrackingControl.Type.Animation;
            property.enumValueIndex = (int)type;
            linecCount++;
        }
    }
}

