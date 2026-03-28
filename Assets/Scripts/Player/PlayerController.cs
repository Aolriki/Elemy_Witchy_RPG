using UnityEngine;
using UnityEngine.InputSystem;

public interface IControllable
{
    void Possess(PlayerController playerController);
    void OnMove(InputAction.CallbackContext context);
    void OnJump(InputAction.CallbackContext context);
    void OnAttack(InputAction.CallbackContext context);
    void OnInteract(InputAction.CallbackContext context);
    void Unpossess();
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public PlayerInput playerInput;
    public IControllable currentControlledBeing;

    public void Init()
    {
        Possess();
    }

    public void Possess()
    {
        currentControlledBeing.Possess(this);
        AddInputBindings();
    }

    public void Unpossess()
    {
        RemoveInputBindings();
        currentControlledBeing.Unpossess();
    }

    #region Inputs

    private void OnEnable()
    {
        EnableInputs();
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.OnDialogueStart += DisableInputs;
            DialogueManager.instance.OnDialogueEnd += EnableInputs;
        }
    }

    private void OnDisable()
    {
        DisableInputs();
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.OnDialogueStart -= DisableInputs;
            DialogueManager.instance.OnDialogueEnd -= EnableInputs;
        }
    }
    public void EnableInputs()
    {
        playerInput.enabled = true;
    }

    public void DisableInputs()
    {
        playerInput.enabled = false;

        // Zera o movimento para o personagem não continuar andando
        if (currentControlledBeing != null)
        {
            var fakeContext = new InputAction.CallbackContext(); // context vazio = input zerado
            currentControlledBeing.OnMove(fakeContext);
        }
    }

    #endregion

    #region Bindings

    public void AddInputBindings()
    {
        if (playerInput == null) return;
        if (currentControlledBeing == null) return;

        var move = playerInput.actions["Move"];
        move.performed += currentControlledBeing.OnMove;
        move.canceled += currentControlledBeing.OnMove;

        var jump = playerInput.actions["Jump"];
        jump.performed += currentControlledBeing.OnJump;
        jump.canceled += currentControlledBeing.OnJump;

        var attack = playerInput.actions["Attack"];
        attack.performed += currentControlledBeing.OnAttack;
        attack.canceled += currentControlledBeing.OnAttack;

        var interact = playerInput.actions["Interact"];
        interact.performed += currentControlledBeing.OnInteract;
        interact.canceled += currentControlledBeing.OnInteract;
    }

    public void RemoveInputBindings()
    {
        if (playerInput == null) return;
        if (currentControlledBeing == null) return;

        var move = playerInput.actions["Move"];
        move.performed -= currentControlledBeing.OnMove;
        move.canceled -= currentControlledBeing.OnMove;

        var jump = playerInput.actions["Jump"];
        jump.performed -= currentControlledBeing.OnJump;
        jump.canceled -= currentControlledBeing.OnJump;

        var attack = playerInput.actions["Attack"];
        attack.performed -= currentControlledBeing.OnAttack;
        attack.canceled -= currentControlledBeing.OnAttack;

        var interact = playerInput.actions["Interact"];
        interact.performed -= currentControlledBeing.OnInteract;
        interact.canceled -= currentControlledBeing.OnInteract;
    }



    #endregion
}
