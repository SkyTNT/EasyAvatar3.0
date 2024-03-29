﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace EasyAvatar
{
    public class EasyBehaviorGroupEditor
    {
        static Component copiedTarget;
        static string copiedBehaviorsPath;
        static EasyBehaviorGroupEditor previewing;
        GameObject avatar;
        AnimationClip currentClip;

        SerializedProperty behaviors, hide;
        ReorderableList behaviorsList;

        int[] typeIndex = { 0, 1, 2, 3, 4 };

        public EasyBehaviorGroupEditor(SerializedProperty behaviors)
        {
            hide = behaviors.FindPropertyRelative("hide");
            behaviors = behaviors.FindPropertyRelative("list");

            this.behaviors = behaviors;

            behaviorsList = new ReorderableList(behaviors.serializedObject, behaviors, true, true, true, true);
            behaviorsList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, Lang.Behavior);
            behaviorsList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => BehaviorField(rect, behaviors.GetArrayElementAtIndex(index));
            behaviorsList.elementHeightCallback = (int index) => {
                if (behaviors.arraySize > 0)
                    return (EditorGUIUtility.singleLineHeight + 6) * 4;
                else return 0;
            };
        }

        public void DoLayout(GameObject avatar)
        {
            this.avatar = avatar;
            Color preBg = GUI.backgroundColor;
            GUI.backgroundColor = previewing == this ? MyGUIStyle.activeButtonColor : preBg;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Lang.Preview))
            {
                if (previewing != this)
                    StartPreview();
                else
                    StopPreview();
            }
            GUI.backgroundColor = preBg;
            if (GUILayout.Button(Lang.Copy))
            {
                CopyBehaviors(behaviors);
            }
            if (GUILayout.Button(Lang.Paste))
            {
                PasteBehaviors(behaviors);
            }
            if (GUILayout.Button(Lang.Clear))
            {
                behaviors.ClearArray();
            }
            preBg = GUI.backgroundColor;
            GUI.backgroundColor = hide.boolValue ? MyGUIStyle.activeButtonColor : preBg;
            if (GUILayout.Button(Lang.Fold))
            {
                hide.boolValue = !hide.boolValue;
            }
            GUI.backgroundColor = preBg;
            EditorGUILayout.EndHorizontal();
            if (!hide.boolValue)
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
            if (!avatar)
                return;
            Animator animator = avatar.GetComponent<Animator>();
            if (!animator)
            {
                animator = avatar.AddComponent<Animator>();
            }
            //没有Controller会崩溃
            if (!animator.runtimeAnimatorController)
            {
                if (!Directory.Exists(EasyAvatarBuilder.workingDirectory + "Build/"))
                    Directory.CreateDirectory(EasyAvatarBuilder.workingDirectory + "Build/");
                AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(EasyAvatarBuilder.workingDirectory + "Build/preview.controller");
                animator.runtimeAnimatorController = controller;
            }
            currentClip = Utility.GenerateCurrentAnimClip(avatar);
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
            Animator animator = avatar.GetComponent<Animator>();
            if (animator)
            {
                animator.runtimeAnimatorController = null;
            }
            
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
        }


        float lastPreviewTime = 0;
        /// <summary>
        /// 预览
        /// </summary>
        public void Preview()
        {
            if (previewing != this || !AnimationMode.InAnimationMode() || !avatar || Time.realtimeSinceStartup - lastPreviewTime < 0.1)
                return;
            lastPreviewTime = Time.realtimeSinceStartup;
            AnimationMode.BeginSampling();
            AnimationClip previewClip = Utility.MergeAnimClip(currentClip, Utility.GenerateAnimClip(avatar, behaviors.GetObject<List<EasyBehavior>>(), true));
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
            type.enumValueIndex = EditorGUI.IntPopup(typeFieldRect, "", type.enumValueIndex, new string[] { Lang.BehaviorTypeProperty, Lang.BehaviorTypeAnim ,Lang.ToggleMusic,Lang.MusicVolume,Lang.ToggleObject}, typeIndex);
            

            switch ((EasyBehavior.Type)type.enumValueIndex)
            {
                case EasyBehavior.Type.Property:
                    PropertyTypeBehaviorLayout(layoutRect, behavior);
                    break;
                case EasyBehavior.Type.AnimationClip:
                    AnimTypeBehaviorLayout(layoutRect, behavior);
                    break;
                case EasyBehavior.Type.ToggleMusic:
                    ToggleMusicTypeBehaviorLayout(layoutRect, behavior);
                    break;
                case EasyBehavior.Type.MusicVolume:
                    MusicVolumeTypeBehaviorLayout(layoutRect, behavior);
                    break;
                case EasyBehavior.Type.ToggleObject:
                    ToggleObjectTypeBehaviorLayout(layoutRect, behavior);
                    break;
            }
        }

        public void PropertyTypeBehaviorLayout(Rect position, SerializedProperty behavior)
        {
            SerializedProperty propertyGroup = behavior.FindPropertyRelative("propertyGroup");
            SerializedProperty targetPath = propertyGroup.FindPropertyRelative("targetPath");
            SerializedProperty properties = propertyGroup.FindPropertyRelative("properties");

            
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
                x = valueLabelRect.x + valueLabelRect.width,
                y = position.y + (position.height + 6) * 2,
                width = position.width - valueLabelRect.width
            };

            //当修改目标时
            if (TargetObjectField(targetLabelRect, targetFieldRect, propertyGroup, out GameObject tempTarget))
            {


                //当前属性不在新目标中存在则删除属性
                if (properties.arraySize > 0 && !HasPropertyGroup(avatar, propertyGroup))
                {
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


        public bool TargetObjectField(Rect targetLabelRect, Rect targetFieldRect, SerializedProperty propertyGroup, out GameObject tempTargetObject)
        {
            SerializedProperty targetPath = propertyGroup.FindPropertyRelative("targetPath");
            SerializedProperty tempTarget = propertyGroup.FindPropertyRelative("tempTarget");

            GameObject newTarget = null;
            tempTargetObject = (GameObject)tempTarget.objectReferenceValue;//首先从tempTarget获取

            if (!tempTargetObject)//tempTarget获取不到就从targetPath获取
            {
                if (avatar && targetPath.stringValue != "")
                {
                    Transform tempTransform = avatar.transform.Find(targetPath.stringValue);
                    if (tempTransform)
                    {
                        tempTargetObject = tempTransform.gameObject;
                        tempTarget.objectReferenceValue = tempTargetObject;
                    }
                        
                }
            }
            //目标物体
            EditorGUI.LabelField(targetLabelRect, Lang.Target);
            newTarget = (GameObject)EditorGUI.ObjectField(targetFieldRect, tempTargetObject, typeof(GameObject), true);

            //目标物体缺失
            if (!avatar||((!tempTargetObject|| !tempTargetObject.transform.IsChildOf(avatar.transform))&&targetPath.stringValue!=""))
            {
                Rect missingRect = new Rect(targetFieldRect) { width = targetFieldRect.width - targetFieldRect.height - 2 };
                GUI.Box(missingRect, GUIContent.none, "Tag MenuItem");
                EditorGUI.LabelField(missingRect, Lang.Missing + ":" + targetPath.stringValue, MyGUIStyle.yellowLabel);
            }

            if (newTarget != tempTargetObject)
            {
                if (!avatar)
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarNotSet, "ok");
                else
                {
                    if (!newTarget|| newTarget.transform.IsChildOf(avatar.transform))//None或者avatar内的物体
                    {
                        tempTarget.objectReferenceValue = newTarget;
                    }
                    else//avatar外的物体
                    {
                        newTarget = tempTargetObject;//取消修改
                    }
                }
            }

            if (!newTarget)
            {
                if (tempTargetObject)
                    targetPath.stringValue = "";//只有原来不是空物体才置空（把目标设置为none时）
            }
            else if(avatar&&newTarget.transform.IsChildOf(avatar.transform))
            {
                targetPath.stringValue = CalculateGameObjectPath(newTarget);

            }

            return newTarget != tempTargetObject;
        }

        public void AnimTypeBehaviorLayout(Rect position, SerializedProperty behavior)
        {
            SerializedProperty anim = behavior.FindPropertyRelative("anim");
            SerializedProperty mask = behavior.FindPropertyRelative("mask");
            SerializedProperty mirror = behavior.FindPropertyRelative("mirror");

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
            Rect mirrorLabelRect = new Rect(position)
            {
                y = position.y + (position.height + 6) * 2,
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect mirrorFieldRect = new Rect(position)
            {
                x = mirrorLabelRect.x + mirrorLabelRect.width,
                y = position.y + (position.height + 6) * 2,
                width = position.width - mirrorLabelRect.width
            };
            GUI.Label(animLabelRect, Lang.AnimClip);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(animFieldRect, anim, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
                EasyAnimationMaskWindow.CalculateMask((AnimationClip)anim.objectReferenceValue, mask);

            GUI.Label(maskLabelRect, Lang.AnimMask);
            EasyAnimationMaskWindow.AnimationMaskField(maskFieldRect, mask);
            //暂未完善
            //GUI.Label(mirrorLabelRect, Lang.Mirror);
            //EditorGUI.PropertyField(mirrorFieldRect, mirror, GUIContent.none);
        }

        public void ToggleMusicTypeBehaviorLayout(Rect position, SerializedProperty behavior)
        {
            SerializedProperty audio = behavior.FindPropertyRelative("audio");
            SerializedProperty isActive = behavior.FindPropertyRelative("isActive");

            position.y += 3;
            position.height = EditorGUIUtility.singleLineHeight;


            Rect audioLabelRect = new Rect(position)
            {
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect audioFieldRect = new Rect(position)
            {
                x = audioLabelRect.x + audioLabelRect.width,
                width = position.width - audioLabelRect.width

            };
            Rect toggleLabel = new Rect(position)
            {
                y = position.y + position.height + 6,
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect toggleFieldRect = new Rect(position)
            {
                x = toggleLabel.x + toggleLabel.width,
                y = position.y + position.height + 6,
                width = position.width - toggleLabel.width
            };
            GUI.Label(audioLabelRect, Lang.Music);
            EditorGUI.PropertyField(audioFieldRect, audio, GUIContent.none);
            GUI.Label(toggleLabel, Lang.Toggle);
            EditorGUI.PropertyField(toggleFieldRect, isActive, GUIContent.none);

        }

        public void MusicVolumeTypeBehaviorLayout(Rect position, SerializedProperty behavior)
        {
            SerializedProperty audio = behavior.FindPropertyRelative("audio");
            SerializedProperty volume = behavior.FindPropertyRelative("volume");

            position.y += 3;
            position.height = EditorGUIUtility.singleLineHeight;


            Rect audioLabelRect = new Rect(position)
            {
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect audioFieldRect = new Rect(position)
            {
                x = audioLabelRect.x + audioLabelRect.width,
                width = position.width - audioLabelRect.width

            };
            Rect volumeLabel = new Rect(position)
            {
                y = position.y + position.height + 6,
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect volumeFieldRect = new Rect(position)
            {
                x = volumeLabel.x + volumeLabel.width,
                y = position.y + position.height + 6,
                width = position.width - volumeLabel.width
            };
            GUI.Label(audioLabelRect, Lang.Music);
            EditorGUI.PropertyField(audioFieldRect, audio, GUIContent.none);
            GUI.Label(volumeLabel, Lang.Volume);
            volume.floatValue = EditorGUI.Slider(volumeFieldRect, volume.floatValue, 0, 1);

        }

        public void ToggleObjectTypeBehaviorLayout(Rect position, SerializedProperty behavior)
        {
            SerializedProperty propertyGroup = behavior.FindPropertyRelative("propertyGroup");
            SerializedProperty isActive = behavior.FindPropertyRelative("isActive");

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
            Rect toggleLabel = new Rect(position)
            {
                y = position.y + position.height + 6,
                width = Mathf.Max(position.width / 4, 50)
            };
            Rect toggleFieldRect = new Rect(position)
            {
                x = toggleLabel.x + toggleLabel.width,
                y = position.y + position.height + 6,
                width = position.width - toggleLabel.width
            };

            TargetObjectField(targetLabelRect, targetFieldRect, propertyGroup, out GameObject tempTarget);
            GUI.Label(toggleLabel, Lang.Toggle);
            EditorGUI.PropertyField(toggleFieldRect, isActive, GUIContent.none);

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
                Color newColor = EditorGUI.ColorField(rect, GUIContent.none, tempColor, true, true, true);//允许HDR
                
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