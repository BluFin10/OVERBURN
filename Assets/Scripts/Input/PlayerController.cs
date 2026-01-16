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
    [SerializeField] private float chargeRate = 1f;
    [SerializeField] private float maxDashPower = 50f;
    
    private Vector3 _velocity;
    private Vector3 _velocityX;
    private Vector3 _movementDir;
    private Vector3 _dashVelocity;
    private Vector3 dashDir;

    private float _verticalVelocity;

    private float _currentSlow = 1f;

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

        // Combine all velocities: 
        // (A/D Input) + (Gravity/Jump) + (The Dash Burst)
        Vector3 totalMove = (_input.Move.x * transform.right * moveSpeed) +
                            (_verticalVelocity * transform.up) +
                            _dashVelocity;

        _controller.Move(totalMove * (_currentSlow * Time.deltaTime));

        // Apply Drag to the dash velocity so it slows down over time
        // This gives it that "burst then drift" Celeste feeling
        _dashVelocity = Vector3.Lerp(_dashVelocity, Vector3.zero, dashDrag * Time.deltaTime);
    }

    private void HandleMovement(float delta)
    {
        // 1. Setup Mouse/Ray data
        _input.MousePos = Mouse.current.position.ReadValue();
        _input.DashRay = Camera.main.ScreenPointToRay(_input.MousePos);

        if (_isGrounded)
        {
            _verticalVelocity = -2f; // Slight downward force to keep grounded
            _input.DashConsumed = false;
            dashPower = 40f;
            _input.DashFire = false;
            _input.DashActive = false;
            _currentSlow = 1f;
        }

        // 2. Jumping
        if (_isGrounded && _input.JumpPressed)
        {
            _verticalVelocity = jumpForce;
            _input.JumpPressed = false;
        }

        if (!_isGrounded && _input.DashActive && !_input.DashConsumed)
        {
            // Increases smoothly every single frame
            dashPower += chargeRate * Time.deltaTime;
            // Clamp it so they can't charge to infinity
            dashPower = Mathf.Clamp(dashPower, 0, maxDashPower);
            _currentSlow = .25f;
            Debug.Log(dashPower);
        }
        // 3. The Dash Burst (The Celeste Moment)
        if (!_isGrounded && _input.DashFire && !_input.DashConsumed)
        {
            // Calculate Direction towards mouse in World Space
            // We use a plane at the player's Z depth to get an accurate 2D direction
            Plane playerPlane = new Plane(Vector3.forward, transform.position);
            if (playerPlane.Raycast(_input.DashRay, out float distance))
            {
                Vector3 targetPoint = _input.DashRay.GetPoint(distance); 
                dashDir = (targetPoint - transform.position).normalized;

                // Inject the power!
                _dashVelocity = dashDir * dashPower;

                // Optional: Zero out vertical velocity on dash for "tighter" control
                _verticalVelocity = dashDir.y * (dashPower * 0.5f);
            }

            _currentSlow = 1f;
            _input.DashActive = false;
            _input.DashFire = false;
            _input.DashConsumed = true;
            gravity = -60f;
        }
        if (!(_verticalVelocity < -30))
        {
            _verticalVelocity += gravity * delta;
        }
    }
}