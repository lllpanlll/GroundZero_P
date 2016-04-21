using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {
    private Transform trTarget;

    public float DIST = 3.5f;
    public float ZOOM_DIST = 1.0f;
    public float RIGHT = 1.0f;
    private float zoomOutDist;

    public float DAMP_TRACE = 20.0f;
    private float fDampTrace;

    private float fDist;
    private float fRight;

    private float fAimOutLerpSpeed = 10.0f;

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
        //Vector3 vTargetToCamDir = (transform.position - trTarget.transform.position).normalized;
        Ray rTargetToTargetBackward = new Ray(trTarget.position, -trTarget.forward * DIST);
        //Debug.DrawRay(rTargetToTargetBackward.origin, (vTargetToCamDir) * fDist, Color.blue);
        RaycastHit hit;

        //Ray rTargetToTargetBackward = new Ray(trTarget.position, (trTarget.position + transform.right * 1f) - trTarget.position);
        //Debug.DrawLine(trTarget.position, (transform.position + trTarget.right * 1f) - trTarget.position, Color.green);

        if (Physics.Raycast(rTargetToTargetBackward, out hit, 5, 1 << 14))
        {
            if (hit.distance < fDist)
            {
                if (zoomOutDist > ZOOM_DIST)
                {
                    zoomOutDist = hit.distance * 0.8f;
                }
                else
                {
                    zoomOutDist = ZOOM_DIST;
                }
            }
        }
        else
        {
            zoomOutDist = fDist;
        }
        //DIST = Mathf.Lerp(DIST, zoomOutDist, Time.deltaTime * fAimOutLerpSpeed); //check
        #endregion


        //transform.position = Vector3.Lerp(transform.position, trTarget.position - (trTarget.forward * fDist) + (transform.right * RIGHT),
        //    Time.deltaTime * DAMP_TRACE);

        transform.position = trTarget.position - (trTarget.forward * DIST) + (transform.right * RIGHT);

        transform.LookAt((trTarget.position + (trTarget.right * RIGHT)));

        // 카메라 바닥 안뚫 2,
        //transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp((transform.localPosition.y), 0.1f, 10f), transform.localPosition.z);
    }

    public void SetDampTrace(float f) { fDampTrace = f; }
    public float GetDampTrace() { return fDampTrace; }
    public void SetDist(float f) { DIST = f; }
    public float GetDist() { return DIST; }
    public void SetRight(float f) { RIGHT = f; }
    public float GetRight() { return RIGHT; }
    public float GetZoom() { return zoomOutDist; } //check

}