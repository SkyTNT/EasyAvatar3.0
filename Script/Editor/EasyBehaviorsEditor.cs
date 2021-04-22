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
            

            typeLabels = new string[] { Lang.BehaviorTypeProperty, Lang.BehaviorTypeAnim };
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
            behaviors.CopyFrom(copiedBehaviors);
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
            AnimationClip previewClip = Utility.GenerateAnimClip(behaviors.GetObject<List<EasyBehavior>>());
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
            SerializedProperty type = behavior.FindPropertyRelative("type");

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
            SerializedProperty targetPath = propertyGroup.FindPropertyRelative("targetPath");
            SerializedProperty properties = propertyGroup.FindPropertyRelative("properties");

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

            //目标物体缺失
            if (isMissing)
            {
                Rect missingRect = new Rect(targetFieldRect) { width = targetFieldRect.width - targetFieldRect.height - 2 };
                GUI.Box(missingRect, GUIContent.none, "Tag MenuItem");
                EditorGUI.LabelField(missingRect, Lang.Missing + ":" + targetPath.stringValue, MyGUIStyle.yellowLabel);
            }
            //当修改目标时
            if (newTarget != tempTarget)
            {
                if (!avatar)
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarNotSet, "ok");
                else
                    targetPath.stringValue = CalculateGameObjectPath(newTarget);

                //当前属性不在新目标中存在则删除属性
                if (properties.arraySize > 0 && !HasPropertyGroup(avatar, propertyGroup))
                {
                    Debug.Log("clear");
                    properties.ClearArray();
                }
                    
            }
            
            //属性选择
            EditorGUI.LabelField(propertyLabelRect, Lang.Property);
            EasyPropertySelector.PropertyField(propertyFieldRect, propertyGroup, avatar, tempTarget);
            //输入值
            EditorGUI.LabelField(valueLabelRect, Lang.SetTo);
            PropertyValueField(valueFieldRect, propertyGroup);
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
            SerializedProperty targetPath = propertyGroup.FindPropertyRelative("targetPath");
            SerializedProperty properties = propertyGroup.FindPropertyRelative("properties");
            int groupSize = properties.arraySize;
            if (groupSize == 0)
                return;

            SerializedProperty property0 = properties.GetArrayElementAtIndex(0);
            SerializedProperty targetProperty0 = property0.FindPropertyRelative("targetProperty");
            SerializedProperty targetPropertyType0 = property0.FindPropertyRelative("targetPropertyType");
            //特殊情况Blend Shape
            if (targetPropertyType0.stringValue.Contains("SkinnedMeshRenderer") && targetProperty0.stringValue.Contains("blendShape") && groupSize == 1)
            {
                SerializedProperty value = property0.FindPropertyRelative("floatValue");
                value.floatValue = EditorGUI.Slider(rect, value.floatValue, 0, 100);
            }
            //特殊情况Color
            else if (targetProperty0.stringValue.ToLower().Contains("color") && groupSize == 4)
            {
                bool isVec4 = false;
                SerializedProperty valueR, valueG, valueB, valueA;
                Dictionary<string, SerializedProperty> colorMap = new Dictionary<string, SerializedProperty>();
                for (int i = 0; i < groupSize; i++)
                {
                    SerializedProperty property = properties.GetArrayElementAtIndex(i);
                    string subName = GetPropertyGroupSubname(property);
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
                
                for (int i = 0; i < groupSize; i++)
                {

                    SerializedProperty property = properties.GetArrayElementAtIndex(i);
                    SerializedProperty propertyValueType = property.FindPropertyRelative("valueType");
                    SerializedProperty isPPtr = property.FindPropertyRelative("isPPtr");
                    SerializedProperty value = null;

                    Type valueType = EasyReflection.FindType(propertyValueType.stringValue);

                    Rect fieldRect = new Rect(rect)
                    {
                        x = rect.x + i * (rect.width / groupSize) + 3,
                        width = rect.width / groupSize - 6
                    };
                    GUIContent label = GUIContent.none;
                    //显示Vector之类的x,y,z,w或r,g,b,a
                    if (groupSize > 1)
                    {
                        label = new GUIContent(GetPropertyGroupSubname(properties.GetArrayElementAtIndex(i)).ToUpper());
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
        /// 判断物体是否具有该属性
        /// </summary>
        /// <param name="avatar">根物体</param>
        /// <param name="property">序列化的EasyProperty</param>
        /// <returns></returns>
        public static bool HasPropertyGroup(GameObject avatar, SerializedProperty propertyGroup)
        {
            SerializedProperty properties = propertyGroup.FindPropertyRelative("properties");
            if (!avatar || properties.arraySize == 0)
                return false;
            SerializedProperty property = properties.GetArrayElementAtIndex(0);
            if (property.FindPropertyRelative("valueType").stringValue == "" || property.FindPropertyRelative("targetProperty").stringValue == "")
                return false;
            return AnimationUtility.GetEditorCurveValueType(avatar, Utility.GetBinding(propertyGroup.FindPropertyRelative("targetPath").stringValue, property.GetObject<EasyProperty>())) != null;
        }

        /// <summary>
        /// 获取Property在PropertyGroup中的后缀名
        /// </summary>
        /// <param name="property">序列化的EasyProperty</param>
        /// <returns>后缀名</returns>
        public static string GetPropertyGroupSubname(SerializedProperty property)
        {
            string targetProperty = property.FindPropertyRelative("targetProperty").stringValue;
            return targetProperty.Substring(targetProperty.Length - 1);
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

        public static void GetMaskProperties(SerializedProperty mask, out SerializedProperty head, out SerializedProperty body, out SerializedProperty rightArm, out SerializedProperty leftArm, out SerializedProperty rightFingers, out SerializedProperty leftFingers, out SerializedProperty rightLeg, out SerializedProperty leftLeg, out SerializedProperty fx)
        {
            head = mask.FindPropertyRelative("head");
            body = mask.FindPropertyRelative("body");
            rightArm = mask.FindPropertyRelative("rightArm");
            leftArm = mask.FindPropertyRelative("leftArm");
            rightFingers = mask.FindPropertyRelative("rightFingers");
            leftFingers = mask.FindPropertyRelative("leftFingers");
            rightLeg = mask.FindPropertyRelative("rightLeg");
            leftLeg = mask.FindPropertyRelative("leftLeg");
            fx = mask.FindPropertyRelative("fx");
        }

        public static string GetMaskStr(SerializedProperty mask)
        {
            GetMaskProperties(mask, out SerializedProperty head, out SerializedProperty body, out SerializedProperty rightArm, out SerializedProperty leftArm, out SerializedProperty rightFingers, out SerializedProperty leftFingers, out SerializedProperty rightLeg, out SerializedProperty leftLeg, out SerializedProperty fx);

            string result = "";
            if (head.boolValue)
                result += Lang.AnimMaskHead + ",";
            if (body.boolValue)
                result += Lang.AnimMaskBody + ",";
            if (rightArm.boolValue)
                result += Lang.AnimMaskRightArm + ",";
            if (leftArm.boolValue)
                result += Lang.AnimMaskLeftArm + ",";
            if (rightFingers.boolValue)
                result += Lang.AnimMaskRightFingers + ",";
            if (leftFingers.boolValue)
                result += Lang.AnimMaskLeftFingers + ",";
            if (rightLeg.boolValue)
                result += Lang.AnimMaskRightLeg + ",";
            if (leftLeg.boolValue)
                result += Lang.AnimMaskLeftLeg + ",";
            if (fx.boolValue)
                result += Lang.AnimMaskFx;

            if (result.Length > 0 && result[result.Length - 1] == ',')
                result = result.Substring(0, result.Length - 1);

            return result;
        }

        public static void CalculateMask(AnimationClip clip, SerializedProperty mask)
        {
            GetMaskProperties(mask, out SerializedProperty head, out SerializedProperty body, out SerializedProperty rightArm, out SerializedProperty leftArm, out SerializedProperty rightFingers, out SerializedProperty leftFingers, out SerializedProperty rightLeg, out SerializedProperty leftLeg, out SerializedProperty fx);

            //重置
            head.boolValue = false;
            body.boolValue = false;
            rightArm.boolValue = false;
            leftArm.boolValue = false;
            rightFingers.boolValue = false;
            leftFingers.boolValue = false;
            rightLeg.boolValue = false;
            leftLeg.boolValue = false;
            fx.boolValue = false;

            if (!clip)
                return;

            //设置
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (binding.type == typeof(Animator) && binding.path == "")
                {
                    string name = binding.propertyName;
                    if (name.Contains("Head") || name.Contains("Neck") || name.Contains("Eye") || name.Contains("Jaw"))
                        head.boolValue = true;
                    else if (name.Contains("Root") || name.Contains("Spine") || name.Contains("Chest"))
                        body.boolValue = true;
                    else if (name.Contains("Left Hand") || name.Contains("Left Arm") || name.Contains("Left Forearm"))
                        leftArm.boolValue = true;
                    else if (name.Contains("LeftHand"))
                        leftFingers.boolValue = true;
                    else if (name.Contains("Right Hand") || name.Contains("Right Arm") || name.Contains("Right Forearm"))
                        rightArm.boolValue = true;
                    else if (name.Contains("RightHand"))
                        rightFingers.boolValue = true;
                    else if (name.Contains("Left Upper Leg") || name.Contains("Left Lower Leg") || name.Contains("Left Foot") || name.Contains("Left Toes"))
                        leftLeg.boolValue = true;
                    else if (name.Contains("Right Upper Leg") || name.Contains("Right Lower Leg") || name.Contains("Right Foot") || name.Contains("Right Toes"))
                        rightLeg.boolValue = true;
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

        Vector2 scroll = new Vector2();

        public void OnGUI()
        {
            mask.serializedObject.Update();
            GetMaskProperties(mask, out SerializedProperty head, out SerializedProperty body, out SerializedProperty rightArm, out SerializedProperty leftArm, out SerializedProperty rightFingers, out SerializedProperty leftFingers, out SerializedProperty rightLeg, out SerializedProperty leftLeg, out SerializedProperty fx);

            scroll = GUILayout.BeginScrollView(scroll);
            head.boolValue = EditorGUILayout.Toggle(Lang.AnimMaskHead, head.boolValue);
            body.boolValue = EditorGUILayout.Toggle(Lang.AnimMaskBody, body.boolValue);
            rightArm.boolValue = EditorGUILayout.Toggle(Lang.AnimMaskRightArm, rightArm.boolValue);
            leftArm.boolValue = EditorGUILayout.Toggle(Lang.AnimMaskLeftArm, leftArm.boolValue);
            rightFingers.boolValue = EditorGUILayout.Toggle(Lang.AnimMaskRightFingers, rightFingers.boolValue);
            leftFingers.boolValue = EditorGUILayout.Toggle(Lang.AnimMaskRightFingers, leftFingers.boolValue);
            rightLeg.boolValue = EditorGUILayout.Toggle(Lang.AnimMaskRightLeg, rightLeg.boolValue);
            leftLeg.boolValue = EditorGUILayout.Toggle(Lang.AnimMaskLeftLeg, leftLeg.boolValue);
            fx.boolValue = EditorGUILayout.Toggle(Lang.AnimMaskFx, fx.boolValue);
            GUILayout.EndScrollView();
            mask.serializedObject.ApplyModifiedProperties();
        }
    }
}

