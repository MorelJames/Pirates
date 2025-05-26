using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    private NavMeshAgent agent;

    private Transform player;

    [SerializeField]
    private LayerMask navigableLayer, playerLayer;

    [SerializeField]
    private float health;

    private Collider collider;

    [SerializeField]
    private Transform canonBallSpawnPos;

    [Header("Patroling")] 
    [SerializeField]
    private float walkPointRange;
    private Vector3 walkPoint;
    bool walkPointSet;

    [Header("Attacking")]
    [SerializeField]
    private float timeBetweenAttacks;
    bool alreadyAttacked;
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    [SerializeField]
    private int angleMargin;
    [SerializeField]
    private float turnSpeed;

    [SerializeField]
    private Slider healthBar;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        collider = GetComponent<Collider>();
        healthBar.maxValue = health;
        healthBar.value = health;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, navigableLayer))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        RotateToSide();

        if (!alreadyAttacked)
        {
            Rigidbody rb = Instantiate(projectile, canonBallSpawnPos.position, Quaternion.identity).GetComponent<Rigidbody>();
            Physics.IgnoreCollision(rb.gameObject.GetComponent<Collider>(), collider);
            rb.AddForce((player.position - transform.position).normalized * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void RotateToSide()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        Vector3 dirToPlayer = toPlayer.normalized;

        Vector3 rightDir = Quaternion.Euler(0, 90, 0) * dirToPlayer;
        Vector3 leftDir = Quaternion.Euler(0, -90, 0) * dirToPlayer;

        Vector3 objectForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

        float angleToRight = Vector3.Angle(objectForward, rightDir);
        float angleToLeft = Vector3.Angle(objectForward, leftDir);
        float closestAngle = Mathf.Min(angleToRight, angleToLeft);

        if (closestAngle >= 90 - angleMargin && closestAngle <= 90 + angleMargin)
        {
            Vector3 targetDir = (angleToRight < angleToLeft) ? rightDir : leftDir;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    private void DestroyEnemy()
    {
        SpawnerManager.Instance.SpawnEnemy();
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
}
