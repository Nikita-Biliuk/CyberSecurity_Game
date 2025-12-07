using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private EnemyStats enemy;
    private Transform player;
    private Rigidbody2D rb;

    [Header("Pathfinding")]
    public float repathInterval = 0.8f;  // time between path recalculations
    private float repathTimer = 0f;
    private List<Vector2> path = new List<Vector2>();
    private int pathIndex = 0;

    [Header("Movement")]
    public float arriveRadius = 0.1f;
    public float moveSpeed = 3f;

    [Header("Avoid Other Enemies")]
    public float avoidRadius = 0.6f;      // detection radius to avoid other enemies
    public float avoidForce = 2f;         // pull strength to avoid other enemies
    public LayerMask enemyMask;           // enemy layer mask

    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindObjectOfType<PlayerMovement>().transform;
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (enemy != null && enemy.currentMoveSpeed > 0)
            moveSpeed = enemy.currentMoveSpeed;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // repathing
        repathTimer -= Time.fixedDeltaTime;
        if (repathTimer <= 0f)
        {
            path = Pathfinder.FindPath(rb.position, player.position);
            pathIndex = 0;
            repathTimer = repathInterval;
        }

        if (path == null || path.Count == 0) return;

        // movement towards next waypoint
        Vector2 target = path[Mathf.Clamp(pathIndex, 0, path.Count - 1)];
        Vector2 dir = target - rb.position;
        float dist = dir.magnitude;

        if (dist <= arriveRadius)
        {
            if (pathIndex < path.Count - 1)
                pathIndex++;
            else
                return;
        }
        else
        {
            dir.Normalize();
        }

        // push away from nearby enemies
        Collider2D[] nearby = Physics2D.OverlapCircleAll(rb.position, avoidRadius, enemyMask);
        Vector2 avoidance = Vector2.zero;

        foreach (var other in nearby)
        {
            if (other.gameObject == gameObject) continue;
            Vector2 away = rb.position - (Vector2)other.transform.position;
            float distance = away.magnitude;
            if (distance > 0f)
                avoidance += away.normalized / distance; // the closer, the stronger the force
        }

        if (avoidance != Vector2.zero)
        {
            dir = (dir + avoidance * avoidForce).normalized;
        }

        // Movement
        Vector2 move = dir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.4f, 0.4f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, avoidRadius);
    }
}
