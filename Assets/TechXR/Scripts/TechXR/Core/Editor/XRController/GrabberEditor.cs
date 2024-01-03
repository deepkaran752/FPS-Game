using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TechXR.Core.Sense;

namespace TechXR.Core.Editor
{
    [CustomEditor(typeof(Grabber))]
    public class GrabberEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(5);

            EditorGUILayout.HelpBox("To prevent the RigidBody Object from falling through the Ground set Interpolation mode to Extrapolate and Collision Detection Mode to Continuous Dynamic inside the rigidbody component of the GameObject", MessageType.Info);
        }
    }
}
