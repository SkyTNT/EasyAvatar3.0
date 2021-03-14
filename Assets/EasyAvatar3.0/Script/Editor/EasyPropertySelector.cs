﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace EasyAvatar
{
    public class EasyPropertySelector : EditorWindow
    {
        
        EasyPropertyTree tree;

        public static void EditorCurveBindingField(Rect rect, SerializedProperty property, GameObject avatar, GameObject target)
        {
            SerializedProperty targetPath = property.FindPropertyRelative("targetPath");
            SerializedProperty targetProperty = property.FindPropertyRelative("targetProperty");
            SerializedProperty targetPropertyType = property.FindPropertyRelative("targetPropertyType");
            SerializedProperty valueType = property.FindPropertyRelative("valueType");

            string displayName = targetProperty.stringValue == "" ? "None" : EasyReflection.FindType(targetPropertyType.stringValue).Name + ":" + targetProperty.stringValue;
            if (GUI.Button(rect, displayName))
            {
                if(!target)
                {
                    EditorUtility.DisplayDialog("Error", Lang.MissingTarget, "ok");
                    return;
                }
                EasyPropertySelector editorWindow = CreateInstance<EasyPropertySelector>();
                editorWindow.Init(property,avatar, target);
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
        

        public void Init(SerializedProperty property,GameObject avatar, GameObject target)
        {
            tree = new EasyPropertyTree(property, avatar, target, new TreeViewState());
        }
        public void OnGUI()
        {
            tree.OnGUI(new Rect(0, 0, position.width, position.height));

        }

    }

    public class EasyPropertyTree : TreeView
    {
        GameObject avatar, target;
        SerializedProperty property,targetProperty, targetPropertyType, valueType, isDiscrete, isPPtr;
        EditorCurveBinding[] bindings;
        public EasyPropertyTree(SerializedProperty property, GameObject avatar, GameObject target, TreeViewState treeViewState)
        : base(treeViewState)
        {
            this.target = target;
            this.avatar = avatar;
            this.property = property;
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
            Dictionary<string, List<EditorCurveBinding>> bindingsDictionary =new Dictionary<string, List<EditorCurveBinding>>();
            foreach(EditorCurveBinding binding in bindings)
            {
                if (!bindingsDictionary.ContainsKey(binding.type.Name))
                    bindingsDictionary.Add(binding.type.Name,new List<EditorCurveBinding>());
                bindingsDictionary[binding.type.Name].Add(binding);
            }
            //构建树
            PropertyTreeItem root = new PropertyTreeItem(0, -1, "Root");
            List<TreeViewItem> items = new List<TreeViewItem>();
            foreach (string name in bindingsDictionary.Keys)
            {
                items.Add(new PropertyTreeItem(items.Count +1, 0, name));
                foreach (EditorCurveBinding binding in bindingsDictionary[name])
                {
                    items.Add(new PropertyTreeItem(items.Count + 1, 1, binding.propertyName, binding));
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
            
            if (item.binding != null)
            {
                EditorCurveBinding binding = (EditorCurveBinding) item.binding;
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
                    ApplyBinding(binding);
                    EditorWindow.GetWindow<EasyPropertySelector>().Close();
                }
                
            }
            
        }

        protected override void DoubleClickedItem(int id)
        {
            PropertyTreeItem item = (PropertyTreeItem)FindItem(id,rootItem);
            if(item.binding != null)
            {
                ApplyBinding((EditorCurveBinding)item.binding);
                EditorWindow.GetWindow<EasyPropertySelector>().Close();
            }
        }

        public void ApplyBinding(EditorCurveBinding binding)
        {
            targetProperty.stringValue = binding.propertyName;
            targetPropertyType.stringValue = binding.type.FullName;
            valueType.stringValue = AnimationUtility.GetEditorCurveValueType(avatar, binding).FullName;
            isDiscrete.boolValue = binding.isDiscreteCurve;
            isPPtr.boolValue = binding.isPPtrCurve;
            targetProperty.serializedObject.ApplyModifiedProperties();
            targetProperty.serializedObject.Update();
        }

        static Texture2D GetIcon(GameObject avatar, EditorCurveBinding binding)
        {
            return AssetPreview.GetMiniThumbnail(AnimationUtility.GetAnimatedObject(avatar, binding));
        }

        public class PropertyTreeItem : TreeViewItem
        {
            public EditorCurveBinding? binding;

            public PropertyTreeItem( int id, int depth,string displayName, EditorCurveBinding? binding = null)
            {
                this.binding = binding;
                this.id = id;
                this.depth = depth;
                this.displayName = displayName;
            }
        }
    }
}

