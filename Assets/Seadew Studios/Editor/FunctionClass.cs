using System.Collections.Generic;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using VRC.Core;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using UnityEngine;


//关于改动 1395行左右SMR函数修改: 干掉了SMR来源为空就抛出警告LOG的功能

namespace SeadewStudios
{
    /// <summary>
    /// 主功能类
    /// </summary>
    public class FunctionClass
    {
        static string SSS = "<color=#1B2CD8FF>[SeadewStudiosTK]</color>";


        static readonly Type[] supportedComponents =
        {
#if !NO_BONES
            typeof(DynamicBone),
            typeof(DynamicBoneCollider),
            typeof(DynamicBoneColliderBase),
#endif
            typeof(Collider),
            typeof(BoxCollider),
            typeof(CapsuleCollider),
            typeof(SphereCollider),
            typeof(MeshCollider),
            typeof(SkinnedMeshRenderer),
            typeof(Transform),
            typeof(VRCAvatarDescriptor),
            typeof(PipelineManager)
        };

        /// <summary>
        /// 查询VRCAvatarDescriptor是否存在
        /// </summary>
        /// <param name="Targe">查询目标</param>
        /// <returns>返回结果 T有 F无</returns>
        public static bool CheckAvatarDescriptor(GameObject Targe)
        {
            var dFrom = Targe.GetComponent<VRCAvatarDescriptor>();

            if (dFrom == null)
                return false;
            else
                return true;
        }

        

        /// <summary>
        /// 在VRCAvatarDescriptor中填充口型
        /// </summary>
        /// <param name="avatar">含有AvatarDescriptor组件的目标OBJ</param>
        public static void FillVisemes(GameObject avatar)
        {
            string log = LocalizationClass.Log.TryFillVisemes + " - ";
            string logFormat = avatar.name;

            string[] visemes =
                {
                    "vrc.v_sil",
                    "vrc.v_pp",
                    "vrc.v_ff",
                    "vrc.v_th",
                    "vrc.v_dd",
                    "vrc.v_kk",
                    "vrc.v_ch",
                    "vrc.v_ss",
                    "vrc.v_nn",
                    "vrc.v_rr",
                    "vrc.v_aa",
                    "vrc.v_e",
                    "vrc.v_ih",
                    "vrc.v_oh",
                    "vrc.v_ou",
                };

            var d = avatar.GetComponent<VRCAvatarDescriptor>();
            if (d == null)
            {
                d = avatar.AddComponent<VRCAvatarDescriptor>();
                d.VisemeBlendShapes = new string[visemes.Length];
            }

            var render = avatar.GetComponentInChildren<SkinnedMeshRenderer>();

            if (render == null)
            {
                log += LocalizationClass.Log.NoSkinnedMeshFound;
                Log(log, LogType.Error, logFormat);
            }

            d.VisemeSkinnedMesh = render;

            if (render.sharedMesh.blendShapeCount > 0)
            {
                d.lipSync = VRCAvatarDescriptor.LipSyncStyle.VisemeBlendShape;

                if (d.VisemeBlendShapes == null)
                {
                    d.VisemeBlendShapes = new string[visemes.Length];
                }
                for (int z = 0; z < visemes.Length; z++)
                {
                    string s = "-none-";
                    if (render.sharedMesh.GetBlendShapeIndex(visemes[z]) != -1)
                        s = visemes[z];
                    d.VisemeBlendShapes[z] = s;
                }

                log += LocalizationClass.Log.Success;
                Log(log, LogType.Log, logFormat);
            }
            else
            {
                d.lipSync = VRCAvatarDescriptor.LipSyncStyle.Default;
                log += LocalizationClass.Log.MeshHasNoVisemes;
                Log(log, LogType.Warning, logFormat);
            }
        }


        /// <summary>
        /// Reset transforms to prefab
        /// </summary>        
        public static void ResetPose(GameObject objTo)
        {
            string toPath = GetGameObjectPath(objTo);
            var pref = PrefabUtility.GetPrefabParent(objTo.transform.root.gameObject) as GameObject;
            Transform tr = pref.transform.Find(toPath);

            if (tr == null)
                return;

            GameObject objFrom = tr.gameObject;

            if (objTo.transform != objTo.transform.root)
            {
                objTo.transform.localPosition = objFrom.transform.localPosition;
                objTo.transform.localEulerAngles = objFrom.transform.localEulerAngles;
                objTo.transform.localRotation = objFrom.transform.localRotation;
            }

            //Loop through Children
            for (int i = 0; i < objFrom.transform.childCount; i++)
            {
                var fromChild = objFrom.transform.GetChild(i).gameObject;
                var t = objTo.transform.Find(fromChild.name);

                if (t == null)
                    continue;

                var toChild = t.gameObject;

                if (fromChild != null && toChild != null)
                {
                    ResetPose(toChild);
                }
            }
        }


        /// <summary>
        /// 把蒙皮网格渲染器中的所有形态键数值重置为0
        /// </summary>        
        public static void ResetBlendShapes(GameObject objTo)
        {
            var renders = objTo.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var r in renders)
            {
                ResetBlendShapes(r);
            }
        }
        /// <summary>
        /// 把蒙皮网格渲染器中的所有形态键数值重置为0
        /// </summary>        
        public static void ResetBlendShapes(SkinnedMeshRenderer render)
        {
            for (int i = 0; i < render.sharedMesh.blendShapeCount; i++)
            {
                render.SetBlendShapeWeight(i, 0);
            }
        }

        /// <summary>
        /// St 编辑视角球坐标
        /// </summary>        
        public static void BeginEditViewpoint(GameObject avatar)
        {
            //Editing Viewpoint
            bool _edittingView = false;
            Vector3 _viewPos;
            Vector3 _viewPosOld;
            VRCAvatarDescriptor _viewPos_descriptor;

            _viewPos_descriptor = avatar.GetComponent<VRCAvatarDescriptor>();
            if (_viewPos_descriptor == null)
            {
                _viewPos_descriptor = avatar.AddComponent<VRCAvatarDescriptor>();
            }

            Vector3 defaultView = new Vector3(0, 1.6f, 0.2f);
            _viewPos = _viewPos_descriptor.ViewPosition;
            _viewPosOld = _viewPos_descriptor.ViewPosition;

            //if (_viewPos == defaultView)
            //{
                var anim = avatar.GetComponent<Animator>();

                if (anim != null && anim.isHuman)
                {
                    //Old
                    _viewPos = anim.GetBoneTransform(HumanBodyBones.Head).position + new Vector3(0, 0, /*defaultView.z*/0.01F);

                    float eyeHeight = anim.GetBoneTransform(HumanBodyBones.LeftEye).position.y;
                    float eyeZ = anim.GetBoneTransform(HumanBodyBones.LeftEye).position.z-avatar.transform.position.z;
                    _viewPos.y = eyeHeight;
                    _viewPos.z += eyeZ;
                    _viewPos.x = 0;

                _viewPos_descriptor.ViewPosition = _viewPos /*+ avatar.transform.position*/;
                }
            //}
            _edittingView = true;
        }

        /// <summary>
        /// 将选择的动骨组件的Root骨设置为自身
        /// </summary>
        public static void SelectedDynamicBoneSetRootFromSelf(GameObject[] gameObjectArray)
        {
            foreach (var gO in gameObjectArray)
            {
                gO.GetComponent<DynamicBone>().m_Root = gO.transform;
                //Debug.Log("<color=#20A13CFF>[Succ]</color>Set:" + gO.name);
            }
            Debug.Log("<color=#1B2CD8FF>[SeadewStudiosTK]</color>动骨Root骨设置完毕");
        }
        /// <summary>
        /// 将选择的OBJ添加基础动骨组件
        /// </summary>
        public static void SelectedOBJAddDynamicBone(GameObject[] gameObjectArray)
        {
            foreach (var gO in gameObjectArray)
            {
                gO.AddComponent<DynamicBone>();
                //Debug.Log("<color=#FF00E3FF>[Shatoo's Tool]</color>Set:" + gO.name);
            }
            Debug.Log("<color=#1B2CD8FF>[SeadewStudiosTK]</color>动骨组件添加完毕");
        }

