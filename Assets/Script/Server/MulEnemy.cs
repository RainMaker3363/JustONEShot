using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulEnemy : MonoBehaviour {

    // 적의 위치와 회전값을 보간해주기 위한 정보들..
    private Vector3 _startPos;
    private Vector3 _destinationPos;
    private Quaternion _startRot;
    private Quaternion _destinationRot;

    // 적의 정보를 보간시켜주기 위해 필요한 정보들..
    private float _lastUpdateTime;
    private float _timePerUpdate = 0.16f;
    private float pctDone;

    // Use this for initialization
    void Start () {
        // 0.16초마다 적의 위치를 갱신시켜 준다.
        _timePerUpdate = 0.16f;

        // 초기 위치값 갱신
        _startPos = this.transform.position;
        _startRot = this.transform.rotation;

        _destinationPos = Vector3.zero;
        _destinationRot = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {
        pctDone = (Time.time - _lastUpdateTime) / _timePerUpdate;

        if(pctDone <= 1.0f)
        {
            transform.position = Vector3.Slerp(_startPos, _destinationPos, pctDone);
            transform.rotation = Quaternion.Slerp(_startRot, _destinationRot, pctDone);
        }
	}

    // 적의 위치값을 갱신시켜 준다.
    public void SetTransformInformation(float posX, float posY, float posZ, float rotY)
    {
        _startPos = this.transform.position;
        _startRot = this.transform.rotation;

        _destinationPos = new Vector3(posX, posY, posZ);
        _destinationRot = Quaternion.Euler(0, rotY, 0);

        _lastUpdateTime = Time.time;
    }
}
