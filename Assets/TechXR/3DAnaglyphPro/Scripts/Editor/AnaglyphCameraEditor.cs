using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(AnaglyphCamera))]
public class AnaglyphCameraEditor : Editor
{
    private SerializedProperty
        propConvergence,
        propEyeSeparation,
        propScheme,
        propSchemeCustomLeft,
        propSchemeCustomRight,
        propCustomMatrix,
        propUseCustomCamera,
        propCustomCamera,
        propKeyword;

    private static bool showParallax = true,
                 showScheme = true,
                 showAdvanced = false;

    private static readonly string[] matrixRowStrings = new string[3]
    {
        "Out R",
        "Out G",
        "Out B",
    };

    private static readonly string[] matrixColumnStrings = new string[6]
    {
        "L. R",
        "L. G",
        "L. B",
        "R. R",
        "R. G",
        "R. B",
    };

    private void OnEnable()
    {
        propConvergence       = serializedObject.FindProperty("convergence");
        propEyeSeparation     = serializedObject.FindProperty("eyeSeparation");
        propScheme            = serializedObject.FindProperty("scheme");
        propSchemeCustomLeft  = serializedObject.FindProperty("customLeft");
        propSchemeCustomRight = serializedObject.FindProperty("customRight");
        propCustomMatrix      = serializedObject.FindProperty("customMatrix");
        propUseCustomCamera   = serializedObject.FindProperty("useCustomCamera");
        propCustomCamera      = serializedObject.FindProperty("customCamera");
        propKeyword           = serializedObject.FindProperty("correctColourSpace");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        AnaglyphCamera cam = (AnaglyphCamera)target;

        if (showParallax = EditorGUILayout.BeginFoldoutHeaderGroup(showParallax, "Parallax Options"))
        {
            ++EditorGUI.indentLevel;

            propEyeSeparation.floatValue = Mathf.Max(
                EditorGUILayout.FloatField(new GUIContent("Eye Separation (m)"), propEyeSeparation.floatValue),
                0.0f);
            propConvergence.floatValue = Mathf.Max(
                EditorGUILayout.FloatField(new GUIContent("Convergence Distance (m)"), propConvergence.floatValue),
                0.05f);

            --EditorGUI.indentLevel;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (showScheme = EditorGUILayout.BeginFoldoutHeaderGroup(showScheme, "Anaglyph Scheme"))
        {
            ++EditorGUI.indentLevel;

            EditorGUILayout.PropertyField(propScheme, new GUIContent("Scheme Type"));

            if (propScheme.enumValueIndex == (int)AnaglyphScheme.CustomColourPair)
            {
                EditorGUILayout.PropertyField(propSchemeCustomLeft, new GUIContent("Custom Left Colour"));
                EditorGUILayout.PropertyField(propSchemeCustomRight, new GUIContent("Custom Right Colour"));
            }

            int oldIndent = EditorGUI.indentLevel;

            if (propScheme.enumValueIndex == (int)AnaglyphScheme.CustomMatrix)
            {
                {
                    EditorGUI.indentLevel = oldIndent;

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(
                        GUIContent.none,
                        GUILayout.MinWidth(32.0f),
                        GUILayout.ExpandWidth(false));

                    EditorGUI.indentLevel = 0;
                    GUILayout.FlexibleSpace();

                    for (int column = 0; column < 6; ++column)
                    {
                        EditorGUILayout.LabelField(
                            new GUIContent(matrixColumnStrings[column]),
                            GUILayout.MinWidth(0.0f),
                            GUILayout.ExpandWidth(true));
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.BeginChangeCheck();
                for (int row = 0; row < 3; ++row)
                {
                    EditorGUI.indentLevel = oldIndent;

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(
                        new GUIContent(matrixRowStrings[row]),
                        GUILayout.MinWidth(32.0f),
                        GUILayout.ExpandWidth(false));

                    EditorGUI.indentLevel = 0;
                    GUILayout.FlexibleSpace();

                    for (int column = 0; column < 6; ++column)
                    {
                        cam.customMatrix[row * 6 + column] = EditorGUILayout.FloatField(
                            cam.customMatrix[row * 6 + column],
                            GUILayout.MinWidth(8.0f),
                            GUILayout.ExpandWidth(true));
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    cam.UpdateKeyword();
                    EditorApplication.QueuePlayerLoopUpdate();
                }
            }

            EditorGUI.indentLevel = oldIndent;
            --EditorGUI.indentLevel;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (showAdvanced = EditorGUILayout.BeginFoldoutHeaderGroup(showAdvanced, "Advanced Effect Options"))
        {
            ++EditorGUI.indentLevel;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propKeyword, new GUIContent("Correct Colour Space?"));
            if (propKeyword.boolValue)
            {
                EditorGUILayout.HelpBox(
                    "Using correct colour space can improve rendering output, but may degrade performance.  " +
                    (PlayerSettings.colorSpace == ColorSpace.Gamma ?
                         "This option has no effect in Gamma colour space" :
                         ""),
                    MessageType.Info);
            }
            if (EditorGUI.EndChangeCheck())
            {
                cam.UpdateKeyword();
                EditorApplication.QueuePlayerLoopUpdate();
            }

            EditorGUILayout.LabelField("Custom Camera Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Setting a custom secondary camera allows you to use the " +
                "anaglyph effect with other post-processing effects.  " +
                "Please ensure that the secondary camera you provide " +
                "has an identical post-processing stack " +
                "to this camera, in order to achieve correct " +
                "render results.  " +
                "The secondary camera you provide should not " +
                "have the AnaglyphCamera component applied to it!",
                MessageType.Info);

            EditorGUILayout.PropertyField(propUseCustomCamera, new GUIContent("Use Custom Camera?"));
            if (propUseCustomCamera.boolValue)
            {
                EditorGUILayout.PropertyField(propCustomCamera, new GUIContent(""));

                if (cam.customCamera)
                {
                    if (cam.customCamera == cam.GetComponent<Camera>())
                    {
                        EditorGUILayout.HelpBox("Please provide a different camera.",
                            MessageType.Error);
                    }
                    else
                    {
                        if (cam.customCamera.transform.parent != cam.transform)
                        {
                            EditorGUILayout.HelpBox(
                                "It is recommended to set the secondary camera " +
                                "to be a child of this camera's transform.",
                                MessageType.Warning);
                        }

                        if (cam.customCamera.GetComponent<AnaglyphCamera>())
                        {
                            EditorGUILayout.HelpBox(
                                "The secondary camera should not have an AnaglyphCamera component!",
                                MessageType.Error);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Please provide a camera to the field above in order for the effect to work.",
                        MessageType.Warning);
                }
            }

            --EditorGUI.indentLevel;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
