using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulEnemy : MonoBehaviour {

    // 상대방의 위치와 회전값을 보간해주기 위한 정보들..
    private Vector3 _startPos;
    private Vector3 _destinationPos;
    private Quaternion _startRot;
    private Quaternion _destinationRot;



    // 상대방의 정보를 보간시켜주기 위해 필요한 정보들..
    private float _lastUpdateTime;
    private float _timePerUpdate = 0.16f;
    private float pctDone;

    // 메시지 순서를 알아낼 변수
    private int _lastMessageNum;

    // 게임 끝의 여부
    public bool GameEndOn;

    // Use this for initialization
    void Start () {

        GameEndOn = false;

        _lastUpdateTime = Time.time;

        _lastMessageNum = 0;

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

    // 적의 타임아웃(Time-Out)의 여부를 확인할때 쓴다.
    // 네트워크 상태가 안좋거나 기타 여부로 체크가 안되면 종료되게 해준다.
    public float lastUpdateTime
    {
        get
        {
            return _lastUpdateTime;
        }
    }

    // 적의 위치값을 갱신시켜 준다.
    public void SetTransformInformation(int messageNum, float posX, float posY, float posZ, float rotY)
    {
        if (messageNum <= _lastMessageNum)
        {
            // Discard any out of order messages
            return;
        }
        
        _lastMessageNum = messageNum;

        _startPos = this.transform.position;
        _startRot = this.transform.rotation;

        _destinationPos = new Vector3(posX, posY, posZ);
        _destinationRot = Quaternion.Euler(0, rotY, 0);

        _lastUpdateTime = Time.time;
    }

    public void SetEndGameInformation(bool GameEnd)
    {
        GameEndOn = GameEnd;
    }

    // 게임을 나가면 해당 플레이어의 모습을 감춘다.
    public void GameOutInformation()
    {
        this.gameObject.SetActive(false);

        GameEndOn = true;
    }
}
