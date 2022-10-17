using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface StateContext
{
    void SetState(SelectState newState);
}

public interface SelectState
{
    string name{ get; set; }
    void GetTriggerButton_down(FieldOfView fow,Run run);
    void GetTriggerButton_up(FieldOfView fow,Run run);
    void GetPrimaryButton_up(StateContext context,FieldOfView fow,Run run);
}

// public class SelectStatePattern : MonoBehaviour, StateContext
public class SelectStatePattern : StateContext
{
    // public SelectState currentState = new NullState();
    public SelectState currentState = new MotionmatchingState();
    FieldOfView currentFow;
    Run currentRun;
    public SelectStatePattern(FieldOfView fow,Run run)
    {
        currentRun = run;
        currentFow = fow;
    }
    
    public void GetTriggerButton_down() => currentState.GetTriggerButton_down(currentFow, currentRun);
    public void GetTriggerButton_up() => currentState.GetTriggerButton_up(currentFow, currentRun);
    public void GetPrimaryButton_up() => currentState.GetPrimaryButton_up(this,currentFow, currentRun);
    void StateContext.SetState(SelectState newState)
    {
        currentState = newState;
    }
    public void SetState(SelectState newState)
    {
        currentState = newState;
    }
}

public class MotionmatchingState : SelectState
{

    private string _name = "MotionmatchingState"; 
    Random_obj random_Obj = GameObject.Find("GameEvent").GetComponent<Random_obj>();

    public string  name //属性成员的实现
    {
        get { return _name; }
        set { _name = name; }
    }

    public void GetTriggerButton_down(FieldOfView fow,Run run)
    {		
        // run._isbegin = !run._isbegin;
        run._isbegin = true;

        TimeRecorder.Instance.AddTxtTextByFileInfo("按下扳机:");
    }
    public void GetTriggerButton_up(FieldOfView fow,Run run)
    {
        //松开按键时计算相关系数，重置所有目标的material
        // run.CalculateCorr();
        // ClearConsole();
        run.ClearMaterial();
        // run._isbegin = !run._isbegin;
        run._isbegin = false;
        if(fow.tobeSelectedTarget)
        {
            RaycastHit hit;
            Transform target = fow.tobeSelectedTarget;
            Transform head = fow.transform;
            Vector3 dirToTarget = (target.position - head.position).normalized;
            float dstToTarget = Vector3.Distance (head.position, target.position);
            // RaycastHit[] hits = Physics.RaycastAll(head.position,dirToTarget,dstToTarget);
            TimeRecorder.Instance.AddTxtTextByFileInfo("松开扳机:");
            // if (fow.finalTargets.Contains(fow.tobeSelectedTarget))
            LayerMask myLayerMask = run.targetMask;
            if(Physics.Raycast(head.position,dirToTarget,out hit,dstToTarget,myLayerMask))
            {
            if (fow.selectedTarget == fow.tobeSelectedTarget)
                {
                    if(hit.transform == target)
                        TimeRecorder.Instance.AddTxtTextByFileInfo("选择正确"+
                        ",方向:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().dir.ToString()+
                        ",速度:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().speed.ToString()+
                        ",位相:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().theta.ToString()+
                        ",目标坐标:" + fow.tobeSelectedTarget.localPosition.ToString()+
                        ",无遮挡", false);
                    else
                        TimeRecorder.Instance.AddTxtTextByFileInfo("选择正确"+
                        ",方向:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().dir.ToString()+
                        ",速度:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().speed.ToString()+
                        ",位相:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().theta.ToString()+
                        ",目标坐标:" + fow.tobeSelectedTarget.localPosition.ToString()+
                        ",遮挡", false);            
                }
                
            else
            {
                if(hit.transform == target)
                        TimeRecorder.Instance.AddTxtTextByFileInfo("选择错误"+
                        ",方向:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().dir.ToString()+
                        ",速度:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().speed.ToString()+
                        ",位相:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().theta.ToString()+
                        ",目标坐标:" + fow.tobeSelectedTarget.localPosition.ToString()+
                        ",无遮挡", false);
                    else
                        TimeRecorder.Instance.AddTxtTextByFileInfo("选择错误"+
                        ",方向:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().dir.ToString()+
                        ",速度:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().speed.ToString()+
                        ",位相:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().theta.ToString()+
                        ",目标坐标:" + fow.tobeSelectedTarget.localPosition.ToString()+
                        ",遮挡", false);
            }
            }
        }

        // 过4秒后刷新界面，一共重复十次
        if( run.gameCount< 19 && run.gameCount >= 0 )
        {
            // // 随机
            // if(random_Obj.waiting == false)
            // {
            //     DelayController.Instance.DelayToCall(random_Obj.StartGame, 4f);
            //     run.gameCount++;
            //     random_Obj.waiting = true;
            // }
            //固定
            if(random_Obj.waiting == false)
            {
                DelayController.Instance.DelayToCall(random_Obj.nextGame, 3f);
                run.gameCount++;
                random_Obj.waiting = true;
                fow.selectedTarget = null;
                run.locations[0].Clear();
                run.locations[1].Clear();
            }
        }
        else
        {
            random_Obj.num = 0;
            DelayController.Instance.DelayToCall(random_Obj.ClearAll, 2f);
            run.gameCount = 0;
            DelayController.Instance.DelayToCall(run.buttonController.showButton, 2f);
            DelayController.Instance.DelayToCall(()=>run.ssPattern.SetState(new NullState()), 2f);
            DelayController.Instance.DelayToCall(random_Obj.closeTarget, 2f);
            GameObject.Find("UIHelpers").transform.Find("LaserPointer").gameObject.SetActive(true);
        }
    
    }