        /// <summary>
        /// 关闭目标OBJ下所有动骨
        /// </summary>
        public static void CloseAllDynamicBone(GameObject targeOBJ)
        {
            ChangeOBJDynamicBoneState(targeOBJ, false);
            for (int i = 0; i < targeOBJ.transform.childCount; i++)
            {
                var fromChild = targeOBJ.transform.GetChild(i).gameObject;
                if (fromChild != null)
                {
                    CloseAllDynamicBone(fromChild);
                }
            }
        }
        /// <summary>
        /// 打开目标OBJ下所有动骨
        /// </summary>
        public static void OpenAllDynamicBone(GameObject targeOBJ)
        {
            ChangeOBJDynamicBoneState(targeOBJ, true);
            for (int i = 0; i < targeOBJ.transform.childCount; i++)
            {
                var fromChild = targeOBJ.transform.GetChild(i).gameObject;
                if (fromChild != null)
                {
                    OpenAllDynamicBone(fromChild);
                }
            }
        }

        /// <summary>
        /// 打开选择的动骨
        /// </summary>
        /// <param name="gameObjectArray">传入包含动骨的OBJ组</param>
        public static void OpenOBJDynamicBone(GameObject[] gameObjectArray)
        {
            ChangeOBJDynamicBoneState(gameObjectArray, true);
        }
        /// <summary>
        /// 关闭选择的动骨
        /// </summary>
        /// <param name="gameObjectArray">传入包含动骨的OBJ组</param>
        public static void CloseOBJDynamicBone(GameObject[] gameObjectArray)
        {
            ChangeOBJDynamicBoneState(gameObjectArray, false);
        }


        /// <summary>
        /// 改变目标OBJ组中动骨组件的状态
        /// </summary>
        /// <param name="gameObjectArray">目标OBJ组</param>
        /// <param name="state">开启/关闭状态 T/F</param>
        public static void ChangeOBJDynamicBoneState(GameObject[] gameObjectArray, bool state)
        {
            var DBList = new List<DynamicBone>();

            foreach (var gO in gameObjectArray)
            {
                DBList.AddRange(gO.GetComponents<DynamicBone>());
                if (DBList == null)
                {
                    Log(SSS + gO.name + "中不包含动骨", LogType.Warning);
                }
                else
                {
                    foreach (var dB in DBList)
                    {
                        dB.enabled = state;
                    }
                }
                DBList.Clear();
            }
        }
        /// <summary>
        /// 改变目标OBJ中动骨组件的状态
        /// </summary>
        /// <param name="gameObject">目标OBJ</param>
        /// <param name="state">开启/关闭状态 T/F</param>
        public static void ChangeOBJDynamicBoneState(GameObject gameObject, bool state)
        {
            var DBList = new List<DynamicBone>();

            DBList.AddRange(gameObject.GetComponents<DynamicBone>());
            if (DBList == null)
            {
                Log(SSS + gameObject.name + "中不包含动骨", LogType.Warning);
            }
            else
            {
                foreach (var dB in DBList)
                {
                    dB.enabled = state;
                }
            }
        }



        /// <summary>
        /// 复制组件
        /// </summary>
        /// <param name="objFrom">OBJ来源</param>
        /// <param name="objTo">OBJ目标</param>
        public void CopyComponents(GameObject objFrom, GameObject objTo, ControlState cState)
        {
            string log = "";
            //Cancel Checks
            if (objFrom == objTo)
            {
                log += LocalizationClass.Log.CantCopyToSelf;
                Log(log, LogType.Warning);
                return;
            }

            //Pre Copying Operations

            //只在根级运行一次
            //Run statment only if root so only run this once
            if (objTo.transform == objTo.transform.root)
            {
                //动骨总IF
                if (cState.copier.dynamicBones.copy)
                {
                    //删除旧动骨碰撞IF
                    if (cState.copier.dynamicBones.removeOldColliders)
                        DestroyAllDynamicBoneColliders(objTo);
                    //删除旧动骨组件IF
                    if (cState.copier.dynamicBones.removeOldBones)
                        DestroyAllDynamicBones(objTo);
                }
                //碰撞体总IF
                if (cState.copier.colliders.copy)
                {
                    //删除旧碰撞IF
                    if (cState.copier.colliders.removeOld)
                        DestroyAllColliders(objTo);
                }
                //碰撞体总IF
                if (cState.copier.colliders.copy)
                {
                    CopyAvatarDescriptor(objFrom, objTo, cState);
                }
            }
            //End run once

            //
            if (cState.copier.transforms.copy)
            {
                CopyTransforms(objFrom, objTo, cState);
            }
            if (cState.copier.dynamicBones.copy)
            {
                if (cState.copier.dynamicBones.copySettings || cState.copier.dynamicBones.copyColliders)
                {
                    CopyDynamicBones(objFrom, objTo, cState, cState.copier.dynamicBones.createMissingBones);
                }
            }
            if (cState.copier.colliders.copy)
            {
                CopyColliders(objFrom, objTo, cState);
            }
            if (cState.copier.skinMeshRender.copy)
            {
                CopySkinMeshRenderer(objFrom, objTo, cState);
            }

            //复制子项递归
            for (int i = 0; i < objFrom.transform.childCount; i++)
            {
                var fromChild = objFrom.transform.GetChild(i).gameObject;
                var t = objTo.transform.Find(fromChild.name);

                if (t == null)
                {
                    t = AddNewOBJ(objTo, fromChild.name);
                    //continue;
                }

                var toChild = t.gameObject;

                if (fromChild != null && toChild != null)
                {
                    CopyComponents(fromChild, toChild, cState);//递归
                }
            }
        }

        #region 复制组件分类处理函数
        /// <summary>
        /// 通过AvatarDescriptor和PipelineManager组件进行复制
        /// </summary>
        void CopyAvatarDescriptor(GameObject from, GameObject to, ControlState cState)
        {
            var dFrom = from.GetComponent<VRCAvatarDescriptor>();
            var pFrom = from.GetComponent<PipelineManager>();
            var dTo = to.GetComponent<VRCAvatarDescriptor>();

            if (dFrom == null)
                return;
            if (dTo == null)
                dTo = to.AddComponent<VRCAvatarDescriptor>();

            var pTo = to.GetComponent<PipelineManager>();

            if (pTo == null) //but it shouldn't be
                pTo = to.AddComponent<PipelineManager>();

            if (cState.copier.avatarDescriptor.copyPipelineId)
            {
                pTo.blueprintId = pFrom.blueprintId;
                pTo.enabled = pFrom.enabled;
                pTo.completedSDKPipeline = true;

                EditorUtility.SetDirty(pTo);
                EditorSceneManager.MarkSceneDirty(pTo.gameObject.scene);
                EditorSceneManager.SaveScene(pTo.gameObject.scene);
            }

            if (cState.copier.avatarDescriptor.copySettings)
            {
                dTo.Animations = dFrom.Animations;
                dTo.apiAvatar = dFrom.apiAvatar;
                dTo.lipSync = dFrom.lipSync;
                dTo.lipSyncJawBone = dFrom.lipSyncJawBone;
                dTo.MouthOpenBlendShapeName = dFrom.MouthOpenBlendShapeName;
                dTo.Name = dFrom.Name;
                dTo.ScaleIPD = dFrom.ScaleIPD;
                dTo.unityVersion = dFrom.unityVersion;
                dTo.ViewPosition = dFrom.ViewPosition;
                dTo.VisemeBlendShapes = dFrom.VisemeBlendShapes;

                string s = GetGameObjectPath(dFrom.VisemeSkinnedMesh.gameObject, true);
                Transform t = dTo.transform.Find(s);
                if (t != null)
                {
                    dTo.VisemeSkinnedMesh = t.GetComponent<SkinnedMeshRenderer>();
                }

                
                //if (cState.copier.avatarDescriptor.copyAnimationOverrides)
                //{
                //    dTo.CustomSittingAnims = dFrom.CustomSittingAnims;
                //    dTo.CustomStandingAnims = dFrom.CustomStandingAnims;
                //}
            }
        }

