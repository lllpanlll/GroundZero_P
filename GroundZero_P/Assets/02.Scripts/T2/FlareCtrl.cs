using UnityEngine;
using System.Collections;

public class FlareCtrl : MonoBehaviour {
    //public int iDamage = 10;
    //public float speed = 1.0f;


    public float LifeTime = 3.0f;

    private float lifeTime;
    private float lifeTimer;

	void OnEnable () {
        lifeTime = LifeTime;
        lifeTimer = 0.0f;
    }

    void FixedUpdate()
    {
        //transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (lifeTimer > lifeTime)
        {
            gameObject.SetActive(false);
        }
        else
            lifeTimer += Time.deltaTime;
    }
}