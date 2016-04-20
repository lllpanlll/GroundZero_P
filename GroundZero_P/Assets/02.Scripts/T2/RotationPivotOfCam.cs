using UnityEngine;
using System.Collections;

public class RotationPivotOfCam : MonoBehaviour {
    private float fAngleX, fAngleY;
    private float fRotSpeed;
    private T_MoveCtrl t_MoveCtrl;
    private T_Mgr t_Mgr;

    private float fStartY;
    private float fClamp = 40.0f;

    private Quaternion rotation = Quaternion.identity;

    void Start () {
        t_MoveCtrl = GetComponentInParent<T_MoveCtrl>();
        t_Mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T_Mgr>();
        fRotSpeed = Camera.main.GetComponent<FollowCam>().fMouseRotSpeed;
        fAngleY = transform.eulerAngles.x;
        fAngleX = transform.eulerAngles.y;

        fStartY = fAngleY;
    }

    void OnEnable() 
    {
        fAngleY = transform.eulerAngles.x;
        fAngleX = transform.eulerAngles.y;

        fStartY = fAngleY;
    }

    void Update () {
        
        if (t_Mgr.GetCtrlPossible().MouseRot == true)
        {
            fAngleY += -Input.GetAxis("Mouse Y") * fRotSpeed * Time.deltaTime;
            fAngleX += Input.GetAxis("Mouse X") * fRotSpeed * Time.deltaTime;
            //Clamp
            if ((fAngleY - fStartY) > fClamp) fAngleY = fStartY + fClamp;
            else if ((fAngleY - fStartY) < -fClamp) fAngleY = fStartY + (-fClamp);
            
            rotation = Quaternion.Euler(fAngleY, fAngleX, 0.0f);
            transform.rotation = rotation;
        }
        else if(t_Mgr.GetCtrlPossible().MouseRot == false)
        {
            //마우스 회전이 정지되어있는 동안 회전이 되었다면, 그 회전값을 유지하기 위해 앵글값을 설정해 준다.  
            if (transform.rotation.eulerAngles.x > fClamp)          
                fAngleY = transform.rotation.eulerAngles.x - 360.0f;
            else
                fAngleY = transform.rotation.eulerAngles.x;

            fAngleX = transform.rotation.eulerAngles.y;
        }
        
        Debug.DrawRay(transform.position, transform.forward);
    }



}
