using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.InputSystem;
using MLAPI.NetworkVariable;



/// <summary>
/// Awake and onenable and ondisable and start don't work on mlapi
/// </summary>
public class PlayerOnline : NetworkBehaviour
{
    #region Network Variables
    public NetworkVariable<Vector3> positionUpdate = new NetworkVariable<Vector3>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });//This will update the position of each gameobject on the map without moving the player gameobject this is a network value and it changes through the network
    // i made it so only the owner can change it and everyone can see it.
    #endregion

    #region  Player Server Set up
    private bool initPlayer = false;
    [SerializeField] public GameplayInput inputActions;
    public CharacterController characterController;
    public Camera playerCamera; // the camera is disable on default so players camera won't interfere with each other.
                                // i enable it on the initplayer method.
    #endregion



    #region Movement
    [Header("Player's movement and jump")]


    private float speed = 15f;
    private float jumpHeight = 30f;
    private bool shouldJump = false;
    [SerializeField] public LayerMask ground; // checking for ground
    public Transform groundCheck; // the transform position of the ground check
    Vector3 movement = Vector3.zero;

    #endregion



    void Update()
    {
        //Checking if the person accessing the script is not the local player and updating the position for that specific gameobject
        if (!IsLocalPlayer)
        {
            transform.position = positionUpdate.Value;
        }




        //The current player movement
        if (IsLocalPlayer)
        {
            //init player setup
            if (inputActions == null && !initPlayer)
            {
                InitInputActions();
                initPlayer = true;

            }
            Gravity();
            movement.x = inputActions.Player.Movement.ReadValue<Vector2>().x * speed;
            movement.z = inputActions.Player.Movement.ReadValue<Vector2>().y * speed;
            PlayerMovement();
            positionUpdate.Value = transform.position;

        }
    }


    #region init inputs
    /// <summary>
    /// initializing input actions for one time only
    /// </summary>
    private void InitInputActions()
    {
        inputActions = new GameplayInput();
        inputActions.Disable();
        var rebinds = PlayerPrefs.GetString("Rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            inputActions.Player.Movement.LoadBindingOverridesFromJson(rebinds);
        characterController = GetComponent<CharacterController>();
        inputActions.Player.Jump.performed += (ctx) => { if (CheckIsGround()) shouldJump = true; };
        characterController.enabled = true;
        playerCamera.gameObject.SetActive(true);
        inputActions.Enable();
    }

    #endregion


    #region Movement and gravity handling
    void PlayerMovement()
    {
        characterController.Move(movement * Time.deltaTime);
    }
    private void Gravity()
    {
        if (CheckIsGround() && shouldJump)
        {
            shouldJump = false;
            movement.y = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight);
        }
        else if (CheckIsGround() && movement.y < -1)
        {
            movement.y = 0;
        }
        movement.y += Physics.gravity.y * Time.deltaTime * 5f;
    }

    //Check if there is a ground
    private bool CheckIsGround()
    {
        return Physics.CheckSphere(groundCheck.position, characterController.radius * 2, ground);
    }


    #endregion
}