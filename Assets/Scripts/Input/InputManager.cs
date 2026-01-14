using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private Controls _controls;
    
    // make properties for our controls
    public Vector2 Move { get; private set; }
    
    public Vector3 MousePos { get; set; }

    public Ray DashRay { get;  set; }
    public InputAction RestartAction { get; private set; }
    public InputAction JumpAction { get; private set; }
    public InputAction DashAction { get; private set; }
    public InputAction MenuAction { get; private set; }
    public InputAction InteractAction { get; private set; }
    
    public bool RestartActive { get; private set; }
    public bool JumpPressed { get; set; }
    public bool DashActive { get; private set; }
    public bool DashConsumed { get; set; }
    public bool MenuActive { get; private set; }
    public bool InteractActive { get; private set; }

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        _controls = new Controls();
        _controls.Enable();
        
        // initialize InputActions
        RestartAction = _controls.GameplayLoco.Restart;
        DashAction = _controls.GameplayLoco.Dash;
        MenuAction = _controls.GameplayLoco.Menu;
        JumpAction = _controls.GameplayLoco.Jump;
        InteractAction = _controls.GameplayLoco.Interact;
        MousePos = Mouse.current.position.ReadValue();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controls.GameplayLoco.Jump.performed += context => JumpPressed = true;
        _controls.GameplayLoco.Jump.canceled += context => JumpPressed = false;
        
        _controls.GameplayLoco.Dash.started += context => DashActive = true;
        _controls.GameplayLoco.Dash.canceled += context => DashActive = false;
        
        _controls.GameplayLoco.Restart.performed += context => RestartActive = true;
        _controls.GameplayLoco.Restart.canceled += context => RestartActive = false;
        
        _controls.GameplayLoco.Interact.performed += context => InteractActive = true;
        _controls.GameplayLoco.Interact.canceled += context => InteractActive = false;
        
        _controls.GameplayLoco.Menu.performed += context => MenuActive = true;
        _controls.GameplayLoco.Menu.canceled += context => MenuActive = false;

        _controls.GameplayLoco.Move.performed += context => Move = context.ReadValue<Vector2>();
        _controls.GameplayLoco.Move.canceled += context => Move = Vector2.zero;
    }

   
}
