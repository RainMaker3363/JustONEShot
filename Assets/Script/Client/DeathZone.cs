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

    public MultiGameManager Mul_GameManger;

    public bool DeadEyePlaying = false;

    public GameObject UI_DeathZoneUp;

    // Use this for initialization
    void Start () {
        StartCoroutine(DeathZoneMove());
	}
    void Update()
    {
        
    }
    IEnumerator DeathZoneMove()
    {
       
        while (Level.Length > LevelIndex)
        {
            
            yield return new WaitForSeconds(DealyTime); //딜레이 시간동안 대기
            UI_DeathZoneUp.SetActive(true);

            MoveSpeed = (Level[LevelIndex] - transform.position.y) / (MoveTime*100);
            Debug.Log("MoveSpeed" + MoveSpeed);
            while (Level[LevelIndex] > transform.position.y)
            {
                Debug.Log("UP");
                if (Mul_GameManger.GetEndGameState())  //게임이 끝났을경우
                {
                    yield break;
                }

                if(!Mul_GameManger.GetDeadEyeChecker() || DeadEyePlaying)//데드아이 발동중엔 안올라옴
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + MoveSpeed, transform.position.z);
                }

                yield return new WaitForSeconds(0.01f);
            }
            LevelIndex++;
            UI_DeathZoneUp.SetActive(false);
        }
    }
}
