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

    private Vector3 _velocity;
    private Vector3 _velocityX;
    private Vector3 _movementDir;

    private float _verticalVelocity;

    private bool _isGrounded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = InputManager.instance;
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = _controller.isGrounded;
            HandleMovement(Time.deltaTime);
        if (!_isGrounded && _input.DashConsumed)
        {
            _velocity.x = _velocityX.x * 1.1f;
        }
        
        _controller.Move(_velocity*Time.deltaTime);
    }

    private void HandleMovement(float delta)
    {
        _input.MousePos = Mouse.current.position.ReadValue();
        _input.DashRay = Camera.main.ScreenPointToRay(_input.MousePos);

        if (_isGrounded)
        {
            _verticalVelocity = -2f;
            _input.DashConsumed = false;
        }

        if (_isGrounded && _input.JumpPressed)
        {
            _verticalVelocity = jumpForce;
            _input.JumpPressed = false;
        }

        if (!_isGrounded && _input.DashActive)
        {
            Vector3 dashTarget = _input.DashRay.origin + _input.DashRay.direction * 10f;
            dashTarget.z = transform.position.z;
            Vector3 dashDir = (dashTarget - transform.position).normalized;
            _controller.Move(dashDir * (delta * dashPower));
            _input.DashConsumed = true;
        }

        _verticalVelocity += gravity * delta;
        
        _velocity = (_input.Move.x * transform.right * moveSpeed) +(_verticalVelocity*transform.up)+ (_input.Move.y * transform.forward);
    }
}