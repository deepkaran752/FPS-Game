using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HitEffect : MonoBehaviour
{
    public float FadeOutDelay = 0.0025f;
    public Image RedEffectImg;
    //
    private bool m_IsHitActive = false;
    private Color m_Color;
    // Start is called before the first frame update
    void Awake()
    {
        m_Color = Color.white;
        m_Color.a = 0;
        //
        RedEffectImg = GetComponentInChildren<Image>();
        RedEffectImg.color = m_Color;
    }
    // Update is called once per frame
    void Update()
    {
        HitEffectNormalise();
    }
    //
    private void HitEffectNormalise()
    {
        if (m_Color.a != 0)
        {
            m_Color.a -= FadeOutDelay;
            RedEffectImg.color = m_Color;
        }
    }
    //
    private IEnumerator HitCoroutine()
    {
        m_Color.a = 0.7f;
        RedEffectImg.color = m_Color;
        //
        m_IsHitActive = true;
        //
        yield return new WaitForSeconds(1f);
        //
        m_IsHitActive = false;
    }
    //
    public void Hit()
    {
        if (!m_IsHitActive)
        {
            StartCoroutine(HitCoroutine());
        }
    }
}












