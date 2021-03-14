using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyControl))]
    public class EasyControlEditor : Editor
    {
        SerializedProperty icon,offBehaviors, onBehaviors;
        GameObject avatar;
        ReorderableList offBehaviorsList, onBehaviorsList;

        private void OnEnable()
        {
            
            serializedObject.Update();
            icon = serializedObject.FindProperty("icon");
            offBehaviors = serializedObject.FindProperty("offBehaviors");
            onBehaviors = serializedObject.FindProperty("onBehaviors");
            serializedObject.ApplyModifiedProperties();
            offBehaviorsList = new ReorderableList(serializedObject, offBehaviors, true, true, true, true);
            onBehaviorsList  = new ReorderableList(serializedObject, onBehaviors, true, true, true, true);
            offBehaviorsList.drawHeaderCallback = (Rect rect) => GUI.Label(rect, Lang.BehaviorOff);
            onBehaviorsList.drawHeaderCallback = (Rect rect) => GUI.Label(rect, Lang.BehaviorOn);
            offBehaviorsList.elementHeight = onBehaviorsList.elementHeight = EditorGUIUtility.singleLineHeight*3 +3*2*3;
            offBehaviorsList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => DrawBehavior(rect, offBehaviors.GetArrayElementAtIndex(index));
            onBehaviorsList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => DrawBehavior(rect, onBehaviors.GetArrayElementAtIndex(index));
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            avatar = GetAvatar();
            //名字设置
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Lang.Name);
            target.name = EditorGUILayout.TextField(target.name);
            if (target.name == "")
                target.name = Lang.Control;
            EditorGUILayout.EndHorizontal();
            //图标设置
            EditorGUILayout.PropertyField(icon, new GUIContent(Lang.Icon));
            //关闭行为
            offBehaviorsList.DoLayoutList();
            GUILayout.Space(10);
            //打开行为
            onBehaviorsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        public void DrawBehavior(Rect position, SerializedProperty behavior)
        {
            SerializedProperty targetPath = behavior.FindPropertyRelative("targetPath");
            SerializedProperty targetProperty = behavior.FindPropertyRelative("targetProperty");
            SerializedProperty targetPropertyType = behavior.FindPropertyRelative("targetPropertyType");

            GameObject tempTarget =null;
            //获取目标物体
            if (avatar && targetPath.stringValue != "")
            {
                Transform tempTransform = avatar.transform.Find(targetPath.stringValue);
                if (tempTransform)
                    tempTarget = tempTransform.gameObject;
            }
            //当前avatar是否缺失目标物体（因为是目标物体相对于avatar的）
            bool isMissing = !tempTarget && targetPath.stringValue != "";
            //计算布局
            position.y += 3;
            position.height = EditorGUIUtility.singleLineHeight;


            Rect targetLabelRect = new Rect(position)
            {
                width = Mathf.Max(position.width/3,100)
            };
            Rect targetFieldRect = new Rect(position)
            {
                x = targetLabelRect.x + targetLabelRect.width,
                width = position.width - targetLabelRect.width
                
            };
            Rect propertyLabelRect = new Rect(position)
            {
                y = position.y + position.height +6,
                width = Mathf.Max(position.width/3,100)
            };
            Rect propertyFieldRect = new Rect(position)
            {
                x = propertyLabelRect.x + propertyLabelRect.width,
                y =position.y + position.height +6,
                width = position.width - propertyLabelRect.width
            };
            Rect valueLabelRect = new Rect(position)
            {
                y = position.y + (position.height + 6) * 2,
                width = Mathf.Max(position.width / 3, 100)
            };
            Rect valueFieldRect = new Rect(position)
            {
                x = propertyLabelRect.x + propertyLabelRect.width,
                y = position.y + (position.height + 6) * 2,
                width = position.width - propertyLabelRect.width
            };

            //目标物体
            EditorGUI.LabelField(targetLabelRect, Lang.Target);
            tempTarget =(GameObject) EditorGUI.ObjectField(targetFieldRect, tempTarget, typeof(GameObject),true);
            if (isMissing)
            {
                Rect missingRect = new Rect(targetFieldRect) { width = targetFieldRect.width - targetFieldRect.height -2 };
                GUI.Box(missingRect, GUIContent.none, "Tag MenuItem");
                EditorGUI.LabelField(missingRect, Lang.Missing +":"+targetPath.stringValue, MyGUIStyle.yellowLabel);
            }
            if (tempTarget)
            {
                targetPath.stringValue = CalculateGameObjectPath(tempTarget);
            }
            
            EditorGUI.LabelField(propertyLabelRect, Lang.Property);
            EasyPropertySelector.EditorCurveBindingField(propertyFieldRect, targetProperty, targetPropertyType, avatar, tempTarget);
            EditorGUI.LabelField(valueLabelRect, Lang.SetTo);
            if (GUI.Button(valueFieldRect, "log"))
            {
                Debug.Log(AnimationUtility.GetEditorCurveValueType(avatar, new EditorCurveBinding { path = targetPath.stringValue, propertyName = targetProperty.stringValue, type = System.Type.GetType(targetPropertyType.stringValue) }));
                
            }

        }
       
        

        public GameObject GetAvatar()
        {
            EasyAvatarHelper avatarHelper = ((MonoBehaviour)target).GetComponentInParent<EasyAvatarHelper>();
            //检测是否本控件在是否在Avatar Helper中
            if (!avatarHelper)
                return null;
            GameObject avatar = avatarHelper.avatar;
            //检测是否在Avatar Helper中设置了avatar
            if (!avatar)
                return null;
            return avatar;
        }


        public string CalculateGameObjectPath(GameObject gameObject)
        {

            return gameObject.transform.GetHierarchyPath(avatar.transform);
        }
        
    }
}

