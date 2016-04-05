using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

    private CapsuleCollider capsuleCollider;
    public GameObject oEffect;

    private float lifeTimer = 0.0f, lifeTime = 1.0f;
    void Awake () {
        capsuleCollider = GetComponent<CapsuleCollider>();
        oEffect = (GameObject)Instantiate(oEffect);
        oEffect.SetActive(false);
    }

    void OnEnable()
    {
        lifeTimer = 0.0f;
        lifeTime = 5.0f;

        oEffect = (GameObject)Instantiate(oEffect);
        oEffect.SetActive(false);
        oEffect.transform.position = transform.position;

        capsuleCollider.enabled = true;
    }
    void OnDisable()
    {
        capsuleCollider.enabled = false;
    }

    void OnCollisionEnter(Collision col)
    {
        //폭파 이펙트 실행
        //T_attack.SetFlareEffect(col.contacts[0].point);


        if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
        {
            if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
            {
                col.collider.gameObject.GetComponent<MonsterHitCtrl>().OnHitMonster(50);
                oEffect.SetActive(true);
                gameObject.SetActive(false);
            }

        }
        oEffect.SetActive(true);
        gameObject.SetActive(false);

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
        {
            if (col.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
            {
                col.gameObject.GetComponent<MonsterHitCtrl>().OnHitMonster(50);
                oEffect.SetActive(true);
                gameObject.SetActive(false);
            }

        }
        oEffect.SetActive(true);
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (lifeTimer > lifeTime)
        {
            lifeTimer = 0.0f;
            oEffect.SetActive(true);
            gameObject.SetActive(false);
        }
        else
            lifeTimer += Time.deltaTime;
    }
}
