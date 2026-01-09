using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private InputManager _input;
    private CharacterController _controller;

    [SerializeField] private float gravity = -20f;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float jumpForce = 7f;

    private float verticalVelocity;

    private bool isGrounded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = InputManager.instance;
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement(Time.deltaTime);
    }

    private void HandleMovement(float delta)
    {
        isGrounded = _controller.isGrounded;

        if (isGrounded)
        {
            verticalVelocity = -2f;
        }

        if (isGrounded && _input.JumpPressed)
        {
            verticalVelocity = jumpForce;
            _input.JumpPressed = false;
        }

        verticalVelocity += gravity * delta;
        
        Vector3 moveDir = (_input.Move.x * transform.right) + (_input.Move.y * transform.forward);
        _controller.Move(moveDir * (delta * moveSpeed));
        
    }
}