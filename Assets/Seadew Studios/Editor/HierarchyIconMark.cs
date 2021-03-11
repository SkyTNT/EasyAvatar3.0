using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRCSDK2;
using VRC.SDKBase;

namespace SeadewStudios
{
    [InitializeOnLoad]
    public class HierarchyIconMark : EditorWindow
    {

        //[MenuItem("Seadew Studios/HierarchyIconMark")]
        //static void Init()
        //{
        //    EditorWindow editorWindow = EditorWindow.GetWindow(typeof(HierarchyIconMark));
        //    editorWindow.autoRepaintOnSceneChange = true;
        //    //editorWindow.minSize = new Vector2(350, 300);
        //    editorWindow.Show();
        //    editorWindow.titleContent = new GUIContent("HierarchyIconMark");

        //}

        //private void OnGUI()
        //{
        //    if (GUILayout.Button("空按钮"))
        //    {

        //    }

        //}

        private static readonly  List<Texture2D> icon;
        enum iconState
        {
            iconAD,
            iconDB,
            iconDBC,
            iconCC
        }

        private static readonly Texture2D iconDBC, iconCC,iconSph,iconAD,selected;
        private static Texture2D head, headI,tail,openEye,CloseEye,Y,G,Black;
       private const float iconWidth = 14;

