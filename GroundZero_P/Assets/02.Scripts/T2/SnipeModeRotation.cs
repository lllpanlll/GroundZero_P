using UnityEngine;
using System.Collections;

public class SnipeModeRotation : MonoBehaviour {
    private float x, y;
    private float fRotSpeed = 20.0f;

    private float fStartX, fStartY;
    private float fClamp = 40.0f;

    private float fReach = 1000.0f;

    void Start () {
        y = transform.eulerAngles.x;
        x = transform.eulerAngles.y;

        fStartX = x;
        fStartY = y;
    }
    void OnEnable()
    {
        y = transform.eulerAngles.x;
        x = transform.eulerAngles.y;

        fStartX = x;
        fStartY = y;
    }
	
	void Update () {
        y += -Input.GetAxis("Mouse Y") * fRotSpeed * Time.deltaTime;
        x += Input.GetAxis("Mouse X") * fRotSpeed * Time.deltaTime;

        //Clamp
        if ((y - fStartY) > fClamp)            y = fStartY + fClamp;
        else if ((y - fStartY) < -fClamp)      y = fStartY + (-fClamp);

        if ((x - fStartX) > fClamp)            x = fStartX + fClamp;
        else if((x - fStartX) < -fClamp)       x = fStartX + (-fClamp);


        Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
        transform.rotation = rotation;
    }
    
    public Vector3 getLastShotPos()
    {
        //화면의 중앙 벡터
        Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
        Ray fireRay = new Ray(centerPos, transform.forward);

        RaycastHit aimRayHit;
        if (Physics.Raycast(fireRay, out aimRayHit, fReach))
        {
            //fReach거리 안에 레이가 맞으면 그 위치를 리턴.
            return aimRayHit.point;
        }
        //사정거리 안에 레이가 맞지 않았다면 fireRay방향 최대 사정거리 위치를 리턴.
        return fireRay.GetPoint(fReach);
    }
}
