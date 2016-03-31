using UnityEngine;
using System.Collections;

public class RotationPivotOfCam : MonoBehaviour {
    private float fAngleX, fAngleY;
    private float fRotSpeed;
    private T_MoveCtrl t_MoveCtrl;
    private T_Mgr t_Mgr;

    private float fStartY;
    private float fClamp = 40.0f;

    void Start () {
        t_MoveCtrl = GetComponentInParent<T_MoveCtrl>();
        t_Mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T_Mgr>();
        fRotSpeed = t_MoveCtrl.fInitRotSpeed;
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


            Quaternion rotation = Quaternion.Euler(fAngleY, fAngleX, 0.0f);
            transform.rotation = rotation;
        }

        Debug.DrawRay(transform.position, transform.forward);
    }



}
