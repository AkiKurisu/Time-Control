using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.TimeControl
{
    /// <summary>
    /// 单个动画层的记录数据
    /// </summary>
    [System.Serializable]
    public struct AnimatorLayerStep :ITimeStep
    {
        public int stateHash;//当前状态的哈希值
        public float stateTime;//当前状态的时刻
        public int transitionHash;//当前转换的哈希值
        public float transitionLength;//当前转换的长度
        public float transitionTime;//当前转换的时刻
        public int transitionDestination;//当前转换的目标（用于正向回溯）
    }
[CreateAssetMenu(fileName = "AnimatorLayerLayer", menuName = "TimeControl/AnimatorLayerLayer")]
public class AnimatorLayerLayer : GenericLayer<AnimatorLayerStep>
{
    
    [SerializeField]
    private int layerIndex=0;
    private Animator animator;


    public override void Record()
    {
        AnimatorStateInfo nextStateInfo=animator.GetNextAnimatorStateInfo(layerIndex);
        AnimatorStateInfo stateInfo=animator.GetCurrentAnimatorStateInfo(layerIndex);
        AnimatorTransitionInfo transitionInfo=animator.GetAnimatorTransitionInfo(layerIndex);
        AnimatorLayerStep newStep=new AnimatorLayerStep();
        newStep.stateHash=stateInfo.fullPathHash;
        newStep.stateTime=stateInfo.normalizedTime;
        if(transitionInfo.fullPathHash!=0)//存在过度动画
        {
            newStep.transitionHash=transitionInfo.fullPathHash;//记录过度
            newStep.transitionLength=transitionInfo.duration;
            newStep.transitionTime=transitionInfo.normalizedTime;
            newStep.transitionDestination=nextStateInfo.fullPathHash;
        }    
        steps.Push(newStep);    
    }
   
    protected override void Execute(AnimatorLayerStep result,bool playBack)
    {
        AnimatorStateInfo stateInfo=animator.GetCurrentAnimatorStateInfo(layerIndex);
        AnimatorStateInfo nextStateInfo=animator.GetNextAnimatorStateInfo(layerIndex);
        if(playBack)
        {
            if(stateInfo.fullPathHash!=result.stateHash)//当前state与目标state不同
            {
                animator.CrossFade(result.stateHash,result.transitionLength,layerIndex,1,1-result.transitionTime);     
            }
            else
                animator.Play(result.stateHash,layerIndex,result.stateTime);
        }
        else
        {
            if(result.transitionDestination!=0)
            {
                animator.CrossFade(result.transitionDestination,result.transitionLength,layerIndex,0,result.transitionTime); 
            }
            else
                animator.Play(result.stateHash,layerIndex,result.stateTime);
            
        }
    }
    
    public override void Init(int Capacity,CustomizedStore store)
    {
        base.Init(Capacity,store);
        animator=store.transform.GetComponent<Animator>();  
    }
    
 
}
}
