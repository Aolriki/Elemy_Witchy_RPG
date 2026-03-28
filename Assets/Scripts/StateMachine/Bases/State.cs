using UnityEngine;

public abstract class State : MonoBehaviour
{
    public bool isComplete { get; protected set; }

    protected float startTime;

    public float time => Time.time - startTime;

    //blackboard variables
    protected Core core;

    public StateMachine machine;

    protected StateMachine parent;

    public StateMachine Parent {  get { return parent; } }

    public State currSubState => machine.currState;

    public StateMachine rootStateMachine => core.rootStateMachine;

    public abstract void Init();

    public abstract void Enter(); 

    protected abstract void Do();

    protected abstract void FixedDo();

    protected abstract void SelectState();

    protected abstract void Exit();

    public abstract void InitializeSubState();

    public void DoBranch()
    {
        Do();
        currSubState?.DoBranch();
    }

    public void FixedDoBranch()
    {
        FixedDo();
        currSubState?.FixedDoBranch();
    }

    public void ExitStates()
    {
        currSubState?.ExitStates();
        machine.currState = null;
        Exit();
    }

    public void SetCore(Core _core)
    {
        machine = new StateMachine();
        core = _core;
    }

    public void Initialise(StateMachine _parent)
    {
        parent = _parent;
        isComplete = false;
        startTime = Time.time;
    }
}
