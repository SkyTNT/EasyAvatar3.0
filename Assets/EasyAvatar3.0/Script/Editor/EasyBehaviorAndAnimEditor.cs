using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class EasyBehaviorAndAnimEditor 
{
    SerializedProperty behaviors, animClips;
    ReorderableList behaviorsList, animClipList;
    public bool useanimClip;

    public EasyBehaviorAndAnimEditor(SerializedProperty behaviors, SerializedProperty animClips)
    {
        this.behaviors = behaviors;
        this.animClips = animClips;

        behaviorsList = new ReorderableList(behaviors.serializedObject, behaviors, true, true, true, true);
        animClipList = new ReorderableList(animClips.serializedObject, animClips, true, true, true, true);
        behaviorsList.drawHeaderCallback = (Rect rect) => { };
        behaviorsList.elementHeight = (EditorGUIUtility.singleLineHeight + 6) * 3;
        //behaviorsList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => DrawBehavior(rect, offBehaviors.GetArrayElementAtIndex(index));
        behaviorsList.onAddCallback = (ReorderableList list) => {
            if (list.serializedProperty != null)
            {
                list.serializedProperty.arraySize++;
                list.index = list.serializedProperty.arraySize - 1;
                SerializedProperty propertyGroup = list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("propertyGroup");
                //要求propertyGroup至少有一个元素
                if (propertyGroup.arraySize == 0)
                    propertyGroup.arraySize++;
            }
            else
            {
                ReorderableList.defaultBehaviours.DoAddButton(list);
            }
        };

        animClipList.drawHeaderCallback =  (Rect rect) => { };
        animClipList.elementHeight = EditorGUIUtility.singleLineHeight + 6;
        //animClipList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => DrawAnimClip(rect, offAnims.GetArrayElementAtIndex(index));
        
    }

    public void LayoutGUI()
    {

    }


}
