﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class T_Attack : MonoBehaviour
{
    public GameObject oBulletPref;
    private GameObject oBullet;
    private ObjectPool bulletPool = new ObjectPool();

    public GameObject oFlarePref;
    private GameObject oFlare;
    private ObjectPool flarePool = new ObjectPool();
    
    public MeshRenderer muzzleFlash;

    public Transform trFire;

    private T_MoveCtrl t_MoveCtrl;  //이동상태 변경을 위함.
    private T_Mgr t_Mgr;
    private Transform trPlayerModel;

    //타이머 변수
    private bool bFire = false;
    private float attackTimer = 0.0f;
    private float attackTime = 0.3f;

    private float fReach = 1000.0f;

    //연사속도 타이머
    private float fRpmSpeed = 0.1f;
    private float fRpmTime;
    private float fRpmTimer = 0.0f;

    //RapidMode 타이머
    private bool bRapidMode = false;
    private float fRapidTime = 2.0f;
    private float fRapidTimer = 0.0f;

    //카메라 줌인
    private Camera cam;
    private FollowCam followCam;
    private float fCamDist = 0.0f;
    private float fOrizinDist;
    public float fTargetDist = 1.0f;
    public float fZoomSpeed = 20.0f;
    
    private int iMaxMagazine;
    //집탄율..?
    private float fAccuracy = 0.1f;
    private bool bFirstShot = true;

    void Start()
    {       
        iMaxMagazine = T_Stat.MAX_MAGAZINE;

        t_MoveCtrl = GetComponent<T_MoveCtrl>();
        t_Mgr = GetComponent<T_Mgr>();
        trPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel).transform;
        cam = Camera.main;
        followCam = cam.GetComponent<FollowCam>();
        fOrizinDist = followCam.GetDist();
        fCamDist = followCam.GetDist();

        bulletPool.CreatePool(oBulletPref, iMaxMagazine);
        flarePool.CreatePool(oFlarePref, iMaxMagazine * 2);

        muzzleFlash.enabled = false;
    }

    void Update()
    {
        if (t_Mgr.GetCtrlPossible().Attack == false)
        {
            bFire = false;
            attackTimer = 0.0f;
            return;
        }else if (Input.GetMouseButton(0))
        {

            //플레이어를 정면을 바라보게 한다.
            float CamRot = Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0.0f, CamRot+15.0f, 0.0f);
            trPlayerModel.rotation = transform.rotation;

            //첫 공격시에는 바로 공격 가능하고, 그 다음부터 연사 속도 적용.
            //if (!bFire)
            if(bFirstShot || !bFire)
                fRpmTime = 0.0f;
            else
                fRpmTime = fRpmSpeed;

            bFirstShot = false;

            //연사속도 조절.
            if (fRpmTimer > fRpmTime)
            {
                Fire();
                bFire = true;
                fRpmTimer = 0.0f;
            }
            else
                fRpmTimer += Time.deltaTime;
        }

        if(Input.GetMouseButtonUp(0))
        {
            fRpmTime = 0.0f;
            //bFire = false;
            bFirstShot = true;
            attackTimer = 0.0f;
        }
        
        //공격하면 플레이어 이동상태를 일정 시간 동안 변경.
        if (bFire)
        {
            t_Mgr.ChangeState(T_Mgr.State.attack);
            if (attackTimer > attackTime)
            {
                bFire = false;
                attackTimer = 0.0f;

                //어택타임이 지난 공격종료 시점.
                bRapidMode = false;
                fRapidTimer = 0.0f;

                fRpmTimer = 0.0f;
                fRpmTime = fRpmSpeed;

                t_Mgr.ChangeState(T_Mgr.State.idle);                
            }
            else
            {
                attackTimer += Time.deltaTime;
            }

            //카메라 줌인
            fCamDist = Mathf.Lerp(fCamDist, fTargetDist, Time.deltaTime * fZoomSpeed);
            followCam.SetDist(fCamDist);

        }
        else
        {
            //카메라 줌아웃
            fCamDist = Mathf.Lerp(fCamDist, followCam.GetZoom(), Time.deltaTime * fZoomSpeed * 0.2f); //check
            followCam.SetDist(fCamDist);
        }

        #region<RapidMode>
        ////캐릭터가 멈춰있고, 기본사격중일 경우.
        //if (t_MoveCtrl.GetMoveState() == T_MoveCtrl.MoveState.Stop && bFire)
        //{
        //    if (fRapidTimer > fRapidTime)
        //    {
        //        print("rapid");
        //        bRapidMode = true;
        //        fRapidTimer = 0.0f;
        //    }
        //    else
        //    {
        //        fRapidTimer += Time.deltaTime;
        //    }
        //}
        ////캐릭터가 stop상태가 아니게 되면 다시 일반 모드로 돌아가도록.
        //else
        //    bRapidMode = false;

        //if (bRapidMode)
        //{
        //    fRpmSpeed = 0.05f;
        //}
        //else
        //    fRpmSpeed = 0.4f;
        #endregion
    }
    
    public void Fire()
    {
        //bFire = true;
        attackTimer = 0.0f;        

        Transform camTr = Camera.main.transform;
        //화면의 중앙 벡터
        Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

        //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
        Ray aimRay = new Ray(centerPos, camTr.forward);
        Debug.DrawLine(aimRay.origin, aimRay.direction, Color.blue);
        
        //카메라에서 쏘는 레이가 부딪힌 위치에 플레이어의 총알이 발사되는 각도를 조정한다.
        RaycastHit aimRayHit;
        //레이어 마스크 ignore처리 (-1)에서 빼 주어야 함
        int mask = (1 << LayerMask.NameToLayer(Layers.T_HitCollider)) | (1 << LayerMask.NameToLayer(Layers.Bullet));
        mask = ~mask;

        if (Physics.Raycast(aimRay, out aimRayHit, fReach, mask) )
        {
            //aimRayHit.point와 플레이어 포지션 위치의 거리.(사정거리 체크)
            float fRangeCheck = Vector3.Distance(transform.position, aimRayHit.point);
            
            //거리에 따라 명중률 조정.
            fAccuracy = 0.1f + (fRangeCheck * 0.02f);
            Vector3 fTarget = aimRayHit.point;
            fTarget = new Vector3(fTarget.x,
                                  Random.Range(fTarget.y - fAccuracy, fTarget.y + fAccuracy),
                                  Random.Range(fTarget.z - fAccuracy, fTarget.z + fAccuracy));

            //레이에 부딪힌 오브젝트가 있으면 부딪힌 위치를 바라보도록 방향 조정.
            trFire.transform.LookAt(fTarget);

           //aimRayHit.point가 사정거리 이내에 위치할 경우.
            if (fRangeCheck < fReach)
            {
                ////데미지 계산 코드 작성 위치

            }
        }
        else
        {
            //최대거리 명중률 조정.
            fAccuracy = 20.0f;
            Vector3 fTarget = aimRay.GetPoint(fReach);
            fTarget = new Vector3(fTarget.x,
                                  Random.Range(fTarget.y - fAccuracy, fTarget.y + fAccuracy),
                                  Random.Range(fTarget.z - fAccuracy, fTarget.z + fAccuracy));
            //trFire.GetComponent<TargetLookAt>().TargetLookat(fTarget);
            trFire.LookAt(fTarget);
        }

        //투사체 오브젝트 풀 생성.    
        oBullet = bulletPool.UseObject();
        oBullet.transform.position = trFire.position;
        oBullet.transform.rotation = trFire.rotation;

        //머즐플래시
        this.StartCoroutine(ShowMuzzleFlash());

    }

    public void TargetFire(Quaternion rot)
    {
        trFire.rotation = rot;

        //투사체 오브젝트 풀 생성.    
        oBullet = bulletPool.UseObject();
        oBullet.transform.position = trFire.position;
        oBullet.transform.rotation = trFire.rotation;

        //머즐플래시
        this.StartCoroutine(ShowMuzzleFlash());
    }

    public bool isFire() { return bFire; }
    public float GetReach() { return fReach; }
    public void SetFlareEffect(Vector3 pos)
    {
        oFlare = flarePool.UseObject();
        oFlare.transform.position = pos;
    }

    IEnumerator ShowMuzzleFlash()
    {
        float scale = Random.Range(1.0f, 1.5f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        muzzleFlash.enabled = true;

        yield return new WaitForSeconds(Random.Range(0.05f, 0.008f));

        muzzleFlash.enabled = false;
    }
}
