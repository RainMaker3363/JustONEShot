using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoManager
{
    static GameInfoManager m_Manager;

    public int SelectIndex = 1;
    public int SelectSkinIndex = 0;
    public bool ZombieInfinityMode = false;
    public int ZombieLevel = 0;

    public bool BackgroundSoundUse = true;
    public bool EffectSoundUse = true;
    public bool FirstTitle = true;

    public bool GameOver = false;
    public bool Pause = false;

    public static int PlayTicket;
    public static int StartTime;
    static System.TimeSpan NowTime;    //클라이언트를 킨 시간
    public static int TicketWaitTime=0; //티켓생기기까지남은시간
    public static int TicketCoolTime = 120;  //티켓생기는시간 임시로 120초


    static int Gold = 0;
    static int SurvivalScore = 1000;
    static int PVPScore = 1000;

    static string LoadLock;
    public static int CharLock;  //1번 캐릭은 잠그지 않음

    

    int damage = 0;

    

    public static GameInfoManager GetInstance()
    {
        if(m_Manager==null)
        {
            m_Manager = new GameInfoManager();
#if UNITY_EDITOR
            // PlayerPrefs.DeleteAll();

#endif
            if (PlayerPrefs.HasKey("CharLock"))
            {
                LoadLock = PlayerPrefs.GetString("CharLock");
            }
            else
            {
                LoadLock = "00000001";  //2,3,4번은 lock상태
            }
            CharLock = System.Convert.ToInt32(LoadLock, 2);// string을 2진수로 변환

            if (PlayerPrefs.HasKey("SurvivalScore"))
            {
                SurvivalScore = PlayerPrefs.GetInt("SurvivalScore");
            }

            if (PlayerPrefs.HasKey("PVPScore"))
            {
                PVPScore = PlayerPrefs.GetInt("PVPScore");
            }

            if (PlayerPrefs.HasKey("Gold"))
            {
                Gold = PlayerPrefs.GetInt("Gold");
            }
            else
            {
                Gold = 10000;
            }

#if UNITY_EDITOR
            Gold = 10000;

#endif

            if (PlayerPrefs.HasKey("PlayTicket"))
            {
                PlayTicket = PlayerPrefs.GetInt("PlayTicket");
            }
            else
            {
                PlayTicket = 10; //시간 테스트를위해 일부로 2개 빼놓음
               // Debug.Log(PlayTicket);
            }

            NowTime = System.DateTime.Now - new System.DateTime(System.DateTime.Now.Year, 1, 1, 0, 0, 0);

            if (PlayerPrefs.HasKey("StartTime"))    //전에 플레이 했던 기록이 있을경우
            {
                StartTime = PlayerPrefs.GetInt("StartTime");
            }
            else //없을경우
            {
                //StartTime = System.DateTime.Now.ToLocalTime();
                
                StartTime = (int)NowTime.TotalSeconds;
                Debug.Log(StartTime);
            }
           // TicketCheck();

        }

        return m_Manager;
    }

    public void GoldAdd(int gold)
    {
        Gold += gold;
        PlayerPrefs.SetInt("Gold", Gold);
    }
    public int SurvivalScoreAdd(int score)
    {
        SurvivalScore += score;

        if(SurvivalScore < 0)
        {
            SurvivalScore = 0;
        }
        else if (SurvivalScore > 10000)
        {
            SurvivalScore = 10000;
        }

        PlayerPrefs.SetInt("SurvivalScore", SurvivalScore);

        int total = SurvivalScore;

        return total;
    }

    public int PVPScoreAdd(int score)
    {
        PVPScore += score;

        if (PVPScore < 0)
        {
            PVPScore = 0;
        }
        else if (PVPScore > 10000)
        {
            PVPScore = 10000;
        }

        PlayerPrefs.SetInt("PVPScore", PVPScore);

        int total = PVPScore;

        return total;
    }

    public int ShowGold()
    {
        return Gold;
    }

    public void Accumulated_damage(int Damage)  //데미지누적
    {
        damage += Damage;
    }
    public int Accumulated_Get()  //데미지누적
    {
        return damage;
    }
    public void Accumulated_Reset() //데미지누적리셋
    {
        damage = 0;
    }

    public static void TicketCheck()    //티켓 회복용
    {
        NowTime = System.DateTime.Now - new System.DateTime(System.DateTime.Now.Year, 1, 1, 0, 0, 0);
        if (PlayTicket < 10)
        {
            int TicketTime = (int)NowTime.TotalSeconds - StartTime;
            Debug.Log(TicketTime);
            if(TicketWaitTime<TicketTime)
            {
                PlayTicket++;
            }
            for (; TicketTime > TicketCoolTime; TicketTime -= TicketCoolTime)
            {
                if (PlayTicket < 10)
                {
                    PlayTicket++;
                    Debug.Log(PlayTicket);
                }
                else
                {
                    break;
                }
                         
            }
          
                if (PlayTicket < 10)
                {
                    TicketWaitTime = TicketCoolTime - TicketTime;
                }
                else
                {
                    TicketWaitTime = 0;
                }
            


        }
    }
}



