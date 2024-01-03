using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Class to check and destroy the warning canvas (trackable warning feature)
    /// </summary>
    public class CheckForWarningFeature : MonoBehaviour
    {
        #region MONOBEHAVIOUR METHODS
        void Start()
        {
            // If there is no SenseXRTrackingStatus class attached destroy the Warning canvas
            SenseXRTrackingStatus obj = FindObjectOfType<SenseXRTrackingStatus>();
            if (!obj) Destroy(this.gameObject);
        }
        #endregion // MonoBehaviour Methods
    }
}