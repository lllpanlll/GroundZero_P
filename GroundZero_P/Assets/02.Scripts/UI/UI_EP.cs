using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_EP : MonoBehaviour
{

    Slider sliderEP;
    public GameObject oPlayer;
    T_Mgr t_mgr;
    Image[] image;
    bool bChk;

    // Use this for initialization
    void Start()
    {
        sliderEP = GetComponent<Slider>();
        t_mgr = oPlayer.GetComponent<T_Mgr>();
        image = gameObject.GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        sliderEP.value = t_mgr.GetEP();

        if (!bChk)
        {
            bChk = true;
            StartCoroutine("InvisibleBar");
        }
        else if (sliderEP.value < sliderEP.maxValue && bChk)
        {
            bChk = false;
            StopCoroutine("InvisibleBar");
            for (int i = 0; i < 2; i++)
            {
                image[i].color = new Color(1, 1, 1, 1);
            }
        }

    }
    IEnumerator InvisibleBar()
    {
        yield return new WaitForSeconds(1f);
        if (sliderEP.value.Equals(sliderEP.maxValue))
            for (int i = 0; i < 2; i++)
            {
                for (float f = 1f; f >= 0f; f -= 0.2f)
                {
                    image[i].color = new Color(1, 1, 1, f);
                    yield return new WaitForSeconds(0.1f);
                }
            }
    }
}
