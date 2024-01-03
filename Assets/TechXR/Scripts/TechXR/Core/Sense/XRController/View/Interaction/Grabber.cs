using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Hand grabber functionality class.
    /// Added to the main XR Pointer
    /// </summary>
    public class Grabber : MonoBehaviour
    {
        #region PUBLIC FIELDS
        public enum GrabType
        {
            GRAB_AND_THROW,
            DISTANCE_LIFT,
            HOLD
        }
        public GrabType SelectGrabType = GrabType.HOLD;
        //[Tooltip("Pick the object from where it is")]
        //public bool DistanceLift;
        [Tooltip("Set the RigidBody properties to default of the grabbed object")]
        public bool SetRigidBodyToDefault = false;
        [Tooltip("Check the Box to make smooth movement of object while moving")]
        public bool MakeChildOfCamera = false;
        public float TimeDelay = 8f;
        #endregion // Public Fields
        //
        #region PRIVATE FIELDS
        private GameObject m_ObjectToGrab;
        private GameObject m_AnchorPoint;
        private Rigidbody m_HoldingTarget;
        private bool m_IsHavingRigidbody;
        private GameObject m_RefObject;
        //
        private GameObject m_Camera;
        private GameObject m_TargetParent;
        //
        #endregion // Private Fields
        //
        #region MONOBEHAVIOUR METHODS
        protected virtual void Awake()
        {
            m_Camera = this.transform.parent.gameObject;
        }
        //
        protected virtual void Start()
        {
            ControllerFactory.GetIXR().AddEventListener(SenseEvent.OBJECT_GRABBED, TryGrabObject);
            ControllerFactory.GetIXR().AddEventListener(SenseEvent.OBJECT_RELEASED, DropObject);
        }
        //
        protected virtual void Update()
        {
            if (SelectGrabType == GrabType.DISTANCE_LIFT && m_ObjectToGrab)
            {
                // Assign the position and rotation of new gameobject to the ObjectToGrab
                m_ObjectToGrab.transform.position = m_RefObject.transform.position;
                m_ObjectToGrab.transform.rotation = m_RefObject.transform.rotation;
            }
        }
        //
        protected virtual void FixedUpdate()
        {
            if (m_ObjectToGrab)
            {
                if (SelectGrabType == GrabType.GRAB_AND_THROW)
                {
                    // Assign Velocity to the target rigidbody
                    //m_HoldingTarget.velocity = (transform.position - m_AnchorPoint.transform.position) / Time.fixedDeltaTime;
                    m_HoldingTarget.velocity = (transform.position - m_AnchorPoint.transform.position) / Time.deltaTime * 1 / TimeDelay;

                    m_HoldingTarget.maxAngularVelocity = 20f;

                    // Calculate Rotation
                    Quaternion deltaRot = transform.rotation * Quaternion.Inverse(m_AnchorPoint.transform.rotation);
                    Vector3 eularRot = new Vector3(Mathf.DeltaAngle(0, deltaRot.eulerAngles.x), Mathf.DeltaAngle(0, deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, deltaRot.eulerAngles.z));
                    eularRot *= 0.95f;
                    eularRot *= Mathf.Deg2Rad;

                    // Assign angular velocity to the target rigidbody
                    m_HoldingTarget.angularVelocity = eularRot / Time.fixedDeltaTime * 1 / TimeDelay;
                }
                if (SelectGrabType == GrabType.HOLD)
                {
                    m_ObjectToGrab.transform.position = this.transform.position;
                    m_ObjectToGrab.transform.rotation = this.transform.rotation;
                }
            }
        }
        #endregion // Monobehaviour Methods
        //
        #region PUBLIC METHODS
        /// <summary>
        /// Grab object
        /// Needs two arguments:
        /// 1. GameObject to be grabbed
        /// 2. Anchor point object of the GameObject. If null then the default center and orientation is used
        /// </summary>
        /// <param name="args"></param>
        public void TryGrabObject(params object[] args)
        {
            // Assign all the fields
            GameObject objectToGrab = (GameObject)args[0];
            GameObject anchorPoint = (GameObject)args[1];

            this.m_ObjectToGrab = objectToGrab;

            if (SelectGrabType == GrabType.DISTANCE_LIFT)
            {
                // Create a new GameObject
                GameObject _newGameObject = new GameObject();

                // Assign the position and rotation as same as the Object to be Grabbed
                _newGameObject.transform.position = objectToGrab.transform.position;
                _newGameObject.transform.rotation = objectToGrab.transform.rotation;

                // Set this gameobject as the parent
                _newGameObject.transform.SetParent(this.gameObject.transform);

                // Give the reference to our private field
                m_RefObject = _newGameObject;
            }
            else if (SelectGrabType == GrabType.GRAB_AND_THROW)
            {
                // Make Child of camera
                if (MakeChildOfCamera)
                {
                    if (objectToGrab.transform.parent)
                        m_TargetParent = objectToGrab.transform.parent.gameObject;
                    if (m_Camera) objectToGrab.transform.SetParent(m_Camera.transform);
                }

                // Assign the anchorpoint
                if (anchorPoint)
                    this.m_AnchorPoint = anchorPoint;
                else
                    this.m_AnchorPoint = objectToGrab;

                // Set Bool for removing the rigidbody after release
                if (objectToGrab.GetComponent<Rigidbody>() == null)
                {
                    m_IsHavingRigidbody = false;
                    m_HoldingTarget = objectToGrab.AddComponent<Rigidbody>();
                }
                else
                {
                    m_IsHavingRigidbody = true;
                    m_HoldingTarget = objectToGrab.GetComponent<Rigidbody>();
                }

                // Set the rigidbody properties
                m_HoldingTarget.interpolation = RigidbodyInterpolation.Extrapolate;
                m_HoldingTarget.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                // Set the toggle pointer and ControllerBody off 
                ControllerFactory.GetIXR().TogglePointerDisplay(false);
                ControllerFactory.GetIXR().ToggleControllerBodyDisplay(false);
            }
            else
            {
                // Set the toggle pointer and ControllerBody off 
                ControllerFactory.GetIXR().TogglePointerDisplay(false);
                ControllerFactory.GetIXR().ToggleControllerBodyDisplay(false);
            }
        }

        /// <summary>
        /// Release the GameObject
        /// </summary>
        /// <param name="args"></param>
        public void DropObject(params object[] args)
        {
            GameObject gameObject = (GameObject)args[0];

            // If gameObject is null return
            if (!gameObject)
                return;

            // Remove the reference for Object
            m_ObjectToGrab = null;

            // Destroy the new GameObject that we have made
            if (SelectGrabType == GrabType.DISTANCE_LIFT)
            {
                Destroy(m_RefObject.gameObject);
            }
            // Remove all the assigned references
            else if (SelectGrabType == GrabType.GRAB_AND_THROW)
            {
                // Revert the parent
                if (MakeChildOfCamera)
                {
                    if (m_TargetParent)
                        gameObject.transform.SetParent(m_TargetParent.transform);

                    else
                    {
                        gameObject.transform.SetParent(null);
                    }

                    m_TargetParent = null;
                }

                // Set back the rigidbody properties
                if (SetRigidBodyToDefault)
                {
                    m_HoldingTarget.interpolation = RigidbodyInterpolation.None;
                    m_HoldingTarget.collisionDetectionMode = CollisionDetectionMode.Discrete;
                }

                // Destroy if the rigidbody was not there
                if (!m_IsHavingRigidbody)
                {
                    Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                    Destroy(rb);
                }
                m_HoldingTarget = null;
                m_AnchorPoint = null;

                // set the toggle pointer and ControllerBody on
                ControllerFactory.GetIXR().TogglePointerDisplay(true);
                ControllerFactory.GetIXR().ToggleControllerBodyDisplay(true);
            }
            else
            {
                // set the toggle pointer and ControllerBody on
                ControllerFactory.GetIXR().TogglePointerDisplay(true);
                ControllerFactory.GetIXR().ToggleControllerBodyDisplay(true);
            }
        }

        /// <summary>
        /// Check to see if the current gameobject is grabbable
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public bool CheckIfGrabbableObject(GameObject gameObject)
        {
            // Return if object is  null
            if (!gameObject)
            {
                Debug.Log("Null Object Found...!!");
                return false;
            }

            // Return true if its a Grabbable object otherwise return false
            if (gameObject.GetComponent<Grabbable>())
                return true;

            return false;
        }
        #endregion // Public Methods
    }
}
