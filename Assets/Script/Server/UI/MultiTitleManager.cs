using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MultiTitleManager : MonoBehaviour, LBUpdateListener
{
    //public Text NetText;
    //public Text NetReadyText;

    //private bool ButtonChecker;
    //private bool MultiStartChecker;


    private int OpponentCharNumber;
    private int OpponentCharSkinNumber;
    private bool bPaused = false;  // 어플리케이션이 내려진 상태인지 아닌지의 스테이트를 저장하기 위한 변수
    private float LogCheckTimer;
    private int MyCharacterNumberBackup;
    private int MyCharacterSkinNumberBackup;

    private Dictionary<string, int> __SurvivalOpponentCharNumbers;
    private IDictionaryEnumerator __SurvivalOpoonentCharNumbers_Iter;

    private Dictionary<string, int> __OpponentCharSkinNumbers;
    private IDictionaryEnumerator __OpoonentCharSkinNumbers_Iter;

    public static HY.MultiGameModeState NowMultiGameModeNumber;


    // Use this for initialization
    void Awake () {
        MyCharacterNumberBackup = 0;
        MyCharacterSkinNumberBackup = 0;
        OpponentCharNumber = 100;
        OpponentCharSkinNumber = 0;
        LogCheckTimer = 3.0f;

        GPGSManager.GetInstance.InitializeGPGS(); // 초기화

        GPGSManager.GetInstance.LoginGPGS();


        GPGSManager.GetInstance.SetMultiGameModeState(0);
        NowMultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();

        // 리스너 설정
        GPGSManager.GetInstance.LBListener = this;
        
        if(__SurvivalOpponentCharNumbers == null)
        {
            __SurvivalOpponentCharNumbers = new Dictionary<string, int>(7);
        }
        else
        {
            //__SurvivalOpoonentCharNumbers_Iter = __SurvivalOpponentCharNumbers.GetEnumerator();
            
            //while(__SurvivalOpoonentCharNumbers_Iter.MoveNext())
            //{
            //    __SurvivalOpponentCharNumbers[__SurvivalOpoonentCharNumbers_Iter.Key.ToString()] = 100;
            //}

            //__SurvivalOpponentCharNumbers.Clear();
        }

        if(__OpponentCharSkinNumbers == null)
        {
            __OpponentCharSkinNumbers = new Dictionary<string, int>(7);
        }
        /* 
        * 유니티 엔진 사용 시 입력을 하지 않으면 모바일 장치의 화면이 어두워지다가 잠기게 되는데,
        * 그러면 플레이어는 잠김을 다시 풀어야 해서 불편합니다. 따라서 화면 잠금 방지 기능 추가는 필수적이고,
        * Screen.sleepTimeout를 아래처럼 설정하면 그걸 할 수 있습니다. 
        */
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 지정해 주면 고정비로 빌드가 되어 단말에서 지정 해상도로 출력이 된다.	
        //Screen.SetResolution(1280, 720, true); // 1280 x 720 으로 조정

        //Screen.SetResolution(1920, 1080, true); // 1920 x 1080 으로 조정

        //Screen.SetResolution(Screen.width, (Screen.width / 2) * 3 ); // 2:3 비율로 개발시

        //Screen.SetResolution(Screen.width, Screen.width * 16 / 9,  true); // 16:9 로 개발시
    }

    void Update()
    {
        if(LogCheckTimer >= 0.0f)
        {
            LogCheckTimer -= Time.deltaTime;
        }
        else
        {
            LogCheckTimer = 3.0f;
        }

        NowMultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();

        //Debug.Log("MultiGameModeNumber : " + NowMultiGameModeNumber);
    }

    //void OnGUI()
    //{
    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(w / 2, h - 500, 100, 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = 60;
    //    style.normal.textColor = new Color(0.0f, 0.0f, 1.5f, 1.5f);

    //    //string text = string.Format("HP : {0}", HP);
    //    string text = string.Format("OpponentCharNumber : {0}\nOpponentCharNumber2 : {1}", OpponentCharNumber, GPGSManager.GetInstance.GetPVPOpponentCharNumber());

    //    GUI.Label(rect, text, style);
    //}

    // 현재 자신이 선택한 캐릭터의 정보를 보내준다.
    public void SendCharacterNumber(int number = 100)
    {
        if(number < 0)
        {
            GPGSManager.GetInstance.SendCharacterSelectNumber(0);
        }
        else if(number >= 100)
        {
            GPGSManager.GetInstance.SendCharacterSelectNumber(100);
        }
        else
        {
            GPGSManager.GetInstance.SendCharacterSelectNumber(number);
        }

    }

    // 현재 자신이 선택한 캐릭터의 스킨 정보를 보내준다.
    public void SendCharacterSkinNumber(int SKinNumber = 0)
    {
        if(SKinNumber < 0)
        {
            GPGSManager.GetInstance.SendPlayerSkinStateMessage(SKinNumber);
        }
        else if(SKinNumber >= 100)
        {
            GPGSManager.GetInstance.SendPlayerSkinStateMessage(SKinNumber);
        }
        else
        {
            GPGSManager.GetInstance.SendPlayerSkinStateMessage(SKinNumber);
        }
    }

    // 현재 자신이 가지고 있는 캐릭터 넘버를 기억한다.
    public int GetOpponentCharNumber()
    {
        return OpponentCharNumber;
    }

    // 현재 PVP에서 상대방이 가지고 있는 캐릭터 스킨 번호를 기억한다.
    public int GetOpponentCharSkinNumber()
    {
        return OpponentCharSkinNumber;
    }

    // 현재 서바이벌 모드에 접속한 사람들의 캐릭터 정보(고유 번호)의 수를 반환한다.
    public int GetSurvivalOpoonentCharNumbers()
    {
        int AlreadyChecker = 0;

        __SurvivalOpoonentCharNumbers_Iter = __SurvivalOpponentCharNumbers.GetEnumerator();

        while (__SurvivalOpoonentCharNumbers_Iter.MoveNext())
        {
            if(__SurvivalOpponentCharNumbers[__SurvivalOpoonentCharNumbers_Iter.Key.ToString()] != 100)
            {
                AlreadyChecker++;
            }
        }

        if(LogCheckTimer >= 3.0f)
        {
            Debug.Log("AlreadyChecker : " + AlreadyChecker);
        }
        
        return AlreadyChecker;
    }

    // 상대방 캐릭터 정보를 기억한다.
    public Dictionary<string, int> GetSurvivalOpponentCharNumber()
    {
        return __SurvivalOpponentCharNumbers;
    }

    // 상대방이 보낸 캐릭터의 정보를 받아서 값을 갱신해준다.
    public void OpponentCharacterNumberReceive(string participantId, int characterNumber)
    {
        switch(NowMultiGameModeNumber)
        {
            case HY.MultiGameModeState.PVP:
                {
                    OpponentCharNumber = characterNumber;

                    Debug.Log("ID : " + participantId + " Number : " + OpponentCharNumber);
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    
                    __SurvivalOpponentCharNumbers[participantId] = characterNumber;

                    Debug.Log("ID : " + participantId + " Number : " + __SurvivalOpponentCharNumbers[participantId]);
                }
                break;

            default:
                {
                    OpponentCharNumber = characterNumber;
                }
                break;
        }
    }

    // 상대방이 보내는 캐릭터 스킨 번호를 갱신한다.
    public void OpponentCharacterSkinNumberReceive(string participantId, int skinNumber)
    {
        switch (NowMultiGameModeNumber)
        {
            case HY.MultiGameModeState.PVP:
                {
                    OpponentCharSkinNumber = skinNumber;

                    Debug.Log("ID : " + participantId + " Number : " + OpponentCharSkinNumber);
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {

                    __OpponentCharSkinNumbers[participantId] = skinNumber;

                    Debug.Log("ID : " + participantId + " Number : " + __OpponentCharSkinNumbers[participantId]);
                }
                break;

            default:
                {
                    OpponentCharSkinNumber = skinNumber;
                }
                break;
        }
    }

    // 어플리케이션이 Home 키가 눌려졌을때의 콜백되는 함수
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            bPaused = true;

            GPGSManager.GetInstance.LeaveRoom();

            MultiMatching_Cancel_Initilize();
        }
    }

    // 멀티 버튼 취소시 호출시키는 초기화 함수
    public void MultiMatching_Cancel_Initilize()
    {
        Debug.Log("Multi Game Matching Cancel");

        MyCharacterNumberBackup = GPGSManager.GetInstance.GetMyCharacterNumber();
        Debug.Log("MyCharacterNumberBackUp : " + MyCharacterNumberBackup);

        MyCharacterSkinNumberBackup = GPGSManager.GetInstance.GetMyCharacterSkinNumber();
        Debug.Log("MyCharacterSkinNumberBackUp : " + MyCharacterSkinNumberBackup);

        OpponentCharNumber = 100;
        OpponentCharSkinNumber = 0;
        LogCheckTimer = 3.0f;

        GPGSManager.GetInstance.LBListener = null;

        // 다시 세팅을 해준다.
        GPGSManager.GetInstance.InitializeGPGS(); // 초기화

        GPGSManager.GetInstance.LoginGPGS();

        GPGSManager.GetInstance.SetMultiGameModeState(0);
        NowMultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();

        // 리스너 설정
        GPGSManager.GetInstance.LBListener = this;

        if (__SurvivalOpponentCharNumbers == null)
        {
            __SurvivalOpponentCharNumbers = new Dictionary<string, int>(7);
        }
        else
        {
            __SurvivalOpponentCharNumbers.Clear();
        }

        if (__OpponentCharSkinNumbers == null)
        {
            __OpponentCharSkinNumbers = new Dictionary<string, int>(7);
        }
        else
        {
            __OpponentCharSkinNumbers.Clear();
        }

        // 캐릭터 선택을 백업해준다.
        GPGSManager.GetInstance.SetMyCharacterNumber(MyCharacterNumberBackup);

        // 캐릭터 번호를 백업해준다.
        GPGSManager.GetInstance.SetMyCharacterSkinNumber(MyCharacterSkinNumberBackup);

        /* 
        * 유니티 엔진 사용 시 입력을 하지 않으면 모바일 장치의 화면이 어두워지다가 잠기게 되는데,
        * 그러면 플레이어는 잠김을 다시 풀어야 해서 불편합니다. 따라서 화면 잠금 방지 기능 추가는 필수적이고,
        * Screen.sleepTimeout를 아래처럼 설정하면 그걸 할 수 있습니다. 
        */
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 지정해 주면 고정비로 빌드가 되어 단말에서 지정 해상도로 출력이 된다.	
        //Screen.SetResolution(1280, 720, true); // 1280 x 720 으로 조정

        //Screen.SetResolution(1920, 1080, true); // 1920 x 1080 으로 조정

        //Screen.SetResolution(Screen.width, (Screen.width / 2) * 3 ); // 2:3 비율로 개발시

        //Screen.SetResolution(Screen.width, Screen.width * 16 / 9,  true); // 16:9 로 개발시
    }

    // 게임이 완전 끝났을때 콜백 된다.
    public void LeftRoomConfirmed()
    {
        Debug.Log("Multi Game Matching Cancel");

        GPGSManager.GetInstance.LBListener = null;

        // 다시 세팅을 해준다.
        GPGSManager.GetInstance.InitializeGPGS(); // 초기화

        GPGSManager.GetInstance.LoginGPGS();

        GPGSManager.GetInstance.SetMultiGameModeState(0);
        NowMultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();

        // 리스너 설정
        GPGSManager.GetInstance.LBListener = this;

        if (__SurvivalOpponentCharNumbers == null)
        {
            __SurvivalOpponentCharNumbers = new Dictionary<string, int>(7);
        }
        else
        {
            __SurvivalOpponentCharNumbers.Clear();
        }

        /* 
        * 유니티 엔진 사용 시 입력을 하지 않으면 모바일 장치의 화면이 어두워지다가 잠기게 되는데,
        * 그러면 플레이어는 잠김을 다시 풀어야 해서 불편합니다. 따라서 화면 잠금 방지 기능 추가는 필수적이고,
        * Screen.sleepTimeout를 아래처럼 설정하면 그걸 할 수 있습니다. 
        */
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 지정해 주면 고정비로 빌드가 되어 단말에서 지정 해상도로 출력이 된다.	
        //Screen.SetResolution(1280, 720, true); // 1280 x 720 으로 조정

        //Screen.SetResolution(1920, 1080, true); // 1920 x 1080 으로 조정

        //Screen.SetResolution(Screen.width, (Screen.width / 2) * 3 ); // 2:3 비율로 개발시

        //Screen.SetResolution(Screen.width, Screen.width * 16 / 9,  true); // 16:9 로 개발시
    }

    //IEnumerator StartMultiGame()
    //{
    //    yield return new WaitForSeconds(1.0f);

    //    AutoFade.LoadLevel("TestMultiScene", 0.2f, 0.2f, Color.black);
    //}

    // Update is called once per frame
    //void Update () {

    //    if (GPGSManager.GetInstance.IsAuthenticated())
    //    {
    //        NetText.text = "구글 계정이 연결 되었습니다.";
    //    }
    //    else
    //    {
    //        NetText.text = "구글 계정이 아직 연결되지 않았습니다.";
    //    }

    //    if (GPGSManager.GetInstance.IsConnected() == true)
    //    {


    //        NetReadyText.text = "잠시후 멀티 게임을 시작합니다.";

    //        if(MultiStartChecker == false)
    //        {
    //            MultiStartChecker = true;

    //            StartCoroutine(StartMultiGame());
    //        }

    //    }
    //    else
    //    {


    //        NetReadyText.text = "아직 멀티 게임을 준비 중입니다...";
    //    }
    //}

    // 터치가 드래그(Drag) 했을때 호출 되는 함수
    //public virtual void OnDrag(PointerEventData ped)
    //{

    //}


    // 터치를 하고 있을 대 발생하는 함수
    //public virtual void OnPointerDown(PointerEventData ped)
    //{
    //    if (GPGSManager.GetInstance.IsAuthenticated() == true)
    //    {
    //        if(ButtonChecker == false)
    //        {
    //            ButtonChecker = true;

    //            GPGSManager.GetInstance.SignInAndStartMPGame();
    //        }




    //        //AutoFade.LoadLevel("TestMultiScene", 0.2f, 0.2f, Color.black);
    //    }


    //}

    // 터치에서 손을 땠을때 발생하는 함수
    //public virtual void OnPointerUp(PointerEventData ped)
    //{

    //}
}
