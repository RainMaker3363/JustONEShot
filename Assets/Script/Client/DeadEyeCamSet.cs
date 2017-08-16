using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEyeCamSet : MonoBehaviour {

    public GameObject cam;
    public Transform camPos;


    void Update()
    {
        cam.transform.position = camPos.position;
    }

    void OnDisable()    //비활성화될때
    {
        cam.SetActive(false);   // 켜져있는 카메라를 끈다
    }
}
