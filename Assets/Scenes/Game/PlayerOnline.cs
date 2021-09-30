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

    private int jumpHeight = 20;
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
            movement.y = Gravity();
            characterController.Move(movement);
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
        characterController.enabled = true;
        playerCamera.gameObject.SetActive(true);
        inputActions.Player.Movement.performed += ctx => MovePlayer(ctx);
        inputActions.Player.Movement.canceled += (ctx) =>
        {
            movement.x = 0;
            movement.z = 0;
        };
        inputActions.Player.Jump.performed += (ctx) => { shouldJump = true; };
        inputActions.Enable();
    }

    #endregion


    #region Movement and gravity handling
    void MovePlayer(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        PlayerMover(ctx.ReadValue<Vector2>());
    }
    public void PlayerMover(Vector2 movement)
    {
        this.movement.x = transform.right.magnitude * movement.x * Time.deltaTime * 15f;
        this.movement.z = transform.forward.magnitude * movement.y * Time.deltaTime * 15f;
    }
    private float Gravity()
    {
        if (CheckIsGround() && shouldJump)
        {
            return Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight) * Time.deltaTime * 100;
        }
        else if (CheckIsGround() && !shouldJump)
        {
            return 0;
        }
        shouldJump = false;
        return (this.movement.y + Physics.gravity.y) * Time.deltaTime;
    }
    private bool CheckIsGround()
    {
        return Physics.CheckSphere(groundCheck.position, characterController.radius * 2, ground);
    }


    #endregion
}
