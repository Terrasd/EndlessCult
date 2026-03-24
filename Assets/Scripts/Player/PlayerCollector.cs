using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    public float pullSpeed;

    private PlayerStats player;
    private CircleCollider2D playerCollector;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        playerCollector.radius = player.CurrentMagnet;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the other GameObject has the ICollectible interface
        if (collision.gameObject.TryGetComponent(out ICollectible collectible))
        {
            // Pulling animation
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(forceDirection * pullSpeed);

            collectible.Collect();
        }
    }
}
