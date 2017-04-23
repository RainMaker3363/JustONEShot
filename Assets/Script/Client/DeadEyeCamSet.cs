using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEyeCamSet : MonoBehaviour {

    public GameObject cam;

    void OnDisable()    //비활성화될때
    {
        cam.SetActive(false);   // 켜져있는 카메라를 끈다
    }
}
