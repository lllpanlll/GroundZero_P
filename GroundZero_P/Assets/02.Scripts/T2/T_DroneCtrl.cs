using UnityEngine;
using System.Collections;

public class T_DroneCtrl : T_SkillMgr {
    public Transform[] setPos = new Transform[6];
    public GameObject dronePref;
    private GameObject[] drones = new GameObject[6];

    private T_Mgr T_mgr;

    //private bool bAction;

    private int iDecEP = 10;

    //드론 지속 시간
    //private bool bLife = false;
    //private float lifeTime = 30.0f, lifeTimer = 0.0f;

    private float beforeDelayTime = 0.0f;
    private float actionTime = 5.0f;
    private float afterDelayTime = 0.2f;
    private float coolTime = 0.2f;

    //드론! 마지막 일격!
    private bool bFinishAttack = false;

    void Start () {
        T_mgr = GetComponent<T_Mgr>();

        //드론 위치 생성
        for (int i = 0; i < 6; i++)
        {
            drones[i] = (GameObject)Instantiate(dronePref);
            drones[i].SetActive(false);
            drones[i].GetComponent<Drone>().setSpeed(Random.Range(2.0f, 7.0f));
            drones[i].GetComponent<Drone>().setTargetPos(setPos[i].position);

        }
        base.setCoolTime(this.coolTime);
    }
	
	void Update () {
        if (T_mgr.getState() == T_Mgr.State.be_Shot)
            base.SkillCancel();

        if (!base.isCoolTime())
        {
            if (Input.GetKeyDown(KeyCode.Alpha2) && !base.isUsing())
                InputCommend(T_Mgr.SkillType.EP, iDecEP);
            if (base.isBeforeDelay())
                BeforeActionDelay(beforeDelayTime);
            if (base.isAction())
                Action(actionTime);
            if (base.isAfterDelay())
                AfterActionDelay(afterDelayTime);
        }
        else
            CoolTimeDelay();
    }

    protected override void InputCommend(T_Mgr.SkillType type, int decPoint)
    {
        base.InputCommend(type, decPoint);
    }
    protected override void BeforeActionDelay(float time)
    {
        print("선딜");
        base.BeforeActionDelay(time);
    }
    protected override void Action(float time)
    {
        print("액션");

        for (int i = 0; i < 6; i++)
        {
            drones[i].transform.position = setPos[i].position;
            drones[i].SetActive(true);
        }

        T_mgr.ChangeState(T_Mgr.State.idle);

       

        this.StartCoroutine(FollowDrone(time));
        this.StartCoroutine(EndDrone(time));

        base.setAction(false);
        //base.Action(time);
    }
    protected override void AfterActionDelay(float time)
    {
        print("후딜");

        for (int i = 0; i < 6; i++)
        {
            drones[i].SetActive(false);
        }

        base.AfterActionDelay(time);
    }
    protected override void CoolTimeDelay()
    {
        base.CoolTimeDelay();
    }

    IEnumerator FollowDrone(float time)
    {

        //회피가 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.

        float timeConut = 0;

        while (time > timeConut)
        {
            print("이동");
            for (int i = 0; i < 6; i++)
            {
                drones[i].GetComponent<Drone>().setTargetPos(setPos[i].position);
            }
            yield return new WaitForEndOfFrame();

            timeConut += Time.deltaTime;
        }               
    }

    IEnumerator EndDrone(float time)
    {
        yield return new WaitForSeconds(time);
        base.setAfterDelay(true);
    }

}
