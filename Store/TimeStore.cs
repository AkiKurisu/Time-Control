using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Kurisu.TimeControl
{

public class TimeStore : MonoBehaviour
{
    [SerializeField,LabelText("锁定回溯器")]
    protected bool locked;
    /// <summary>
    /// 回溯栈
    /// </summary>
    protected Stack<TransformStep> steps;
    
    protected virtual void Start() {
        if(TimeController.IsInitialized)
        {
            TimeController.Instance.Add(this);
        }
        steps=new Stack<TransformStep>(TimeController.Instance.Capacity);
        
    }
    protected virtual void OnDestroy() {
        if(TimeController.IsInitialized)
        {
            TimeController.Instance.Remove(this);
            TimeController.Instance.OnRecallEndEvent-=TimeStoreOver;
            TimeController.Instance.OnRecallEvent-=RecallTick;
            TimeController.Instance.OnRecordEvent-=RecordTick;
            
        }
        steps.Clear();
    }
    /// <summary>
    /// 锁定回溯器
    /// </summary>
    /// <param name="state"></param>
    public void LockTimeStore(bool state)
    {
        this.locked=state;
    }
    /// <summary>
    /// 开始记录
    /// </summary>
    public virtual void Record()
    {
        
        steps.Clear();
        if(locked)
            return;
        TimeController.Instance.OnRecordEvent+=RecordTick;
    }
    /// <summary>
    /// 开始回溯
    /// </summary>
    public virtual void Recall()
    {
        TimeController.Instance.OnRecordEvent-=RecordTick;
        if(locked)
            return;
        TimeController.Instance.OnRecallEvent+=RecallTick;
        TimeController.Instance.OnRecallEndEvent+=TimeStoreOver;
    }
    /// <summary>
    /// 终止回溯和记录
    /// </summary>
    public virtual void TimeStoreOver()
    {
        steps.Clear();
        TimeController.Instance.OnRecallEndEvent-=TimeStoreOver;
        TimeController.Instance.OnRecallEvent-=RecallTick;
        TimeController.Instance.OnRecordEvent-=RecordTick;
    }
    /// <summary>
    /// 强制关闭回溯器
    /// </summary>
    public virtual void ShutDown()
    {
        TimeStoreOver();
    }
    /// <summary>
    /// 记录时刻
    /// </summary>
    public virtual void RecordTick()
    {
        
        TransformStep newStep=new TransformStep();
        newStep.position=transform.position;
        newStep.rotation=transform.rotation; 
        steps.Push(newStep);
               
    }
    /// <summary>
    /// 回溯时刻
    /// </summary>
    public virtual void RecallTick()
    {
        if(steps.Count==0)
            return;
        var oldStep=steps.Pop();
        transform.position=oldStep.position;
        transform.rotation=oldStep.rotation;
               
    }
                
            
}          
}
