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
            offBehaviorsList.elementHeight = onBehaviorsList.elementHeight = EditorGUIUtility.singleLineHeight;
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
            GameObject tempTarget =null;
            //获取目标物体
            if (avatar)
            {
                Transform tempTransform = avatar.transform.Find(targetPath.stringValue);
                if (tempTransform)
                    tempTarget = tempTransform.gameObject;
            }
            //当前avatar是否缺失目标物体（因为是目标物体相对于avatar的）
            bool isMissing = !tempTarget && targetPath.stringValue != "";
            //计算布局
            position.width /= 3;
            Rect targetLabelRect = new Rect(position)
            {
                width = 60
            };
            Rect targetFieldRect = new Rect(position)
            {
                x = targetLabelRect.width,
                width = position.width - targetLabelRect.width
                
            };
            Rect propertyLabelRect = new Rect(position)
            {
                x = position.width,
                width = 60
            };
            Rect propertyFieldRect = new Rect(position)
            {
                x = propertyLabelRect.x + propertyLabelRect.width,
                width = position.width - propertyLabelRect.width
            };

            //目标物体
            EditorGUI.LabelField(targetLabelRect, Lang.Target);
            tempTarget =(GameObject) EditorGUI.ObjectField(targetFieldRect, tempTarget, typeof(GameObject),true);
            if (isMissing)
                EditorGUI.LabelField(targetFieldRect, "Missing",MyGUIStyle.yellowLabel);
            if (tempTarget)
            {
                targetPath.stringValue = CalculateGameObjectPath(tempTarget);
            }
            if (GUILayout.Button("log"))
            {
                EasyPropertySelector.open(avatar,tempTarget);
                
                //LogProperty(tempTarget);
            }
            //EditorGUI.LabelField(propertyLabelRect, Lang.Target);

            //EditorGUI.PropertyField(propertyFieldRect, behavior.FindPropertyRelative("targetProperty"), new GUIContent());
           
        }

        public bool DrawButton(Rect rect,string content)
        {
            /*Event evt = Event.current;
            EventType type = evt.rawType;
            switch (type)
            {
                case EventType.MouseDown:
                    if (position.Contains(evt.mousePosition) && evt.button == 0)
                    {
                        GUIUtility.hotControl = id;
                        evt.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        GUIUtility.hotControl = 0;
                        evt.Use();

                        if (position.Contains(evt.mousePosition))
                        {
                            return true;
                        }
                    }
                    break;
            }*/
            return false;
        }

        public void LogProperty(GameObject gameObject)
        {
            EditorCurveBinding[] bindings= AnimationUtility.GetAnimatableBindings(gameObject, avatar);
            foreach(EditorCurveBinding binding in bindings)
            {
                Debug.Log(binding.propertyName);
                Debug.Log(binding.type);
                Debug.Log(binding.path);
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

