using UnityEngine;
using System.Collections;

public class T_BlinkCounter : T_SkillMgr {
    private T_MoveCtrl T_moveCtrl;  //이동상태 변경을 위함
    private T_Mgr T_mgr;

    private GameObject playerModel;  //플레이어의 모습을 사라지게 하기 위함

    private int iDecEP = 10;

    private float beforeDelayTime = 0.0f;
    private float afterDelayTime = 0.2f;
    private float coolTime = 0.2f;

    private float blinkTime = 0.2f;
    private float blinkDist = 15.0f;
    private float blinkSpeed;

    public GameObject grenade;

    void Start () {
        T_moveCtrl = GetComponent<T_MoveCtrl>();
        T_mgr = GetComponent<T_Mgr>();
        playerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel);

        blinkSpeed = blinkDist / blinkTime;

        grenade = (GameObject)Instantiate(grenade);
        grenade.SetActive(false);

        base.setCoolTime(this.coolTime);
    }

    void Update ()
    {
        if (T_mgr.getState() == T_Mgr.State.be_Shot)
            base.SkillCancel();

        if (!base.isCoolTime())
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && !base.isUsing())
                InputCommend(T_Mgr.SkillType.EP, iDecEP);
            if (base.isBeforeDelay())
                BeforeActionDelay(beforeDelayTime);
            if (base.isAction())
                Action(blinkTime);
            if (base.isAfterDelay())
                AfterActionDelay(afterDelayTime);
        }
        else
        {
            CoolTimeDelay();
        }
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
                
        float CamRot = Camera.main.transform.eulerAngles.y;
        //플레이어와 캐릭터 모델링을 뒤로 돌린다.
        float targetRot = CamRot + 180.0f;

        //플레이어와 캐릭터(모델)를 회전시킨다.
        transform.rotation = Quaternion.Euler(0.0f, targetRot, 0.0f);
        playerModel.transform.rotation = transform.rotation;

        //수류탄 처리
        grenade.transform.position = transform.position + (transform.up * 2.5f);
        grenade.SetActive(true);

        //회피가 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
        T_moveCtrl.setMoveState(T_MoveCtrl.MoveState.Stop);

        //이동 코루틴.
        this.StartCoroutine(StartBlinkCounter(time));

        base.Action(time);
    }
    protected override void AfterActionDelay(float time)
    {
        print("후딜");
        playerModel.transform.position = transform.position;

        base.AfterActionDelay(time);
    }
    protected override void CoolTimeDelay()
    {
        base.CoolTimeDelay();
    }

    IEnumerator StartBlinkCounter(float time)
    {
        playerModel.transform.position = new Vector3(1000.0f, 1000.0f, 1000.0f);

        //회피가 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
        T_moveCtrl.setMoveState(T_MoveCtrl.MoveState.Stop);

        float timeConut = 0;

        while (time > timeConut)
        {
            print("이동");
            transform.Translate(transform.forward * Time.deltaTime * blinkSpeed, Space.World);

            yield return new WaitForEndOfFrame();

            timeConut += Time.deltaTime;
        }
    }
}