        /// <summary>
        /// 复制DynamicBoneCollider组件。
        /// </summary>
        void CopyDynamicBoneColliders(GameObject from, GameObject to, ControlState cState, bool removeOld = false)
        {
#if !NO_BONES

            string[] logFormat = { "DynamicBoneCollider", from.name, to.name };
            string log = LocalizationClass.Log.CopyAttempt;

            List<DynamicBoneCollider> dFromList = new List<DynamicBoneCollider>();
            dFromList.AddRange(from.GetComponents<DynamicBoneCollider>());

            List<DynamicBoneCollider> dToList = new List<DynamicBoneCollider>();
            dToList.AddRange(to.GetComponents<DynamicBoneCollider>());

#if !OLD_BONES

            if (dFromList.Count == 0)
            {
                var ar = from.GetComponents<DynamicBoneColliderBase>();
                foreach (var obj in ar)
                {
                    dFromList.Add((DynamicBoneCollider)obj);
                }
            }

            if (dToList.Count == 0)
            {
                var ar = to.GetComponents<DynamicBoneColliderBase>();
                foreach (var obj in ar)
                {
                    dToList.Add((DynamicBoneCollider)obj);
                }
            }

#endif
            if (removeOld)
            {
                foreach (var c in dToList)
                {
                    UnityEngine.Object.DestroyImmediate(c);
                }
            }

            for (int i = 0; i < dFromList.Count; i++)
            {
                var dFrom = dFromList[i];
                DynamicBoneCollider dTo = null;

                if (dFrom == null)
                {
                    //"Failed: {2} has no {1}"
                    log += "\n[操作失败:在{2}中没有{1}]";
                    Log(log, LogType.Warning, logFormat);
                    return;
                }
                //"{2} has no {1}. Creating - "
                log += "\n[由于在{2}中没有{1}, 正在创建中...]";
                dTo = to.AddComponent<DynamicBoneCollider>();

                dTo.m_Bound = dFrom.m_Bound;
                dTo.m_Center = dFrom.m_Center;
                dTo.m_Direction = dFrom.m_Direction;
                dTo.m_Height = dFrom.m_Height;
                dTo.m_Radius = dFrom.m_Radius;

                dTo.enabled = dFrom.enabled;

                if (!removeOld)
                {
                    foreach (var c in dToList)
                    {
                        if (c.m_Bound == dTo.m_Bound && c.m_Center == dTo.m_Center && c.m_Direction == dTo.m_Direction && c.m_Height == dTo.m_Height && c.m_Radius == dTo.m_Radius)
                        {
                            UnityEngine.Object.DestroyImmediate(dTo);
                            break;
                        }
                    }
                    //"_Duplicate {1} with the same settings already exists. Removing duplicate."
                    log += "\n[在{1} 中已有相同的设置存在，正在删除其副本。]";
                    Log(log, LogType.Warning, logFormat);
                }
                else
                {
                    //"_Success: Added {0} to {2}."
                    log += "\n[完成操作-已将{0}添加到{2}]";
                    Log(log, LogType.Log, logFormat);


                }
            }
#endif
        }

