using UnityEngine;
using System.Collections;

public class T_Blink : MonoBehaviour {

    private T_MoveCtrl t_MoveCtrl;  //이동상태 변경을 위함
    private T_Mgr t_Mgr;
    private GameObject oPlayerModel;  //플레이어의 모습을 사라지게 하기 위함

    public GameObject oBlinkEffect; //이동중 나타나는 이펙트

    private bool bAction;
    private bool bDelay;

    private float blinkMoveTimer;
    private float blinkMoveTime = 0.5f;
    private float delayTime = 0.0f;
    private float delayTimer;
    private float fDist = 10.0f;
    private float fSpeed;
    private int iDecEP = 10;
    
    private float fTargetRot;

    void Start () {
        t_MoveCtrl = GetComponent<T_MoveCtrl>();
        t_Mgr = GetComponent<T_Mgr>();
        oPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel);


        oBlinkEffect.SetActive(false);

        bAction = false;
        bDelay = false;
        blinkMoveTimer = 0.0f;
        delayTimer = 0.0f;

        fSpeed = fDist/blinkMoveTime;
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space) && t_Mgr.GetCtrlPossible().Skill == true)
        {           
            if (!bAction)
            {
                if (t_Mgr.GetEP() <= 0.0f || t_Mgr.GetEP() < iDecEP)
                {
                    print("EP가 부족합니다.");
                    return;
                }
                else
                {                   
                    //캐릭터 상태와 레이어 상태를 각각 스킬 상태, 무적 상태로 만든다.
                    t_Mgr.ChangeState(T_Mgr.State.Skill);
                    t_Mgr.SetLayerState(T_Mgr.LayerState.invincibility);

                    t_Mgr.DecreaseSkillPoint(T_Mgr.SkillType.EP, iDecEP);

                    Quaternion rotation = Quaternion.identity;
                    //플레이어와 캐릭터(모델)를 회전시킬 값을 구한다.
                    fTargetRot = t_MoveCtrl.GetTargetRot();

                    //가만히 있는 경우, 뒤로 이동시키기 위해 fTargetRot값을 바꾼다.
                    if (t_MoveCtrl.GetMoveState() == T_MoveCtrl.MoveState.Stop)
                    {
                        float CamRot = Camera.main.transform.eulerAngles.y;
                        //플레이어와 캐릭터 모델링을 뒤로 돌린다.
                        fTargetRot = CamRot + 180.0f;
                    }

                    //플레이어와 캐릭터(모델)를 회전시킨다.
                    rotation.eulerAngles = new Vector3(0.0f, fTargetRot, 0.0f);
                    transform.rotation = rotation;
                    oPlayerModel.transform.rotation = rotation;

                    //회피가 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
                    t_MoveCtrl.SetMoveState(T_MoveCtrl.MoveState.Stop);

                    bAction = true;
                }
            }
        }

        if (bAction)
        {            
            if (blinkMoveTimer < blinkMoveTime)
            {
                transform.Translate(transform.forward * Time.deltaTime * fSpeed, Space.World);

                oBlinkEffect.SetActive(true);
                //oPlayerModel.SetActive(false);
                oPlayerModel.transform.position = new Vector3(1000.0f, 1000.0f, 1000.0f);

                blinkMoveTimer += Time.deltaTime;
            }
            else
            {
                ////이동 완료 후, 전방으로 캐릭터(모델)회전.
                //characterRotation.setRot(Camera.main.transform.eulerAngles.y);

                //이동 완료 후, 딜레이 시작 지점.
                oBlinkEffect.SetActive(false);
                //oPlayerModel.SetActive(true);
                oPlayerModel.transform.position = transform.position;

                //blinkTime동안의 이동이 끝나면 delayTime으로 넘겨준다.
                bDelay = true;

                //스킬이 끝난 직후 피격될 수 있으니 여기서부터 LayerState를 normal로 바꾸어 준다.
                t_Mgr.SetLayerState(T_Mgr.LayerState.normal);
                
            }
        }

        //스킬 무적판정이 끝나고, 후딜레이가 끝나기 전까지의 시간동안 피격이 된다면 후딜레이 무시하고 return
        if(t_Mgr.GetState() == T_Mgr.State.be_Shot)
        {           
            return;
        }

        if(bDelay)
        {
            if(delayTimer > delayTime)
            {
                //후 딜레이 종료.
                delayTimer = 0.0f;
                bDelay = false;

                //후 딜레이 뒤에 쿨타임 없이 바로 스킬을 사용 할 수 있다.
                blinkMoveTimer = 0.0f;
                bAction = false;

                t_Mgr.ChangeState(T_Mgr.State.idle);

            }
            else
            {
                

                delayTimer += Time.deltaTime;
            }
        }
    }
}
