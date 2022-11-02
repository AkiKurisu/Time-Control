using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Kurisu.TimeControl
{
public class TimeManager : MonoBehaviour
{
    [LabelText("触发按键"),SerializeField]
    private KeyCode key=KeyCode.T;
    public static TimeManager instance;
    public static TimeManager Instance
    {
        get{return instance;}
    }
    protected virtual void  Awake()
     {
       if (instance!=null)
           Destroy(gameObject);
       else
            instance=(TimeManager)this; 
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
    private void Update() {
        if(Input.GetKeyDown(key))
        {
            if(TimeController.Instance.CurrentState==TimeController.TimeState.正常)
            {
                TimeController.Instance.RecordAll();
            }
            else
            {
                if(TimeController.Instance.CurrentState==TimeController.TimeState.记录)
                TimeController.Instance.RecallAll();
            }
        }
    }
}
}