        /// <summary>
        /// 复制动骨组件与其对应使用碰撞组
        /// </summary>
        public static void CopyDynamicBones(GameObject from, GameObject target, ControlState cState, bool createMissing = true)
        {

#if !NO_BONES
            string log = LocalizationClass.Log.CopyAttempt;
            string[] logFormat = { "DynamicBoneCollider", from.name, target.name };

            var dbFromList = new List<DynamicBone>();
            var dbToList = new List<DynamicBone>();

            dbFromList.AddRange(from.GetComponents<DynamicBone>());
            dbToList.AddRange(target.GetComponents<DynamicBone>());

            //处理源动骨组件列表
            for (int i = 0; i < dbFromList.Count; i++)
            {
                var dFrom = dbFromList[i];
                var garbageBones = new List<DynamicBone>();
#if !OLD_BONES
                var newColliderList = new List<DynamicBoneColliderBase>();
#else
                var newColliderList = new List<DynamicBoneCollider>();
#endif


                DynamicBone dbTarget = null;

                foreach (var db in dbToList)
                {
                    //判断目标OBJ动骨root项是否为空或名称相同
                    if (db.m_Root == null || (db.m_Root.name == dFrom.m_Root.name))
                    {
                        garbageBones.Add(db);//加入无效骨骼
                        dbTarget = db;
                        break;
                    }
                }

                //目标OBJ动骨为空则进行补全丢失项操作
                if (dbTarget == null)
                    if (createMissing)
                    {
                        dbTarget = target.AddComponent<DynamicBone>();

#if !OLD_BONES
                        dbTarget.m_Colliders = new List<DynamicBoneColliderBase>();
#else
                        dTarget.m_Colliders = new List<DynamicBoneCollider>();
#endif
                    }
                    else//不补全则返回
                        return;

                //无效动骨组不为空则先删除无效动骨
                if (garbageBones != null)
                {
                    foreach (var d in garbageBones)
                    {
                        dbToList.Remove(d);
                    }
                }

                //检查碰撞类型复制选项
                if (cState.copier.dynamicBones.copyColliders)
                {
                    //获取来源动骨使用的碰撞体
                    var colls = dFrom.m_Colliders;
                    for (int z = 0; z < colls.Count; z++)
                    {//遍历来源动骨使用的碰撞体集合
                     //获取复制来源碰撞体OBJ路径
                        if (colls[z]==null)
                        {
                            string[] llogFormat = { from.name, target.name };
                            Log("已跳过{0}中动骨空碰撞对象",LogType.Warning, llogFormat);
                            continue;
                        }
                        
                        string tFromPath = GetGameObjectPath(colls[z].gameObject);
                        //获取目标OBJ路径
                        var tTo = target.transform.root.Find(tFromPath);

                        DynamicBoneCollider fromCollider = null;
                        if (fromCollider == null)//意义不明
                        {
                            //获取来源碰撞体对象 存入临时碰撞体变量tempCol
                            var tempCol = colls[z].GetComponent<DynamicBoneCollider>();
                            if (tempCol != null)//成功则存入来源碰撞变量FC
                                fromCollider = tempCol;
                            else//失败则强制转换入FC
                                fromCollider = (DynamicBoneCollider)colls[z];
                        }

                        //目标OBJ路径不为空
                        if (tTo != null)
                        {
                            //在目标OBJ获取旧动骨碰撞体
                            var oldColls = tTo.GetComponents<DynamicBoneCollider>();

                            bool isSame = false;
                            foreach (var c in oldColls)
                            {
                                //判断旧碰撞是否等于FC来源碰撞体
                                if (c.m_Bound == fromCollider.m_Bound && c.m_Center == fromCollider.m_Center && c.m_Direction == fromCollider.m_Direction && c.m_Height == fromCollider.m_Height && c.m_Radius == fromCollider.m_Radius)
                                {
                                    isSame = true;//两者相同
#if OLD_BONES
                                    DynamicBoneCollider tempC = c;
#else
                                    DynamicBoneColliderBase tempC = c;//旧碰撞体存入临时碰撞TC
#endif
                                    foreach (var cc in dbTarget.m_Colliders)
                                    {//遍历目标碰撞体组
                                        if (c == cc)
                                        {
                                            tempC = null;
                                            break;
                                        }
                                    }
                                    if (tempC != null)
                                    {
                                        newColliderList.Add(tempC);
                                    }   
                                    break;
                                }
                            }
                            if (!isSame)
                            {//不相同则直接新增碰撞
                                var cTo = tTo.gameObject.AddComponent<DynamicBoneCollider>();
                                cTo.m_Bound = fromCollider.m_Bound;
                                cTo.m_Center = fromCollider.m_Center;
                                cTo.m_Direction = fromCollider.m_Direction;
                                cTo.m_Height = fromCollider.m_Height;
                                cTo.m_Radius = fromCollider.m_Radius;

                                cTo.enabled = fromCollider.enabled;

                                newColliderList.Add(cTo);//碰撞体存入待添加列表 等待遍历结束传入动骨组件
                                                         //"Success: Added {0} to {2}"
                                log += "\n[完成操作-已将{0}添加到{2}]";
                                Log(log, LogType.Log, logFormat);
                            }
                        }
                        else//为空则创建新空obj
                        {

                        }
                    }
                }

                logFormat = new string[] { "_DynamicBone", from.name, target.name };
                log = LocalizationClass.Log.CopyAttempt;

                if (dFrom == null)
                {
                    //"_Failed: {1} has no {0}. Ignoring"
                    log += "\n[操作失败:{1}中没有{0}，忽略本次操作]";
                    Log(log, LogType.Warning, logFormat);
                    return;
                }
                else if (!cState.copier.dynamicBones.copySettings)
                {
                    //"_Failed: Not allowed to - Copy settings is unchecked"
                    log += "\n[操作失败:禁止的操作 - 参数复制选项没有被选择(unchecked)]";
                    Log(log, LogType.Warning, logFormat);
                    return;
                }
                else if (dFrom.m_Root == null)
                {
                    //
                    log += "\n[操作失败:{2}的{0}没有根集，忽略本次操作]";
                    Log(log, LogType.Warning, logFormat);
                    return;
                }

                dbTarget.enabled = dFrom.enabled;

                dbTarget.m_Damping = dFrom.m_Damping;
                dbTarget.m_DampingDistrib = dFrom.m_DampingDistrib;
                dbTarget.m_DistanceToObject = dFrom.m_DistanceToObject;
                dbTarget.m_DistantDisable = dFrom.m_DistantDisable;
                dbTarget.m_Elasticity = dFrom.m_Elasticity;
                dbTarget.m_ElasticityDistrib = dFrom.m_ElasticityDistrib;
                dbTarget.m_EndLength = dFrom.m_EndLength;
                dbTarget.m_EndOffset = dFrom.m_EndOffset;
                dbTarget.m_Force = dFrom.m_Force;
                dbTarget.m_FreezeAxis = dFrom.m_FreezeAxis;
                dbTarget.m_Gravity = dFrom.m_Gravity;
                dbTarget.m_Inert = dFrom.m_Inert;
                dbTarget.m_InertDistrib = dFrom.m_InertDistrib;
                dbTarget.m_Radius = dFrom.m_Radius;
                dbTarget.m_RadiusDistrib = dFrom.m_RadiusDistrib;
                dbTarget.m_ReferenceObject = dFrom.m_ReferenceObject;
                dbTarget.m_Stiffness = dFrom.m_Stiffness;
                dbTarget.m_StiffnessDistrib = dFrom.m_StiffnessDistrib;
                dbTarget.m_UpdateMode = dFrom.m_UpdateMode;
                dbTarget.m_UpdateRate = dFrom.m_UpdateRate;

                if (dbTarget.m_Colliders != null)
                    dbTarget.m_Colliders.TrimExcess();

                if (newColliderList!=null)
                {
                    dbTarget.m_Colliders.AddRange(newColliderList);
                }

                List<Transform> el = new List<Transform>();
                for (int z = 0; z < dFrom.m_Exclusions.Count; z++)
                {
                    if (dFrom.m_Exclusions[z] != null)
                    {
                        string p = GetGameObjectPath(dFrom.m_Exclusions[z].gameObject, true);
                        var t = target.transform.root.Find(p);

                        if (t != null && dFrom.m_Exclusions[z].name == t.name)
                            el.Add(t);
                    }
                }
                dbTarget.m_Exclusions = el;

                if (dFrom.m_Root != null)
                {
                    string rootPath = GetGameObjectPath(dFrom.m_Root.gameObject, true);
                    if (!string.IsNullOrEmpty(rootPath))
                    {
                        var toRoot = dbTarget.transform.root.Find(rootPath);
                        if (!string.IsNullOrEmpty(rootPath))
                            dbTarget.m_Root = toRoot;
                    }
                }

                if (dFrom.m_ReferenceObject != null)
                {
                    string refPath = GetGameObjectPath(dFrom.m_ReferenceObject.gameObject, true);
                    if (!string.IsNullOrEmpty(refPath))
                    {
                        var toRef = dbTarget.transform.root.Find(refPath);
                        if (!string.IsNullOrEmpty(refPath))
                            dbTarget.m_ReferenceObject = toRef;
                    }
                }
                //"Success: Copied {0} from {1} to {2}"
                log += "\n[完成操作:将<{0}>从{1}复制到{2}]";
                Log(log, LogType.Log, logFormat);
            }
#endif
        }


