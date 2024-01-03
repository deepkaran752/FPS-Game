using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Grabbable Object
    /// Manages all the configurations required to make a 3D object grabbable by the controller
    /// </summary>
    //[RequireComponent(typeof(Rigidbody))]
    public class Grabbable : MonoBehaviour, IObject, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        #region PUBLIC FIELDS
        [Tooltip("To hold this object from a particular angle Make a new Empty GameObject as a child of this object give it the Direction and angle as you want and assign it to this Field")]
        public GameObject AnchorPoint;
        #endregion // Public Fields
        //
        #region MONOBEHAVIOUR METHODS
        void Start()
        {
            // Set the rigidbody property
            //this.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        #endregion // Monobehaviour Methods
        //
        #region PUBLIC METHODS
        /// <summary>
        /// Get local position of the object
        /// </summary>
        /// <returns></returns>
        public Vector3 GetObjectLocalPosition()
        {
            return this.gameObject.transform.localPosition;
        }

        /// <summary>
        /// Get local rotation of the object
        /// </summary>
        /// <returns></returns>
        public Quaternion GetObjectLocalRotation()
        {
            return this.gameObject.transform.localRotation;
        }

        /// <summary>
        /// Get global position of the object
        /// </summary>
        /// <returns></returns>
        public Vector3 GetObjectPosition()
        {
            return this.gameObject.transform.position;
        }

        /// <summary>
        /// Get global rotation of the object
        /// </summary>
        /// <returns></returns>
        public Quaternion GetObjectRotation()
        {
            return this.gameObject.transform.rotation;
        }

        /// <summary>
        /// Object grabbed event
        /// Dispatches SenseEvent.OBJECT_GRABBED
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            ControllerFactory.GetIXR().TriggerEvent(SenseEvent.OBJECT_GRABBED, this.gameObject, AnchorPoint);
        }

        /// <summary>
        /// Object released event
        /// Dispatches SenseEvent.OBJECT_RELEASED
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            ControllerFactory.GetIXR().TriggerEvent(SenseEvent.OBJECT_RELEASED, this.gameObject);
        }

        /// <summary>
        /// Object clicked event
        /// Dispatches SenseEvent.OBJECT_CLICKED
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            ControllerFactory.GetIXR().TriggerEvent(SenseEvent.OBJECT_CLICKED, this.gameObject);
        }
        #endregion // Public Methods
    }
}
