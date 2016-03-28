using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class T_Mgr : MonoBehaviour
{
    private int HP;
    private int DP;
    private int PP;
    private int AP;
    private float EP;

    public Scrollbar fillGaugeBar;

    public enum LayerState { invincibility, normal }
    LayerState curLayerState;

    public enum State { idle, attack, Skill, be_Shot }
    State state;

    public enum SkillType { AP, EP, PP }

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

    // 스매쉬 온 오프 위한 것,
    public GameObject _smash;

    private T_MoveCtrl T_moveCtrl;

    #region <달리기 상태에 따른 ep증감>
    private float fDecEP = 0.4f, fIncEP = 0.6f;
    private float fIncAccelPoint = 0.2f;
    private float decreaseTime = 0.01f, decreaseTimer = 0.0f;
    private float increaseTime = 0.005f, increaseTimer = 0.0f;
    private float incAccelTime = 1.0f, incAccelTimer = 0.0f;
    #endregion

    void Start()
    {
        HP = T_Stat.MAX_HP;
        DP = T_Stat.INIT_DP;
        PP = T_Stat.MAX_PP;
        AP = T_Stat.MAX_AP;
        //EP = T_Stat.MAX_EP;
        EP = 100.0f;

        curLayerState = LayerState.normal;

        T_moveCtrl = GetComponent<T_MoveCtrl>();

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
        GUI.Label(new Rect(20, 90, 200, 25), "EP : " + EP + " / " + T_Stat.MAX_EP);
        GUI.Label(new Rect(20, 110, 200, 25), "HP : " + HP + " / " + T_Stat.MAX_HP);
        GUI.Label(new Rect(20, 130, 200, 25), "DP : " + DP + " / " + T_Stat.MAX_DP);
        GUI.Label(new Rect(20, 150, 200, 25), "PP : " + PP + " / " + T_Stat.MAX_PP);
    }
    void Update()
    {
        //임시 지구력 UI.
        fillGaugeBar.size = EP * 0.01f;

        if (state == State.idle || state == State.attack)
        {
            if (EP < 0.0f)
                ctrlPossible.Sprint = false;

            if (T_moveCtrl.getMoveState() == T_MoveCtrl.MoveState.Run ||
                T_moveCtrl.getMoveState() == T_MoveCtrl.MoveState.Stop)
            {
                EpIncrease();
            }
            else if (T_moveCtrl.getMoveState() == T_MoveCtrl.MoveState.Sprint)
            {
                EpDecrease();
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        //몬스터 공격이면 데미지만큼 HP 차감
        if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterAttkCollider))
        {
            print("hit");
            int iDamage = coll.gameObject.GetComponent<MonsterAttkCtrl>().GetDamage();

            if (iDamage != 0 && DP > 0)
            {
                if (DP > iDamage)
                {
                    //DP -= iDamage;
                    //피격 지속시간은 나중에 피격상태에 따라 달라지도록 구현헤야 할 듯,
                    StartCoroutine(BeShotTimer(2.0f));
                }
                else
                {
                    DP = 0;
                }
            }
            else
            {
                //쥬금
                print("쥬금");
            }
        }
    }

    public float getEP() { return EP; }
    public void setEP(float f) { EP = f; }
    public int getAP() { return AP; }
    public void setAP(int i) { AP = i; }
    public int getDP() { return DP; }
    public void setDP(int i) { DP = i; }
    public int getPP() { return PP; }
    public void setPP(int i) { PP = i; }

    public LayerState getLayerState() { return curLayerState; }
    public void setLayerState(LayerState s)
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
    public void setCtrlPossible(CtrlPossibleIndex index, bool b)
    {
        if (index == CtrlPossibleIndex.Run) ctrlPossible.Run = b;
        else if (index == CtrlPossibleIndex.Sprint) ctrlPossible.Sprint = b;
        else if (index == CtrlPossibleIndex.Attack) ctrlPossible.Attack = b;
        else if (index == CtrlPossibleIndex.MouseRot) ctrlPossible.MouseRot = b;
        else if (index == CtrlPossibleIndex.Skill) ctrlPossible.Skill = b;
    }
    public CtrlPossible getCtrlPossible() { return ctrlPossible; }

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
    public State getState() { return state; }
    IEnumerator BeShotTimer(float time)
    {
        print("beShot");
        ChangeState(State.be_Shot);

        yield return new WaitForSeconds(time);
        ChangeState(State.idle);
    }

    //전력질주시에 사용하는 EP감소, 충전, 충전가속 함수.
    void EpDecrease()
    {
        //가속으로 인해 바뀐 수치들 초기화
        fIncEP = 0.2f;
        incAccelTimer = 0.0f;

        if (EP > 0.0f)
        {
            if (decreaseTimer > decreaseTime)
            {
                EP -= fDecEP;
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
        if (EP < T_Stat.MAX_EP)
        {
            EpIncAccel();
            if (increaseTimer > increaseTime)
            {
                EP += fIncEP;
                increaseTimer = 0.0f;
            }
            else
                increaseTimer += Time.deltaTime;
        }
        else
        {
            if (EP > 100)
                EP = 100.0f;
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
    public bool DecreaseSkillPoint(T_Mgr.SkillType type, int decPoint)
    {
        if (type == T_Mgr.SkillType.AP)
        {
            if (AP <= 0 || AP < decPoint)
            {
                print("AP가 부족합니다.");
                return false;
            }
            else
            {
                AP -= decPoint;
                return true;
            }
        }
        else if (type == T_Mgr.SkillType.EP)
        {
            if (EP <= 0 || EP < decPoint)
            {
                print("EP가 부족합니다.");
                return false;
            }
            else
            {
                //가속으로 인해 바뀐 수치들 초기화
                fIncEP = 0.6f;
                incAccelTimer = 0.0f;

                EP -= decPoint;
                return true;
            }
        }
        else if (type == T_Mgr.SkillType.PP)
        {
            if (PP <= 0 || PP < decPoint)
            {
                print("PP가 부족합니다.");
                return false;
            }
            else
            {
                PP -= decPoint;
                return true;
            }
        }
        return false;
    }
}
