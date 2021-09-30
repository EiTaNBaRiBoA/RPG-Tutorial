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
    #region UpdatingVisualsToAllPlayers
    public NetworkVariable<Vector3> positionUpdate = new NetworkVariable<Vector3>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });//This will update the position of each gameobject on the map without moving the player gameobject this is a network value and it changes through the network
    // i made it so only the owner can change it and everyone can see it.
    #endregion

    #region  CurrentPlayer
    [SerializeField] public GameplayInput inputActions;
    public CharacterController characterController;
    public Camera playerCamera; // the camera is disable on default so players camera won't interfere with each other.
    // i enable it on the initplayer method.
    [Header("Falling and moving")]
    private bool isGround = false;
    private int jumpHeight = 5;
    Vector3 movement = Vector3.zero;

    [SerializeField] public LayerMask ground; // checking for ground
    public Transform groundCheck; // the transform position of the ground check
    bool initPlayer = false;
    #endregion


    // Update is called once per frame
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
            if (inputActions == null && !initPlayer)
            {
                InitInputActions();
                initPlayer = true;

            }
            isGround = CheckIsGround();
            if (isGround) // checks if the jump key is being pressed
            {
                movement.y = JumpGravity();
            }
            else if (!isGround)
            {
                movement.y += JumpGravity();
            }


            Debug.Log(movement);
            characterController.Move(movement);
            positionUpdate.Value = transform.position;

        }


    }

    /// <summary>
    /// initializing input actions for one time only
    /// </summary>
    private void InitInputActions()
    {
        #region InputActions
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
        inputActions.Player.Jump.performed += ctx => JumpGravity();
        inputActions.Enable();
        #endregion
    }

    void MovePlayer(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        PlayerMover(ctx.ReadValue<Vector2>());
    }
    public void PlayerMover(Vector2 movement)
    {
        this.movement.x = transform.right.magnitude * movement.x * Time.deltaTime * 15f;
        this.movement.z = transform.forward.magnitude * movement.y * Time.deltaTime * 15f;
    }
    private float JumpGravity()
    {
        if (isGround && inputActions.Player.Jump.inProgress)
        {
            isGround = false;
            return Mathf.Sqrt(-2 * Physics.gravity.y * 2) * Time.deltaTime * jumpHeight;

        }
        else if (isGround)
        {
            this.movement.y = 0;
        }
        return Physics.gravity.y * Time.deltaTime * 0.1f;
    }
    private bool CheckIsGround()
    {
        return Physics.CheckSphere(groundCheck.position, characterController.radius * 2, ground);
    }

    private void OnDrawGizmos()
    {
        if (IsLocalPlayer)
        {
            Gizmos.DrawSphere(groundCheck.position, characterController.radius * 2);
        }
    }

}
