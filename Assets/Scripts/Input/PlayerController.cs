using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputManager _input;
    private CharacterController _controller;

    [Header("Movement")]
    [SerializeField] private float gravity = -50f;
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float baseJumpForce = 20f;
    [SerializeField] private float maxJumpForce = 35f;
    [SerializeField] private float jumpChargeSpeed = 20f;
    [SerializeField] private float fastFallMultiplier = 1.1f;
    [SerializeField] private float fastFallLimit = -50f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBuffer = 0.15f;
    [Header("Friction & Drag")]
    [SerializeField] private float groundFriction = 5f; 
    [SerializeField] private float airFriction = 2f;         
    [SerializeField] private float horizontalAcceleration = 3f; 
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
    public bool maxCharge;
    private bool _jumpConsumed;
    private bool _isFastFalling;
    private float _timeSinceGrounded;
    private float _jumpBufferCounter;

    private Collider _ignoredPlatform;
    
    void Start()
    {
        _input = InputManager.instance;
        _controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        _isGrounded = _controller.isGrounded;
        
        if (_ignoredPlatform != null)
        {
            if (!_controller.bounds.Intersects(_ignoredPlatform.bounds))
            {
                if (_verticalVelocity <= 0)
                {
                    Physics.IgnoreCollision(_controller, _ignoredPlatform, false);
                    _ignoredPlatform = null;
                }
            }
            else
            {
                Physics.IgnoreCollision(_controller,_ignoredPlatform,true);
            }
        }

        HandleMovement(Time.deltaTime);
        HandleDash(Time.deltaTime);
        
        float activeDrag = _isGrounded ? groundFriction : airFriction;
        
        Vector3 targetInputVelocity = _input.Move.x * transform.right * moveSpeed;
        
        _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetInputVelocity, horizontalAcceleration * Time.deltaTime);
        
        _dashVelocity = Vector3.Lerp(_dashVelocity, Vector3.zero, activeDrag * Time.deltaTime);
        
        Vector3 finalMotion = (_horizontalVelocity + _dashVelocity + (Vector3.up * _verticalVelocity));
        _controller.Move(finalMotion * (_currentSlow * Time.deltaTime));
    }

    private void HandleMovement(float delta)
    {
        if (_isGrounded)
        {
            _timeSinceGrounded = 0f;
            if (_input.Move.y < -0.1f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, _controller.height / 2 + 0.1f))
                {
                    if (hit.collider.CompareTag("Platform"))
                    {
                        Physics.IgnoreCollision(_controller, hit.collider, true);
                        _ignoredPlatform = hit.collider;
                        return; // Skip the rest, don't reset to -5f
                    }
                }
            }
            _jumpConsumed = false;
                _verticalVelocity = -5f; 
        }
        else
        {
            if (_input.JumpFire)
            {
                _jumpBufferCounter = jumpBuffer;
                _input.JumpFire = false;
            }

            _jumpBufferCounter -= Time.deltaTime;
            _timeSinceGrounded += Time.deltaTime;
            _jumpPower = baseJumpForce;
            Debug.Log(_timeSinceGrounded);
        }

        if (_timeSinceGrounded < coyoteTime)
        {
            if (_input.JumpPressed && !_jumpConsumed)
            {
                if (_jumpPower < baseJumpForce)
                {
                    _jumpPower = baseJumpForce;
                }

                if (_jumpPower <= maxJumpForce)
                {
                    _jumpPower += jumpChargeSpeed * delta;
                }
                else
                {
                    _jumpPower = maxJumpForce;
                }

                float powerRatio = (_jumpPower - baseJumpForce) / (maxJumpForce - baseJumpForce);
                _currentSlow = Mathf.Lerp(1.0f, .05f, powerRatio);
            }

            if (_input.JumpFire && !_jumpConsumed || _jumpBufferCounter >= 0)
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
            if (!_input.DashActive)
            {
                _currentSlow = 1;
            } 
            _jumpConsumed = true;
            _input.JumpFire = false;
            _input.JumpPressed = false;
        }

        _isFastFalling = _input.Move.y < -0.1f;
        float gravityMultiplier = _isFastFalling ? fastFallMultiplier : 1f;
        float terminalVelocity = _isFastFalling ? fastFallLimit : -30f;
    
        if (_verticalVelocity > terminalVelocity)
        {
            _verticalVelocity += gravity * gravityMultiplier * delta;
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
            maxCharge = false;
        }
        
        if (_input.DashActive && !_input.DashConsumed)
        {
            dashActive = true;
            if (!maxCharge)
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
                    if (_currentHoldTime >= maxPowerHoldTime) maxCharge = true;
                }
            }
            else
            {
                dashPower -= chargeRate * delta;
                if (dashPower < minDashPower) dashPower = minDashPower;
            }

            float powerRatio = (dashPower - minDashPower) / (maxDashPower - minDashPower);
            _currentSlow = Mathf.Lerp(1.0f, 0.25f, powerRatio);
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
            maxCharge = false;
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Platform"))
        {
            if (_verticalVelocity > 0 || hit.point.y > transform.position.y)
            {
                Physics.IgnoreCollision(_controller, hit.collider, true);
                _ignoredPlatform = hit.collider;
            }
            else
            {
                Physics.IgnoreCollision(_controller, hit.collider, false);
                _ignoredPlatform = null;
            }
            if (_isFastFalling)
            {
                Physics.IgnoreCollision(_controller, hit.collider, true);
                _ignoredPlatform = hit.collider;
            }
        }
    }
}