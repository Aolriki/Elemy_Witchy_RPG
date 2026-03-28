using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Core : MonoBehaviour, IControllable
{
    public StateMachine rootStateMachine;

    private List<Context> contexts = new();
    public Dictionary<EContextType, Context> contextDict = new Dictionary<EContextType, Context>();

    public State currState => rootStateMachine.currState;

    [Header("Inputs & Controller")]
    [HideInInspector] public PlayerController controller;

    [HideInInspector] public UnityEvent<InputAction.CallbackContext> OnMoveEvent = new();
    [HideInInspector] public UnityEvent<InputAction.CallbackContext> OnRunEvent = new();
    [HideInInspector] public UnityEvent<InputAction.CallbackContext> OnJumpEvent = new();
    [HideInInspector] public UnityEvent<InputAction.CallbackContext> OnAttackEvent = new();
    [HideInInspector] public UnityEvent<InputAction.CallbackContext> OnInteractEvent = new();

    protected virtual void Awake()
    {
        contexts = GetComponentsInChildren<Context>(true).ToList();

        foreach (Context ctx in contexts)
        {
            if(!contextDict.ContainsKey(ctx.GetContextType()))
                contextDict.Add(ctx.GetContextType(), ctx);
            else
                Debug.LogWarning("Core: Duplicate context type found: " + ctx.GetContextType());
        }

        SetupInstances();
    }

    protected void Update()
    {
        SelectState();
        rootStateMachine.currState?.DoBranch();
    }

    protected void FixedUpdate()
    {
        rootStateMachine.currState?.FixedDoBranch();
    }

    protected virtual void SelectState() { }

    public void Set(State newState, bool forceReset = false)
    {
        rootStateMachine.Set(newState, forceReset);
    }

    public void SetupInstances()
    {
        rootStateMachine = new StateMachine();

        State[] allChildStates = GetComponentsInChildren<State>();
        foreach (State state in allChildStates)
        {
            state.SetCore(this);
            state.Init();
        }
    }

    protected void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            List<State> states = rootStateMachine.GetActiveStateBranch();
            UnityEditor.Handles.Label(transform.position, "Active States: " + string.Join(">", states));
        }
#endif
    }

    #region IControllable Implementation

    public virtual void Possess(PlayerController playerController)
    {
        controller = playerController;
    }

    public virtual void OnMove(InputAction.CallbackContext context)
    {
        OnMoveEvent.Invoke(context);
    }

    public virtual void OnJump(InputAction.CallbackContext context)
    {
        OnJumpEvent?.Invoke(context);
    }

    public virtual void OnAttack(InputAction.CallbackContext context)
    {
        OnAttackEvent?.Invoke(context);
    }

    public virtual void OnInteract(InputAction.CallbackContext context)
    {
        OnInteractEvent?.Invoke(context);
    }

    public virtual void Unpossess()
    {
        controller = null;
    }

    #endregion
}
