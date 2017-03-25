using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRespawn : MonoBehaviour {

    [SerializeField]
    float Distance; //캐릭터와의 거리

    public Transform CharPos;

    [SerializeField]
    int BulletIndex = -1;   //총알 인덱스

    float CreateCoolTime = 3;   // 먹은후 재생성 쿨타임

    bool CreateAble = true; //생성 가능여부
	// Use this for initialization
	void Start () {
       

    }
	
	// Update is called once per frame
	void Update () {

        if (CreateAble) //생성이 가능할경우
        {
            Distance = Vector3.Distance(CharPos.position, this.transform.position);

            if (Distance < 11 && BulletIndex == -1) //거리가 11이하고 총알인덱스를 배정받지않았을떄(생성 전)
            {
                BulletIndex = B_RespawnManager.GetInstance().CreateItemBullet(this.transform);
            }

            if (BulletIndex > -1 && Distance >= 11)   //생성을 했지만 거리가 벗어난경우
            {
                B_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex);// 총알 아이템 제거
                BulletIndex = -1;   //인덱스 초기화
            }


            if (Distance < 1 && BulletIndex > -1) //생성을 했고 캐릭터가 근처에 왔을경우
            {
                CharMove.m_UseGun.GetBullet();  //캐릭터 총알갯수 추가
                B_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex); // 총알 아이템 제거
                BulletIndex = -1; //인덱스 초기화
                StartCoroutine(BulletCreateDelay());    //재생성 쿨타임 시작
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
