using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.TimeControl
{
    public struct AnimatorParameterStep:ITimeStep
    {
        public AnimatorParameterLayer.ParameterType type;
        public float value;
    }
[CreateAssetMenu(fileName = "AnimatorParameterLayer", menuName = "TimeControl/AnimatorParameterLayer")]
public class AnimatorParameterLayer :GenericLayer<AnimatorParameterStep>
{
    public enum ParameterType
    {
        Float,Int,Bool
    }
    [SerializeField]
    private ParameterType type;
    [SerializeField]
    private string parameterName;
    private Animator animator;
   
    public override void Record()
    {
        AnimatorParameterStep newStep=new AnimatorParameterStep();
        newStep.type=type;
        switch(type)
        {
            case ParameterType.Float:
            {
                newStep.value=animator.GetFloat(parameterName);
                break;
            }
            case ParameterType.Int:
            {
                newStep.value=animator.GetInteger(parameterName);
                break;
            }
            case ParameterType.Bool:
            {
                newStep.value=animator.GetBool(parameterName)?1:0;
                break;
            }
        }
        steps.Push(newStep);
    }
    protected override void Execute(AnimatorParameterStep result,bool playBack)
    {
        switch(result.type)
        {
            case ParameterType.Float:
            {
                animator.SetFloat(parameterName,result.value);
                break;
            }
            case ParameterType.Int:
            {
                animator.SetInteger(parameterName,(int)result.value);
                break;
            }
            case ParameterType.Bool:
            {
                animator.SetBool(parameterName,result.value==1);
                break;
            }
        }
    }
    public override void Init(int Capacity,CustomizedStore store)
    {
        base.Init(Capacity,store);
        animator=store.transform.GetComponent<Animator>();
    }
    
}
}