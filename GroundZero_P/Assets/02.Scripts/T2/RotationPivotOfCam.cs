using UnityEngine;
using System.Collections;

public class RotationPivotOfCam : MonoBehaviour {
    private float x, y;
    private float fRotSpeed;
    private T_MoveCtrl T_moveCtrl;
    private T_Mgr T_mgr;

    private float fStartY;
    private float fClamp = 40.0f;

    void Start () {
        T_moveCtrl = GetComponentInParent<T_MoveCtrl>();
        T_mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T_Mgr>();
        fRotSpeed = T_moveCtrl.fInitRotSpeed;
        y = transform.eulerAngles.x;
        x = transform.eulerAngles.y;

        fStartY = y;
    }

    void OnEnable() 
    {
        y = transform.eulerAngles.x;
        x = transform.eulerAngles.y;

        fStartY = y;
    }

    void Update () {
        if (T_mgr.getCtrlPossible().MouseRot == true)
        {
            y += -Input.GetAxis("Mouse Y") * fRotSpeed * Time.deltaTime;
            x += Input.GetAxis("Mouse X") * fRotSpeed * Time.deltaTime;

            //Clamp
            if ((y - fStartY) > fClamp) y = fStartY + fClamp;
            else if ((y - fStartY) < -fClamp) y = fStartY + (-fClamp);


            Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
            transform.rotation = rotation;
        }

        Debug.DrawRay(transform.position, transform.forward);
    }



}
