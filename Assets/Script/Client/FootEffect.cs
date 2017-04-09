using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootEffect : MonoBehaviour {

    public GameObject m_FootEffect;
    GameObject temp;

    public Transform m_LeftFoot;
    public Transform m_RightFoot;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void StempFoot_L()
    {
        Debug.Log("LeftFoot");

        temp = Instantiate(m_FootEffect);
        temp.transform.position = m_LeftFoot.position;
        temp.transform.rotation = this.transform.rotation;
    }
    

    void StempFoot_R()
    {
        Debug.Log("RightFoot");

        temp = Instantiate(m_FootEffect);
        temp.transform.position = m_RightFoot.position;
        temp.transform.rotation = this.transform.rotation;
    }
}
