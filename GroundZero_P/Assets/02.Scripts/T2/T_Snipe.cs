using UnityEngine;
using System.Collections;

public class T_Snipe : T_SkillMgr {

    public GameObject bulletPrefeb;
    private GameObject bullet;
    private ObjectPool bulletPool = new ObjectPool();

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

    private float beforeDelayTime = 1.0f;
    private float actionTime = 100.0f;
    private float afterDelayTime = 1.0f;
    private float coolTime = 0.2f;

    private Ray fireRay;

    void Start () {
        T_mgr = GetComponent<T_Mgr>();
        rotationPivotOfCam = GetComponentInChildren<RotationPivotOfCam>();
        playerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel);
        followCam = Camera.main.GetComponent<FollowCam>();
        snipeModeRotation = Camera.main.GetComponent<SnipeModeRotation>();

        snipeModeRotation.enabled = false;

        camTr = Camera.main.transform;
        zoomTr = GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform;

        bulletPool.CreatePool(bulletPrefeb, 5);

        base.setCoolTime(coolTime);
    }
	
	void Update () {

        //화면의 중앙 벡터
        Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
        fireRay = new Ray(centerPos, camTr.forward);

        if (T_mgr.getState() == T_Mgr.State.be_Shot)
            base.SkillCancel();

        if (!base.isCoolTime())
        {
            if (Input.GetMouseButtonDown(1) && !base.isUsing())
                InputCommend(T_Mgr.SkillType.AP, iDecAP);
            if (base.isBeforeDelay())
                BeforeActionDelay(beforeDelayTime);
            if (base.isAction())
                Action(actionTime);
            if (base.isAfterDelay())
                AfterActionDelay(afterDelayTime);
        }
        else
        {
            CoolTimeDelay();
        }

        if (bZoom)
        {
            if (Input.GetMouseButtonDown(0))
            {
                bZoom = false;

                Camera.main.fieldOfView = fOriginFOV;

                //먼저 저격 중 카메라 회전 스크립트 종료.
                snipeModeRotation.enabled = false;
                //다음으로 카메라 Pivot을 저격으로 조준했던 방향으로 조정.              
                //rotationPivotOfCam.TargetLookat(snipeModeRotation.getLastShotPos());
                rotationPivotOfCam.transform.LookAt(snipeModeRotation.getLastShotPos());
                //Pivot스크립트 재활성화.(카메라 회전 역할)
                rotationPivotOfCam.enabled = true;
                //followCam스크립트 재활성화.
                followCam.enabled = true;
                //발사 방향으로 캐릭터(모델) 방향 조정.
                playerModel.SetActive(true);
                playerModel.transform.rotation = Quaternion.Euler(0.0f, camTr.eulerAngles.y, 0.0f);

                //카메라에서 쏘는 레이가 부딪힌 위치에 플레이어의 총알이 발사되는 각도를 조정한다.
                RaycastHit aimRayHit;
                if (Physics.Raycast(fireRay, out aimRayHit, fReach))
                {
                    //데미지 계산 코드 작성 위치
                    if (aimRayHit.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
                    {
                        aimRayHit.collider.gameObject.GetComponent<MonsterHitCtrl>().OnHitMonster(10);
                        GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().stiffValue += 70; ;
                    }
                }
                base.EndAction();

                #region<투사체>
                bullet = bulletPool.UseObject();
                bullet.transform.position = zoomTr.position;
                bullet.transform.rotation = camTr.rotation;
                bullet.SetActive(true);
                #endregion
            }
        }

    }

    protected override void InputCommend(T_Mgr.SkillType type, int decPoint)
    {
        base.InputCommend(type, decPoint);
    }
    protected override void BeforeActionDelay(float time)
    {
        print("선딜");
        T_mgr.setCtrlPossible(T_Mgr.CtrlPossibleIndex.MouseRot, false);
        base.BeforeActionDelay(time);
    }
    protected override void Action(float time)
    {
        print("액션");
       

        followCam.enabled = false;
        rotationPivotOfCam.enabled = false;

        //카메라 이동 및 회전.
        camTr.position = zoomTr.position + (zoomTr.right * followCam.getRight());
        camTr.LookAt(fireRay.GetPoint(100.0f));

        bZoom = true;

        Camera.main.fieldOfView = fZoomInFOV;
        playerModel.SetActive(false);

        snipeModeRotation.enabled = true;

        //base.Action(time);
    }
    protected override void AfterActionDelay(float time)
    {
        print("후딜");        

        base.AfterActionDelay(time);
    }
    protected override void CoolTimeDelay()
    {
        T_mgr.setCtrlPossible(T_Mgr.CtrlPossibleIndex.MouseRot, true);
        base.CoolTimeDelay();
    }
}
