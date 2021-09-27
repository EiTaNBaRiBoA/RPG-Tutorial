using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.InputSystem;
using MLAPI.NetworkVariable;

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
    public GameplayInput inputActions;
    public CharacterController characterController;
    [Header("Falling")]

    [SerializeField] public LayerMask ground;
    public Transform groundCheck;
    private float fallSpeed = 0;
    #endregion




    private void Awake()
    {
        #region InputActions
        if (IsLocalPlayer)
        {
            inputActions = new GameplayInput();
            var rebinds = PlayerPrefs.GetString("Rebinds");
            if (!string.IsNullOrEmpty(rebinds))
                inputActions.Player.Movement.LoadBindingOverridesFromJson(rebinds);
            characterController = GetComponent<CharacterController>();
        }
        #endregion
    }
    public void PlayerMover(Vector2 movement)
    {
        characterController.Move(new Vector3(movement.x, 0, movement.y) * Time.fixedDeltaTime * 2f);
    }

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
            if (CheckIsGround())
            {
                fallSpeed = Mathf.Epsilon;
            }
            else
            {
                fallSpeed += Physics.gravity.y * Time.fixedDeltaTime;
                characterController.Move(new Vector3(0, fallSpeed * Time.deltaTime, 0));
            }
            if (inputActions.Player.Movement.IsPressed())
            {
                PlayerMover(inputActions.Player.Movement.ReadValue<Vector2>());
            }
            positionUpdate.Value = transform.position;

        }


    }

    void MovePlayer(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        PlayerMover(ctx.ReadValue<Vector2>());
    }
    void JumpPlayer(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (CheckIsGround())
        {
            fallSpeed = Mathf.Sqrt(50f * -2 * Physics.gravity.y);
            characterController.Move(new Vector3(0, fallSpeed * Time.deltaTime, 0));
        }
    }
    private bool CheckIsGround()
    {
        return Physics.CheckSphere(groundCheck.position, characterController.radius * 2, ground);
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Movement.performed += ctx => MovePlayer(ctx);
        inputActions.Player.Jump.performed += ctx => JumpPlayer(ctx);
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, characterController.radius * 2);
    }

}
