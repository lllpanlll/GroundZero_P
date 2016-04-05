using UnityEngine;
using System.Collections;

public class UI_IconObject : MonoBehaviour {

    public GameObject oIconObjPref;
    GameObject oUIMap;
    GameObject oPlayer;

    float oX, oZ, objX, objZ;

    void Awake()
    {
        oPlayer = GameObject.FindGameObjectWithTag(Tags.Player);
        oUIMap = GameObject.Find("Center_UI"); // warning
        oIconObjPref = Instantiate(oIconObjPref) as GameObject;
    }

    void Start ()
    {
        oIconObjPref.transform.parent = oUIMap.transform;
    }
	
	// Update is called once per frame
	void Update () {
        // obj
        oX = transform.position.x - oPlayer.transform.position.x;
        oZ = transform.position.z - oPlayer.transform.position.z;
        objX = (oX * 5f);
        objZ = (oZ * 5f);
        oIconObjPref.transform.localPosition = new Vector3(objX, objZ);
        oIconObjPref.transform.rotation = Quaternion.Euler(0, 0, -oUIMap.transform.rotation.z);
    }
}
