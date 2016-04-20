using UnityEngine;
using System.Collections;

public class T_SeventhFlow : T_SkillMgr
{
    private T_MoveCtrl t_MoveCtrl;
    private T_Mgr t_Mgr;
    private CharacterController controller;
    private T_Attack t_Attack;
    private Animator animator;
    private Camera cam;

    private GameObject oPlayerModel;

    public GameObject oAfterModelPref;
    private GameObject oAfterModel;
    private ObjectPool afterModelPool = new ObjectPool();

    private Vector3 vTargetPos = Vector3.zero;
    public Transform trFire;
    private Transform trCamPivot;

   

    //기본 스킬 사용 파라미터
    private int iDecEP = 10;

    private float beforeDelayTime = 0.0f;
    private float afterDelayTime = 0.0f;
    private float coolTime = 0.0f;

    private float blinkTime = 0.25f;
    private float blinkDist = 8f;
    private float blinkSpeed;

    //고유 스킬 사용 파라미터
    private float fPuseTime = 0.2f;

    private float[] moveFlow;
    private int iFlow = 0;

    Vector3 moveDir = Vector3.zero;

    private float fReach = 100.0f;

    private int i = 0;

    //카메라 줌 인,아웃
    private float fTargetFOV = 80.0f;
    private float fOrizinFOV;
    private float fZoomSpeed = 25.0f;

    void Start () {
        t_MoveCtrl = GetComponent<T_MoveCtrl>();
        oPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel);
        t_Mgr = GetComponent<T_Mgr>();
        controller = GetComponent<CharacterController>();
        t_Attack = GetComponent<T_Attack>();
        trCamPivot = GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform;
        animator = GetComponentInChildren<Animator>();
        cam = Camera.main;

        fOrizinFOV = cam.fieldOfView;

        blinkSpeed = blinkDist / blinkTime;

        afterModelPool.CreatePool(oAfterModelPref, 30);

        base.SetCoolTime(this.coolTime);


        moveFlow = new float[7];
        moveFlow[0] = 180.0f;
        moveFlow[1] = 20.0f;
        moveFlow[2] = 240.0f;
        moveFlow[3] = 160.0f;
        moveFlow[4] = 10.0f;
        moveFlow[5] = 280.0f;
        moveFlow[6] = 170.0f;

