using UnityEngine;
using UnityEngine.UI;

namespace TechXR.Core.Sense
{
    public class SliderValue : MonoBehaviour
    {
        #region PRIVATE MEMBERS
        Text m_ValueText;
        #endregion // PRIVATE MEMBERS


        #region MONOBEHAVIOUR METHODS
        void Start()
        {
            m_ValueText = GetComponent<Text>();
        }
        #endregion // MONOBEHAVIOUR METHODS


        #region PUBLIC METHODS
        public void ValueUpdate(float value)
        {
            m_ValueText.text = value.ToString("F1");
        }
        #endregion // PUBLIC METHODS
    }
}
