using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class T_Attack : MonoBehaviour
{
    public GameObject bulletPref;
    private GameObject bullet;
    private ObjectPool bulletPool = new ObjectPool();

    public GameObject flarePref;
    private GameObject flare;
    private ObjectPool flarePool = new ObjectPool();

    public Transform firePos;

    private T_MoveCtrl T_moveCtrl;  //이동상태 변경을 위함.
    private T_Mgr T_mgr;
    private Transform playerModel;

    //타이머 변수
    private bool bFire = false;
    private float attackTimer = 0.0f;
    private float attackTime = 0.5f;

    private float fReach = 1000.0f;

    //연사속도 타이머
    private float rpmSpeed = 0.4f;
    private float rpmTime = 0.2f;
    private float rpmTimer = 0.0f;

    //RapidMode 타이머
    private bool bRapidMode = false;
    private float rapidTime = 2.0f;
    private float rapidTimer = 0.0f;

    private int maxMagazine;

    private float accuracy = 0.1f;

    void Start()
    {       
        maxMagazine = T_Stat.MAX_MAGAZINE;

        T_moveCtrl = GetComponent<T_MoveCtrl>();
        T_mgr = GetComponent<T_Mgr>();
        playerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel).transform;

        bulletPool.CreatePool(bulletPref, maxMagazine);
        flarePool.CreatePool(flarePref, maxMagazine);

    }

    void Update()
    {
        if (T_mgr.getCtrlPossible().Attack == false)
        {
            bFire = false;
            return;
        }else if (Input.GetMouseButton(0))
        {

            //플레이어를 정면을 바라보게 한다.
            float CamRot = Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0.0f, CamRot, 0.0f);
            playerModel.rotation = transform.rotation;

            //첫 공격시에는 바로 공격 가능하고, 그 다음부터 연사 속도 적용.
            if (!bFire)
                rpmTime = 0.0f;
            else
                rpmTime = rpmSpeed;

            //연사속도 조절.
            if (rpmTimer > rpmTime)
            {
                Fire();
                rpmTimer = 0.0f;
            }
            else
                rpmTimer += Time.deltaTime;
        }
        
        //공격하면 플레이어 이동상태를 일정 시간 동안 변경.
        if (bFire)
        {
            T_mgr.ChangeState(T_Mgr.State.attack);
            if (attackTimer > attackTime)
            {
                bFire = false;
                attackTimer = 0.0f;

                //어택타임이 지난 공격종료 시점.
                bRapidMode = false;
                rapidTimer = 0.0f;

                rpmTimer = 0.0f;
                rpmTime = rpmSpeed;

                T_mgr.ChangeState(T_Mgr.State.idle);                
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }

        #region<RapidMode>
        //캐릭터가 멈춰있고, 기본사격중일 경우.
        if (T_moveCtrl.getMoveState() == T_MoveCtrl.MoveState.Stop && bFire)
        {
            if (rapidTimer > rapidTime)
            {
                print("rapid");
                bRapidMode = true;
                rapidTimer = 0.0f;
            }
            else
            {
                rapidTimer += Time.deltaTime;
            }
        }
        //캐릭터가 stop상태가 아니게 되면 다시 일반 모드로 돌아가도록.
        else
            bRapidMode = false;

        if (bRapidMode)
        {
            rpmSpeed = 0.05f;
        }
        else
            rpmSpeed = 0.4f;
        #endregion
    }
    
    void Fire()
    {
        bFire = true;
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
            accuracy = 0.1f + (fRangeCheck * 0.02f);
            Vector3 fTarget = aimRayHit.point;
            fTarget = new Vector3(fTarget.x,
                                  Random.Range(fTarget.y - accuracy, fTarget.y + accuracy),
                                  Random.Range(fTarget.z - accuracy, fTarget.z + accuracy));

            //레이에 부딪힌 오브젝트가 있으면 부딪힌 위치를 바라보도록 방향 조정.
            firePos.transform.LookAt(fTarget);

            /*머즐플래쉬 코드 작성 위치*/


           //aimRayHit.point가 사정거리 이내에 위치할 경우.
            if (fRangeCheck < fReach)
            {
                ////데미지 계산 코드 작성 위치

            }
        }
        else
        {
            //최대거리 명중률 조정.
            accuracy = 20.0f;
            Vector3 fTarget = aimRay.GetPoint(fReach);
            fTarget = new Vector3(fTarget.x,
                                  Random.Range(fTarget.y - accuracy, fTarget.y + accuracy),
                                  Random.Range(fTarget.z - accuracy, fTarget.z + accuracy));
            firePos.GetComponent<TargetLookAt>().TargetLookat(fTarget);
        }

        //투사체 오브젝트 풀 생성.    
        bullet = bulletPool.UseObject();
        bullet.transform.position = firePos.position;
        bullet.transform.rotation = firePos.rotation;

    }

    public bool isFire() { return bFire; }
    public float getReach() { return fReach; }
    public void setFlareEffect(Vector3 pos)
    {
        flare = flarePool.UseObject();
        flare.transform.position = pos;

    }
}
