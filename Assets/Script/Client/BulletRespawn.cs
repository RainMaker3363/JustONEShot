﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRespawn : MonoBehaviour {

    [SerializeField]
    float P_Distance; //캐릭터와의 거리

    [SerializeField]
    float E_Distance; //캐릭터와의 거리

    public Transform P_CharPos;
    public Transform E_CharPos;

    [SerializeField]
    int BulletIndex = -1;   //총알 인덱스

    float CreateCoolTime = 3;   // 먹은후 재생성 쿨타임

    bool CreateAble = true; //생성 가능여부

   public MultiGameManager Mul_GameManager;

    RaycastHit HitObj;
    // Use this for initialization
    void Start () {


        if (Physics.Raycast(transform.position, Vector3.down, out HitObj, 5f))
        {
            transform.position = new Vector3(transform.position.x, HitObj.point.y+0.5f, transform.position.z);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Mul_GameManager.GetEndGameState())
        {
            B_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex); // 총알 아이템 제거
            BulletIndex = -1; //인덱스 초기화
            gameObject.SetActive(false);
        }

        if (CreateAble) //생성이 가능할경우
        {
            P_Distance = Vector3.Distance(P_CharPos.position, this.transform.position);
            E_Distance = Vector3.Distance(E_CharPos.position, this.transform.position);

            if (P_Distance < 11 && BulletIndex == -1) //거리가 11이하고 총알인덱스를 배정받지않았을떄(생성 전)
            {
                BulletIndex = B_RespawnManager.GetInstance().CreateItemBullet(this.transform);
            }

            if (BulletIndex > -1 && P_Distance >= 11)   //생성을 했지만 거리가 벗어난경우
            {
                B_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex);// 총알 아이템 제거
                BulletIndex = -1;   //인덱스 초기화
            }


            if (BulletIndex > -1) //생성을 했고 캐릭터가 근처에 왔을경우
            {
                if (P_Distance < 1) //가까이온게 플레이어일경우
                {
                    CharMove.m_UseGun.GetBullet();  //캐릭터 총알갯수 추가
                    B_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex); // 총알 아이템 제거
                    BulletIndex = -1; //인덱스 초기화
                    StartCoroutine(BulletCreateDelay());    //재생성 쿨타임 시작
                }
                else if(E_Distance<1)   //가까이온게 적 플레인경우
                {
                    B_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex); // 총알 아이템 제거
                    BulletIndex = -1; //인덱스 초기화
                    StartCoroutine(BulletCreateDelay());    //재생성 쿨타임 시작
                }
            }
        }
    }

    IEnumerator BulletCreateDelay()
    {
        CreateAble = false;
        yield return new WaitForSeconds(CreateCoolTime);
        CreateAble = true;  //재생성 가능하게 설정

        yield return null;
    }
}
