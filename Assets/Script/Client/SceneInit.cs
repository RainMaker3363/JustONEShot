using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInit : MonoBehaviour {

    [SerializeField]
    GameObject[] InitObject;

    [SerializeField]
    public GameObject[] Player;
    public Transform PlayerStartPos;
    [SerializeField]
    public GameObject[] Enemy;
    public Transform EnemyStartPos;
    public MultiGameManager Mul_Manager;
    GameObject m_Player;
    GameObject m_Enemy;


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


#if UNITY_EDITOR    //유니티에디터에서 실행시킬경우 이쪽코드를 실행

        if (m_Player == null)
        {

            m_Player = Instantiate(Player[1]);   //에디터상에서는 고정
            m_Player.transform.position = PlayerStartPos.position;
            m_Player.name = "PlayerCharacter";
            m_Player.transform.SetParent(GamePlayObj.transform);
        }

        if (m_Enemy == null)
        {
            m_Enemy = Instantiate(Enemy[0]);   //에디터상에서는 고정

            m_Enemy.transform.position = EnemyStartPos.position;
            m_Enemy.name = "EnemyCharacter";
            m_Enemy.transform.SetParent(GamePlayObj.transform);
        }

#else
        GPGSManager.GetInstance.GetStartRandomSeed();

        if (m_Player == null)
        {
           // Mul_Manager.SendCharacterNumberMessage(Mul_Manager.GetMyCharNumber());

            m_Player = Instantiate(Player[GPGSManager.GetInstance.GetMyCharacterNumber()]);
            m_Player.transform.position = PlayerStartPos.position;
            m_Player.name = "PlayerCharacter";
            m_Player.transform.SetParent(GamePlayObj.transform);

        }


        //while (true)
        //{       
        //    if (Mul_Manager.GetPVPOpponentCharNumber() != 100)
        //    {
        //        break;
        //    }
        //}
        //Debug.Log(Mul_Manager.GetPVPOpponentCharNumber());
        if (m_Enemy == null)// && Mul_Manager.GetPVPOpponentCharNumber() != 100)
        {
            m_Enemy = Instantiate(Enemy[GPGSManager.GetInstance.GetPVPOpponentCharNumber()]);

            m_Enemy.transform.position = EnemyStartPos.position;
            m_Enemy.name = "EnemyCharacter";
            m_Enemy.transform.SetParent(GamePlayObj.transform);
        }

#endif


    }



}
	