        /// <summary>
        /// 复制布料组件与其对应使用碰撞组
        /// </summary>
        public static void CopyCloth(GameObject from, GameObject target, ControlState cState)
        {
            string log = SSS + LocalizationClass.Log.CopyAttempt;
            string[] logFormat = { "CapsuleCollider", from.name, target.name };

            var cFrom = from.GetComponent<Cloth>();
            if (cFrom == null)
            {
                Log(SSS + "目标["+from.transform.name + "]不包含Cloth组件 已跳过", LogType.Warning);
            }
            else
            {
                var cTo = target.GetComponent<Cloth>();

                {

                    var newCAPColliderList = new List<CapsuleCollider>();
                    var newSPHColliderList = new List<ClothSphereColliderPair>();


                    Cloth clothTarget = null;


                    if (cTo == null)
                    {
                        clothTarget = target.AddComponent<Cloth>();
                    }
                    else
                    {
                        clothTarget = cTo;
                    }

                    //获取CapsuleCollider组
                    var capColliders = cFrom.capsuleColliders;
                    if (capColliders != null)
                    {
                        Log(SSS + "目标[{0}]CAP组件数量:"+ capColliders.Length, LogType.Log, from.transform.name);

                        for (int z = 0; z < capColliders.Length; z++)
                        {//遍历来源布料使用的碰撞体集合
                         //获取复制来源碰撞体OBJ路径
                            if (capColliders[z] == null)
                            {
                                string[] llogFormat = { from.name, target.name };
                                Log("已跳过{0}中布料空碰撞对象", LogType.Warning, llogFormat);
                                continue;
                            }

                            //获取来源OBJ路径
                            string tFromPath = GetGameObjectPath(capColliders[z].gameObject);

                            //从来源相对路径获取目标OBJ
                            var tTo = target.transform.root.Find(tFromPath);

                            CapsuleCollider fromCollider = null;

                            //获取来源碰撞体对象 存入临时碰撞体变量tempCol
                            var tempCol = capColliders[z].GetComponent<CapsuleCollider>();
                            if (tempCol != null)//成功则存入来源碰撞变量FC
                                fromCollider = tempCol;
                            else//失败则强制转换入FC
                                fromCollider = (CapsuleCollider)capColliders[z];


                            //目标OBJ路径不为空
                            //为空则 寻找同名父级创建新空obj
                            if (tTo == null)
                            {
                                //同名父级为ROOT直接新建
                                if (capColliders[z].gameObject.transform.parent == capColliders[z].gameObject.transform.root)
                                {
                                    Log(SSS + "目标[{0}]序列[{2}]补充GameOBJ:{1}", LogType.Log, from.transform.name, capColliders[z].gameObject.name, z.ToString());
                                    tTo = AddNewOBJ(target.transform.root.gameObject, capColliders[z].gameObject.name, capColliders[z].gameObject.transform);
                                }
                                else
                                {
                                    //获取补充碰撞体OBJ的父级OBJ路径
                                    string tFromPathParent = GetGameObjectPath(capColliders[z].gameObject.transform.parent.gameObject);
                                    //Log(SSS+ "[REP]尝试获取目标:"+ tFromPathParent);

                                    //寻找目标Avatar下同级的父级目标
                                    var tToParent = target.transform.root.Find(tFromPathParent);
                                    if (tToParent != null)
                                    {
                                        Log(SSS + "目标[{0}]序列[{2}]补充GameOBJ:{1}", LogType.Log, from.transform.name, capColliders[z].gameObject.name, z.ToString());
                                        tTo = AddNewOBJ(tToParent.gameObject, capColliders[z].gameObject.name, capColliders[z].gameObject.transform);
                                    }
                                    else
                                    {
                                        Log(SSS + "目标[{0}]序列[{2}]添加CAP失败!父级路径不存在!CAP路径:\n{1}", LogType.Warning, from.transform.name, tFromPath, z.ToString());
                                        continue;
                                    }

                                }

                            }
                                //在目标OBJ获取旧布料碰撞体
                                var oldColls = tTo.GetComponents<CapsuleCollider>();

                                bool isSame = false;
                                foreach (var c in oldColls)
                                {
                                    //判断旧碰撞是否等于FC来源碰撞体
                                    if (CollidersAreIdentical(c, fromCollider))
                                    {
                                        isSame = true;//两者相同
                                        CapsuleCollider tempC = c;//旧碰撞体存入临时碰撞TC
                                        foreach (var cc in clothTarget.capsuleColliders)
                                        {//遍历目标碰撞体组
                                            if (c == cc)
                                            {
                                                tempC = null;
                                                break;
                                            }
                                        }
                                        if (tempC != null)
                                        {
                                            Log(SSS + "目标[{0}]序列[{2}]添加CAP > {1}", LogType.Log,from.transform.name , tTo.name, z.ToString());
                                            newCAPColliderList.Add(tempC);
                                        }
                                        break;
                                    }
                                }
                                if (!isSame)
                                {//不相同则直接新增碰撞
                                    var tempTo = tTo.gameObject.AddComponent<CapsuleCollider>();
                                    tempTo.isTrigger = fromCollider.isTrigger;
                                    PhysicMaterial tempMat = new PhysicMaterial();
                                    if (fromCollider.material == tempMat)
                                    {
                                        tempTo.material = null;
                                    }
                                    else
                                    {
                                        tempTo.material = fromCollider.material;
                                    }
                                    tempTo.material.name = tempTo.material.name.Replace("(Instance)", "");
                                    tempTo.center = fromCollider.center;
                                    tempTo.radius = fromCollider.radius;
                                    tempTo.height = fromCollider.height;
                                    tempTo.direction = fromCollider.direction;

                                    tempTo.enabled = fromCollider.enabled;

                                    Log(SSS + "目标[{0}]序列[{2}]新增CAP > {1}", LogType.Log, from.transform.name, tTo.name, z.ToString());
                                    newCAPColliderList.Add(tempTo);//碰撞体存入待添加列表 等待遍历结束传入动骨组件
                                                                   //"Success: Added {0} to {2}"
                                    log += "\n[完成操作-已将{0}添加到{2}]";
                                    Log(log, LogType.Log, logFormat);
                                }
                            
                        }
                    }


                    //获取SphereCollider组
                    var sphColliders = cFrom.sphereColliders;
                    if (sphColliders != null)
                    {
                        Log(SSS + "目标[" + from.transform.name + "]SPH组件数量:" + sphColliders.Length, LogType.Log);
                        for (int z = 0; z < sphColliders.Length; z++)
                        {
                            //获取复制来源碰撞体OBJ路径

                            //sphColliders[z].first

                            string tFromPathFirst = GetGameObjectPath(sphColliders[z].first.gameObject);
                            string tFromPathSecond = GetGameObjectPath(sphColliders[z].second.gameObject);

                            SphereCollider fromColliderF = null;
                            SphereCollider fromColliderS = null;

                            //获取来源碰撞体对象 存入临时碰撞体变量tempCol
                            var tempColF = sphColliders[z].first.GetComponent<SphereCollider>();
                            var tempColS = sphColliders[z].second.GetComponent<SphereCollider>();

                            if (tempColF != null)//成功则存入来源碰撞变量FC
                                fromColliderF = tempColF;
                            else//失败则强制转换入FC
                                fromColliderF = (SphereCollider)sphColliders[z].first;
                            if (tempColS != null)//成功则存入来源碰撞变量FC
                                fromColliderS = tempColS;
                            else//失败则强制转换入FC
                                fromColliderS = (SphereCollider)sphColliders[z].second;


                            var tToF = target.transform.root.Find(tFromPathFirst);
                            var tToS = target.transform.root.Find(tFromPathSecond);

                            //获取目标OBJ路径

                            ClothSphereColliderPair tCSC = new ClothSphereColliderPair();
                            
                            //目标OBJ路径不为空
                            if (tToF == null)
                            {

                                string tFromPathParent = GetGameObjectPath(sphColliders[z].first.gameObject.transform.parent.gameObject);
                                var tToParent = target.transform.root.Find(tFromPathParent);
                                if (tToParent != null)
                                {
                                    Log(SSS + "目标[{0}]CSC序列F[{2}]补充GameOBJ:{1}", LogType.Log, from.transform.name, sphColliders[z].first.gameObject.name, z.ToString());
                                    tToF = AddNewOBJ(tToParent.gameObject, sphColliders[z].first.gameObject.name, sphColliders[z].first.gameObject.transform);
                                }
                                else
                                {
                                    Log(SSS + "目标[{0}]CSC序列F[{2}]添加CAP失败!父级路径不存在!CAP路径:\n{1}", LogType.Warning, from.transform.name, tFromPathFirst, z.ToString());
                                    continue;
                                }
                            }

                            //在目标OBJ获取旧动骨碰撞体
                            var oldCollsF = tToF.GetComponents<SphereCollider>();

                                bool isSameF = false;
                                foreach (var c in oldCollsF)
                                {
                                    //判断旧碰撞是否等于FC来源碰撞体
                                    if (CollidersAreIdentical(c, fromColliderF))
                                    {
                                        isSameF = true;//两者相同
                                        SphereCollider tempC = c;//旧碰撞体存入临时碰撞TC
                                        foreach (var cc in clothTarget.sphereColliders)
                                        {//遍历目标碰撞体组
                                            if (c == cc.first)
                                            {
                                                tempC = null;
                                                break;
                                            }
                                        }
                                        if (tempC != null)
                                        {
                                            Log(SSS + "目标[{0}]CSC序列[{2}]添加CSC > {1}", LogType.Log, from.transform.name, tToF.name, z.ToString());
                                            tCSC.first = tempC;
                                        }
                                        break;
                                    }
                                }
                                if (!isSameF)
                                {
                                    var tempTo = tToF.gameObject.AddComponent<SphereCollider>();
                                    tempTo.isTrigger = fromColliderF.isTrigger;
                                    PhysicMaterial tempMat = new PhysicMaterial();
                                    if (fromColliderS.material == tempMat)
                                    {
                                        tempTo.material = null;
                                    }
                                    else
                                    {
                                        tempTo.material = fromColliderS.material;
                                    }
                                    tempTo.material.name = tempTo.material.name.Replace("(Instance)", "");
                                    tempTo.center = fromColliderF.center;
                                    tempTo.radius = fromColliderF.radius;

                                    tempTo.enabled = fromColliderF.enabled;
                                    Log(SSS + "目标[{0}]CSC序列[{2}]添加CSC > {1}", LogType.Log, from.transform.name, tToF.name, z.ToString());
                                    tCSC.first = tempTo;
                                }


                            //目标OBJ路径不为空
                            if (tToS == null)
                            {

                                string tFromPathParent = GetGameObjectPath(sphColliders[z].second.gameObject.transform.parent.gameObject);
                                var tToParent = target.transform.root.Find(tFromPathParent);
                                if (tToParent != null)
                                {
                                    Log(SSS + "目标[{0}]CSC序列S[{2}]补充GameOBJ:{1}", LogType.Log, from.transform.name, sphColliders[z].first.gameObject.name, z.ToString());
                                    tToS = AddNewOBJ(tToParent.gameObject, sphColliders[z].second.gameObject.name, sphColliders[z].second.gameObject.transform);
                                }
                                else
                                {
                                    Log(SSS + "目标[{0}]CSC序列S[{2}]添加CAP失败!父级路径不存在!CAP路径:\n{1}", LogType.Warning, from.transform.name, tFromPathSecond, z.ToString());
                                    continue;
                                }
                            }

                            //在目标OBJ获取旧动骨碰撞体
                            var oldCollsS = tToS.GetComponents<SphereCollider>();

                                bool isSameS = false;
                                foreach (var c in oldCollsS)
                                {
                                    //判断旧碰撞是否等于FC来源碰撞体
                                    if (CollidersAreIdentical(c, fromColliderS))
                                    {
                                        isSameS = true;//两者相同
                                        SphereCollider tempC = c;//旧碰撞体存入临时碰撞TC
                                        foreach (var cc in clothTarget.sphereColliders)
                                        {//遍历目标碰撞体组
                                            if (c == cc.second)
                                            {
                                                tempC = null;
                                                break;
                                            }
                                        }
                                        if (tempC != null)
                                        {
                                            Log(SSS + "目标[{0}]CSC序列[{2}]添加CSC > {1}", LogType.Log, from.transform.name, tToS.name, z.ToString());
                                            tCSC.second = tempC;
                                        }
                                        break;
                                    }
                                }
                                if (!isSameS)
                                {
                                    var tempTo = tToS.gameObject.AddComponent<SphereCollider>();
                                    tempTo.isTrigger = fromColliderS.isTrigger;
                                    PhysicMaterial tempMat = new PhysicMaterial();
                                    if (fromColliderS.material == tempMat)
                                    {
                                        tempTo.material = null;
                                    }
                                    else
                                    {
                                        tempTo.material = fromColliderS.material;
                                    }
                                    tempTo.material.name = tempTo.material.name.Replace("(Instance)", "");
                                    tempTo.center = fromColliderS.center;
                                    tempTo.radius = fromColliderS.radius;

                                    tempTo.enabled = fromColliderS.enabled;
                                    Log(SSS + "目标[{0}]CSC序列[{2}]添加CSC > {1}", LogType.Log, from.transform.name, tToS.name, z.ToString());
                                    tCSC.second = tempTo;
                                }


                            

                            newSPHColliderList.Add(tCSC);//碰撞体存入待添加列表 等待遍历结束传入组件
                            log += "\n[完成操作-已将[ClothSphereColliderPair]添加到{2}]";
                            Log(log, LogType.Log, logFormat);
                        }
                    }



                    logFormat = new string[] { "_CapsuleCollider", from.name, target.name };
                    log = LocalizationClass.Log.CopyAttempt;

                    if (cFrom == null)
                    {
                        //"_Failed: {1} has no {0}. Ignoring"
                        log += "\n[操作失败:{1}中没有{0}，忽略本次操作]";
                        Log(log, LogType.Warning, logFormat);
                        return;
                    }

                    clothTarget.enabled = cFrom.enabled;
                    //try
                    //{
                    //    clothTarget.coefficients = cFrom.coefficients;
                    //}
                    //catch (Exception ex)
                    //{
                    //    log += "来源{1}与目标{2}的Mesh顶点数量不匹配 错误信息:\n"+ ex.Message;
                    //    Log(log, LogType.Exception, logFormat);
                    //}
                    if (clothTarget.coefficients.Length == cFrom.coefficients.Length )
                    {
                        clothTarget.coefficients = cFrom.coefficients;
                    }
                    else
                    {
                        Debug.LogWarningFormat(SSS + "来源[{0}]\n与目标[{1}]的[{2}]Mesh顶点数量不匹配", from.transform.root.name, target.transform.root.name, from.name);
                    }
                    clothTarget.selfCollisionDistance = cFrom.selfCollisionDistance;
                    clothTarget.selfCollisionStiffness = cFrom.selfCollisionStiffness;
                    clothTarget.stretchingStiffness = cFrom.stretchingStiffness;
                    clothTarget.bendingStiffness = cFrom.bendingStiffness;
                    clothTarget.useTethers = cFrom.useTethers;
                    clothTarget.useGravity = cFrom.useGravity;
                    clothTarget.damping = cFrom.damping;
                    clothTarget.externalAcceleration = cFrom.externalAcceleration;
                    clothTarget.randomAcceleration = cFrom.randomAcceleration;
                    clothTarget.worldVelocityScale = cFrom.worldVelocityScale;
                    clothTarget.worldAccelerationScale = cFrom.worldAccelerationScale;
                    clothTarget.friction = cFrom.friction;
                    clothTarget.collisionMassScale = cFrom.collisionMassScale;
                    clothTarget.enableContinuousCollision = cFrom.enableContinuousCollision;
                    clothTarget.clothSolverFrequency = cFrom.clothSolverFrequency;
                    clothTarget.sleepThreshold = cFrom.sleepThreshold;

                    newCAPColliderList.AddRange(new List<CapsuleCollider>(clothTarget.capsuleColliders));
                    clothTarget.capsuleColliders = newCAPColliderList.ToArray();
                    newSPHColliderList.AddRange(new List<ClothSphereColliderPair>(clothTarget.sphereColliders));
                    var removeList = new List<ClothSphereColliderPair>();
                    foreach (var sphPair in newSPHColliderList)
                    {
                        if (sphPair.first == null|| sphPair.second == null)
                        {
                            removeList.Add(sphPair);
                        }
                    }
                    foreach (var sphPair in removeList)
                    {
                        newSPHColliderList.Remove(sphPair);
                    }
                    clothTarget.sphereColliders = newSPHColliderList.ToArray();

                    //"Success: Copied {0} from {1} to {2}"
                    log += "\n[完成操作:将<{0}>从{1}复制到{2}]";
                    Log(log, LogType.Log, logFormat);
                }
            }
        }


