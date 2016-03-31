using UnityEngine;
using System.Collections;

public class FlareCtrl : MonoBehaviour {
    public float lifeTime = 3.0f;

    private float lifeTimer;

	void OnEnable () {
        lifeTimer = 0.0f;
    }

    void Update()
    {
        if (lifeTimer > lifeTime)
        {
            gameObject.SetActive(false);
        }
        else
            lifeTimer += Time.deltaTime;
    }
}