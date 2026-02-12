using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private PlayerController _playerController;

    private InputManager _input;

    private Vector3 _targetCamPos;

    private Vector3 _mousePos;

    private Camera _cam;
    
    private Transform _camTransform;

    private Vector3 _playerPos;

    private Vector3 _camPos;
    
    private Vector3 _currentVelocity = Vector3.zero;

    private Vector3 _movementOffset;

    private float _maxLookDistance;

    private float _currentMaxDist;

    [SerializeField] private float baseSmoothSpeed = 0.66f;
    [SerializeField] private float baseZOffset = 25;
    [SerializeField] private float dashZOffset = 10;
    [SerializeField] private float jumpZOffset = 20;
    [SerializeField] private float mouseInfluence = 1;
    [SerializeField] private float baseLookDistance = .75f;
    [SerializeField] private float dashLookDist = 1.5f;
    [SerializeField] private float jumpXMovement = 1;
    [SerializeField] private float jumpYMovement = 1;
    [SerializeField] private float jumpOffsetWeight = 1.5f;

    private float _smoothSpeed;
    private float _zOffset;
    
    void Start()
    {
        
    }

    private void Awake()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _input = FindFirstObjectByType<InputManager>();
        _cam = GetComponent<Camera>();
        _camTransform = _cam.transform;
        _zOffset = baseZOffset;
        _smoothSpeed = baseSmoothSpeed;
    }
    void LateUpdate()
    {
        _mousePos = Mouse.current.position.ReadValue();
        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        if (_playerController == null) return;
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 mouseWorldPos = Vector3.zero;

        if (plane.Raycast(ray, out float distance))
        {
            mouseWorldPos = ray.GetPoint(distance);
        }
        _camPos = _camTransform.position;
        _playerPos = _playerController.transform.position;

        float targetLimit = _playerController.dashActive ? dashLookDist : baseLookDistance;
        
        _currentMaxDist = Mathf.Lerp(_currentMaxDist, targetLimit, 10f * Time.deltaTime);
        
        Vector3 lookDirection = mouseWorldPos - _playerPos;
        Vector3 cameraOffset = lookDirection * mouseInfluence;
        cameraOffset = Vector3.ClampMagnitude(cameraOffset, _currentMaxDist);
        
        _movementOffset.x = _input.Move.x * jumpXMovement;
        _movementOffset.y = _input.Move.y * jumpYMovement;
        
        if (_input.JumpPressed)
        {
            _movementOffset *= jumpOffsetWeight;
            _zOffset = jumpZOffset;
        }
        else
        {
            _zOffset = baseZOffset;
        }

        if (_input.DashActive)
        {
            dashZOffset = baseZOffset - ((_playerController.dashPower / _playerController.maxDashPower)*5);
            if (_playerController.maxCharge)
            {
                dashZOffset = baseZOffset - ((_playerController.dashPower / _playerController.maxDashPower)*2.5f);
                _smoothSpeed = 0.3f;
            }

            _zOffset = dashZOffset;
        }
        else
        {
            _smoothSpeed = baseSmoothSpeed;
        }
        
        _targetCamPos = _playerPos + cameraOffset + _movementOffset;
        
        _targetCamPos.z = _playerPos.z - _zOffset;

        
        _camTransform.position = Vector3.SmoothDamp(
            _camTransform.position, 
            _targetCamPos, 
            ref _currentVelocity, 
            _smoothSpeed 
        );
    }
}
