using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Simple image effect for testing anaglyph effect with post-processing.
 */
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[RequireComponent(typeof(Camera))]
[AddComponentMenu("")]
public class ExampleImageEffect : MonoBehaviour
{
    private Shader m_FxShader;
    private Material m_Mat;

    private void OnEnable()
    {
        if (!m_FxShader &&
            !(m_FxShader = Shader.Find("Hidden/ExampleImageEffect")))
        {
            enabled = false;
            return;
        }

        m_Mat = new Material(m_FxShader);
        m_Mat.hideFlags = HideFlags.HideAndDontSave;
    }

    private void OnDisable()
    {
        if (m_Mat != null) DestroyImmediate(m_Mat);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (!m_Mat)
        {
            Debug.LogWarning("Could not render image.");
            enabled = false;
            return;
        }

        Graphics.Blit(src, dst, m_Mat);
    }
}