        /// <summary>
        /// 将 Box Capsule Sphere Mesh colliders 复制到另一个目标
        /// </summary>        
        public static void CopyColliders(GameObject from, GameObject to, ControlState cState)
        {

            if (!(cState.copier.colliders.copyBox || cState.copier.colliders.copyCapsule || cState.copier.colliders.copyMesh || cState.copier.colliders.copySphere))
                return;

            string log = LocalizationClass.Log.CopyAttempt;

            var cFromList = new List<Collider>();
            var cToList = new List<Collider>();

            cFromList.AddRange(from.GetComponents<BoxCollider>());
            cFromList.AddRange(from.GetComponents<CapsuleCollider>());
            cFromList.AddRange(from.GetComponents<SphereCollider>());
            cFromList.AddRange(from.GetComponents<MeshCollider>());

            cToList.AddRange(to.GetComponents<BoxCollider>());
            cToList.AddRange(to.GetComponents<CapsuleCollider>());
            cToList.AddRange(to.GetComponents<SphereCollider>());
            cToList.AddRange(to.GetComponents<MeshCollider>());

            foreach (var cFrom in cFromList)
            {
                bool found = false;
                string[] logFormat = { cFrom.GetType().ToString(), from.name, to.name };

                foreach (var c in cToList)
                {
                    found = CollidersAreIdentical(c, cFrom);
                }
                if (!found)
                {
                    PhysicMaterial tempMat = new PhysicMaterial();
                    if (cState.copier.colliders.copyBox && cFrom is BoxCollider)
                    {
                        BoxCollider cc = (BoxCollider)cFrom;
                        BoxCollider cTo = to.AddComponent<BoxCollider>();
                        cTo.size = cc.size;
                        cTo.center = cc.center;
                        cTo.contactOffset = cc.contactOffset;

                        cTo.isTrigger = cc.isTrigger;

                        if (cc.material == tempMat)
                            cTo.material = null;
                        else
                            cTo.material = cc.material;

                        if (cc.sharedMaterial == tempMat)
                            cTo.sharedMaterial = null;
                        else
                            cTo.sharedMaterial = cc.sharedMaterial;

                        cTo.enabled = cc.enabled;
                    }
                    else if (cState.copier.colliders.copyCapsule && cFrom is CapsuleCollider)
                    {
                        CapsuleCollider cc = (CapsuleCollider)cFrom;
                        CapsuleCollider cTo = to.AddComponent<CapsuleCollider>();
                        cTo.direction = cc.direction;
                        cTo.center = cc.center;
                        cTo.radius = cc.radius;
                        cTo.height = cc.height;
                        cTo.contactOffset = cc.contactOffset;

                        cTo.isTrigger = cc.isTrigger;

                        if (cc.material == tempMat)
                            cTo.material = null;
                        else
                            cTo.material = cc.material;

                        if (cc.sharedMaterial == tempMat)
                            cTo.sharedMaterial = null;
                        else
                            cTo.sharedMaterial = cc.sharedMaterial;

                        cTo.enabled = cc.enabled;
                    }
                    else if (cState.copier.colliders.copySphere && cFrom is SphereCollider)
                    {
                        SphereCollider cc = (SphereCollider)cFrom;
                        SphereCollider cTo = to.AddComponent<SphereCollider>();
                        cTo.center = cc.center;
                        cTo.radius = cc.radius;

                        cTo.contactOffset = cc.contactOffset;
                        cTo.isTrigger = cc.isTrigger;

                        if (cc.material == tempMat)
                            cTo.material = null;
                        else
                            cTo.material = cc.material;

                        if (cc.sharedMaterial == tempMat)
                            cTo.sharedMaterial = null;
                        else
                            cTo.sharedMaterial = cc.sharedMaterial;

                        cTo.enabled = cc.enabled;
                    }
                    else if (cState.copier.colliders.copyMesh && cFrom is MeshCollider)
                    {
                        MeshCollider cc = (MeshCollider)cFrom;
                        MeshCollider cTo = to.AddComponent<MeshCollider>();

                        cTo.convex = cc.convex;
                        cTo.inflateMesh = cc.inflateMesh;
                        cTo.sharedMesh = cc.sharedMesh;
                        cTo.skinWidth = cc.skinWidth;

                        cTo.contactOffset = cc.contactOffset;
                        cTo.isTrigger = cc.isTrigger;

                        if (cc.material == tempMat)
                            cTo.material = null;
                        else
                            cTo.material = cc.material;

                        if (cc.sharedMaterial == tempMat)
                            cTo.sharedMaterial = null;
                        else
                            cTo.sharedMaterial = cc.sharedMaterial;

                        cTo.enabled = cc.enabled;
                    }
                    else
                    {
                        //"_Failed: Unsupported Collider type {0} on {1}. Ignoring"
                        log += "\n[操作失败:{1}上不支持的碰撞器类型{0}。忽略本次操作]";
                        Log(log, LogType.Error, logFormat);
                        return;
                    }
                }
                else
                {
                    //"_Failed: {0} already exists on {2}. Ignoring"
                    log += "\n[操作失败:{0}已存在于{2}中。忽略本次操作]";
                    Log(log, LogType.Warning, logFormat);
                    return;
                }
                //"_Success - Added {0} to {2}"
                log += "\n[完成操作-已将{0}添加到{2}]";
                Log(log, LogType.Log, logFormat);
            }
        }

