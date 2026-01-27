using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraContoller : MonoBehaviour
{
    private PlayerController _playerController;

    private Vector3 _targetCamPos;

    private Vector3 _mousePos;

    private Camera _cam;

    private Vector3 _playerPos;
    
    private Vector3 _currentVelocity = Vector3.zero;

    [SerializeField] private float smoothSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _cam = GetComponent<Camera>();
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
        _playerPos = _playerController.transform.position;
        _targetCamPos = _mousePos - _playerPos;
        transform.position = Vector3.SmoothDamp(transform.position,_targetCamPos,ref _currentVelocity,smoothSpeed);
        Debug.Log(_targetCamPos);
    }
}
