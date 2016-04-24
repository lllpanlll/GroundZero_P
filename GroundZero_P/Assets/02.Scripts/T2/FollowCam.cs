using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {
    private Transform trTarget;

    public float DIST = 3.5f;
    public float ZOOM_DIST = 1f;
    public float RIGHT = 1.0f;
    private float zoomOutDist;

    public float DAMP_TRACE = 20.0f;
    private float fDampTrace;

    private float fDist;
    private float fRight;

    private float fAimOutLerpSpeed = 10.0f;

    Vector3 norm;
    float ddist;
    Vector3 camToWallDir;
    bool bRay;

    //초기화 변수
    public float fMouseRotSpeed = 200.0f;


    void Start() {
        fDist = DIST;
        zoomOutDist = DIST;
        fRight = RIGHT;
        fDampTrace = DAMP_TRACE;
        trTarget = GameObject.FindGameObjectWithTag(Tags.CameraTarget).GetComponent<Transform>();
    }

    void LateUpdate() {
        #region<바닥충돌처리>
        // 카메라 바닥 안뚫,
        RaycastHit hit;

        Ray rTargetToTargetBackward = new Ray(trTarget.position, -trTarget.forward);

        //Ray rTargetToTargetBackward = new Ray(trTarget.position, (transform.position + transform.right * 2f) - trTarget.position);
        Debug.DrawLine(trTarget.position, (transform.position + trTarget.right * 1f) - trTarget.position, Color.green);

        //Ray rTargetToTargetBackward = new Ray(trTarget.position, (transform.position - trTarget.position).normalized);

        if (Physics.Raycast(rTargetToTargetBackward, out hit, fDist + 1f, 1 << 14))
        {
            Debug.DrawRay(hit.point, hit.normal, Color.magenta);
            norm = hit.normal;

            // 코사인 계산으로 카메라 옆면이 벽 못뚫게 시도
            camToWallDir = (transform.position - hit.point).normalized;
            float cos = Vector3.Dot(camToWallDir, hit.normal);

            bRay = true;

            if (fDist + 0.3f > hit.distance)
            {
                // 거리 기반
                zoomOutDist = hit.distance;
                zoomOutDist = Mathf.Clamp(zoomOutDist, ZOOM_DIST, hit.distance * 0.8f);

                // 벡터기반 벽 못 뚫게 시도
                ddist = Vector3.Distance(transform.position, hit.point);
                ddist = Mathf.Clamp(ddist, 0.1f, 5f);


                // 벡터 기반
                //transform.position = Vector3.Lerp(transform.position, (hit.point + (hit.normal * ddist)), Time.deltaTime * 20f);
                //print(hit.distance + " || " + ddist);
            }
        }
        else
        {
            bRay = false;
            ddist = 0;
            zoomOutDist = fDist;
            //transform.position = Vector3.Lerp(transform.position, trTarget.position - (trTarget.forward * fDist) + (transform.right * RIGHT), Time.deltaTime * 20);
        }
        //DIST = Mathf.Lerp(DIST, zoomOutDist, Time.deltaTime * fAimOutLerpSpeed); //check

        #endregion
        #region 충돌 보류
        //if (Physics.SphereCast(transform.position, 0.2f, -trTarget.forward, out hit, 5, 1 << 14))
        //{
        //    print(hit.collider.name + " " + hit.distance);
        //    if (hit.distance < fDist)
        //    {
        //        if (zoomOutDist > ZOOM_DIST)
        //        {
        //            zoomOutDist = hit.distance * 0.8f;
        //        }
        //        else
        //        {
        //            zoomOutDist = ZOOM_DIST;
        //        }
        //    }
        //}
        //else
        //{
        //    zoomOutDist = fDist;
        //}
        #endregion


        //transform.position = Vector3.Lerp(transform.position, trTarget.position - (trTarget.forward * fDist) + (transform.right * RIGHT),
        //    Time.deltaTime * DAMP_TRACE);
        
        transform.position = trTarget.position - (trTarget.forward * DIST) + (transform.right * RIGHT);

        transform.LookAt((trTarget.position + (trTarget.right * RIGHT)));
        

        // 카메라 바닥 안뚫 2,
        //transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp((transform.localPosition.y), 0.1f, 10f), transform.localPosition.z);
    }

    void OnTriggerStay(Collider col)
    {

    }

    public void SetDampTrace(float f) { fDampTrace = f; }
    public float GetDampTrace() { return fDampTrace; }
    public void SetDist(float f) { DIST = f; }
    public float GetDist() { return DIST; }
    public void SetRight(float f) { RIGHT = f; }
    public float GetRight() { return RIGHT; }
    public float GetZoom() { return zoomOutDist; } //check

}