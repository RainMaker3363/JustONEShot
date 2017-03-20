using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulEnemy : MonoBehaviour {

    //private Vector3 _startPos;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetTransformInformation(float posX, float posY, float posZ, float rotY)
    {
        //_startPos = this.transform.position;

        transform.position = new Vector3(posX, posY, posZ);

        transform.rotation = Quaternion.Euler(0, rotY, 0);
        // We're going to do nothing with velocity.... for now
    }
}
