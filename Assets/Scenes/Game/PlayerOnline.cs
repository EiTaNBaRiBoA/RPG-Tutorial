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
    });
    #endregion

    #region  CurrentPlayer
    [SerializeField] public GameplayInput inputActions;
    public CharacterController characterController;
    public Camera playerCamera;
    [Header("Falling")]

    [SerializeField] public LayerMask ground;
    public Transform groundCheck;
    private float fallSpeed = 0;
    bool initPlayer = false;
    #endregion


    // Update is called once per frame
    void Update()
    {

        //updating for non players
        if (!IsLocalPlayer)
        {
            transform.position = positionUpdate.Value;
        }
        //updating for current player
        if (IsLocalPlayer)
        {
            if (inputActions == null && !initPlayer)
            {
                InitInputActions();

            }
            if (CheckIsGround())
            {
                fallSpeed = Mathf.Epsilon;
            }
            else
            {
                fallSpeed += Physics.gravity.y * Time.deltaTime;
                characterController.Move(new Vector3(0, fallSpeed, 0));
            }
            if (inputActions.Player.Movement.IsPressed())
            {
                PlayerMover(inputActions.Player.Movement.ReadValue<Vector2>());
            }
            positionUpdate.Value = transform.position;

        }


    }

    private void InitInputActions()
    {
        if (!initPlayer)
        {
            Debug.Log("Initplayer");
            #region InputActions
            inputActions = new GameplayInput();
            inputActions.Disable();
            var rebinds = PlayerPrefs.GetString("Rebinds");
            if (!string.IsNullOrEmpty(rebinds))
                inputActions.Player.Movement.LoadBindingOverridesFromJson(rebinds);
            characterController = GetComponent<CharacterController>();
            characterController.enabled = true;
            playerCamera.enabled = true;
            inputActions.Player.Movement.performed += ctx => MovePlayer(ctx);
            inputActions.Player.Jump.performed += ctx => JumpPlayer(ctx);
            inputActions.Enable();
            initPlayer = true;
        }
        #endregion
    }

    void MovePlayer(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        PlayerMover(ctx.ReadValue<Vector2>());
    }
    public void PlayerMover(Vector2 movement)
    {
        characterController.Move(new Vector3(movement.x, 0, movement.y) * Time.deltaTime * 15f);
    }
    void JumpPlayer(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (CheckIsGround())
        {
            fallSpeed = Mathf.Sqrt(-2 * Physics.gravity.y);
            characterController.Move(new Vector3(0, fallSpeed * Time.deltaTime / 5, 0));
        }
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
