using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public enum State { Roaming, Chasing, Investigating, Retreating }
    private State currentState;

    [Header("References")]
    public Transform player;
    public Transform exit;

    private NavMeshAgent agent;

    [Header("Patrol")]
    public List<Transform> patrolPoints = new List<Transform>();
    private int currentPatrolIndex = 0;
    public float patrolWaitTime = 1.5f;
    private float waitTimer;

    private Vector3 dynamicPlayerPatrolPoint;
    private bool hasDynamicPlayerPoint;

    [Header("Tracking")]
    private float repathTimer;
    private Vector3 trackedPlayerPosition; 
    public float repathInterval = 2f;


    [Header("Chasing")]
    public float sightRange = 10f;
    public LayerMask obstacleMask;

    [Header("Retreat")]
    public float safeDistanceFromExit = 6f;

    [Header("Speed Scaling")]
    [SerializeField] float speedIncreasePerEscape = 0.4f;
    [SerializeField] float maxSpeed = 7f;

    private bool hasCaughtPlayer = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Roaming;
        waitTimer = patrolWaitTime;

        hasDynamicPlayerPoint = false;
        repathTimer = repathInterval;

        agent.stoppingDistance = 0.8f;
    }

    private void Update()
    {
        if (player == null || exit == null) return;

        float distanceToExit = Vector3.Distance(transform.position, exit.position);

        if (distanceToExit < safeDistanceFromExit)
            currentState = State.Retreating;
        else if (CanSeePlayer())
            currentState = State.Chasing;
        else
            currentState = State.Roaming;

        HandleState();
    }

    void HandleState()
    {
        switch (currentState)
        {
            case State.Roaming:
                HandlePatrol();
                break;

            case State.Chasing:
                agent.SetDestination(player.position);
                break;

            case State.Retreating:
                RetreatFromExit();
                break;
        }
    }

    // -------------------------
    // PATROL SYSTEM
    // -------------------------
    void HandlePatrol()
    {
        if (patrolPoints.Count == 0) return;

        Transform target;

        bool isPlayerPoint = (currentPatrolIndex == 1 && patrolPoints.Count > 1);

        // --- PLAYER TRACKING POINT ---
        if (isPlayerPoint)
        {
            // Refresh player position at intervals ONLY (not every frame)
            repathTimer -= Time.deltaTime;

            if (repathTimer <= 0f || !hasDynamicPlayerPoint)
            {
                dynamicPlayerPatrolPoint = player.position;
                hasDynamicPlayerPoint = true;

                repathTimer = repathInterval;
            }

            agent.SetDestination(dynamicPlayerPatrolPoint);

            // Arrival check (IMPORTANT: use NavMesh distance ONLY)
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                AdvancePatrol();
                hasDynamicPlayerPoint = false;
                waitTimer = patrolWaitTime;
            }

            return;
        }

        // --- NORMAL PATROL POINTS ---
        target = patrolPoints[currentPatrolIndex];

        agent.SetDestination(target.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                AdvancePatrol();
                waitTimer = patrolWaitTime;
            }
        }
    }


    void AdvancePatrol()
    {
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        waitTimer = patrolWaitTime;
    }

    // -------------------------
    // DETECTION
    // -------------------------
    bool CanSeePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > sightRange) return false;

        return !Physics.Raycast(transform.position, dir, dist, obstacleMask);
    }

    // -------------------------
    // RETREAT
    // -------------------------
    void RetreatFromExit()
    {
        Vector3 dir = (transform.position - exit.position).normalized;
        Vector3 retreatPoint = transform.position + dir * 10f;

        if (NavMesh.SamplePosition(retreatPoint, out NavMeshHit hit, 10f, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    // -------------------------
    // CATCH SYSTEM
    // -------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (hasCaughtPlayer) return;

        PlayerController pc = other.GetComponent<PlayerController>();

        if (pc != null)
        {
            hasCaughtPlayer = true;
            GameManager.Instance.StartStruggle(pc, this);
        }
    }

    public void ResetCatch()
    {
        hasCaughtPlayer = false;
    }

    public void IncreaseSpeed()
    {
        agent.speed = Mathf.Min(agent.speed + speedIncreasePerEscape, maxSpeed);
    }
}