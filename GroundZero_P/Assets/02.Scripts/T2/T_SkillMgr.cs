using UnityEngine;
using System.Collections;

public class T_SkillMgr : MonoBehaviour {
    
    private bool bBeforeDelay;
    private bool bAction;
    private bool bAfterDelay;
    private bool bCoolTime;

    private float fCoolTime;
    
    void Start () {       
        bBeforeDelay = false;
        bAction = false;
        bAfterDelay = false;
        bCoolTime = false;
        fCoolTime = 0.0f;
    }

    protected virtual void InputCommend(T_Mgr.SkillType type, int decPoint)
    {        
        if (T_Mgr.GetInstance().getCtrlPossible().Skill == true && bCoolTime == false)
        {
            //point가 성공적으로 소모 된다면,
            if(T_Mgr.GetInstance().DecreaseSkillPoint(type, decPoint))
            {
                T_Mgr.GetInstance().ChangeState(T_Mgr.State.Skill);

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
    protected virtual void Action(float time)
    {
        this.StartCoroutine(ActionTimer(time));
    }
    protected virtual void AfterActionDelay(float time)
    {
        this.StartCoroutine(AfterDelayTimer(time));
    }

    protected virtual void SkillCancel()
    {
        bBeforeDelay = false;
        bAction = false;
        bAfterDelay = false;
        bCoolTime = false;
        fCoolTime = 0.0f;
    }
    protected bool isAfterDelay() { return bAfterDelay; }
    protected bool isBeforeDelay() { return bBeforeDelay; }
    protected bool isAction() { return bAction; }
    protected void setCoolTime(float time) { fCoolTime = time; }
    protected T_Mgr getT_Mgr() { return T_Mgr.GetInstance(); }
    
    IEnumerator BeforeDelayTimer(float time)
    {
        //선 딜레이 애니메이션 플레이

        //if문에 한번만 작동 되도록 bBeforeDelay를 바로 false시켜준다.
        bBeforeDelay = false;
        yield return new WaitForSeconds(time);
        //선 딜레이 애니메이션 정지?
       
        bAction = true;
    }
    IEnumerator ActionTimer(float time)
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
        this.StartCoroutine(CoolTimer(fCoolTime));
    }
    IEnumerator CoolTimer(float time)
    {        
        yield return new WaitForSeconds(time);
        bCoolTime = false;
    }
}
