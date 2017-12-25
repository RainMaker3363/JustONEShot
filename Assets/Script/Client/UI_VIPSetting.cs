using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_VIPSetting : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
        if(GameInfoManager.m_iVIPUser ==3434)
        {
            this.gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (GameInfoManager.m_iVIPUser == 3434)
        {
            this.gameObject.SetActive(false);
        }
    }
}
