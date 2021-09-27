using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.InputSystem;

public class PlayerOnline : NetworkBehaviour
{
    //bool isPlayerInputInit = false; to local palyer
    public GameplayInput inputActions;
    CharacterController characterController;

    private void Awake()
    {
        //if(IsLocalPlayer)
        //Input Actions init
        #region InputActions
        inputActions = new GameplayInput();
        var rebinds = PlayerPrefs.GetString("Rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            inputActions.Player.Movement.LoadBindingOverridesFromJson(rebinds);


        characterController = GetComponent<CharacterController>();
        #endregion
    }
    public void PlayerMover(Vector2 movement)
    {
        characterController.Move(new Vector3(movement.x, 0, movement.y) * Time.fixedDeltaTime * 2f);
    }

    // Update is called once per frame
    void Update()
    {


        if (inputActions.Player.Movement.IsPressed())
        {
            PlayerMover(inputActions.Player.Movement.ReadValue<Vector2>());
        }
        if (IsLocalPlayer)
        {

        }
    }

    void MovePlayer(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        PlayerMover(ctx.ReadValue<Vector2>());
    }
    private bool CheckIsGround()
    {
        return false;
        //return Physics.CheckSphere(characterController.transform.position, characterController.radius, ground);
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Movement.performed += ctx => MovePlayer(ctx);
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