        /// <summary>
        /// 复制 Transform 组件 (一个GO只能存在一个 并且也只能处理一个)
        /// </summary>    
        void CopyTransforms(GameObject from, GameObject to, ControlState cState)
        {
            var tFrom = from.transform;
            var tTo = to.transform;

            string log = LocalizationClass.Log.CopyAttempt;
            string[] logFormat = { "Transforms", from.name, to.name };

            if (tTo == null || tFrom == null)
            {
                //"_Failed: {1} or {2} is null. This shouldn't even be possible. What are you doing?"
                log += "\n[操作失败:{1}或{2}为空对象，正常不会出现这种错误，如果可以请反馈此情况。]";
                Log(log, LogType.Error);
                return;
            }

            if (tFrom == tFrom.root || tFrom == tFrom.root.Find(tFrom.name))
            {
                //"_Ignored: {2} is root or child of root."
                log += "\n[忽略本次操作:{2}是根(Root)或根的子级]";
                Log(log, LogType.Warning, logFormat);
                return;
            }

            if (cState.copier.transforms.copyPosition)
                tTo.localPosition = tFrom.localPosition;
            if (cState.copier.transforms.copyScale)
                tTo.localScale = tFrom.localScale;
            if (cState.copier.transforms.copyRotation)
            {
                tTo.localEulerAngles = tFrom.localEulerAngles;
                tTo.localRotation = tFrom.localRotation;
            }
            //"Success: Copied {0} from {1} to {2}"
            log += "\n[完成操作:将<{0}>从{1}复制到{2}]";
            Log(log, LogType.Log, logFormat);
        }

        /// <summary>
        /// 复制 SkinnedMeshRenderer 组件 (一个GO只支持处理一个)
        /// </summary>                
        void CopySkinMeshRenderer(GameObject from, GameObject to, ControlState cState)
        {
            //检查复制类型设定选项
            if (!(cState.copier.skinMeshRender.copyBlendShapeValues || cState.copier.skinMeshRender.copyMaterials || cState.copier.skinMeshRender.copySettings))
                return;

            //调试输出准备:正在进行SMR复制操作
            string log = LocalizationClass.Log.CopyAttempt + " - ";
            string[] logFormat = { "SkinnedMeshRenderer", from.name, to.name };

            SkinnedMeshRenderer rFrom = from.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer rTo = to.GetComponent<SkinnedMeshRenderer>();

            //来源不包含SMR 输出警告信息 备注：由于大部分OBJ不具有body的SMR,LOG刷屏过于烦人 于是注释掉LOG输出 直接跳过
            if (rFrom == null)
            {
                //log += LocalizationClass.Log.FailedIsNull;
                //Log(log, LogType.Warning, logFormat);
                return;
            }
            if (rTo == null)
            {
                return;
            }
            //复制SMR具体参数
            if (cState.copier.skinMeshRender.copySettings)
            {
                rTo.enabled = rFrom.enabled;
                rTo.quality = rFrom.quality;
                rTo.updateWhenOffscreen = rFrom.updateWhenOffscreen;
                rTo.skinnedMotionVectors = rFrom.skinnedMotionVectors;
                rTo.lightProbeUsage = rFrom.lightProbeUsage;
                rTo.reflectionProbeUsage = rFrom.reflectionProbeUsage;
                rTo.shadowCastingMode = rFrom.shadowCastingMode;
                rTo.receiveShadows = rFrom.receiveShadows;
                rTo.motionVectorGenerationMode = rFrom.motionVectorGenerationMode;

                string path = null;
                if (rFrom.probeAnchor != null)
                    path = GetGameObjectPath(rFrom.probeAnchor.gameObject);

                if (!string.IsNullOrEmpty(path))
                    rTo.probeAnchor = rTo.transform.root.Find(path);

                path = null;
                if (rFrom.rootBone != null)
                    path = GetGameObjectPath(rFrom.rootBone.gameObject);

                if (!string.IsNullOrEmpty(path))
                    rTo.rootBone = rTo.transform.root.Find(path);
            }

            //复制SMR表情形态键参数
            if (cState.copier.skinMeshRender.copyBlendShapeValues)
            {
                for (int i = 0; i < rFrom.sharedMesh.blendShapeCount; i++)
                {
                    int index = rFrom.sharedMesh.GetBlendShapeIndex(rFrom.sharedMesh.GetBlendShapeName(i));
                    if (index != -1)
                    {
                        rTo.SetBlendShapeWeight(index, rFrom.GetBlendShapeWeight(index));
                    }
                }
            }

            //复制SMR材质球参数
            if (cState.copier.skinMeshRender.copyMaterials)
            {
                rTo.sharedMaterials = rFrom.sharedMaterials;
            }

            rTo.sharedMesh.RecalculateBounds();
            //"Success: Copied {0} from {1} to {2}"
            log += "\n[完成操作:将<{0}>从{1}复制到{2}]";
            Log(log, LogType.Log, logFormat);
        }
        #endregion

