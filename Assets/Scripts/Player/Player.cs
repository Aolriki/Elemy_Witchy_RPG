using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Core
{
    public static Player instance;

    [Header("ElemyState")]
    public EElements currentElement;
    public float elemyDuration;
    private Coroutine elemyTimerCoroutine;
    public float ElementTimeRemaining { get; private set; }

    [Header("States")]
    public FallState fallState;
    public TakeDamageState damageState;
    public SimpleAnimationState deathState;

    [Header("References")]
    public PlayerInteraction playerInteraction;
    public Transform playerDialoguePosition;
    public Health health;

    [Header("Inputs")]
    [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
    public bool SnapInput = true;

    [Tooltip("Minimum input required before a up or down is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
    public float VerticalDeadZoneThreshold = 0.1f;
    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
    public float HorizontalDeadZoneThreshold = 0.1f;

    [Header("Contexts")]
    [HideInInspector] public PhysicsContext PhysicsContext;
    [HideInInspector] public CombatContext CombatContext;

    protected override void Awake()
    {
        base.Awake();

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (contextDict.TryGetValue(EContextType.Physics, out var physicsContext))
        {
            PhysicsContext = (PhysicsContext)physicsContext;
        }
        else
        {
            Debug.LogError("Missing PhysicsContext in core context dictionary.");
            enabled = false;
        }

        if (contextDict.TryGetValue(EContextType.Combat, out var attackContext))
        {
            CombatContext = (CombatContext)attackContext;
        }
    }

    private void Start()
    {
        if (PlayerStatus.Instance.IsReady)
            InitStats();
        else
            PlayerStatus.Instance.onStatsChanged += InitStats;

        rootStateMachine.Set(fallState);
    }

    private void InitStats()
    {
        PlayerStatus.Instance.onStatsChanged -= InitStats;
        elemyDuration = PlayerStatus.Instance.ElemyTimer;
        PlayerStatus.Instance.onStatsChanged += UpdateElemyStats;
    }

    private void UpdateElemyStats()
    {
        elemyDuration = PlayerStatus.Instance.ElemyTimer;
    }

    public void SetElemy(EElements element)
    {
        if (elemyTimerCoroutine != null)
            StopCoroutine(elemyTimerCoroutine);

        if (element != EElements.None)
            elemyTimerCoroutine = StartCoroutine(ElemyTimer());
    }

    private IEnumerator ElemyTimer()
    {
        float elapsed = 0f;
        while (elapsed < elemyDuration)
        {
            elapsed += Time.deltaTime;
            ElementTimeRemaining = elemyDuration - elapsed;
            yield return null;
        }

        ElementTimeRemaining = 0f;
        currentElement = EElements.None;
        elemyTimerCoroutine = null;
        Debug.Log("Element expired — back to None.");
    }

    #region Inputs

    public override void OnMove(InputAction.CallbackContext context)
    {
        base.OnMove(context);

        if (context.performed)
        {
            Vector2 Move = context.ReadValue<Vector2>().normalized;

            if (SnapInput)
            {
                Move.x = Mathf.Abs(Move.x) < HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(Move.x);
                Move.y = Mathf.Abs(Move.y) < VerticalDeadZoneThreshold ? 0 : Mathf.Sign(Move.y);
            }

            PhysicsContext.movementDirection = Move;
        }

        if (context.canceled)
        {
            PhysicsContext.movementDirection = Vector2.zero;
        }
    }

    public override void OnInteract(InputAction.CallbackContext context)
    {
        base.OnInteract(context);

        if (context.performed)
        {
            playerInteraction.Interact();
        }
    }

    public override void OnJump(InputAction.CallbackContext context)
    {
        base.OnJump(context);

        if (context.performed)
            PhysicsContext.orderingJump = true;
        else if (context.canceled)
            PhysicsContext.orderingJump = false;

        if (context.performed) StartCoroutine(JumpEventToBool());
    }

    public IEnumerator JumpEventToBool()
    {
        PhysicsContext.onJump = true;
        yield return new WaitForEndOfFrame();
        PhysicsContext.onJump = false;
    }

    public override void OnAttack(InputAction.CallbackContext context)
    {
        base.OnAttack(context);
        AudioManager.Instance.PlayEffect(SFXID.Attack);

        if (context.performed) StartCoroutine(AttackEventToBool());
    }

    public IEnumerator AttackEventToBool()
    {
        CombatContext.onAttack = true;
        CombatContext.currentElement = currentElement;
        yield return new WaitForEndOfFrame();
        CombatContext.onAttack = false;
    }

    #endregion
}