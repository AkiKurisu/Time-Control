using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Kurisu.TimeControl.Command
{
    /// <summary>
    /// 该类用于处理外部同步请求(多个记录层可以同时订阅同一个Command的更新Event,实现例如分身效果)
    /// </summary>
    /// <typeparam name="T"></typeparam>
public class GenericTimeStepCommand<T>  where T:struct
{
 
    /// <summary>
    /// 数据更新事件
    /// </summary>
    public event Action<T,bool> OnUpdateStepEvent;
    /// <summary>
    /// 处理外部输入
    /// </summary>
    /// <param name="step"></param>
    public void Execute(T step,bool playBack)
    {
        OnUpdateStepEvent?.Invoke(step,playBack);
    }
   
}
}