public class WaitRoom : MonoBehaviour {

    [SerializeField]
    GameObject[] PlayerChar;

    static int SelectIndex = 0;
    GameObject SelectChar;
    public Transform CharPos;

    //GPGSManager m_GPGSManager;
    MultiTitleManager m_MultiTitleManager;
    private IEnumerator SendRoutine;

    public Button Zombie_Button;

    [SerializeField]
    GameObject[] CharInfo;

    public GameObject SelectPos;

    [SerializeField]
    Sprite[] GameExplainImage;
    public Image ExplainScreen;
    int ExplainIndex=0;

    public GameObject Title;
    public GameObject Touch;
    public GameObject MainUI;
    public GameObject WaitRoomBGM;

    public TMPro.TextMeshProUGUI GoldText;
    public TMPro.TextMeshProUGUI TicketText;
    public TMPro.TextMeshProUGUI TicketTimeText;

    IEnumerator PlayTicketRecovery;


    public GameObject UI_Buy;
    public Image Buy_CharImage;
    public TMPro.TextMeshProUGUI Buy_Price_Text;
    int Buy_Price;
    int Buy_CharIndex;


    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    // Use this for initialization
    void Start () {
        //m_GPGSManager = GameObject.Find("GPGSManager").GetComponent<GPGSManager>();

        if(m_MultiTitleManager == null)
        {
            m_MultiTitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
        }

        SelectChar = Instantiate(PlayerChar[SelectIndex]);
        SelectChar.transform.position = CharPos.position;
        SelectChar.transform.rotation = CharPos.rotation;
        SelectChar.transform.SetParent(CharPos);

        GameInfoManager.GetInstance().SelectIndex = SelectIndex;
        GPGSManager.GetInstance.SetMyCharacterNumber(SelectIndex);//GPGS캐릭터 인덱스 설정
        SendRoutine = SendCharacterRoutine();

        StopCoroutine(SendRoutine);
        StartCoroutine(SendRoutine);

        Vector3 Postion = Vector3.zero;
        switch (SelectIndex)
        {
            case 0:
                {
                    Postion = new Vector3(-360, 160, 0);
                    break;
                }
            case 1:
                {
                    Postion = new Vector3(-120, 160, 0);
                    break;
                }
            case 2:
                {
                    Postion = new Vector3(120, 160, 0);
                    break;
                }
            case 3:
                {
                    Postion = new Vector3(360, 160, 0);
                    break;
                }
            default:
                break;
        }

        SelectPos.transform.localPosition = Postion;

        Zombie_Button.onClick.AddListener(OnZombieButton);
        if(GameInfoManager.GetInstance().FirstTitle)
        {
            GameInfoManager.GetInstance().FirstTitle = false;
            Title.SetActive(true);
            Touch.SetActive(true);
            MainUI.SetActive(false);
            GoldText.gameObject.SetActive(false);
            TicketText.gameObject.SetActive(false);
            WaitRoomBGM.SetActive(false);
        }
        else
        {
            Title.SetActive(false);
            Touch.SetActive(false);
            MainUI.SetActive(true);
            GoldText.gameObject.SetActive(true);
            TicketText.gameObject.SetActive(true);
            WaitRoomBGM.SetActive(true);
        }

        GoldText.text = GameInfoManager.GetInstance().ShowGold().ToString();
        //GameInfoManager.TicketCheck();
        TicketText.text = GameInfoManager.PlayTicket.ToString();
        //PlayTicketRecovery = TicketTime();
        //StartCoroutine(PlayTicketRecovery);
    }
	
