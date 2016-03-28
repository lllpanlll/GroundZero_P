using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Drone : MonoBehaviour {

    private Vector3 vTargetPos;
    private float fTargetSpeed;

    private T_Mgr T_mgr;
    private Transform playerTr;

    //연사속도 타이머
    private float rpmTime = 0.5f;
    private float rpmTimer = 0.0f;

    public GameObject bulletPrefeb;
    public List<GameObject> bulletPool = new List<GameObject>();

    void Start () {
        T_mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T_Mgr>();
        playerTr = GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform;

        for (int i = 0; i < 10; i++)
        {
            GameObject bullet = (GameObject)Instantiate(bulletPrefeb);

            bullet.name = "bulletPrefeb" + i.ToString();
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }

    void OnEnable()
    {
        rpmTime = Random.Range(0.5f, 1.0f);
    }
	
	void Update () {
        transform.position = Vector3.Lerp(transform.position, vTargetPos, Time.deltaTime * fTargetSpeed);

        if (Input.GetMouseButton(0) && T_mgr.getCtrlPossible().Attack == true)
        {
            //연사속도 조절.
            if (rpmTimer > rpmTime)
            {
                Fire();
                rpmTimer = 0.0f;
            }
            else
                rpmTimer += Time.deltaTime;
        }
        Debug.DrawRay(transform.position, transform.forward);
    }

    private void Fire()
    {
        float CamRotX = Camera.main.transform.eulerAngles.x;
        transform.rotation = Quaternion.Euler(CamRotX, playerTr.eulerAngles.y, 0.0f);

       

        //투사체 오브젝트 풀 생성.
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeSelf)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                break;
            }
        }
    }

    public void setTargetPos(Vector3 pos)
    {
        vTargetPos = pos;
    }
    public void setSpeed(float speed)
    {
        fTargetSpeed = speed;
    }
}
