using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCollector : MonoBehaviour
{
    public float pullSpeed;
    private PlayerStats player;
    private CircleCollider2D detector;

    private void Start()
    {
        player = GetComponentInParent<PlayerStats>();
    }

    public void SetRadius(float r)
    {
        if (!detector)
        {
            detector = GetComponent<CircleCollider2D>();
        }
        detector.radius = r;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the other GameObject is a Pickup
        if (collision.TryGetComponent(out Pickup p))
        {
            p.Collect(player, pullSpeed);
        }
    }
}
