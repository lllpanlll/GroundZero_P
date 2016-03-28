using UnityEngine;
using System.Collections;

public class T_DroneCtrl : MonoBehaviour {
    public Transform[] setPos = new Transform[6];
    public GameObject dronePref;
    private GameObject[] drones = new GameObject[6];

    private T_Mgr T_mgr;

    private bool bAction;

    private int iDecEP = 10;

    //드론 지속 시간
    private bool bLife = false;
    private float lifeTime = 30.0f, lifeTimer = 0.0f;

    //드론! 마지막 일격!
    private bool bFinishAttack = false;

    void Start () {
        T_mgr = GetComponent<T_Mgr>();

        //드론 위치 생성
        for (int i = 0; i < 6; i++)
        {
            drones[i] = (GameObject)Instantiate(dronePref);
            drones[i].SetActive(false);
            drones[i].GetComponent<Drone>().setSpeed(Random.Range(2.0f, 7.0f));
            drones[i].GetComponent<Drone>().setTargetPos(setPos[i].position);

        }
        bAction = false;
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha2) && T_mgr.getCtrlPossible().Skill == true)
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
                    bAction = true;

                    T_mgr.ChangeState(T_Mgr.State.Skill);

                    T_mgr.DecreaseSkillPoint(T_Mgr.SkillType.EP, iDecEP);
                }
            }
        }

        if(bAction)
        {
            for(int i=0; i<6; i++)
            {
                drones[i].transform.position = setPos[i].position;
                drones[i].SetActive(true);
            }
            bLife = true;
            bAction = false;
            T_mgr.ChangeState(T_Mgr.State.idle);
        }


        if(bLife)
        {      
            for (int i = 0; i < 6; i++)
            {
                drones[i].GetComponent<Drone>().setTargetPos(setPos[i].position);
            }

            if (lifeTimer > lifeTime)
            {
                for (int i = 0; i < 6; i++)
                {
                    drones[i].SetActive(false);
                }

                bLife = false;
                lifeTimer = 0.0f;
            }
            else
            {
                lifeTimer += Time.deltaTime;
            }
        }

    }
}
