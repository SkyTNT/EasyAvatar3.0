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
    public class EasyBehaviorsEditor
    {
        static Component copiedTarget;
        static string copiedBehaviorsPath;
        static EasyBehaviorsEditor previewing;
        public GameObject avatar;

        SerializedProperty behaviors;
        ReorderableList behaviorsList;

        int[] typeIndex = { 0, 1 };
        string[] typeLabels;

        public EasyBehaviorsEditor(SerializedProperty behaviors)
        {
            behaviors = behaviors.FindPropertyRelative("list");
            this.behaviors = behaviors;

            behaviorsList = new ReorderableList(behaviors.serializedObject, behaviors, true, true, true, true);
            behaviorsList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, Lang.Behavior);
            behaviorsList.elementHeight = (EditorGUIUtility.singleLineHeight + 6) * 4;
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

            typeLabels = new string[] {Lang.BehaviorTypeProperty,Lang.BehaviorTypeAnim };
        }

        public void DoLayout()
        {
            Color preBg = GUI.backgroundColor;
            GUI.backgroundColor = previewing == this ? MyGUIStyle.activeButtonColor : preBg;
            if (GUILayout.Button(Lang.Preview))
            {
                if (previewing != this)
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
            Preview();
        }

        /// <summary>
        /// 复制Behaviors
        /// </summary>
        /// <param name="behaviors"></param>
        public static void CopyBehaviors(SerializedProperty behaviors)
        {
            copiedTarget = (Component)behaviors.serializedObject.targetObject;
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
                Utility.CopyBehavior(behaviors.GetArrayElementAtIndex(i), copiedBehaviors.GetArrayElementAtIndex(i));
            }
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
            previewing = this;
            if (!AnimationMode.InAnimationMode())
                AnimationMode.StartAnimationMode();
        }

        /// <summary>
        /// 停止预览
        /// </summary>
        public void StopPreview()
        {
            previewing = null;
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
        }

        /// <summary>
        /// 预览
        /// </summary>
        public void Preview()
        {
            if (previewing != this || !AnimationMode.InAnimationMode() || !avatar)
                return;
            AnimationMode.BeginSampling();
            AnimationClip previewClip = Utility.GenerateAnimClip(behaviors);
            //if (useAnimClip)
             //   previewClip = Utility.MergeAnimClip(Utility.MergeAnimClip(animClips), previewClip);
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
            SerializedProperty type= behavior.FindPropertyRelative("type");

            position.y += 3;
            position.height = EditorGUIUtility.singleLineHeight;
            Rect typeLabelRect = new Rect(position)
            {
                width = Mathf.Max(position.width / 4, 50)
            };

            Rect typeFieldRect = new Rect(position)
            {
                x = typeLabelRect.x + typeLabelRect.width,
                width = position.width - typeLabelRect.width

            };
            Rect layoutRect = new Rect(position)
            {
                y = position.y + typeLabelRect.height + 3,
            };

            GUI.Label(typeLabelRect, Lang.BehaviorType);
            type.enumValueIndex = EditorGUI.IntPopup(typeFieldRect, "", type.enumValueIndex, typeLabels, typeIndex);

            if (type.enumValueIndex == (int)EasyBehavior.Type.Property)
                PropertyTypeBehaviorLayout(layoutRect, behavior);
            else if (type.enumValueIndex == (int)EasyBehavior.Type.AnimationClip)
                AnimTypeBehaviorLayout(layoutRect, behavior);
        }

        public void PropertyTypeBehaviorLayout(Rect position, SerializedProperty behavior)
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
                    Utility.PropertyGroupEdit(propertyGroup, "targetPath", CalculateGameObjectPath(newTarget));
                //检查目标是否具有当前属性
                if (!Utility.CheckProperty(avatar, property))
                    Utility.ClearPropertyGroup(propertyGroup);
            }
            //属性选择
            EditorGUI.LabelField(propertyLabelRect, Lang.Property);
            EasyPropertySelector.PropertyField(propertyFieldRect, propertyGroup, avatar, tempTarget);
            EditorGUI.LabelField(valueLabelRect, Lang.SetTo);


            //输入值
            if (property.FindPropertyRelative("valueType").stringValue != "")
                PropertyValueField(valueFieldRect, behavior.FindPropertyRelative("propertyGroup"));
        }


        public void AnimTypeBehaviorLayout(Rect position, SerializedProperty behavior)
        {
            SerializedProperty anim = behavior.FindPropertyRelative("anim");
            SerializedProperty mask = behavior.FindPropertyRelative("mask");

            position.y += 3;
            position.height = EditorGUIUtility.singleLineHeight;


            Rect animLabelRect = new Rect(position)
            {
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect animFieldRect = new Rect(position)
            {
                x = animLabelRect.x + animLabelRect.width,
                width = position.width - animLabelRect.width

            };
            Rect maskLabelRect = new Rect(position)
            {
                y = position.y + position.height + 6,
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect maskFieldRect = new Rect(position)
            {
                x = maskLabelRect.x + maskLabelRect.width,
                y = position.y + position.height + 6,
                width = position.width - maskLabelRect.width
            };
            GUI.Label(animLabelRect, Lang.AnimClip);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(animFieldRect, anim, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
                EasyAnimationMaskWindow.CalculateMask((AnimationClip)anim.objectReferenceValue, mask);

            GUI.Label(maskLabelRect, Lang.AnimMask);
            EasyAnimationMaskWindow.AnimationMaskField(maskFieldRect, mask);
        }

        /// <summary>
        /// 绘制PropertyGroup的值输入框
        /// </summary>
        /// <param name="rect">位置</param>
        /// <param name="propertyGroup">PropertyGroup</param>
        public void PropertyValueField(Rect rect, SerializedProperty propertyGroup)
        {

            if (Utility.PropertyGroupIsBlendShape(propertyGroup))//特殊情况Blend Shape
            {
                SerializedProperty value = propertyGroup.GetArrayElementAtIndex(0).FindPropertyRelative("floatValue");
                value.floatValue = EditorGUI.Slider(rect, value.floatValue, 0, 100);
            }
            else if (Utility.PropertyGroupIsColor(propertyGroup))//特殊情况Color
            {
                bool isVec4 = false;
                SerializedProperty valueR, valueG, valueB, valueA;
                Dictionary<string, SerializedProperty> colorMap = new Dictionary<string, SerializedProperty>();
                for (int i = 0; i < propertyGroup.arraySize; i++)
                {
                    SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i);
                    string subName = Utility.GetPropertyGroupSubname(property);
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
                        label = new GUIContent(Utility.GetPropertyGroupSubname(propertyGroup, i).ToUpper());
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

    class EasyAnimationMaskWindow : EditorWindow
    {
        SerializedProperty mask;
        public static void AnimationMaskField(Rect rect, SerializedProperty animationMask)
        {
            if (GUI.Button(rect, GetMaskStr(animationMask), EditorStyles.objectField))
            {
                Vector2 vector2 = EditorGUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
                Rect screenFixedRect = new Rect(rect)
                {
                    x = vector2.x,
                    y = vector2.y
                };
                EasyAnimationMaskWindow editorWindow = CreateInstance<EasyAnimationMaskWindow>();
                editorWindow.Init(animationMask);
                editorWindow.ShowAsDropDown(screenFixedRect, new Vector2(rect.width, EditorGUIUtility.singleLineHeight * 14));
            }

        }

        public static void GetMaskProperties(SerializedProperty mask, out SerializedProperty head, out SerializedProperty mouth, out SerializedProperty eyes, out SerializedProperty hip, out SerializedProperty rightHand, out SerializedProperty leftHand, out SerializedProperty rightFingers, out SerializedProperty leftFingers, out SerializedProperty rightFoot, out SerializedProperty leftFoot, out SerializedProperty fx)
        {
            head = mask.FindPropertyRelative("head");
            mouth = mask.FindPropertyRelative("mouth");
            eyes = mask.FindPropertyRelative("eyes");
            hip = mask.FindPropertyRelative("hip");
            rightHand = mask.FindPropertyRelative("rightHand");
            leftHand = mask.FindPropertyRelative("leftHand");
            rightFingers = mask.FindPropertyRelative("rightFingers");
            leftFingers = mask.FindPropertyRelative("leftFingers");
            rightFoot = mask.FindPropertyRelative("rightFoot");
            leftFoot = mask.FindPropertyRelative("leftFoot");
            fx = mask.FindPropertyRelative("fx");
        }

        public static string GetMaskStr(SerializedProperty mask)
        {
            GetMaskProperties(mask, out SerializedProperty head, out SerializedProperty mouth, out SerializedProperty eyes, out SerializedProperty hip, out SerializedProperty rightHand, out SerializedProperty leftHand, out SerializedProperty rightFingers, out SerializedProperty leftFingers, out SerializedProperty rightFoot, out SerializedProperty leftFoot, out SerializedProperty fx);

            string result = "";
            if (head.boolValue)
                result += Lang.BodyPartHead + ",";
            if (mouth.boolValue)
                result += Lang.BodyPartMouth + ",";
            if (eyes.boolValue)
                result += Lang.BodyPartEyes + ",";
            if (hip.boolValue)
                result += Lang.BodyPartHip + ",";
            if (rightHand.boolValue)
                result += Lang.BodyPartRightHand + ",";
            if (leftHand.boolValue)
                result += Lang.BodyPartLeftHand + ",";
            if (rightFingers.boolValue)
                result += Lang.BodyPartRightFingers + ",";
            if (leftFingers.boolValue)
                result += Lang.BodyPartLeftFingers + ",";
            if (rightFoot.boolValue)
                result += Lang.BodyPartRightFoot + ",";
            if (leftFoot.boolValue)
                result += Lang.BodyPartLeftFoot + ",";
            if (fx.boolValue)
                result += Lang.BodyPartFx;

            if (result.Length > 0 && result[result.Length - 1] == ',')
                result = result.Substring(0, result.Length - 1);

            return result;
        }

        public static void CalculateMask(AnimationClip clip, SerializedProperty mask)
        {
            GetMaskProperties(mask, out SerializedProperty head, out SerializedProperty mouth, out SerializedProperty eyes, out SerializedProperty hip, out SerializedProperty rightHand, out SerializedProperty leftHand, out SerializedProperty rightFingers, out SerializedProperty leftFingers, out SerializedProperty rightFoot, out SerializedProperty leftFoot, out SerializedProperty fx);

            //重置
            head.boolValue = false;
            mouth.boolValue = false;
            eyes.boolValue = false;
            hip.boolValue = false;
            rightHand.boolValue = false;
            leftHand.boolValue = false;
            rightFingers.boolValue = false;
            leftFingers.boolValue = false;
            rightFoot.boolValue = false;
            leftFoot.boolValue = false;
            fx.boolValue = false;

            if (!clip)
                return;

            //设置
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (binding.type == typeof(Animator) && binding.path == "")
                {
                    string name = binding.propertyName;
                    if (name.Contains("Head"))
                        head.boolValue = true;
                    else if (name.Contains("Root"))
                        hip.boolValue = true;
                    else if (name.Contains("Eye"))
                        eyes.boolValue = true;
                    else if (name.Contains("Jaw"))
                        mouth.boolValue = true;
                    else if (name.Contains("Left Hand"))
                        leftHand.boolValue = true;
                    else if (name.Contains("LeftHand"))
                        leftFingers.boolValue = true;
                    else if (name.Contains("Right Hand"))
                        rightHand.boolValue = true;
                    else if (name.Contains("RightHand"))
                        rightFingers.boolValue = true;
                    else if (name.Contains("Left Foot"))
                        leftFoot.boolValue = true;
                    else if (name.Contains("Right Foot"))
                        rightFoot.boolValue = true;
                }
                else
                    fx.boolValue = true;
            }
            if (AnimationUtility.GetObjectReferenceCurveBindings(clip).Length > 0)
                fx.boolValue = true;
        }

        public void Init(SerializedProperty animationMask)
        {
            mask = animationMask;
        }

        Vector2 scroll =new Vector2();

        public void OnGUI()
        {
            mask.serializedObject.Update();
            GetMaskProperties(mask, out SerializedProperty head, out SerializedProperty mouth, out SerializedProperty eyes, out SerializedProperty hip, out SerializedProperty rightHand, out SerializedProperty leftHand, out SerializedProperty rightFingers, out SerializedProperty leftFingers, out SerializedProperty rightFoot, out SerializedProperty leftFoot, out SerializedProperty fx);

            scroll = GUILayout.BeginScrollView(scroll);
            head.boolValue = EditorGUILayout.Toggle(Lang.BodyPartHead, head.boolValue);
            mouth.boolValue = EditorGUILayout.Toggle(Lang.BodyPartMouth, mouth.boolValue);
            eyes.boolValue = EditorGUILayout.Toggle(Lang.BodyPartEyes, eyes.boolValue);
            hip.boolValue = EditorGUILayout.Toggle(Lang.BodyPartHip, hip.boolValue);
            rightHand.boolValue = EditorGUILayout.Toggle(Lang.BodyPartRightHand, rightHand.boolValue);
            leftHand.boolValue = EditorGUILayout.Toggle(Lang.BodyPartLeftHand, leftHand.boolValue);
            rightFingers.boolValue = EditorGUILayout.Toggle(Lang.BodyPartRightFingers, rightFingers.boolValue);
            leftFingers.boolValue = EditorGUILayout.Toggle(Lang.BodyPartLeftFingers, leftFingers.boolValue);
            rightFoot.boolValue = EditorGUILayout.Toggle(Lang.BodyPartRightFoot, rightFoot.boolValue);
            leftFoot.boolValue = EditorGUILayout.Toggle(Lang.BodyPartLeftFoot, leftFoot.boolValue);
            fx.boolValue = EditorGUILayout.Toggle(Lang.BodyPartFx, fx.boolValue);
            GUILayout.EndScrollView();
            mask.serializedObject.ApplyModifiedProperties();
        }
    }
}

