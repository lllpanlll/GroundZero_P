using UnityEngine;
using System.Collections;

public class CharacterRotation : MonoBehaviour {
    public void setRot(float f)
    {
        transform.rotation = Quaternion.Euler(0.0f, f, 0.0f);
    }
}
