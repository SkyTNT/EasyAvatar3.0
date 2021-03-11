using System.Collections.Generic;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using VRC.Core;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDKBase;


/*============================================*
 * Seadew Studios Tool Kit
 * 蒸馏海工作组工具包
 * By - 命运为何(DTC_Destiny)
 *          
 *                                                  
 *                                           
 * 感谢:                   
 *       山兔(Shatoo)
 *       触手(Yueou)                  
 *       拉狗子(_Lappland_)                  
 *       杨哥(Yangalen_610)     
 *       禁忌(禁忌Apostle)
 *       咖喱给给(Currygeygey)
 *          等海露群友提供的思路代码等帮助...    
 *============================================*/

namespace SeadewStudios
{
    //海露工具箱主菜单UI

    [ExecuteInEditMode, CanEditMultipleObjects]
    public class SeadewStudiosToolKit : EditorWindow
    {
        string V = "2.0.3.2";
        #region 申明部分

        #region Enum部分
        /// <summary>
        /// 杂项功能Enum定义
        /// </summary>
        enum LitFunction
        {
            RemoveDynamicBones,
            RemoveDynamicBoneColliders,
            RemoveColliders,
            RemoveCapsuleColliders,
            RemoveBoxColliders,
            RemoveSphereColliders,
            RemoveMeshColliders,
            ResetPose,
            ResetBlendShapes,
            FixRandomMouth,
            DisableBlinking,
            EditViewpoint,
            FillVisemes,
            SelectedDynamicBoneSetRoot,
            SelectedOBJAddDynamicBone,
            CloseOBJDynamicBone,
            OpenOBJDynamicBone,
            CloseAllDynamicBone,
            OpenAllDynamicBone
        }
        /// <summary>
        /// Log颜色种类
        /// </summary>
        enum LogColor
        {
            DarkGreen,
            DarkBlue,
            DarkRed,
            DarkYellow,
            DarkPurple,
            Red,
            Orange,
            Blue,
            Purple,
            Green,
        }
        #endregion

        #region 类实例化
        /// <summary>
        /// UI控件状态类
        /// </summary>
        public static ControlState cState = new ControlState();
        /// <summary>
        /// 功能类
        /// </summary>
        private FunctionClass functionClass = new FunctionClass();
        #endregion

        static readonly string SSS = "<color=#1B2CD8FF>[SeadewStudiosTK]</color>";

        //bool isVRCD;

        //=UI=
        /// <summary>
        /// 子功能垂直滚动条位置变量
        /// </summary>
        Vector2 vertScroll = Vector2.zero;
        /// <summary>
        /// 版本日记垂直滚动条位置变量
        /// </summary>
        Vector2 vertScroll_Versionlog = Vector2.zero;
        /// <summary>
        /// 目标模型信息类
        /// </summary>
        static AvatarInfo avatarInfo = null;
        static string _avatarInfoString;
        static string gRadiusString = "0.5";
        static float gRadius;
        float sFoldSpaceWidth = 20;

        #region 遗留变量
        //bool bCopier_skinMeshRender_resetBlendShapeValues = true;
        bool bCopier_skinMeshRender_copyMaterials = true;

        //Editing Viewpoint
        bool _edittingView = false;
        #endregion




        //private static Texture2D iconAD, iconDB;
        //private static Texture2D iconView;
        //private const float iconWidth = 12;

        /// <summary>
        /// 目标选择状态
        /// </summary>
        SelectionStates SelectionState = SelectionStates.Empty;

        enum SelectionStates
        {
            Empty,
            ProjectMulitpleObject,
            ProjectSingleObject,
            ProjectMulitpleGameObject,
            ProjectSingleGameObject,
            HierarchyMulitpleGameObject,
            HierarchySingleGameObject,
        }

        #endregion

        #region 函数部分
        //================================================================================================================================================================================================================
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //===============================================================================================⬇Editor窗口初始化⬇===============================================================================================
        [MenuItem("Seadew Studios/工具包(Tool Kit)")]
        static void Init()
        {
            EditorWindow editorWindow = EditorWindow.GetWindow(typeof(SeadewStudiosToolKit));
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.minSize = new Vector2(350, 300);
            editorWindow.Show();
            editorWindow.titleContent = new GUIContent(LocalizationClass.Main.WindowName);

            _DependecyChecker.Check();


            //Debug.Log("LoadingIcon");
            //iconAD = AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/AvatarDescriptorIcon.png", typeof(Texture2D)) as Texture2D;
            //iconDB = AssetDatabase.LoadAssetAtPath("Assets/Seadew Studios/Resources/Icon/BoneIcon.png", typeof(Texture2D)) as Texture2D;

            //if (iconAD == null | iconDB == null) Debug.Log("textrue load error!");
            
        }
        
        //===============================================================================================⬆Editor窗口初始化⬆===============================================================================================
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //================================================================================================⬇GUI窗口生成部分⬇===============================================================================================

