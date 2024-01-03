using System.Collections;
using System.Collections.Generic;
using TechXR.Core.Sense;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsPlayerController : MonoBehaviour, IDamageable
{
    #region PUBLIC FIELDS
    public GameObject Body, CharacterBodyContainer;
    public float Health => m_Health;
    public bool UseGravity = true;
    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;
    public float Speed;
    public GameOver GameOverCanvas;
    public HitEffect HitEffectCanvas;
    //
    public float JumpHeight = 3f;
    #endregion // Public Fields
    //
    #region PRIVATE FIELDS
    [SerializeField] private AudioClip m_WalkingSound;
    [SerializeField] private AudioClip m_RunningSound;
    [SerializeField] private float m_Health = 100f;
    private AudioSource m_AudioSource;
    private CharacterController m_CharacterController;
    private float m_Gravity = -9.81f;
    private Vector3 m_InitialPlayerPosition;
    private Quaternion m_InitialPlayerRotation;
    private HealthScoreSystem m_HealthScoreSystem;
    private float m_InitialHealth;
    private Vector3 m_Velocity;
    private bool m_IsGrounded;
    private bool m_IsRunning = false;
    #endregion // Private Fields
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
        m_InitialPlayerPosition = this.transform.position;
        m_InitialPlayerRotation = this.transform.rotation;
        //
        m_InitialHealth = Health;
        m_HealthScoreSystem = FindObjectOfType<HealthScoreSystem>();
        UpdateHealthInfo();
        //
        //RegisterGameOverOnClick(GameOverCanvas);
        GameOverCanvas = FindObjectOfType<GameOver>();
        if (GameOverCanvas)
        {
            GameOverCanvas.gameObject.SetActive(false);
        }
        //
        HitEffectCanvas = FindObjectOfType<HitEffect>();
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
        float x = SenseInput.GetAxis(JoystickAxis.HORIZONTAL);// Input.GetAxis("Horizontal");
        float z = SenseInput.GetAxis(JoystickAxis.VERTICAL);// Input.GetAxis("Vertical");

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
    //
    private void ResetHealthAndScoreInfo()
    {
        if (m_HealthScoreSystem != null)
        {
            m_HealthScoreSystem.UpdateHealth(Health, m_InitialHealth);
            m_HealthScoreSystem.CheckAndRegisterHighScore(m_HealthScoreSystem.PlayerCurrentScore);
            m_HealthScoreSystem.ResetScore();
        }
    }
    //
    private void UpdateHealthInfo()
    {
        if (m_HealthScoreSystem != null)
        {
            m_HealthScoreSystem.UpdateHealth(Health, m_InitialHealth);
        }
    }
    //
    private void RegisterGameOverOnClick(GameObject gameOverCanvas)
    {
        if (gameOverCanvas == null)
            return;

        Button restartButton = gameOverCanvas.transform.GetChild(0).Find("RestartButton").GetComponent<Button>();
        restartButton.onClick.AddListener(RestartGame);
    }
    #endregion // Private Methods
    //
    #region PUBLIC_METHODS
    public void TakeDamage(float damage)
    {
        m_Health -= damage;
        if (m_Health <= 0)
        {
            // Show GameOver Canvas
            if (GameOverCanvas != null)
            {
                // Destroy Enemies
                Enemy[] enemies = FindObjectsOfType<Enemy>();
                foreach (Enemy e in enemies)
                    Destroy(e.gameObject);

                // Disable Guns
                Gun[] guns = FindObjectsOfType<Gun>();
                foreach (Gun g in guns)
                    g.enabled = false;

                //
                GameOverCanvas.gameObject.SetActive(true);
                //
                Vector3 pos = transform.position + transform.forward * 10f;
                pos.y += 2f;
                GameOverCanvas.transform.position = pos;
                GameOverCanvas.transform.rotation = transform.rotation;
                //
            }
            else
            {
                // Destroy Enemies
                Enemy[] enemies = FindObjectsOfType<Enemy>();
                foreach (Enemy e in enemies)
                    Destroy(e.gameObject);

                if (m_HealthScoreSystem)
                {
                    Debug.Log("Your score is : " + m_HealthScoreSystem.Score.text);
                }
            }
            //
            m_Health = 0;
            UpdateHealthInfo();
        }
        else
        {
            UpdateHealthInfo();
        }

        // Play Hit Blood-effect on screen
        if (HitEffectCanvas != null)
        {
            HitEffectCanvas.Hit();
        }
    }
    //
    public void UpgradeHealth()
    {
        m_Health = m_InitialHealth;
        //
        UpdateHealthInfo();
    }
    //
    public void RestartGame()
    {
        // Enable Guns
        Gun[] guns = FindObjectsOfType<Gun>();
        foreach (Gun g in guns)
            g.enabled = true;

        Debug.Log("Respawn the player at..!!" + m_InitialPlayerPosition + " and " + m_InitialPlayerRotation);

        // Reset Transform
        m_CharacterController.enabled = false;
        this.transform.position = m_InitialPlayerPosition;
        this.transform.rotation = m_InitialPlayerRotation;
        m_CharacterController.enabled = true;

        // Reset other variables
        m_Health = m_InitialHealth;
        ResetHealthAndScoreInfo();
        //
        GameOverCanvas.gameObject.SetActive(false);
    }
    #endregion // PUBLIC_METHODS
}