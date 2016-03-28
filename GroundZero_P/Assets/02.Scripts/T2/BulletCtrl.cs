using UnityEngine;
using System.Collections;

public class BulletCtrl : MonoBehaviour {

    private float speed;

    public float LifeTime = 3.0f;

    private float lifeTime;
    private float lifeTimer;

    private T_Attack T_attack;

    void Awake()
    {
        speed = 100.0f;
        T_attack = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T_Attack>();
    }

    void OnEnable() {
        lifeTime = LifeTime;
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

        T_attack.setFlareEffect(col.contacts[0].point);
        gameObject.SetActive(false);

    }

    void OnTriggerEnter(Collider coll)
    {
        gameObject.SetActive(false);
    }


    void FixedUpdate () {

        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        if (lifeTimer > lifeTime)
        {
            T_attack.setFlareEffect(transform.position);
            gameObject.SetActive(false);
        }
        else
            lifeTimer += Time.deltaTime;
    }
}