    public void GetPrimaryButton_up(StateContext context,FieldOfView fow,Run run)
    {
        //按下a键重置程序解决bug
        run._isbegin = false;
        // fow.finalTargets.Clear();
        run.ClearMaterial();
        //切换选择模式
        context.SetState(new RayState());
        GameObject.Find("UIHelpers").transform.Find("LaserPointer").gameObject.SetActive(true);
    }

}

public class RayState : SelectState
{
    Random_obj random_Obj = GameObject.Find("GameEvent").GetComponent<Random_obj>();

    private string _name = "RayState"; 
    public string  name //属性成员的实现
    {
        get { return _name; }
        set { _name = name; }
    }
    public void GetTriggerButton_down(FieldOfView fow,Run run)
    {
        TimeRecorder.Instance.AddTxtTextByFileInfo("按下扳机:");
        run._isRaybegin =false;
        RaycastHit hit;
        Transform target = fow.tobeSelectedTarget;
        Transform head = fow.transform;
        Vector3 dirToTarget = (target.position - head.position).normalized;
        float dstToTarget = Vector3.Distance (head.position, target.position);
        LayerMask myLayerMask = run.targetMask;
        // RaycastHit[] hits = Physics.RaycastAll(head.position,dirToTarget,dstToTarget);
        if(Physics.Raycast(head.position,dirToTarget,out hit,dstToTarget,myLayerMask))
        {
        if (fow.selectedTarget == fow.tobeSelectedTarget)
        {
            if(hit.transform == target)
                TimeRecorder.Instance.AddTxtTextByFileInfo("选择正确"+
                ",方向:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().dir.ToString()+
                ",速度:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().speed.ToString()+
                ",位相:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().theta.ToString()+
                ",目标坐标:" + fow.tobeSelectedTarget.localPosition.ToString()+
                ",无遮挡", false);
            else
                TimeRecorder.Instance.AddTxtTextByFileInfo("选择正确"+
                ",方向:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().dir.ToString()+
                ",速度:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().speed.ToString()+
                ",位相:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().theta.ToString()+
                ",目标坐标:" + fow.tobeSelectedTarget.localPosition.ToString()+
                ",遮挡", false);            
        }
            
        else
        {
            if(hit.transform == target)
                    TimeRecorder.Instance.AddTxtTextByFileInfo("选择错误"+
                    ",方向:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().dir.ToString()+
                    ",速度:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().speed.ToString()+
                    ",位相:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().theta.ToString()+
                    ",目标坐标:" + fow.tobeSelectedTarget.localPosition.ToString()+
                    ",无遮挡", false);
                else
                    TimeRecorder.Instance.AddTxtTextByFileInfo("选择错误"+
                    ",方向:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().dir.ToString()+
                    ",速度:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().speed.ToString()+
                    ",位相:" + fow.tobeSelectedTarget.GetComponent<RecordPosition>().theta.ToString()+
                    ",目标坐标:" + fow.tobeSelectedTarget.localPosition.ToString()+
                    ",遮挡", false);
        }
        }
        // 过4秒后刷新界面，一共重复十次
        if( run.gameCount< 19 && run.gameCount >= 0 )
        {
            if(random_Obj.waiting == false)
            {
                DelayController.Instance.DelayToCall(random_Obj.nextGame, 3f);
                // DelayController.Instance.DelayToCall(random_Obj.StartGame, 3f);
                DelayController.Instance.DelayToCall(()=>run._isRaybegin = true, 3f);
                
                run.gameCount++;
                random_Obj.waiting = true;
                fow.selectedTarget = null;
            }
        }
        else
        {
            random_Obj.num = 0;
            DelayController.Instance.DelayToCall(random_Obj.ClearAll, 2f);
            run.gameCount = 0;
            DelayController.Instance.DelayToCall(run.buttonController.showButton, 2f);
            DelayController.Instance.DelayToCall(()=>run.ssPattern.SetState(new NullState()), 4f);
            DelayController.Instance.DelayToCall(random_Obj.closeTarget, 2f);
        }
    }
    public void GetTriggerButton_up(FieldOfView fow,Run run)
    {

    }
    public void GetPrimaryButton_up(StateContext context,FieldOfView fow,Run run)
    {
        context.SetState(new MotionmatchingState());
        fow.selectedTarget = null;
        run.ClearMaterial();
        GameObject.Find("UIHelpers").transform.Find("LaserPointer").gameObject.SetActive(false);
    }
}

public class NullState : SelectState
{
    
    private string _name = "NullState"; 
    public string  name //属性成员的实现
    {
        get { return _name; }
        set { _name = name; }
    }
    public void GetTriggerButton_down(FieldOfView fow,Run run)
    {

    }
    public void GetTriggerButton_up(FieldOfView fow,Run run)
    {

    }
    public void GetPrimaryButton_up(StateContext context,FieldOfView fow,Run run)
    {
        context.SetState(new MotionmatchingState());
    }

}


