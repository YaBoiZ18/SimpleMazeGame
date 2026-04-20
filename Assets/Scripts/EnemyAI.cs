using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public enum State { Roaming, Chasing, Retreating } // Added Retreating state
    private State currentState;

    [Header("References")]
    public Transform player;
    public Transform exit;

    private NavMeshAgent agent; // Reference to the NavMeshAgent component

    [Header("Patrol")] // Added patrol variables
    public List<Transform> patrolPoints = new List<Transform>();
    private int currentPatrolIndex = 0;
    public float patrolWaitTime = 1.5f;
    private float waitTimer;

    [Header("Chasing")] // Added chasing variables
    public float sightRange = 10f;
    public LayerMask obstacleMask; // Layer mask to detect obstacles between enemy and player

    [Header("Retreat")] // Added retreating variables
    public float safeDistanceFromExit = 6f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Roaming;
        waitTimer = patrolWaitTime;
    }

    private void Update()
    {
        // Check if player and exit references are assigned
        if (player == null || exit == null) return;

        float distanceToExit = Vector3.Distance(transform.position, exit.position);

        if (distanceToExit < safeDistanceFromExit)
        {
            currentState = State.Retreating;
        }
        else if (CanSeePlayer())
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Roaming;
        }

        HandleState();
    }

    void HandleState() // Refactored state handling into a separate method
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

    void HandlePatrol() // Refactored patrol logic into a separate method
    {
        if (patrolPoints.Count == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];

        agent.SetDestination(target.position);

        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
                waitTimer = patrolWaitTime;
            }
        }
    }

    bool CanSeePlayer() // Refactored player detection logic into a separate method
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > sightRange) return false;

        if (!Physics.Raycast(transform.position, direction, distance, obstacleMask))
        {
            return true;
        }

        return false;
    }

    void RetreatFromExit() // Refactored retreating logic into a separate method
    {
        Vector3 dir = (transform.position - exit.position).normalized;
        Vector3 retreatPoint = transform.position + dir * 10f;

        if (NavMesh.SamplePosition(retreatPoint, out NavMeshHit hit, 10f, 1))
        {
            agent.SetDestination(hit.position);
        }
    }
}