        /// <summary>
        /// GUI生成
        /// </summary>
        void OnGUI()
        {
            #region UI显示数值设置杂项
            //字体大小
            int tempSize = UIStyles.Label_mainTitle.fontSize + 6;
            //图标大小
            EditorGUIUtility.SetIconSize(new Vector2(tempSize - 3, tempSize - 3));
            #endregion

            //[主功能版本号] [子功能版本号] [功能类版本号] [修订号]
            GUILabel("Seadew 工具包 V"+V,13,TextAnchor.MiddleCenter,FontStyle.Bold);

            //获取目标Avatar选框
            cState.selectedAvatar = (GameObject)EditorGUILayout.ObjectField(LocalizationClass.Main.SelectedAvatar, cState.selectedAvatar, typeof(GameObject), true);
            //[Button]控件:从Scene选中的obj获取父级未目标Avatar
            if (GUILayout.Button(LocalizationClass.Buttons.SelectFromScene))
            {
                if (Selection.activeObject != null)
                {
                    //try
                    //{
                        cState.selectedAvatar = Selection.activeGameObject.transform.root.gameObject;
                        //avatarInfo = AvatarInfo.GetInfo(cState.selectedAvatar, out _avatarInfoString);
                        
                        //isVRCD = FunctionClass.CheckAvatarDescriptor(cState.selectedAvatar);
                    //}
                    //catch
                    //{
                        //暂时无
                    //}
                }
            }


            //=====================================================
            //                    选择目标类型筛选
            //=====================================================
            //是否选择
            if (Selection.objects.Length > 0)
            {
                //选择对象是否为Project中的Objects
                if (Selection.objects.Length > Selection.gameObjects.Length)
                {
                    if (Selection.objects.Length > 1)
                    {
                        //=选取对象为Project内的多项Object=
                        SelectionState = SelectionStates.ProjectMulitpleObject;

                    }
                    else
                    {
                        //=选取对象为Project内的单项Object=
                        SelectionState = SelectionStates.ProjectSingleObject;
                    }
                }
                else
                {
                    //剔除Project选择全部为GameObject的可能性
                    if (Selection.activeGameObject.scene.name == null)
                    {//选择目标Scene名称为空则选择对象为Project内的GameObject

                        if (Selection.gameObjects.Length > 1)
                        {
                            //=选取对象为Project内的多项GameObject=
                            SelectionState = SelectionStates.ProjectMulitpleGameObject;
                        }
                        else
                        {
                            //=选取对象为Project内的单项GameObject=
                            SelectionState = SelectionStates.ProjectSingleGameObject;
                        }

                    }
                    else
                    {
                        //选择目标Scene名称不为空则选择对象为Hierarchy内的GameObject

                        if (Selection.gameObjects.Length > 1)
                        {
                            //=选取对象为Hierarchy内的多项GameObject=
                            SelectionState = SelectionStates.HierarchyMulitpleGameObject;
                        }
                        else
                        {
                            //=选取对象为Hierarchy内的单项GameObject=
                            SelectionState = SelectionStates.HierarchySingleGameObject;
                        }
                    }

                }
            }
            else
            {
                //=无对象选择=
                SelectionState = SelectionStates.Empty;
            }



            //快速菜单切换
            if (cState.fastMenu.enable)
            {
                GUILayout.Box(new GUIContent("快速操作菜单(未完成)"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.Height(24), GUILayout.ExpandWidth(true), /*GUILayout.ExpandHeight(true)*/ });



                //测试信息
                //if (Selection.activeGameObject != null)
                //{
                //    //GUILayout.Box(new GUIContent("SelectiongameObjects状态:"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                //    GUILayout.Box(new GUIContent(string.Format("SAG:{0}", Selection.activeGameObject.name)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                //    if (Selection.activeGameObject.scene != null)
                //    {
                //        //GUILayout.Box(new GUIContent("SelectiongameObjects状态:"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                //        GUILayout.Box(new GUIContent(string.Format("SAGSN:[{0}]", Selection.activeGameObject.scene.name)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                //        if (Selection.activeGameObject.scene.name == null)
                //        {
                //            GUILayout.Box(new GUIContent("SAGS.name:Null"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                //        }
                //    }
                //    else
                //        GUILayout.Box(new GUIContent("SAGS:null"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                //}
                //else
                //    GUILayout.Box(new GUIContent("SAG:null"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                //if (Selection.gameObjects != null)
                //{
                //    //GUILayout.Box(new GUIContent("SelectiongameObjects状态:"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                //    GUILayout.Box(new GUIContent(string.Format("SGO:{0}", Selection.gameObjects.Length)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                //}
                //if (Selection.objects != null)
                //{
                //    //GUILayout.Box(new GUIContent("SelectionObjects状态:"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                //    GUILayout.Box(new GUIContent(string.Format("SO:{0}", Selection.objects.Length)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.Height(22), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                //}
                //测试信息


                //GUILabel("----");

                //=====================================================
                //=====================================================

                //=选取对象为Project内的多项Object=
                void FastMenuProjectMulitpleObject()
                {
                    GUILayout.Box(new GUIContent(string.Format("=选取对象为Project内的多项Object=", Selection.objects.Length)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                    GUILayout.Box(new GUIContent(string.Format("已选取Project内的[{0}]项Objects", Selection.objects.Length)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });


                    //Project无法操作Transform对象还原prefab
                    //材质球类型检测
                    int i = 0, b = 0;
                    for (; i < Selection.objects.Length; i++)
                    {
                        if (Selection.objects[i] is Material)
                        {
                            b++;
                        }
                    }
                    if (i == b)
                    {
                        GUILayout.Box(new GUIContent(string.Format("所选的[{0}]项Obj皆为材质球(Material)", Selection.objects.Length)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                        GUILayout.Box(new GUIContent("可选操作"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                        InitializeMaterialFunction();
                    }
                    //Project内无Object动骨组件
                    //Project内无Object布料组件
                }

                //=选取对象为Project内的单项Object=
                void FastMenuProjectSingleObject()

                {
                    GUILayout.Box(new GUIContent(string.Format("=选取对象为Project内的单项Object=", Selection.activeObject.name)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                    GUILayout.Box(new GUIContent(string.Format("已选取Project内的Object:{0}", Selection.activeObject.name)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });


                    //Project无法操作Transform对象还原prefab
                    //材质球类型检测
                    if (Selection.activeObject is Material)
                    {
                        GUILayout.Box(new GUIContent(string.Format("所选[{0}]Obj为材质球(Material)", Selection.activeObject.name)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                        GUILayout.Box(new GUIContent("可选操作"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                        InitializeMaterialFunction();
                    }
                    //Project内无Object动骨组件
                    //Project内无Object布料组件

                }

                //=选取对象为Project内的多项GameObject=
                void FastMenuProjectMulitpleGameObject()
                {
                    GUILayout.Box(new GUIContent("=选取对象为Project内的多项GameObject="), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                    GUILayout.Box(new GUIContent(string.Format("已选取Project内的[{0}]项GameObjects", Selection.objects.Length)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                    //Project无法操作Transform对象还原prefab
                    //GameObject类型不包括材质球
                    //Project内无Object动骨组件
                    //Project内无Object布料组件
                }

                //=选取对象为Project内的单项GameObject=
                void FastMenuProjectSingleGameObject()

                {
                    GUILayout.Box(new GUIContent("=选取对象为Project内的单项GameObject="), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                    GUILayout.Box(new GUIContent(string.Format("已选取Project内的GameObject:{0}", Selection.activeObject.name)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                    //GameObject类型不包括材质球
                    //Project内无Object动骨组件
                    //Project内无Object布料组件
                }

                //=选取对象为Hierarchy内的多项GameObject=
                void FastMenuHierarchyMulitpleGameObject()

                {
                    GUILayout.Box(new GUIContent("=选取对象为Hierarchy内的多项GameObject="), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                    
                    GUILayout.Box(new GUIContent(string.Format("已选取Hierarchy内的[{0}]项GameObject", Selection.objects.Length)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                    if (GUILayout.Button("选定物体恢复Transform预制(prefab)状态"))
                    {
                        foreach (var go in Selection.gameObjects)
                        {

                            string toPath = GetGameObjectPath(go);
                            //var pref = PrefabUtility.GetPrefabParent(go.transform.root.gameObject) as GameObject;  //操作已过时 2017Unity > 2018Unity
                            var pref = PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(go.transform.root.gameObject);
                            Transform tr = pref.transform.Find(toPath);
                            go.transform.localPosition = tr.localPosition;
                            go.transform.localRotation = tr.localRotation;
                            go.transform.localScale = tr.localScale;
                        }

                    }

                    //Hierarchy内无材质球对象

                    //动骨筛选
                    int b = 0;
                    foreach (var gameObject in Selection.gameObjects)
                    {
                        if (gameObject.GetComponent<DynamicBone>())
                        {
                            b++;
                        }
                    }
                    if (b > 0)
                    {
                        GUILayout.Box(new GUIContent(string.Format("集合包含{0}项动骨组件", b)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                        InitializeDynamicBoneFunction();
                    }
                }


                //=选取对象为Hierarchy内的单项GameObject=
                void FastMenuHierarchySingleGameObject()

                {
                    GUILayout.Box(new GUIContent("=选取对象为Hierarchy内的单项GameObject="), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                    GUILayout.Box(new GUIContent(string.Format("已选取Hierarchy内的GameObject:{0}", Selection.activeObject.name)), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                    if (GUILayout.Button("选定物体恢复Transform预制(prefab)状态"))
                    {
                        foreach (var go in Selection.gameObjects)
                        {

                            string toPath = GetGameObjectPath(go);
                            //var pref = PrefabUtility.GetPrefabParent(go.transform.root.gameObject) as GameObject;  //操作已过时 2017Unity > 2018Unity
                            var pref = PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(go.transform.root.gameObject);
                            Transform tr = pref.transform.Find(toPath);
                            go.transform.localPosition = tr.localPosition;
                            go.transform.localRotation = tr.localRotation;
                            go.transform.localScale = tr.localScale;
                        }

                    }


                    //Hierarchy内无材质球对象

                    //动骨筛选
                    if (Selection.activeGameObject.GetComponent<DynamicBone>())
                    {
                        GUILayout.Box(new GUIContent(string.Format("目标含有动骨组件")), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                        InitializeDynamicBoneFunction();
                    }
                    if (Selection.activeGameObject.GetComponent<Cloth>())
                    {
                        GUILayout.Box(new GUIContent(string.Format("布料组件")), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                        //复制布料组件至同OBJ路径下"
                        if (GUILayout.Button("复制选择OBJ的[布料]组件至同AvatarOBJ路径下"))
                        {
                            //litFunctionButtonReplace(LitFunction.OpenAllDynamicBone);

                            foreach (var sGO in Selection.gameObjects)
                            {
                                GameObject gO = GetTargetTransforms(sGO.transform, true).gameObject;
                                if (gO.transform == cState.selectedAvatar.transform | gO.transform == Selection.activeTransform.root)
                                {
                                    Debug.Log(SSS + "未找到对应OBJ");
                                }
                                else
                                {
                                    FunctionClass.CopyCloth(sGO, gO, cState);
                                    //Debug.Log(SSS + "源" + sGO.transform.root.gameObject.name);
                                    //Debug.Log(SSS + "目标" + gO.transform.root.gameObject.name);
                                    //Debug.Log(SSS + "碰撞组件添加完毕");
                                }
                            }


                        }
                    }
                }


                //=====================================================
                //=====================================================


                switch (SelectionState)
                {
                    case SelectionStates.Empty:


                        GUILayout.Box(new GUIContent("=无选取对象="), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft }, new[] { GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });


                        break;
                    case SelectionStates.ProjectMulitpleObject:
                        //=选取对象为Project内的多项Object=
                        FastMenuProjectMulitpleObject();
                        break;
                    case SelectionStates.ProjectSingleObject:
                        //=选取对象为Project内的单项Object=
                        FastMenuProjectSingleObject();
                        break;
                    case SelectionStates.ProjectMulitpleGameObject:
                        //=选取对象为Project内的多项GameObject=
                        FastMenuProjectMulitpleGameObject();
                        break;
                    case SelectionStates.ProjectSingleGameObject:
                        //=选取对象为Project内的单项GameObject=
                        FastMenuProjectSingleGameObject();
                        break;
                    case SelectionStates.HierarchyMulitpleGameObject:
                        //=选取对象为Hierarchy内的多项GameObject=
                        FastMenuHierarchyMulitpleGameObject();
                        break;
                    case SelectionStates.HierarchySingleGameObject:
                        //=选取对象为Hierarchy内的单项GameObject=
                        FastMenuHierarchySingleGameObject();
                        break;
                    default:
                        break;
                }

                //
                GUILabel("----");


            }
            else
            {

                //
                //检查项 
                //======================================
                PreCheck();
                //======================================

                EditorGUILayout.Space();


                //>SV滚动控件开头
                vertScroll = EditorGUILayout.BeginScrollView(vertScroll);

                EditorGUILayout.Space();

                //======================================

                //材质球设置部分UI绘制
                InitializeMaterialFunction();
                //动骨相关功能UI绘制
                InitializeDynamicBoneFunction();
                //预制体碰撞添加部分UI绘制
                InitializePresetColliderFunction();
                //绑定功能部分UI绘制
                InitializeBindFunction();
                //杂项功能部分UI绘制
                InitializeLitFunction();
                //复制组件功能部分UI绘制
                InitializeCopierFunction();
                //======================================

                //SV<滚动控件结束
                EditorGUILayout.EndScrollView();

            }


            GUILabel("----");

            if (cState.fastMenu.enable)
            {
                if (GUILayout.Button(new GUIContent("使用传统菜单")))
                {
                    cState.fastMenu.enable = false;
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("使用快速菜单")))
                {
                    cState.fastMenu.enable = true;
                }
            }

            //关于工具包
            if (cState.misc.expand = GUILayout.Toggle(cState.misc.expand, LocalizationClass.Main.Misc, UIStyles.WhiteFirstFoldout_title))
            {
                //>SV滚动控件开头
                vertScroll_Versionlog = EditorGUILayout.BeginScrollView(vertScroll_Versionlog);
                GUILabel("Seadew Studios Tool Kit");
                GUILabel("蒸馏海工作组工具包");
                GUILabel("By - 命运为何(DTC_Destiny)");
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                GUILabel("感谢:  ");
                GUILabel("       山兔(Shatoo)");
                GUILabel("       触手(Yueou)");
                GUILabel("       拉狗子(_Lappland_)");
                GUILabel("       杨哥(Yangalen_610)");
                GUILabel("       禁忌(禁忌Apostle)");
                GUILabel("       咖喱给给(Currygeygey)");
                GUILabel("          等海露群友提供的思路代码等帮助...");
                //SV<滚动控件结束
                EditorGUILayout.EndScrollView();
            }
        }

        //================================================================================================⬆GUI窗口生成部分⬆===============================================================================================
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //=============================================================================================⬇GUI功能分类初始化函数⬇============================================================================================

        #region GUI功能分类初始化函数
        /// <summary>
        /// 对Avatar预检查项
        /// </summary>
        void PreCheck()
        {


            //============================================================================================================================================================================
            //PreCheck menu
            if (cState.preCheck.expand = GUILayout.Toggle(cState.preCheck.expand, /*LocalizationClass.Main.preCheck*/"预检查项", UIStyles.BlackFirstFoldout_title))
            {

                if (cState.selectedAvatar == null)
                {
                    GUILayout.Box(new GUIContent("没有选择目标Avatar"), new GUIStyle("flow node 4") { alignment = TextAnchor.MiddleCenter, }, new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.MaxHeight(30) });
                }
                else
                {
                    //========================================================================================================================================================================
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    //========================================================================================================================================================================
                    if (cState.preCheck.uploadRequired.expand = GUILayout.Toggle(cState.preCheck.uploadRequired.expand, "上传必须", UIStyles.WhiteSecondFoldout_title))
                    {
                        //EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                        //{
                        //    //Box分类项
                        //    GUILayout.Box(new GUIContent("Pre1"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.Height(38), GUILayout.Width(100), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                        //EditorGUILayout.BeginVertical();
                        //{
                        //[检测]目标Avatar是否包含AvatarDescriptor组件 并显示自动补充VRCD部分UI控件
                        if (FunctionClass.CheckAvatarDescriptor(cState.selectedAvatar))
                        {
                            //EditorGUILayout.HelpBox("模型(Avatar)缺少AvatarDescriptor组件！", MessageType.Error);
                            GUILayout.Box(new GUIContent("已包含AvatarDescriptor组件"), new GUIStyle("flow node 3") { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.MaxHeight(30) });
                        }
                        else
                        {
                            GUILayout.BeginVertical();
                            {
                                //EditorGUILayout.HelpBox("模型(Avatar)缺少AvatarDescriptor组件！", MessageType.Error);
                                GUILayout.Box(new GUIContent("模型(Avatar)缺少AvatarDescriptor组件！"), new GUIStyle("flow node 6") { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.MaxHeight(30) });

                                GUILayout.BeginHorizontal();
                                {

                                    //GUILayout.BeginVertical();

                                    cState.auto_Visemes = GUILayout.Toggle(cState.auto_Visemes, "自动口型");
                                    cState.auto_Viewpoint = GUILayout.Toggle(cState.auto_Viewpoint, "自动视角球");

                                    //GUILayout.EndVertical();


                                    if (GUILayout.Button(new GUIContent("自动添加AvatarDescriptor组件", Icons.CollabCreateIcon)))
                                    {
                                        var sAvatarVRCAD = cState.selectedAvatar.AddComponent<VRCAvatarDescriptor>();
                                        var sAvatarAnimator = cState.selectedAvatar.GetComponent<Animator>();
                                        var sAvatarSMR = cState.selectedAvatar.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
                                        sAvatarVRCAD.Animations = VRCAvatarDescriptor.AnimationSet.Female;//默认为女性
                                        if (cState.auto_Visemes)
                                        {
                                            FunctionClass.FillVisemes(cState.selectedAvatar);
                                        }
                                        if (cState.auto_Viewpoint)
                                        {
                                            FunctionClass.BeginEditViewpoint(cState.selectedAvatar);
                                        }
                                        sAvatarVRCAD.enableEyeLook = true;
                                        sAvatarVRCAD.customEyeLookSettings.leftEye = sAvatarAnimator.GetBoneTransform(HumanBodyBones.LeftEye);
                                        sAvatarVRCAD.customEyeLookSettings.rightEye = sAvatarAnimator.GetBoneTransform(HumanBodyBones.RightEye);

                                        var eyeRup = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations();
                                        eyeRup.right = eyeRup.left;
                                        eyeRup.left = Quaternion.Euler(new Vector3(-20, 0, 0));
                                        var eyeRDown = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations();
                                        eyeRDown.right = eyeRDown.left;
                                        eyeRDown.left = Quaternion.Euler(new Vector3(25, 0, 0));
                                        var eyeRLeft = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations();
                                        eyeRLeft.right = eyeRLeft.left;
                                        eyeRLeft.left = Quaternion.Euler(new Vector3(0, 22, 0));
                                        var eyeRRight = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations();
                                        eyeRRight.right = eyeRRight.left;
                                        eyeRRight.left = Quaternion.Euler(new Vector3(0, -22, 0));

                                        sAvatarVRCAD.customEyeLookSettings.eyesLookingUp = eyeRup;
                                        sAvatarVRCAD.customEyeLookSettings.eyesLookingDown = eyeRDown;
                                        sAvatarVRCAD.customEyeLookSettings.eyesLookingLeft = eyeRLeft;
                                        sAvatarVRCAD.customEyeLookSettings.eyesLookingRight = eyeRRight;
                                        sAvatarVRCAD.customEyeLookSettings.eyelidType = VRCAvatarDescriptor.EyelidType.Blendshapes;
                                        sAvatarVRCAD.customEyeLookSettings.eyelidsSkinnedMesh = sAvatarSMR;

                                        int bs20index = sAvatarSMR.sharedMesh.GetBlendShapeIndex("まばたき 20");
                                        int bsindex = sAvatarSMR.sharedMesh.GetBlendShapeIndex("まばたき");
                                        
                                            sAvatarVRCAD.customEyeLookSettings.eyelidsBlendshapes = new int[] { bsindex, -1, bs20index };
                                        //d.VisemeBlendShapes[z] = s;


                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                        }

                        //}
                        //EditorGUILayout.EndVertical();
                        //}
                        //EditorGUILayout.EndHorizontal();
                    }

                    //========================================================================================================================================================================
                    EditorGUILayout.Space();
                    //========================================================================================================================================================================

                    if (cState.preCheck.general.expand = GUILayout.Toggle(cState.preCheck.general.expand, "常规项", UIStyles.WhiteSecondFoldout_title))
                    {

                        //[检测项]BodyOBJ包含Animator组件 眨眼动画控制器检测 并显示自动补充Animator组件附带预设眨眼控制器UI控件
                        if (cState.selectedAvatar != null)
                        {
                            //Animator comA = cState.selectedAvatar.transform.Find("Body").GetComponent<Animator>();
                            var tF = cState.selectedAvatar.transform.Find("Body");
                            if (tF != null)
                            {





                                //if (tF.GetComponent<Animator>() == null)
                                //{
                                //    EditorGUILayout.HelpBox("Body无Animator组件,可能缺失眨眼动画", MessageType.Warning);
                                //    if (GUILayout.Button(new GUIContent("添加预设100%状态眨眼", Icons.CsScript)))
                                //    {
                                //        Animator bodyAnimator = tF.gameObject.AddComponent<Animator>();

                                //        bodyAnimator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Blink Animator\100% slow\Body.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
                                //    }
                                //}
                                //else
                                //{
                                //    if (tF.GetComponent<Animator>().runtimeAnimatorController == null)
                                //    {
                                //        EditorGUILayout.HelpBox("Body的Animator组件的runtimeAnimatorController为空,可能缺失眨眼动画", MessageType.Warning);
                                //        if (GUILayout.Button(new GUIContent("添加预设100%状态眨眼", Icons.CsScript)))
                                //        {


                                //            tF.GetComponent<Animator>().runtimeAnimatorController = AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Blink Animator\100% slow\Body.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
                                //        }
                                //    }

                                //}

                                //[检测]SDKfutureProofPublish项
                                if (EditorPrefs.GetBool("futureProofPublish"))
                                {
                                    EditorGUILayout.HelpBox("检测到VRCSDK开启了Future Proof Publish项\n会影响Avatar上传,建议关闭.", MessageType.Warning);
                                    if (GUILayout.Button(new GUIContent("关闭Future Proof Publish", Icons.CsScript)))
                                    {
                                        EditorPrefs.SetBool("futureProofPublish", false);
                                    }
                                }
                                else
                                {

                                }
                            }
                        }


                        //Box分割线---------------------------------------------------------------------------------------------------------------------------------------------------
                        GUILayout.Box(new GUIContent(""), new[] { GUILayout.Height(1), GUILayout.ExpandWidth(true) });
                        //Box分割线---------------------------------------------------------------------------------------------------------------------------------------------------

                        //[检测项]AvatarDescriptor组件 
                        if (cState.selectedAvatar != null & AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\PresetCustomStandingAnimatorController.overrideController", typeof(AnimatorOverrideController)) != null)
                        {
                            if (FunctionClass.CheckAvatarDescriptor(cState.selectedAvatar))
                            {
                                //Animator comA = cState.selectedAvatar.transform.Find("Body").GetComponent<Animator>();
                                //var sAvatarVRCAD = cState.selectedAvatar.transform.GetComponent<VRCAvatarDescriptor>();
                                //if (sAvatarVRCAD.CustomStandingAnims == null)
                                //{
                                //    EditorGUILayout.HelpBox("VRCAvatarDescriptor组件的站立自定义动画控制器(CustomStandingAnims)为空\n可能缺失动画控制器", MessageType.Warning);
                                //    if (GUILayout.Button(new GUIContent("添加预设动画控制器组件附带表情", Icons.CsScript)))
                                //    {
                                //        sAvatarVRCAD.CustomStandingAnims = AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\PresetCustomStandingAnimatorController.overrideController", typeof(AnimatorOverrideController)) as AnimatorOverrideController;
                                //    }
                                //}
                            }
                        }




                    }

                    //========================================================================================================================================================================
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    //========================================================================================================================================================================
                }

            }



            //selectedAvatar项
            if (cState.selectedAvatar != null)
            {

                //这不应当-目标Avatar不为空时 场景名称为空
                if (cState.selectedAvatar.gameObject.scene.name == null)
                {
                    FunctionClass.Log(LocalizationClass.Warning.SelectSceneObject, LogType.Warning);
                    cState.selectedAvatar = null;
                }

            }

            //功能测试
            //if (EditorPrefs.GetBool("futureProofPublish"))
            //{
            //    GUILabel("futureProofPublish开启");
            //}
            //else
            //{
            //    GUILabel("futureProofPublish关闭");
            //}


        }


        /// <summary>
        /// [UI控件初始化]模块功能加载
        /// </summary>
        void ModularFunctionManage()
        {

        }

        /// <summary>
        /// [UI控件初始化]材质球相关操作
        /// </summary>
        void InitializeMaterialFunction()
        {
            //============================================================================================================================================================================
            //MaterialFunction menu
            if (cState.materialFunction.expand = GUILayout.Toggle(cState.materialFunction.expand, LocalizationClass.Main.MaterialFunction, UIStyles.BlackFirstFoldout_title))
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(sFoldSpaceWidth);
                EditorGUILayout.BeginVertical();
                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

                EditorGUILayout.BeginVertical(UIStyles.boxGuiStyle);
                if (cState.materialFunction.matAll.expand = GUILayout.Toggle(cState.materialFunction.matAll.expand, "通用Shader操作",UIStyles.WhiteSecondFoldout_title))
                {
                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        //Box分类项
                        GUILayout.Box(new GUIContent("操作优化"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.Height(38), GUILayout.Width(100), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                        EditorGUILayout.BeginVertical();
                        {
                            if (GUILayout.Button("设置材质球[主贴图]颜色[默认灰变白]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(SSS + "设置目标材质球<" + activeObject.name + ">主贴图颜色[白]");
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        if (mat.color == new Color(204f / 255f, 204f / 255f, 204f / 255f))
                                        {
                                            mat.SetColor("_Color", new Color(1f, 1f, 1f));
                                        }
                                        Debug.Log(SSS + "跳过目标材质球<" + activeObject.name + ">");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球[Emission贴图]使用主贴图(MainTex)"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {

                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(SSS + "设置目标材质球<" + activeObject.name + ">Emission贴图(UseMainTex)");
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.SetTexture("_EmissionMap", mat.mainTexture);
                                    }
                                }
                                #region Shader部分数值
                                // Gloss
                                //[ATSToggle] _UseGloss("[Gloss] Enabled", Int) = 0
                                //_GlossBlend("[Gloss] Smoothness", Range(0, 1)) = 0.5
                                //_GlossBlendMask("[Gloss] Smoothness Mask", 2D) = "white" {}
                                //_GlossPower("[Gloss] Metallic", Range(0, 1)) = 0.5
                                //_GlossColor("[Gloss] Color", Color) = (1,1,1,1)
                                #endregion
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

                EditorGUILayout.BeginVertical(UIStyles.boxGuiStyle);
                if (cState.materialFunction.matArktoon.expand = GUILayout.Toggle(cState.materialFunction.matArktoon.expand, "Arktoon Shader", UIStyles.WhiteSecondFoldout_title))
                {
                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        //Box分类项
                        GUILayout.Box(new GUIContent("设置RenderMode"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.Height(100),GUILayout.Width(100),GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                        EditorGUILayout.BeginVertical();
                        {
                            if (GUILayout.Button("设置材质球为[Opaque]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString("arktoon/Opaque", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find("arktoon/Opaque");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[Fade]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString("arktoon/Fade", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find("arktoon/Fade");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[FadeRefracted]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString("arktoon/FadeRefracted", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find("arktoon/FadeRefracted");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[AlphaCutout]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString("arktoon/AlphaCutout", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find("arktoon/AlphaCutout");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[_Extra/EmissiveFreak]类"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    Material mat = activeObject as Material;
                                    if (mat.shader.name.IndexOf("arktoon") > -1)
                                    {
                                        if (mat.shader.name.IndexOf("EmissiveFreak") == -1)
                                        {

                                            //Debug.Log("目标>" + Selection.activeGameObject.name);
                                            string[] matSplit = mat.shader.name.Split('/');
                                            //foreach (var item in matSplit)
                                            //{
                                            //    if (item != "EmissiveFreak")
                                            //    {


                                            //    }
                                            //    break;
                                            //}
                                            mat.shader = Shader.Find("arktoon/_Extra/EmissiveFreak/" + matSplit[matSplit.Length - 1]);
                                            Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString("arktoon/_Extra/EmissiveFreak/" + matSplit[matSplit.Length - 1], LogColor.DarkPurple)));
                                        }

                                        //
                                        //if (mat is Material)
                                        //{
                                        //    mat.shader = Shader.Find("arktoon/AlphaCutout");
                                        //}
                                        else
                                        {
                                            Debug.LogWarning(string.Format(SSS + "目标材质球<{0}>为EmissiveFreak,已跳过此设置操作", tagString(activeObject.name, LogColor.Orange)));
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogWarning(string.Format(SSS + "目标材质球<{0}>使用Shader不为Arktoon,已跳过此设置操作", tagString(activeObject.name, LogColor.Orange)));
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();

                    //Box分割线---------------------------------------------------------------------------------------------------------------------------------------------------
                    GUILayout.Box(new GUIContent(""), new[] { GUILayout.Height(1), GUILayout.ExpandWidth(true) });
                    //Box分割线---------------------------------------------------------------------------------------------------------------------------------------------------

                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        GUILayout.Box(new GUIContent("预设参数"),new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter}, new[] { GUILayout.Height(100), GUILayout.Width(100), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                        EditorGUILayout.BeginVertical();
                        {
                            if (GUILayout.Button("预设阴影参数"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>的Arktoon阴影", tagString(activeObject.name, LogColor.Orange)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        if (mat.shader.name.IndexOf("arktoon/") > -1)
                                        {
                                            //mat.shader = Shader.Find("arktoon/Opaque");
                                            //阴影
                                            mat.SetFloat("_Shadowborder", 0.75f);
                                            mat.SetFloat("_ShadowStrength", 1f);
                                            mat.SetFloat("_ShadowborderBlur", 0.33f);
                                            //1层阴影
                                            mat.SetInt("_ShadowPlanBUsePlanB", 1);
                                            mat.SetFloat("_ShadowPlanBDefaultShadowMix", .45f);
                                            //2层阴影
                                            mat.SetInt("_CustomShadow2nd", 1);
                                            mat.SetFloat("_ShadowPlanB2border", 0.65f);
                                            mat.SetFloat("_ShadowPlanB2borderBlur", 0.63f);
                                            mat.SetFloat("_ShadowPlanB2ValueFromBase", 2f);

                                            //点光源
                                            mat.SetInt("_LightSampling", 1);//Arktoon 0      Cube 1
                                            mat.SetFloat("_PointAddIntensity", 1f);
                                            mat.SetFloat("_PointShadowStrength", .5f);
                                            mat.SetFloat("_PointShadowborder", .5f);
                                            mat.SetFloat("_PointShadowborderBlur", .6f);


                                            //mat.SetFloat("_UseDoubleSided", 1.0f);//双面
                                        }
                                    }
                                }
                            }
                            if (GUILayout.Button("去除阴影参数"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "去除目标材质球<{0}>的Arktoon阴影", tagString(activeObject.name, LogColor.Orange)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        if (mat.shader.name.IndexOf("arktoon/") > -1)
                                        {
                                            //mat.shader = Shader.Find("arktoon/Opaque");
                                            //阴影
                                            mat.SetFloat("_Shadowborder", 0f);
                                            mat.SetFloat("_ShadowStrength", 0f);
                                            mat.SetFloat("_ShadowborderBlur", 0f);
                                            //1层阴影
                                            mat.SetInt("_ShadowPlanBUsePlanB", 0);
                                            //mat.SetFloat("_ShadowPlanBDefaultShadowMix", .45f);
                                            //2层阴影
                                            mat.SetInt("_CustomShadow2nd", 0);
                                            //mat.SetFloat("_ShadowPlanB2border", 0.65f);
                                            //mat.SetFloat("_ShadowPlanB2borderBlur", 0.63f);
                                            //mat.SetFloat("_ShadowPlanB2ValueFromBase", 2f);

                                            mat.SetInt("_LightSampling", 1);//Arktoon 0      Cube 1
                                                                            //点光源
                                            mat.SetFloat("_PointAddIntensity", 0f);
                                            mat.SetFloat("_PointShadowStrength", 0f);
                                            mat.SetFloat("_PointShadowborder", 0f);
                                            mat.SetFloat("_PointShadowborderBlur", 0f);


                                            //mat.SetFloat("_UseDoubleSided", 1.0f);//双面
                                        }
                                    }
                                }
                            }
                            if (GUILayout.Button("去除阴影参数&重命名"))
                            {
                                int i = -1;
                                foreach (var activeObject in Selection.objects)
                                {
                                    i++;
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "去除目标材质球<{0}>的Arktoon阴影", tagString(activeObject.name, LogColor.Orange)));
                                    Material mat = activeObject as Material;
                                    string matPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);
                                    string newMatPath = matPath.Replace(".mat", "(NoShadow).mat");
                                    Debug.Log(string.Format(SSS + "重命名目标材质球<{0}>\n路径:{1}", tagString(activeObject.name, LogColor.Orange), tagString(matPath, LogColor.DarkPurple)));
                                    //Debug.Log(SSS + "重命名路径:" + newMatPath);
                                    string info = AssetDatabase.RenameAsset(matPath, newMatPath.Split('/')[newMatPath.Split('/').Length - 1]);
                                    if (info != string.Empty)
                                    {
                                        Debug.Log(string.Format(SSS + "目标材质球<{0}>命名失败\n错误信息:{1}", tagString(activeObject.name, LogColor.Orange), tagString(info, LogColor.DarkPurple)));
                                        break;
                                    }
                                    //mat.name = mat.name + "(NoShadow)"; //仅改变材质球名称 文件名称无法改变
                                    if (mat is Material)
                                    {
                                        if (mat.shader.name.IndexOf("arktoon/") > -1)
                                        {
                                            //mat.shader = Shader.Find("arktoon/Opaque");
                                            //阴影
                                            mat.SetFloat("_Shadowborder", 0f);
                                            mat.SetFloat("_ShadowStrength", 0f);
                                            mat.SetFloat("_ShadowborderBlur", 0f);
                                            //1层阴影
                                            mat.SetInt("_ShadowPlanBUsePlanB", 0);
                                            //mat.SetFloat("_ShadowPlanBDefaultShadowMix", .45f);
                                            //2层阴影
                                            mat.SetInt("_CustomShadow2nd", 0);
                                            //mat.SetFloat("_ShadowPlanB2border", 0.65f);
                                            //mat.SetFloat("_ShadowPlanB2borderBlur", 0.63f);
                                            //mat.SetFloat("_ShadowPlanB2ValueFromBase", 2f);

                                            mat.SetInt("_LightSampling", 1);//Arktoon 0      Cube 1
                                                                            //点光源
                                            mat.SetFloat("_PointAddIntensity", 0f);
                                            mat.SetFloat("_PointShadowStrength", 0f);
                                            mat.SetFloat("_PointShadowborder", 0f);
                                            mat.SetFloat("_PointShadowborderBlur", 0f);


                                            //mat.SetFloat("_UseDoubleSided", 1.0f);//双面
                                        }
                                    }
                                }
                            }
                            if (GUILayout.Button("设置预设[皮肤]Gloss参数"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {

                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(SSS + "设置目标材质球<" + activeObject.name + ">Gloss皮肤预设");
                                    Material mat = activeObject as Material;
                                    if (mat.shader.name.IndexOf("arktoon/") > -1)
                                    {

                                        if (mat is Material)
                                        {
                                            mat.SetInt("_UseGloss", 1);
                                            mat.SetFloat("_GlossBlend", .2f);
                                            mat.SetFloat("_GlossPower", .4f);
                                        }
                                    }
                                }
                                #region Shader部分数值
                                // Gloss
                                //[ATSToggle] _UseGloss("[Gloss] Enabled", Int) = 0
                                //_GlossBlend("[Gloss] Smoothness", Range(0, 1)) = 0.5
                                //_GlossBlendMask("[Gloss] Smoothness Mask", 2D) = "white" {}
                                //_GlossPower("[Gloss] Metallic", Range(0, 1)) = 0.5
                                //_GlossColor("[Gloss] Color", Color) = (1,1,1,1)
                                #endregion
                            }
                            if (GUILayout.Button("[Face]分类预设生成材质球"))
                    {
                        foreach (var activeObject in Selection.objects)
                        {
                            //Debug.Log("目标>" + Selection.activeGameObject.name);
                            Debug.Log(string.Format(SSS + "设置目标材质球<{0}>的脸部预设", tagString(activeObject.name, LogColor.Orange)));
                            string matPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

                            if ((activeObject as Material).shader.name.IndexOf("arktoon/") > -1)
                            {
                                //OpaqueEmission
                                string opemPath = matPath.Replace(".mat", " OpE.mat");
                                Material opemMat = Material.Instantiate((activeObject as Material));
                                opemMat.SetTexture("_EmissionMap", opemMat.mainTexture);
                                opemMat.SetColor("_EmissionColor", new Color(.6f, .6f, .6f));
                                AssetDatabase.CreateAsset(opemMat, opemPath);

                                //Fade
                                string fadePath = matPath.Replace(".mat", " Fade.mat");
                                Material fadeMat = Material.Instantiate((activeObject as Material));
                                fadeMat.shader = Shader.Find("arktoon/Fade");
                                fadeMat.SetInt("_ZWrite", 0);//Off, 0, On, 1
                                AssetDatabase.CreateAsset(fadeMat, fadePath);

                                //FadeEmission
                                string fadeEPath = matPath.Replace(".mat", " FadeE.mat");
                                Material fadeEMat = Material.Instantiate((activeObject as Material));
                                fadeEMat.shader = Shader.Find("arktoon/Fade");
                                fadeEMat.SetTexture("_EmissionMap", opemMat.mainTexture);
                                fadeEMat.SetColor("_EmissionColor", new Color(1f, 1f, 1f));
                                fadeEMat.SetInt("_ZWrite", 0);//Off, 0, On, 1
                                AssetDatabase.CreateAsset(fadeEMat, fadeEPath);


                                Debug.Log(SSS + "目标路径设置完成<" + matPath + ">");
                                //Material mat = activeObject as Material;
                                //if (mat is Material)
                                //{
                                //    mat.SetInt("_UseGloss", 1);
                                //    mat.SetFloat("_GlossBlend", .2f);
                                //    mat.SetFloat("_GlossPower", .4f);
                                //}
                            }
                        }
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (cState.materialFunction.matPoiyomi.expand = GUILayout.Toggle(cState.materialFunction.matPoiyomi.expand, "Poiyomi Shader V4.3", UIStyles.WhiteSecondFoldout_title))
                {
                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        //Box分类项
                        GUILayout.Box(new GUIContent("设置RenderMode"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.Height(100), GUILayout.Width(100), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                        EditorGUILayout.BeginVertical();
                        {
                            if (GUILayout.Button("设置材质球为[Opaque]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString(".poiyomi/Toon/Default/Opaque", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find(".poiyomi/Toon/Default/Opaque");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[Cutout]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString(".poiyomi/Toon/Default/Cutout", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find(".poiyomi/Toon/Default/Cutout");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[Transparent]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString(".poiyomi/Toon/Default/Transparent", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find(".poiyomi/Toon/Default/Transparent");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[outlines Cutout]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString(".poiyomi/Toon/Default/outlines Cutout", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find(".poiyomi/Toon/Default/outlines Cutout");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[outlines Transparent]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString(".poiyomi/Toon/Default/outlines Transparent", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find(".poiyomi/Toon/Default/outlines Transparent");
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();

                }
                if (cState.materialFunction.matPoiyomi.expand = GUILayout.Toggle(cState.materialFunction.matPoiyomi.expand, "Poiyomi Shader V6.0", UIStyles.WhiteSecondFoldout_title))
                {
                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        //Box分类项
                        GUILayout.Box(new GUIContent("设置RenderMode"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.Height(20*5), GUILayout.Width(100), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                        EditorGUILayout.BeginVertical();
                        {
                            if (GUILayout.Button("设置材质球为[Opaque]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString(".poiyomi/Toon/Default/Opaque", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find(".poiyomi/Toon/Advanced/Opaque");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[Cutout]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString(".poiyomi/Toon/Default/Cutout", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find(".poiyomi/Toon/Advanced/Cutout");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[Transparent]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString(".poiyomi/Toon/Default/Transparent", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find(".poiyomi/Toon/Advanced/Transparent");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[outlines Cutout]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString(".poiyomi/Toon/Default/outlines Cutout", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find(".poiyomi/Toon/Advanced/outlines Cutout");
                                    }
                                }
                            }
                            if (GUILayout.Button("设置材质球为[outlines Transparent]"))
                            {
                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>为{1}", tagString(activeObject.name, LogColor.Orange), tagString(".poiyomi/Toon/Default/outlines Transparent", LogColor.DarkPurple)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        mat.shader = Shader.Find(".poiyomi/Toon/Advanced/outlines Transparent");
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        //Box分类项
                        GUILayout.Box(new GUIContent("预设参数"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.Height(20*5), GUILayout.Width(100), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                        EditorGUILayout.BeginVertical();
                        {
                            if (GUILayout.Button("设置预设光照参数[Light]"))
                            {

                                foreach (var activeObject in Selection.objects)
                                {
                                    //Debug.Log("目标>" + Selection.activeGameObject.name);
                                    Debug.Log(string.Format(SSS + "设置目标材质球<{0}>的[6.0]Poiyomi光照参数 (未完成)", tagString(activeObject.name, LogColor.Orange)));
                                    Material mat = activeObject as Material;
                                    if (mat is Material)
                                    {
                                        if (mat.shader.name.IndexOf(".poiyomi/") > -1)
                                        {
                                            //mat.shader = Shader.Find(".poiyomi/Toon/Advanced/Opaque");

                                            // Lighting
                                            //[HideInInspector] m_lightingOptions("Lighting", Float) = 0
                                            //[HideInInspector] m_start_Lighting("Light and Shadow", Float) = 0

                                            //==Enable Lighting== 开关
                                            //[Toggle(LOD_FADE_CROSSFADE)] _EnableLighting("Enable Lighting", Float) = 1
                                            mat.SetFloat("_Shadowborder", 1f);
                                            //==LightingType==
                                            //[Enum(Natural, 0, Controlled, 1, Standardish, 2, Math, 3)] _LightingType("Lighting Type", Int) = 1
                                            mat.SetInt("_LightingType", 0);
                                            //==LightingRamp==
                                            //[Gradient] _ToonRamp("Lighting Ramp", 2D) = "white" { }
                                            mat.SetTexture("_ToonRamp", null);
                                            //==ShadowMask==
                                            //_LightingShadowMask("Shadow Mask (RGBA)", 2D) = "white" { }
                                            mat.SetTexture("_LightingShadowMask", null);

                                            //[HideInInspector] [Vector2] _LightingShadowMaskPan("Panning", Vector) = (0, 0, 0, 0)
                                            //[HideInInspector] [Enum(UV0, 0, UV1, 1, UV2, 2, UV3, 3, DistortedUV1, 4)] _LightingShadowMaskUV("UV", Int) = 0

                                            //==Shadow Strength==
                                            //_ShadowStrength("Shadow Strength", Range(0, 1)) = .2
                                            mat.SetFloat("_ShadowStrength", .2f);
                                            //==Shadow Offset==
                                            //_ShadowOffset("Shadow Offset", Range(-1, 1)) = 0
                                            mat.SetFloat("_ShadowOffset", 0f);
                                            //==AO Map==
                                            //_LightingAOTex("AO Map", 2D) = "white" { }
                                            mat.SetTexture("_LightingAOTex", null);

                                            //[HideInInspector] [Vector2] _LightingAOTexPan("Panning", Vector) = (0, 0, 0, 0)
                                            //[HideInInspector] [Enum(UV0, 0, UV1, 1, UV2, 2, UV3, 3, DistortedUV1, 4)] _LightingAOTexUV("UV", Int) = 0

                                            //==AO Strength==
                                            //_AOStrength("AO Strength", Range(0, 1)) = 0
                                            mat.SetFloat("_AOStrength", 0f);

                                            //==Min Brightness==
                                            //_LightingMinLightBrightness("Min Brightness", Range(0, 1)) = 0
                                            mat.SetFloat("_LightingMinLightBrightness", 0f);

                                            //==Indirect Contribution==
                                            //_LightingIndirectContribution("Indirect Contribution", Range(0, 1)) = .2
                                            mat.SetFloat("_LightingIndirectContribution", .2f);


                                            //==Indirect Contribution==
                                            //_AttenuationMultiplier("Recieve Casted Shadows?", Range(0, 1)) = 0
                                            mat.SetFloat("_AttenuationMultiplier", 0f);

                                            //==Indirect Contribution==
                                            //_LightingDetailShadows("Detail Shadows", 2D) = "white" { }
                                            mat.SetTexture("_LightingDetailShadows", null);

                                            //[HideInInspector] [Vector2] _LightingDetailShadowsPan("Panning", Vector) = (0, 0, 0, 0)
                                            //[HideInInspector] [Enum(UV0, 0, UV1, 1, UV2, 2, UV3, 3, DistortedUV1, 4)] _LightingDetailShadowsUV("UV", Int) = 0

                                            //==Indirect Contribution==
                                            //_LightingDetailStrength("Detail Strength", Range(0, 1)) = 1
                                            mat.SetFloat("_LightingDetailStrength", 1f);

                                            //[HideInInspector] m_start_lightingStandard("Standardish Settings", Float) = 0

                                            //==Indirect Contribution==
                                            //_LightingStandardSmoothness("Smoothness", Range(0, 1)) = 0
                                            mat.SetFloat("_LightingStandardSmoothness", 0f);

                                            //[HideInInspector] m_end_lightingStandard("Standardish Settings", Float) = 0

                                        }
                                    }
                                }

                            }
                            EditorGUI.BeginDisabledGroup(true);
                            {
                                if (GUILayout.Button("----"))
                                {
                                }
                                if (GUILayout.Button("----"))
                                {
                                }
                                if (GUILayout.Button("----"))
                                {
                                }
                                if (GUILayout.Button("----"))
                                {
                                }
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();

                }
                EditorGUILayout.EndVertical();

                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================
                EditorGUILayout.EndVertical();
                GUILayout.Space(sFoldSpaceWidth);
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// [UI控件初始化]动骨组件操作功能
        /// </summary>
        void InitializeDynamicBoneFunction()
        {

            //============================================================================================================================================================================
            //DynamicBoneFunction menu
            if (cState.dynamicBoneFunction.expand = GUILayout.Toggle(cState.dynamicBoneFunction.expand, "动骨操作[DynamicBone]", UIStyles.BlackFirstFoldout_title))
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(sFoldSpaceWidth);
                EditorGUILayout.BeginVertical();

                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

                EditorGUILayout.BeginVertical(UIStyles.boxGuiStyle);
                if (cState.dynamicBoneFunction.other.expand = GUILayout.Toggle(cState.dynamicBoneFunction.other.expand, "单项功能", UIStyles.WhiteSecondFoldout_title))
                {

                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        //Box分类项
                        GUILayout.Box(new GUIContent("对选择的\nGameObject操作"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.Height(140), GUILayout.Width(100), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });
                        
                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUI.BeginDisabledGroup(SelectionState==SelectionStates.Empty);
                            {


                                //[Button]为选择的对象添加动骨组件
                                if (GUILayout.Button(new GUIContent("添加动骨组件", Icons.CollabCreateIcon)))
                                {
                                    litFunctionButtonReplace(LitFunction.SelectedOBJAddDynamicBone);
                                }
                                //---------------------------------

                                EditorGUI.BeginDisabledGroup(!SelectionCheckComponent<DynamicBone>());
                                {
                                    //[Button]将选择对象的动骨组件Root骨设置为自身
                                    if (GUILayout.Button(new GUIContent(LocalizationClass.litFunction.SelectedDynamicBoneSetRoot, Icons.BoneIcon)))
                                    {
                                        litFunctionButtonReplace(LitFunction.SelectedDynamicBoneSetRoot);
                                    }
                                    //---------------------------------


                                    //[Button]复制选择目标包含的动骨组件至同目标Avatar的同路径GameObject下"
                                    if (GUILayout.Button(new GUIContent("复制动骨组件至目标Avatar同路径下", Icons.GameObject)))
                                    {
                                    //litFunctionButtonReplace(LitFunction.OpenAllDynamicBone);

                                    foreach (var sGO in Selection.gameObjects)
                                    {
                                        GameObject gO = GetTargetTransforms(sGO.transform).gameObject;
                                        if (gO.transform == cState.selectedAvatar.transform | gO.transform == Selection.activeTransform.root)
                                        {
                                            Debug.Log(SSS + "未找到对应OBJ");
                                        }
                                        else
                                        {
                                            FunctionClass.CopyDynamicBones(sGO, gO, cState);
                                            //Debug.Log(SSS + "源" + sGO.transform.root.gameObject.name);
                                            //Debug.Log(SSS + "目标" + gO.transform.root.gameObject.name);
                                            //Debug.Log(SSS + "动骨组件添加完毕");
                                        }
                                    }


                                    }
                                    //---------------------------------


                                    //[Button]关闭选择对象的所有动骨组件
                                    if (GUILayout.Button(new GUIContent("查看碰撞关联生效部位[单项]", Icons.SearchIcon)))
                                    {
                                        var tDB = Selection.activeGameObject.GetComponent<DynamicBone>();
                                        if (tDB)
                                        {
                                            Selection.objects = tDB.m_Colliders.ToArray();
                                        }

                                    }
                                    //---------------------------------
                                }
                                EditorGUI.EndDisabledGroup();

                                //[Button]开启选择对象的所有动骨组件
                                if (GUILayout.Button(new GUIContent("开启动骨", Icons.Enable)))
                                {
                                    litFunctionButtonReplace(LitFunction.OpenOBJDynamicBone);
                                }
                                //---------------------------------

                                //[Button]关闭选择对象的所有动骨组件
                                if (GUILayout.Button(new GUIContent("关闭动骨", Icons.Disable)))
                                {
                                    litFunctionButtonReplace(LitFunction.CloseOBJDynamicBone);
                                }
                                //---------------------------------
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();

                    //Box分割线---------------------------------------------------------------------------------------------------------------------------------------------------
                    GUILayout.Box(new GUIContent(""), new[] { GUILayout.Height(1), GUILayout.ExpandWidth(true) });
                    //------------------------------------------------------------------------------------------------------------------------------------------------------------

                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        //Box分类项
                        GUILayout.Box(new GUIContent("全局操作"), new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter }, new[] { GUILayout.Height(38), GUILayout.Width(100), GUILayout.ExpandWidth(false), /*GUILayout.ExpandHeight(true)*/ });

                        EditorGUILayout.BeginVertical();
                        {

                            //[Button]打开Avatar所有动骨
                            if (GUILayout.Button(new GUIContent("打开所有动骨", Icons.Enable)))
                            {
                                litFunctionButtonReplace(LitFunction.OpenAllDynamicBone);
                            }
                            //---------------------------------
                            //[Button]关闭Avatar所有动骨
                            if (GUILayout.Button(new GUIContent("关闭所有动骨", Icons.Disable)))
                            {
                                litFunctionButtonReplace(LitFunction.CloseAllDynamicBone);
                            }
                            //---------------------------------

                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();

                }
                EditorGUILayout.EndVertical();

                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

                /*
                if (cState.dynamicBoneFunction.prefab.expand = GUILayout.Toggle(cState.dynamicBoneFunction.prefab.expand, "预制动骨参数填充", UIStyles.Foldout_title))
                {
                    GUILabel("前发:", 11, TextAnchor.MiddleLeft, FontStyle.Bold);
                }
                */
                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

                EditorGUILayout.BeginVertical(UIStyles.boxGuiStyle);
                if (cState.dynamicBoneFunction.skirtCollider.expand = GUILayout.Toggle(cState.dynamicBoneFunction.skirtCollider.expand, "动骨裙用腿部[内碰撞球]生成", UIStyles.WhiteSecondFoldout_title))
                {



                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        GUILayout.Box(new GUIContent("生成数量"));
                            cState.dynamicBoneFunction.skirtCollider.count = GUILayout.TextField(cState.dynamicBoneFunction.skirtCollider.count);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        GUILayout.Box(new GUIContent("腿骨半径"));
                        cState.dynamicBoneFunction.skirtCollider.r = GUILayout.TextField(cState.dynamicBoneFunction.skirtCollider.r);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        GUILayout.Box(new GUIContent("碰撞半径"));
                        cState.dynamicBoneFunction.skirtCollider.colliderR = GUILayout.TextField(cState.dynamicBoneFunction.skirtCollider.colliderR);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) });
                    {
                        GUILayout.Box(new GUIContent("碰撞高度"));
                        cState.dynamicBoneFunction.skirtCollider.colliderH = GUILayout.TextField(cState.dynamicBoneFunction.skirtCollider.colliderH);
                    }
                    EditorGUILayout.EndHorizontal();




                    if (GUILayout.Button("预制碰撞生成"))
                    {
                        var avatarAnimator = cState.selectedAvatar.GetComponent<Animator>();

                        Transform leftlegTF = avatarAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                        Transform rightlegTF = avatarAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg);



                        DynamicBoneCollider insideDBC = new DynamicBoneCollider();

                        insideDBC.m_Bound = DynamicBoneColliderBase.Bound.Inside;
                        insideDBC.m_Radius = float.Parse(cState.dynamicBoneFunction.skirtCollider.colliderR);
                        insideDBC.m_Height = float.Parse(cState.dynamicBoneFunction.skirtCollider.colliderH);
                        insideDBC.m_Center = new Vector3(0, 0, /*0.01f*/ - (float.Parse(cState.dynamicBoneFunction.skirtCollider.r) + (insideDBC.m_Radius)));

                        CreatSKInsideDBC(leftlegTF.gameObject, insideDBC, true);
                        CreatSKInsideDBC(rightlegTF.gameObject, insideDBC, false);

                    }






                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(UIStyles.boxGuiStyle);
                if (cState.dynamicBoneFunction.addCollider.expand = GUILayout.Toggle(cState.dynamicBoneFunction.addCollider.expand, "动骨预设碰撞关联", UIStyles.WhiteSecondFoldout_title))
                {

                    //GUILabel("  选择得到动骨组件类人骨骨骼部分关联碰撞:", 11, TextAnchor.MiddleLeft, FontStyle.Bold);

                    if (cState.selectedAvatar)
                    {
                        if (Selection.activeGameObject)
                        {
                            if (Selection.activeGameObject.transform.root == cState.selectedAvatar.transform)
                            {
                                if (Selection.activeGameObject.GetComponent<DynamicBone>() != null)
                                {
                                    var avatarAnimator = cState.selectedAvatar.GetComponent<Animator>();
                                    if (avatarAnimator)
                                    {

                                        if (avatarAnimator.isHuman)
                                        {
                                            Dictionary<string, Transform> avatarBone = new Dictionary<string, Transform>
                                            {
                                                { "Head", avatarAnimator.GetBoneTransform(HumanBodyBones.Head) },
                                                { "Neck", avatarAnimator.GetBoneTransform(HumanBodyBones.Neck) },
                                                { "Chest", avatarAnimator.GetBoneTransform(HumanBodyBones.Chest) },
                                                { "Spine", avatarAnimator.GetBoneTransform(HumanBodyBones.Spine) },
                                                { "Hips", avatarAnimator.GetBoneTransform(HumanBodyBones.Hips) },
                                                { "Shoulder L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftShoulder) },
                                                { "Shoulder R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightShoulder) },
                                                { "Arm L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm) },
                                                { "Arm R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm) },
                                                { "Elbow L ", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm) },
                                                { "Elbow R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm) },
                                                { "Wrist L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftHand) },
                                                { "Wrist R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightHand) },
                                                { "Leg L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg) },
                                                { "Leg R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg) },
                                                { "Knee L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg) },
                                                { "Knee R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg) }
                                            };
                                            //avatarBone.Add("Ankle L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftFoot));
                                            //avatarBone.Add("Ankle R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightFoot));


                                            foreach (var bone in avatarBone)
                                            {
                                                if (bone.Value == null)
                                                {
                                                    EditorGUILayout.HelpBox(bone.Key + "没有绑定的骨骼!", MessageType.Error);
                                                    avatarBone.Remove(bone.Key);
                                                }
                                            }

                                            //骨骼字典序列生成关联按钮
                                            GeDBRelateButton(avatarBone);
                                        }
                                        else
                                        {
                                            EditorGUILayout.HelpBox("所选目标不是类人骨骼，请检查导入设置!", MessageType.Error);

                                        }
                                    }
                                    else
                                    {
                                        EditorGUILayout.HelpBox("所选目标不为人体模型或缺失Animator组件!", MessageType.Error);
                                    }


                                }
                                else
                                {
                                    EditorGUILayout.HelpBox("请选择目标动骨组件", MessageType.Info);
                                }
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("选择的GameObject不是目标Avatar的!", MessageType.Warning);
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("请选择一个类人骨Avatar", MessageType.Info);
                    }


                }
                EditorGUILayout.EndVertical();



                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

                EditorGUILayout.EndVertical();
                GUILayout.Space(sFoldSpaceWidth);
                EditorGUILayout.EndHorizontal();

            }
        }

        /// <summary>
        /// [UI控件初始化]预制体碰撞填充
        /// </summary>
        void InitializePresetColliderFunction()
        {

            //============================================================================================================================================================================
            if (cState.colliderFunction.expand = GUILayout.Toggle(cState.colliderFunction.expand, "预制类人骨碰撞体[Collider]", UIStyles.BlackFirstFoldout_title))
            {
                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(sFoldSpaceWidth);
                EditorGUILayout.BeginVertical(UIStyles.boxGuiStyle);

                if (cState.colliderFunction.tDA.expand = GUILayout.Toggle(cState.colliderFunction.tDA.expand, "MMD模型-TDA预制体碰撞", UIStyles.WhiteSecondFoldout_title))
                {
                    if (cState.colliderFunction.tDA.dynamicBoneColliderExpand = GUILayout.Toggle(cState.colliderFunction.tDA.dynamicBoneColliderExpand, "动骨(DynamicBone)", UIStyles.WhiteSecondFoldout_title))
                    {

                        if (cState.selectedAvatar)
                        {
                            var avatarAnimator = cState.selectedAvatar.GetComponent<Animator>();
                            if (avatarAnimator)
                            {

                                if (avatarAnimator.isHuman)
                                {
                                    Dictionary<string, Transform> avatarBone = new Dictionary<string, Transform>
                                    {
                                        { "Head", avatarAnimator.GetBoneTransform(HumanBodyBones.Head) },
                                        { "Neck", avatarAnimator.GetBoneTransform(HumanBodyBones.Neck) },
                                        { "Chest", avatarAnimator.GetBoneTransform(HumanBodyBones.Chest) },
                                        { "Spine", avatarAnimator.GetBoneTransform(HumanBodyBones.Spine) },
                                        { "Hips", avatarAnimator.GetBoneTransform(HumanBodyBones.Hips) },
                                        { "Shoulder L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftShoulder) },
                                        { "Shoulder R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightShoulder) },
                                        { "Arm L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm) },
                                        { "Arm R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm) },
                                        { "Elbow L ", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm) },
                                        { "Elbow R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm) },
                                        { "Wrist L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftHand) },
                                        { "Wrist R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightHand) },
                                        { "Leg L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg) },
                                        { "Leg R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg) },
                                        { "Knee L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg) },
                                        { "Knee R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg) }
                                    };
                                    //avatarBone.Add("Ankle L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftFoot));
                                    //avatarBone.Add("Ankle R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightFoot));

                                    var removeList = new List<string>();
                                    foreach (var bone in avatarBone)
                                    {
                                        if (bone.Value == null)
                                        {
                                            EditorGUILayout.HelpBox(bone.Key + "没有绑定的骨骼!", MessageType.Error);
                                            removeList.Add(bone.Key);
                                        }
                                    }
                                    foreach (var item in removeList)
                                    {
                                        avatarBone.Remove(item);
                                    }



                                    if (GUILayout.Button(new GUIContent("添加所有碰撞体", Icons.PrefabIcon)))
                                    {
                                        foreach (var boneTransform in avatarBone)
                                        {
                                            string[] boneName = boneTransform.Key.Split(' ');
                                            string end = string.Empty;
                                            if (boneName.Length > 1)
                                            {
                                                end = " " + boneName[1];
                                            }
                                            string colliderName = boneName[0] + " DynamicBone Collider" + end;

                                            if (!boneTransform.Value.Find(colliderName))
                                            {
                                                var tempGO = (PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab", typeof(GameObject)) as GameObject, cState.selectedAvatar.scene) as GameObject);
                                                tempGO.transform.SetParent(boneTransform.Value, false);
                                            }

                                        }
                                    }


                                    GenAvatarBoneDBCColliderButton(avatarBone);
                                }
                                else
                                {
                                    EditorGUILayout.HelpBox("所选目标不是类人骨骼，请检查导入设置!", MessageType.Error);

                                }
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("所选目标不为人体模型或缺失Animator组件!", MessageType.Error);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("请选择一个类人骨Avatar", MessageType.Info);
                        }
                    }
                    EditorGUI.BeginDisabledGroup(true);
                    if (cState.colliderFunction.tDA.clothColliderExpand = GUILayout.Toggle(cState.colliderFunction.tDA.clothColliderExpand, "布料(Cloth)", UIStyles.WhiteSecondFoldout_title))
                    {

                        if (cState.selectedAvatar)
                        {
                            var avatarAnimator = cState.selectedAvatar.GetComponent<Animator>();
                            if (avatarAnimator)
                            {

                                if (avatarAnimator.isHuman)
                                {
                                    Dictionary<string, Transform> avatarBone = new Dictionary<string, Transform>
                                    {
                                        { "Head", avatarAnimator.GetBoneTransform(HumanBodyBones.Head) },
                                        { "Neck", avatarAnimator.GetBoneTransform(HumanBodyBones.Neck) },
                                        { "Chest", avatarAnimator.GetBoneTransform(HumanBodyBones.Chest) },
                                        { "Spine", avatarAnimator.GetBoneTransform(HumanBodyBones.Spine) },
                                        { "Hips", avatarAnimator.GetBoneTransform(HumanBodyBones.Hips) },
                                        { "Shoulder L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftShoulder) },
                                        { "Shoulder R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightShoulder) },
                                        { "Arm L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm) },
                                        { "Arm R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm) },
                                        { "Elbow L ", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm) },
                                        { "Elbow R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm) },
                                        { "Wrist L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftHand) },
                                        { "Wrist R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightHand) },
                                        { "Leg L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg) },
                                        { "Leg R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg) },
                                        { "Knee L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg) },
                                        { "Knee R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg) }
                                    };
                                    //avatarBone.Add("Ankle L", avatarAnimator.GetBoneTransform(HumanBodyBones.LeftFoot));
                                    //avatarBone.Add("Ankle R", avatarAnimator.GetBoneTransform(HumanBodyBones.RightFoot));

                                    var removeList = new List<string>();
                                    foreach (var bone in avatarBone)
                                    {
                                        if (bone.Value == null)
                                        {
                                            EditorGUILayout.HelpBox(bone.Key + "没有绑定的骨骼!", MessageType.Error);
                                            removeList.Add(bone.Key);
                                        }
                                    }
                                    foreach (var item in removeList)
                                    {
                                        avatarBone.Remove(item);
                                    }



                                    if (GUILayout.Button(new GUIContent("添加所有布料碰撞体", Icons.PrefabIcon)))
                                    {
                                        foreach (var boneTransform in avatarBone)
                                        {
                                            string[] boneName = boneTransform.Key.Split(' ');
                                            string end = string.Empty;
                                            if (boneName.Length > 1)
                                            {
                                                end = " " + boneName[1];
                                            }
                                            string colliderName = boneName[0] + " Cloth Collider" + end;

                                            if (!boneTransform.Value.Find(colliderName))
                                            {
                                                var tempGO = (PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\Cloth Collider\TDA\" + colliderName + ".prefab", typeof(GameObject)) as GameObject, cState.selectedAvatar.scene) as GameObject);
                                                tempGO.transform.SetParent(boneTransform.Value, false);
                                            }

                                        }
                                    }


                                    GenAvatarBoneClothColliderButton(avatarBone);//生成按钮
                                }
                                else
                                {
                                    EditorGUILayout.HelpBox("所选目标不是类人骨骼，请检查导入设置!", MessageType.Error);

                                }
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("所选目标不为人体模型或缺失Animator组件!", MessageType.Error);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("请选择一个类人骨Avatar", MessageType.Info);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(sFoldSpaceWidth);
                EditorGUILayout.EndHorizontal();

                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

            }


        }

        /// <summary>
        /// [UI控件初始化]绑定相关功能-Constraint
        /// </summary>
        void InitializeBindFunction()
        {
            //============================================================================================================================================================================
            //BindFunction menu
            if (cState.bindFunction.expand = GUILayout.Toggle(cState.bindFunction.expand, LocalizationClass.Main.BindFunction, UIStyles.BlackFirstFoldout_title))
            {

                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(sFoldSpaceWidth);
                EditorGUILayout.BeginVertical(UIStyles.boxGuiStyle);

                GUILayout.Box(new GUIContent("Constraint组件绑定世界等:"), new[] { GUILayout.Height(20), GUILayout.ExpandWidth(true) });
                
                cState.bindFunction.itemRoot = (GameObject)EditorGUILayout.ObjectField("绑定内容", cState.bindFunction.itemRoot, typeof(GameObject), true);
                cState.bindFunction.targetGO = (GameObject)EditorGUILayout.ObjectField("绑定目标", cState.bindFunction.targetGO, typeof(GameObject), true);

                EditorGUILayout.Space();

                GUILabel("添加:");

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("相对绑定"))
                    {

                        //创建新游戏对象 “Constraint”
                        //GameObject nGO = new GameObject("Constraint");


                        //给指定avatar对象下建立父级
                        //nGO.transform.parent = cState.selectedAvatar.transform; //旧方法
                        //nGO.transform.SetParent(cState.selectedAvatar.transform, false);

                        //将绑定内容移动至对象Constraint 下
                        //cState.bindFunction.itemRoot.transform.parent = nGO.transform;

                        //添加ParentConstraint组件
                        ParentConstraint pC = cState.bindFunction.itemRoot.AddComponent<ParentConstraint>();

                        //实例绑定来源
                        var pCS = new ConstraintSource();
                        pCS.sourceTransform = cState.bindFunction.targetGO.transform;//装填来源
                        pCS.weight = 1;
                        pC.AddSource(pCS);//Source装填
                        pC.weight = 1;
                        pC.rotationAxis = Axis.X & Axis.Y & Axis.Z;
                        pC.translationAxis = Axis.X & Axis.Y & Axis.Z;

                        pC.constraintActive = true;//启动绑定

                    }
                    EditorGUI.BeginDisabledGroup(true);
                    {
                        if (GUILayout.Button("世界绑定"))
                        {

                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                GUILayout.Space(sFoldSpaceWidth);
                EditorGUILayout.EndHorizontal();
                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

            }

        }

        /// <summary>   
        /// [UI控件初始化]杂项功能
        /// </summary>
        void InitializeLitFunction()
        {
            //============================================================================================================================================================================
            //LitFunction menu
            if (cState.litFunction.expand = GUILayout.Toggle(cState.litFunction.expand, LocalizationClass.Main.LitFunction, UIStyles.BlackFirstFoldout_title))
            {

                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(sFoldSpaceWidth);
                EditorGUILayout.BeginVertical(UIStyles.boxGuiStyle);

                EditorGUI.BeginDisabledGroup(cState.selectedAvatar == null);
                {
                    EditorGUILayout.Space();

                    GUILayout.BeginVertical(); //Row

                    //temp
                    if (GUILayout.Button("为[CharacterJoint]连接上级刚体"))
                    {
                        foreach (var gO in Selection.gameObjects)
                        {
                            try
                            {
                                gO.GetComponent<CharacterJoint>().connectedBody = gO.transform.parent.GetComponent<Rigidbody>();
                            }
                            catch (Exception ex)
                            {
                                Debug.Log(string.Format("无法为[{0}]指定父级的Rigidbody\n错误:{1}", gO.name, ex.Message));
                            }

                        }
                    }

                    //填充口型
                    if (GUILayout.Button(LocalizationClass.litFunction.FillVisemes))
                    {
                        litFunctionButtonReplace(LitFunction.FillVisemes);
                    }

                    //重置形态键
                    if (GUILayout.Button(LocalizationClass.litFunction.ResetBlendshapes))
                    {
                        litFunctionButtonReplace(LitFunction.ResetBlendShapes);
                    }

                    //自动设置视角球
                    EditorGUI.BeginDisabledGroup(_edittingView);
                    if (GUILayout.Button(LocalizationClass.litFunction.EditViewpoint))
                    {
                        litFunctionButtonReplace(LitFunction.EditViewpoint);
                    }
                    EditorGUI.EndDisabledGroup();

                    //重置姿态
                    if (GUILayout.Button(LocalizationClass.litFunction.ResetPose))
                    {
                        litFunctionButtonReplace(LitFunction.ResetPose);
                    }

                    //复制碰撞体至同OBJ路径下"
                    if (GUILayout.Button("复制选择OBJ的碰撞体至同AvatarOBJ路径下"))
                    {
                        //litFunctionButtonReplace(LitFunction.OpenAllDynamicBone);

                        foreach (var sGO in Selection.gameObjects)
                        {
                            GameObject gO = GetTargetTransforms(sGO.transform).gameObject;
                            if (gO.transform == cState.selectedAvatar.transform | gO.transform == Selection.activeTransform.root)
                            {
                                Debug.Log(SSS + "未找到对应OBJ");
                            }
                            else
                            {
                                FunctionClass.CopyColliders(sGO, gO, cState);
                                Debug.Log(SSS + "源" + sGO.transform.root.gameObject.name);
                                Debug.Log(SSS + "目标" + gO.transform.root.gameObject.name);
                                Debug.Log(SSS + "碰撞组件添加完毕");
                            }
                        }


                    }
                    //复制碰撞体至同OBJ路径下" add
                    if (GUILayout.Button("复制选择OBJ的碰撞体至同AvatarOBJ路径下(add)"))
                    {
                        //litFunctionButtonReplace(LitFunction.OpenAllDynamicBone);

                        foreach (var sGO in Selection.gameObjects)
                        {
                            GameObject gO = GetTargetTransforms(sGO.transform ,true).gameObject;
                            if (gO.transform == cState.selectedAvatar.transform | gO.transform == Selection.activeTransform.root)
                            {
                                Debug.Log(SSS + "未找到对应OBJ");
                            }
                            else
                            {
                                FunctionClass.CopyColliders(sGO, gO, cState);
                                Debug.Log(SSS + "源" + sGO.transform.root.gameObject.name);
                                Debug.Log(SSS + "目标" + gO.transform.root.gameObject.name);
                                Debug.Log(SSS + "碰撞组件添加完毕");
                            }
                        }


                    }
                    //复制布料组件至同OBJ路径下"
                    if (GUILayout.Button("复制选择OBJ的[布料](Cloth)组件至同AvatarOBJ路径下"))
                    {
                        //litFunctionButtonReplace(LitFunction.OpenAllDynamicBone);

                        foreach (var sGO in Selection.gameObjects)
                        {
                            GameObject gO = GetTargetTransforms(sGO.transform, true).gameObject;
                            if (gO.transform == cState.selectedAvatar.transform | gO.transform == Selection.activeTransform.root)
                            {
                                Debug.Log(SSS + "未找到对应OBJ");
                            }
                            else
                            {
                                FunctionClass.CopyCloth(sGO, gO, cState);
                                //Debug.Log(SSS + "源" + sGO.transform.root.gameObject.name);
                                //Debug.Log(SSS + "目标" + gO.transform.root.gameObject.name);
                                //Debug.Log(SSS + "碰撞组件添加完毕");
                            }
                        }


                    }

                    GUILayout.Box(new GUIContent("生成碰撞球包覆外碰撞球 半径(Radius)"), new[] { GUILayout.Height(20), GUILayout.ExpandWidth(true) });

                    gRadiusString = GUILayout.TextField(gRadiusString);

                    bool sParse = float.TryParse((gRadiusString), out gRadius);

                    EditorGUI.BeginDisabledGroup(!sParse);
                    {
                        if (GUILayout.Button("生成包覆碰撞球"))
                        {
                            if (sParse)
                            {
                                foreach (var sGO in Selection.gameObjects)
                                {
                                    GameObject gO = GetTargetTransforms(sGO.transform, true).gameObject;
                                    if (gO.transform == cState.selectedAvatar.transform | gO.transform == Selection.activeTransform.root)
                                    {
                                        Debug.Log(SSS + "未找到对应OBJ");
                                    }
                                    else
                                    {
                                        DynamicBoneCollider dbc = gO.GetComponent<DynamicBoneCollider>();
                                        float colliderR = dbc.m_Radius;
                                        float colliderL = colliderR * 2 / (float)Math.Sqrt(3);
                                        float colliderLp = gRadius * 2 / (float)Math.Sqrt(3);
                                        float colliderL2 = (colliderL + colliderLp) / 2;
                                        DynamicBoneCollider[] dynamicBoneColliderArray = new DynamicBoneCollider[8]
                                        {
                                            new DynamicBoneCollider(){ m_Center=new Vector3(-colliderL2,colliderL2,colliderL2)},
                                            new DynamicBoneCollider(){ m_Center=new Vector3(colliderL2,-colliderL2,colliderL2)},
                                            new DynamicBoneCollider(){ m_Center=new Vector3(colliderL2,colliderL2,-colliderL2)},
                                            new DynamicBoneCollider(){ m_Center=new Vector3(-colliderL2,-colliderL2,colliderL2)},
                                            new DynamicBoneCollider(){ m_Center=new Vector3(colliderL2,-colliderL2,-colliderL2)},
                                            new DynamicBoneCollider(){ m_Center=new Vector3(-colliderL2,colliderL2,-colliderL2)},
                                            new DynamicBoneCollider(){ m_Center=new Vector3(-colliderL2,-colliderL2,-colliderL2)},
                                            new DynamicBoneCollider(){ m_Center=new Vector3(colliderL2,colliderL2,colliderL2)}
                                        };


                                        //double[,] dA = new double[3,8];

                                        //for (int i = 0; i < dynamicBoneColliderArray.Length; i++)
                                        //{
                                        //    dynamicBoneColliderArray[i] = new DynamicBoneCollider();
                                        //}

                                        //dynamicBoneColliderArray[0].m_Center = new Vector3(colliderL2, colliderL2, colliderL2);






                                        for (int i = 0; i < dynamicBoneColliderArray.Length; i++)
                                        {
                                            //switch (i%3)
                                            //{
                                            //    case 0:
                                            //        dynamicBoneColliderArray[i].m_Center.x = -dynamicBoneColliderArray[i].m_Center.x;
                                            //        dynamicBoneColliderArray[(i + 1) % 8].m_Center = dynamicBoneColliderArray[i].m_Center;
                                            //        break;
                                            //    case 1:
                                            //        dynamicBoneColliderArray[i].m_Center.y = -dynamicBoneColliderArray[i].m_Center.x;
                                            //        dynamicBoneColliderArray[(i + 1) % 8].m_Center = dynamicBoneColliderArray[i].m_Center;
                                            //        break;
                                            //    case 2:
                                            //        dynamicBoneColliderArray[i].m_Center.z = -dynamicBoneColliderArray[i].m_Center.x;
                                            //        dynamicBoneColliderArray[(i + 1) % 8].m_Center = dynamicBoneColliderArray[i].m_Center;
                                            //        break;
                                            //    default:
                                            //        break;
                                            //}
                                            GameObject nGO = FunctionClass.AddNewOBJ(sGO.transform, "Sph DynamicBone Collider " + i);
                                            nGO.transform.localPosition = new Vector3();
                                            nGO.transform.localRotation = new Quaternion();
                                            nGO.transform.localScale = new Vector3(1, 1, 1);
                                            var nDBC = nGO.AddComponent<DynamicBoneCollider>();
                                            Debug.Log(SSS + string.Format("ADD X{0} Y{1} Z{2}", dynamicBoneColliderArray[i].m_Center.x, dynamicBoneColliderArray[i].m_Center.y, dynamicBoneColliderArray[i].m_Center.z));
                                            nDBC.m_Center = dynamicBoneColliderArray[i].m_Center;
                                            nDBC.m_Radius = gRadius;
                                        }
                                    }
                                }
                            }


                        }
                    }
                    EditorGUI.EndDisabledGroup();


                    GUILayout.EndVertical();

                    /*if(GUILayout.Button("_Fix Mouth Randomly Opening"))
                    {
                        ActionButton(LitFunction.FixRandomMouth);
                    }

                    if(GUILayout.Button("_Disable Blinking"))
                    {
                        ActionButton(LitFunction.DisableBlinking);
                    }*/

                    EditorGUILayout.Space();

                    //LableBOX
                    GUILayout.Box(new GUIContent(LocalizationClass.Main.RemoveAll), new[] { GUILayout.Height(20), GUILayout.ExpandWidth(true) });


                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(); //Row

                    if (GUILayout.Button(new GUIContent(LocalizationClass.Copier.Colliders_box, Icons.ColliderBox)))
                    {
                        litFunctionButtonReplace(LitFunction.RemoveBoxColliders);
                    }
                    if (GUILayout.Button(new GUIContent(LocalizationClass.Copier.Colliders_capsule, Icons.ColliderCapsule)))
                    {
                        litFunctionButtonReplace(LitFunction.RemoveCapsuleColliders);
                    }
                    if (GUILayout.Button(new GUIContent(LocalizationClass.Copier.Colliders_sphere, Icons.ColliderSphere)))
                    {
                        litFunctionButtonReplace(LitFunction.RemoveSphereColliders);
                    }

                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(); //Row
#if NO_BONES
                        EditorGUI.BeginDisabledGroup(true);
#endif
                    if (GUILayout.Button(new GUIContent(LocalizationClass.Copier.DynamicBones, Icons.BoneIcon)))
                    {
                        litFunctionButtonReplace(LitFunction.RemoveDynamicBones);
                    }
#if NO_BONES
                        EditorGUI.EndDisabledGroup();
#endif

#if NO_BONES
                        EditorGUI.BeginDisabledGroup(true);
#endif
                    if (GUILayout.Button(new GUIContent(LocalizationClass.Copier.DynamicBones_colliders, Icons.DynamicBoneCollider)))
                    {
                        litFunctionButtonReplace(LitFunction.RemoveDynamicBoneColliders);
                    }
#if NO_BONES
                        EditorGUI.EndDisabledGroup();
#endif
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndVertical();
                GUILayout.Space(sFoldSpaceWidth);
                EditorGUILayout.EndHorizontal();
                //========================================================================================================================================================================
                EditorGUILayout.Separator();
                //========================================================================================================================================================================


            }

        }

        /// <summary>
        /// [UI控件初始化]复制组件功能
        /// </summary>
        void InitializeCopierFunction()
        {
            //============================================================================================================================================================================
            //复制功能组件菜单 Component Copier menu
            if (cState.copier.expand = GUILayout.Toggle(cState.copier.expand, LocalizationClass.Main.Copier, UIStyles.BlackFirstFoldout_title))
            {
                if (false)
                {
                    EditorGUILayout.HelpBox("功能调整中", MessageType.Info);
                }
                else
                {

                    //========================================================================================================================================================================
                    EditorGUILayout.Separator();
                    //========================================================================================================================================================================

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(sFoldSpaceWidth);
                    EditorGUILayout.BeginVertical(UIStyles.boxGuiStyle);

                    cState.copier.copierSelectedFrom = (GameObject)EditorGUILayout.ObjectField(LocalizationClass.Copier.CopyFrom + ":", cState.copier.copierSelectedFrom, typeof(GameObject), true);

                    EditorGUILayout.BeginHorizontal();//横向排列开始
                    if (GUILayout.Button(LocalizationClass.Buttons.SelectFromScene))
                    {
                        if (Selection.activeGameObject != null)
                            cState.copier.copierSelectedFrom = Selection.activeGameObject.transform.root.gameObject;
                    }
                    EditorGUILayout.EndHorizontal();//横向排列结束

                    EditorGUILayout.Space();

                    if (cState.selectedAvatar == null)
                    {
                        EditorGUILayout.HelpBox("复制来源Avatar为空!", MessageType.Error);
                    }
                    else
                    {
                        if (cState.selectedAvatar == cState.copier.copierSelectedFrom)
                        {
                            EditorGUILayout.HelpBox("目标Avatar与来源Avatar相同!", MessageType.Error);
                        }
                        else
                        {
                            EditorGUI.BeginDisabledGroup(cState.copier.copierSelectedFrom == null || cState.selectedAvatar == null);
                            {
                                //转换菜单
                                //Transforms menu
                                EditorGUILayout.BeginHorizontal();//横向排列开始
                                cState.copier.transforms.expand = GUILayout.Toggle(cState.copier.transforms.expand, Icons.Transform, "Foldout", GUILayout.MinWidth(20), GUILayout.ExpandHeight(true), GUILayout.MaxWidth(30), GUILayout.MaxHeight(10));
                                cState.copier.transforms.copy = GUILayout.Toggle(cState.copier.transforms.copy, LocalizationClass.Copier.Transforms, GUILayout.ExpandWidth(false), GUILayout.MinWidth(20));
                                EditorGUILayout.EndHorizontal();//横向排列结束

                                if (cState.copier.transforms.expand)
                                {
                                    EditorGUI.BeginDisabledGroup(!cState.copier.transforms.copy);
                                    EditorGUILayout.Space();

                                    cState.copier.transforms.copyPosition = EditorGUILayout.Toggle(LocalizationClass.Copier.Transforms_position, cState.copier.transforms.copyPosition, GUILayout.ExpandWidth(false));
                                    cState.copier.transforms.copyRotation = EditorGUILayout.Toggle(LocalizationClass.Copier.Transforms_rotation, cState.copier.transforms.copyRotation, GUILayout.ExpandWidth(false));
                                    cState.copier.transforms.copyScale = EditorGUILayout.Toggle(LocalizationClass.Copier.Transforms_scale, cState.copier.transforms.copyScale);


                                    EditorGUILayout.Space();
                                    EditorGUI.EndDisabledGroup();
                                }

                                //动骨菜单
                                //DynamicBones menu
                                //controlState.copier.dynamicBone = new ControlState.Copier.DynamicBones();
#if NO_BONES
                                        EditorGUI.BeginDisabledGroup(true);
#endif
                                EditorGUILayout.BeginHorizontal();//横向排列开始
                                cState.copier.dynamicBones.expand = GUILayout.Toggle(cState.copier.dynamicBones.expand, Icons.BoneIcon, "Foldout", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true), GUILayout.MaxWidth(30), GUILayout.MaxHeight(10));
#if NO_BONES
                            cState.copier.dynamicBones.expand = GUILayout.Toggle(false, cState.copier.dynamicBones + " " + LocalizationClass.Warning.NotFound, GUILayout.ExpandWidth(false), GUILayout.MinWidth(20));
#else
                                cState.copier.dynamicBones.copy = GUILayout.Toggle(cState.copier.dynamicBones.copy, LocalizationClass.Copier.DynamicBones, GUILayout.ExpandWidth(false), GUILayout.MinWidth(20));
#endif
                                EditorGUILayout.EndHorizontal();//横向排列结束

                                if (cState.copier.dynamicBones.expand)
                                {
                                    EditorGUI.BeginDisabledGroup(!cState.copier.dynamicBones.copy);
                                    EditorGUILayout.Space();

                                    cState.copier.dynamicBones.copySettings = EditorGUILayout.Toggle(LocalizationClass.Copier.DynamicBones_settings, cState.copier.dynamicBones.copySettings, GUILayout.ExpandWidth(false));
                                    cState.copier.dynamicBones.copyColliders = EditorGUILayout.Toggle(LocalizationClass.Copier.DynamicBones_colliders, cState.copier.dynamicBones.copyColliders, GUILayout.ExpandWidth(false));
                                    cState.copier.dynamicBones.createMissingBones = EditorGUILayout.Toggle(LocalizationClass.Copier.DynamicBones_createMissingBones, cState.copier.dynamicBones.createMissingBones, GUILayout.ExpandWidth(false));
                                    cState.copier.dynamicBones.removeOldBones = EditorGUILayout.Toggle(LocalizationClass.Copier.DynamicBones_removeOldBones, cState.copier.dynamicBones.removeOldBones, GUILayout.ExpandWidth(false));
                                    cState.copier.dynamicBones.removeOldColliders = EditorGUILayout.Toggle(LocalizationClass.Copier.DynamicBones_removeOldColliders, cState.copier.dynamicBones.removeOldColliders, GUILayout.ExpandWidth(false));

                                    EditorGUILayout.Space();
                                    EditorGUI.EndDisabledGroup();
                                }
#if NO_BONES
                                        EditorGUI.EndDisabledGroup();
#endif

                                //AvatarDescriptor组件菜单
                                //AvatarDescriptor menu
                                EditorGUILayout.BeginHorizontal();//横向排列开始
                                cState.copier.avatarDescriptor.expand = GUILayout.Toggle(cState.copier.avatarDescriptor.expand, Icons.AvatarDescriptor, "Foldout", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true), GUILayout.MaxWidth(30), GUILayout.MaxHeight(10));
                                cState.copier.avatarDescriptor.copy = GUILayout.Toggle(cState.copier.avatarDescriptor.copy, LocalizationClass.Copier.Descriptor, GUILayout.ExpandWidth(false), GUILayout.MinWidth(20));
                                EditorGUILayout.EndHorizontal();//横向排列结束

                                if (cState.copier.avatarDescriptor.expand)
                                {
                                    EditorGUI.BeginDisabledGroup(!cState.copier.avatarDescriptor.copy);
                                    EditorGUILayout.Space();

                                    cState.copier.avatarDescriptor.copySettings = EditorGUILayout.Toggle(LocalizationClass.Copier.Descriptor_settings, cState.copier.avatarDescriptor.copySettings, GUILayout.ExpandWidth(false));
                                    cState.copier.avatarDescriptor.copyPipelineId = EditorGUILayout.Toggle(LocalizationClass.Copier.Descriptor_pipelineId, cState.copier.avatarDescriptor.copyPipelineId, GUILayout.ExpandWidth(false));
                                    cState.copier.avatarDescriptor.copyAnimationOverrides = EditorGUILayout.Toggle(LocalizationClass.Copier.Descriptor_animationOverrides, cState.copier.avatarDescriptor.copyAnimationOverrides, GUILayout.ExpandWidth(false));

                                    EditorGUILayout.Space();
                                    EditorGUI.EndDisabledGroup();
                                }

                                //SkinnedMeshRenderer菜单
                                //SkinnedMeshRenderer menu
                                EditorGUILayout.BeginHorizontal();//横向排列开始
                                cState.copier.skinMeshRender.expand = GUILayout.Toggle(cState.copier.skinMeshRender.expand, Icons.SkinnedMeshRenderer, "Foldout", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true), GUILayout.MaxWidth(30), GUILayout.MaxHeight(10));
                                cState.copier.skinMeshRender.copy = GUILayout.Toggle(cState.copier.skinMeshRender.copy, LocalizationClass.Copier.SkinMeshRender, GUILayout.ExpandWidth(false), GUILayout.MinWidth(20));
                                EditorGUILayout.EndHorizontal();//横向排列结束

                                if (cState.copier.skinMeshRender.expand)
                                {
                                    EditorGUI.BeginDisabledGroup(!cState.copier.skinMeshRender.copy);
                                    EditorGUILayout.Space();

                                    cState.copier.skinMeshRender.copySettings = EditorGUILayout.Toggle(LocalizationClass.Copier.SkinMeshRender_settings, cState.copier.skinMeshRender.copySettings, GUILayout.ExpandWidth(false));
                                    cState.copier.skinMeshRender.copyMaterials = EditorGUILayout.Toggle(LocalizationClass.Copier.SkinMeshRender_materials, cState.copier.skinMeshRender.copyMaterials, GUILayout.ExpandWidth(false));
                                    cState.copier.skinMeshRender.copyBlendShapeValues = EditorGUILayout.Toggle(LocalizationClass.Copier.SkinMeshRender_blendShapeValues, cState.copier.skinMeshRender.copyBlendShapeValues, GUILayout.ExpandWidth(false));

                                    EditorGUILayout.Space();
                                    EditorGUI.EndDisabledGroup();
                                }

                                //碰撞菜单
                                //Collider menu
                                EditorGUILayout.BeginHorizontal();//横向排列开始
                                cState.copier.colliders.expand = GUILayout.Toggle(cState.copier.colliders.expand, Icons.ColliderBox, "Foldout", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true), GUILayout.MaxWidth(30), GUILayout.MaxHeight(10));
                                cState.copier.colliders.copy = GUILayout.Toggle(cState.copier.colliders.copy, LocalizationClass.Copier.Colliders, GUILayout.ExpandWidth(false), GUILayout.MinWidth(20));
                                EditorGUILayout.EndHorizontal();//横向排列结束

                                if (cState.copier.colliders.expand)
                                {
                                    EditorGUI.BeginDisabledGroup(!cState.copier.colliders.copy);
                                    EditorGUILayout.Space();

                                    cState.copier.colliders.copyBox = EditorGUILayout.Toggle(LocalizationClass.Copier.Colliders_box, cState.copier.colliders.copyBox, GUILayout.ExpandWidth(false));
                                    cState.copier.colliders.copyCapsule = EditorGUILayout.Toggle(LocalizationClass.Copier.Colliders_capsule, cState.copier.colliders.copyCapsule, GUILayout.ExpandWidth(false));
                                    cState.copier.colliders.copySphere = EditorGUILayout.Toggle(LocalizationClass.Copier.Colliders_sphere, cState.copier.colliders.copySphere, GUILayout.ExpandWidth(false));
                                    cState.copier.colliders.copyMesh = EditorGUILayout.Toggle(LocalizationClass.Copier.Colliders_mesh, cState.copier.colliders.copyMesh, GUILayout.ExpandWidth(false));

                                    cState.copier.colliders.removeOld = EditorGUILayout.Toggle(LocalizationClass.Copier.Colliders_removeOld, cState.copier.colliders.removeOld, GUILayout.ExpandWidth(false));

                                    EditorGUILayout.Space();
                                    EditorGUI.EndDisabledGroup();
                                }

                                EditorGUILayout.Space();


                                EditorGUI.BeginDisabledGroup(!(cState.copier.dynamicBones.copyColliders || cState.copier.dynamicBones.copy || cState.copier.colliders.copy || cState.copier.avatarDescriptor.copy || cState.copier.skinMeshRender.copy));
                                {
                                    //转移按钮
                                    if (GUILayout.Button(LocalizationClass.Buttons.CopySelected))
                                    {
                                        string log = "";
                                        if (cState.copier.copierSelectedFrom == null)
                                        {
                                            log += LocalizationClass.Log.CopyFromInvalid;
                                            FunctionClass.Log(log, LogType.Warning);
                                        }
                                        else
                                        {
                                            /*//Prefab Check. Disabled copying to prefabs
                                            if(selectedAvatar.gameObject.scene.name == null)
                                            {
                                                if(!EditorUtility.DisplayDialog(Strings.GetString("warn_warning") ?? "_Warning",
                                                    Strings.GetString("warn_copyToPrefab") ?? "_You are trying to copy components to a prefab.\nThis cannot be undone.\nAre you sure you want to continue?",
                                                    Strings.GetString("warn_prefabOverwriteYes") ?? "_Yes, Overwrite", Strings.GetString("warn_prefabOverwriteNo") ?? "_No, Cancel"))
                                                {
                                                    _msg = Strings.GetString("log_cancelled") ?? "_Canceled.";
                                                    return;
                                                }
                                            }*/

                                            //Cancel Checks
                                            if (cState.copier.copierSelectedFrom == cState.selectedAvatar)
                                            {
                                                log += LocalizationClass.Log.CantCopyToSelf;
                                                FunctionClass.Log(log, LogType.Warning);
                                                return;
                                            }

                                            //Figure out how to prevent undo from adding multiple copies of the same component on
                                            /*//Record Undo
                                            Undo.RegisterFullObjectHierarchyUndo(cState.selectedAvatar, "Copy Components");
                                            if(cState.selectedAvatar.gameObject.scene.name == null) //In case it's a prefab instance, which it probably is
                                                PrefabUtility.RecordPrefabInstancePropertyModifications(cState.selectedAvatar);*/

                                            functionClass.CopyComponents(cState.copier.copierSelectedFrom, cState.selectedAvatar, cState);

                                            EditorUtility.SetDirty(cState.selectedAvatar);
                                            EditorSceneManager.MarkSceneDirty(cState.selectedAvatar.scene);

                                            //avatarInfo = AvatarInfo.GetInfo(cState.selectedAvatar, out _avatarInfoString);

                                            log += LocalizationClass.Log.Done;
                                            FunctionClass.Log(log, LogType.Log);
                                        }
                                    }
                                }
                                EditorGUI.EndDisabledGroup();
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                    }

                    EditorGUILayout.EndVertical();
                    GUILayout.Space(sFoldSpaceWidth);
                    EditorGUILayout.EndHorizontal();

                    //========================================================================================================================================================================
                    EditorGUILayout.Separator();
                    //========================================================================================================================================================================

                }
            }
        }
        #endregion


        //void OnHierarchyChange()
        //{
        //    Debug.Log("刷新HierarchyIcon...");

        //    EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
        //}


        //static void HierarchyItemCB(int instanceID, Rect rect)
        //{

        //    EditorApplication.hierarchyWindowItemOnGUI -= HierarchyItemCB;
        //    if (iconAD == null | iconDB == null) return;

        //    GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        //    if (gameObject == null) return;

        //    var avatarDescriptor = gameObject.GetComponent<VRC_AvatarDescriptor>();

        //    if (avatarDescriptor == null)
        //    {
        //        var dynamicBone = gameObject.GetComponent<DynamicBone>();
        //        if (dynamicBone == null)
        //        {
        //            return;
        //        }
        //        else { iconView = iconDB; }
        //    }
        //    else { iconView = iconAD; }

        //    EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconWidth));
        //    var padding = new Vector2(2, 1);
        //    var iconDrawRect = new Rect(
        //        rect.xMax - (iconWidth + padding.x),
        //        rect.yMin + padding.y,
        //        rect.width,
        //        rect.height);

        //    Debug.Log("ComponentMark...");
        //    var iconGUIContent = new GUIContent(iconView);
        //    EditorGUI.LabelField(iconDrawRect, iconGUIContent);
        //    EditorGUIUtility.SetIconSize(Vector2.zero);

        //}

        //=============================================================================================⬆GUI功能分类初始化函数⬆============================================================================================
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //=================================================================================================⬇GUI子功能函数⬇================================================================================================

        #region GUI子功能函数


        /// <summary>
        /// DebugLog用 对引用部分 添加颜色代码(暗绿,暗蓝,暗红,暗黄,暗紫,红,橙,蓝,紫,绿)
        /// </summary>
        /// <param name="str">变色文字</param>
        /// <param name="logColor">LogColor颜色Enum</param>
        /// <returns></returns>
        string tagString(string str, LogColor logColor)
        {
            string tagColor;
            switch (logColor)
            {
                case LogColor.DarkGreen:
                    tagColor = "239000FF";
                    break;
                case LogColor.DarkBlue:
                    tagColor = "1B2CD8FF";
                    break;
                case LogColor.DarkRed:
                    tagColor = "900000FF";
                    break;
                case LogColor.DarkYellow:
                    tagColor = "C9B725FF";
                    break;
                case LogColor.DarkPurple:
                    tagColor = "8500B4FF";
                    break;
                case LogColor.Red:
                    tagColor = "EA2626FF";
                    break;
                case LogColor.Orange:
                    tagColor = "EC7000FF";
                    break;
                case LogColor.Blue:
                    tagColor = "FFFF00FF";
                    break;
                case LogColor.Purple:
                    tagColor = "A700FFFF";
                    break;
                case LogColor.Green:
                    tagColor = "12F400FF";
                    break;
                default:
                    tagColor = "FFFFFFFF";
                    break;
            }

            return string.Format("<color=#{0}>{1}</color>", tagColor, str);

        }


        /// <summary>
        /// 根据cState.dynamicBoneFunction.skirtCollider.count 创建碰撞串
        /// </summary>
        /// <param name="gO">root GO</param>
        /// <param name="dbc">添加的动骨碰撞组件</param>
        /// <returns></returns>
        void CreatSKInsideDBC (GameObject gO , DynamicBoneCollider dbc ,bool lr)
        {

            GameObject nGO;
            int i = -1, gol;
            float t = 0;
            gol = gO.name.Split(' ').Length;

            string strP = gO.name.Split(' ')[gol - 1];

            nGO = new GameObject("Skirt inside DBCollider");

            if (int.TryParse(strP, out i))
            {
                i++;
                t = 180 / float.Parse(cState.dynamicBoneFunction.skirtCollider.count);
                //nGO.transform.localPosition = gO.transform.localPosition;
                //nGO.transform.localRotation = gO.transform.localRotation;

                //if (nGO.transform.rotation == gO.transform.rotation)
                //if (lr)
                //{
                //    nGO.transform.Rotate(0, 180 / float.Parse(cState.dynamicBoneFunction.skirtCollider.count),0);
                //}
                //else
                //{
                //    nGO.transform.Rotate(0,- 180 / float.Parse(cState.dynamicBoneFunction.skirtCollider.count), 0);
                //}
            }
            else
            {
                i = 0;
                //t = 0;
                //nGO.transform.position = gO.transform.position;
                //nGO.transform.rotation = gO.transform.rotation;
                //nGO.transform.localScale = gO.transform.localScale;
            }

            nGO.transform.SetParent(gO.transform, false);

            if (lr)
            {
                nGO.name += " L " + i.ToString();
                if (t != 0)
                {
                    nGO.transform.Rotate(0, t, 0);
                }
            }
            else
            {
                nGO.name += " R " + i.ToString();
                if (t != 0)
                {
                    nGO.transform.Rotate(0, -t, 0);
                }
            }

            var ndbc = nGO.AddComponent<DynamicBoneCollider>();
            //ndbc = dbc;

            ndbc.m_Bound = dbc.m_Bound;
            ndbc.m_Center = dbc.m_Center;
            ndbc.m_Direction = dbc.m_Direction;
            ndbc.m_Height = dbc.m_Height;
            ndbc.m_Radius = dbc.m_Radius;

            

            //Quaternion qt = nGO.transform.localRotation;
            //if (lr)
            //{
            //    qt.y = 360/int.Parse(cState.dynamicBoneFunction.skirtCollider.count);
            //}
            //else
            //{
            //    qt.y = 360 / int.Parse(cState.dynamicBoneFunction.skirtCollider.count);
            //}

            //nGO.transform.localRotation = qt;

            if (i.ToString() != cState.dynamicBoneFunction.skirtCollider.count)
            {
                CreatSKInsideDBC(nGO,dbc, lr);
            }


        }


        /// <summary>
        /// 得到选择OBJ路径GameOBJ同[selectedAvatar]路径下的GOBJ的Transform
        /// </summary>
        /// <param name="tF">选择的目标GOBJ的Transform</param>
        /// <returns>返回同[selectedAvatar]路径Transform</returns>
        Transform GetTargetTransforms(Transform tF)
        {
            Transform tempTf;
            if (tF != tF.root)
            {
                tempTf = GetTargetTransforms(tF.parent);
            }
            else
            {//替换目标Root/最后一级失败返回
                return cState.selectedAvatar.transform;
            }
            if (tempTf.Find(tF.name))
            {
                tempTf = tempTf.Find(tF.name);
                return tempTf;
            }
            else
            {//无OBJ目标 目标为空对象返回tf.root
                return Selection.activeTransform.root;
            }
        }

        /// <summary>
        /// 得到选择OBJ路径GameOBJ同[selectedAvatar]路径下的GOBJ的Transform(附带补充空GOBJ项)
        /// </summary>
        /// <param name="tF">选择的目标GOBJ的Transform</param>
        /// <param name="replace">补充GOBJ项</param>
        /// <returns>返回同[selectedAvatar]路径Transform</returns>
        Transform GetTargetTransforms(Transform tF, bool replace)
        {
            Transform tempTf;
            if (tF != tF.root)
            {
                tempTf = GetTargetTransforms(tF.parent);
            }
            else
            {//替换目标Root
                return cState.selectedAvatar.transform;
            }
            if (tempTf.Find(tF.name))
            {
                tempTf = tempTf.Find(tF.name);
                return tempTf;
            }
            else
            {//无OBJ目标 目标为空对象返回tf.root/补充项/

                if (replace)
                {
                    GameObject gO = new GameObject(tF.name);
                    FunctionClass.Log("正在创建目标空GameObject:" + gO.name, LogType.Log);
                    FunctionClass.Log("正在设置GameObject<" + gO.name + ">父级至:" + tempTf.transform.name, LogType.Log);
                    gO.transform.parent = tempTf.transform;
                    gO.transform.localRotation = tF.localRotation;
                    gO.transform.localScale = tF.localScale;
                    gO.transform.localPosition = tF.localPosition;
                    FunctionClass.Log("设置GameObject<" + gO.name + ">父级完成", LogType.Log);
                    return gO.transform;
                }
                else
                {
                    return Selection.activeTransform.root;
                }
            }
        }

        /// <summary>
        /// 通过GO得到String路径
        /// </summary>
        /// <param name="obj">代入GameObject</param>
        /// <param name="skipRoot">是否跳过Root级</param>
        /// <returns></returns>
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

        /// <summary>
        /// 基于人体骨骼字典类生成DBC预制体GO创建按钮[GenerateAvatarBoneCollider]
        /// </summary>
        /// <param name="tFDictionary"></param>
        void GenAvatarBoneDBCColliderButton(Dictionary<string, Transform> tFDictionary)
        {
            foreach (var boneTransform in tFDictionary)
            {
                string[] boneName = boneTransform.Key.Split(' ');
                string end = string.Empty;
                if (boneName.Length > 1)
                {
                    end = " " + boneName[1];
                }
                string colliderName = boneName[0] + " DynamicBone Collider" + end;

                if (boneTransform.Value.Find(colliderName))
                {
                    if (GUILayout.Button(new GUIContent(boneTransform.Key + "去除碰撞" + colliderName, Icons.CollabDeletedIcon)))
                    {
                        DestroyImmediate(boneTransform.Value.Find(colliderName).gameObject);
                    }
                }
                else
                {
                    if (GUILayout.Button(new GUIContent(boneTransform.Key + "添加碰撞" + colliderName, Icons.CollabCreateIcon)))
                    {
                        var tempGO = (PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab", typeof(GameObject)) as GameObject, cState.selectedAvatar.scene) as GameObject);
                        tempGO.transform.SetParent(boneTransform.Value, false);


                        //骨架缩放比例匹配 100倍比例
                        for (int i = 0; i < cState.selectedAvatar.transform.childCount; i++)
                        {
                            Transform tF = cState.selectedAvatar.transform.GetChild(i);

                            if (tF.name =="Armature")
                            {
                                if (tF.localScale.x == 1)
                                {
                                    tempGO.transform.localScale = new Vector3(100, 100, 100);
                                }

                                break;
                            }
                        }





                        //PrefabUtility.RevertPrefabInstance(tempGO);
                        //PrefabUtility.ReplacePrefab(tempGO, @"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab",ReplacePrefabOptions.);
                        //tempGO.transform.in
                        //.transform.SetParent(boneTransform.Value);
                        //Instantiate(AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab", typeof(GameObject)) as GameObject).transform.SetParent(boneTransform.Value);
                        //(AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab", typeof(GameObject)) as GameObject)is GameObject;
                        //(AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab", typeof(GameObject)) as GameObject)
                        //  .transform.SetParent(boneTransform.Value);
                    }
                }

            }
        }
        /// <summary>
        /// 基于人体骨骼字典类生成Cloth预制体GO创建按钮[GenerateAvatarBoneCollider]
        /// </summary>
        /// <param name="tFDictionary"></param>
        void GenAvatarBoneClothColliderButton(Dictionary<string, Transform> tFDictionary)
        {
            foreach (var boneTransform in tFDictionary)
            {
                string[] boneName = boneTransform.Key.Split(' ');
                string end = string.Empty;
                if (boneName.Length > 1)
                {
                    end = " " + boneName[1];
                }
                string colliderName = boneName[0] + " Cloth Collider" + end;

                if (boneTransform.Value.Find(colliderName))
                {
                    if (GUILayout.Button(new GUIContent(boneTransform.Key + "去除碰撞" + colliderName, Icons.CollabDeletedIcon)))
                    {
                        DestroyImmediate(boneTransform.Value.Find(colliderName).gameObject);
                    }
                }
                else
                {
                    if (GUILayout.Button(new GUIContent(boneTransform.Key + "添加碰撞" + colliderName, Icons.CollabCreateIcon)))
                    {
                        var tempGO = (PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab", typeof(GameObject)) as GameObject, cState.selectedAvatar.scene) as GameObject);
                        tempGO.transform.SetParent(boneTransform.Value, false);
                        //PrefabUtility.RevertPrefabInstance(tempGO);
                        //PrefabUtility.ReplacePrefab(tempGO, @"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab",ReplacePrefabOptions.);
                        //tempGO.transform.in
                        //.transform.SetParent(boneTransform.Value);
                        //Instantiate(AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab", typeof(GameObject)) as GameObject).transform.SetParent(boneTransform.Value);
                        //(AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab", typeof(GameObject)) as GameObject)is GameObject;
                        //(AssetDatabase.LoadAssetAtPath(@"Assets\Seadew Studios\Prefab\Preset Humanoid Bone Collider\DynamicBone Collider\TDA\" + colliderName + ".prefab", typeof(GameObject)) as GameObject)
                        //  .transform.SetParent(boneTransform.Value);
                    }
                }

            }
        }

        /// <summary>
        /// [Predicate方法]判断DBCBase空 - 用于DBC~.RemoveAll() Math方法
        /// </summary>
        /// <param name="d"></param>
        /// <returns>DBCBase是否为空</returns>
        private static bool IsNullDB(DynamicBoneColliderBase d)
        {
            if (d)
                return false;
            else
                return true;
        }


        /// <summary>
        /// 基于人体骨骼字典类生成DBC预制体关联按钮[GenerateDynamicBoneRelateButton]
        /// </summary>
        /// <param name="tFDictionary"></param>
        void GeDBRelateButton(Dictionary<string, Transform> tFDictionary)
        {
            List<DynamicBone> tDBList = new List<DynamicBone>();

            //填入所有选中的动骨组件
            foreach (var gO in Selection.gameObjects)
            {
                tDBList.AddRange(gO.GetComponents<DynamicBone>());
            }

            //动骨碰撞查重记录字典数组 int为重复数量
            Dictionary<DynamicBoneColliderBase, int> dBaseDictionary = new Dictionary<DynamicBoneColliderBase, int>();


            //动骨关联碰撞去重去空
            foreach (var tDB in tDBList)
            {
                if (tDB.m_Colliders != null)
                {
                    //去空

                    tDB.m_Colliders.RemoveAll(IsNullDB);

                    //去除动骨单组件重复碰撞
#if !OLD_BONES
                    tDB.m_Colliders = new List<DynamicBoneColliderBase>(new HashSet<DynamicBoneColliderBase>(tDB.m_Colliders));
#else
                tDB.m_Colliders = new List<DynamicBoneCollider>( new HashSet<DynamicBoneCollider>(tDB.m_Colliders));
#endif

                    //记录所有动骨碰撞体
                    foreach (var collider in tDB.m_Colliders)
                    {
                        if (dBaseDictionary.ContainsKey(collider))
                        {
                            dBaseDictionary[collider]++;
                        }
                        else
                        {
                            dBaseDictionary.Add(collider, 1);
                        }
                    }

                }
            }
            int lostNum = 0;

            //遍历骨骼碰撞字典
            foreach (var boneTransform in tFDictionary)
            {
                //按规则取得碰撞名称 [colliderName]
                string[] boneName = boneTransform.Key.Split(' ');
                string end = string.Empty;
                if (boneName.Length > 1)
                {
                    end = " " + boneName[1];
                }
                string colliderName = boneName[0] + " DynamicBone Collider" + end;

                //获取骨骼Transform
                var tboneTransform = boneTransform.Value.Find(colliderName);
                if (tboneTransform)//确认骨骼TF
                {
                    if (tboneTransform.GetComponent<DynamicBoneCollider>())//确认骨骼包含DB碰撞
                    {
                        bool isFind = false;

                        if (dBaseDictionary != null)
                        {
                            foreach (var dBDitem in dBaseDictionary)
                            {
                                if (dBDitem.Key.GetComponent<DynamicBoneCollider>() == boneTransform.Value.Find(colliderName).GetComponent<DynamicBoneCollider>())
                                {
                                    if (dBDitem.Value == tDBList.Count)
                                    {
                                        if (GUILayout.Button(new GUIContent(boneTransform.Key + "取消关联碰撞" + colliderName, Icons.CollabExcludeIcon)))
                                        {
                                            foreach (var tDB in tDBList)
                                            {
                                                tDB.m_Colliders.Remove(dBDitem.Key);
                                            }
                                        }
                                        isFind = true;
                                        continue;
                                    }
                                    if (dBDitem.Value == 0)
                                    {
                                        if (GUILayout.Button(new GUIContent(boneTransform.Key + "关联碰撞" + colliderName, Icons.CollabMovedIcon)))
                                        {
                                            foreach (var tDB in tDBList)
                                            {
                                                tDB.m_Colliders.Add(tboneTransform.GetComponent<DynamicBoneCollider>());
                                            }
                                        }
                                        isFind = true;
                                        continue;
                                    }
                                    else
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        if (GUILayout.Button(new GUIContent(boneTransform.Key + "补充关联碰撞" + colliderName, Icons.CollabChangesIcon)))
                                        {
                                            foreach (var tDB in tDBList)
                                            {
                                                tDB.m_Colliders.Add(tboneTransform.GetComponent<DynamicBoneCollider>());
                                            }
                                        }
                                        if (GUILayout.Button(new GUIContent(boneTransform.Key + "取消关联碰撞" + colliderName, Icons.CollabExcludeIcon)))
                                        {
                                            foreach (var tDB in tDBList)
                                            {
                                                tDB.m_Colliders.Remove(dBDitem.Key);
                                            }
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        isFind = true;
                                        continue;
                                    }
                                }
                            }
                        }
                        if (!isFind)
                            if (GUILayout.Button(new GUIContent(boneTransform.Key + "关联碰撞" + colliderName, Icons.CollabMovedIcon)))
                            {
                                foreach (var tDB in tDBList)
                                {
                                    if (tDB.m_Colliders == null)//空对象补充
                                    {
                                        tDB.m_Colliders = new List<DynamicBoneColliderBase>();
                                    }
                                    tDB.m_Colliders.Add(tboneTransform.GetComponent<DynamicBoneCollider>());
                                }
                            }
                        //EditorGUILayout.HelpBox(colliderName + "目标碰撞无法匹配目标动骨的列表", MessageType.Error);


                    }
                    else
                    {
                        //EditorGUILayout.HelpBox(colliderName + "动骨Transform无法获取<DynamicBoneCollider>", MessageType.Error);
                        if (GUILayout.Button(new GUIContent(boneTransform.Key + "删除无效碰撞OBJ" + colliderName, Icons.CollabConflictIcon)))
                        {
                            foreach (var tDB in tDBList)
                            {
                                tDB.m_Colliders.Add(tboneTransform.GetComponent<DynamicBoneCollider>());
                            }
                        }
                    }
                }
                else
                {
                    lostNum++;
                    //EditorGUILayout.HelpBox(colliderName+ "动骨tboneTransform为空", MessageType.Info);
                }
            }
            if (lostNum == tFDictionary.Count)
            {
                EditorGUILayout.HelpBox("目标Avatar内无预制动骨碰撞可用!", MessageType.Warning);
            }
        }

        /// <summary>
        /// GUI文字显示
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textSize"></param>
        /// <param name="anchor"></param>
        /// <param name="fontStyle"></param>
        public static void GUILabel(string text, int textSize = 11, TextAnchor anchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal)
        {
            GUILayout.Label(text, new GUIStyle(EditorStyles.label)
            {
                alignment = anchor,
                wordWrap = true,
                fontSize = textSize
            });
        }

        /// <summary>
        /// LitFunctionButton集合触发
        /// </summary>
        /// <param name="function"></param>
        void litFunctionButtonReplace(LitFunction function)
        {
            if (cState.selectedAvatar == null)
            {
                //Shouldn't be possible with disable group
                Debug.LogWarning(SSS + "<color=#C9B725FF>没有选择Avatar</color> 请检查选择的Avatar对象(SelectedAvatar)");
                return;
            }

            //Record Undo
            Undo.RegisterFullObjectHierarchyUndo(cState.selectedAvatar, "Tools menu: " + function.ToString());
            if (cState.selectedAvatar.gameObject.scene.name == null) //In case it's a prefab instance, which it probably is
                PrefabUtility.RecordPrefabInstancePropertyModifications(cState.selectedAvatar);

            switch (function)
            {
                case LitFunction.RemoveColliders:
                    FunctionClass.DestroyAllComponentsOfType(cState.selectedAvatar, typeof(Collider));
                    break;
                case LitFunction.RemoveBoxColliders:
                    FunctionClass.DestroyAllComponentsOfType(cState.selectedAvatar, typeof(BoxCollider));
                    break;
                case LitFunction.RemoveCapsuleColliders:
                    FunctionClass.DestroyAllComponentsOfType(cState.selectedAvatar, typeof(CapsuleCollider));
                    break;
                case LitFunction.RemoveMeshColliders:
                    FunctionClass.DestroyAllComponentsOfType(cState.selectedAvatar, typeof(MeshCollider));
                    break;
                case LitFunction.RemoveSphereColliders:
                    FunctionClass.DestroyAllComponentsOfType(cState.selectedAvatar, typeof(SphereCollider));
                    break;
                case LitFunction.RemoveDynamicBoneColliders:
#if !NO_BONES
                    FunctionClass.DestroyAllComponentsOfType(cState.selectedAvatar, typeof(DynamicBoneCollider));
#endif
                    break;
                case LitFunction.RemoveDynamicBones:
#if !NO_BONES
                    FunctionClass.DestroyAllComponentsOfType(cState.selectedAvatar, typeof(DynamicBone));
#endif
                    break;
                case LitFunction.ResetPose:
                    FunctionClass.ResetPose(cState.selectedAvatar);
                    break;
                case LitFunction.ResetBlendShapes:
                    FunctionClass.ResetBlendShapes(cState.selectedAvatar);
                    break;
                case LitFunction.FixRandomMouth:
                    //FunctionClass.FixRandomMouthOpening(cState.selectedAvatar);
                    break;
                case LitFunction.DisableBlinking:
                    //FunctionClass.DisableBlinking(cState.selectedAvatar);
                    break;
                case LitFunction.FillVisemes:
                    FunctionClass.FillVisemes(cState.selectedAvatar);
                    break;
                case LitFunction.EditViewpoint:
                    FunctionClass.BeginEditViewpoint(cState.selectedAvatar);
                    break;
                case LitFunction.SelectedDynamicBoneSetRoot:
                    FunctionClass.SelectedDynamicBoneSetRootFromSelf(Selection.gameObjects);
                    break;
                case LitFunction.OpenAllDynamicBone:
                    FunctionClass.OpenAllDynamicBone(cState.selectedAvatar);
                    break;
                case LitFunction.OpenOBJDynamicBone:
                    FunctionClass.OpenOBJDynamicBone(Selection.gameObjects);
                    break;
                case LitFunction.CloseAllDynamicBone:
                    FunctionClass.CloseAllDynamicBone(cState.selectedAvatar);
                    break;
                case LitFunction.CloseOBJDynamicBone:
                    FunctionClass.CloseOBJDynamicBone(Selection.gameObjects);
                    break;
                case LitFunction.SelectedOBJAddDynamicBone:
                    FunctionClass.SelectedOBJAddDynamicBone(Selection.gameObjects);
                    break;
                default:
                    break;
            }

            //avatarInfo = AvatarInfo.GetInfo(cState.selectedAvatar, out _avatarInfoString);

            EditorUtility.SetDirty(cState.selectedAvatar);
            EditorSceneManager.MarkSceneDirty(cState.selectedAvatar.scene);
        }


        /// <summary>
        /// He大纲中检查选择的所有GameObject是否包含目标组件
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <returns>包含为True 找不到为False</returns>
        bool SelectionCheckComponent<T>()
        {
            foreach (var gameobject in Selection.gameObjects)
            {
                if (gameobject)
                {
                    if (gameobject.GetComponent<T>() != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        //=================================================================================================⬆GUI子功能函数⬆================================================================================================
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //================================================================================================================================================================================================================
        #endregion
    }


    //======================================================================================================⬇控件状态类⬇==================================================================================================
    
    /// <summary>
    /// Avatar综合信息控件状态类
    /// </summary>
    public class AvatarInfo
    {
        string name;
        string cachedInfo;

        int skinnedMeshRenders,
        skinnedMeshRenders_total,
        meshRenderers,
        meshRenderers_total,
        dynamicBoneTransforms,
        dynamicBoneTransforms_total,
        dynamicBoneColliders,
        dynamicBoneColliders_total,
        dynamicBoneColliderTransforms,
        dynamicBoneColliderTransforms_total,
        triangles,
        triangles_total,
        materialSlots,
        materialSlots_total,
        uniqueMaterials,
        uniqueMaterials_total,
        shaderCount,
        particleSystems,
        particleSystems_total,
        maxParticles,
        maxParticles_total,
        gameObjects,
        gameObjects_total;

        AvatarInfo()
        {
            cachedInfo = null;

            skinnedMeshRenders = 0;
            skinnedMeshRenders_total = 0;

            meshRenderers = 0;
            meshRenderers_total = 0;

            dynamicBoneTransforms = 0;
            dynamicBoneTransforms_total = 0;
            dynamicBoneColliders = 0;
            dynamicBoneColliders_total = 0;
            dynamicBoneColliderTransforms = 0;
            dynamicBoneColliderTransforms_total = 0;

            triangles = 0;
            triangles_total = 0;
            materialSlots = 0;
            materialSlots_total = 0;
            uniqueMaterials = 0;
            uniqueMaterials_total = 0;
            shaderCount = 0;

            particleSystems = 0;
            particleSystems_total = 0;
            maxParticles = 0;
            maxParticles_total = 0;

            gameObjects = 0;
            gameObjects_total = 0;
        }

        public AvatarInfo(GameObject o) : base()
        {
            if (o == null)
                return;

            name = o.name;

            var shaderHash = new HashSet<Shader>();
            var matList = new List<Material>();
            var matList_total = new List<Material>();

            var ts = o.GetComponentsInChildren<Transform>(true);
            foreach (var t in ts)
            {
                gameObjects_total += 1;
                if (t.gameObject.activeInHierarchy)
                    gameObjects += 1;
            }

            var sRenders = o.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var r in sRenders)
            {
                skinnedMeshRenders_total += 1;
                triangles_total += r.sharedMesh.triangles.Length / 3;

                if (r.gameObject.activeInHierarchy && r.enabled)
                {
                    skinnedMeshRenders += 1;
                    triangles += r.sharedMesh.triangles.Length / 3;
                }

                foreach (var mat in r.sharedMaterials)
                {
                    if (mat != null)
                    {
                        shaderHash.Add(mat.shader);
                        matList_total.Add(mat);

                        if (r.gameObject.activeInHierarchy && r.enabled)
                        {
                            matList.Add(mat);
                        }
                    }
                }
            }

            var renders = o.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var r in renders)
            {
                var filter = r.GetComponent<MeshFilter>();

                if (filter != null && filter.sharedMesh != null)
                {
                    meshRenderers_total += 1;
                    triangles_total += filter.sharedMesh.triangles.Length;

                    if (r.gameObject.activeInHierarchy && r.enabled)
                    {
                        meshRenderers += 1;
                        triangles += filter.sharedMesh.triangles.Length;
                    }
                }

                foreach (var mat in r.sharedMaterials)
                {
                    if (mat != null)
                    {
                        shaderHash.Add(mat.shader);
                        matList_total.Add(mat);

                        if (r.gameObject.activeInHierarchy && r.enabled)
                        {
                            matList.Add(mat);
                        }
                    }
                }
            }

            materialSlots = matList.Count;
            materialSlots_total = matList_total.Count;

            uniqueMaterials = new HashSet<Material>(matList).Count;
            uniqueMaterials_total = new HashSet<Material>(matList_total).Count;

#if !NO_BONES

            var dbColliders = o.GetComponentsInChildren<DynamicBoneCollider>(true);
            foreach (var c in dbColliders)
            {
                dynamicBoneColliders_total += 1;

                if (c.gameObject.activeInHierarchy)
                    dynamicBoneColliders += 1;
            }

            var dbones = o.GetComponentsInChildren<DynamicBone>(true);
            foreach (var d in dbones)
            {
                if (d.m_Root != null)
                {
                    var exclusions = d.m_Exclusions;
                    var rootChildren = d.m_Root.GetComponentsInChildren<Transform>(true);

                    int affected = 0;
                    int affected_total = 0;

                    foreach (var t in rootChildren)
                    {
                        if (exclusions.IndexOf(t) == -1)
                        {
                            affected_total += 1;

                            if (t.gameObject.activeInHierarchy && d.enabled)
                            {
                                affected += 1;
                            }
                        }
                        else
                        {
                            var childChildren = t.GetComponentsInChildren<Transform>(true);

                            for (int z = 1; z < childChildren.Length; z++)
                            {
                                affected_total -= 1;

                                if (childChildren[z].gameObject.activeInHierarchy && d.enabled)
                                {
                                    affected -= 1;
                                }
                            }
                        }
                    }

                    foreach (var c in d.m_Colliders)
                    {
                        if (c != null)
                        {
                            dynamicBoneColliderTransforms += affected;
                            dynamicBoneColliderTransforms_total += affected_total;
                            break;
                        }
                    }

                    dynamicBoneTransforms += affected;
                    dynamicBoneTransforms_total += affected_total;
                }
            }

#endif

            var ptc = o.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var p in ptc)
            {
                particleSystems_total += 1;
                maxParticles_total += p.main.maxParticles;

                if (p.gameObject.activeInHierarchy && p.emission.enabled)
                {
                    particleSystems += 1;
                    maxParticles += p.main.maxParticles;
                }
            }

            shaderCount = shaderHash.Count;
        }

        public static AvatarInfo GetInfo(GameObject o, out string toString)
        {
            AvatarInfo a = new AvatarInfo(o);
            toString = a.ToString();
            return a;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(cachedInfo))
                return cachedInfo;
            else
            {
                if (this == null)
                {
                    return null;
                }
                try
                {
                    cachedInfo = string.Format
                    (
                        LocalizationClass.Main.AvatarInfo_template,
                        name,
                        gameObjects,
                        gameObjects_total,
                        skinnedMeshRenders,
                        skinnedMeshRenders_total,
                        meshRenderers,
                        meshRenderers_total,
                        triangles,
                        triangles_total,
                        materialSlots,
                        materialSlots_total,
                        uniqueMaterials,
                        uniqueMaterials_total,
                        shaderCount,
                        dynamicBoneTransforms,
                        dynamicBoneTransforms_total,
                        dynamicBoneColliders,
                        dynamicBoneColliders_total,
                        dynamicBoneColliderTransforms,
                        dynamicBoneColliderTransforms_total,
                        particleSystems,
                        particleSystems_total,
                        maxParticles,
                        maxParticles_total
                    );
                }
                catch (Exception)
                {
                    cachedInfo = null;
                }
                return cachedInfo;
            }
        }


    }


    /// <summary>
    /// GUI控件状态类
    /// </summary>
    public class ControlState
    {
        /// <summary>
        /// 目标AvatarGobj
        /// </summary>
        public GameObject selectedAvatar;
        /// <summary>
        /// 添加缺失VRCD的自动口型选项
        /// </summary>
        public bool auto_Visemes = true;
        /// <summary>
        /// 添加缺失VRCD的自动视角球选项
        /// </summary>
        public bool auto_Viewpoint = true;
        public ControlState()
        {
            copier = new Copier();
            bindFunction = new BindFunction();
            litFunction = new LitFunction();
            preCheck = new PreCheck();
            materialFunction = new MaterialFunction();
            dynamicBoneFunction = new DynamicBoneFunction();
            colliderFunction = new ColliderFunction();
            avatarInfo = new AvatarInfo();
            misc = new Misc();


            fastMenu = new FastMenu();
        }
        public FastMenu fastMenu;

        public class FastMenu
        {
            public FastMenu()
            {

            }

            public bool enable = false;


        }




        public Copier copier;
        public class Copier
        {
            /// <summary>
            /// 复制源GameOBJ
            /// </summary>
            public GameObject copierSelectedFrom;
            public Copier()
            {
                transforms = new Transforms();
                dynamicBones = new DynamicBones();
                avatarDescriptor = new AvatarDescriptor();
                colliders = new Colliders();
                skinMeshRender = new SkinMeshRender();
            }


            public bool expand = false;

            public Transforms transforms;
            public class Transforms
            {
                public bool expand = false;
                public bool copy = true;
                public bool copyPosition = true;
                public bool copyRotation = true;
                public bool copyScale = true;
            }
            public DynamicBones dynamicBones;
            public class DynamicBones
            {
                public bool expand = false;
                public bool copy = true;
                public bool copySettings = true;
                public bool createMissingBones = true;
                public bool copyColliders = true;
                public bool removeOldColliders = true;
                public bool removeOldBones = true;
            }
            public AvatarDescriptor avatarDescriptor;
            public class AvatarDescriptor
            {
                public bool expand = false;
                public bool copy = true;
                public bool copySettings = true;
                public bool copyPipelineId = true;
                public bool copyAnimationOverrides = true;
            }
            public Colliders colliders;
            public class Colliders
            {
                public bool expand = false;
                public bool copy = true;
                public bool copyBox = true;
                public bool copyCapsule = true;
                public bool copySphere = true;
                public bool copyMesh = true;
                public bool removeOld = true;
            }
            public SkinMeshRender skinMeshRender;
            public class SkinMeshRender
            {
                public bool expand = false;
                public bool copy = true;
                public bool copySettings = true;
                public bool copyBlendShapeValues = true;
                public bool copyMaterials = true;
            }
        }
        public BindFunction bindFunction;
        public class BindFunction
        {
            public bool expand = false;
            public GameObject[] itemList;
            public GameObject targetGO;
            public GameObject itemRoot;
        }
        public PreCheck preCheck;
        public class PreCheck
        {
            public bool expand = true;

            public UploadRequired uploadRequired;
            public General general;

            public PreCheck()
            {
                uploadRequired = new UploadRequired();
                general = new General();
            }

            public class UploadRequired
            {
                public bool expand = true;
            }
            public class General
            {
                public bool expand = true;
            }
        }

        public LitFunction litFunction;
        public class LitFunction
        {
            public bool expand = false;
        }
        public MaterialFunction materialFunction;
        public class MaterialFunction
        {
            public bool expand = false;

            public MaterialFunction()
            {
                matAll = new MatAll();
                matArktoon = new MatArktoon();
                matPoiyomi = new MatPoiyomi();
            }
            public MatAll matAll;
            public MatArktoon matArktoon;
            public MatPoiyomi matPoiyomi;
            public class MatAll
            {
                public bool expand = false;
            }
            public class MatArktoon
            {
                public bool expand = false;
            }
            public class MatPoiyomi
            {
                public bool expand = false;
            }
        }
        public ColliderFunction colliderFunction;
        public class ColliderFunction
        {
            public ColliderFunction()
            {
                tDA = new TDA();
            }

            public bool expand = false;

            public TDA tDA = new TDA();
            public class TDA
            {
                public bool expand = false;
                public bool dynamicBoneColliderExpand = false;
                public bool clothColliderExpand = false;
            }
        }

        public DynamicBoneFunction dynamicBoneFunction;
        public class DynamicBoneFunction
        {
            public DynamicBoneFunction()
            {
                prefab = new Prefab();
                addCollider = new AddCollider();
                skirtCollider = new SkirtCollider();
                other = new Other();
            }

            public bool expand = false;

            public Prefab prefab;
            public AddCollider addCollider;
            public SkirtCollider skirtCollider;
            public Other other;
            public class Prefab 
            {
                public bool expand = false;
            }
            public class SkirtCollider
            {
                public bool expand = false;
                public string r = "0.00075";
                public string count = "8";
                public string colliderR = "0.01";
                public string colliderH = "0.04";
            }
            public class AddCollider
            {
                public bool expand = false;
            }
            public class Other
            {
                public bool expand = true;
            }
        }

        public AvatarInfo avatarInfo;
        public class AvatarInfo
        {
            public bool expand = false;
        }

        public Misc misc;
        public class Misc
        {
            public bool expand = false;
        }
    }

    //======================================================================================================⬆控件状态类⬆==================================================================================================
    

    /// <summary>
    /// GUILabel预设Styles
    /// </summary>
    public static class UIStyles
    {
        public static GUIStyle Foldout_title { get; private set; }
        public static GUIStyle BlackFirstFoldout_title { get; private set; }
        public static GUIStyle BlackSecondFoldout_title { get; private set; }
        public static GUIStyle WhiteFirstFoldout_title { get; private set; }
        public static GUIStyle WhiteSecondFoldout_title { get; private set; }
        public static GUIStyle Label_mainTitle { get; private set; }
        public static GUIStyle Label_centered { get; private set; }
        public static GUIStyle boxGuiStyle { get; private set; }
        //public static GUIStyle boxGuiStyleY { get; private set; }
        //public static GUIStyle boxGuiStyleG { get; private set; }

        static UIStyles()
        {
            boxGuiStyle = new GUIStyle();
            if (EditorGUIUtility.isProSkin)
            {
                boxGuiStyle.normal.background = CreateBackgroundColorImage(new Color(0.3f, 0.3f, 0.3f));
                boxGuiStyle.normal.textColor = Color.white;
            }
            else
            {
                boxGuiStyle.normal.background = CreateBackgroundColorImage(new Color(0.85f, 0.85f, 0.85f));
                boxGuiStyle.normal.textColor = Color.black;
            }

            ////238 226 149
            //boxGuiStyleY = new GUIStyle();
            //boxGuiStyleY.normal.background = CreateBackgroundColorImage(new Color(256 / 238F, 256 / 226F, 256 / 149F));
            //boxGuiStyleY.normal.textColor = Color.white;

            ////145 220 69
            //boxGuiStyleG = new GUIStyle();
            //boxGuiStyleG.normal.background = CreateBackgroundColorImage(new Color(256 / 145F, 256 / 220F, 256 / 69F));
            //boxGuiStyleG.normal.textColor = Color.white;

            Foldout_title = new GUIStyle("Foldout")
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
            };

            BlackFirstFoldout_title = new GUIStyle("PreDropDown")
            {
                fontSize = 13,
                fixedHeight = 26,
                fontStyle = FontStyle.Bold,
                //alignment = TextAnchor.MiddleCenter,
            };

            BlackSecondFoldout_title = new GUIStyle("PreDropDown")
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
            };

            WhiteFirstFoldout_title = new GUIStyle("ToolbarDropDown")
            {
                fontSize = 13,
                fixedHeight = 26,
                fontStyle = FontStyle.Bold,
                contentOffset = new Vector2(5f, 0),
            };
            WhiteSecondFoldout_title = new GUIStyle("PreDropDown")
            {
                fontSize = 12,
                //fixedHeight = 26,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                //contentOffset = new Vector2(5f, 0),
            };

            Label_mainTitle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
            };

            Label_centered = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.UpperCenter,
            };
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

    }

    /// <summary>
    /// Unity图标资源存储struct
    /// </summary>
    public struct Icons
    {
        //黄色星星
        public static Texture2D Star { get; private set; }
        public static Texture2D CsScript { get; private set; }
        public static Texture2D Transform { get; private set; }
        //半身图标
        public static Texture2D Avatar { get; private set; }
        public static Texture2D SkinnedMeshRenderer { get; private set; }
        public static Texture2D ColliderBox { get; private set; }
        public static Texture2D ColliderSphere { get; private set; }
        public static Texture2D ColliderCapsule { get; private set; }
        public static Texture2D DefaultAsset { get; private set; }
        public static Texture2D ParticleSystem { get; private set; }
        public static Texture2D Refresh { get; private set; }
        public static Texture2D CollabCreateIcon { get; private set; }
        public static Texture2D DeletedOBJ { get; private set; }
        public static Texture2D OutOfSync { get; private set; }
        public static Texture2D CollabMovedIcon { get; private set; }
        public static Texture2D CollabExcludeIcon { get; private set; }
        public static Texture2D CollabConflictIcon { get; private set; }
        public static Texture2D CollabChangesIcon { get; private set; }
        public static Texture2D CollabDeletedIcon { get; private set; }
        public static Texture2D SearchIcon { get; private set; }
        public static Texture2D PrefabIcon { get; private set; }
        public static Texture2D CollabChangesConflictIcon { get; private set; }
        public static Texture2D BoneIcon { get; private set; }
        public static Texture2D Enable { get; private set; }
        public static Texture2D Disable { get; private set; }
        public static Texture2D GameObject { get; private set; }
        public static Texture2D DynamicBoneCollider { get; private set; }
        public static Texture2D AvatarDescriptor { get; private set; }



        static Icons()
        {
            Star = EditorGUIUtility.FindTexture("Favorite Icon");
            CsScript = EditorGUIUtility.FindTexture("cs Script Icon");
            Transform = EditorGUIUtility.ObjectContent(null, typeof(Transform)).image as Texture2D;
            Avatar = EditorGUIUtility.ObjectContent(null, typeof(Avatar)).image as Texture2D;
            SkinnedMeshRenderer = EditorGUIUtility.ObjectContent(null, typeof(SkinnedMeshRenderer)).image as Texture2D; 
            ColliderBox = EditorGUIUtility.ObjectContent(null, typeof(BoxCollider)).image as Texture2D; 
            ColliderSphere = EditorGUIUtility.ObjectContent(null, typeof(SphereCollider)).image as Texture2D; 
            ColliderCapsule = EditorGUIUtility.ObjectContent(null, typeof(CapsuleCollider)).image as Texture2D; 
            DefaultAsset = EditorGUIUtility.FindTexture("DefaultAsset Icon");
            ParticleSystem = EditorGUIUtility.ObjectContent(null, typeof(ParticleSystem)).image as Texture2D;
            Refresh = EditorGUIUtility.FindTexture("Refresh");
            CollabCreateIcon = EditorGUIUtility.FindTexture("CollabCreate Icon");
            DeletedOBJ = EditorGUIUtility.FindTexture("d_P4_DeletedLocal");
            OutOfSync = EditorGUIUtility.FindTexture("d_P4_OutOfSync");
            CollabMovedIcon = EditorGUIUtility.FindTexture("CollabMoved Icon");
            CollabExcludeIcon = EditorGUIUtility.FindTexture("CollabExclude Icon");
            CollabConflictIcon = EditorGUIUtility.FindTexture("CollabConflict Icon");
            CollabChangesIcon = EditorGUIUtility.FindTexture("CollabChanges Icon");
            CollabDeletedIcon = EditorGUIUtility.FindTexture("CollabDeleted Icon");
            SearchIcon = EditorGUIUtility.FindTexture("Search Icon");
            PrefabIcon = EditorGUIUtility.FindTexture("Prefab Icon");
            CollabChangesConflictIcon = EditorGUIUtility.FindTexture("CollabChangesConflict Icon");
            BoneIcon = Resources.Load("Icon/BoneIcon") as Texture2D;
            Enable = Resources.Load("Icon/Enable+25") as Texture2D;
            Disable = Resources.Load("Icon/Disable") as Texture2D;
            DynamicBoneCollider = Resources.Load("Icon/BoneColliderIcon") as Texture2D;
            AvatarDescriptor = Resources.Load("Icon/AvatarDescriptorIcon - 副本") as Texture2D;
            GameObject = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image as Texture2D;
        }
    }

}