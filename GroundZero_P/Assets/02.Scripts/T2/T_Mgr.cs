using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class T_Mgr : MonoBehaviour
{
    private int hp;
    private int dp;
    private int pp;
    private int ap;
    private float ep;

    public Scrollbar fillGaugeBar;

    public enum LayerState { invincibility, normal }
    LayerState curLayerState;

    public enum State { idle, attack, Skill, be_Shot }
    State state;

    public enum SkillType { AP, EP, PP }

    //현재 T_SkillMgr에서 사용중이다.
    private static T_Mgr instance;
    public static T_Mgr GetInstance()
    {
        if (!instance)
        {
            instance = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T_Mgr>();
            if (!instance)
                print("T_Mgr스크립트가 있는 게임오브젝트가 없습니다.");
        }
        return instance;
    }

    public struct CtrlPossible
    {
        public bool Run;
        public bool Sprint;
        public bool Attack;
        public bool MouseRot;
        public bool Skill;
    }
    public enum CtrlPossibleIndex { Run, Sprint, Attack, MouseRot, Skill };
    CtrlPossible ctrlPossible;

    private T_MoveCtrl t_MoveCtrl;

    #region <달리기 상태에 따른 ep증감>
    private float fDecEP = 0.4f, fIncEP = 0.6f;
    private float fIncAccelPoint = 0.2f;
    private float decreaseTime = 0.01f, decreaseTimer = 0.0f;
    private float increaseTime = 0.005f, increaseTimer = 0.0f;
    private float incAccelTime = 1.0f, incAccelTimer = 0.0f;
    #endregion

    void Start()
    {
        hp = T_Stat.MAX_HP;
        dp = T_Stat.INIT_DP;
        pp = T_Stat.MAX_PP;
        ap = T_Stat.MAX_AP;
        ep = T_Stat.MAX_EP;

        curLayerState = LayerState.normal;

        t_MoveCtrl = GetComponent<T_MoveCtrl>();

        //마우스 커서 숨기기
        //Cursor.visible = false;

        ctrlPossible.Run = true;
        ctrlPossible.Sprint = true;
        ctrlPossible.Attack = true;
        ctrlPossible.MouseRot = true;
        ctrlPossible.Skill = true;

        ChangeState(State.idle);
    }
    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 200, 25), "State : " + state);
        GUI.Label(new Rect(20, 90, 200, 25), "ep : " + ep + " / " + T_Stat.MAX_EP);
        GUI.Label(new Rect(20, 110, 200, 25), "hp : " + hp + " / " + T_Stat.MAX_HP);
        GUI.Label(new Rect(20, 130, 200, 25), "dp : " + dp + " / " + T_Stat.MAX_DP);
        GUI.Label(new Rect(20, 150, 200, 25), "pp : " + pp + " / " + T_Stat.MAX_PP);
    }
    void Update()
    {
        //임시 지구력 UI.
        fillGaugeBar.size = ep * 0.01f;

        if (state == State.idle || state == State.attack)
        {
            if (ep < 0.0f)
                ctrlPossible.Sprint = false;

            if (t_MoveCtrl.GetMoveState() == T_MoveCtrl.MoveState.Run ||
                t_MoveCtrl.GetMoveState() == T_MoveCtrl.MoveState.Stop)
            {
                EpIncrease();
            }
            else if (t_MoveCtrl.GetMoveState() == T_MoveCtrl.MoveState.Sprint)
            {
                EpDecrease();
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        //몬스터 공격이면 데미지만큼 hp 차감
        if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterAttkCollider))
        {
            print("hit");
            int iDamage = coll.gameObject.GetComponent<MonsterAttkCtrl>().GetDamage();

            if (iDamage != 0 && dp > 0)
            {
                if (dp > iDamage)
                {
                    //dp -= iDamage;
                    //피격 지속시간은 나중에 피격상태에 따라 달라지도록 구현헤야 할 듯,
                    StartCoroutine(BeShotTimer(2.0f));
                }
                else
                {
                    dp = 0;
                }
            }
            else
            {
                //쥬금
                print("쥬금");
            }
        }
    }

    public float GetEP() { return ep; }
    public void SetEP(float f) { ep = f; }
    public int GetAP() { return ap; }
    public void SetAP(int i) { ap = i; }
    public int GetDP() { return dp; }
    public void SetDP(int i) { dp = i; }
    public int GetPP() { return pp; }
    public void SetPP(int i) { pp = i; }

    public LayerState GetLayerState() { return curLayerState; }
    public void SetLayerState(LayerState s)
    {
        curLayerState = s;
        //캐릭터의 State를 이용하여 무적상태와 일반 상태를 변경시켜주는 부분,
        if (curLayerState == LayerState.normal)
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.T_HitCollider);
        }
        else if (curLayerState == LayerState.invincibility)
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.T_Invincibility);
        }
    }

    //possibleState함수들.
    public void SetCtrlPossible(CtrlPossibleIndex index, bool b)
    {
        if (index == CtrlPossibleIndex.Run) ctrlPossible.Run = b;
        else if (index == CtrlPossibleIndex.Sprint) ctrlPossible.Sprint = b;
        else if (index == CtrlPossibleIndex.Attack) ctrlPossible.Attack = b;
        else if (index == CtrlPossibleIndex.MouseRot) ctrlPossible.MouseRot = b;
        else if (index == CtrlPossibleIndex.Skill) ctrlPossible.Skill = b;
    }
    public CtrlPossible GetCtrlPossible() { return ctrlPossible; }

    public void ChangeState(State s)
    {
        state = s;
        if (state == State.idle)
        {
            ctrlPossible.Run = true;
            ctrlPossible.Sprint = true;
            ctrlPossible.Attack = true;
            ctrlPossible.MouseRot = true;
            ctrlPossible.Skill = true;
        }
        else if (state == State.attack)
        {
            ctrlPossible.Run = true;
            ctrlPossible.Sprint = false;
            ctrlPossible.Attack = true;
            ctrlPossible.MouseRot = true;
            ctrlPossible.Skill = true;
        }
        else if (state == State.Skill)
        {
            ctrlPossible.Run = false;
            ctrlPossible.Sprint = false;
            ctrlPossible.Attack = false;
            ctrlPossible.MouseRot = true;
            ctrlPossible.Skill = false;
        }
        else if (state == State.be_Shot)
        {
            ctrlPossible.Run = false;
            ctrlPossible.Sprint = false;
            ctrlPossible.Attack = false;
            ctrlPossible.MouseRot = true;
            ctrlPossible.Skill = false;
        }
    }
    public State GetState() { return state; }
    IEnumerator BeShotTimer(float time)
    {
        print("beShot");
        ChangeState(State.be_Shot);

        yield return new WaitForSeconds(time);
        ChangeState(State.idle);
    }

    //전력질주시에 사용하는 ep감소, 충전, 충전가속 함수.
    void EpDecrease()
    {
        //가속으로 인해 바뀐 수치들 초기화
        fIncEP = 1;
        incAccelTimer = 0.0f;

        if (ep > 0.0f)
        {
            if (decreaseTimer > decreaseTime)
            {
                ep -= fDecEP;
                decreaseTimer = 0.0f;
            }
            else
                decreaseTimer += Time.deltaTime;
        }
        else
            return;
    }
    void EpIncrease()
    {
        if (ep < T_Stat.MAX_EP)
        {
            EpIncAccel();
            if (increaseTimer > increaseTime)
            {
                ep += fIncEP;
                increaseTimer = 0.0f;
            }
            else
                increaseTimer += Time.deltaTime;
        }
        else
        {
            if (ep > 100)
                ep = 100;
        }
    }
    void EpIncAccel()
    {
        if (incAccelTimer > incAccelTime)
        {
            fIncEP += fIncAccelPoint;
            incAccelTimer = 0.0f;
        }
        else
            incAccelTimer += Time.deltaTime;
    }

    //스킬 타입별 포인트 감소
    public bool DecreaseSkillPoint (T_Mgr.SkillType type, int decPoint)
    {
        if (type == T_Mgr.SkillType.AP)
        {
            if (ap <= 0 || ap < decPoint)
            {
                print("ap가 부족합니다.");
                return false;
            }
            else
            {
                ap -= decPoint;
                return true;
            }
        }
        else if (type == T_Mgr.SkillType.EP)
        {
            if (ep <= 0 || ep < decPoint)
            {
                print("ep가 부족합니다.");
                return false;
            }
            else
            {
                //가속으로 인해 바뀐 수치들 초기화
                fIncEP = 0;
                incAccelTimer = 0.0f;

                ep -= decPoint;
                return true;
            }
        }
        else if (type == T_Mgr.SkillType.PP)
        {
            if (pp <= 0 || pp < decPoint)
            {
                print("pp가 부족합니다.");
                return false;
            }
            else
            {
                pp -= decPoint;
                return true;
            }
        }
        return false;
    }
}
