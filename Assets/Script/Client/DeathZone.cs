using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathZone : MonoBehaviour {

    [SerializeField]
    float[] Level;

    int LevelIndex=0;

    public float DealyTime; 
    public float MoveTime; //데스존이 올라오는 시간

    float MoveSpeed; //데스존 올라오는 속도

    public int Damage;  //데스존 데미지
    public float DamageDealay;  //데스존 데미지 딜레이
    float DeathZoneInit;    //데스존위치 초기값

    public MultiGameManager Mul_GameManger;

    public bool DeadEyePlaying = false;

    public GameObject UI_DeathZoneUp;
    public GameObject UI_Main;

    public bool Reset;


    // Use this for initialization
    void Start () {
        StartCoroutine(DeathZoneMove());
        DeathZoneInit = transform.position.y;
        Reset = false;
         }
    void Update()
    {
        
    }
    IEnumerator DeathZoneMove()
    {
        while (true)
        {
           while (Level.Length > LevelIndex)
            {
                if (UI_Main.activeSelf) //메인UI가 꺼져있는경우 플레이 또는 연출중이므로 데스존이 올라오지않음
                {
                    yield return new WaitForSeconds(DealyTime); //딜레이 시간동안 대기
                    UI_DeathZoneUp.SetActive(true);

                    MoveSpeed = (Level[LevelIndex] - transform.position.y) / (MoveTime * 100);
                    Debug.Log("MoveSpeed" + MoveSpeed);
                    while (Level[LevelIndex] > transform.position.y)
                    {
                        Debug.Log("UP");
                        if (Mul_GameManger.GetEndGameState())  //게임이 끝났을경우
                        {
                            yield break;
                        }
                        if (Reset)
                        {
                            Reset = false;
                            StartCoroutine(ZombieDeathZoneReset());
                            yield break;
                        }

                        if (!Mul_GameManger.GetDeadEyeChecker() || DeadEyePlaying)//데드아이 발동중엔 안올라옴
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y + MoveSpeed, transform.position.z);
                        }

                        yield return new WaitForSeconds(0.01f);
                    }
                    LevelIndex++;
                    UI_DeathZoneUp.SetActive(false);
                }
                if (Reset)
                {
                    Reset = false;
                    StartCoroutine(ZombieDeathZoneReset());
                    yield break;
                }
                yield return new WaitForEndOfFrame();
            }
            if (Reset)
            {
                Reset = false;
                StartCoroutine(ZombieDeathZoneReset());
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ZombieDeathZoneReset()
    {
       // StopCoroutine(DeathZoneMove());
        UI_DeathZoneUp.SetActive(false);
        MoveSpeed = (DeathZoneInit - transform.position.y) / (MoveTime * 100);
        Debug.Log("MoveSpeed" + MoveSpeed);
        while (DeathZoneInit < transform.position.y)
        {
            Debug.Log("Down");
            
            transform.position = new Vector3(transform.position.x, transform.position.y + MoveSpeed, transform.position.z);

            yield return new WaitForSeconds(0.01f);
        }
        
        LevelIndex = 0;
       
        StartCoroutine(DeathZoneMove());
        yield break;
    }
}
