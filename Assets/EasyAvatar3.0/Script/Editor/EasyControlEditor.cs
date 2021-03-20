using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using System.IO;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyControl))]
    public class EasyControlEditor : Editor
    {
        //当前复制的Behaviors
        static EasyControl copiedTarget;
        static string copiedBehaviorsPath;

        SerializedProperty icon,offBehaviors, onBehaviors , previewingBehaviors;
        GameObject avatar;
        ReorderableList offBehaviorsList, onBehaviorsList;
        SkinnedMeshRenderer testObject;



        private void OnEnable()
        {
            
            serializedObject.Update();
            icon = serializedObject.FindProperty("icon");
            offBehaviors = serializedObject.FindProperty("offBehaviors");
            onBehaviors = serializedObject.FindProperty("onBehaviors");
            serializedObject.ApplyModifiedProperties();
            offBehaviorsList = new ReorderableList(serializedObject, offBehaviors, true, true, true, true);
            onBehaviorsList  = new ReorderableList(serializedObject, onBehaviors, true, true, true, true);
            offBehaviorsList.drawHeaderCallback = onBehaviorsList.drawHeaderCallback = (Rect rect) => { };
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

        private void OnDestroy()
        {
            StopPreview();
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
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Lang.Icon);
            EditorGUILayout.PropertyField(icon, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            //关闭行为
            GUILayout.Label(Lang.BehaviorOff, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            Color preBg = GUI.backgroundColor;
            GUI.backgroundColor = previewingBehaviors == offBehaviors ? MyGUIStyle.activeButtonColor : preBg;
            if (GUILayout.Button(Lang.Preview))
            {
                if (previewingBehaviors != offBehaviors)
                    StartPreview(offBehaviors);
                else
                    StopPreview();
            }
            GUI.backgroundColor = preBg;

            if (GUILayout.Button(Lang.Copy))
            {
                CopyBehaviors(offBehaviors);
            }
            if (GUILayout.Button(Lang.Paste))
            {
                PasteBehaviors(offBehaviors);
            }
            EditorGUILayout.EndHorizontal();
            offBehaviorsList.DoLayoutList();

            //打开行为
            GUILayout.Label(Lang.BehaviorOn, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = previewingBehaviors == onBehaviors ? MyGUIStyle.activeButtonColor : preBg;
            if (GUILayout.Button(Lang.Preview))
            {
                if (previewingBehaviors != onBehaviors)
                    StartPreview(onBehaviors);
                else
                    StopPreview();

            }
            GUI.backgroundColor = preBg;
            if (GUILayout.Button(Lang.Copy))
            {
                CopyBehaviors(onBehaviors);
            }
            if (GUILayout.Button(Lang.Paste))
            {
                PasteBehaviors(onBehaviors);
            }
            EditorGUILayout.EndHorizontal();
            onBehaviorsList.DoLayoutList();
            

            if (previewingBehaviors != null)
            Preview(previewingBehaviors);

            serializedObject.ApplyModifiedProperties();
            
        }

        public void StartPreview(SerializedProperty behaviors)
        {
            Animator animator = avatar.GetComponent<Animator>();
            if(!animator)
            {
                animator = avatar.AddComponent<Animator>();
            }
            animator.applyRootMotion = false;
            //没有Controller会崩溃
            if (!animator.runtimeAnimatorController)
            {
                if (!Directory.Exists("Assets/EasyAvatar3.0/Build/"))
                    Directory.CreateDirectory("Assets/EasyAvatar3.0/Build/");
                AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/EasyAvatar3.0/Build/preview.controller");
                animator.runtimeAnimatorController = controller;
            }
            previewingBehaviors = behaviors;
            if (!AnimationMode.InAnimationMode())
                AnimationMode.StartAnimationMode();
        }

        public void StopPreview()
        {
            previewingBehaviors = null;
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
        }

        public void Preview(SerializedProperty behaviors)
        {
            if (!AnimationMode.InAnimationMode() || !avatar)
                return;

            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(avatar, EasyAvatarTool.Utility.GenerateAnimClip(behaviors), 0);
            AnimationMode.EndSampling();
        }

        public static void CopyBehaviors(SerializedProperty behaviors)
        {
            copiedTarget =(EasyControl)behaviors.serializedObject.targetObject;
            copiedBehaviorsPath = behaviors.propertyPath;
        }

        public static void PasteBehaviors(SerializedProperty behaviors)
        {
            if (!copiedTarget)
                return;
            SerializedObject serializedObject = new SerializedObject(copiedTarget);
            serializedObject.Update();
            SerializedProperty copiedBehaviors = serializedObject.FindProperty(copiedBehaviorsPath);
            behaviors.arraySize = copiedBehaviors.arraySize;
            for (int i = 0; i < behaviors.arraySize; i++)
            {
                EasyAvatarTool.Utility.CopyBehavior(behaviors.GetArrayElementAtIndex(i), copiedBehaviors.GetArrayElementAtIndex(i));
            }
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
                    EasyAvatarTool.Utility.PropertyGroupEdit(propertyGroup, "targetPath", CalculateGameObjectPath(newTarget));
                //检查目标是否具有当前属性
                if (!EasyAvatarTool.Utility.CheckProperty(avatar, property))
                    EasyAvatarTool.Utility.ClearPropertyGroup(propertyGroup);
            }
            //属性选择
            EditorGUI.LabelField(propertyLabelRect, Lang.Property);
            EasyPropertySelector.DoSelect(propertyFieldRect, propertyGroup, avatar, tempTarget);
            EditorGUI.LabelField(valueLabelRect, Lang.SetTo);
            

            //输入值
            if (property.FindPropertyRelative("valueType").stringValue != "")
                PropertyValueField(valueFieldRect, behavior.FindPropertyRelative("propertyGroup"));
        }


        public void PropertyValueField(Rect rect, SerializedProperty propertyGroup)
        {

            if(EasyAvatarTool.Utility.PropertyGroupIsBlendShape(propertyGroup))//特殊情况Blend Shape
            {
                SerializedProperty value = propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("floatValue");
                value.floatValue = EditorGUI.Slider(rect, value.floatValue, 0, 100);
            }
            else if (EasyAvatarTool.Utility.PropertyGroupIsColor(propertyGroup))//特殊情况Color
            {
                Dictionary<string, SerializedProperty> colorMap = new Dictionary<string, SerializedProperty>();
                for (int i = 0; i < propertyGroup.arraySize; i++)
                {
                    SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i);
                    colorMap.Add(EasyAvatarTool.Utility.GetPropertyGroupSubname(property), property.FindPropertyRelative("floatValue"));
                }
                SerializedProperty valueR = colorMap["r"];
                SerializedProperty valueG = colorMap["g"];
                SerializedProperty valueB = colorMap["b"];
                SerializedProperty valueA = colorMap["a"];

                Color tempColor = new Color(valueR.floatValue, valueG.floatValue, valueB.floatValue, valueA.floatValue);
                Color newColor = EditorGUI.ColorField(rect, tempColor);
                if (tempColor != newColor)
                {
                    valueR.floatValue = newColor.r;
                    valueG.floatValue = newColor.g;
                    valueB.floatValue = newColor.b;
                    valueA.floatValue = newColor.a;
                }
            }
            else //一般情况
            {
                rect.x -= 3;
                rect.width += 6;

                for (int i = 0; i < propertyGroup.arraySize; i++)
                {

                    SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i);
                    SerializedProperty propertyValueType = property.FindPropertyRelative("valueType");
                    SerializedProperty isPPtr = property.FindPropertyRelative("isPPtr");
                    SerializedProperty value = null;

                    Type valueType = EasyReflection.FindType(propertyValueType.stringValue);

                    Rect fieldRect = new Rect(rect)
                    {
                        x = rect.x + i * (rect.width / propertyGroup.arraySize) + 3,
                        width = rect.width / propertyGroup.arraySize - 6
                    };
                    GUIContent label = GUIContent.none;
                    //显示Vector之类的x,y,z,w或r,g,b,a
                    if (propertyGroup.arraySize > 1)
                    {
                        label = new GUIContent(EasyAvatarTool.Utility.GetPropertyGroupSubname(propertyGroup, i).ToUpper());
                    }

                    float preLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(label).x;
                    if (!isPPtr.boolValue)
                    {
                        value = property.FindPropertyRelative("floatValue");
                        if (valueType == typeof(bool))
                            value.floatValue = Convert.ToSingle(EditorGUI.Toggle(fieldRect, label, Convert.ToBoolean(value.floatValue)));
                        else if (valueType == typeof(float))
                            value.floatValue = EditorGUI.FloatField(fieldRect, label, value.floatValue);
                        else if (valueType == typeof(int))
                            value.floatValue = Convert.ToSingle(EditorGUI.IntField(fieldRect, label, Convert.ToInt32(value.floatValue)));
                        else if (valueType == typeof(long))
                            value.floatValue = Convert.ToSingle(EditorGUI.LongField(fieldRect, label, Convert.ToInt64(value.floatValue)));
                    }
                    else
                    {
                        value = property.FindPropertyRelative("objectValue");
                        value.objectReferenceValue = EditorGUI.ObjectField(fieldRect, label, value.objectReferenceValue, valueType, true);
                    }
                    EditorGUIUtility.labelWidth = preLabelWidth;
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

