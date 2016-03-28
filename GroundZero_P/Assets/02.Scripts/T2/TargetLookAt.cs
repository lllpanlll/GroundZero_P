using UnityEngine;
using System.Collections;

public class TargetLookAt : MonoBehaviour {

    public void TargetLookat(Vector3 target)
    {
        transform.LookAt(target);
    }
}
