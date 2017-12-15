using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//#if UNITY_ADS
using UnityEngine.Advertisements;
//#endif
public class GameInfoManager
{
    static GameInfoManager m_Manager;

    public int SelectIndex = 0;
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
    public static int[] LockCode;   //1번비트 캐릭 잠금여부 이후 비트 스킨잠금여부

    public static int[] Skin_Char;  //선택했던 스킨저장

    public static int BeforeCharSelect;


    int damage = 0;

    

    public static GameInfoManager GetInstance()
    {
        if(m_Manager==null)
        {
            m_Manager = new GameInfoManager();
#if UNITY_EDITOR
            // PlayerPrefs.DeleteAll();

#endif
            LockCode = new int[4];

            for (int i = 0; i < 4; i++)
            {
                string Key = "LockCode" + i.ToString();
                if (PlayerPrefs.HasKey(Key))
                {
                    LoadLock = PlayerPrefs.GetString(Key);
                }
                else
                {
                    if(i==0)    //기본 첫번째 캐릭인경우
                    {
                        LoadLock = "00000001";  //캐릭만 열려있음
                    }
                    else //아닌경우(게임을 처음 플레이함)
                    {
                        LoadLock = "00000000";  //전부 잠금상태
                    }
                    
                }
                LockCode[i] = System.Convert.ToInt32(LoadLock, 2);// string을 2진수로 변환
            }

            Skin_Char = new int[4];

            for (int i = 0; i < 4; i++)
            {
                string Key = "Skin_Char" + i.ToString();
                if (PlayerPrefs.HasKey(Key))
                {
                    Skin_Char[i] = PlayerPrefs.GetInt(Key);
                }
                else
                {
                    Skin_Char[i] = 0;
                }
            }
            if (PlayerPrefs.HasKey("BeforeCharSelect"))
            {
                BeforeCharSelect = PlayerPrefs.GetInt("BeforeCharSelect");
            }
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
                PlayTicket = 3;
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

    static int SelectIndex;
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

    public static GameObject UI_GameStart_Fail;
    public GameObject UI_Custom;
    public GameObject UI_BuyChar;
    public GameObject UI_BuySkin;
   // public GameObject UI_Skin;
    public GameObject UI_BuyFail;
    public GameObject UI_BuyFail_Char;
    public Image Buy_CharImage;
    public TMPro.TextMeshProUGUI Buy_Price_Text;
    public TMPro.TextMeshProUGUI SkinBuy_Price_Text;
    int Buy_Price;
    int Buy_CharIndex;
    int Buy_SkinIndex;
    int BeforSkinIndex;

    [SerializeField]
    public GameObject[] WaitRoomChar;

    //광고
    public string zoneId;
    public int rewardQty = 250; //보상?

    public GameObject UI_Reward;

    [SerializeField]
    public AudioClip[] CharSelectSound;
    AudioSource CharSoundAudio;

    [SerializeField]
    GameObject[] SkinButton;

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

        
        GameInfoManager.GetInstance().SelectIndex = GameInfoManager.BeforeCharSelect;
        SelectIndex = GameInfoManager.GetInstance().SelectIndex;
        //Debug.Log("BeforeCharSelect" + SelectIndex);
        SelectChar = Instantiate(PlayerChar[SelectIndex]);
        SelectChar.transform.position = CharPos.position;
        SelectChar.transform.rotation = CharPos.rotation;
        SelectChar.transform.SetParent(CharPos);
        SkinButton[SelectIndex].SetActive(true);

        SendRoutine = SendCharacterRoutine();

        string Path = "Client/InGamePrefab/Skin/0" + SelectIndex + "/" + GameInfoManager.Skin_Char[SelectIndex].ToString();
        Debug.Log(Path);
        Material Mat = (Material)Resources.Load(Path, typeof(Material));
        SelectChar.GetComponent<WaitRoomSkin>().Skin.material = Mat;

        GameInfoManager.GetInstance().SelectSkinIndex = GameInfoManager.Skin_Char[SelectIndex];

        if (GPGSManager.GetInstance.IsAuthenticated())
        {
            GPGSManager.GetInstance.SetMyCharacterNumber(SelectIndex);//GPGS캐릭터 인덱스 설정
            GPGSManager.GetInstance.SetMyCharacterSkinNumber(GameInfoManager.Skin_Char[SelectIndex]);//GPGS캐릭터스킨 인덱스 설정
        }
        else
        {
            StartCoroutine(GameStartSendChar());
        }

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

        for(int i =0; i< WaitRoomChar.Length;i++)
        {
            Path = "Client/InGamePrefab/Skin/0" + i.ToString() + "/" + GameInfoManager.Skin_Char[i].ToString();
            
            Mat = (Material)Resources.Load(Path, typeof(Material));
            WaitRoomChar[i].GetComponent<WaitRoomSkin>().Skin.material = Mat;
        }

        WaitRoomChar[SelectIndex].SetActive(false);

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

        CharSoundAudio = GetComponent<AudioSource>();

        UI_GameStart_Fail = GameObject.Find("SubCanvas").transform.FindChild("GameStart_Fail").gameObject;
    }
	
	// Update is called once per frame
	void Update () {

	}
    void OnZombieButton()
    {
        if (TicketUse())
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ZombieScene");
        }
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
                        m_MultiTitleManager.SendCharacterSkinNumber(GameInfoManager.GetInstance().SelectSkinIndex);
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
       // int select = Mask << Index;
        int LockCheck = (GameInfoManager.LockCode[Index] & Mask);

        if (LockCheck > 0)  //잠금이 해제되어있음
        {
            Debug.Log("True");
            ChangeChar(Index);
        }
        else //잠김 구매창 표시
        {
            Debug.Log("false");
            UI_BuyChar.SetActive(true);
            Buy_CharIndex = Index;
            switch (Index)
            {
                case 0:
                    {
                        Buy_Price = 1000;
                        Buy_Price_Text.text = Buy_Price.ToString() + " BUY";
                        break;
                    }
                case 1:
                    {
                        Buy_Price = 5000;
                        Buy_Price_Text.text = Buy_Price.ToString() + " BUY\n OR\n ZOMBIE LIMITED HARD MODE";
                        break;
                    }
                case 2:
                    {
                        Buy_Price = 2500;
                        Buy_Price_Text.text = Buy_Price.ToString() + " BUY\n OR\n ZOMBIE LIMITED EASY MODE";
                        Index++;
                        break;
                    }
                case 3:
                    {
                        Buy_Price = 1000;
                        Buy_Price_Text.text = Buy_Price.ToString() + " BUY\n OR\n ZOMBIE LIMITED NOMAL MODE";
                        Index--;
                        break;
                    }
                default:
                    break;
            }
            string Path = "Client/UI/WaitRoom/Color_Character_0" + (Index+1);
            Debug.Log(Path);
            Buy_CharImage.sprite = (Sprite)Resources.Load(Path, typeof(Sprite));
            
        }
    }
    public void SkinLockCheck(int Index)
    {
        int Mask = 1; // 00000001
        int select = Mask << Index;
        int LockCheck = (GameInfoManager.LockCode[GameInfoManager.GetInstance().SelectIndex] & select);
        BeforSkinIndex = GameInfoManager.GetInstance().SelectSkinIndex;//이전 스킨인덱스 저장

        SelectSkin(Index);

        if (LockCheck > 0)  //잠금이 해제되어있음
        {
            Debug.Log("True");
        }
        else //잠김 구매창 표시
        {
            Debug.Log("false");
            //UI_Skin.SetActive(false);
            UI_BuySkin.SetActive(true);
            UI_Custom.SetActive(false);

            Buy_SkinIndex = Index;
            switch (Index)
            {
                case 0:
                    {
                        Buy_Price = 3000;
                        break;
                    }
                case 1:
                    {
                        Buy_Price = 3000;
                        break;
                    }
                case 2:
                    {
                        Buy_Price = 3000;
                        //Index++;
                        break;
                    }
                case 3:
                    {
                        Buy_Price = 3000;
                        //Index--;
                        break;
                    }
                default:
                    break;
            }
            //string Path = "Client/UI/WaitRoom/Color_Character_0" + (Index + 1);
            //Debug.Log(Path);
            //Buy_CharImage.sprite = (Sprite)Resources.Load(Path, typeof(Sprite));
            SkinBuy_Price_Text.text = Buy_Price.ToString();
        }
    }

