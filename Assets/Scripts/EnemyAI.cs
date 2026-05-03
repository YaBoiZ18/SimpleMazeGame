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

    [Header("Player Investigation")]
    public float investigationInterval = 2f;
    private float investigationTimer;
    private Vector3 lastKnownPlayerPos;

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

        lastKnownPlayerPos = player.position;
        investigationTimer = investigationInterval;

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

        // Inject "investigation behavior" every few seconds
        investigationTimer -= Time.deltaTime;

        if (investigationTimer <= 0f)
        {
            lastKnownPlayerPos = player.position;
            investigationTimer = investigationInterval;

            // Force agent to investigate player position
            agent.SetDestination(lastKnownPlayerPos);

            currentState = State.Investigating;
            return;
        }

        Transform target = patrolPoints[currentPatrolIndex];

        agent.SetDestination(target.position);

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.3f)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
                AdvancePatrol();
        }
    }

    // -------------------------
    // INVESTIGATION STATE (Not in use yet)
    // -------------------------
    void HandleInvestigation()
    {
        agent.SetDestination(lastKnownPlayerPos);

        bool reached =
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.4f;

        if (reached)
        {
            currentState = State.Roaming;
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