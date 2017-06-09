using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiSceneInit : MonoBehaviour
{

    [SerializeField]
    GameObject[] InitObject;

    [SerializeField]
    public GameObject[] Player;
    public Transform PlayerStartPos;
       
    GameObject m_Player;
    
    public GameObject GamePlayObj;  //게임에 필요한것들은 이 오브젝트 하위에 생성됩니다

    // Use this for initialization
    void Awake()
    {

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
            if(GPGSManager.GetInstance.GetMyCharacterNumber() != 100)
                m_Player = Instantiate(Player[GPGSManager.GetInstance.GetMyCharacterNumber()]);
            else
                m_Player = Instantiate(Player[1]);   //에디터상에서는 고정

            m_Player.transform.position = PlayerStartPos.position;
            m_Player.name = "PlayerCharacter";
            m_Player.transform.SetParent(GamePlayObj.transform);
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
      
#endif


    }



}

