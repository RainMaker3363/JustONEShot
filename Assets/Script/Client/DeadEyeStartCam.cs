using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEyeStartCam : MonoBehaviour {

    Animator anim_Cam;
    public Animator anim_Char;


	// Use this for initialization
	void Start () {
        anim_Cam = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetAnimSpeed(float speed) //애니메이션 속도 조절
    {
        anim_Cam.speed = speed;
        anim_Char.speed = speed;
    }
}
