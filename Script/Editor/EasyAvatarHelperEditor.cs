using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using System.IO;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyAvatarHelper))]
    public class EasyAvatarHelperEditor : Editor
    {
        SerializedProperty avatar;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            avatar = serializedObject.FindProperty("avatar");
            EditorGUI.BeginChangeCheck();
            GameObject oldAvatar = (GameObject)avatar.objectReferenceValue;
            avatar.objectReferenceValue = EditorGUILayout.ObjectField(Lang.Avatar, avatar.objectReferenceValue, typeof(GameObject), true);
            GameObject avatarObj = (GameObject)avatar.objectReferenceValue;
            if (EditorGUI.EndChangeCheck()&& avatarObj)
            {
                //防止设置为自己
                if(avatarObj.transform == ((EasyAvatarHelper)target).transform)
                {
                    avatar.objectReferenceValue = avatarObj = null;
                }
                SetNewAvatar(avatarObj, oldAvatar);
                //检测Avatar Helper是否包含在Avatar中，vrchat是不允许avatar包含非白名单内的脚本的。
                if (avatarObj&&((EasyAvatarHelper)target).transform.IsChildOf(avatarObj.transform))
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarHelperInAvatar, "ok");
            }
            
            if (avatarObj)
            {
                target.name = Lang.AvatarHelper + avatar.objectReferenceValue.name;
                if (!avatarObj.GetComponent<VRCAvatarDescriptor>())
                {
                    if (GUILayout.Button(Lang.AvataNoDescriptor))
                    {
                        avatarObj.AddComponent<VRCAvatarDescriptor>();
                    }
                }
                else
                {
                    if (GUILayout.Button(Lang.AvatarApply))
                    {
                        new EasyAvatarBuilder((EasyAvatarHelper)target).Build();
                    }
                    EditorGUILayout.HelpBox(Lang.AvatarApplyHelpBox, MessageType.Warning);

                    EasyMissingTargetReplacer.Field((EasyAvatarHelper)target);
                    EditorGUILayout.HelpBox(Lang.OneClickReplaceMissingTargetNote, MessageType.Info);
                }
                //检测Avatar Helper是否包含在Avatar中，vrchat是不允许avatar包含非白名单内的脚本的。
                if (((EasyAvatarHelper)target).transform.IsChildOf(avatarObj.transform))
                    EditorGUILayout.HelpBox(Lang.ErrAvatarHelperInAvatar, MessageType.Error);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SetNewAvatar(GameObject avatar, GameObject oldAvatar)
        {
            EasyMenu mainMenu = null;
            EasyGestureManager gestureManager = null;
            EasyAvatarHelper helper = (EasyAvatarHelper)target;

            foreach (Transform child in helper.gameObject.transform)
            {
                EasyMenu tempMenu = child.GetComponent<EasyMenu>();
                EasyGestureManager tempGestureManager = child.GetComponent<EasyGestureManager>();
                if (tempMenu)
                {
                    if (mainMenu)//检测是否有多个主菜单
                    {
                        EditorUtility.DisplayDialog("Error", Lang.ErrAvatarMenuLen1, "ok");
                        return;
                    }
                    mainMenu = tempMenu;
                }

                if (tempGestureManager)
                {
                    if (gestureManager)//检测是否有多个手势管理
                    {
                        EditorUtility.DisplayDialog("Error", Lang.ErrAvatarGestureManagerLen1, "ok");
                        return;
                    }
                    gestureManager = tempGestureManager;
                }
            }

            if (mainMenu)
                SearchMenu(mainMenu.transform);
            if (gestureManager)
                foreach (Transform child in gestureManager.transform)
                {
                    EasyGesture gesture = child.GetComponent<EasyGesture>();
                    if (!gesture)
                        continue;
                    FindBehaviorGroup(gesture.behaviors1);
                    FindBehaviorGroup(gesture.behaviors2);
                }

            void SearchMenu(Transform menu)
            {
                foreach (Transform child in menu)
                {
                    EasyMenu subMenu = child.GetComponent<EasyMenu>();
                    EasyControl control = child.GetComponent<EasyControl>();
                    if (control)
                    {
                        foreach (var behaviorGroup in control.behaviors)
                            FindBehaviorGroup(behaviorGroup);
                    }

                    if (subMenu)
                        SearchMenu(subMenu.transform);
                }
            }

            void FindBehaviorGroup(EasyBehaviorGroup behaviorGroup)
            {
                if (behaviorGroup == null)
                    return;
                foreach (var behavior in behaviorGroup.list)
                {
                    EasyPropertyGroup propertyGroup = behavior.propertyGroup;

                    if (oldAvatar && propertyGroup.tempTarget && propertyGroup.tempTarget.transform.IsChildOf(oldAvatar.transform))//先获取在原来的avatar上的propertyGroup.targetPath
                    {
                        propertyGroup.targetPath = propertyGroup.tempTarget.transform.GetHierarchyPath(oldAvatar.transform);
                    }

                    if (propertyGroup.targetPath != "")
                    {
                        Transform tempTransform = avatar.transform.Find(propertyGroup.targetPath);
                        if (tempTransform)
                        {
                            propertyGroup.tempTarget = tempTransform.gameObject;
                        }
                    }
                }
            }
        }
        
    }
}

