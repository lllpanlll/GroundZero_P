using UnityEngine;
using System.Collections;

public class T_Blink : MonoBehaviour {

    private T_MoveCtrl T_moveCtrl;  //이동상태 변경을 위함
    private T_Mgr T_mgr;
    private GameObject playerModel;  //플레이어의 모습을 사라지게 하기 위함

    public GameObject blinkEffect; //이동중 나타나는 이펙트

    private bool bAction;
    private bool bDelay;

    private float blinkMoveTimer;
    private float blinkMoveTime = 0.5f;
    private float delayTime = 0.0f;
    private float delayTimer;
    private float fDist = 10.0f;
    private float fSpeed;
    private int iDecEP = 10;
    
    private float targetRot;

    void Start () {
        T_moveCtrl = GetComponent<T_MoveCtrl>();
        T_mgr = GetComponent<T_Mgr>();
        playerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel);


        blinkEffect.SetActive(false);

        bAction = false;
        bDelay = false;
        blinkMoveTimer = 0.0f;
        delayTimer = 0.0f;

        fSpeed = fDist/blinkMoveTime;
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space) && T_mgr.getCtrlPossible().Skill == true)
        {           
            if (!bAction)
            {
                if (T_mgr.getEP() <= 0.0f || T_mgr.getEP() < iDecEP)
                {
                    print("EP가 부족합니다.");
                    return;
                }
                else
                {                   
                    //캐릭터 상태와 레이어 상태를 각각 스킬 상태, 무적 상태로 만든다.
                    T_mgr.ChangeState(T_Mgr.State.Skill);
                    T_mgr.setLayerState(T_Mgr.LayerState.invincibility);

                    T_mgr.DecreaseSkillPoint(T_Mgr.SkillType.EP, iDecEP);

                    Quaternion rotation = Quaternion.identity;
                    //플레이어와 캐릭터(모델)를 회전시킬 값을 구한다.
                    targetRot = T_moveCtrl.getTargetRot();

                    //가만히 있는 경우, 뒤로 이동시키기 위해 targetRot값을 바꾼다.
                    if (T_moveCtrl.getMoveState() == T_MoveCtrl.MoveState.Stop)
                    {
                        float CamRot = Camera.main.transform.eulerAngles.y;
                        //플레이어와 캐릭터 모델링을 뒤로 돌린다.
                        targetRot = CamRot + 180.0f;
                    }

                    //플레이어와 캐릭터(모델)를 회전시킨다.
                    rotation.eulerAngles = new Vector3(0.0f, targetRot, 0.0f);
                    transform.rotation = rotation;
                    playerModel.transform.rotation = rotation;

                    //회피가 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
                    T_moveCtrl.setMoveState(T_MoveCtrl.MoveState.Stop);

                    bAction = true;
                }
            }
        }

        if (bAction)
        {            
            if (blinkMoveTimer < blinkMoveTime)
            {
                transform.Translate(transform.forward * Time.deltaTime * fSpeed, Space.World);

                blinkEffect.SetActive(true);
                //playerModel.SetActive(false);
                playerModel.transform.position = new Vector3(1000.0f, 1000.0f, 1000.0f);

                blinkMoveTimer += Time.deltaTime;
            }
            else
            {
                ////이동 완료 후, 전방으로 캐릭터(모델)회전.
                //characterRotation.setRot(Camera.main.transform.eulerAngles.y);

                //이동 완료 후, 딜레이 시작 지점.
                blinkEffect.SetActive(false);
                //playerModel.SetActive(true);
                playerModel.transform.position = transform.position;

                //blinkTime동안의 이동이 끝나면 delayTime으로 넘겨준다.
                bDelay = true;

                //스킬이 끝난 직후 피격될 수 있으니 여기서부터 LayerState를 normal로 바꾸어 준다.
                T_mgr.setLayerState(T_Mgr.LayerState.normal);
                
            }
        }

        //스킬 무적판정이 끝나고, 후딜레이가 끝나기 전까지의 시간동안 피격이 된다면 후딜레이 무시하고 return
        if(T_mgr.getState() == T_Mgr.State.be_Shot)
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

                T_mgr.ChangeState(T_Mgr.State.idle);

            }
            else
            {
                

                delayTimer += Time.deltaTime;
            }
        }
    }
}
