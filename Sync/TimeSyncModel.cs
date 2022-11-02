using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Kurisu.TimeControl.Sync
{
    /// <summary>
    /// 接口：时间回溯同步模型接口
    /// </summary>
    public interface ITimeSync
    {
        public void Register<T>(GenericLayer<T> layer,int index) where T:struct,ITimeStep;
    }
    /// <summary>
    /// 本地回溯同步模型，记录层Layer将记录的回溯Step数据丢给同步模型SyncModel，由模型发布Command给记录层进行更新
    /// 实际上本地玩家不需要此步骤，直接在记录层内部操作即可
    /// </summary>
public class TimeSyncModel : MonoBehaviour,ITimeSync
{
    private Dictionary<int,BaseLayer> layers=new Dictionary<int, BaseLayer>();
    /// <summary>
    /// 注册待同步记录层
    /// </summary>
    public void Register<T>(GenericLayer<T> layer,int index) where T:struct,ITimeStep
    {
        layers.Add(index,layer);
        layer.OnStepChangeEvent+=delegate(T step,bool playBack){UpdateStep(step,playBack,index);};
    }
    void UpdateStep<T>(T step,bool playback,int index) where T:struct,ITimeStep
    {
        var layer=layers[index] as GenericLayer<T>;
        layer.GetCommand().Execute(step,playback);
    }
}
}