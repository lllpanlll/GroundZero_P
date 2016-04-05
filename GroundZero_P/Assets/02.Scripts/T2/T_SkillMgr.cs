using UnityEngine;
using System.Collections;

public class T_SkillMgr : MonoBehaviour {
    
    private bool bBeforeDelay;
    private bool bAction;
    private bool bAfterDelay;
    private bool bCoolTime;
    private bool bUsing;

    private float fCoolTime;
    
    void Start () {       
        bBeforeDelay = false;
        bAction = false;
        bAfterDelay = false;
        bCoolTime = false;
        bUsing = false;
        fCoolTime = 0.0f;
    }

    protected virtual void InputCommand (T_Mgr.SkillType type, int decPoint)
    {        
        if (T_Mgr.GetInstance().GetCtrlPossible().Skill == true && bCoolTime == false)
        {
            //point가 성공적으로 소모 된다면,
            if(T_Mgr.GetInstance().DecreaseSkillPoint(type, decPoint))
            {
                T_Mgr.GetInstance().ChangeState(T_Mgr.State.Skill);

                bUsing = true;
                bBeforeDelay = true;
                return;
            }           
        }

        bBeforeDelay = false;
    }
    protected virtual void BeforeActionDelay(float time)
    {
        this.StartCoroutine(BeforeDelayTimer(time));
    }
    protected virtual void Execute(float time)
    {
        this.StartCoroutine(ExecuteTimer(time));
    }
    protected virtual void AfterActionDelay(float time)
    {
        this.StartCoroutine(AfterDelayTimer(time));
    }
    protected virtual void CoolTimeDelay()
    {
        this.StartCoroutine(CoolTimer(fCoolTime));
    }

    protected virtual void SkillCancel()
    {
        bBeforeDelay = false;
        bAction = false;
        bAfterDelay = false;
        bCoolTime = false;
        fCoolTime = 0.0f;
    }
    protected bool IsAfterDelay() { return bAfterDelay; }
    protected bool IsBeforeDelay() { return bBeforeDelay; }
    protected bool IsExecute() { return bAction; }
    protected bool IsRunning() { return bUsing; }
    protected bool IsCoolTime() { return bCoolTime; }

    protected void FinishExecute() { bAction = false;  bAfterDelay = true; }
    protected void SetAction(bool b) { bAction = b; }
    protected void SetAfterDelay(bool b) { bAfterDelay = b; }
    protected void SetCoolTime(float time) { fCoolTime = time; }
    
    IEnumerator BeforeDelayTimer(float time)
    {        
        //선 딜레이 애니메이션 플레이

        //if문에 한번만 작동 되도록 bBeforeDelay를 바로 false시켜준다.
        bBeforeDelay = false;
        yield return new WaitForSeconds(time);
        //선 딜레이 애니메이션 정지?
       
        bAction = true;
    }
    IEnumerator ExecuteTimer(float time)
    {
        //애니메이션 플레이

        //if문에 한번만 작동 되도록 bAction을 바로 false시켜준다.
        bAction = false;
        yield return new WaitForSeconds(time);
        //애니메이션 정지?

        bAfterDelay = true;        
    }
    IEnumerator AfterDelayTimer(float time)
    {
        //후 딜레이 애니메이션 플레이

        //if문에 한번만 작동 되도록 bAfterDelay를 바로 false시켜준다.
        bAfterDelay = false;
        yield return new WaitForSeconds(time);
        //후 딜레이 애니메이션 정지?

        T_Mgr.GetInstance().ChangeState(T_Mgr.State.idle);
        bCoolTime = true;        
    }
    IEnumerator CoolTimer(float time)
    {
        bUsing = false;
        yield return new WaitForSeconds(time);
        print("쿨타임 종료");
        bCoolTime = false;        
    }
}
