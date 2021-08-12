using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    public class EasyMissingTargetReplacer : EditorWindow
    {
        GameObject avatar;
        EasyAvatarHelper helper;
        List<EasyPropertyGroup> missingPropertyGroups = new List<EasyPropertyGroup>();
        Dictionary<string, string> pathMap = new Dictionary<string, string>();
        Dictionary<string, List<EasyPropertyGroup>> groupMap = new Dictionary<string, List<EasyPropertyGroup>>();
        List<Object> undoObjects = new List<Object>();
        Vector2 scrollPos = new Vector2();

        public static void Field(EasyAvatarHelper helper)
        {
            if (GUILayout.Button(Lang.OneClickReplaceMissingTarget))
            {
                
                EasyMissingTargetReplacer easyMissingTargetReplacer = CreateWindow<EasyMissingTargetReplacer>(Lang.OneClickReplaceMissingTarget);
                easyMissingTargetReplacer.Init(helper);
                easyMissingTargetReplacer.Show();
            }
        }

        public void Init(EasyAvatarHelper helper)
        {
            this.helper = helper;
            avatar = helper.avatar;
        }

        float lastReloadTime = 0;
        private void ReLoad()
        {
            if (Time.realtimeSinceStartup - lastReloadTime < 0.1)
                return;
            lastReloadTime = Time.realtimeSinceStartup;
            EasyMenu mainMenu = null;
            EasyGestureManager gestureManager = null;
            var oldPathMap = pathMap;
            pathMap = new Dictionary<string, string>();
            missingPropertyGroups.Clear();
            groupMap.Clear();
            undoObjects.Clear();

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

            SearchMenu(mainMenu.transform);

            foreach (Transform child in gestureManager.transform)
            {
                EasyGesture gesture = child.GetComponent<EasyGesture>();
                if (!gesture)
                    continue;
                undoObjects.Add(gesture);
                FindBehaviorGroup(gesture.behaviors1);
                FindBehaviorGroup(gesture.behaviors2);
            }

            {
                var pathMapList = new List<KeyValuePair<string, string>>(pathMap);
                pathMapList.Sort((KeyValuePair<string, string> s1, KeyValuePair<string, string> s2) =>
                {
                    return s1.Key.CompareTo(s2.Key);
                });
                pathMap.Clear();
                foreach (var p in pathMapList)
                {
                    pathMap.Add(p.Key, p.Value);
                }
            }


            void SearchMenu(Transform menu)
            {
                foreach (Transform child in menu)
                {
                    EasyMenu subMenu = child.GetComponent<EasyMenu>();
                    EasyControl control = child.GetComponent<EasyControl>();
                    if (control)
                    {
                        undoObjects.Add(control);
                        foreach (var behaviorGroup in control.behaviors)
                            FindBehaviorGroup(behaviorGroup);
                    }

                    if (subMenu)
                        SearchMenu(subMenu.transform);
                }
            }

            void FindBehaviorGroup(EasyBehaviorGroup behaviorGroup)
            {
                foreach (var behavior in behaviorGroup.list)
                {
                    string path = behavior.propertyGroup.targetPath;
                    if (!avatar.transform.Find(path))
                    {
                        EasyPropertyGroup propertyGroup = behavior.propertyGroup;
                        missingPropertyGroups.Add(propertyGroup);
                        if (!oldPathMap.TryGetValue(propertyGroup.targetPath, out string oldVal))
                            oldVal = "";
                        pathMap[propertyGroup.targetPath] = oldVal;
                        if (!groupMap.TryGetValue(propertyGroup.targetPath, out var propertyGroups))
                        {
                            propertyGroups = new List<EasyPropertyGroup>();
                            groupMap.Add(propertyGroup.targetPath, propertyGroups);
                        }
                        propertyGroups.Add(propertyGroup);
                    }
                }
            }
        }

        private void OnGUI()
        {
            ReLoad();
            if (pathMap.Count == 0)
            {
                GUILayout.Label(Lang.NoMissingTarget);
                return;
            }
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginHorizontal();
            GUILayout.Label(Lang.Missing, GUILayout.MaxWidth(position.width / 2));
            GUILayout.Label(Lang.Replacement, GUILayout.MaxWidth(position.width / 2));
            GUILayout.EndHorizontal();
            KeyValuePair<string, string> change;
            bool changed = false;
            foreach (var missingPath in pathMap)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(missingPath.Key , GUILayout.MaxWidth(position.width/2));
                string tempPath = missingPath.Value;
                if(TargetObjectField(ref tempPath, out GameObject tempTarget, GUILayout.MaxWidth(position.width / 2)))
                {
                    change = new KeyValuePair<string, string>(missingPath.Key, tempPath);
                    changed = true;
                }
                GUILayout.EndHorizontal();
            }
            if (changed)
            {
                pathMap[change.Key] = change.Value;
            }
            if (GUILayout.Button(Lang.OneClickReplaceMissingTarget))
            {
                Undo.RecordObjects(undoObjects.ToArray(), Lang.OneClickReplaceMissingTarget);
                foreach (var missingPath in pathMap)
                {
                    GameObject tempTarget = null;
                    if (avatar && missingPath.Value != "")
                    {
                        Transform tempTransform = avatar.transform.Find(missingPath.Value);
                        if (tempTransform)
                            tempTarget = tempTransform.gameObject;
                    }
                    if (tempTarget)
                    {
                        foreach (var group in groupMap[missingPath.Key])
                        {
                            group.targetPath = missingPath.Value;
                            group.tempTarget = tempTarget;

                        }
                    }
                }
                EditorUtility.DisplayDialog("Info", Lang.BuildSucceed, "ok");
            }
            GUILayout.Space(100);
            GUILayout.EndScrollView();
        }


        public bool TargetObjectField(ref string targetPath, out GameObject tempTarget, params GUILayoutOption[] options)
        {
            GameObject newTarget = null;
            tempTarget = null;
            //获取目标物体
            if (avatar && targetPath != "")
            {
                Transform tempTransform = avatar.transform.Find(targetPath);
                if (tempTransform)
                    tempTarget = tempTransform.gameObject;
            }
            //当前avatar是否缺失目标物体（因为是目标物体相对于avatar的）
            bool isMissing = !tempTarget && targetPath != "";
            //目标物体
            newTarget = (GameObject)EditorGUILayout.ObjectField( tempTarget, typeof(GameObject), true, options);

            if (newTarget != tempTarget)
            {
                if (!avatar)
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarNotSet, "ok");
                else
                    targetPath = newTarget.transform.GetHierarchyPath(avatar.transform);
            }

            return newTarget != tempTarget;
        }
    }
}