        #region 杂项子函数
        static bool CollidersAreIdentical(Collider col1, Collider col2)
        {
            if (col1 == null && col2 == null)
                return true;
            else if (col1.GetType() != col2.GetType())
                return false;

            if (!PhysMaterialsAreIdentical(col1.material, col2.material) ||
                !PhysMaterialsAreIdentical(col1.sharedMaterial, col2.sharedMaterial) || col1.isTrigger != col2.isTrigger)
                return false;

            if (col1 is BoxCollider)
            {
                var c = (BoxCollider)col1;
                var cc = (BoxCollider)col2;

                if (c.size != cc.size || c.center != c.center)
                    return false;
            }
            else if (col1 is CapsuleCollider)
            {
                var c = (CapsuleCollider)col1;
                var cc = (CapsuleCollider)col2;

                if (c.center != cc.center || c.radius != c.radius || c.height != cc.height || c.direction != cc.direction)
                    return false;

            }
            else if (col1 is SphereCollider)
            {
                var c = (SphereCollider)col1;
                var cc = (SphereCollider)col2;

                if (c.center != cc.center || c.radius != cc.radius)
                    return false;
            }
            return true;
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

        static bool PhysMaterialsAreIdentical(PhysicMaterial mat1, PhysicMaterial mat2)
        {
            if (mat1 == null && mat2 == null)
                return true;

            if (mat1.bounceCombine == mat2.bounceCombine && mat1.bounciness == mat2.bounciness && mat1.dynamicFriction == mat2.dynamicFriction &&
                mat1.frictionCombine == mat2.frictionCombine && mat1.staticFriction == mat2.staticFriction)
                return true;
            else
                return false;
        }

        public static Transform AddNewOBJ(GameObject objTo, string OBJName)
        {
            GameObject gO = new GameObject(OBJName);
            gO.transform.parent = objTo.transform;
            Log(SSS + "为目标[{0}]创建空GameObject{1}完成", LogType.Log, objTo.transform.name, OBJName);
            return gO.transform;
        }
        public static GameObject AddNewOBJ(Transform TranTo, string OBJName)
        {
            GameObject gO = new GameObject(OBJName);
            gO.transform.parent = TranTo;
            Log(SSS + "为目标[{0}]创建空GameObject{1}完成", LogType.Log, TranTo.name, OBJName);
            return gO;
        }
        static Transform AddNewOBJ(GameObject objTo, string OBJName , Transform tT)
        {
            GameObject gO = new GameObject(OBJName);
            gO.transform.parent = objTo.transform;
            gO.transform.localPosition = tT.localPosition;
            gO.transform.localRotation = tT.localRotation;
            gO.transform.localScale = tT.localScale;
            Log(SSS + "为目标[{0}]创建空GameObject{1}并同步参数", LogType.Log, objTo.transform.name, OBJName);
            return gO.transform;
        }



        /// <summary>
        /// 辅助生成Log信息
        /// </summary>
        /// <param name="message">传达信息</param>
        /// <param name="logType">信息类型</param>
        /// <param name="logFormat">格式化信息</param>
        public static void Log(string message, LogType logType = LogType.Log, params string[] logFormat)
        {
            if (logFormat.Length > 0)
                message = string.Format(message, logFormat);
            switch (logType)
            {
                case LogType.Error:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Exception:
                    Debug.LogException(new Exception(message));
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }

        #endregion



        #region 销毁函数  

        /// <summary>
        /// 从对象及其所有子对象中删除所有Collider组件。
        /// </summary>    
        public static void DestroyAllColliders(GameObject from)
        {
            var col = from.GetComponentsInChildren<Collider>(true);
            foreach (var c in col)
            {
                //"Removing collider {0} from {1}"
                Log(string.Format("\n[正在从{1}中删除碰撞(Collider):{0}]", c, from.name));
                UnityEngine.Object.DestroyImmediate(c);
            }
        }

        /// <summary>
        /// 从对象及其所有子对象中删除所有DynamicBone组件。
        /// </summary>    
        public static void DestroyAllDynamicBones(GameObject from)
        {
#if !NO_BONES
            var bones = from.GetComponentsInChildren<DynamicBone>(true);
            foreach (var b in bones)
            {
                UnityEngine.Object.DestroyImmediate(b);
            }
#endif
        }

        /// <summary>
        /// 从对象及其所有子对象中删除所有DynamicBoneCollider组件
        /// 并清除所有DynamicBone的Collider关联
        /// </summary>    
        public static void DestroyAllDynamicBoneColliders(GameObject from)
        {

#if !NO_BONES
            List<DynamicBoneColliderBase> cl = new List<DynamicBoneColliderBase>();
            cl.AddRange(from.GetComponentsInChildren<DynamicBoneColliderBase>(true));

            foreach (var c in cl)
            {
                UnityEngine.Object.DestroyImmediate(c);
            }

            List<DynamicBone> dl = new List<DynamicBone>();
            dl.AddRange(from.GetComponentsInChildren<DynamicBone>(true));

            foreach (var d in dl)
            {
                if (d.m_Colliders != null)
                    d.m_Colliders.Clear();
            }
#endif
        }

        /// <summary>
        /// 如果目标GO的组件在supportedComponents中，则删除该类型的所有组件
        /// </summary>        
        public static void DestroyAllComponentsOfType(GameObject obj, Type type)
        {
            string log = "";
            string[] logFormat = { type.ToString(), obj.name };
            if (!IsSupportedComponentType(type))
            {
                log += LocalizationClass.Log.TryRemoveUnsupportedComponent;
                Log(log, LogType.Assert, logFormat);
                return;
            }

            Component[] comps = obj.transform.GetComponentsInChildren(type, true);

            if (comps != null && comps.Length > 0)
            {
                for (int i = 0; i < comps.Length; i++)
                {
                    log = LocalizationClass.Log.RemoveAttempt + " - ";
                    string name = comps[i].name;

                    try
                    {
                        UnityEngine.Object.DestroyImmediate(comps[i]);
                        log += LocalizationClass.Log.Success;
                        Log(log, LogType.Log, type.ToString(), name);
                    }
                    catch (Exception e)
                    {
                        log += LocalizationClass.Log.Failed + ": " + e.Message;
                        Log(log, LogType.Exception, type.ToString(), name);
                    }
                }
            }
        }

        #endregion



        #region Helper Functions

        static bool IsSupportedComponentType(Type type)
        {
            foreach (Type t in supportedComponents)
            {
                if (t == type)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }

}
