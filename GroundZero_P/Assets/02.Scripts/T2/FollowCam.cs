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

    private float fAimOutLerpSpeed = 2.0f;


    void Start () {
        fDist = DIST;
        zoomOutDist = DIST;
        fRight = RIGHT;
        fDampTrace = DAMP_TRACE;
        trTarget = GameObject.FindGameObjectWithTag(Tags.CameraTarget).GetComponent<Transform>();
    }

    void LateUpdate () {
        #region<바닥충돌처리>
        // 카메라 바닥 안뚫,
        Vector3 vTargetToCamDir = (transform.position - trTarget.transform.position);
        Ray rTargetToTargetBackward = new Ray(trTarget.position, -trTarget.forward * fDist);
        Debug.DrawRay(rTargetToTargetBackward.origin, (vTargetToCamDir.normalized) * fDist, Color.blue);
        RaycastHit hit;
        if (Physics.Raycast(rTargetToTargetBackward, out hit) && hit.collider.CompareTag(Tags.Floor))
        {
                if (hit.distance < fDist)
                {
                    if (zoomOutDist > ZOOM_DIST)
                        zoomOutDist = Mathf.Lerp(zoomOutDist, zoomOutDist * 0.8f, Time.deltaTime * fAimOutLerpSpeed);
                    else
                        zoomOutDist = DIST;
                }
        }
        else
        {
            zoomOutDist = DIST;
        }
        #endregion

        transform.position = trTarget.position - (trTarget.forward * fDist) + (transform.right * RIGHT);

        transform.LookAt((trTarget.position + (trTarget.right * fRight)));
    }

    public void SetDampTrace(float f) { fDampTrace = f; }
    public float GetDampTrace() { return fDampTrace; }
    public void SetDist(float f) { fDist = f; }
    public float GetDist() { return fDist; }
    public void SetRight(float f) { fRight = f; }
    public float GetRight() { return fRight; }

}
