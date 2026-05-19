using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public float lastHorVector;
    [HideInInspector] public float lastVertVector;
    [HideInInspector] public Vector2 moveDir;
    [HideInInspector] public Vector2 lastMovedVector;

    public const float DEFAULT_MOVESPEED = 4f;

    private Rigidbody2D rigidBody;
    private PlayerStats player;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerStats>();
        lastMovedVector = new Vector2(1f, 0f);
    }

    private void Update()
    {
        InputManagement();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void InputManagement()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x != 0)
        {
            lastHorVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorVector, 0f);
        }

        if (moveDir.y != 0)
        {
            lastVertVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVertVector);
        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorVector, lastVertVector);
        }
    }

    private void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        rigidBody.linearVelocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed;
    }
}
