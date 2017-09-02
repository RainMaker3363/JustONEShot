using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoManager
{
    static GameInfoManager m_Manager;

    public int SelectIndex = 3;
    public int SelectSkinIndex = 1;
    public bool ZombieInfinityMode = false;
    public int ZombieLevel = 0;

    public bool BackgroundSoundUse = true;
    public bool EffectSoundUse = true;
    public bool FirstTitle = true;

    public bool GameOver = false;
    public bool Pause = false;

    static int Gold = 0;
    static int SurvivalScore = 1000;
    static int PVPScore = 1000;

    int damage = 0;

    public static GameInfoManager GetInstance()
    {
        if(m_Manager==null)
        {
            m_Manager = new GameInfoManager();

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
            WaitRoomBGM.SetActive(false);
        }
        else
        {
            Title.SetActive(false);
            Touch.SetActive(false);
            MainUI.SetActive(true);
            WaitRoomBGM.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {

	}
    void OnZombieButton()
    {
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
}
