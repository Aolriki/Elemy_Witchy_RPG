using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ElementalAnimator : MonoBehaviour
{
    [Header("Base")]
    private RuntimeAnimatorController baseController;

    [Header("Element Overrides")]
    [SerializeField] private AnimatorOverrideController ignaOverride;
    [SerializeField] private AnimatorOverrideController floraOverride;
    [SerializeField] private AnimatorOverrideController aquaOverride;

    private Animator animator;
    private EElements lastElement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        baseController = animator.runtimeAnimatorController;
    }

    private void Update()
    {
        if (Player.instance == null) return;

        EElements current = Player.instance.currentElement;
        if (current == lastElement) return;

        lastElement = current;
        ApplyElement(current);
    }

    private void ApplyElement(EElements element)
    {
        // Salva todos os parâmetros antes de trocar
        AnimatorControllerParameter[] parameters = animator.parameters;
        var savedParams = new System.Collections.Generic.Dictionary<string, object>();

        foreach (var param in parameters)
        {
            savedParams[param.name] = param.type switch
            {
                AnimatorControllerParameterType.Bool => (object)animator.GetBool(param.name),
                AnimatorControllerParameterType.Int => animator.GetInteger(param.name),
                AnimatorControllerParameterType.Float => animator.GetFloat(param.name),
                _ => null
            };
        }

        // Troca o controller
        animator.runtimeAnimatorController = element switch
        {
            EElements.Igna => ignaOverride,
            EElements.Flora => floraOverride,
            EElements.Aqua => aquaOverride,
            _ => baseController
        };

        // Restaura os parâmetros no novo controller
        foreach (var param in parameters)
        {
            if (savedParams[param.name] == null) continue;

            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(param.name, (bool)savedParams[param.name]);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(param.name, (int)savedParams[param.name]);
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(param.name, (float)savedParams[param.name]);
                    break;
            }
        }
    }
}
