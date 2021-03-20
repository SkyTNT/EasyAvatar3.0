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

        public enum PreviewType
        {
            None,
            Behaviors,
            AnimationClips
        }
        //当前复制的Behaviors
        static EasyControl copiedTarget;
        static string copiedBehaviorsPath;
        static List<AnimationClip> copidClips;
        AnimationClip testClip;

        SerializedProperty icon, offBehaviors, onBehaviors, useAnimClip, offAnims, onAnims, previewing;
        PreviewType previewType;
        GameObject avatar;
        ReorderableList offBehaviorsList, onBehaviorsList, offAnimsList, onAnimsList;
        SkinnedMeshRenderer testObject;

        private void OnEnable()
        {
            
            serializedObject.Update();
            icon = serializedObject.FindProperty("icon");
            offBehaviors = serializedObject.FindProperty("offBehaviors");
            onBehaviors = serializedObject.FindProperty("onBehaviors");
            useAnimClip = serializedObject.FindProperty("useAnimClip");
            offAnims = serializedObject.FindProperty("offAnims");
            onAnims = serializedObject.FindProperty("onAnims");

            offBehaviorsList = new ReorderableList(serializedObject, offBehaviors, true, true, true, true);
            onBehaviorsList  = new ReorderableList(serializedObject, onBehaviors, true, true, true, true);
            offBehaviorsList.drawHeaderCallback = onBehaviorsList.drawHeaderCallback = (Rect rect) => { };
            offBehaviorsList.elementHeight = onBehaviorsList.elementHeight = (EditorGUIUtility.singleLineHeight + 6) * 3;
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

            offAnimsList = new ReorderableList(serializedObject, offAnims, true, true, true, true);
            onAnimsList = new ReorderableList(serializedObject, onAnims, true, true, true, true);
            offAnimsList.drawHeaderCallback = onAnimsList.drawHeaderCallback = (Rect rect) => { };
            offAnimsList.elementHeight = onAnimsList.elementHeight = EditorGUIUtility.singleLineHeight + 6;
            offAnimsList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => DrawAnimClip(rect,offAnims.GetArrayElementAtIndex(index));
            onAnimsList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => DrawAnimClip(rect, onAnims.GetArrayElementAtIndex(index));
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
            GUI.backgroundColor = previewing == offBehaviors ? MyGUIStyle.activeButtonColor : preBg;
            if (GUILayout.Button(Lang.Preview))
            {
                if (previewing != offBehaviors)
                    StartPreview(offBehaviors, PreviewType.Behaviors);
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
            GUI.backgroundColor = previewing == onBehaviors ? MyGUIStyle.activeButtonColor : preBg;
            if (GUILayout.Button(Lang.Preview))
            {
                if (previewing != onBehaviors)
                    StartPreview(onBehaviors, PreviewType.Behaviors);
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
            
            useAnimClip.boolValue = EditorGUILayout.ToggleLeft(Lang.UseAnimClip, useAnimClip.boolValue);

            if (useAnimClip.boolValue)
            {
                //关闭动画
                GUILayout.Label(Lang.AnimClipOff, EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = previewing == offAnims ? MyGUIStyle.activeButtonColor : preBg;
                if (GUILayout.Button(Lang.Preview))
                {
                    if (previewing != offAnims)
                        StartPreview(offAnims, PreviewType.AnimationClips);
                    else
                        StopPreview();
                }
                GUI.backgroundColor = preBg;
                if (GUILayout.Button(Lang.Copy))
                {
                    CopyAnimClips(offAnims);
                }
                if (GUILayout.Button(Lang.Paste))
                {
                    PasteAnimClips(offAnims);
                }
                EditorGUILayout.EndHorizontal();
                offAnimsList.DoLayoutList();

                EditorGUILayout.HelpBox(Lang.AnimClipOffNote, MessageType.Info);
                //打开动画
                GUILayout.Label(Lang.AnimClipOn, EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = previewing == onAnims ? MyGUIStyle.activeButtonColor : preBg;
                if (GUILayout.Button(Lang.Preview))
                {
                    if (previewing != onAnims)
                        StartPreview(onAnims, PreviewType.AnimationClips);
                    else
                        StopPreview();
                }
                GUI.backgroundColor = preBg;
                if (GUILayout.Button(Lang.Copy))
                {
                    CopyAnimClips(onAnims);
                }
                if (GUILayout.Button(Lang.Paste))
                {
                    PasteAnimClips(onAnims);
                }
                EditorGUILayout.EndHorizontal();
                onAnimsList.DoLayoutList();
                
            }

            
            Preview();
            serializedObject.ApplyModifiedProperties();
            
        }

        public void StartPreview(SerializedProperty prev , PreviewType type)
        {
            Animator animator = avatar.GetComponent<Animator>();
            if(!animator)
            {
                animator = avatar.AddComponent<Animator>();
            }
            //没有Controller会崩溃
            if (!animator.runtimeAnimatorController)
            {
                if (!Directory.Exists(EasyAvatarTool.workingDirectory + "Build/"))
                    Directory.CreateDirectory(EasyAvatarTool.workingDirectory +"Build/");
                AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(EasyAvatarTool.workingDirectory + "Build/preview.controller");
                animator.runtimeAnimatorController = controller;
            }
            previewing = prev;
            previewType = type;
            if (!AnimationMode.InAnimationMode())
                AnimationMode.StartAnimationMode();
        }

        public void StopPreview()
        {
            previewing = null;
            previewType = PreviewType.None;
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
        }

        public void Preview()
        {
            if (!AnimationMode.InAnimationMode() || !avatar ||previewType == PreviewType.None)
                return;
            
            AnimationMode.BeginSampling();
            if (previewType == PreviewType.Behaviors)
                AnimationMode.SampleAnimationClip(avatar, EasyAvatarTool.Utility.GenerateAnimClip(previewing), 0);
            else if(previewType == PreviewType.AnimationClips)
                AnimationMode.SampleAnimationClip(avatar, EasyAvatarTool.Utility.MergeAnimClip(previewing), 0);
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

        public static void CopyAnimClips(SerializedProperty animClips)
        {
            copidClips = new List<AnimationClip>();
            for (int i = 0; i < animClips.arraySize; i++)
                copidClips.Add((AnimationClip)animClips.GetArrayElementAtIndex(i).objectReferenceValue);

        }

        public static void PasteAnimClips(SerializedProperty animClips)
        {
            if (copidClips == null)
                return;
            animClips.arraySize = copidClips.Count;
            for (int i = 0; i < animClips.arraySize; i++)
                animClips.GetArrayElementAtIndex(i).objectReferenceValue = copidClips[i];
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

        public void DrawAnimClip(Rect position, SerializedProperty clip)
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
