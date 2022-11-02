using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Kurisu.TimeControl.Sync;
namespace Kurisu.TimeControl
{
/// <summary>
/// 自定义回溯器,按需求加入需要记录的层,因此Transform也是可以自定义记录的层
/// </summary>
public class CustomizedStore : TimeStore
{

    
    [SerializeField,LabelText("自定义记录层"),DisableInPlayMode]
    private BaseLayer[] layers;
    [LabelText("运行中记录层"),DisableInEditorMode]
    public BaseLayer[] runTimeLayers;
    [SerializeField,LabelText("暂存记录到SO"),DisableInPlayMode]
    private bool saveToSO=true;
    [SerializeField,LabelText("使用同步模型"),InfoBox("该选项需要挂载SyncModel,勾选后除了强制本地操作的记录层均使用同步机制进行回溯"),DisableInPlayMode]
    private bool useSyncModel=false;
    [SerializeField,LabelText("独播强制同步"),InfoBox("独播模式为正向播放,一般不需要同步"),ShowIf("useSyncModel"),DisableInPlayMode]
    private bool alonePlaySync=false;
    private ITimeSync syncModel;
    /// <summary>
    /// 独播开始事件
    /// </summary>
    public event Action OnAlonePlayStartEvent;
    /// <summary>
    /// 独播结束事件
    /// </summary>
    public event Action OnAlonePlayEndEvent;
    protected override void Start() {
        if(TimeController.IsInitialized)
        {
            TimeController.Instance.Add(this);
        }
        runTimeLayers=new BaseLayer[layers.Length];
        if(useSyncModel)
            syncModel=GetComponent<ITimeSync>();
        for(int i=0;i<layers.Length;i++)
        {
            runTimeLayers[i]=Instantiate(layers[i]);
            runTimeLayers[i].Init(TimeController.Instance.Capacity,this);
            if(useSyncModel)
                runTimeLayers[i].RegisterToSyncModel(syncModel,i);
        }
    }
    protected override void OnDestroy() {
        if(TimeController.IsInitialized)
        {
            TimeController.Instance.Remove(this);
            TimeController.Instance.OnRecallEndEvent-=TimeStoreOver;
            TimeController.Instance.OnRecallEvent-=RecallTick;
            TimeController.Instance.OnRecordEvent-=RecordTick;
            
        }
        foreach(var layer in runTimeLayers)
        {
            layer.ShutDown();
        }
    }
    
    public override void Record()
    {
        if(locked)
            return;
        TimeController.Instance.OnRecordEvent+=RecordTick;
        foreach(var layer in runTimeLayers)
        {
            layer.ClearStep();
        }

    }
    public override void Recall()
    {
        TimeController.Instance.OnRecordEvent-=RecordTick;
        if(locked)
            return;
        TimeController.Instance.OnRecallEvent+=RecallTick;
        TimeController.Instance.OnRecallEndEvent+=TimeStoreOver;
        foreach(var layer in runTimeLayers)
        {
            if(saveToSO)
                layer.Save();
        }
    }

    public override void TimeStoreOver()
    {
        TimeController.Instance.OnRecallEndEvent-=TimeStoreOver;
        TimeController.Instance.OnRecallEvent-=RecallTick;
        TimeController.Instance.OnRecordEvent-=RecordTick;
        foreach(var layer in runTimeLayers)
        {
            layer.ClearStep();
        }      
    }

    public override void RecordTick()
    {  
        
        foreach(var layer in runTimeLayers)
        {
            layer.Record();
        }  
    }

    public override void RecallTick()
    {
        
        foreach(var layer in runTimeLayers)
        {
            layer.Recall(true,!useSyncModel);
        }   
    }
    int stepCount=0;
    [Button("倒放上次记录"),DisableInEditorMode]
    public virtual void RecallFromSO_Back()
    {
        stepCount=0;
        stepCount=runTimeLayers[0].DataCount();
        if(stepCount==0)
            return;
        TimeStoreOver();
        foreach(var layer in runTimeLayers)
        {
            layer.Load();
        } 
        LockTimeStore(true);
        timer=0;
        OnAlonePlayStartEvent?.Invoke();
    }
    [Button("正放上次记录"),DisableInEditorMode]
    public virtual void RecallFromSO_Forward()
    {
        stepCount=0;
        stepCount=runTimeLayers[0].DataCount();
        if(stepCount==0)
            return;
        TimeStoreOver();
        foreach(var layer in runTimeLayers)
        {
            layer.Load(false);
        } 
        LockTimeStore(true);
        timer=0;
        OnAlonePlayStartEvent?.Invoke();
    }
    [Button("清空记录"),DisableInEditorMode]
    protected void ClearSOData()
    {
        stepCount=0;
        foreach(var layer in runTimeLayers)
        {
            layer.ClearData();
        } 
    }
    float timer=0;
    private void FixedUpdate() {
        if(TimeController.Instance.UseFixedUpdate)
            if(stepCount!=0)
            {
                UpdateStoreAlone(Time.fixedDeltaTime);
            }
    } 
    private void Update() {
        if(!TimeController.Instance.UseFixedUpdate)
            if(stepCount!=0)
            {
                UpdateStoreAlone(Time.deltaTime);
            }
    }   
    /// <summary>
    /// 独播更新状态
    /// </summary>
    /// <param name="deltaTime"></param>
    void UpdateStoreAlone(float deltaTime)
    {
        timer+=deltaTime;
        if(timer>=TimeController.Instance.RecordStep)
        {
            timer=0;
            stepCount--;
             foreach(var layer in runTimeLayers)
            {
                layer.Recall(false,!alonePlaySync||!useSyncModel);
            }  
            if(stepCount==0)
            {
                LockTimeStore(false);
                OnAlonePlayEndEvent?.Invoke();
                TimeStoreOver();
            }
        }
    } 
    /// <summary>
    ///暂存数据长度
    /// </summary>
    /// <value></value>
     public float DataCount
     {
        get{return runTimeLayers[0].DataCount();}
     }       
            
    public override void ShutDown()
    {
        base.ShutDown();
        stepCount=0;
        foreach(var layer in runTimeLayers)
        {
            layer.ClearData();
        } 
    }     
    /// <summary>
    /// 正在独播
    /// </summary>
    /// <value></value>
    public bool IsAlonePlay
    {
        get{return stepCount>0;}
    }
}
}