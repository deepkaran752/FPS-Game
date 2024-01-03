using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    public GameObject Explosion;
    public Animator Anim;
    public float Health => m_Health;
    public int DeathPoints = 30;
    //
    [HideInInspector]
    public EnemyManager EnemyManager;
    //
    private Transform m_PlayerTransform;
    private NavMeshAgent m_Agent;
    [SerializeField] private float m_Health = 30f;
    [SerializeField] private float m_Damage = 3f;
    private bool m_IsAttacking = false;
    private Image m_HealthBar;
    private float m_InitialHealth;
    private HealthScoreSystem m_HealthScoreSystem;
    //
    //
    private void OnDestroy()
    {
        if (m_HealthScoreSystem != null)
        {
            m_HealthScoreSystem.UpdateScore(DeathPoints);
        }
    }
    //
    private void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        //
        m_Agent = GetComponent<NavMeshAgent>();
        m_PlayerTransform = FindObjectOfType<FpsPlayerController>().transform;
        m_HealthScoreSystem = FindObjectOfType<HealthScoreSystem>();
        m_HealthBar = GetComponentInChildren<Image>();
        m_InitialHealth = m_Health;
        //
        m_Agent.SetDestination(m_PlayerTransform.position);
    }
    //
    private void Update()
    {
        m_Agent.SetDestination(m_PlayerTransform.position);

        transform.LookAt(new Vector3(m_PlayerTransform.transform.position.x, transform.position.y, m_PlayerTransform.position.z));

        if (CheckDistance(m_PlayerTransform.position, transform.position) <= 5f)
        {
            Anim.ResetTrigger("walk");
            Anim.SetTrigger("attack");

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
            Anim.SetTrigger("walk");
        }
    }
    //
    private float CheckDistance(Vector3 player, Vector3 enemy)
    {
        return Vector3.Distance(player, enemy);
    }
    //
    public void Death()
    {
        Explosion.SetActive(true);
        Explosion.transform.SetParent(null);
        Destroy(gameObject);
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
    //
    private void UpdateHealthBar()
    {
        if (m_HealthBar != null)
        {
            m_HealthBar.fillAmount = (m_Health / m_InitialHealth);
        }
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
}
