using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace EasyAvatar
{

    public class EasyBehaviorAndAnimEditor
    {
        static EasyControl copiedTarget;
        static string copiedBehaviorsPath;
        static List<AnimationClip> copidClips;
        public GameObject avatar;

        SerializedProperty behaviors, animClips;
        ReorderableList behaviorsList, animClipList;
        public bool useAnimClip, previewing;
        private bool m_previewStarted;

        /// <summary>
        /// 是否刚刚开始预览
        /// </summary>
        public bool previewStarted { get
            {
                bool result = m_previewStarted;
                m_previewStarted = false;
                return result;
            }
        }

        public EasyBehaviorAndAnimEditor(SerializedProperty behaviors, SerializedProperty animClips)
        {
            this.behaviors = behaviors;
            this.animClips = animClips;

            behaviorsList = new ReorderableList(behaviors.serializedObject, behaviors, true, true, true, true);
            animClipList = new ReorderableList(animClips.serializedObject, animClips, true, true, true, true);
            behaviorsList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect,Lang.BehaviorListLabel);
            behaviorsList.elementHeight = (EditorGUIUtility.singleLineHeight + 6) * 3;
            behaviorsList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => BehaviorField(rect, behaviors.GetArrayElementAtIndex(index));
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

            animClipList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, Lang.AnimClipListLabel);
            animClipList.elementHeight = EditorGUIUtility.singleLineHeight + 6;
            animClipList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => AnimField(rect, animClips.GetArrayElementAtIndex(index));

        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        public void LayoutGUI()
        {
            //行为
            Color preBg = GUI.backgroundColor;
            GUI.backgroundColor = previewing ? MyGUIStyle.activeButtonColor : preBg;
            if (GUILayout.Button(Lang.Preview))
            {
                if (!previewing)
                    StartPreview();
                else
                    StopPreview();
            }
            GUI.backgroundColor = preBg;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Lang.Copy))
            {
                CopyBehaviors(behaviors);
            }
            if (GUILayout.Button(Lang.Paste))
            {
                PasteBehaviors(behaviors);
            }
            EditorGUILayout.EndHorizontal();
            behaviorsList.DoLayoutList();
            
            if (useAnimClip)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(Lang.Copy))
                {
                    CopyAnimClips(animClips);
                }
                if (GUILayout.Button(Lang.Paste))
                {
                    PasteAnimClips(animClips);
                }
                EditorGUILayout.EndHorizontal();
                animClipList.DoLayoutList();
                
            }

            Preview();
        }

        /// <summary>
        /// 复制Behaviors
        /// </summary>
        /// <param name="behaviors"></param>
        public static void CopyBehaviors(SerializedProperty behaviors)
        {
            copiedTarget = (EasyControl)behaviors.serializedObject.targetObject;
            copiedBehaviorsPath = behaviors.propertyPath;
        }

        /// <summary>
        /// 粘贴Behaviors
        /// </summary>
        /// <param name="behaviors"></param>
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

        /// <summary>
        /// 复制AnimationClips
        /// </summary>
        /// <param name="animClips"></param>
        public static void CopyAnimClips(SerializedProperty animClips)
        {
            copidClips = new List<AnimationClip>();
            for (int i = 0; i < animClips.arraySize; i++)
                copidClips.Add((AnimationClip)animClips.GetArrayElementAtIndex(i).objectReferenceValue);

        }

        /// <summary>
        /// 粘贴AnimationClips
        /// </summary>
        /// <param name="animClips"></param>
        public static void PasteAnimClips(SerializedProperty animClips)
        {
            if (copidClips == null)
                return;
            animClips.arraySize = copidClips.Count;
            for (int i = 0; i < animClips.arraySize; i++)
                animClips.GetArrayElementAtIndex(i).objectReferenceValue = copidClips[i];
        }


        /// <summary>
        /// 准备预览
        /// </summary>
        public void StartPreview()
        {
            Animator animator = avatar.GetComponent<Animator>();
            if (!animator)
            {
                animator = avatar.AddComponent<Animator>();
            }
            //没有Controller会崩溃
            if (!animator.runtimeAnimatorController)
            {
                if (!Directory.Exists(EasyAvatarTool.workingDirectory + "Build/"))
                    Directory.CreateDirectory(EasyAvatarTool.workingDirectory + "Build/");
                AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(EasyAvatarTool.workingDirectory + "Build/preview.controller");
                animator.runtimeAnimatorController = controller;
            }
            previewing = true;
            m_previewStarted = true;
            if (!AnimationMode.InAnimationMode())
                AnimationMode.StartAnimationMode();
        }

        /// <summary>
        /// 停止预览
        /// </summary>
        public void StopPreview()
        {
            previewing = false;
            m_previewStarted = false;
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
        }

        /// <summary>
        /// 预览
        /// </summary>
        public void Preview()
        {
            if (!previewing || !AnimationMode.InAnimationMode() || !avatar)
                return;
            AnimationMode.BeginSampling();
            AnimationClip previewClip = EasyAvatarTool.Utility.GenerateAnimClip(behaviors);
            if (useAnimClip)
                previewClip = EasyAvatarTool.Utility.MergeAnimClip(EasyAvatarTool.Utility.MergeAnimClip(animClips), previewClip);
            AnimationMode.SampleAnimationClip(avatar, previewClip, 0);
            AnimationMode.EndSampling();
        }


        /// <summary>
        /// Behavior界面
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="behavior">Behavior</param>
        public void BehaviorField(Rect position, SerializedProperty behavior)
        {
            SerializedProperty propertyGroup = behavior.FindPropertyRelative("propertyGroup");
            SerializedProperty property = propertyGroup.GetArrayElementAtIndex(0);
            SerializedProperty targetPath = property.FindPropertyRelative("targetPath");
            SerializedProperty targetProperty = property.FindPropertyRelative("targetProperty");
            SerializedProperty targetPropertyType = property.FindPropertyRelative("targetPropertyType");

            GameObject tempTarget = null, newTarget = null;
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
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect targetFieldRect = new Rect(position)
            {
                x = targetLabelRect.x + targetLabelRect.width,
                width = position.width - targetLabelRect.width

            };
            Rect propertyLabelRect = new Rect(position)
            {
                y = position.y + position.height + 6,
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect propertyFieldRect = new Rect(position)
            {
                x = propertyLabelRect.x + propertyLabelRect.width,
                y = position.y + position.height + 6,
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
            newTarget = (GameObject)EditorGUI.ObjectField(targetFieldRect, tempTarget, typeof(GameObject), true);

            if (isMissing)
            {
                Rect missingRect = new Rect(targetFieldRect) { width = targetFieldRect.width - targetFieldRect.height - 2 };
                GUI.Box(missingRect, GUIContent.none, "Tag MenuItem");
                EditorGUI.LabelField(missingRect, Lang.Missing + ":" + targetPath.stringValue, MyGUIStyle.yellowLabel);
            }
            if (newTarget != tempTarget && !avatar)
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


        /// <summary>
        /// AnimationClip输入框
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="clip">AnimationClip</param>
        public void AnimField(Rect position, SerializedProperty clip)
        {
            position.y += 3;
            position.height = EditorGUIUtility.singleLineHeight;
            Rect labelRect = new Rect(position)
            {
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect fieldRect = new Rect(position)
            {
                x = labelRect.x + labelRect.width,
                width = position.width - labelRect.width
            };
            EditorGUI.LabelField(labelRect, Lang.AnimClip);
            EditorGUI.PropertyField(fieldRect, clip, GUIContent.none);
        }

        /// <summary>
        /// 绘制PropertyGroup的值输入框
        /// </summary>
        /// <param name="rect">位置</param>
        /// <param name="propertyGroup">PropertyGroup</param>
        public void PropertyValueField(Rect rect, SerializedProperty propertyGroup)
        {

            if (EasyAvatarTool.Utility.PropertyGroupIsBlendShape(propertyGroup))//特殊情况Blend Shape
            {
                SerializedProperty value = propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("floatValue");
                value.floatValue = EditorGUI.Slider(rect, value.floatValue, 0, 100);
            }
            else if (EasyAvatarTool.Utility.PropertyGroupIsColor(propertyGroup))//特殊情况Color
            {
                bool isVec4 = false;
                SerializedProperty valueR, valueG, valueB, valueA;
                Dictionary<string, SerializedProperty> colorMap = new Dictionary<string, SerializedProperty>();
                for (int i = 0; i < propertyGroup.arraySize; i++)
                {
                    SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i);
                    string subName = EasyAvatarTool.Utility.GetPropertyGroupSubname(property);
                    colorMap.Add(subName, property.FindPropertyRelative("floatValue"));
                    if (subName == "w")
                        isVec4 = true;
                }

                if (isVec4)
                {
                    valueR = colorMap["x"];
                    valueG = colorMap["y"];
                    valueB = colorMap["z"];
                    valueA = colorMap["w"];
                }
                else
                {
                    valueR = colorMap["r"];
                    valueG = colorMap["g"];
                    valueB = colorMap["b"];
                    valueA = colorMap["a"];
                }

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

        /// <summary>
        /// 计算相对于avatar的路径
        /// </summary>
        /// <param name="gameObject">物体</param>
        /// <returns>相对路径</returns>
        public string CalculateGameObjectPath(GameObject gameObject)
        {
            if (!gameObject)
                return "";
            return gameObject.transform.GetHierarchyPath(avatar.transform);
        }
    }
}

