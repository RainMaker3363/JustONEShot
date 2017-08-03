using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalSceneInit : MonoBehaviour
{

    [SerializeField]
    GameObject[] InitObject;

    [SerializeField]
    public GameObject[] PlayerObj;
    public Transform PlayerStartPos;
    [SerializeField]
    public GameObject[] EnemyObj;
    [SerializeField]
    public Transform[] EnemyStartPos;
    public MultiGameManager Mul_Manager;
    GameObject m_Player;
    [SerializeField]
    GameObject[] m_Enemy;

    [SerializeField]
    Sprite[] Poster;
    public SpriteRenderer Char1;
    [SerializeField]
    public SpriteRenderer[] CharEnemyPoster;


    public GameObject GamePlayObj;  //게임에 필요한것들은 이 오브젝트 하위에 생성됩니다

    // Use this for initialization
    void Awake()
    {

        //프레임 고정
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        /////


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

            m_Player = Instantiate(PlayerObj[3]);   //에디터상에서는 고정
            m_Player.transform.position = PlayerStartPos.position;
            m_Player.name = "PlayerCharacter";
            m_Player.transform.SetParent(GamePlayObj.transform);
        }

       

        Char1.sprite = Poster[3];

        //CharEnemyPoster = new SpriteRenderer[7];
        m_Enemy = new GameObject[7];
        for (int i = 0; i < 7; i++)
        {
            if (m_Enemy[i] == null)
            {
                m_Enemy[i] = Instantiate(EnemyObj[0]);   //에디터상에서는 고정

                m_Enemy[i].transform.position = EnemyStartPos[i].position;
                m_Enemy[i].name = "EnemyCharacter"+i;
                m_Enemy[i].transform.SetParent(GamePlayObj.transform);
            }
            CharEnemyPoster[i].sprite = Poster[0];
            
        }

#else
        // GPGSManager.GetInstance.GetStartRandomSeed();

        if (m_Player == null)
        {
           // Mul_Manager.SendCharacterNumberMessage(Mul_Manager.GetMyCharNumber());

            m_Player = Instantiate(PlayerObj[GPGSManager.GetInstance.GetMyCharacterNumber()]);
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
        for(int i = 0; i<7;i++)
        {
            CharEnemyPoster[i].sprite = null;
        }
        Dictionary<string, int> Diction = GPGSManager.GetInstance.GetSurvivalOpponentCharNumbers();
        IDictionaryEnumerator iter = GPGSManager.GetInstance.GetSurvivalOpponentCharNumbers().GetEnumerator();

        m_Enemy = new GameObject[GPGSManager.GetInstance.GetAllPlayers().Count];

        int j = 0;
        while (j < GPGSManager.GetInstance.GetAllPlayers().Count - 1)
        {
            if (m_Enemy[j] == null)// && Mul_Manager.GetPVPOpponentCharNumber() != 100)
            {
                iter.MoveNext();
                m_Enemy[j] = Instantiate(EnemyObj[Diction[iter.Key.ToString()]]);
                m_Enemy[j].transform.position = EnemyStartPos[j].position;
                m_Enemy[j].name = "EnemyCharacter" + j;
                m_Enemy[j].transform.SetParent(GamePlayObj.transform);
            }


            CharEnemyPoster[j].sprite = Poster[Diction[iter.Key.ToString()]];
            j++;
        }

        Char1.sprite = Poster[GPGSManager.GetInstance.GetMyCharacterNumber()];
#endif


    }



}

