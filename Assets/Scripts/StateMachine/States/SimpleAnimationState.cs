using UnityEngine;

public enum AnimationParameterType
{
    None,
    Bool,
    Trigger,
}

public class SimpleAnimationState : State
{
    public string animationParameterName;
    public AnimationParameterType animationParameterType;

    private GraphicContext GraphicContext;
    public override void Init()
    {
        if (core.contextDict.TryGetValue(EContextType.Graphic, out var graphicContext))
        {
            GraphicContext = (GraphicContext)graphicContext;
        }
        else
        {
            Debug.LogError("Simple Animation State: Missing GraphicContext in core context dictionary.");
            enabled = false;
        }
    }

    public override void Enter()
    {
        if (animationParameterName == null) return;

        if (animationParameterType == AnimationParameterType.Bool)
            GraphicContext.animator.SetBool(animationParameterName, true);

        if (animationParameterType == AnimationParameterType.Trigger)
            GraphicContext.animator.SetTrigger(animationParameterName);
    }

    public override void InitializeSubState()
    {
    }

    protected override void Do()
    {
    }

    protected override void Exit()
    {
        if (animationParameterName == null) return;

        if(animationParameterType  == AnimationParameterType.Bool)
            GraphicContext.animator.SetBool(animationParameterName, false);

        if (animationParameterType == AnimationParameterType.Trigger)
            GraphicContext.animator.ResetTrigger(animationParameterName);
    }

    protected override void FixedDo()
    {
    }

    protected override void SelectState()
    {
    }
}
