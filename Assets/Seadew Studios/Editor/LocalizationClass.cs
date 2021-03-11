using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SeadewStudios
{
    /// <summary>
    /// 语言本地化类
    /// </summary>
    public class LocalizationClass
    {
        public static readonly string version = "0.0.1a";
        readonly static Dictionary<string, string> dictionary_english, dictionary_uwu, dictionary_chinese;
        static Dictionary<string, string> stringDictionary;
        static DictionaryLanguage language;

        public enum DictionaryLanguage { English, Chinese, uwu = 100 };

        static public DictionaryLanguage Language
        {
            get { return language; }
            set
            {
                if (value != language)
                {
                    switch (value)
                    {
                        case DictionaryLanguage.English:
                            stringDictionary = dictionary_english;
                            break;
                        case DictionaryLanguage.Chinese:
                            stringDictionary = dictionary_chinese;
                            break;
                        case DictionaryLanguage.uwu:
                            stringDictionary = dictionary_uwu;
                            break;
                        default:
                            stringDictionary = dictionary_english;
                            break;
                    }
                    language = value;
                    ReloadStrings();
                }
            }
        }

        public static class Main
        {
            public static string Title { get; private set; }
            public static string WindowName { get; private set; }
            public static string Version { get; private set; }
            public static string SelectedAvatar { get; private set; }
            public static string BindFunction { get; private set; }
            public static string LitFunction { get; private set; }
            public static string MaterialFunction { get; private set; }
            public static string Copier { get; private set; }
            public static string RemoveAll { get; private set; }
            public static string AvatarInfo { get; private set; }
            public static string AvatarInfo_template { get; private set; }
            public static string Misc { get; private set; }

            static Main()
            {
                Reload();
            }

            public static void Reload()
            {
                SelectedAvatar = GetString("ui_main_avatar") ?? "_Avatar";
                Title = GetString("ui_main_title") ?? "_Pumkin's Avatar Tools (Beta)";
                Version = GetString("ui_main_version") ?? "_Version";
                WindowName = GetString("ui_main_windowName") ?? "_Avatar Tools";
                LitFunction = GetString("ui_litFunction") ?? "_LitFunction";
                BindFunction = GetString("ui_bindFunction") ?? "_BindFunction";
                MaterialFunction = GetString("ui_materialFunction") ?? "_MaterialFunction";
                Copier = GetString("ui_copier") ?? "_Copy Components";
                AvatarInfo = GetString("ui_avatarInfo") ?? "_Avatar Info";
                RemoveAll = GetString("ui_removeAll") ?? "_Remove All";
                Misc = GetString("ui_misc") ?? "_Misc";

                AvatarInfo_template = GetString("ui_avatarInfo_template") ??
                "_{0}\n---------------------\n" +
                "_GameObjects: {1} ({2})\n\n" +
                "_Skinned Mesh Renderers: {3} ({4})\n" +
                "_Mesh Renderers: {5} ({6})\n" +
                "_Triangles: {7} ({8})\n\n" +
                "_Used Material Slots: {9} ({10})\n" +
                "_Unique Materials: {11} ({12})\n" +
                "_Shaders: {13} \n\n" +
                "_Dynamic Bone Transforms: {14} ({15})\n" +
                "_Dynamic Bone Colliders: {16} ({17})\n" +
                "_Collider Affected Transforms: {17} ({19})\n\n" +
                "_Particle Systems: {20} ({21})\n" +
                "_Max Particles: {22} ({23})";
            }
        };
        public static class Buttons
        {
            public static string SelectFromScene { get; private set; }
            public static string CopySelected { get; private set; }
            public static string Cancel { get; private set; }
            public static string Apply { get; private set; }
            public static string Refresh { get; private set; }

            static Buttons()
            {
                Reload();
            }

            public static void Reload()
            {
                SelectFromScene = GetString("buttons_selectFromScene") ?? "_Select from Scene";
                CopySelected = GetString("buttons_copySelected") ?? "_Copy Selected";
                Refresh = GetString("buttons_refresh") ?? "_Refresh";
                Cancel = GetString("buttons_cancel") ?? "_Cancel";
                Apply = GetString("buttons_apply") ?? "_Apply";
            }
        };
        public static class bindFunction
        {
            public static string CloseOBJDynamicBone { get; private set; }

            static bindFunction()
            {
                Reload();
            }

            public static void Reload()
            {
                CloseOBJDynamicBone = GetString("ui_litFunction_closeDynamicBone") ?? "_Disable selected OBJ DynamicBone";
            }
        };
        public static class litFunction
        {
            public static string FillVisemes { get; private set; }
            public static string EditViewpoint { get; private set; }
            public static string ResetBlendshapes { get; private set; }
            public static string ResetPose { get; private set; }
            public static string SelectedDynamicBoneSetRoot { get; private set; }
            public static string SelectedOBJAddDynamicBone { get; private set; }
            public static string CloseAllDynamicBone { get; private set; }
            public static string OpenAllDynamicBone { get; private set; }
            public static string OpenOBJDynamicBone { get; private set; }
            public static string CloseOBJDynamicBone { get; private set; }

            static litFunction()
            {
                Reload();
            }

            public static void Reload()
            {
                FillVisemes = GetString("ui_litFunction_fillVisemes") ?? "_Fill Visemes";
                EditViewpoint = GetString("ui_litFunction_editViewpoint") ?? "_Edit Viewpoint";
                ResetBlendshapes = GetString("ui_litFunction_resetBlendShapes") ?? "_Reset Blendshapes";
                ResetPose = GetString("ui_litFunction_resetPose") ?? "_Reset Pose";
                SelectedDynamicBoneSetRoot = GetString("ui_litFunction_selectedDynamicBoneSetRootFromSelf") ?? "_Selected Dynamic Bone set root from self";
                SelectedOBJAddDynamicBone = GetString("ui_litFunction_selectedOBJAddDynamicBone") ?? "_Selected OBJ add DynamiBone";
                CloseAllDynamicBone = GetString("ui_litFunction_closeAllDynamicBone") ?? "_Disable Avatar all DynamicBone";
                OpenAllDynamicBone = GetString("ui_litFunction_openAllDynamicBone") ?? "_Enable Avatar all DynamicBone";
                OpenOBJDynamicBone = GetString("ui_litFunction_openDynamicBone") ?? "_Enable selected OBJ DynamicBone";
                CloseOBJDynamicBone = GetString("ui_litFunction_closeDynamicBone") ?? "_Disable selected OBJ DynamicBone";
            }
        };
        public static class Copier
        {
            public static string CopyFrom { get; private set; }
            public static string Transforms { get; private set; }
            public static string Transforms_position { get; private set; }
            public static string Transforms_rotation { get; private set; }
            public static string Transforms_scale { get; private set; }
            public static string DynamicBones { get; private set; }
            public static string DynamicBones_settings { get; private set; }
            public static string DynamicBones_colliders { get; private set; }
            public static string DynamicBones_removeOldBones { get; private set; }
            public static string DynamicBones_removeOldColliders { get; private set; }
            public static string DynamicBones_createMissingBones { get; private set; }
            public static string Colliders { get; private set; }
            public static string Colliders_box { get; private set; }
            public static string Colliders_capsule { get; private set; }
            public static string Colliders_sphere { get; private set; }
            public static string Colliders_mesh { get; private set; }
            public static string Colliders_removeOld { get; private set; }
            public static string Descriptor { get; private set; }
            public static string Descriptor_settings { get; private set; }
            public static string Descriptor_pipelineId { get; private set; }
            public static string Descriptor_animationOverrides { get; private set; }
            public static string SkinMeshRender { get; private set; }
            public static string SkinMeshRender_settings { get; private set; }
            public static string SkinMeshRender_materials { get; private set; }
            public static string SkinMeshRender_blendShapeValues { get; private set; }

            static Copier()
            {
                Reload();
            }

            public static void Reload()
            {
                CopyFrom = GetString("ui_copier_copyFrom") ?? "_Copy From";
                Transforms = GetString("ui_copier_transforms") ?? "_Transforms";
                Transforms_position = GetString("ui_copier_transforms_position") ?? "_Position";
                Transforms_rotation = GetString("ui_copier_transforms_rotation") ?? "_Rotation";
                Transforms_scale = GetString("ui_copier_transforms_scale") ?? "_Scale";
                DynamicBones = GetString("ui_copier_dynamicBones") ?? "_Dynamic Bones";
                DynamicBones_settings = GetString("ui_copier_dynamicBones_settings") ?? "_Settings";
                DynamicBones_colliders = GetString("ui_copier_dynamicBones_colliders") ?? "_Colliders";
                DynamicBones_removeOldBones = GetString("ui_copier_dynamicBones_removeOld") ?? "_Remove Old Bones";
                DynamicBones_removeOldColliders = GetString("ui_copier_dynamicBones_removeOldColliders") ?? "_Remove Old Colliders";
                DynamicBones_createMissingBones = GetString("ui_copier_dynamicBones_createMissing") ?? "_Create Missing Bones";
                Colliders = GetString("ui_copier_colliders") ?? "_Colliders";
                Colliders_box = GetString("ui_copier_colliders_box") ?? "_Box Colliders";
                Colliders_capsule = GetString("ui_copier_colliders_capsule") ?? "_Capsule Colliders";
                Colliders_sphere = GetString("ui_copier_colliders_sphere") ?? "_Sphere Colliders";
                Colliders_mesh = GetString("ui_copier_colliders_mesh") ?? "_Mesh Colliders";
                Colliders_removeOld = GetString("ui_copier_colliders_removeOld") ?? "_Remove Old Colliders";
                Descriptor = GetString("ui_copier_descriptor") ?? "_Avatar Descriptor";
                Descriptor_settings = GetString("ui_copier_descriptor_settings") ?? "_Settings";
                Descriptor_pipelineId = GetString("ui_copier_descriptor_pipelineId") ?? "_Pipeline Id";
                Descriptor_animationOverrides = GetString("ui_copier_descriptor_animationOverrides") ?? "_Animation Overrides";
                SkinMeshRender = GetString("ui_copier_skinMeshRender") ?? "_Skinned Mesh Renderers";
                SkinMeshRender_settings = GetString("ui_copier_skinMeshRender_settings") ?? "_Settings";
                SkinMeshRender_materials = GetString("ui_copier_skinMeshRender_materials") ?? "_Materials";
                SkinMeshRender_blendShapeValues = GetString("ui_copier_skinMeshRender_blendShapeValues") ?? "_BlendShape Values";
            }
        };
        public static class Log
        {
            public static string CopyAttempt { get; private set; }
            public static string RemoveAttempt { get; private set; }
            public static string CopyFromInvalid { get; private set; }
            public static string Done { get; private set; }
            public static string Failed { get; private set; }
            public static string CantCopyToSelf { get; private set; }
            public static string ViewpointApplied { get; private set; }
            public static string ViewpointCancelled { get; private set; }
            public static string Cancelled { get; private set; }
            public static string NoSkinnedMeshFound { get; private set; }
            public static string DescriptorIsNull { get; private set; }
            public static string Success { get; private set; }
            public static string TryFillVisemes { get; private set; }
            public static string TryRemoveUnsupportedComponent { get; private set; }
            public static string MeshHasNoVisemes { get; private set; }
            public static string FailedIsNull { get; private set; }

            static Log()
            {
                Reload();
            }

            public static void Reload()
            {
                Cancelled = GetString("log_cancelled") ?? "_Cancelled";
                CantCopyToSelf = GetString("log_cantCopyToSelf") ?? "_Can't copy Components from an object to itself.";
                CopyAttempt = GetString("log_copyAttempt") ?? "_Attempting to copy {0} from {1} to {2}";
                RemoveAttempt = GetString("log_removeAttempt") ?? "_Attempting to remove {0} from {1}";
                CopyFromInvalid = GetString("log_copyFromInvalid") ?? "_Can't copy Components because 'Copy From' is invalid";
                Done = GetString("log_done") ?? "_Done";
                ViewpointApplied = GetString("log_viewpointApplied") ?? "_Set Viewposition to {0}";
                ViewpointCancelled = GetString("log_viewpointCancelled") ?? "_Cancelled Viewposition changes";
                TryFillVisemes = GetString("log_tryFillVisemes") ?? "_Attempting to fill visemes on {0}";
                NoSkinnedMeshFound = GetString("log_noSkinnedMeshFound") ?? "_Failed: No skinned mesh found";
                DescriptorIsNull = GetString("log_descriptorIsNull") ?? "_Avatar descriptor is null";
                Success = GetString("log_success") ?? "_Success";
                MeshHasNoVisemes = GetString("log_meshHasNoVisemes") ?? "_Failed. Mesh has no Visemes. Set to Default";
                TryRemoveUnsupportedComponent = GetString("log_tryRemoveUnsupportedComponent") ?? "_Attempted to remove unsupported component {0} from {1}";
                Failed = GetString("log_failed") ?? "_Failed";
                FailedIsNull = GetString("log_failedIsNull") ?? "_Failed {1} is null";
            }
        };
        public static class Warning
        {
            public static string Warn { get; private set; }
            public static string NotFound { get; private set; }
            public static string SelectSceneObject { get; private set; }
            public static string OldVersion { get; private set; }

            static Warning()
            {
                Reload();
            }

            public static void Reload()
            {
                Warn = GetString("warn_warning") ?? "_Warning";
                NotFound = GetString("warn_notFound") ?? "_(Not Found)";
                OldVersion = GetString("warn_oldVersion") ?? "_(Old Version)";
                SelectSceneObject = GetString("warn_selectSceneObject") ?? "_Please select an object from the scene";
            }
        };
        public static class Credits
        {
            public static string C1 { get; private set; }

            static Credits()
            {
                Reload();
            }

            public static void Reload()
            {
                C1 = GetString("credits_C1") ?? "C1";
            }
        };
        public static class Misc
        {
            public static string uwu { get; private set; }
            public static string SearchForBones { get; private set; }

            private static string searchForBones;

            static Misc()
            {
                Reload();
            }

            public static void Reload()
            {
                uwu = GetString("misc_uwu") ?? "_uwu";
                SearchForBones = GetString("misc_searchForBones") ?? "_Search for DynamicBones";
            }
        }

        static LocalizationClass()
        {
            //Language Dictionaries
            dictionary_english = new Dictionary<string, string>
            {
#region Main
                //Main
                {"ui_main_title", "Seadew Studios Tool Kit" },
                {"ui_main_windowName", "Seadew Tool Kit" },
                {"ui_main_version", "Version" },
                {"ui_main_avatar", "Avatar" },
                {"ui_bindFunction", "bindFunction" },
                {"ui_litFunction", "litFunction" },
                {"ui_materialFunction", "MaterialSetting" },
                {"ui_copier", "Copy Components" },
                {"ui_avatarInfo", "Avatar Info" },
                {"ui_misc", "Misc" },
                {"ui_removeAll", "Remove All" },
                {
                    "ui_avatarInfo_template",

                    "{0}\n---------------------\n" +
                    "GameObjects: {1} ({2})\n\n" +
                    "Skinned Mesh Renderers: {3} ({4})\n" +
                    "Mesh Renderers: {5} ({6})\n" +
                    "△Triangles: {7} ({8})\n\n" +
                    "Used Material Slots: {9} ({10})\n" +
                    "Unique Materials: {11} ({12})\n" +
                    "Shaders: {13} \n\n"+
                    "Dynamic Bone Transforms: {14} ({15})\n" +
                    "Dynamic Bone Colliders: {16} ({17})\n" +
                    "Collider Affected Transforms: {17} ({19})\n\n" +
                    "Particle Systems: {20} ({21})\n" +
                    "Max Particles: {22} ({23})"
                },

#region Buttons
                {"buttons_selectFromScene", "Select from Scene" },
                {"buttons_copySelected" , "Copy Selected" },
                {"buttons_refresh", "Refresh" },
                {"buttons_apply", "Apply" },
                {"buttons_cancel", "Cancel" },
#endregion

#endregion
#region LitFunction
                //UI LitFunction              
                {"ui_litFunction_fillVisemes", "Fill Visemes" },
                {"ui_litFunction_editViewpoint", "Edit Viewpoint" },
                {"ui_litFunction_resetBlendShapes", "Reset Blendshapes" },
                {"ui_litFunction_resetPose", "Reset Pose" },
                {"ui_litFunction_selectedDynamicBoneSetRootFromSelf", "Selected DynamicBone set Root from Self" },
                {"ui_litFunction_selectedOBJAddDynamicBone","Selected OBJ add DynamiBone" },
                {"ui_litFunction_closeAllDynamicBone", "Disable Avatar all DynamicBone"},
                {"ui_litFunction_openAllDynamicBone", "Enable Avatar all DynamicBone"},
                {"ui_litFunction_openDynamicBone", "Enable selected OBJ DynamicBone"},
                {"ui_litFunction_closeDynamicBone", "Disable selected OBJ DynamicBone"},

        #endregion
        #region Copier
        //UI Copier
        { "ui_copier_copyFrom", "Copy from" },                

                //UI Copier Transforms
                {"ui_copier_transforms", "Transforms" },
                {"ui_copier_transforms_position", "Position" },
                {"ui_copier_transforms_rotation", "Rotation" },
                {"ui_copier_transforms_scale", "Scale" },
            
                //UI Copier Dynamic Bones
                {"ui_copier_dynamicBones", "Dynamic Bones" },
                {"ui_copier_dynamicBones_settings", "Settings" },
                {"ui_copier_dynamicBones_colliders", "Dynamic Bone Colliders" },
                {"ui_copier_dynamicBones_removeOld", "Remove Old Bones" },
                {"ui_copier_dynamicBones_removeOldColliders", "Remove Old Colliders" },
                {"ui_copier_dynamicBones_createMissing", "Create Missing Bones" },

                //UI Copier Colliders
                {"ui_copier_colliders", "Colliders" },
                {"ui_copier_colliders_box", "Box Colliders" },
                {"ui_copier_colliders_capsule", "Capsule Colliders" },
                {"ui_copier_colliders_sphere", "Sphere Colliders" },
                {"ui_copier_colliders_mesh", "Mesh Colliders" },
                {"ui_copier_colliders_removeOld", "Remove Old Colliders" },

                //UI Copier Avatar Descriptor
                {"ui_copier_descriptor", "VRC_Avatar Descriptor" },
                {"ui_copier_descriptor_settings", "Settings" },
                {"ui_copier_descriptor_pipelineId", "Pipeline Id" },
                {"ui_copier_descriptor_animationOverrides", "Animation Overrides" },

                //UI Copier Skinned Mesh Renderer
                {"ui_copier_skinMeshRender", "Skinned Mesh Renderers" },
                {"ui_copier_skinMeshRender_settings", "Settings" },
                {"ui_copier_skinMeshRender_materials", "Materials" },
                {"ui_copier_skinMeshRender_blendShapeValues", "BlendShape Values" },
#endregion

#region Log
                //Log
                { "log_failed", "Failed" },
                { "log_cancelled", "Cancelled" },
                { "log_success", "Success" },
                { "log_done", "Done. Check Unity Console for full Output Log" },
                { "log_copyAttempt", "Attempting to copy {0} from {1} to {2}" },
                { "log_removeAttempt", "Attempting to remove {0} from {1}" },
                { "log_copyFromInvalid", "Can't copy Components because 'Copy From' is invalid" },
                { "log_cantCopyToSelf", "Can't copy Components from an object to itself. What are you doing?" },
                { "log_viewpointApplied", "Set Viewposition to {0}" },
                { "log_viewpointCancelled", "Cancelled Viewposition changes" },
                { "log_tryFillVisemes", "Attempting to fill visemes on {0}" },
                { "log_noSkinnedMeshFound", "Failed: No skinned mesh found" },
                { "log_descriptorIsNull", "Avatar descriptor is null"},
                { "log_meshHasNoVisemes", "Failed. Mesh has no Visemes. Set to Default" },
                { "log_tryRemoveUnsupportedComponent", "Attempted to remove unsupported component {0} from {1}" },
                { "log_failedIsNull" , "Failed {1} is null. Ignoring." },
#endregion

#region Warnings
                //Warnings
                { "log_warning", "Warning" },
                { "warn_selectSceneObject" , "Please select an object from the scene" },
                { "warn_notFound", "(Not Found)" },
                { "warn_oldVersion", "(Old Version)" },
                //{ "warn_copyToPrefab", "You are trying to copy components to a prefab.\nThis cannot be undone.\nAre you sure you want to continue?" },
                //{ "warn_prefabOverwriteYes", "Yes, Overwrite" },
                //{ "warn_prefabOverwriteNo", "No, Cancel" },
#endregion

#region Credits
                //Credits
                { "credits_line1", "Seadew Studios Tool Kit"},
                { "credits_line2", "Version" + " " + version },
                { "credits_line3", "..."},
                { "credits_line4", ".." },
                { "credits_line5", "." },
#endregion

                //Misc                
                { "misc_uwu", "uwu" },
                { "misc_searchForBones", "Search for DynamicBones" },
            };
            //中文翻译文本集合
            dictionary_chinese = new Dictionary<string, string>
            {
#region Main
                //UI菜单主要部分
                {"ui_main_title", "Seadew Studios Tool Kit" },
                {"ui_main_windowName", "Seadew Tool Kit" },
                {"ui_main_version", "版本" },
                {"ui_main_avatar", "目标模型Avatar" },
                {"ui_bindFunction", "绑定相关功能-[Constraint]" },
                {"ui_litFunction", "杂项功能" },
                {"ui_materialFunction", "材质球设置项" },
                {"ui_copier", "复制组件面板" },
                {"ui_avatarInfo", "模型信息" },
                {"ui_misc", "关于Seadew Tool Kit" },
                {"ui_removeAll", "清理部分-删除所有目标的:" },
                {
                    "ui_avatarInfo_template",

                    "{0}\n---------------------\n" +
                    "物体数量(GameObjects): {1} ({2})\n\n" +
                    "蒙皮网格渲染器(Skinned Mesh Renderers): {3} ({4})\n" +
                    "网格渲染器(Mesh Renderers): {5} ({6})\n" +
                    "△三角形面数(Triangles): {7} ({8})\n\n" +
                    "已使用的材质球槽(Used Material Slots): {9} ({10})\n" +
                    "独立材质球(Unique Materials): {11} ({12})\n" +
                    "着色器(Shaders): {13} \n\n"+
                    "动态骨骼影响骨骼数量(Dynamic Bone Transforms): {14} ({15})\n" +
                    "动态骨骼碰撞数量(Dynamic Bone Colliders): {16} ({17})\n" +
                    "动态骨骼碰撞影响骨骼数量(Collider Affected Transforms): {17} ({19})\n\n" +
                    "粒子系统(Particle Systems): {20} ({21})\n" +
                    "最大粒子数量(Max Particles): {22} ({23})"
                },

#region Buttons
                {"buttons_selectFromScene", "从场景(Scene)中选择Avatar" },
                {"buttons_copySelected" , "开始复制选定类型的组件" },
                {"buttons_refresh", "刷新" },
                {"buttons_apply", "应用" },
                {"buttons_cancel", "取消" },
#endregion

#endregion
#region LitFunction
                //小功能UI部分      
                {"ui_litFunction_fillVisemes", "填充[口型]" },
                {"ui_litFunction_editViewpoint", "调整[视角球]" },
                {"ui_litFunction_resetBlendShapes", "重置表情形态键(Blendshapes)数值" },
                {"ui_litFunction_resetPose", "重置姿势" },
                {"ui_litFunction_selectedDynamicBoneSetRootFromSelf", "将选择的动骨Root设置为自身" },
                {"ui_litFunction_selectedOBJAddDynamicBone","为选择的对象添加动骨组件" },
                {"ui_litFunction_closeAllDynamicBone", "关闭Avatar所有动骨"},
                {"ui_litFunction_openAllDynamicBone", "开启Avatar所有动骨"},
                {"ui_litFunction_openDynamicBone", "开启选择的动骨组件"},
                {"ui_litFunction_closeDynamicBone", "关闭选择的动骨组件"},
                
#endregion
#region Copier
                //复制组件分类部分
                {"ui_copier_copyFrom", "复制来源Avatar" },                

                //Transforms分类
                {"ui_copier_transforms", "外观(Transforms)" },
                {"ui_copier_transforms_position", "位置(Position)" },
                {"ui_copier_transforms_rotation", "旋转(Rotation)" },
                {"ui_copier_transforms_scale", "大小(Scale)" },
            
                //Dynamic Bones分类
                {"ui_copier_dynamicBones", "动态骨骼(Dynamic Bones)" },
                {"ui_copier_dynamicBones_settings", "参数" },
                {"ui_copier_dynamicBones_colliders", "动态骨骼碰撞(Dynamic Bone Colliders)" },
                {"ui_copier_dynamicBones_removeOld", "删除旧的动态骨骼组件" },
                {"ui_copier_dynamicBones_removeOldColliders", "删除旧的动态骨骼碰撞" },
                {"ui_copier_dynamicBones_createMissing", "创建缺少的动态骨骼组件" },

                //Colliders分类
                {"ui_copier_colliders", "碰撞体(Colliders)" },
                {"ui_copier_colliders_box", "盒状碰撞(Box Colliders)" },
                {"ui_copier_colliders_capsule", "胶囊体碰撞(Capsule Colliders)" },
                {"ui_copier_colliders_sphere", "球状碰撞(Sphere Colliders)" },
                {"ui_copier_colliders_mesh", "网格碰撞(Mesh Colliders)" },
                {"ui_copier_colliders_removeOld", "删除旧的碰撞体" },
                
                //Avatar Descriptor分类
                {"ui_copier_descriptor", "VRC模型描述插件(VRC_Avatar Descriptor)" },
                {"ui_copier_descriptor_settings", "参数" },
                {"ui_copier_descriptor_pipelineId", "上传蓝图ID" },
                {"ui_copier_descriptor_animationOverrides", "动画改写控制器(Animation Overrides)" },

                //Skinned Mesh Renderer分类
                {"ui_copier_skinMeshRender", "网格渲染器(Skinned Mesh Renderers)" },
                {"ui_copier_skinMeshRender_settings", "参数" },
                {"ui_copier_skinMeshRender_materials", "材质球(Materials)" },
                {"ui_copier_skinMeshRender_blendShapeValues", "表情形态键数值(BlendShape Values)" },
#endregion

#region Log
                //输出日记部分(Log)
                { "log_failed", "已失败" },
                { "log_cancelled", "已取消" },
                { "log_success", "成功" },
                { "log_done", "已完成，请检查Unity控制台的完整输出日志(Log)" },
                { "log_copyAttempt", "试图将<{0}>从{1}复制到{2}" },
                { "log_removeAttempt", "正在尝试从{1}中删除{0}" },
                { "log_copyFromInvalid", "无法复制组件，因为“源目标”无效(invalid) "},
                { "log_cantCopyToSelf", "无法将组件从对象复制到其自身...请不要干傻事..谢谢wwww" },
                { "log_viewpointApplied", "已将视角球设置为{0}" },
                { "log_viewpointCancelled", "已取消更改视角球位置操作" },
                { "log_tryFillVisemes", "试图在{0}上填充口型" },
                { "log_noSkinnedMeshFound", "失败：未找到蒙皮网格(skinned mesh)" },
                { "log_descriptorIsNull", "VRC模型描述组件(Avatar descriptor)不存在"},
                { "log_meshHasNoVisemes", "已失败，网格(mesh)没有口型(Visemes)，将设置为默认值" },
                { "log_tryRemoveUnsupportedComponent", "试图从{1}中删除不受支持的组件{0}" },
                { "log_failedIsNull" , "\n操作已失败。目标<{1}>为空对象，忽略本次操作" },
#endregion

#region Warnings
                //警告部分
                { "log_warning", "警告" },
                { "warn_selectSceneObject" , "请从场景(scene)中选择一个对象(object)" },
                { "warn_notFound", "(未找到)" },
                { "warn_oldVersion", "(旧版本)" },
                { "warn_copyToPrefab", "您正试图将组件复制到预置体中，\n这无法撤消！\n确实要继续吗？" },//
                { "warn_prefabOverwriteYes", "是的，覆盖吧" },//
                { "warn_prefabOverwriteNo", "不，取消操作" },//
#endregion

#region Credits
                //工具包相关信息  
                { "credits_line1", "Seadew Tool Kit"},
                { "credits_line2", "版本" + " " + version },
                { "credits_line3", "。。。"},
                { "credits_line4", "。。" },
                { "credits_line5", "。" },
#endregion

                //杂项                
                { "misc_uwu", "uwu" },
                { "misc_searchForBones", "Search for DynamicBones" },
            };
            //Mistakes
            dictionary_uwu = new Dictionary<string, string>
            {
#region Main
                //Main
                {"ui_main_title", "Pumkin's Avataw Awoos! ÒwÓ" },
                {"ui_main_windowName", "Avataw Awoos" },
                {"ui_main_version", "Vewsion~" },
                {"ui_main_avatar", "Avataw :o" },
                {"ui_litFunction", "Toows òwó" },
                {"ui_materialFunction", "ui_materialFunction" },
                {"ui_copier", "Copy Componyents uwu" },
                {"ui_avatarInfo", "Avataw Info 0w0" },
                {"ui_misc", "Misc ;o" },
                {"ui_removeAll", "Wemuv Aww (⁰д⁰ )" },
                {
                    "ui_avatarInfo_template",

                    "{0}\n---------------------\n" +
                    "GameObjects: {1} ({2})\n\n" +
                    "Skinnyed Mesh Wendewews: {3} ({4})\n" +
                    "Mesh Wendewews: {5} ({6})\n" +
                    "Twiangwes: {7} ({8})\n\n" +
                    "Used Matewiaw Swots: {9} ({10})\n" +
                    "Unyique Matewiaws: {11} ({12})\n" +
                    "Shadews: {11} \n\n"+
                    "Dynyamic Bonye Twansfowms: {12} ({13})\n" +
                    "Dynyamic Bonye Cowwidews: {14} ({15})\n" +
                    "Cowwidew Affected Twansfowms: {16} ({17})\n\n" +
                    "Pawticwe Systems: {18} ({19})\n" +
                    "Max Pawticwes: {20} ({21})\n\n" +
                    "owos: 12000 (42000)\n" +
                    "uwus: 10"
                },

#region Buttons
                {"buttons_selectFromScene", "Sewect fwom Scenye x3" },
                {"buttons_copySelected" , "Copy Sewected (´• ω •`)" },
                {"buttons_refresh", "Wefwesh (ﾟωﾟ;)" },
                {"buttons_apply", "Appwy （>﹏<）" },
                {"buttons_cancel", "Cancew ; o;" },
#endregion

#endregion
#region LitFunction
                //UI Toows                
                {"ui_litFunctions_fillVisemes", "Fiww Visemes ;~;" },
                {"ui_litFunctions_editViewpoint", "Edit Viewpoint o-o" },
                {"ui_litFunctions_resetBlendShapes", "Weset Bwendshapes uwu" },
                {"ui_litFunctions_resetPose", "Weset Pose ;3" },

#endregion
#region Copier
                //UI Copier
                {"ui_copier_copyFrom", "Copy fwom~" },                

                //UI Copier Transforms
                {"ui_copier_transforms", "Twansfowms!" },
                {"ui_copier_transforms_position", "Position~" },
                {"ui_copier_transforms_wotation", "Wotation @~@" },
                {"ui_copier_transforms_scawe", "Scawe www" },

                //UI Copier Dynamic Bones
                {"ui_copier_dynamicBones", "Dynyamic Bonyes~" },
                {"ui_copier_dynamicBones_settings", "Settings º ^º" },
                {"ui_copier_dynamicBones_colliders", "Dynyamic Bonye Cowwidews~" },
                {"ui_copier_dynamicBones_removeOld", "Wemuv Owd Bonyes uwu" },
                {"ui_copier_dynamicBones_removeOldColliders", "Wemuv Owd Cowwidews ;w;" },
                {"ui_copier_dynamicBones_createMissing", "Cweate Missing Bonyes!" },

                //UI Copier Colliders
                {"ui_copier_colliders", "Cowwidews ;o;" },
                {"ui_copier_colliders_box", "Box Cowwidews!" },
                {"ui_copier_colliders_capsule", "Capsuwe Cowwidews o-o" },
                {"ui_copier_colliders_sphere", "Sphewe Cowwidews O~O" },
                {"ui_copier_colliders_mesh", "Mesh Cowwidews zzz" },
                {"ui_copier_colliders_removeOld", "Wemuv Owd Cowwidews uwu" },

                //UI Copier Avatar Descriptor
                {"ui_copier_descriptor", "Avataw Descwiptow~" },
                {"ui_copier_descriptor_settings", "Settings agen" },
                {"ui_copier_descriptor_pipelineId", "Pipewinye Id!" },
                {"ui_copier_descriptor_animationOverrides", "Anyimation Ovewwides :o" },

                //UI Copier Skinned Mesh Renderer
                {"ui_copier_skinMeshRender", "Skinnyed Mesh Wendewews ;w;" },
                {"ui_copier_skinMeshRender_settings", "Settings ageeen" },
                {"ui_copier_skinMeshRender_materials", "Matewiaws uwu" },
                {"ui_copier_skinMeshRender_blendShapeValues", "BwendShape Vawues ùwú" },
#endregion

#region Log
                //Log
                { "log_failed", "Faiwed ùwú" },
                { "log_cancelled", "Cancewwed .-." },
                { "log_success", "Success OWO" },
                { "log_done", "Donye. Check Unyity Consowe fow fuww Output Wog uwus" },
                { "log_copyAttempt", "Attempting to copy {0} fwom {1} to {2} o-o" },
                { "log_remuveAttempt", "Attempting to wemuv {0} fwom {1} ;-;" },
                { "log_copyFromInvalid", "Can't copy Componyents because 'Copy Fwom' is invawid ; o ;" },
                { "log_cantCopyToSelf", "Can't copy Componyents fwom an object to itsewf. What awe you doing? ;     w     ;" },
                { "log_viewpointApplied", "Set Viewposition to {0}!" },
                { "log_viewpointCancelled", "Cancewwed Viewposition changes uwu" },
                { "log_tryFixVisemes", "Attempting to fiww visemes on {0}!" },
                { "log_noSkinnedMeshFound", "Faiwed: Nyo skinnyed mesh found ;o;" },
                { "log_descriptorIsNull", "Avataw descwiptow is nyuww humpf"},
                { "log_meshHasNoVisemes", "Faiwed. Mesh has nyo Visemes. Set to Defauwt ;w;" },
                { "log_tryRemoveUnsupportedComponent", "Attempted to wemuv unsuppowted componyent {0} fwom {1} uwu7" },
                { "log_failedIsNull" , "Faiwed {1} is nyull /w\\. Ignyowing uwu" },
#endregion

#region Warnings
                //Warnings
                { "log_warning", "Wawnying! unu" },
                { "warn_selectSceneObject" , "Pwease sewect an object fwom the scenye!!" },
                { "warn_notFound", "(Nyot Fownd ;~;)" },
                { "warn_oldVersion", "(Old Version)" },
#endregion

#region Credits
                //Credits
                { "credits_line1", "Pumkin's Avataw Awoos~ :3"},
                { "credits_line2", "Vewsion" + " " + version },
                { "credits_line3", "Nyow with 0W0% mowe noticin things~"},
                { "credits_line4", "I'ww add mowe stuff to this eventuawwy >w<" },
                { "credits_line5", "Poke me! But on Discowd at Pumkin#2020~ uwus" },
#endregion

                //Misc                
                { "misc_uwu", "OwO" },
                { "misc_searchForBones", "Seawch fow DynyamicBonyes" },
            };

            stringDictionary = dictionary_chinese;
            language = DictionaryLanguage.Chinese;
            ReloadStrings();
        }

        static void ReloadStrings()
        {
            Main.Reload();
            Buttons.Reload();
            litFunction.Reload();
            Copier.Reload();
            Log.Reload();
            Warning.Reload();
            Credits.Reload();
            Misc.Reload();
        }

        static string GetString(string stringName)//, params string[] formatArgs)
        {
            if (string.IsNullOrEmpty(stringName))
                return stringName;

            string s = string.Empty;
            stringDictionary.TryGetValue(stringName, out s);

            /*if(formatArgs.Length > 0)
            {
                if(!string.IsNullOrEmpty(s))
                {
                    s = string.Format(stringName, formatArgs);
                }
            }*/
            return s;
        }
    }

}




