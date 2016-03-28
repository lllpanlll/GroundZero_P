using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {
    private Transform targetTr;

    public float dist = 3.5f;
    public float zoomDist = 1.0f;
    private float zoomOutDist;
    public float right = 1.0f;

    public float dampTrace = 20.0f;
    private float fDampTrace;

    private float fDist;
    private float fRight;

    private float aimOutLerpSpeed = 2.0f;


    void Start () {
        fDist = dist;
        zoomOutDist = dist;
        fRight = right;
        fDampTrace = dampTrace;
        targetTr = GameObject.FindGameObjectWithTag(Tags.CameraTarget).GetComponent<Transform>();
    }

    void LateUpdate () {
        #region<바닥충돌처리>
        // 카메라 바닥 안뚫,
        Vector3 rayDir = (transform.position - targetTr.transform.position);
        Ray ray = new Ray(targetTr.transform.position, -targetTr.forward * fDist);
        Debug.DrawRay(ray.origin, (rayDir.normalized) * fDist, Color.blue);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag(Tags.Floor))
        {
                if (hit.distance < fDist)
                {
                    if (zoomOutDist > zoomDist)
                        zoomOutDist = Mathf.Lerp(zoomOutDist, zoomOutDist * 0.8f, Time.deltaTime * aimOutLerpSpeed);
                    else
                        zoomOutDist = dist;
                }
        }
        else
        {
            zoomOutDist = dist;
        }
        #endregion

        transform.position = targetTr.position - (targetTr.forward * fDist) + (transform.right * right);

        transform.LookAt((targetTr.position + (targetTr.right * fRight)));
    }

    public void setDampTrace(float f) { fDampTrace = f; }
    public float getDampTrace() { return fDampTrace; }
    public void setDist(float f) { fDist = f; }
    public float getDist() { return fDist; }
    public void setRight(float f) { fRight = f; }
    public float getRight() { return fRight; }

}
