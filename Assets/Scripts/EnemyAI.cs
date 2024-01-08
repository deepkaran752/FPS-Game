using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Transform Player;
    public LayerMask isPlayer;

    List<Transform> points;
    int destPoints = 0;
    NavMeshAgent agent;

    public Shooting enemyShoot;

    public float sightRange, attackRange;
    private bool inSightRange, inAttackRange;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("XRPlayerController").transform;
        agent = GetComponent<NavMeshAgent>();
        points = EnemySpawner.spawnPoints;
    }

    // Update is called once per frame
    void Update()
    {
        inSightRange = Physics.CheckSphere(transform.position, sightRange, isPlayer);
        inAttackRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);

        if (!inSightRange && !inAttackRange) Patrol();
        else if (inSightRange && !inAttackRange) Chase();
        else if (inSightRange && inAttackRange) Attack();
    }

    //for patroling
    void Patrol()
    {
        if(!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (points.Count == 0)
                return;

            agent.destination = points[destPoints].position;
            destPoints = (destPoints + 1) % points.Count;
        }
    }
    
    //for chasing 
    void Chase()
    {
        agent.SetDestination(Player.position);
    }

    //for attacking
    void Attack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(Player);
        enemyShoot.Shoot();
    }
}
