using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalEnemy : MonoBehaviour, IDamageable
{
    public float Speed = 1.0f;
    public float PlayerRange = 15f;
    public float StoppingDistance = 5f;
    public GameObject Explosion;
    public Animator Anim;
    public float Health => m_Health;
    public int DeathPoints = 30;
    //
    private Transform m_PlayerTransform;
    [SerializeField] private float m_Health = 30f;
    [SerializeField] private float m_Damage = 3f;
    private bool m_IsAttacking = false;
    private Image m_HealthBar;
    private float m_InitialHealth;
    private HealthScoreSystem m_HealthScoreSystem;
    //
    private void OnDestroy()
    {
        if (m_HealthScoreSystem != null)
        {
            m_HealthScoreSystem.UpdateScore(DeathPoints);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        //
        m_PlayerTransform = FindObjectOfType<FpsPlayerController>().transform;
        m_HealthScoreSystem = FindObjectOfType<HealthScoreSystem>();
        m_HealthBar = GetComponentInChildren<Image>();
        m_InitialHealth = m_Health;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = CheckDistance(m_PlayerTransform.position, transform.position);
        if (dist < PlayerRange && dist > StoppingDistance)
        {
            transform.LookAt(new Vector3(m_PlayerTransform.transform.position.x, transform.position.y, m_PlayerTransform.position.z));
            //
            var step = Speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, m_PlayerTransform.position, step);
            //
            Anim.SetTrigger("walk");
        }
        else if (dist <= StoppingDistance)
        {
            Anim.ResetTrigger("walk");
            Anim.SetTrigger("attack");
            //
            if (Vector3.Angle(this.transform.forward, m_PlayerTransform.forward) > 90f)
            {
                if (!m_IsAttacking)
                {
                    StartCoroutine(Attack());
                }
            }
        }
        else
        {
            Anim.ResetTrigger("walk");
            Anim.SetTrigger("idle");
        }
    }
    //
    private float CheckDistance(Vector3 player, Vector3 enemy)
    {
        return Vector3.Distance(player, enemy);
    }
    //
    IEnumerator Attack()
    {
        m_IsAttacking = true;

        m_PlayerTransform.GetComponent<FpsPlayerController>().TakeDamage(m_Damage);

        //Debug.Log("Attacking------->>>>>>>");

        yield return new WaitForSeconds(3f);

        m_IsAttacking = false;
    }
    //
    public void Death()
    {
        Explosion.SetActive(true);
        Explosion.transform.SetParent(null);
        Destroy(gameObject);
    }
    //
    private void UpdateHealthBar()
    {
        if (m_HealthBar != null)
        {
            m_HealthBar.fillAmount = (m_Health / m_InitialHealth);
        }
    }
    //
    public void TakeDamage(float damage)
    {
        m_Health -= damage;
        if (m_Health <= 0)
        {
            Death();
        }
        //
        UpdateHealthBar();
    }
}
