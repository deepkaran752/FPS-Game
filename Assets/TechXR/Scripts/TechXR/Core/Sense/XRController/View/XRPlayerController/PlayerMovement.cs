using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Class to manage the Player Movement
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        #region PUBLIC FIELDS
        public GameObject Body, CharacterBodyContainer;
        public bool UseGravity = true;
        public Transform GroundCheck;
        public float GroundDistance = 0.4f;
        public LayerMask GroundMask;
        public float Speed;
        //
        public float JumpHeight = 3f;
        #endregion // Public Fields
        //
        #region PRIVATE FIELDS
        [SerializeField] private AudioClip m_WalkingSound;
        [SerializeField] private AudioClip m_RunningSound;
        private AudioSource m_AudioSource;
        private CharacterController m_CharacterController;
        private float m_Gravity = -9.81f;
        #endregion // Private Fields
        //
        #region INTERAL FIELDS
        Vector3 m_Velocity;
        bool m_IsGrounded;
        bool m_IsRunning = false;
        #endregion // Internal Fields
        //
        #region MONOBEHAVIOUR METHODS

        private void Awake()
        {
            GameObject[] playerBody = GameObject.FindGameObjectsWithTag("CharacterBody");
            foreach (GameObject pb in playerBody) if (pb.activeSelf) Body = pb;

            if (Body == null) Body = CharacterBodyContainer;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_AudioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!SenseInput.Instance.JoystickMovement) return;

            // Check if the player is grounded to give a velocity that can act as gravity.
            m_IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
            if (m_IsGrounded && m_Velocity.y < 0f)
            {
                m_Velocity.y = -2f;
            }

            // Input from Joystick/Keyboard
            float x = SenseInput.GetAxis(JoystickAxis.HORIZONTAL);
            float z = SenseInput.GetAxis(JoystickAxis.VERTICAL);

            // Define the direction where the player has to move.
            Vector3 move = Body.transform.right * x + Body.transform.forward * z;

            // Check if the player is sprinting
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                //Speed = 20f;
                m_IsRunning = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                //Speed = 12f;
                m_IsRunning = false;
            }

            // Play foot step audio
            PlayFootstepSounds(move.magnitude);

            // Give speed per frame to the directional movement.
            if (m_CharacterController.enabled) m_CharacterController.Move(move * Speed * Time.deltaTime);

            // Jump if required
            if (Input.GetKeyDown(KeyCode.Space) && UseGravity && m_IsGrounded)
            {
                // According to Physics -> v = Sqrt(h * -2 * g)
                m_Velocity.y = Mathf.Sqrt(JumpHeight * -2f * m_Gravity);
            }

            // According to physics -> Delta y = 1/2(g * t^2)
            if (UseGravity) m_Velocity.y += m_Gravity * Time.deltaTime;
            if (m_CharacterController.enabled) m_CharacterController.Move(m_Velocity * Time.deltaTime);
        }
        #endregion // Monobehaviour Methods
        //
        #region PRIVATE METHODS
        private void PlayFootstepSounds(float input)
        {
            // If the player is grounded and some input is provided, play the audio otherwise pause the audio.
            if (m_IsGrounded && input != 0f)
            {
                m_AudioSource.clip = m_IsRunning ? m_RunningSound : m_WalkingSound;
                if (!m_AudioSource.isPlaying)
                {
                    m_AudioSource.Play();
                }
            }
            else
            {
                if (m_AudioSource.isPlaying)
                {
                    m_AudioSource.Pause();
                }
            }
        }
        #endregion // Private Methods
    }
}