	// Update is called once per frame
	void Update () {

	}
    void OnZombieButton()
    {
        TicketUse();
        UnityEngine.SceneManagement.SceneManager.LoadScene("ZombieScene");
    }

    IEnumerator SendCharacterRoutine()
    {
        while(true)
        {
            if (GPGSManager.GetInstance.IsAuthenticated())
            {
                if (GPGSManager.GetInstance.IsConnected())
                {
                    if (m_MultiTitleManager.GetOpponentCharNumber() == 100)
                    {
                        yield return new WaitForSeconds(0.5f);

                        m_MultiTitleManager.SendCharacterNumber(SelectIndex);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }


        
    }

    public void CharLockCheck(int Index)
    {
        int Mask = 1; // 00000001
        int select = Mask << Index;
        int LockCheck = (GameInfoManager.CharLock & select);

        if (LockCheck > 0)  //잠금이 해제되어있음
        {
            Debug.Log("True");
            ChangeChar(Index);
        }
        else //잠김 구매창 표시
        {
            Debug.Log("false");
            UI_Buy.SetActive(true);
            Buy_CharIndex = Index;
            switch (Index)
            {
                case 0:
                    {
                        Buy_Price = 1000;
                        break;
                    }
                case 1:
                    {
                        Buy_Price = 1002;
                        break;
                    }
                case 2:
                    {
                        Buy_Price = 1003;
                        Index++;
                        break;
                    }
                case 3:
                    {
                        Buy_Price = 1004;
                        Index--;
                        break;
                    }
                default:
                    break;
            }
            string Path = "Client/UI/WaitRoom/Color_Character_0" + (Index+1);
            Debug.Log(Path);
            Buy_CharImage.sprite = (Sprite)Resources.Load(Path, typeof(Sprite));
            Buy_Price_Text.text = Buy_Price.ToString();
        }
    }

    public void CharBuy()
    {

        if(Buy_Price<=GameInfoManager.GetInstance().ShowGold())
        {
            GameInfoManager.GetInstance().GoldAdd(-Buy_Price);
            GoldText.text = GameInfoManager.GetInstance().ShowGold().ToString();
            int Mask = 1; // 00000001
            int select = Mask << Buy_CharIndex;
            GameInfoManager.CharLock += select;
            //PlayerPrefs.SetString("CharLock", GameInfoManager.CharLock.ToString());
        }
    }

    public void ChangeChar(int Index)  //대기실은 끊겨도되니 GC를 고려하지 않고 삭제후 생성합니다
    {

        if (Index < 4)  //캐릭터 최대개수
        {
            if (SelectIndex != Index)   //전에 선택한것과 다를경우
            {
                SelectIndex = Index;    //선택한것으로 바꿈
                Destroy(SelectChar);    //전에 있던 캐릭터는 파기
                SelectChar = Instantiate(PlayerChar[SelectIndex]);  //현재 선택한거로 다시 생성
                SelectChar.transform.position = CharPos.position;
                SelectChar.transform.rotation = CharPos.rotation;
                SelectChar.transform.SetParent(CharPos);
                if (GPGSManager.GetInstance.IsAuthenticated())
                {
                    GPGSManager.GetInstance.SetMyCharacterNumber(SelectIndex);//GPGS캐릭터 인덱스 설정
                }
                GameInfoManager.GetInstance().SelectIndex = SelectIndex;
                GameInfoManager.GetInstance().SelectSkinIndex = 0; // 스킨인덱스 기본으로 변경
                Vector3 Postion = Vector3.zero;
                switch (Index)
                {
                    case 0:
                        {
                            Postion = new Vector3(-360, 160, 0);
                            break;
                        }
                    case 1:
                        {
                            Postion = new Vector3(-120, 160, 0);
                            break;
                        }
                    case 2:
                        {
                            Postion = new Vector3(120, 160, 0);
                            break;
                        }
                    case 3:
                        {
                            Postion = new Vector3(360, 160, 0);
                            break;
                        }
                    default:
                        break;
                }
                SelectPos.transform.localPosition = Postion;
            }
        }
    }

    public void CharInformation(bool set)
    {
        CharInfo[SelectIndex].SetActive(set);
    }

    public void CharInformationSet(int Index)
    {
        CharInfo[Index].SetActive(true);
    }

    public void CharInformationOff()
    {
        for (int i = 0; i < 4; i++)
        {
            CharInfo[i].SetActive(false);
        }
    }

    public void SelectLevel(int SelectLevel)
    {
        GameInfoManager.GetInstance().ZombieLevel = SelectLevel;
    }
    public void SelectMode(bool Selectmode)
    {
        GameInfoManager.GetInstance().ZombieInfinityMode = Selectmode;
    }

    public void ShowGameExplain()
    {
        if (ExplainIndex >= 7)
        {
            ExplainIndex = 0;
            ExplainScreen.transform.parent.gameObject.SetActive(false);
        }
        ExplainScreen.sprite = GameExplainImage[ExplainIndex];
        ExplainIndex++;

       
    }

    public void SelectSkin(int Index)
    {
        GameInfoManager.GetInstance().SelectSkinIndex = Index;
        string Path = "Client/InGamePrefab/Skin/0" + GameInfoManager.GetInstance().SelectIndex.ToString() + "/" + GameInfoManager.GetInstance().SelectSkinIndex.ToString();
        Debug.Log(Path);
        Material Mat = (Material)Resources.Load(Path, typeof(Material));
        SelectChar.GetComponent<WaitRoomSkin>().Skin.material = Mat;
    }

    public void TicketUse()
    {
        GameInfoManager.PlayTicket--;
        PlayerPrefs.SetInt("PlayTicket", GameInfoManager.PlayTicket);
        Debug.Log(GameInfoManager.PlayTicket);
        //System.TimeSpan now = System.DateTime.Now - new System.DateTime(System.DateTime.Now.Year, 1, 1, 0, 0, 0);
        ////if (GameInfoManager.PlayTicket <10)
        ////{
        //GameInfoManager.StartTime = (int)now.TotalSeconds;
        //    PlayerPrefs.SetInt("StartTime", GameInfoManager.StartTime);
        //    //Debug.Log(now.Seconds);
        ////}
    }

    IEnumerator TicketTime()
    {
        float ShowTicketWaitTime;
        TicketTimeText.gameObject.SetActive(true);
        if (GameInfoManager.PlayTicket < 10)
        {
            while (true)
            {
                GameInfoManager.TicketWaitTime--;
                int WaitTime = GameInfoManager.TicketWaitTime;
                if (WaitTime > 60)
                {
                    ShowTicketWaitTime = WaitTime / 60;
                    ShowTicketWaitTime += (WaitTime % 60) * 0.01f;
                }
                else
                {
                    ShowTicketWaitTime = WaitTime * 0.01f;
                }

                if (GameInfoManager.TicketWaitTime > 0)
                {
                    //string TimeText;
                    //TimeText = "" + ShowTicketWaitTime.ToString("00.00");
                    //TimeText = TimeText.Replace(".", ":");
                    TicketTimeText.text = ShowTicketWaitTime.ToString("00.00").Replace(".", ":");
                    yield return new WaitForSeconds(1);
                }
                else
                {
                    GameInfoManager.PlayTicket++;

                    TicketText.text = GameInfoManager.PlayTicket.ToString();
                    if (GameInfoManager.PlayTicket < 10) //티켓이 10장미만인경우
                    {
                        GameInfoManager.TicketWaitTime = GameInfoManager.TicketCoolTime;
                    }
                    else //티켓이 10장있는경우
                    {
                        TicketTimeText.gameObject.SetActive(false);
                        break;
                    }

                }
            }
        }
        else
        {
            TicketTimeText.gameObject.SetActive(false);
        }

        yield return null;
    }

}
