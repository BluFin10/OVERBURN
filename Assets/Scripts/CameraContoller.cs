using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraContoller : MonoBehaviour
{
    private PlayerController _playerController;

    private Vector3 _targetCamPos;

    private Vector3 _mousePos;

    private Camera _cam;
    
    private Transform _camTransform;

    private Vector3 _playerPos;

    private Vector3 _camPos;
    
    private Vector3 _currentVelocity = Vector3.zero;

    private float _maxLookDistance;

    private float _currentMaxDist;

    [SerializeField] private float smoothSpeed = 0.5f;
    [SerializeField] private float zOffset = 10;
    [SerializeField] private float mouseInfluence = 1;
    [SerializeField] private float baseLookDistance = .75f;
    [SerializeField] private float dashLookDist = 1.5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _cam = GetComponent<Camera>();
        _camTransform = _cam.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _mousePos = Mouse.current.position.ReadValue();
        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        // Safety check: Don't do anything if the player doesn't exist (e.g., they died)
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

        // 2. SMOOTH THE LIMIT: Transition the max look distance smoothly
        // This makes the 'reset' feel like a camera zoom rather than a glitch
        float targetLimit = _playerController.dashActive ? dashLookDist : baseLookDistance;
    
        // We use a higher speed (10f) for the distance reset so it feels snappy
        _currentMaxDist = Mathf.Lerp(_currentMaxDist, targetLimit, 10f * Time.deltaTime);

        // 3. Calculate and Clamp Offset
        Vector3 lookDirection = mouseWorldPos - _playerPos;
        Vector3 cameraOffset = lookDirection * mouseInfluence;
        cameraOffset = Vector3.ClampMagnitude(cameraOffset, _currentMaxDist);

        // 4. Set Target Position
        _targetCamPos = _playerPos + cameraOffset;
        _targetCamPos.z = _playerPos.z - zOffset;

        // 5. APPLY MOVEMENT: SmoothDamp is much better for high-speed tracking
        // Use smoothSpeed values between 0.05 (fast) and 0.3 (slow)
        _camTransform.position = Vector3.SmoothDamp(
            _camTransform.position, 
            _targetCamPos, 
            ref _currentVelocity, 
            smoothSpeed 
        );
        Debug.Log(_targetCamPos);
    }
}