        //moveFlow[0] = 90.0f;
        //moveFlow[1] = 90.0f;
        //moveFlow[2] = 90.0f;
        //moveFlow[3] = 90.0f;
        //moveFlow[4] = 90.0f;
        //moveFlow[5] = 90.0f;
        //moveFlow[6] = 90.0f;
    }
	
	void Update () {
        if (t_Mgr.GetState() == T_Mgr.State.be_Shot)
        {
            base.SkillCancel();
        }

        if (!base.IsCoolTime())
        {
            if (Input.GetKeyDown(KeyCode.Q))
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




        Transform camTr = Camera.main.transform;
        //화면의 중앙 벡터
        Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

        //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
        Ray aimRay = new Ray(centerPos, camTr.forward);
        Debug.DrawRay(centerPos, camTr.forward, Color.blue);
        Debug.DrawLine(aimRay.origin, aimRay.GetPoint(100.0f), Color.blue);
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

        //스킬이 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
        t_MoveCtrl.SetMoveState(T_MoveCtrl.MoveState.Stop);

        Transform camTr = Camera.main.transform;
        //화면의 중앙 벡터
        Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

        //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
        Ray aimRay = new Ray(centerPos, camTr.forward);

        //카메라에서 쏘는 레이가 부딪힌 위치를 바라보도록 한다.
        RaycastHit aimRayHit;
        //레이어 마스크 ignore처리 (-1)에서 빼 주어야 함
        int mask = (1 << LayerMask.NameToLayer(Layers.T_HitCollider)) | (1 << LayerMask.NameToLayer(Layers.Bullet));
        mask = ~mask;
        
        if (Physics.Raycast(aimRay, out aimRayHit, fReach, mask))
        {
            //레이에 부딪힌 오브젝트가 있으면 부딪힌 위치를 바라보도록 타겟 조정.
            vTargetPos = aimRayHit.point;
            print(aimRayHit.point);
        }
        else
        {
            //레이에 부딪힌 오브젝트가 없다면 최대 사거리 지점을 바라보도록 타겟 조정.
            vTargetPos = aimRay.GetPoint(fReach);
            print(aimRay.GetPoint(fReach));
        }       

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
        //플레이어와 캐릭터(모델)를 회전시킬 값을 구한다.
        float targetRot = cam.transform.eulerAngles.y + moveFlow[iFlow];

        //캐릭터(모델)를 회전시킨다.
        //transform.rotation = Quaternion.Euler(0.0f, targetRot, 0.0f);
        oPlayerModel.transform.rotation = Quaternion.Euler(0.0f, targetRot, 0.0f);

        moveDir = oPlayerModel.transform.forward;

        //이동시 애니메이션 플레이.
        int iSprintHash = 0;
        iSprintHash = Animator.StringToHash("T2_Sprint");
        animator.speed = 2.5f;
        animator.Play(iSprintHash);

        //이동 코루틴.
        this.StartCoroutine(StartBlink(blinkTime));

        //AfterImageDraw();
    }

    IEnumerator StartBlink(float time)
    {
        float timeConunt = 0.0f;

        bool bAfterImageOn = false;
        while (time > timeConunt)
        {
            controller.Move(moveDir * Time.deltaTime * blinkSpeed);

            if (timeConunt > (time * 0.75) && !bAfterImageOn)
            {
                bAfterImageOn = true;
                this.StartCoroutine(AfterImagesDraw());
            }

            if(iFlow == 0)
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fTargetFOV, Time.deltaTime * fZoomSpeed);
            else if (iFlow == moveFlow.Length)
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fOrizinFOV, Time.deltaTime * fZoomSpeed * 0.3f);

            yield return new WaitForEndOfFrame();

            timeConunt += Time.fixedDeltaTime;
        }

        this.StartCoroutine(puseTime(fPuseTime));
    }

    IEnumerator puseTime(float time)
    {
        //모델과 카메라 방향을 타겟 위치로 회전시킨다.
        oPlayerModel.transform.LookAt(vTargetPos);
        trCamPivot.LookAt(vTargetPos);
        trFire.LookAt(vTargetPos);
        oPlayerModel.transform.rotation = Quaternion.Euler(0.0f, oPlayerModel.transform.eulerAngles.y, 0.0f);        

        //정지시 애니메이션 플레이.
        int iSprintHash = 0;
        iSprintHash = Animator.StringToHash("T2_Idle");
        animator.speed = 1.0f;
        animator.Play(iSprintHash);

        //총알을 발사하고, 다음 이동방향 각도를 위해 iFlow를 증가시킨다.
        t_Attack.TargetFire(trFire.rotation);
        iFlow++;       

        yield return new WaitForSeconds(time);     
                
        //iFlow가 마지막이 아니라면 다시 StartAction함수를 실행시키고 마지막이라면 Execute를 0초로 실행시켜 종료시킨다.
        if (iFlow < moveFlow.Length)
        {            
            StartAction();
        }
        else
        {
            iFlow = 0;
            t_Mgr.SetCtrlPossible(T_Mgr.CtrlPossibleIndex.MouseRot, true);
            base.Execute(0.0f);
        }
    }

    IEnumerator AfterImagesDraw()
    {
        float freezeTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        oAfterModel = afterModelPool.UseObject();
        oAfterModel.transform.position = oPlayerModel.transform.position;
        oAfterModel.transform.rotation = oPlayerModel.transform.rotation;

        oAfterModel.GetComponent<Animator>().Play(stateInfo.fullPathHash, 0, freezeTime);

        this.StartCoroutine(AfterImageStopDelay(oAfterModel));

        yield return new WaitForSeconds(0.002f);
        if (i < 4)
        {
            i++;
            this.StartCoroutine(AfterImagesDraw());
        }
        else
            i = 0;
    }

    IEnumerator AfterImageStopDelay(GameObject obj)
    {
        yield return new WaitForEndOfFrame();
        obj.GetComponent<Animator>().Stop();
    }



}
