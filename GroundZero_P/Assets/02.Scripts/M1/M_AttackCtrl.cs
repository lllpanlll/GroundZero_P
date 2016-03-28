using UnityEngine;
using System.Collections;

public class M_AttackCtrl : MonoBehaviour
{
    public MonsterState attkState = MonsterState.Idle;

    private int damage;                 //데미지
    private bool isRigid;               //경직
    private bool isTotter;              //비틀거림
    private bool isKnockback;           //넉벡
    private bool isKnockdown;           //넉다운
    private bool isSlam;                //날아감

    void OnEnable()
    {
        //공격이 시작될 때 활성화되면서 현재 공격 상태 가져옴
        //attkState = GameObject.FindWithTag(Tags.Monster).GetComponent<MonsterCtrl>().GetMonsterState();

        SetAttkProperty();
    }

    #region <공격에 따른 수치 설정>
    public void SetAttkProperty()
    {
        switch (attkState)
        {
            case MonsterState.NearAttk:
                damage = GameObject.FindWithTag(Tags.Monster).GetComponent<MonsterCtrl>().nearAttkDamage;
                isRigid = false;
                isTotter = false;
                isKnockback = false;
                isKnockdown = false;
                isSlam = false;

                break;

            case MonsterState.BreathAttk:
                damage = GameObject.FindWithTag(Tags.Monster).GetComponent<MonsterCtrl>().breathAttkDamage;
                isRigid = false;
                isTotter = false;
                isKnockback = false;
                isKnockdown = false;
                isSlam = false;

                break;

            case MonsterState.JumpAttk:
                damage = GameObject.FindWithTag(Tags.Monster).GetComponent<MonsterCtrl>().jumpAttkDamage;
                isRigid = false;
                isTotter = false;
                isKnockback = false;
                isKnockdown = false;
                isSlam = false;

                break;
        }
    }
    #endregion

    public int GetDamage() { return damage; }
    public bool IsRigid() { return isRigid; }
    public bool IsTotter() { return isTotter; }
    public bool IsKnockback() { return isKnockback; }
    public bool IsKnockdown() { return isKnockdown; }
    public bool IsSlam() { return isSlam; }
}

