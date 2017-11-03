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
    //public MultiGameManager Mul_Manager;
    GameObject m_Player;
    [SerializeField]
    GameObject[] m_Enemy;

    [SerializeField]
    Sprite[] Poster;
    public SpriteRenderer Char1;
    [SerializeField]
    public SpriteRenderer[] CharEnemyPoster;


    public GameObject GamePlayObj;  //게임에 필요한것들은 이 오브젝트 하위에 생성됩니다

    int EditorIndex = 4;
    // Use this for initialization
    void Awake()
    {

        //프레임 고정
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
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

            m_Player = Instantiate(PlayerObj[1]);   //에디터상에서는 고정
            m_Player.transform.position = PlayerStartPos.position;
            m_Player.name = "PlayerCharacter";
            m_Player.transform.SetParent(GamePlayObj.transform);
        }

       

        Char1.sprite = Poster[3];

        //CharEnemyPoster = new SpriteRenderer[7];
        for (int i = 0; i < 7; i++)
        {
            CharEnemyPoster[i].sprite = null;
        }
        m_Enemy = new GameObject[EditorIndex];
        for (int i = 0; i < EditorIndex; i++)
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
        for (int i = 0; i < 7; i++)
        {
            CharEnemyPoster[i].sprite = null;
        }
        //Dictionary<string, int> Diction = GPGSManager.GetInstance.GetSurvivalOpponentCharNumbers();
        //IDictionaryEnumerator iter = Diction.GetEnumerator();

        m_Enemy = new GameObject[GPGSManager.GetInstance.GetAllPlayers().Count - 1];

        int j = 0;

        //while(iter.MoveNext())
        //{
        //    Debug.Log("iter.key : "+ iter.Key + "iter.value : " + iter.Value);
        //    //iter.MoveNext();
        //}

        //iter = Diction.GetEnumerator();
        //iter.MoveNext();
        string EnemyID;
        int MyIDNumber = GPGSManager.GetInstance.GetMySurvival_ID_Index();
        int MyIDCheck = 0;  //i 가 ,PlayerIDNumber와 같을경우 보정
        int CharCode = 0;
        int CharSkinCode = 0;
        while (j < GPGSManager.GetInstance.GetAllPlayers().Count)
        {
            int index = j - MyIDCheck;
            if (m_Enemy[index] == null)// && Mul_Manager.GetPVPOpponentCharNumber() != 100)
            {
                //Debug.Log("EnemyIndex j " + j);
        
                if ((MyIDNumber == j) && (MyIDCheck<1))
                {
                    //iter.MoveNext();
                    j++;                   
                    MyIDCheck++;
                    continue;
                }
                else
                {
                    EnemyID = GPGSManager.GetInstance.GetSurvivalAllPlayerCharacterID(j);
                    CharCode = GPGSManager.GetInstance.GetSurvivalAllPlayerCharacterNumber(EnemyID);
                    CharSkinCode = GPGSManager.GetInstance.GetSurvivalAllPlayerCharacterSkinNumber(EnemyID);
                    
                }

                m_Enemy[index] = Instantiate(EnemyObj[CharCode]);
                m_Enemy[index].transform.position = EnemyStartPos[index].position;
                m_Enemy[index].name = "EnemyCharacter" + index;
                m_Enemy[index].transform.SetParent(GamePlayObj.transform);
                m_Enemy[index].GetComponent<EnemyMove>().EnemyID = EnemyID;
                m_Enemy[index].GetComponent<EnemyMove>().PlayerNumber = CharCode;
                m_Enemy[index].GetComponent<EnemyMove>().CharSkinIndex = CharSkinCode;
            }


            CharEnemyPoster[index].sprite = Poster[CharCode];
            //iter.MoveNext();
            j++;
        }

        Char1.sprite = Poster[GPGSManager.GetInstance.GetMyCharacterNumber()];
#endif
    }
}