    public void CharBuy()
    {

        if(Buy_Price<=GameInfoManager.GetInstance().ShowGold())
        {
            GameInfoManager.GetInstance().GoldAdd(-Buy_Price);
            GoldText.text = GameInfoManager.GetInstance().ShowGold().ToString();
            //int Mask = 1; // 00000001
            //int select = Mask << Buy_CharIndex;
            // GameInfoManager.CharLock += select;
            GameInfoManager.LockCode[Buy_CharIndex] += 1;   //00000001
            PlayerPrefs.SetString("LockCode" + Buy_CharIndex.ToString(), System.Convert.ToString(GameInfoManager.LockCode[Buy_CharIndex], 2));

        }
        else
        {
            UI_BuyFail_Char.SetActive(true);
        }
    }

    public void SkinBuy()
    {

        if (Buy_Price <= GameInfoManager.GetInstance().ShowGold())
        {
            GameInfoManager.GetInstance().GoldAdd(-Buy_Price);
            GoldText.text = GameInfoManager.GetInstance().ShowGold().ToString();
            int Mask = 1; // 00000001
            int select = Mask << Buy_SkinIndex;
           // GameInfoManager.CharLock += select;
            GameInfoManager.LockCode[GameInfoManager.GetInstance().SelectIndex] += select;   
           PlayerPrefs.SetString("LockCode" + GameInfoManager.GetInstance().SelectIndex, System.Convert.ToString(GameInfoManager.LockCode[GameInfoManager.GetInstance().SelectIndex], 2));
        }
        else
        {
            UI_BuyFail.SetActive(true);
            SelectSkin(BeforSkinIndex);
        }
        UI_Custom.SetActive(true);
    }