         static HierarchyIconMark()
        {
            //Debug.Log("LoadingIcon");
            //iconAD = AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/AvatarDescriptorIcon.png", typeof(Texture2D)) as Texture2D;
            //iconDB = AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/BoneIcon.png", typeof(Texture2D)) as Texture2D;
            //iconDBC = EditorGUIUtility.FindTexture("CapsuleCollider Icon");
            //iconCC = EditorGUIUtility.FindTexture("CapsuleCollider Icon");
            icon = new List<Texture2D>();
            selected = AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/Selected.png", typeof(Texture2D)) as Texture2D;
            head = AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/Head.png", typeof(Texture2D)) as Texture2D;
            headI = AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/HeadI.png", typeof(Texture2D)) as Texture2D;
            tail = AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/Tail.png", typeof(Texture2D)) as Texture2D;
            openEye=(AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/OpenEyeIcon.png", typeof(Texture2D)) as Texture2D);
            CloseEye=(AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/CloseEyeIcon.png", typeof(Texture2D)) as Texture2D);
            iconAD = (AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/AvatarDescriptorIcon.png", typeof(Texture2D)) as Texture2D);
            icon.Add(iconAD);
            icon.Add(AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/BoneIcon.png", typeof(Texture2D)) as Texture2D);
            iconDBC = (AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/BoneColliderIcon.png", typeof(Texture2D)) as Texture2D);
            icon.Add(iconDBC);
            //iconCC = (AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/CapsuleColliderIcon.png", typeof(Texture2D)) as Texture2D);
            iconCC = EditorGUIUtility.ObjectContent(null, typeof(CapsuleCollider)).image as Texture2D;
            icon.Add(iconCC);
            iconSph = EditorGUIUtility.ObjectContent(null, typeof(SphereCollider)).image as Texture2D;
            icon.Add(iconSph);
            icon.Add(EditorGUIUtility.ObjectContent(null, typeof(SkinnedMeshRenderer)).image as Texture2D);
            icon.Add(EditorGUIUtility.ObjectContent(null, typeof(Cloth)).image as Texture2D);
            //icon.Add(AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/ClothIcon.png", typeof(Texture2D)) as Texture2D);
            Y = CreateBackgroundColorImage(new Color(238F / 256, 226F / 256, 149F / 256));
            G = CreateBackgroundColorImage(new Color(145F / 256, 220F / 256, 69F / 256));
            Black = CreateBackgroundColorImage(new Color(50F / 256, 50F / 256, 50F / 256));

            //if (iconAD == null | iconDB == null) return;
            if (icon.Count==0)
            {
                return;
            }

            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
        }

        private static void HierarchyItemCB(int instanceID, Rect rect)
        {

            //Debug.Log("CheckComponent...");
            //if (iconAD == null | iconDB == null) return;
            if (icon.Count == 0)
            {
                return;
            }

            int iconIndex = 0;
            List<Texture2D> viewicon = new List<Texture2D>();

            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;


            if (gameObject == null) return;

            //Debug.Log(string.Format("ID:{0}H:{1}N:{2}", instanceID, gameObject.name, gameObject.transform.GetSiblingIndex()));

            //Scene视图状态获取
            //var avatarDescriptor = gameObject.GetComponent<VRC_AvatarDescriptor>();
            //if (gameObject.activeInHierarchy == true)
            //{
            //    viewicon.Add(icon[iconIndex]);
            //    iconIndex+=2;
            //}
            //else
            //{
            //    iconIndex++;
            //    viewicon.Add(icon[iconIndex]);
            //    iconIndex++;
            //}


            //==组件获取==
            if (gameObject.GetComponent<VRC_AvatarDescriptor>() == null)
            {
            }
            else { viewicon.Add(icon[iconIndex]);   }
            iconIndex++;
            
            if (gameObject.GetComponent<DynamicBone>() == null)
            {
            }
            else { viewicon.Add(icon[iconIndex]);  }
            iconIndex++;
            
            if (gameObject.GetComponent<DynamicBoneCollider>() == null)
            {
            }
            else { viewicon.Add(icon[iconIndex]);  }
            iconIndex++;

            if (gameObject.GetComponent<CapsuleCollider>() == null)
            {
            }
            else { viewicon.Add(icon[iconIndex]); }
            iconIndex++;

            if (gameObject.GetComponent<SphereCollider>() == null)
            {
            }
            else { viewicon.Add(icon[iconIndex]); }
            iconIndex++;

            if (gameObject.GetComponent<SkinnedMeshRenderer>() == null)
            {
            }
            else { viewicon.Add(icon[iconIndex]); }
            iconIndex++;

            if (gameObject.GetComponent<Cloth>() == null)
            {
            }
            else { viewicon.Add(icon[iconIndex]); }
            iconIndex++;


            EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconWidth));
            //控件图标偏移
                var padding = new Vector2(2, 0);


            Rect headRect = new Rect(rect.x, rect.y, iconWidth + 2, iconWidth + 2);

            //==头部列表==

            //Seadew选择Avatar标记
            if (SeadewStudiosToolKit.cState.selectedAvatar == gameObject)
            {
                if (gameObject.transform.childCount == 0)
                {
                    GUI.DrawTexture(
                new Rect(
                    headRect.x -= (iconWidth + 2)/*(iconWidth + 3)*/,
                    headRect.y /*+ padding.y*/,
                    headRect.width,
                    headRect.height),
                 selected
                 );
                }
                else
                {
                    GUI.DrawTexture(
                new Rect(
                    headRect.x -= (iconWidth*2 + 2)/*(iconWidth + 3)*/,
                    headRect.y /*+ padding.y*/,
                    headRect.width,
                    headRect.height),
                 selected
                 );
                }
            }

            //顶级对象不需要分层指示
            if (gameObject.transform.root == gameObject.transform)
            {
                //GUI.DrawTexture(
                //new Rect(
                //    headRect.x -= (iconWidth*2 + 1),
                //    headRect.y /*+ padding.y*/,
                //    headRect.width,
                //    headRect.height),
                // icon[2]
                // );//L2

                //GUIStyle style = new GUIStyle()
                //{
                //    padding =
                //{
                //    left =EditorStyles.label.padding.left+1,
                //    top = EditorStyles.label.padding.top+1
                //},
                //    normal =
                //{
                //    textColor =Color.blue
                //}
                //};
                //if (style != null && gameObject != null)
                //{
                //    GUI.Label(rect, gameObject.name, style);
                //}
            }
            else
            if (gameObject.transform.GetSiblingIndex()==gameObject.transform.parent.childCount-1)
            {
                GUI.DrawTexture(
                new Rect(
                    headRect.x -= ((iconWidth * 2) + 2)/*(iconWidth + 3)*/,
                    headRect.y /*+ padding.y*/,
                    headRect.width,
                    headRect.height),
                 tail
                 );//尾部
            }
            else
            if (gameObject.transform.childCount == 0)
            {
                GUI.DrawTexture(
                new Rect(
                    headRect.x -= ((iconWidth * 2) + 2)/*(iconWidth + 3)*/,
                    headRect.y /*+ padding.y*/,
                    headRect.width,
                    headRect.height),
                 head
                 );//无子物体

                //if (gameObject.transform.parent != null)
                //    if (gameObject.transform.parent != gameObject.transform.root)
                //    {
                //        DrawHead(gameObject.transform);
                //    }
            }
            else
            {
                GUI.DrawTexture(
                new Rect(
                    headRect.x -= ((iconWidth * 2) + 2),
                    headRect.y /*+ padding.y*/,
                    headRect.width,
                    headRect.height),
                 head
                 );//有子物体

                
            }

            //迭代判断父级物体链接关系
            DrawHead(gameObject.transform);


            
            void DrawHead(Transform tf)
            {
                if (tf.parent != null)
                    if (tf.parent != tf.root)
                        if (tf.parent.parent != null)
                        {
                            if (tf.parent.GetSiblingIndex() != tf.parent.parent.childCount - 1)
                            {
                                GUI.DrawTexture(
                                new Rect(
                                    headRect.x -= (iconWidth),
                                    headRect.y /*+ padding.y*/,
                                    headRect.width,
                                    headRect.height),
                                 headI
                                 );//L2
                            }
                            else
                                headRect.x -= (iconWidth);

                            DrawHead(tf.parent);
                        }
            }

            //Debug.Log(string.Format( "CheckComponent{0}{1}...", viewicon.Count,icon.Count));

            //==尾部列表==



            //预留色块位置
            padding.x += 2;

            if (viewicon.IndexOf(iconAD) > -1)
            {
                var iconDrawRect = new Rect(
                    rect.xMax - padding.x,
                    rect.yMin - 0.5f/*+ padding.y*/,
                    8,
                    rect.height + 1);
                EditorGUI.LabelField(iconDrawRect, new GUIContent(Black));
            }
            else
            if (viewicon.IndexOf(iconCC) > -1)
            {
                var iconDrawRect = new Rect(
                    rect.xMax - padding.x,
                    rect.yMin - 0.5f/*+ padding.y*/,
                    8,
                    rect.height + 1);
                EditorGUI.LabelField(iconDrawRect, new GUIContent(G));
            }
            else
            if (viewicon.IndexOf(iconSph) > -1)
            {
                var iconDrawRect = new Rect(
                    rect.xMax - padding.x,
                    rect.yMin - 0.5f/*+ padding.y*/,
                    8,
                    rect.height + 1);
                EditorGUI.LabelField(iconDrawRect, new GUIContent(G));
            }
            else
            if (viewicon.IndexOf(iconDBC) > -1)
            {
                var iconDrawRect = new Rect(
                    rect.xMax - padding.x,
                    rect.yMin - 0.5f/*+ padding.y*/,
                    8,
                    rect.height + 1);
                EditorGUI.LabelField(iconDrawRect, new GUIContent(Y));
            }

            padding.x += 2;



            //视图状态显示
            if (gameObject.activeInHierarchy == true)
            {
                var iconDrawRect = new Rect(
                    rect.xMax - (iconWidth+ padding.x),
                    rect.yMin + padding.y,
                    rect.width,
                    rect.height);
                var iconGUIContent = new GUIContent(openEye);

                if (GUI.Button(iconDrawRect, iconGUIContent, EditorStyles.label))
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                var iconDrawRect = new Rect(
                    rect.xMax - (iconWidth + padding.x),
                    rect.yMin + padding.y,
                    rect.width,
                    rect.height);
                var iconGUIContent = new GUIContent(CloseEye);

                if (GUI.Button(iconDrawRect, iconGUIContent, EditorStyles.label))
                {
                    show(gameObject.transform);
                }
            }
            //矫正到Root级的显示
            void show (Transform tf)
            {
                if (!tf.gameObject.activeSelf)
                {
                    tf.gameObject.SetActive(true);
                }
                if (tf.parent != null)
                        show(tf.parent);
            }

            if (viewicon.Count == 0)
            { return; }
            //特定组件图标显示
            for (int i = 0; i < viewicon.Count; i++)
            {
                var iconDrawRect = new Rect(
                    rect.xMax - ((iconWidth+1)*(i+2)  + padding.x),
                    rect.yMin + padding.y,
                    rect.width,
                    rect.height);
                var iconGUIContent = new GUIContent(viewicon[i]);
                EditorGUI.LabelField(iconDrawRect, iconGUIContent);//L1
            }

            //Debug.Log("ComponentMark...");
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }


        /// <summary>
        /// 纯色生成图片
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Texture2D CreateBackgroundColorImage(UnityEngine.Color color)
        {
            int w = 4, h = 4;
            Texture2D back = new Texture2D(w, h);
            UnityEngine.Color[] buffer = new UnityEngine.Color[w * h];
            for (int i = 0; i < w; ++i)
                for (int j = 0; j < h; ++j)
                    buffer[i + w * j] = color;
            back.SetPixels(buffer);
            back.Apply(false);
            return back;
        }

        static string GetGameObjectPath(GameObject obj, bool skipRoot = true)
        {
            string path = null;
            if (obj.transform != obj.transform.root)
            {
                if (!skipRoot)
                    path = obj.transform.root.name + "/";
                path += (AnimationUtility.CalculateTransformPath(obj.transform, obj.transform.root));
            }
            else
            {
                if (!skipRoot)
                    path = obj.transform.root.name;
            }

            return path;
        }
    }

   
}
