using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Sense;
using UnityEditor;

namespace TechXR.Core.Editor
{
    [CustomEditor(typeof(XRModeSwitch))]
    public class XRModeSwitchEditor : UnityEditor.Editor
    {
        #region PRIVATE FIELDS
        private XRModeSwitch xrModeSwitch;
        #endregion // PRIVATE FIELDS


        private void OnEnable()
        {
            xrModeSwitch = (XRModeSwitch)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            xrModeSwitch.IsAR = EditorGUILayout.Toggle("IsAR", xrModeSwitch.IsAR);

            // If the IsAR option is not checked, show Stereo check
            if (!xrModeSwitch.IsAR)
            {
                xrModeSwitch.IsStereo = EditorGUILayout.Toggle("IsStereo", xrModeSwitch.IsStereo);
            }
        }
    }
}
