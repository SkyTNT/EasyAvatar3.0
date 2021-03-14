using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace EasyAvatar
{
    public class EasyPropertySelector : EditorWindow
    {
        static GameObject avatar, target;
        static EditorCurveBinding[] bindings;
        static EasyPropertyTree tree;

        public static void open(GameObject _avatar, GameObject _target,Rect rect)
        {
            avatar = _avatar;
            target = _target;
            bindings = AnimationUtility.GetAnimatableBindings(target, avatar);
            tree = new EasyPropertyTree(bindings, new TreeViewState());
            EditorWindow editorWindow = GetWindow<EasyPropertySelector>();
            //editorWindow.Show();
            editorWindow.ShowAsDropDown(rect,new Vector2(rect.width*3,rect.width*9));
        }
        public void OnGUI()
        {
            tree.OnGUI(new Rect(0, 0, position.width, position.height));
        }

    }

    public class EasyPropertyTree : TreeView
    {
        EditorCurveBinding[] bindings;
        public EasyPropertyTree(EditorCurveBinding[] bindings, TreeViewState treeViewState)
        : base(treeViewState)
        {
            this.bindings = bindings;
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
            TreeViewItem root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            List<TreeViewItem> items = new List<TreeViewItem>();
            foreach (string name in bindingsDictionary.Keys)
            {
                items.Add(new TreeViewItem { id = items.Count+1, depth = 0, displayName = name });
                foreach (EditorCurveBinding binding in bindingsDictionary[name])
                {
                    items.Add(new TreeViewItem { id = items.Count + 1, depth = 1, displayName = binding.propertyName });
                }
            }
            SetupParentsAndChildrenFromDepths(root, items);
            return root;
        }

        static Texture2D GetIcon(GameObject avatar, EditorCurveBinding binding)
        {
            return AssetPreview.GetMiniThumbnail(AnimationUtility.GetAnimatedObject(avatar, binding));
        }
    }
}

