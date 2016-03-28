using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_EP : MonoBehaviour {

    Slider sliderEP;
    public GameObject Player;
    T_Mgr t_mgr;

    // Use this for initialization
    void Start()
    {
        sliderEP = GetComponent<Slider>();
        t_mgr = Player.GetComponent<T_Mgr>();
    }

    // Update is called once per frame
    void Update()
    {
        sliderEP.value = t_mgr.getEP();
    }
}
