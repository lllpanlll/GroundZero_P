using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_DP_PP : MonoBehaviour {

    Slider sliderDP;
    Slider sliderPP;
    public GameObject Player;
    T_Mgr t_mgr;
    Image imageHP;
    RectTransform oHighlight;
    int iHighlight;

	// Use this for initialization
	void Start ()
    {
        sliderDP = transform.FindChild("DP").GetComponent<Slider>();
        sliderPP = transform.FindChild("PP").GetComponent<Slider>();
        t_mgr = Player.GetComponent<T_Mgr>();
        imageHP = transform.FindChild("DpDanger").GetComponent<Image>();
        oHighlight = transform.FindChild("PpHighlight").GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        sliderDP.value = t_mgr.GetDP();
        //sliderPP.value = t_mgr.GetPP();

        if (sliderDP.value.Equals(0))
        {
            imageHP.enabled = true;
        }
        else
            imageHP.enabled = false;

        iHighlight = (int)(sliderPP.value * 0.04);
        switch (iHighlight)
        {
            case 1:
                oHighlight.rotation = Quaternion.Euler(0, 0, 135);
                break;
            case 2:
                oHighlight.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 3:
                oHighlight.rotation = Quaternion.Euler(0, 0, 45);
                break;
            case 4:
                oHighlight.rotation = Quaternion.Euler(0, 0, 0);
                break;
        }
        if (sliderPP.value >= 25) oHighlight.gameObject.SetActive(true);
        else oHighlight.gameObject.SetActive(false);
    }
}
