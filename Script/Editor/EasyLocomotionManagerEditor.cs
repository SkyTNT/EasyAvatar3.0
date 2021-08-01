using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyAvatar
{
    [CustomEditor(typeof(EasyLocomotionManager))]
    public class EasyLocomotionManagerEditor : Editor
    {
        bool standFold = true, crouchFold = true, proneFold = true, jumpFold = true, otherFold = true;
        static EasyLocomotion copiedLocomotion;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SerializedProperty useController = serializedObject.FindProperty("useAnimatorController");
            EditorGUILayout.PropertyField(useController, new GUIContent(Lang.UseController));

            if (!useController.boolValue)
            {
                standFold = EditorGUILayout.Foldout(standFold, Lang.LocomotionStand);
                if (standFold)
                {
                    SerializedProperty useBlendTree = serializedObject.FindProperty("useStandBlendTree");
                    EditorGUILayout.PropertyField(useBlendTree, new GUIContent(Lang.UseBlendTree));
                    if (!useBlendTree.boolValue)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(Lang.Default))
                        {
                            SetDefaultStandLocomotion(serializedObject);
                        }
                        if (GUILayout.Button(Lang.Clear))
                        {
                            ClearStandLocomotion(serializedObject);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginVertical(GUI.skin.box);
                        LocomotionFieldHeader();
                        LocomotionField(serializedObject.FindProperty("standStill"), Lang.StandStill);
                        LocomotionField(serializedObject.FindProperty("walkForward"), Lang.WalkForward);
                        LocomotionField(serializedObject.FindProperty("walkBackward"), Lang.WalkBackward);
                        LocomotionField(serializedObject.FindProperty("walkLeft"), Lang.WalkLeft);
                        LocomotionField(serializedObject.FindProperty("walkRight"), Lang.WalkRight);
                        LocomotionField(serializedObject.FindProperty("walkForwardLeft"), Lang.WalkForwardLeft);
                        LocomotionField(serializedObject.FindProperty("walkForwardRight"), Lang.WalkForwardRight);
                        LocomotionField(serializedObject.FindProperty("walkBackwardLeft"), Lang.WalkBackwardLeft);
                        LocomotionField(serializedObject.FindProperty("walkBackwardRight"), Lang.WalkBackwardRight);
                        LocomotionField(serializedObject.FindProperty("runForward"), Lang.RunForward);
                        LocomotionField(serializedObject.FindProperty("runBackward"), Lang.RunBackward);
                        LocomotionField(serializedObject.FindProperty("runLeft"), Lang.RunLeft);
                        LocomotionField(serializedObject.FindProperty("runRight"), Lang.RunRight);
                        LocomotionField(serializedObject.FindProperty("runForwardLeft"), Lang.RunForwardLeft);
                        LocomotionField(serializedObject.FindProperty("runForwardRight"), Lang.RunForwardRight);
                        LocomotionField(serializedObject.FindProperty("runBackwardLeft"), Lang.RunBackwardLeft);
                        LocomotionField(serializedObject.FindProperty("runBackwardRight"), Lang.RunBackwardRight);
                        LocomotionField(serializedObject.FindProperty("sprintForward"), Lang.SprintForward);
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        if (GUILayout.Button(Lang.Default))
                        {
                            serializedObject.FindProperty("standBlendTree").objectReferenceValue = VRCAssets.standBlendTree;
                        }
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("standBlendTree"), new GUIContent(Lang.BlendTree));
                    }
                }

                crouchFold = EditorGUILayout.Foldout(crouchFold, Lang.LocomotionCrouch);
                if (crouchFold)
                {
                    SerializedProperty useBlendTree = serializedObject.FindProperty("useCrouchBlendTree");
                    EditorGUILayout.PropertyField(useBlendTree, new GUIContent(Lang.UseBlendTree));
                    if (!useBlendTree.boolValue)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(Lang.Default))
                        {
                            SetDefaultCrouchLocomotion(serializedObject);
                        }
                        if (GUILayout.Button(Lang.Clear))
                        {
                            ClearCrouchLocomotion(serializedObject);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginVertical(GUI.skin.box);
                        LocomotionFieldHeader();
                        LocomotionField(serializedObject.FindProperty("crouchStill"), Lang.CrouchStill);
                        LocomotionField(serializedObject.FindProperty("crouchForward"), Lang.CrouchForward);
                        LocomotionField(serializedObject.FindProperty("crouchBackward"), Lang.CrouchBackward);
                        LocomotionField(serializedObject.FindProperty("crouchLeft"), Lang.CrouchLeft);
                        LocomotionField(serializedObject.FindProperty("crouchRight"), Lang.CrouchRight);
                        LocomotionField(serializedObject.FindProperty("crouchForwardLeft"), Lang.CrouchForwardLeft);
                        LocomotionField(serializedObject.FindProperty("crouchForwardRight"), Lang.CrouchForwardRight);
                        LocomotionField(serializedObject.FindProperty("crouchBackwardLeft"), Lang.CrouchBackwardLeft);
                        LocomotionField(serializedObject.FindProperty("crouchBackwardRight"), Lang.CrouchBackwardRight);
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        if (GUILayout.Button(Lang.Default))
                        {
                            serializedObject.FindProperty("crouchBlendTree").objectReferenceValue = VRCAssets.crouchBlendTree;
                        }
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("crouchBlendTree"), new GUIContent(Lang.BlendTree));
                    }
                    
                }

                proneFold = EditorGUILayout.Foldout(proneFold, Lang.LocomotionProne);
                if (proneFold)
                {
                    SerializedProperty useBlendTree = serializedObject.FindProperty("useProneBlendTree");
                    EditorGUILayout.PropertyField(useBlendTree, new GUIContent(Lang.UseBlendTree));
                    if (!useBlendTree.boolValue)
                    {
                        
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(Lang.Default))
                        {
                            SetDefaultProneLocomotion(serializedObject);
                        }
                        if (GUILayout.Button(Lang.Clear))
                        {
                            ClearProneLocomotion(serializedObject);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginVertical(GUI.skin.box);
                        LocomotionFieldHeader();
                        LocomotionField(serializedObject.FindProperty("proneStill"), Lang.ProneStill);
                        LocomotionField(serializedObject.FindProperty("proneForward"), Lang.ProneForward);
                        LocomotionField(serializedObject.FindProperty("proneBackward"), Lang.ProneBackward);
                        LocomotionField(serializedObject.FindProperty("proneLeft"), Lang.ProneLeft);
                        LocomotionField(serializedObject.FindProperty("proneRight"), Lang.ProneLeft);
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        if (GUILayout.Button(Lang.Default))
                        {
                            serializedObject.FindProperty("proneBlendTree").objectReferenceValue = VRCAssets.proneBlendTree;
                        }
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("proneBlendTree"), new GUIContent(Lang.BlendTree));
                    }
                }

                jumpFold = EditorGUILayout.Foldout(jumpFold, Lang.LocomotionJump);
                if (jumpFold)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(Lang.Default))
                    {
                        SetDefaultJumpLocomotion(serializedObject);
                    }
                    if (GUILayout.Button(Lang.Clear))
                    {
                        ClearJumpLocomotion(serializedObject);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginVertical(GUI.skin.box);
                    LocomotionFieldHeader();
                    LocomotionField(serializedObject.FindProperty("shortFall"), Lang.ShortFall);
                    LocomotionField(serializedObject.FindProperty("longFall"), Lang.LongFall);
                    LocomotionField(serializedObject.FindProperty("quickLand"), Lang.QuickLand);
                    LocomotionField(serializedObject.FindProperty("land"), Lang.Land);
                    GUILayout.EndVertical();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("controller"), new GUIContent(Lang.AnimatorController));
                if (GUILayout.Button(Lang.Default))
                {
                    serializedObject.FindProperty("controller").objectReferenceValue = VRCAssets.locomotionController;
                }
                GUILayout.EndHorizontal();
            }
            

            otherFold = EditorGUILayout.Foldout(otherFold, Lang.LocomotionOther);
            if (otherFold)
            {
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Lang.Default))
                {
                    SetDefaultOtherLocomotion(serializedObject);
                }
                if (GUILayout.Button(Lang.Clear))
                {
                    ClearOtherLocomotion(serializedObject);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                LocomotionFieldHeader();
                LocomotionField(serializedObject.FindProperty("afk"), Lang.AFK);
                GUILayout.EndVertical();
            }

            if (GUILayout.Button(Lang.Default))
            {
                SetAllDefaultLocomotion(serializedObject);
            }


            serializedObject.ApplyModifiedProperties();
        }

        public void LocomotionFieldHeader()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(150));
            EditorGUILayout.LabelField(new GUIContent(Lang.AnimClip, Lang.AnimClip));
            EditorGUILayout.LabelField(new GUIContent(Lang.Speed, Lang.Speed), GUILayout.Width(40));
            EditorGUILayout.LabelField(new GUIContent(Lang.Mirror, Lang.Mirror), GUILayout.Width(40));
            GUILayout.EndHorizontal();
        }
        public void LocomotionField(SerializedProperty serializedProperty, string label)
        {
            SerializedProperty animClip = serializedProperty.FindPropertyRelative("animClip");
            SerializedProperty speed = serializedProperty.FindPropertyRelative("speed");
            SerializedProperty mirror = serializedProperty.FindPropertyRelative("mirror");
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent(label, label), GUILayout.Width(150));
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(animClip, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                speed.floatValue = 1;
                mirror.boolValue = false;
            }
            speed.floatValue = EditorGUILayout.FloatField(speed.floatValue, GUILayout.Width(40));
            mirror.boolValue = EditorGUILayout.Toggle(mirror.boolValue, GUILayout.Width(40));
            GUILayout.EndHorizontal();
        }

        static void SetSingleLocomotion(SerializedObject serializedObject, string name, AnimationClip animationClip, float speed, bool mirror)
        {
            SerializedProperty serializedProperty = serializedObject.FindProperty(name);
            serializedProperty.FindPropertyRelative("animClip").objectReferenceValue = animationClip;
            serializedProperty.FindPropertyRelative("speed").floatValue = speed;
            serializedProperty.FindPropertyRelative("mirror").boolValue = mirror;
        }

        static void ClearLocomotions(SerializedObject serializedObject , params string[] names)
        {
            foreach(var name in names)
            {
                SetSingleLocomotion(serializedObject, name, null, 1, false);
            }
        }

        public static void SetDefaultStandLocomotion(SerializedObject serializedObject)
        {
            SetSingleLocomotion(serializedObject, "standStill", VRCAssets.proxy_stand_still, 1, false);
            SetSingleLocomotion(serializedObject, "walkForward", VRCAssets.proxy_walk_forward, 1, false);
            SetSingleLocomotion(serializedObject, "walkBackward", VRCAssets.proxy_walk_backward, 1, false);
            SetSingleLocomotion(serializedObject, "walkLeft", VRCAssets.proxy_strafe_right, 1, true);
            SetSingleLocomotion(serializedObject, "walkRight", VRCAssets.proxy_strafe_right, 1, false);
            SetSingleLocomotion(serializedObject, "walkForwardLeft", VRCAssets.proxy_strafe_right_45, 1, true);
            SetSingleLocomotion(serializedObject, "walkForwardRight", VRCAssets.proxy_strafe_right_45, 1, false);
            SetSingleLocomotion(serializedObject, "walkBackwardLeft", VRCAssets.proxy_strafe_right_135, 1, true);
            SetSingleLocomotion(serializedObject, "walkBackwardRight", VRCAssets.proxy_strafe_right_135, 1, false);
            SetSingleLocomotion(serializedObject, "runForward", VRCAssets.proxy_run_forward, 1, false);
            SetSingleLocomotion(serializedObject, "runBackward", VRCAssets.proxy_run_backward, 1, false);
            SetSingleLocomotion(serializedObject, "runLeft", VRCAssets.proxy_run_strafe_right, 2, true);
            SetSingleLocomotion(serializedObject, "runRight", VRCAssets.proxy_run_strafe_right, 2, false);
            SetSingleLocomotion(serializedObject, "runForwardLeft", VRCAssets.proxy_run_strafe_right_45, 1, true);
            SetSingleLocomotion(serializedObject, "runForwardRight", VRCAssets.proxy_run_strafe_right_45, 1, false);
            SetSingleLocomotion(serializedObject, "runBackwardLeft", VRCAssets.proxy_run_strafe_right_45, 1, true);
            SetSingleLocomotion(serializedObject, "runBackwardRight", VRCAssets.proxy_run_strafe_right_45, 1, false);
            SetSingleLocomotion(serializedObject, "sprintForward", VRCAssets.proxy_sprint_forward, 1, false);
        }

        public static void SetDefaultCrouchLocomotion(SerializedObject serializedObject)
        {
            SetSingleLocomotion(serializedObject, "crouchStill", VRCAssets.proxy_crouch_still, 1, false);
            SetSingleLocomotion(serializedObject, "crouchForward", VRCAssets.proxy_crouch_walk_forward, 1, false);
            SetSingleLocomotion(serializedObject, "crouchBackward", VRCAssets.proxy_crouch_walk_forward, -1, true);
            SetSingleLocomotion(serializedObject, "crouchLeft", VRCAssets.proxy_crouch_walk_right, 1, true);
            SetSingleLocomotion(serializedObject, "crouchRight", VRCAssets.proxy_crouch_walk_right, 1, false);
            SetSingleLocomotion(serializedObject, "crouchForwardLeft", VRCAssets.proxy_crouch_walk_right_45, 1, true);
            SetSingleLocomotion(serializedObject, "crouchForwardRight", VRCAssets.proxy_crouch_walk_right_45, 1, false);
            SetSingleLocomotion(serializedObject, "crouchBackwardLeft", VRCAssets.proxy_crouch_walk_right_135, 1, true);
            SetSingleLocomotion(serializedObject, "crouchBackwardRight", VRCAssets.proxy_crouch_walk_right_135, 1, false);
        }

        public static void SetDefaultProneLocomotion(SerializedObject serializedObject)
        {
            SetSingleLocomotion(serializedObject, "proneStill", VRCAssets.proxy_low_crawl_still, 1, false);
            SetSingleLocomotion(serializedObject, "proneForward", VRCAssets.proxy_low_crawl_forward, 0.5f, false);
            SetSingleLocomotion(serializedObject, "proneBackward", VRCAssets.proxy_low_crawl_forward, -0.5f, true);
            SetSingleLocomotion(serializedObject, "proneLeft", VRCAssets.proxy_low_crawl_right, 0.5f, true);
            SetSingleLocomotion(serializedObject, "proneRight", VRCAssets.proxy_low_crawl_right, 0.5f, false);
        }

        public static void SetDefaultJumpLocomotion(SerializedObject serializedObject)
        {
            SetSingleLocomotion(serializedObject, "shortFall", VRCAssets.proxy_fall_short, 1, false);
            SetSingleLocomotion(serializedObject, "longFall", VRCAssets.proxy_fall_long, 1, false);
            SetSingleLocomotion(serializedObject, "quickLand", VRCAssets.proxy_land_quick, 1, false);
            SetSingleLocomotion(serializedObject, "land", VRCAssets.proxy_landing, 1, false);
        }

        public static void SetDefaultOtherLocomotion(SerializedObject serializedObject)
        {
            SetSingleLocomotion(serializedObject, "afk", VRCAssets.proxy_afk, 1, false);
        }

        public static void ClearStandLocomotion(SerializedObject serializedObject)
        {
            ClearLocomotions(serializedObject,
                "standStill",
                "walkForward",
                "walkBackward",
                "walkLeft",
                "walkRight",
                "walkForwardLeft",
                "walkForwardRight",
                "walkBackwardLeft",
                "walkBackwardRight",
                "runForward",
                "runBackward",
                "runLeft",
                "runRight",
                "runForwardLeft",
                "runForwardRight",
                "runBackwardLeft",
                "runBackwardRight",
                "sprintForward"
                );
        }

        public static void ClearCrouchLocomotion(SerializedObject serializedObject)
        {
            ClearLocomotions(serializedObject,
                "crouchStill",
                "crouchForward",
                "crouchBackward",
                "crouchLeft",
                "crouchRight",
                "crouchForwardLeft",
                "crouchForwardRight",
                "crouchBackwardLeft",
                "crouchBackwardRight"
                );

        }

        public static void ClearProneLocomotion(SerializedObject serializedObject)
        {
            ClearLocomotions(serializedObject,
                "proneStill",
                "proneForward",
                "proneBackward",
                "proneLeft",
                "proneRight"
                );
            
        }

        public static void ClearJumpLocomotion(SerializedObject serializedObject)
        {
            ClearLocomotions(serializedObject,
                "shortFall",
                "longFall",
                "quickLand",
                "land"
               );
            
        }

        public static void ClearOtherLocomotion(SerializedObject serializedObject)
        {
            ClearLocomotions(serializedObject,
                "afk"
               );
        }


        public static void SetAllDefaultLocomotion(SerializedObject serializedObject)
        {
            serializedObject.Update();
            SetDefaultStandLocomotion(serializedObject);
            SetDefaultCrouchLocomotion(serializedObject);
            SetDefaultProneLocomotion(serializedObject);
            SetDefaultJumpLocomotion(serializedObject);
            SetDefaultOtherLocomotion(serializedObject);
            serializedObject.FindProperty("controller").objectReferenceValue = VRCAssets.locomotionController;
            serializedObject.FindProperty("standBlendTree").objectReferenceValue = VRCAssets.standBlendTree;
            serializedObject.FindProperty("crouchBlendTree").objectReferenceValue = VRCAssets.crouchBlendTree;
            serializedObject.FindProperty("proneBlendTree").objectReferenceValue = VRCAssets.proneBlendTree;
            serializedObject.ApplyModifiedProperties();
        }

    }
}


