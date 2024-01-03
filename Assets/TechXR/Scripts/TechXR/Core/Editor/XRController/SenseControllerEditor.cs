using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TechXR.Core.Sense;
using UnityEngine.SceneManagement;

namespace TechXR.Core.Editor
{
    [CustomEditor(typeof(SenseController))]
    public class SenseControllerEditor : UnityEditor.Editor
    {
        #region PRIVATE FIELDS
        private SenseController senseController;
        #endregion // PRIVATE FIELDS


        private void OnEnable()
        {
            senseController = (SenseController)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            //
            senseController.Teleport = EditorGUILayout.Toggle("Teleport", senseController.Teleport);


            // If the teleport option is checked, show teleport Button Field for Button selection
            if (senseController.Teleport)
            {
                senseController.UsingOculus = EditorGUILayout.Toggle("Using Oculus", senseController.UsingOculus);

                if (senseController.UsingOculus)
                {
                    senseController.OculusTeleportButton = (ButtonName)EditorGUILayout.EnumPopup("Teleport Button ", senseController.OculusTeleportButton);
                }
                else
                {
                    senseController.SenseXRTeleportButton = (ButtonName)EditorGUILayout.EnumPopup("Teleport Button ", senseController.SenseXRTeleportButton);
                }
            }
            //
            if (EditorGUI.EndChangeCheck())
            {
                // Mark Scene Dirty
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
    }
}