    public void SkinBuyNot()
    {
        SelectSkin(BeforSkinIndex);
        UI_Custom.SetActive(true);
    }

    void ChangeChar(int Index)  //대기실은 끊겨도되니 GC를 고려하지 않고 삭제후 생성합니다
    {

        if (Index < 4)  //캐릭터 최대개수
        {
            if (SelectIndex != Index)   //전에 선택한것과 다를경우
            {
                WaitRoomChar[SelectIndex].SetActive(true);
                SkinButton[SelectIndex].SetActive(false);

                //스킨적용
                string Path = "Client/InGamePrefab/Skin/0" + SelectIndex.ToString() + "/" + GameInfoManager.Skin_Char[SelectIndex].ToString();
                Debug.Log("waitskin "+Path);
                Material Mat = (Material)Resources.Load(Path, typeof(Material));
                WaitRoomChar[SelectIndex].GetComponent<WaitRoomSkin>().Skin.material = Mat;
                
                WaitRoomChar[Index].SetActive(false);
                SkinButton[Index].SetActive(true);

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
                // Debug.Log("SaveIndex"+SelectIndex);
                //선택한 캐릭터 저장
                GameInfoManager.BeforeCharSelect = SelectIndex;
                PlayerPrefs.SetInt("BeforeCharSelect", SelectIndex);
                //GameInfoManager.GetInstance().SelectSkinIndex = 0; // 스킨인덱스 기본으로 변경
                Vector3 Postion = Vector3.zero;
                int SkinIndex = 0;
                switch (Index)
                {
                    case 0:
                        {
                            Postion = new Vector3(-360, 160, 0);
                            SkinIndex = GameInfoManager.Skin_Char[0];
                            break;
                        }
                    case 1:
                        {
                            Postion = new Vector3(-120, 160, 0);
                            SkinIndex = GameInfoManager.Skin_Char[1];
                            break;
                        }
                    case 2:
                        {
                            Postion = new Vector3(120, 160, 0);
                            SkinIndex = GameInfoManager.Skin_Char[2];
                            break;
                        }
                    case 3:
                        {
                            Postion = new Vector3(360, 160, 0);
                            SkinIndex = GameInfoManager.Skin_Char[3];
                            break;
                        }
                    default:
                        break;
                }
                SelectPos.transform.localPosition = Postion;

                if (GameInfoManager.GetInstance().EffectSoundUse)
                {
                    CharSoundAudio.PlayOneShot(CharSelectSound[Index]); //캐릭터 선택 사운드 출력
                }

                GameInfoManager.GetInstance().SelectSkinIndex = SkinIndex;
                //스킨적용
                Path = "Client/InGamePrefab/Skin/0" + Index.ToString() + "/" + SkinIndex.ToString();
                //Debug.Log(Path);
                Mat = (Material)Resources.Load(Path, typeof(Material));
                SelectChar.GetComponent<WaitRoomSkin>().Skin.material = Mat;
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
        int NowCharIndex = GameInfoManager.GetInstance().SelectIndex;
        string Path = "Client/InGamePrefab/Skin/0" + NowCharIndex.ToString() + "/" + Index.ToString();
        Debug.Log(Path);
        Material Mat = (Material)Resources.Load(Path, typeof(Material));
        SelectChar.GetComponent<WaitRoomSkin>().Skin.material = Mat;

        GameInfoManager.Skin_Char[NowCharIndex] = Index;
        string Key = "Skin_Char" + NowCharIndex.ToString();

        PlayerPrefs.SetInt(Key, GameInfoManager.Skin_Char[NowCharIndex]);

        if (GPGSManager.GetInstance.IsAuthenticated())
        {
            GPGSManager.GetInstance.SetMyCharacterSkinNumber(Index);//GPGS캐릭터 인덱스 설정
        }
    }

    public static bool TicketUse()
    {
        if(GameInfoManager.PlayTicket>0)
        {
            GameInfoManager.PlayTicket--;
            PlayerPrefs.SetInt("PlayTicket", GameInfoManager.PlayTicket);
            Debug.Log(GameInfoManager.PlayTicket);
        }
        else
        {
            UI_GameStart_Fail.SetActive(true);
            return false;
        }
        
        //System.TimeSpan now = System.DateTime.Now - new System.DateTime(System.DateTime.Now.Year, 1, 1, 0, 0, 0);
        ////if (GameInfoManager.PlayTicket <10)
        ////{
        //GameInfoManager.StartTime = (int)now.TotalSeconds;
        //    PlayerPrefs.SetInt("StartTime", GameInfoManager.StartTime);
        //    //Debug.Log(now.Seconds);
        ////}
        return true;
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

    IEnumerator GameStartSendChar()
    {
        while(true)
        {
            if (GPGSManager.GetInstance.IsAuthenticated())
            {
                GPGSManager.GetInstance.SetMyCharacterNumber(SelectIndex);//GPGS캐릭터 인덱스 설정
                GPGSManager.GetInstance.SetMyCharacterSkinNumber(GameInfoManager.GetInstance().SelectSkinIndex);//GPGS캐릭터스킨 인덱스 설정
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
            
        }
       
    }

    public void OpenLeaderboard()
    {
        GPGSManager.GetInstance.OpenLeaderboardUI();
    }

    public void OnButton_AD()
    {
        bool test = false;
#if UNITY_EDITOR    //유니티에디터에서 실행시킬경우 이쪽코드를 실행
        test = true;
#endif

        if (GameInfoManager.PlayTicket < 1 || test)
        {
#if !UNITY_ADS // If the Ads service is not enabled...
            if (Advertisement.isSupported)
            { // If runtime platform is supported...
                Advertisement.Initialize(zoneId, true); // ...initialize.
            }
#endif

            // Wait until Unity Ads is initialized,
            //  and the default ad placement is ready.
            //while (!Advertisement.isInitialized || !Advertisement.IsReady())
            //{
            //   // yield return new WaitForSeconds(0.5f);
            //}


            if (string.IsNullOrEmpty(zoneId))
                zoneId = null;

            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleShowResult;
            Advertisement.Show(zoneId, options);
        }

    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Video completed. User rewarded " + rewardQty + " credits.");
                UI_Reward.SetActive(true);
                break;
            case ShowResult.Skipped:
                Debug.LogWarning("Video was skipped.");
                break;
            case ShowResult.Failed:
                Debug.LogError("Video failed to show.");
                break;
        }
    }

    public void GetReward()
    {
        GameInfoManager.PlayTicket = 3;
        TicketText.text = GameInfoManager.PlayTicket.ToString();
    }

    public void OnButtonTutorial()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TutorialScene");
    }
}
