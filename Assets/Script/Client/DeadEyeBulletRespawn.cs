using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEyeBulletRespawn : MonoBehaviour {

    [SerializeField]
    float P_Distance; //캐릭터와의 거리

    [SerializeField]
    float E_Distance; //캐릭터와의 거리

    public Transform P_CharPos;
    public Transform E_CharPos;

    [SerializeField]
    int BulletIndex = -1;   //총알 인덱스

    float CreateCoolTime = 30;   // 먹은후 재생성 쿨타임

    public bool CreateAble = false; //생성 가능여부

   public MultiGameManager Mul_GameManager;
    public Transform DeathZone;
    RaycastHit HitObj;

    // Use this for initialization
    void Start()
    {
        GameObject GamePlayObj = GameObject.Find("GamePlayObj");
        P_CharPos = GamePlayObj.transform.Find("PlayerCharacter");
        E_CharPos = GamePlayObj.transform.Find("EnemyCharacter");

        if (Physics.Raycast(transform.position, Vector3.down, out HitObj, 5f))
        {
            transform.position = new Vector3(transform.position.x, HitObj.point.y + 0.5f, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update () {
        //if (P_CharPos == null)
        //{
        //    GameObject GamePlayObj = GameObject.Find("GamePlayObj");
        //    P_CharPos = GamePlayObj.transform.Find("PlayerCharacter");
        //}

        //if (E_CharPos == null)
        //{
        //    GameObject GamePlayObj = GameObject.Find("GamePlayObj");
        //    E_CharPos = GamePlayObj.transform.Find("EnemyCharacter");
        //}

        //    CreateAble = true;


        if (DeathZone.position.y + 0.5f > transform.position.y) //데스존에 잠겼다면
        {
            gameObject.SetActive(false);
            //Debug.Log("PointPos" + transform.position.y);
            //Debug.Log("DeathZonePos" + DeathZone.position.y);
        }
        if (Mul_GameManager.GetEndGameState())
        {
            DB_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex); // 총알 아이템 제거
            BulletIndex = -1; //인덱스 초기화
            gameObject.SetActive(false);
        }

 
        if (CreateAble)// && (P_CharPos != null && E_CharPos != null)) //생성이 가능할경우
        {
            P_Distance = Vector3.Distance(P_CharPos.position, this.transform.position);
            E_Distance = Vector3.Distance(E_CharPos.position, this.transform.position);

            if (P_Distance < 11 && BulletIndex == -1) //거리가 11이하고 총알인덱스를 배정받지않았을떄(생성 전)
            {
                BulletIndex = DB_RespawnManager.GetInstance().CreateItemBullet(this.transform);
            }

            if (BulletIndex > -1 && P_Distance >= 11)   //생성을 했지만 거리가 벗어난경우
            {
                DB_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex);// 총알 아이템 제거
                BulletIndex = -1;   //인덱스 초기화
            }


            if (BulletIndex > -1) //생성을 했고 캐릭터가 근처에 왔을경우
            {
                if (P_Distance < 1) //플레이어인경우
                {
                    CharMove.DeadEye();  //캐릭터 데드아이 시작
                    EnemyMove.PlayerDeadEye();
                    DB_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex); // 총알 아이템 제거
                    BulletIndex = -1; //인덱스 초기화
                    CreateAble = false;
                    StartCoroutine(BulletCreateDelay());    //재생성 쿨타임 시작
                }
                else if(E_Distance < 1)
                {
                    //EnemyMove.EnemyDeadEye();
                    DB_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex); // 총알 아이템 제거
                    BulletIndex = -1; //인덱스 초기화
                    CreateAble = false; //적이 먹은경우 난수생성요청을하지않기위해 코루틴은 실행하지않는다
                    // StartCoroutine(BulletCreateDelay());    //재생성 쿨타임 시작
                }

                
            }
          
        }
    }

    void OnDisable()
    {
        if (CreateAble)//생성이되어있는데 물에 잠긴경우
        {
            CreateAble = false;
            DB_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex); // 총알 아이템 제거
            BulletIndex = -1; //인덱스 초기화
            DB_CreateManager.GetInstance().Request = true; //물에 잠겨 사라졌으니 재생성을 요청
        }
    }

    public void BulletInit()
    {
        BulletIndex = -1; //인덱스 초기화
        DB_RespawnManager.GetInstance().DeleteItemBullet(BulletIndex); // 총알 아이템 제거
        CreateAble = false;
    }
    IEnumerator BulletCreateDelay()
    {
        //CreateAble = false;
        yield return new WaitForSeconds(CreateCoolTime);
        //CreateAble = true;  //재생성 가능하게 설정
        DB_CreateManager.GetInstance().Request = true;//먹었음을 쿨타임 끝나고 알려주므로써 쿨타임 공유
        yield return null;
    }
}
