using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kurisu.TimeControl
{
/// <summary>
/// 演示用的动画记录回溯器
/// </summary>
public class RecordAnimStore : TimeStore
{

   //更简单的动画回溯方式
    private float animTimer;
    private Animator animator;
    protected override void Start() {
        base.Start();
        animator=GetComponent<Animator>();
    }
    protected override void OnDestroy() {
        base.OnDestroy();
    }
    
    /// <summary>
    /// 开始记录
    /// </summary>
    public override void Record()
    {
        base.Record();
        animator.StartRecording(TimeController.Instance.Capacity);

    }
    public override void Recall()
    {
        base.Recall();
        animTimer=0;
        animator.StopRecording();
        animator.StartPlayback();
        animator.playbackTime=animator.recorderStopTime;
        
    }
   
    /// <summary>
    /// 终止回溯和记录
    /// </summary>
    public override void TimeStoreOver()
    {
        base.TimeStoreOver();
        animator.StopPlayback();
    }
    public override void RecallTick()
    {
        base.RecallTick();
        animTimer=animator.playbackTime;
        animTimer-=Time.fixedDeltaTime;
        if(animTimer<animator.recorderStartTime)
            return;
        animator.playbackTime=animTimer;
               
    }

            
           
}
}