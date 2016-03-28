using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_DP_PP : MonoBehaviour {

    Slider sliderDP;
    Slider sliderPP;
    public GameObject Player;
    T_Mgr t_mgr;

	// Use this for initialization
	void Start ()
    {
        sliderDP = transform.FindChild("DP").GetComponent<Slider>();
        sliderPP = transform.FindChild("PP").GetComponent<Slider>();
        t_mgr = Player.GetComponent<T_Mgr>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        sliderDP.value = t_mgr.getDP();
        sliderPP.value = t_mgr.getPP();
    }
}
