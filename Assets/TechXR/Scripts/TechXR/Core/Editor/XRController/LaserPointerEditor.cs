using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Sense;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace TechXR.Core.Editor
{
    [CustomEditor(typeof(LaserPointer))]
    public class LaserPointerEditor : UnityEditor.Editor
    {
        #region PRIVATE FIELDS
        private LaserPointer laserPointer;
        #endregion // PRIVATE FIELDS


        private void OnEnable()
        {
            laserPointer = (LaserPointer)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            //
            laserPointer.UsingOculus = EditorGUILayout.Toggle("Using Oculus", laserPointer.UsingOculus);


            // If the teleport option is checked, show teleport Button Field for Button selection
            if (laserPointer.UsingOculus)
            {
                laserPointer.OculusTriggerButton = (ButtonName)EditorGUILayout.EnumPopup("Trigger Button ", laserPointer.OculusTriggerButton);
            }
            else
            {
                laserPointer.SenseXRTriggerButton = (ButtonName)EditorGUILayout.EnumPopup("Trigger Button ", laserPointer.SenseXRTriggerButton);
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