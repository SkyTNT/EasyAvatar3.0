using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

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
            offBehaviorsList.onAddCallback = onBehaviorsList.onAddCallback = (ReorderableList list) => {
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
            if (GUILayout.Button("test"))
            {
                EasyBehavior.GenerateAnimClip("Assets/test.anim", onBehaviors);
            }
            if (GUILayout.Button("anim"))
            {
                //Animator animator = avatar.GetComponent<Animator>();
                


            }
            serializedObject.ApplyModifiedProperties();
        }  

        public void DrawBehavior(Rect position, SerializedProperty behavior)
        {
            SerializedProperty propertyGroup = behavior.FindPropertyRelative("propertyGroup");
            SerializedProperty property = propertyGroup.GetArrayElementAtIndex(0);
            SerializedProperty targetPath = property.FindPropertyRelative("targetPath");
            SerializedProperty targetProperty = property.FindPropertyRelative("targetProperty");
            SerializedProperty targetPropertyType = property.FindPropertyRelative("targetPropertyType");

            GameObject tempTarget =null,newTarget = null;
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
                width = Mathf.Max(position.width/4,50)
            };
            Rect targetFieldRect = new Rect(position)
            {
                x = targetLabelRect.x + targetLabelRect.width,
                width = position.width - targetLabelRect.width
                
            };
            Rect propertyLabelRect = new Rect(position)
            {
                y = position.y + position.height +6,
                width = Mathf.Max(position.width/4,50)
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
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect valueFieldRect = new Rect(position)
            {
                x = propertyLabelRect.x + propertyLabelRect.width,
                y = position.y + (position.height + 6) * 2,
                width = position.width - propertyLabelRect.width 
            };

            //目标物体
            EditorGUI.LabelField(targetLabelRect, Lang.Target);
            newTarget =(GameObject) EditorGUI.ObjectField(targetFieldRect, tempTarget, typeof(GameObject),true);
            
            if (isMissing)
            {
                Rect missingRect = new Rect(targetFieldRect) { width = targetFieldRect.width - targetFieldRect.height -2 };
                GUI.Box(missingRect, GUIContent.none, "Tag MenuItem");
                EditorGUI.LabelField(missingRect, Lang.Missing +":"+targetPath.stringValue, MyGUIStyle.yellowLabel);
            }
            if (newTarget != tempTarget&&!avatar)
            {
                EditorUtility.DisplayDialog("Error", Lang.ErrAvatarNotSet, "ok");
            }
            
            //当修改目标时
            if (newTarget != tempTarget)
            {
                if (avatar)
                {
                    EasyBehavior.PropertyGroupEdit(propertyGroup, "targetPath", CalculateGameObjectPath(newTarget));
                }
                //检查目标是否具有当前属性
                if (!EasyProperty.CheckProperty(avatar, property))
                    EasyProperty.ClearPropertyGroup(propertyGroup);
            }
            //属性选择
            EditorGUI.LabelField(propertyLabelRect, Lang.Property);
            EasyPropertySelector.DoSelect(propertyFieldRect, propertyGroup, avatar, tempTarget);
            EditorGUI.LabelField(valueLabelRect, Lang.SetTo);
            

            //输入值
            if (property.FindPropertyRelative("valueType").stringValue != "")
            {
                PropertyValueField(valueFieldRect, behavior);
            }
            

        }

        public void PropertyValueField(Rect rect, SerializedProperty behavior)
        {
            rect.x -= 3;
            rect.width += 6;

            SerializedProperty propertyGroup = behavior.FindPropertyRelative("propertyGroup");
            for(int i=0;i< propertyGroup.arraySize; i++)
            {
                SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i);
                SerializedProperty propertyValueType = property.FindPropertyRelative("valueType");
                SerializedProperty value = null;

                Type valueType = EasyReflection.FindType(propertyValueType.stringValue);

                Rect fieldRect = new Rect(rect)
                {
                    x = rect.x + i * (rect.width / propertyGroup.arraySize) + 3,
                    width = rect.width / propertyGroup.arraySize - 6
                };

                //显示Vector4之类的x,y,z,w或r,g,b,a
                if (propertyGroup.arraySize > 1)
                {
                    Rect lableRect = new Rect(fieldRect)
                    {
                        width = fieldRect.height
                    };
                    fieldRect.x += lableRect.width;
                    fieldRect.width -= lableRect.width;
                    string targetProperty = property.FindPropertyRelative("targetProperty").stringValue;
                    EditorGUI.LabelField(lableRect, targetProperty.Substring(targetProperty.Length-1));
                }

                if (valueType == typeof(bool))
                {
                    value = property.FindPropertyRelative("boolValue");
                    EditorGUI.PropertyField(fieldRect, value, GUIContent.none);
                }
                else if (valueType == typeof(float))
                {
                    value = property.FindPropertyRelative("floatValue");
                    EditorGUI.PropertyField(fieldRect, value, GUIContent.none);
                }
                else if (valueType == typeof(int))
                {
                    value = property.FindPropertyRelative("intValue");
                    EditorGUI.PropertyField(fieldRect, value, GUIContent.none);
                }
                else
                {
                    value = property.FindPropertyRelative("objectValue");
                    value.objectReferenceValue = EditorGUI.ObjectField(fieldRect, "", value.objectReferenceValue, valueType, true);
                }
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
            if (!gameObject)
                return "";
            return gameObject.transform.GetHierarchyPath(avatar.transform);
        }
        
    }
}

