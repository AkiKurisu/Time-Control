using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.UI;
namespace Kurisu.TimeControl
{
/// <summary>
/// 控制层
/// </summary>
public class TimeController : MonoBehaviour
{
    public enum TimeState
    {
        正常,记录,回溯
    }
    [LabelText("回溯器列表"),SerializeField]
    private List<TimeStore> stores=new List<TimeStore>();
    private TimeStore playerStore;
    [LabelText("当前状态"),ReadOnly,SerializeField]
    private TimeState state;
    public bool IsRecalling
    {
        get{return state==TimeState.回溯;}
    }
    public TimeState CurrentState
    {
        get{return state;}
    }
    [SerializeField,LabelText("记录上限"),DisableInPlayMode]
    private int capacity=3000;
    public int Capacity
    {
        get{return capacity;}
    }
    [SerializeField,LabelText("当前记录数"),ProgressBar(0,"capacity"),ReadOnly]
    private int currentCount;
    [SerializeField,LabelText("记录步长"),Range(0.01f,0.2f),Tooltip("每多少秒记录一次"),DisableIf("state",TimeState.记录)]
    private float recordStep=0.1f;
    [SerializeField,LabelText("回溯步长"),Range(0.01f,0.2f),Tooltip("每多少秒回溯一次"),DisableIf("state",TimeState.回溯)]
    private float recallStep=0.02f;
    /// <summary>
    /// 当前记录步长
    /// </summary>
    /// <value></value>
    public float RecordStep
    {
        get{return recordStep;}
    }
    private float timer;
    public event Action OnRecordStartEvent;
    /// <summary>
    /// 回溯开始事件
    /// </summary>
    public event Action OnRecallStartEvent;
    /// <summary>
    /// 回溯结束事件
    /// </summary>
    public event Action OnRecallEndEvent;
    /// <summary>
    /// 记录中事件
    /// </summary>
    public event Action OnRecordEvent;
    /// <summary>
    /// 回溯中事件
    /// </summary>
    public event Action OnRecallEvent;
    /// <summary>
    /// 记录数变更事件（适用于UI更新）
    /// </summary>
    public event Action<float> OnStepChangeEvent;
    /// <summary>
    /// 控制器状态变更事件（适用于多人游戏状态同步）
    /// </summary>
    public event Action<TimeState>OnStateChangeEvent;

    [LabelText("固定更新"),SerializeField]
    private bool useFixedUpdate;
    /// <summary>
    /// 使用物理更新FixedUpdateMode
    /// </summary>
    /// <value></value>
    public bool UseFixedUpdate
    {
        get{return useFixedUpdate;}
    }
    public static TimeController instance;
    public static TimeController Instance
    {
        get{return instance;}
    }
    protected virtual void  Awake()
     {
       if (instance!=null)
           Destroy(gameObject);
       else
            instance=(TimeController)this; 
    }
    public static bool IsInitialized
    {
        get{return instance !=null;}
    }
    protected virtual void OnDestroy() 
    {
        if (instance==this)
        {
            instance=null;
        }
    }
    public void Add(TimeStore store)
    {
        if(!stores.Contains(store))
        {
            stores.Add(store);
        }
    }
    public void Remove(TimeStore store)
    {
        if(stores.Contains(store))
        { 
            stores.Remove(store);
        }
    }
    /// <summary>
    /// 回溯器开始记录
    /// </summary>
    [Button("记录"),HideInEditorMode,EnableIf("state",TimeState.正常)]
    public void RecordAll()
    {
        timer=recordStep;
        UpdateState(TimeState.记录);
        OnRecordStartEvent?.Invoke();
            foreach(var store in stores)
            {
                store.Record();
            }
       
    }
    /// <summary>
    /// 回溯器开始回溯
    /// </summary>
    [Button("回溯"),HideInEditorMode,EnableIf("state",TimeState.记录)]
    public void RecallAll()
    {
        timer=0;
        UpdateState(TimeState.回溯);
        OnRecallStartEvent?.Invoke();     
            foreach(var store in stores)
            {
                store.Recall();
            }
        recallCount=currentCount;
    }
    /// <summary>
    /// 强制关闭所有回溯器
    /// </summary>
    public void ShutdownAll()
    {
        state=TimeState.正常;
        currentCount=0;
        foreach(var store in stores)
        {
            store.ShutDown();
        }
    }
    private void Update() {
        if(!useFixedUpdate)
            UpdateStore(Time.deltaTime);
    }
    private void FixedUpdate() {
        if(useFixedUpdate)
            UpdateStore(Time.fixedDeltaTime);
    }
    int recallCount;
    void UpdateStore(float deltaTime) {
        switch(state)
        {
            case TimeState.正常:
            {
                break;
            }
            case TimeState.记录:
            {
                if(currentCount>=capacity)//到达上限直接回溯
                {
                    RecallAll();
                    break;
                }
                timer+=deltaTime;
                if(timer>=recordStep)
                {
                    timer=0;
                    currentCount+=1;
                    OnRecordEvent?.Invoke();//调用记录时刻
                    OnStepChangeEvent?.Invoke((float)currentCount/Capacity);
                }
                break;
            }
            case TimeState.回溯:
            {
                if(currentCount==0)//回溯结束调用回溯结束事件
                {
                    OnRecallEndEvent?.Invoke();
                    UpdateState(TimeState.正常);
                    break;
                }
                timer+=Time.deltaTime;
                if(timer>=recallStep)
                {
                    timer=0;
                    currentCount-=1;
                    OnStepChangeEvent?.Invoke((float)currentCount/Capacity);
                    OnRecallEvent?.Invoke();//未结束则调用回溯时刻
                }
                break;
            }
        }
    }
    void UpdateState(TimeState newState)
    {
        state=newState;
        OnStateChangeEvent?.Invoke(state);
    }
}
}