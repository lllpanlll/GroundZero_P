using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class T_Snipe : T_SkillMgr {

    public GameObject bulletPrefeb;
    public List<GameObject> bulletPool = new List<GameObject>();

    private T_Mgr T_mgr;
    private RotationPivotOfCam rotationPivotOfCam;
    private FollowCam followCam;
    private SnipeModeRotation snipeModeRotation;

    private Transform zoomTr;
    private Transform camTr;
    private GameObject playerModel;

    private int iDecAP = 10;

    private bool bZoom = false;
    private float fZoomInFOV = 30.0f;
    private float fOriginFOV = 60.0f;
        
    private float fReach = 1000.0f;
    //private bool bDelay = false;
    //private float delayTime = 0.5f, delayTimer = 0.0f;

    private float beforeDelayTime = 0.0f;
    private float actionTime = 100.0f;
    private float afterDelayTime = 1.2f;
    private float coolTime = 0.2f;

    void Start () {
        T_mgr = GetComponent<T_Mgr>();
        rotationPivotOfCam = GetComponentInChildren<RotationPivotOfCam>();
        playerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel);
        followCam = Camera.main.GetComponent<FollowCam>();
        snipeModeRotation = Camera.main.GetComponent<SnipeModeRotation>();

        snipeModeRotation.enabled = false;

        camTr = Camera.main.transform;
        zoomTr = GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform;

        for (int i = 0; i < 5; i++)
        {
            GameObject bullet = (GameObject)Instantiate(bulletPrefeb);

            bullet.name = "SnipeBullet_" + i.ToString();
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }
	
	void Update () {

        ////화면의 중앙 벡터
        //Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        ////화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
        //Ray fireRay = new Ray(centerPos, camTr.forward);

        //if (Input.GetMouseButtonDown(1) && T_mgr.getCtrlPossible().Skill == true)
        //{
        //    if (T_mgr.getAP() < iDecAP)
        //        return;

        //    T_mgr.ChangeState(T_Mgr.State.Skill);
        //    T_mgr.setAP(iDecAP);

        //    followCam.enabled = false;
        //    rotationPivotOfCam.enabled = false;

        //    //카메라 이동 및 회전.
        //    camTr.position = zoomTr.position + (zoomTr.right * followCam.getRight());
        //    camTr.LookAt(fireRay.GetPoint(100.0f));                       

        //    bZoom = true;

        //    Camera.main.fieldOfView = fZoomInFOV;
        //    playerModel.SetActive(false);
        //}

        //if (bZoom)
        //{
        //    snipeModeRotation.enabled = true;

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        bZoom = false;

        //        Camera.main.fieldOfView = fOriginFOV;

        //        //먼저 저격 중 카메라 회전 스크립트 종료.
        //        snipeModeRotation.enabled = false;  
        //        //다음으로 카메라 Pivot을 저격으로 조준했던 방향으로 조정.              
        //        //rotationPivotOfCam.TargetLookat(snipeModeRotation.getLastShotPos());
        //        rotationPivotOfCam.transform.LookAt(snipeModeRotation.getLastShotPos());
        //        //Pivot스크립트 재활성화.(카메라 회전 역할)
        //        rotationPivotOfCam.enabled = true;
        //        //followCam스크립트 재활성화.
        //        followCam.enabled = true;
        //        //발사 방향으로 캐릭터(모델) 방향 조정.
        //        playerModel.SetActive(true);
        //        playerModel.transform.rotation = Quaternion.Euler(0.0f, camTr.eulerAngles.y, 0.0f);

        //        //카메라에서 쏘는 레이가 부딪힌 위치에 플레이어의 총알이 발사되는 각도를 조정한다.
        //        RaycastHit aimRayHit;
        //        if (Physics.Raycast(fireRay, out aimRayHit, fReach))
        //        {
        //            //데미지 계산 코드 작성 위치
        //            if (aimRayHit.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
        //            {
        //                aimRayHit.collider.gameObject.GetComponent<MonsterHitCtrl>().OnHitMonster(10);
        //                GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().stiffValue += 70; ;
        //            }
        //        }
        //        bDelay = true;

        //        #region<투사체>
        //        //투사체 오브젝트 풀 생성.
        //        foreach (GameObject bullet in bulletPool)
        //        {
        //            if (!bullet.activeSelf)
        //            {
        //                bullet.transform.position = zoomTr.position;
        //                bullet.transform.rotation = camTr.rotation;
        //                bullet.SetActive(true);
        //                break;
        //            }
        //        }
        //        #endregion
        //    }
        //}

        //if(bDelay)
        //{
        //    if(T_mgr.getState() == T_Mgr.State.Skill)
        //    {
        //        if (delayTimer > delayTime)
        //        {
        //            //좀더 나중 타이밍으로 옮겨야 할 듯.
        //            T_mgr.ChangeState(T_Mgr.State.idle);
        //            delayTimer = 0.0f;
        //            bDelay = false;
        //        }
        //        else
        //            delayTimer += Time.deltaTime;
        //    }
        //}

        if (T_mgr.getState() == T_Mgr.State.be_Shot)
        {
            base.SkillCancel();
        }

        if (Input.GetMouseButtonDown(0))
            InputCommend(T_Mgr.SkillType.AP, iDecAP);
        if (base.isBeforeDelay())
            BeforeActionDelay(beforeDelayTime);
        if (base.isAction())
            Action(actionTime);
        if (base.isAfterDelay())
            AfterActionDelay(afterDelayTime);

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
        T_mgr.setLayerState(T_Mgr.LayerState.invincibility);






        base.Action(time);
    }
    protected override void AfterActionDelay(float time)
    {
        print("후딜");
        //스킬이 끝난 직후 피격될 수 있으니 여기서부터 LayerState를 normal상태로 바꾸어 준다.

        base.AfterActionDelay(time);
    }
}
