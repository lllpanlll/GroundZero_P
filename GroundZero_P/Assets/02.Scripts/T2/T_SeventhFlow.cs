using UnityEngine;
using System.Collections;

public class T_SeventhFlow : T_SkillMgr
{
    private T_MoveCtrl t_MoveCtrl;
    private T_Mgr t_Mgr;
    private CharacterController controller;
    private T_Attack t_Attack;

    private GameObject oPlayerModel;
    public GameObject oTarget;
    private Transform trCamPivot;

    Vector3 moveDir = Vector3.zero;

    private int iDecEP = 10;

    private float beforeDelayTime = 0.0f;
    private float afterDelayTime = 0.0f;
    private float coolTime = 0.0f;

    private float blinkTime = 0.1f;
    private float blinkDist = 15.0f;
    private float blinkSpeed;

    private float[] moveFlow;
    private int iFlow = 0;

    void Start () {
        t_MoveCtrl = GetComponent<T_MoveCtrl>();
        oPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel);
        t_Mgr = GetComponent<T_Mgr>();
        controller = GetComponent<CharacterController>();
        t_Attack = GetComponent<T_Attack>();
        trCamPivot = GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform;

        blinkSpeed = blinkDist / blinkTime;

        base.SetCoolTime(this.coolTime);


        moveFlow = new float[7];
        moveFlow[0] = 270.0f;
        moveFlow[1] = 90.0f;
        moveFlow[2] = 180.0f;
        moveFlow[3] = 350.0f;
        moveFlow[4] = 210.0f;
        moveFlow[5] = 60.0f;
        moveFlow[6] = 150.0f;
    }
	
	void Update () {
        if (t_Mgr.GetState() == T_Mgr.State.be_Shot)
        {
            base.SkillCancel();
        }

        if (!base.IsCoolTime())
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
                InputCommand(T_Mgr.SkillType.EP, iDecEP);
            if (base.IsBeforeDelay())
                BeforeActionDelay(beforeDelayTime);
            if (base.IsExecute())
                Execute(0.0f);
            if (base.IsAfterDelay())
                AfterActionDelay(afterDelayTime);
        }
        else
        {
            if (base.IsRunning())
            {
                base.CoolTimeDelay();
            }
        }
    }


    protected override void InputCommand(T_Mgr.SkillType type, int decPoint)
    {
        base.InputCommand(type, decPoint);

    }
    protected override void BeforeActionDelay(float time)
    {
        print("선딜");
        t_Mgr.SetCtrlPossible(T_Mgr.CtrlPossibleIndex.MouseRot, false);

       

        base.BeforeActionDelay(time);
    }
    protected override void Execute(float time)
    {
        print("액션");
        base.SetAction(false);
               
        Transform camTr = Camera.main.transform;
        //화면의 중앙 벡터
        Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

        //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
        Ray aimRay = new Ray(centerPos, camTr.forward);

        Vector3 vTargetPos = aimRay.GetPoint(30.0f);

        oTarget.transform.position = vTargetPos;
        
        StartAction();

        
    }
    protected override void AfterActionDelay(float time)
    {
        print("후딜");
        //스킬이 끝난 직후 피격될 수 있으니 여기서부터 LayerState를 normal상태로 바꾸어 준다.
        t_Mgr.SetLayerState(T_Mgr.LayerState.normal);

        base.AfterActionDelay(time);
    }

    protected override void SkillCancel()
    {        
        base.SkillCancel();        
    }


    void StartAction()
    {        
        //회피가 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
        t_MoveCtrl.SetMoveState(T_MoveCtrl.MoveState.Stop);

        //플레이어와 캐릭터(모델)를 회전시킬 값을 구한다.
        float targetRot = transform.eulerAngles.y + moveFlow[iFlow];

        //캐릭터(모델)를 회전시킨다.
        //transform.rotation = Quaternion.Euler(0.0f, targetRot, 0.0f);
        oPlayerModel.transform.rotation = Quaternion.Euler(0.0f, targetRot, 0.0f);

        moveDir = oPlayerModel.transform.forward;
        //이동 코루틴.
        this.StartCoroutine(StartBlink(blinkTime));
    }


    IEnumerator StartBlink(float time)
    {
        oPlayerModel.transform.position = new Vector3(1000.0f, 1000.0f, 1000.0f);

        float timeConut = 0;

        while (time > timeConut)
        {
            controller.Move(moveDir * Time.deltaTime * blinkSpeed);
            yield return new WaitForEndOfFrame();

            timeConut += Time.deltaTime;
        }

        this.StartCoroutine(puseTime(0.2f));
    }

    IEnumerator puseTime(float time)
    {
        //플레이어 모델을 현재 위치로 옮긴다.
        oPlayerModel.transform.position = transform.position;
        
        //모델과 카메라 방향을 타겟 위치로 회전시킨다.
        oPlayerModel.transform.LookAt(oTarget.transform);
        trCamPivot.LookAt(oTarget.transform);
                      

        yield return new WaitForSeconds(time);
        //총알을 발사하고, 다음 이동방향 각도를 위해 iFlow를 증가시킨다.
        t_Attack.Fire();
        iFlow++;

        //iFlow가 마지막이 아니라면 다시 StartAction함수를 실행시키고 마지막이라면 Execute를 0초로 실행시켜 종료시킨다.
        if (iFlow < moveFlow.Length)
            StartAction();
        else
        {
            iFlow = 0;
            
            t_Mgr.SetCtrlPossible(T_Mgr.CtrlPossibleIndex.MouseRot, true);
            
            base.Execute(0.0f);
        }
    }
}
