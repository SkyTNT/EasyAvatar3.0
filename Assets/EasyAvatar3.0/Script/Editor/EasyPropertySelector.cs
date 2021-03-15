using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;

namespace EasyAvatar
{
    public class EasyPropertySelector : EditorWindow
    {
        
        EasyPropertyTree tree;

        public static void DoSelect(Rect rect, SerializedProperty propertyGroup, GameObject avatar, GameObject target)
        {
            SerializedProperty property = propertyGroup.GetArrayElementAtIndex(0);
            SerializedProperty targetProperty = property.FindPropertyRelative("targetProperty");
            SerializedProperty targetPropertyType = property.FindPropertyRelative("targetPropertyType");
            string displayName = "";
            if (targetProperty.stringValue == "")
                displayName = "None";
            else
            {
                Type properTytype = EasyReflection.FindType(targetPropertyType.stringValue);
                displayName = properTytype.Name + ":" + EasyBehavior.NicifyPropertyGroupName(properTytype, targetProperty.stringValue);
            }
            if (GUI.Button(rect, displayName))
            {
                if(!target)
                {
                    EditorUtility.DisplayDialog("Error", Lang.MissingTarget, "ok");
                    return;
                }
                EasyPropertySelector editorWindow = CreateInstance<EasyPropertySelector>();
                editorWindow.Init(propertyGroup, avatar, target);
                //转换到屏幕坐标
                Vector2 vector2 = EditorGUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
                Rect screenFixedRect = new Rect(rect)
                {
                    x = vector2.x,
                    y = vector2.y
                };
                float width = Mathf.Min(Mathf.Max(100, screenFixedRect.width), 300);
                editorWindow.ShowAsDropDown(screenFixedRect, new Vector2(width * 2, width * 3));
            }
        }
        

        public void Init(SerializedProperty propertyGroup, GameObject avatar, GameObject target)
        {
            tree = new EasyPropertyTree(propertyGroup, avatar, target, new TreeViewState());
        }
        public void OnGUI()
        {
            tree.OnGUI(new Rect(0, 0, position.width, position.height));

        }

    }

    public class EasyPropertyTree : TreeView
    {
        GameObject avatar, target;
        SerializedProperty propertyGroup,property, targetProperty, targetPropertyType, valueType, isDiscrete, isPPtr;
        EditorCurveBinding[] bindings;
        public EasyPropertyTree(SerializedProperty propertyGroup, GameObject avatar, GameObject target, TreeViewState treeViewState)
        : base(treeViewState)
        {
            this.target = target;
            this.avatar = avatar;
            this.propertyGroup = propertyGroup;
            property = propertyGroup.GetArrayElementAtIndex(0);
            targetProperty = property.FindPropertyRelative("targetProperty");
            targetPropertyType = property.FindPropertyRelative("targetPropertyType");
            valueType = property.FindPropertyRelative("valueType");
            isDiscrete = property.FindPropertyRelative("isDiscrete");
            isPPtr = property.FindPropertyRelative("isPPtr");
            bindings = AnimationUtility.GetAnimatableBindings(target, avatar);
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            //按照type分类
            Dictionary<string, List<EditorCurveBinding[]>> bindingsDictionary =new Dictionary<string, List<EditorCurveBinding[]>>();
            //分组
            List<EditorCurveBinding> tempGroup = new List<EditorCurveBinding>();
            for(int i=0;i<bindings.Length;i++)
            {
                EditorCurveBinding binding = bindings[i];
                tempGroup.Add(binding);
                if (!bindingsDictionary.ContainsKey(binding.type.Name))
                    bindingsDictionary.Add(binding.type.Name,new List<EditorCurveBinding[]>());
                if(i == bindings.Length-1 || EasyBehavior.NicifyPropertyGroupName(bindings[i+1].type,bindings[i+1].propertyName)!= EasyBehavior.NicifyPropertyGroupName(binding.type, binding.propertyName))
                {
                    bindingsDictionary[binding.type.Name].Add(tempGroup.ToArray());
                    tempGroup.Clear();
                }
            }
            //构建树
            PropertyTreeItem root = new PropertyTreeItem(0, -1, "Root");
            List<TreeViewItem> items = new List<TreeViewItem>();
            foreach (string name in bindingsDictionary.Keys)
            {
                //把Is Active单独列出来
                if (!name.Contains("GameObject"))
                    items.Add(new PropertyTreeItem(items.Count +1, 0, name));

                foreach (EditorCurveBinding[] bindingGroup in bindingsDictionary[name])
                {
                    items.Add(new PropertyTreeItem(items.Count + 1, bindingGroup[0].propertyName == "m_IsActive" ? 0 : 1, EasyBehavior.NicifyPropertyGroupName(bindingGroup[0].type, bindingGroup[0].propertyName), bindingGroup));
                }
            }
            SetupParentsAndChildrenFromDepths(root, items);
            return root;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);
            Rect position = args.rowRect;
            PropertyTreeItem item = (PropertyTreeItem) args.item;
            
