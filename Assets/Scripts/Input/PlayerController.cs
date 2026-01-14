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
    [SerializeField] private float dashDrag = 20f;

    private Vector3 velocity;

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
        if (!isGrounded && _input.DashActive && !_input.DashConsumed)
        {
            _input.DashConsumed = true;
            HandleDash(Time.deltaTime);
        }
        
        else
        {
            HandleMovement(Time.deltaTime);
        }
    }

    private void HandleMovement(float delta)
    {
        isGrounded = _controller.isGrounded;
        _input.MousePos = Mouse.current.position.ReadValue();
        _input.DashRay = Camera.main.ScreenPointToRay(_input.MousePos);

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
            Vector3 dashTarget = _input.DashRay.origin + _input.DashRay.direction * 10f;
            dashTarget.z = transform.position.z;
            Vector3 dashDir = (dashTarget - transform.position).normalized;
            _controller.Move(dashDir * (delta * dashPower));
            Debug.Log(dashDir);
        }

        verticalVelocity += gravity * delta;
        
        Vector3 moveDir = (_input.Move.x * transform.right) +(verticalVelocity*transform.up)+ (_input.Move.y * transform.forward);
        _controller.Move(moveDir * (delta * moveSpeed));
    }

    private void HandleDash(float delta)
    {
        Vector3 dashTarget = _input.DashRay.origin + _input.DashRay.direction * 10f;
        dashTarget.z = transform.position.z;
        Vector3 dashDir = (dashTarget - transform.position).normalized; 
        velocity += dashDir * dashPower * delta;
        if (!isGrounded && _input.DashConsumed)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, dashDrag * delta);
            
            velocity.y += gravity * delta;
            
            _controller.Move(velocity * delta);
        }
        Debug.Log(dashDir);
    }
}