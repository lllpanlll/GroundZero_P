using UnityEngine;
using System.Collections;

public class testSkill : T_SkillMgr {
    private T_MoveCtrl t_MoveCtrl;
    private T_Mgr t_Mgr;
    private CharacterController controller;

    private GameObject playerModel;
    public GameObject blinkEffect;

    Vector3 moveDir = Vector3.zero;

    private int iDecEP = 10;

    private float beforeDelayTime = 0.0f;
    private float afterDelayTime = 0.0f;
    private float coolTime = 0.0f;

    private float blinkTime = 0.2f;
    private float blinkDist = 10.0f;
    private float blinkSpeed;

    void Start () {
        t_MoveCtrl = GetComponent<T_MoveCtrl>();
        playerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel);
        t_Mgr = GetComponent<T_Mgr>();
        controller = GetComponent<CharacterController>();

        blinkSpeed = blinkDist / blinkTime;

        base.SetCoolTime(this.coolTime);
    }
	
	void Update () {
        if(t_Mgr.GetState() == T_Mgr.State.be_Shot)
        {
            base.SkillCancel();
        }

        if (!base.IsCoolTime())
        {
            if (Input.GetKeyDown(KeyCode.Space))
                InputCommand(T_Mgr.SkillType.EP, iDecEP);
            if (base.IsBeforeDelay())
                BeforeActionDelay(beforeDelayTime);
            if (base.IsExecute())
                Execute(blinkTime);
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
        base.BeforeActionDelay(time);       
    }
    protected override void Execute(float time)
    {
        print("액션");
        t_Mgr.SetLayerState(T_Mgr.LayerState.invincibility);

        //플레이어와 캐릭터(모델)를 회전시킬 값을 구한다.
        float targetRot = t_MoveCtrl.GetTargetRot();

        //가만히 있는 경우, 뒤로 이동시키기 위해 targetRot값을 바꾼다.
        if (t_MoveCtrl.GetMoveState() == T_MoveCtrl.MoveState.Stop)
        {
            print("뒤로!");
            float CamRot = Camera.main.transform.eulerAngles.y;
            //플레이어와 캐릭터 모델링을 뒤로 돌린다.
            targetRot = CamRot + 180.0f;            
        }

        //플레이어와 캐릭터(모델)를 회전시킨다.
        transform.rotation = Quaternion.Euler(0.0f, targetRot, 0.0f);
        playerModel.transform.rotation = transform.rotation;

        moveDir = transform.forward;
        //이동 코루틴.
        this.StartCoroutine(StartBlink(time));

        base.Execute(time);
    }
    protected override void AfterActionDelay(float time)
    {
        print("후딜");
        playerModel.transform.position = transform.position;
        blinkEffect.SetActive(false);
        //스킬이 끝난 직후 피격될 수 있으니 여기서부터 LayerState를 normal상태로 바꾸어 준다.
        t_Mgr.SetLayerState(T_Mgr.LayerState.normal);

        base.AfterActionDelay(time);
    }

    protected override void SkillCancel()
    {
        base.SkillCancel();
    }

    IEnumerator StartBlink(float time)
    {
        playerModel.transform.position = new Vector3(1000.0f, 1000.0f, 1000.0f);
        blinkEffect.SetActive(true);

        //회피가 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
        t_MoveCtrl.SetMoveState(T_MoveCtrl.MoveState.Stop);

        float timeConut = 0;
        
        while(time > timeConut)
        {
            //transform.Translate(transform.forward * Time.deltaTime * blinkSpeed, Space.World);
            
            //moveDir.y -= 20.0f * Time.deltaTime;
            controller.Move(moveDir * Time.deltaTime * blinkSpeed);
            yield return new WaitForEndOfFrame();

            timeConut += Time.deltaTime;
        }
    }
}
