using UnityEngine;
using System.Collections;

public class BulletCtrl : MonoBehaviour {

    public float fSpeed = 300.0f;

    public float lifeTime = 3.0f;

    private float lifeTimer;

    private T_Attack t_Attack;

    void Awake()
    {
        t_Attack = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T_Attack>();
    }

    void OnEnable() {
        lifeTimer = 0.0f;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
        {
            if(col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
            {
                col.collider.gameObject.GetComponent<MonsterHitCtrl>().OnHitMonster(10);
            }

        }

        t_Attack.SetFlareEffect(col.contacts[0].point);
        gameObject.SetActive(false);

    }

    void OnTriggerEnter(Collider coll)
    {
        gameObject.SetActive(false);
    }


    void FixedUpdate () {

        transform.Translate(Vector3.forward * fSpeed * Time.deltaTime, Space.Self);

        if (lifeTimer > lifeTime)
        {
            t_Attack.SetFlareEffect(transform.position);
            gameObject.SetActive(false);
        }
        else
            lifeTimer += Time.deltaTime;
    }
}
