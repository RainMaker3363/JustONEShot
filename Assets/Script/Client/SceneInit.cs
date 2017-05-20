using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInit : MonoBehaviour {

    [SerializeField]
    GameObject[] InitObject;

    public GameObject Player;
    public Transform PlayerStartPos;
    public GameObject Enemy;
    public Transform EnemyStartPos;
    public MultiGameManager Mul_Manager;

    public GameObject GamePlayObj;  //게임에 필요한것들은 이 오브젝트 하위에 생성됩니다

    // Use this for initialization
    void Awake () {

        //for(int i =0; i< InitObject.Length;i++)
        //{
        //    if (InitObject[i] != null)
        //        InitObject[i].SetActive(true);
        //    else
        //        break;
        //}
       // Mul_Manager.CharacterSelectStateReceived();

        GameObject temp = Instantiate(Enemy);
        temp.transform.position = EnemyStartPos.position;
        temp.name = "EnemyCharacter";
        temp.transform.SetParent(GamePlayObj.transform);

         temp = Instantiate(Player);
        temp.transform.position = PlayerStartPos.position;
        temp.name = "PlayerCharacter";
        temp.transform.SetParent(GamePlayObj.transform);


    }
	

}
