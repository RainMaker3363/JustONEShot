using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffect : MonoBehaviour {

    float SetTime;
    public float LateTime;
	// Update is called once per frame
	void Update () {
		if(SetTime<Time.time)
        {
            gameObject.SetActive(false);
        }
	}
    
    void OnEnable()
    {
        SetTime = Time.time + LateTime;
    }
}