            if (item.bindingGroup != null)
            {
                EditorCurveBinding binding = item.bindingGroup[0];
                Texture2D icon = GetIcon(avatar,binding);
                //绘制图标
                if (icon)
                {
                    Rect iconRect = new Rect(position) { width = position.height*icon.width/icon.height};
                    position.x += iconRect.width + 5;
                    position.width -= iconRect.width + 5;
                    GUI.DrawTexture(iconRect, icon);
                }
                //绘制添加按钮
                Rect addBottonRect = new Rect(position) { width = position.height};
                position.width -= addBottonRect.width;
                addBottonRect.x = position.x+ position.width;
                GUI.Box(addBottonRect,GUIContent.none, "Tag MenuItem");
                if (GUI.Button(addBottonRect, EditorGUIUtility.TrIconContent("Toolbar Plus"), "IconButton"))
                {
                    ApplyBinding(item.bindingGroup);
                    EditorWindow.GetWindow<EasyPropertySelector>().Close();
                }
                
            }
            
        }

        protected override void DoubleClickedItem(int id)
        {
            PropertyTreeItem item = (PropertyTreeItem)FindItem(id,rootItem);
            if(item.bindingGroup != null)
            {
                ApplyBinding(item.bindingGroup);
                EditorWindow.GetWindow<EasyPropertySelector>().Close();
            }
        }

        public void ApplyBinding(EditorCurveBinding[] bindingGroup)
        {
            propertyGroup.arraySize = bindingGroup.Length;
            for (int i = 0; i < bindingGroup.Length; i++)
            {
                EditorCurveBinding binding = bindingGroup[i];
                SerializedProperty property = propertyGroup.GetArrayElementAtIndex(i);
                property.FindPropertyRelative("targetProperty").stringValue = binding.propertyName;
                property.FindPropertyRelative("targetPropertyType").stringValue = binding.type.FullName;
                property.FindPropertyRelative("valueType").stringValue = AnimationUtility.GetEditorCurveValueType(avatar, binding).FullName;
                property.FindPropertyRelative("isDiscrete").boolValue = binding.isDiscreteCurve;
                property.FindPropertyRelative("isPPtr").boolValue = binding.isPPtrCurve;

                
            }
            propertyGroup.serializedObject.ApplyModifiedProperties();
            propertyGroup.serializedObject.Update();
        }

        static Texture2D GetIcon(GameObject avatar, EditorCurveBinding binding)
        {
            return AssetPreview.GetMiniThumbnail(AnimationUtility.GetAnimatedObject(avatar, binding));
        }

        public class PropertyTreeItem : TreeViewItem
        {
            public EditorCurveBinding[] bindingGroup;

            public PropertyTreeItem( int id, int depth,string displayName, EditorCurveBinding[] bindingGroup = null)
            {
                this.bindingGroup = bindingGroup;
                this.id = id;
                this.depth = depth;
                this.displayName = displayName;
            }
        }
    }
}

