using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

    private CapsuleCollider capsuleCollider;
    public GameObject Effect;

    private float lifeTimer = 0.0f, lifeTime = 1.0f;
    void Awake () {
        capsuleCollider = GetComponent<CapsuleCollider>();
        Effect = (GameObject)Instantiate(Effect);
        Effect.SetActive(false);
    }

    void OnEnable()
    {
        lifeTimer = 0.0f;
        lifeTime = 5.0f;

        Effect = (GameObject)Instantiate(Effect);
        Effect.SetActive(false);
        Effect.transform.position = transform.position;

        capsuleCollider.enabled = true;
    }
    void OnDisable()
    {
        capsuleCollider.enabled = false;
    }

    void OnCollisionEnter(Collision col)
    {
        //폭파 이펙트 실행
        //T_attack.setFlareEffect(col.contacts[0].point);


        if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
        {
            if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
            {
                col.collider.gameObject.GetComponent<MonsterHitCtrl>().OnHitMonster(50);
                Effect.SetActive(true);
                gameObject.SetActive(false);
            }

        }
        Effect.SetActive(true);
        gameObject.SetActive(false);

    }

    void Update()
    {
        if (lifeTimer > lifeTime)
        {
            lifeTimer = 0.0f;
            Effect.SetActive(true);
            gameObject.SetActive(false);
        }
        else
            lifeTimer += Time.deltaTime;
    }
}
