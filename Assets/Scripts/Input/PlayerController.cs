using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private InputManager _input;
    private CharacterController _controller;

    [SerializeField] private float gravity = -20f;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float dashPower = 10f;

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

        if (!isGrounded && _input.DashActive)
        {
            Vector3 dashDir = (_input.MousePos - _controller.transform.right) + (_input.MousePos - _controller.transform.up);
            _controller.Move(dashDir * (delta * dashPower));
            Debug.Log(dashDir);
        }

        verticalVelocity += gravity * delta;
        
        Vector3 moveDir = (_input.Move.x * transform.right) +(verticalVelocity*transform.up)+ (_input.Move.y * transform.forward);
        _controller.Move(moveDir * (delta * moveSpeed));
        
    }
}