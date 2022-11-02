using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Kurisu.TimeControl.Command;
using Kurisu.TimeControl.Sync;
namespace Kurisu.TimeControl
{
    /// <summary>
    /// 接口：可记录数据接口
    /// </summary>
    public interface ITimeStep
{

}
    /// <summary>
    /// 泛型记录层
    /// </summary>
    /// <typeparam name="T"></typeparam>
public abstract class GenericLayer<T>  :BaseLayer where T:struct,ITimeStep
{
    protected Stack<T> steps;
    public GenericTimeStepCommand<T> OutCommand=null;
    public event Action<T,bool> OnStepChangeEvent;
    protected List<T>data; 
    [SerializeField,Tooltip("勾选后不会将数据同步到外部,仅在本地操作")]
    private bool executeOnlyLocal;
    /// <summary>
    /// 初始化记录层
    /// </summary>
    /// <param name="Capacity"></param>
    /// <param name="store"></param>
    public override void Init(int Capacity,CustomizedStore store)
    {
        steps=new Stack<T>(Capacity);
        data=new List<T>(Capacity);
    }
    /// <summary>
    /// 获取更新命令
    /// 使用命令模式便于对多个回溯器进行同步
    /// </summary>
    /// <returns></returns>
    public GenericTimeStepCommand<T> GetCommand()
    {
        if(OutCommand==null)
        {
            OutCommand=new GenericTimeStepCommand<T>();
            OutCommand.OnUpdateStepEvent+=Execute;
        }     
        return OutCommand;
    }
    /// <summary>
    /// 绑定更新命令
    /// 使用命令模式便于对多个回溯器进行同步
    /// </summary>
    /// <param name="command"></param>
    public void SetCommand(GenericTimeStepCommand<T> command)
    {
        if(OutCommand!=null)
        {
            OutCommand.OnUpdateStepEvent-=Execute;
        }
        OutCommand=command;
        OutCommand.OnUpdateStepEvent+=Execute; 
    }

    /// <summary>
    /// 将回溯结果输出到外部模型中
    /// </summary>
    public void PopResult(T result,bool playBack)
    {
        OnStepChangeEvent?.Invoke(result,playBack);
    }
    sealed public override void Recall(bool playBack,bool localRecall)
    {
        if(steps.Count==0)
            return;
        var oldStep=steps.Pop();
        if(localRecall||executeOnlyLocal)//回溯器本地回溯或者记录层强制本地回溯时
            Execute(oldStep,playBack);
        if(!executeOnlyLocal)//非强制本地回溯则抛出回溯数据,避免开启同步模型后额外的无效性能开销
            PopResult(oldStep,playBack);
    }
    /// <summary>
    /// 处理记录层内部回溯逻辑
    /// </summary>
    /// <param name="result"></param>
    /// <param name="playBack"></param>
    protected virtual void Execute(T result,bool playBack)
    {
        
    }
    sealed public override void ClearStep()
    {
        steps.Clear();
        
    }
   
    sealed public override void Save()
    {
        data.Clear();
        foreach(var step in steps)
        {
            data.Add(step);
        }
    }
    
    sealed public override void Load(bool playBack=true)
    {
        int dataCount =data.Count;
        if(playBack)
            for(int i=dataCount-1;i>=0;i--)
            {
                steps.Push(data[i]);
            }
        else
            for(int i=0;i<dataCount;i++)
            {
                steps.Push(data[i]);
            }
    }

    sealed public override int DataCount()
    {
        return data.Count;
    }
    sealed public override void ClearData()
    {
        data.Clear();
    }
    public override void ShutDown()
    {
        steps.Clear();
        data.Clear();
        if(OutCommand!=null)
        {
            OutCommand.OnUpdateStepEvent-=Execute;
        }  
    }
    public override void RegisterToSyncModel(ITimeSync model,int index)
    {
        model.Register<T>(this,index);
    }
}
}