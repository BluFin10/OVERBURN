using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputManager _input;
    private CharacterController _controller;

    [Header("Movement")]
    [SerializeField] private float gravity = -60f;
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float baseJumpForce = 20f;
    [SerializeField] private float maxJumpForce = 35f;
    [SerializeField] private float jumpChargeSpeed = 20f;
    [SerializeField] private float fastFallMultiplier = 1.5f;
    [SerializeField] private float fastFallLimit = -50f;
    [Header("Friction & Drag")]
    [SerializeField] private float groundFriction = 5f; 
    [SerializeField] private float airFriction = 2f;         
    [SerializeField] private float horizontalAcceleration = 10f; 
    [Header("Dash Settings")]
    [SerializeField] public float minDashPower = 20f;
    [SerializeField] public float maxDashPower = 50f;
    [SerializeField] private float chargeRate = 50f;
    [SerializeField] private float maxPowerHoldTime = .25f;
    

    private Vector3 _dashVelocity;
    private Vector3 _horizontalVelocity;
    private float _verticalVelocity;
    private float _jumpPower;
    public float dashPower;
    public bool dashActive;
    private float _currentSlow = 1f;
    private float _currentHoldTime;
    private bool _isGrounded;
    private bool _maxCharge;
    private bool _jumpConsumed;
    private float _currentFallModifier = 1f;

    void Start()
    {
        _input = InputManager.instance;
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        _isGrounded = _controller.isGrounded;

        HandleMovement(Time.deltaTime);
        HandleDash(Time.deltaTime);

        // --- CONTEXTUAL FRICTION ---
        // 1. Determine active friction
        float activeDrag = _isGrounded ? groundFriction : airFriction;

        // 2. Calculate Target Input Velocity
        Vector3 targetInputVelocity = _input.Move.x * transform.right * moveSpeed;

        // 3. Move current velocity toward target input
        // This makes starting/stopping feel "weighty" rather than instant
        _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetInputVelocity, horizontalAcceleration * Time.deltaTime);

        // 4. Apply Drag to the Dash specifically, OR the whole horizontal vector
        // To make the dash feel separate but still affected by air/ground:
        _dashVelocity = Vector3.Lerp(_dashVelocity, Vector3.zero, activeDrag * Time.deltaTime);

        // 5. Combine and Move
        Vector3 finalMotion = (_horizontalVelocity + _dashVelocity + (Vector3.up * _verticalVelocity));
        _controller.Move(finalMotion * (_currentSlow * Time.deltaTime));
    }

    private void HandleMovement(float delta)
    {
        if (_isGrounded)
        {
            _jumpConsumed = false;
            _verticalVelocity = -5f;
            if (_input.JumpPressed && !_jumpConsumed)
            {
                if (_jumpPower < baseJumpForce)
                {
                    _jumpPower = baseJumpForce;
                }
                if (_jumpPower <= maxJumpForce)
                {
                    _jumpPower += jumpChargeSpeed*delta;
                }
                else
                {
                    _jumpPower = maxJumpForce;
                }
                float powerRatio = (_jumpPower - baseJumpForce) / (maxJumpForce - baseJumpForce);
                _currentSlow = Mathf.Lerp(1.0f, .05f, powerRatio);
                Debug.Log(_jumpPower+" power");
                Debug.Log(_currentSlow+" slow");
            }

            if (_input.JumpFire && !_jumpConsumed)
            {
                _input.JumpPressed = false;
                _verticalVelocity = _jumpPower;
                _currentSlow = 1;
                _jumpPower = baseJumpForce;
                _jumpConsumed = true;
                _input.JumpFire = false;
            }
        }
        else
        {
            _jumpPower = baseJumpForce;
            if (!_input.DashActive)
            {
                _currentSlow = 1;
            } 
            _jumpConsumed = true;
            _input.JumpFire = false;
            _input.JumpPressed = false;
        }

        if (_input.Move.y < -0.1)
        {
            if (_verticalVelocity > fastFallLimit)
            {
                _verticalVelocity += gravity * fastFallMultiplier * delta;
            }
        }
        if (_verticalVelocity > -30f)
        {
            _verticalVelocity += gravity * delta;
        }
    }

    private void HandleDash(float delta)
    {
        _input.MousePos = Mouse.current.position.ReadValue();
        _input.DashRay = Camera.main.ScreenPointToRay(_input.MousePos);

        if (_isGrounded)
        {
            _horizontalVelocity += _dashVelocity * 0.5f; 
            _dashVelocity = Vector3.zero;
            _input.DashConsumed = false;
            _input.DashFire = false;
            _input.DashActive = false;
            dashPower = minDashPower;
            _maxCharge = false;
        }
        
        if (_input.DashActive && !_input.DashConsumed)
        {
            dashActive = true;
            if (!_maxCharge)
            {
                if (dashPower < maxDashPower)
                {
                    dashPower += chargeRate * delta;
                    if (dashPower >= maxDashPower)
                    {
                        dashPower = maxDashPower;
                        _currentHoldTime = 0f;
                    }
                }
                else if (_currentHoldTime < maxPowerHoldTime)
                {
                    _currentHoldTime += delta;
                    if (_currentHoldTime >= maxPowerHoldTime) _maxCharge = true;
                }
            }
            else
            {
                dashPower -= chargeRate * delta;
                if (dashPower < minDashPower) dashPower = minDashPower;
            }

            float powerRatio = (dashPower - minDashPower) / (maxDashPower - minDashPower);
            _currentSlow = Mathf.Lerp(1.0f, 0.25f, powerRatio);
            Debug.Log(dashPower + " Power");
            Debug.Log(_currentSlow + " Slow");
        }
        
        if (_input.DashFire && !_input.DashConsumed)
        {
            Plane playerPlane = new Plane(Vector3.forward, transform.position);
            if (playerPlane.Raycast(_input.DashRay, out float distance))
            {
                Vector3 targetPoint = _input.DashRay.GetPoint(distance);
                Vector3 dashDir = (targetPoint - transform.position).normalized;

                _dashVelocity = dashDir * dashPower;
                _verticalVelocity = dashDir.y * (dashPower * 0.5f);
            }

            _currentSlow = 1f;
            _input.DashActive = false;
            dashActive = false;
            _input.DashFire = false;
            _input.DashConsumed = true;
            gravity = -60f;
            _maxCharge = false;
        }
    }
}