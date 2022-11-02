using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Kurisu.TimeControl.Sync;
namespace Kurisu.TimeControl
{
    /// <summary>
    /// 基础记录层
    /// </summary>
public abstract class BaseLayer : ScriptableObject
{
    /// <summary>
    /// 自定义记录方法
    /// </summary>
    public virtual void Record()
    {

    }
    /// <summary>
    /// 自定义回溯方法
    /// </summary>
    /// <param name="playBack">是否倒播</param>
    public virtual void Recall(bool playBack,bool localRecall)
    {
        
    }
    /// <summary>
    /// 初始化栈
    /// </summary>
    /// <param name="Capacity"></param>
    public virtual void Init(int Capacity,CustomizedStore store)
    {
        
    }
    /// <summary>
    /// 清空栈
    /// </summary>
    public virtual void ClearStep()
    {
        
    }
    /// <summary>
    /// 保存到暂存数据
    /// </summary>
    public virtual void Save()
    {
        
    }
    /// <summary>
    /// 加载暂存数据
    /// </summary>
    /// <param name="playBack">是否倒放</param>
    public virtual void Load(bool playBack=true)
    {
        
    }
    /// <summary>
    /// 返回暂存数据长度
    /// </summary>
    /// <returns></returns>
    public virtual int DataCount()
    {
        return 0;
    }
    /// <summary>
    /// 清空暂存数据
    /// </summary>
    public virtual void ClearData()
    {

    }
    /// <summary>
    /// 退出模型
    /// </summary>
    public virtual void ShutDown()
    {
        
    }
    /// <summary>
    /// 注册到同步模型中
    /// </summary>
    /// <param name="model"></param>
    /// <param name="index"></param>
    public virtual void RegisterToSyncModel(ITimeSync model,int index)
    {

    }
}
}