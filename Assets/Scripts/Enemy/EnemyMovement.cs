using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private EnemyStats enemy;
    private Transform playerPos;

    private Vector2 knockbackVelocity;
    private float knockbackDuration;

    [Header("Separation Parameters")]
    [SerializeField] private float separationRadius;
    [SerializeField] private float separationForce;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int maxNeighbors;
    private ContactFilter2D contactFilter;
    private Collider2D[] neighbourBuffer = new Collider2D[16];

    private void Awake()
    {
        // Contact filter for enemies separation
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(enemyLayer);
        contactFilter.useLayerMask = true;
    }

    private void Start()
    {
        enemy = GetComponent<EnemyStats>();
        playerPos = FindAnyObjectByType<PlayerMovement>().transform;
    }

    private void Update()
    {
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
            return;
        }
        
        Vector2 moveToPlayer = (playerPos.position - transform.position).normalized;
        Vector2 separation = GetSeparation();
        Vector2 finalDir = (moveToPlayer + separation * separationForce).normalized;

        transform.position += (Vector3)(finalDir * enemy.currentMoveSpeed * Time.deltaTime);
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0)
        {
            return;
        }

        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }

    private Vector2 GetSeparation()
    {
        int count = Physics2D.OverlapCircle(
            transform.position,
            separationRadius,
            contactFilter,
            neighbourBuffer
        );

        Vector2 force = Vector2.zero;
        int processed = 0;

        for (int i = 0; i < count; i++)
        {
            Transform other = neighbourBuffer[i].transform;
            if (other == transform)
            {
                continue;
            }

            Vector2 dir = (Vector2)(transform.position - other.position);
            float dist = dir.magnitude;

            if (dist > 0)
            {
                force += dir.normalized / dist;
                processed++;
                
                if (processed >= maxNeighbors)
                {
                    break;
                }
            }
        }

        return force;
    }
}
