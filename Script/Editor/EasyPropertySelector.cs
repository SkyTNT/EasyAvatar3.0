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

        /// <summary>
        /// 绘制Property选择框
        /// </summary>
        /// <param name="rect">位置</param>
        /// <param name="propertyGroup">PropertyGroup</param>
        /// <param name="avatar">avatar</param>
        /// <param name="target">target</param>
        public static void PropertyField(Rect rect, SerializedProperty propertyGroup, GameObject avatar, GameObject target)
        {
            SerializedProperty properties = propertyGroup.FindPropertyRelative("properties");
            GUIContent content = new GUIContent();

            if(properties.arraySize == 0)
            {
                content.text = "None";
            }
            else
            {
                SerializedProperty property0 = properties.GetArrayElementAtIndex(0);
                SerializedProperty targetProperty0 = property0.FindPropertyRelative("targetProperty");
                SerializedProperty targetPropertyType0 = property0.FindPropertyRelative("targetPropertyType");
                Type propertyType = EasyReflection.FindType(targetPropertyType0.stringValue);
                content.text = ObjectNames.NicifyVariableName(propertyType.Name) + ":" + Utility.NicifyPropertyGroupName(propertyType, targetProperty0.stringValue);
                content.image = AssetPreview.GetMiniTypeThumbnail(propertyType);
            }
            
            if (GUI.Button(rect, content,EditorStyles.objectField))
            {
                if(!avatar)
                {
                    EditorUtility.DisplayDialog("Error", Lang.ErrAvatarNotSet, "ok");
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

        /// <summary>
        /// EasyPropertySelector窗口初始化
        /// </summary>
        /// <param name="propertyGroup">PropertyGroup</param>
        /// <param name="avatar">avatar</param>
        /// <param name="target">target</param>
        public void Init(SerializedProperty propertyGroup, GameObject avatar, GameObject target)
        {
            tree = new EasyPropertyTree(propertyGroup, avatar, target, new TreeViewState());
        }

        public void OnGUI()
        {
            Rect rect = new Rect(position) { x = 0, y = 0 };
            tree.OnGUI(rect);
        }

    }

    public class EasyPropertyTree : TreeView
    {
        GameObject avatar, target;
        SerializedProperty propertyGroup;
        EditorCurveBinding[] bindings;
        Dictionary<string, List<EditorCurveBinding[]>> bindingsDictionary;

        /// <summary>
        /// EasyPropertyTree构造函数
        /// </summary>
        /// <param name="propertyGroup">要选择的PropertyGroup</param>
        /// <param name="avatar">avatar</param>
        /// <param name="target">target</param>
        /// <param name="treeViewState">treeViewState</param>
        public EasyPropertyTree(SerializedProperty propertyGroup, GameObject avatar, GameObject target, TreeViewState treeViewState)
        : base(treeViewState)
        {
            this.target = target;
            this.avatar = avatar;
            this.propertyGroup = propertyGroup;
            if(!target)
                bindings = AnimationUtility.GetAnimatableBindings(avatar, avatar);
            else
                bindings = AnimationUtility.GetAnimatableBindings(target, avatar);

            //按照type分类
            bindingsDictionary = new Dictionary<string, List<EditorCurveBinding[]>>();
            //分组; x,y,z,w,r,g,b,a在bindings中是分开的，把它们合成一个Vector,Color之类的
            List<EditorCurveBinding> tempGroup = new List<EditorCurveBinding>();
            for (int i = 0; i < bindings.Length; i++)
            {
                EditorCurveBinding binding = bindings[i];
                tempGroup.Add(binding);
                if (!bindingsDictionary.ContainsKey(binding.type.Name))
                    bindingsDictionary.Add(binding.type.Name, new List<EditorCurveBinding[]>());
                if (i == bindings.Length - 1 || Utility.NicifyPropertyGroupName(bindings[i + 1].type, bindings[i + 1].propertyName) != Utility.NicifyPropertyGroupName(binding.type, binding.propertyName))
                {
                    bindingsDictionary[binding.type.Name].Add(tempGroup.ToArray());
                    tempGroup.Clear();
                }
            }

            Reload();
        }


        /// <summary>
        /// 构建树
        /// </summary>
        /// <returns></returns>
        protected override TreeViewItem BuildRoot()
        {
            
            //构建树
            PropertyTreeItem root = new PropertyTreeItem(0, -1, "Root");
            int id = 1;
            foreach (string name in bindingsDictionary.Keys)
            {
                TreeViewItem typeItem = new PropertyTreeItem(id++, 0, ObjectNames.NicifyVariableName(name));
                List<TreeViewItem> propertyItems = new List<TreeViewItem>();

                //不加GameObject
                if (!name.Contains("GameObject"))
                    root.AddChild(typeItem);
                //先排序
                foreach (EditorCurveBinding[] bindingGroup in bindingsDictionary[name])
                    propertyItems.Add(new PropertyTreeItem(id++, bindingGroup[0].propertyName == "m_IsActive" ? 0 : 1, Utility.NicifyPropertyGroupName(bindingGroup[0].type, bindingGroup[0].propertyName), bindingGroup));
                propertyItems.Sort();
                //再把它加到树中
                foreach (PropertyTreeItem item in propertyItems)
                    if (item.bindingGroup[0].propertyName == "m_IsActive")
                        root.AddChild(item);
                    else
                        typeItem.AddChild(item);
            }

            return root;
        }
        
        /// <summary>
        /// 绘制每行
        /// </summary>
        /// <param name="args"></param>
        protected override void RowGUI(RowGUIArgs args)
        {
            Rect position = args.rowRect;
            PropertyTreeItem item = (PropertyTreeItem) args.item;
            Rect lableRect = new Rect(position) { x = position.x +position.height };
            GUI.Label(lableRect, args.label);
            if (item.bindingGroup != null)
            {
                EditorCurveBinding binding = item.bindingGroup[0];
                Texture2D icon = AssetPreview.GetMiniTypeThumbnail(binding.type);

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


        /// <summary>
        /// 双击
        /// </summary>
        /// <param name="id">击中Item的id</param>
        protected override void DoubleClickedItem(int id)
        {
            PropertyTreeItem item = (PropertyTreeItem)FindItem(id,rootItem);
            if(item.bindingGroup != null)
            {
                ApplyBinding(item.bindingGroup);
                EditorWindow.GetWindow<EasyPropertySelector>().Close();
            }
        }


        /// <summary>
        /// 应用选择
        /// </summary>
        /// <param name="bindingGroup">选择的bindingGroup</param>
        public void ApplyBinding(EditorCurveBinding[] bindingGroup)
        {
            //获取到的旋转是四元数，把它转换成欧拉角
            if (bindingGroup[0].type == typeof(Transform) && bindingGroup[0].propertyName.Contains("m_LocalRotation"))
                bindingGroup = ConvertRotationBindings(bindingGroup);

            SerializedProperty properties = propertyGroup.FindPropertyRelative("properties");
            properties.arraySize = bindingGroup.Length;

            for (int i = 0; i < bindingGroup.Length; i++)
            {
                EditorCurveBinding binding = bindingGroup[i];
                Type valueType = AnimationUtility.GetEditorCurveValueType(avatar, binding);
                SerializedProperty property = properties.GetArrayElementAtIndex(i);
                propertyGroup.FindPropertyRelative("targetPath").stringValue = binding.path;
                property.FindPropertyRelative("targetProperty").stringValue = binding.propertyName;
                property.FindPropertyRelative("targetPropertyType").stringValue = binding.type.FullName;
                property.FindPropertyRelative("valueType").stringValue = valueType.FullName;
                property.FindPropertyRelative("isDiscrete").boolValue = binding.isDiscreteCurve;
                property.FindPropertyRelative("isPPtr").boolValue = binding.isPPtrCurve;

                //获取默认值
                if (binding.isPPtrCurve)
                {
                    UnityEngine.Object value;
                    AnimationUtility.GetObjectReferenceValue(avatar, binding, out value);
                    property.FindPropertyRelative("objectValue").objectReferenceValue = value;
                }
                else
                {
                    float value;
                    AnimationUtility.GetFloatValue(avatar, binding, out value);
                    property.FindPropertyRelative("floatValue").floatValue = value;
                }
            }
            propertyGroup.serializedObject.ApplyModifiedProperties();
            propertyGroup.serializedObject.Update();
        }

        /// <summary>
        /// 把四元数Bindings转换成欧拉角Bindings
        /// </summary>
        /// <param name="bindingGroup"></param>
        /// <returns></returns>
        public static EditorCurveBinding[] ConvertRotationBindings(EditorCurveBinding[] bindingGroup)
        {
            List<EditorCurveBinding> reslut = new List<EditorCurveBinding>();
            foreach(EditorCurveBinding binding in bindingGroup)
            {
                EditorCurveBinding newBinding = EditorCurveBinding.FloatCurve(binding.path, binding.type, binding.propertyName);
                newBinding.propertyName = newBinding.propertyName.Replace("m_LocalRotation", "localEulerAngles");
                if (newBinding.propertyName.Substring(newBinding.propertyName.Length - 1) != "w")
                    reslut.Add(newBinding);
            }
            return reslut.ToArray();
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

            public override int CompareTo(TreeViewItem other)
            {
                PropertyTreeItem otherNode = other as PropertyTreeItem;
                if (otherNode != null)
                {
                    //调整Position和Rotation的位置
                    if (displayName.Contains("Rotation") && otherNode.displayName.Contains("Position"))
                        return 1;
                    if (displayName.Contains("Position") && otherNode.displayName.Contains("Rotation"))
                        return -1;
                }
                return base.CompareTo(other);
            }
        }
    }
}

