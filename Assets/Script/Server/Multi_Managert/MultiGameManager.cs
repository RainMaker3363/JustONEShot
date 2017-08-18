using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GooglePlayGames.BasicApi.Multiplayer;



public class MultiGameManager : MonoBehaviour, MPUpdateListener
{

    // 전체적으로 관리될 게임, 캐릭터 상태 값
    public static HY.MultiGameState MultiState;
    public static HY.MultiPlayerState PlayerState;
    private HY.MultiGameModeState MultiGameModeState;

    private bool WaitSignal;
    private bool SelectSignal;

    // 네트워크 정보 텍스트...
    public Text MyInfoText;
    public Text EnemyInfoText;
    public Text NetText;

    public Text PlayerName;
    public Text EnemyName;

    // 테스트해볼 아이템 적용 여부
    public Text ItemGetCount;
    private int ItemCount;


    // 플레이어의 정보
    public GameObject MyCharacter;
    private CharMove MyPlayerCharacter;
    public GameObject MyCharacterPos;
    private string MyPlayerNick;

    

    // 적의 정보
    public GameObject EnemyCharacter;
    private EnemyMove OpponentPlayerCharacter;
    public GameObject EnemyCharacterPos;
    private string OpponentPlayerNick;

    private List<Participant> allPlayers;
    private List<Vector3> PlayerCharacters_Pos;
    private Dictionary<string, EnemyMove> _opponentScripts;
    private Dictionary<string, GameObject> PlayerCharacters;
    private Dictionary<string, string> PlayerCharacters_Nick;

    // 서바이벌 모드에서 쓰일 여러 유저들의 ID 기반의 값들
    private Dictionary<string, bool> _SurvivalOpponentWaitSignals;
    private Dictionary<string, bool> _SurvivalOpponentSelectSignals;
    private Dictionary<string, int> _SurvivalOpponentCharacterNumber;
    private Dictionary<string, int> _SurvivalOpponentWeaponNumber;
    private Dictionary<int, string> _SurvivalPlayersID;
    private Dictionary<string, int> _SurvivalPlayersRank;

    // 서바이벌 모드에서 쓰일 여러 이벤트 및 값들
    public bool BossEvent;
    private int LeftPlayerCount;
    private bool bPaused; // 어플리케이션이 내려진 상태인지 아닌지의 스테이트를 저장하기 위한 변수
    private int EnemyIndexChecker;
    private int MySurvivalRank;


    //private List<int> SurvivalOpponentCharNumbersList;
    //private List<bool> _SurvivalOpponentSelectSignalsList;
    //private List<bool> _SurvivalOpponentWaitSignalsList;
    //private List<int> _SurvivalOpponentWeaponNumberList;

    // 상대방이 갖고 있을 애니메이션 값
    private LSD.PlayerState m_state;

    public bool _multiplayerReady = false;
    private string _MyParticipantId;
    private int _MyParticipantId_Index;
    private string _EnemyParticipantId;
    private Vector2 _startingPoint;

    // PVP용으로써 서바이벌 모드에선 다른 로직을...
    // 기본값은 100이다.
    private int OpponentGunNumber;
    private int OppenentCharNumber;

    // 나의 캐릭터 고유 번호 및 총기 번호 정보
    private int MyGunNumber;
    private int MyCharNumber;


    // 네트워크 최적화 부분
    private float _nextBroadcastTime;

    // 게임이 끝났는지의 여부
    private bool ThisGameIsEnd;

    // 게임 재 매치 여부
    private bool ReMatchingOn;
    private bool MultiStartChecker;

    // 타임 아웃 정보
    [HideInInspector]
    public float timeOutThreshold;
    private float _timeOutCheckInterval;
    private float _nextTimeoutCheck;

    //데드아이 체크
    private float _DeadEyeTimer;
    private bool _DeadEyeChecker;
    private int _DeadEyeRespawnIndex;

    // PVP에서 사용하는 맨 처음 데드 아이 총알의 위치 관련 변수들
    private int DeadEyeStartIndex;
    private int _OpponentDeadEyeStartIndex;
    private int My_DeadEYeSTartIndex;

    // 서바이벌 모드에서 현재 남은 플레이어의 수를 체크한다.
    private float LeftPlayerCountTimer;

    // Use this for initialization
    void Awake()
    {

        // 임시로 만든 상태 값이므로 추후에 수정해주세요
        MultiState = HY.MultiGameState.WAIT;
        PlayerState = HY.MultiPlayerState.LIVE;

        GPGSManager.GetInstance.updateListener = this;
        MultiGameModeState = GPGSManager.GetInstance.GetMultiGameModeState();
        GPGSManager.GetInstance.SetMultiGameStart(true);

        LeftPlayerCountTimer = 1.5f;
        bPaused = false;

        //if (SurvivalOpponentCharNumbers == null)
        //{
        //    SurvivalOpponentCharNumbers = new List<int>(8);

        //    for (int i = 0; i < SurvivalOpponentCharNumbers.Count; i++)
        //    {
        //        SurvivalOpponentCharNumbers[i] = 100;
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < SurvivalOpponentCharNumbers.Count; i++)
        //    {
        //        SurvivalOpponentCharNumbers[i] = 100;
        //    }
        //}

        MyGunNumber = 100;
        OpponentGunNumber = 100;
        BossEvent = false;

        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {
                    Debug.Log("MultiGameModeState : " + MultiGameModeState);
                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    Debug.Log("MultiGameModeState : " + MultiGameModeState);

                    MyCharNumber = GPGSManager.GetInstance.GetMyCharacterNumber();

                    OppenentCharNumber = GPGSManager.GetInstance.GetPVPOpponentCharNumber();

                    // PVP에서 사용할 데드아이 총알 초기 위치
                    My_DeadEYeSTartIndex = Random.Range(1, 4);
                    _OpponentDeadEyeStartIndex = 1;
                    SendDeadEyeRespawnIndexMessage();

                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    Debug.Log("MultiGameModeState : " + MultiGameModeState);

                    MySurvivalRank = 1;
                    MyCharNumber = GPGSManager.GetInstance.GetMyCharacterNumber();

                    #region OpoonentInfoMap

                    _SurvivalOpponentCharacterNumber = GPGSManager.GetInstance.GetSurvivalOpponentCharNumbers();

                    //if (_SurvivalOpponentWeaponNumber == null)
                    //{
                    //    _SurvivalOpponentWeaponNumber = new Dictionary<string, int>(8);
                    //   //_SurvivalOpponentWeaponNumber.Clear();
                    //}
                    //else
                    //{
                    //    _SurvivalOpponentWeaponNumber.Clear();
                    //}

                    //if (_SurvivalOpponentWaitSignals == null)
                    //{
                    //    _SurvivalOpponentWaitSignals = new Dictionary<string, bool>(8);
                    //    //_SurvivalOpponentWaitSignals.Clear();
                    //}
                    //else
                    //{
                    //    _SurvivalOpponentWeaponNumber.Clear();
                    //}

                    //if (_SurvivalOpponentSelectSignals == null)
                    //{
                    //    _SurvivalOpponentSelectSignals = new Dictionary<string, bool>(8);
                    //    //_SurvivalOpponentSelectSignals.Clear();
                    //}
                    //else
                    //{
                    //    _SurvivalOpponentWeaponNumber.Clear();
                    //}

                    #endregion

                    #region OpoonentInfoList

                    //// 상대방 캐릭터 번호에 대한 리스트
                    //if (SurvivalOpponentCharNumbersList == null)
                    //{
                    //    SurvivalOpponentCharNumbersList = new List<int>(8);

                    //    for (int i = 0; i < SurvivalOpponentCharNumbersList.Count; i++)
                    //    {
                    //        SurvivalOpponentCharNumbersList[i] = 100;
                    //    }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < SurvivalOpponentCharNumbersList.Count; i++)
                    //    {
                    //        SurvivalOpponentCharNumbersList[i] = 100;
                    //    }
                    //}

                    //// 상대방 시그널 정보에 대한 리스트

                    //if (_SurvivalOpponentSelectSignalsList == null)
                    //{
                    //    _SurvivalOpponentSelectSignalsList = new List<bool>(8);

                    //    for (int i = 0; i < _SurvivalOpponentSelectSignalsList.Count; i++)
                    //    {
                    //        _SurvivalOpponentSelectSignalsList[i] = false;
                    //    }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < _SurvivalOpponentSelectSignalsList.Count; i++)
                    //    {
                    //        _SurvivalOpponentSelectSignalsList[i] = false;
                    //    }
                    //}

                    //// 상대방 Wait 시그널 정보에 대한 리스트

                    //if (_SurvivalOpponentWaitSignalsList == null)
                    //{
                    //    _SurvivalOpponentWaitSignalsList = new List<bool>(8);

                    //    for (int i = 0; i < _SurvivalOpponentWaitSignalsList.Count; i++)
                    //    {
                    //        _SurvivalOpponentWaitSignalsList[i] = false;
                    //    }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < _SurvivalOpponentWaitSignalsList.Count; i++)
                    //    {
                    //        _SurvivalOpponentWaitSignalsList[i] = false;
                    //    }
                    //}

                    //// 상대방 무기 정보에 대한 리스트

                    //if (_SurvivalOpponentWeaponNumberList == null)
                    //{
                    //    _SurvivalOpponentWeaponNumberList = new List<int>(8);

                    //    for (int i = 0; i < _SurvivalOpponentWeaponNumberList.Count; i++)
                    //    {
                    //        _SurvivalOpponentWeaponNumberList[i] = 100;
                    //    }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < _SurvivalOpponentWeaponNumberList.Count; i++)
                    //    {
                    //        _SurvivalOpponentWeaponNumberList[i] = 100;
                    //    }
                    //}

                    #endregion
                }
                break;
        }

        // 네트워크 초기화에 필요한 변수들
        _MyParticipantId = GPGSManager.GetInstance.GetMyParticipantId();

        allPlayers = GPGSManager.GetInstance.GetAllPlayers();


        // 네트워크 체크 변수들
        ThisGameIsEnd = false;
        LeftPlayerCount = 0;
        EnemyIndexChecker = 0;
        ItemCount = 0;

        _DeadEyeTimer = 0.0f;
        _DeadEyeChecker = false;
        _DeadEyeRespawnIndex = -1;

        WaitSignal = false;
        SelectSignal = false;

        ReMatchingOn = false;
        MultiStartChecker = false;

        // 네트워크 트래픽 최적화 변수 초기화
        _nextBroadcastTime = 0;

        // 타임 아웃 정보 초기화
        timeOutThreshold = 30.0f;
        _timeOutCheckInterval = 1.0f;
        _nextTimeoutCheck = 0.0f;

        MyPlayerNick = "";
        OpponentPlayerNick = "";



    }

    void Start()
    {
        SetupMultiplayerGame();
    }

    void SetupMultiplayerGame()
    {

        //GPGSManager.GetInstance.updateListener = this;

        // 1
        //_MyParticipantId = GPGSManager.GetInstance.GetMyParticipantId();

        //// 2
        //List<Participant> allPlayers = GPGSManager.GetInstance.GetAllPlayers();
        //_opponentScripts = new Dictionary<string, EnemyMove>(allPlayers.Count - 1);

        _MyParticipantId = GPGSManager.GetInstance.GetMyParticipantId();

        allPlayers = GPGSManager.GetInstance.GetAllPlayers();

        MultiGameModeState = GPGSManager.GetInstance.GetMultiGameModeState();//HY.MultiGameModeState.PVP;

        #region MPGame Setting

        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {
                    Debug.Log("MultiGameModeState : " + MultiGameModeState);
                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    Debug.Log("MultiGameModeState : " + MultiGameModeState);

                    Debug.Log("MultiGame Session 25% Settings");

                    _opponentScripts = new Dictionary<string, EnemyMove>(allPlayers.Count - 1);

                    for (int i = 0; i < allPlayers.Count; i++)
                    {
                        string nextParticipantId = allPlayers[i].ParticipantId;
                        Debug.Log("Setting up for " + nextParticipantId);


                        // 나의 식별 ID일때...
                        if (nextParticipantId == _MyParticipantId)
                        {
                            // 4
                            if (MyCharacter == null)
                            {

                                MyCharacter = GameObject.Find("GamePlayObj").transform.Find("PlayerCharacter").gameObject;

                            }
                            else
                            {

                                MyCharacter = GameObject.Find("GamePlayObj").transform.Find("PlayerCharacter").gameObject;

                            }
                        }
                        else
                        {
                            if (EnemyCharacter == null)
                            {
                                //EnemyCharacter = GameObject.Find("Enemy_Character");
                                EnemyCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").gameObject;
                                OpponentPlayerCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").GetComponent<EnemyMove>();
                                //MyCharacterPos.transform.position = EnemyCharacter.transform.position;

                                //EnemyCharacter.transform.position = EnemyCharacterPos.transform.position;

                                EnemyMove opponentScript = OpponentPlayerCharacter;//EnemyCharacter.GetComponent<EnemyMove>();
                                _EnemyParticipantId = nextParticipantId;
                                _opponentScripts[nextParticipantId] = opponentScript;

                            }
                            else
                            {
                                EnemyCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").gameObject;
                                OpponentPlayerCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").GetComponent<EnemyMove>();
                                //MyCharacterPos.transform.position = EnemyCharacter.transform.position;

                                EnemyMove opponentScript = OpponentPlayerCharacter;//EnemyCharacter.GetComponent<EnemyMove>();
                                _EnemyParticipantId = nextParticipantId;
                                _opponentScripts[nextParticipantId] = opponentScript;
                            }

                        }
                    }

                    Debug.Log("MultiGame Session 75% Settings");

                    if (allPlayers[0].ParticipantId == _MyParticipantId)
                    {
                        MyCharacter.transform.position = MyCharacterPos.transform.position;
                        EnemyCharacter.transform.position = EnemyCharacterPos.transform.position;

                        //PlayerName.text = GPGSManager.GetInstance.GetOtherNameGPGS(1);//_opponentScripts[_MyParticipantId].name;//GPGSManager.GetInstance.GetOtherNameGPGS(0);
                        //EnemyName.text = GPGSManager.GetInstance.GetOtherNameGPGS(0);

                        MyPlayerNick = GPGSManager.GetInstance.GetOtherNameGPGS(0);
                        OpponentPlayerNick = GPGSManager.GetInstance.GetOtherNameGPGS(1);
                    }
                    else
                    {
                        MyCharacter.transform.position = EnemyCharacterPos.transform.position;
                        EnemyCharacter.transform.position = MyCharacterPos.transform.position;

                        MyPlayerNick = GPGSManager.GetInstance.GetOtherNameGPGS(1);
                        OpponentPlayerNick = GPGSManager.GetInstance.GetOtherNameGPGS(0);
                    }

                    Debug.Log("MultiGame Session 100% Settings");

                    _multiplayerReady = true;

                    //for (int i = 0; i < allPlayers.Count; i++)
                    //{
                    //    string nextParticipantId = allPlayers[i].ParticipantId;
                    //    Debug.Log("Setting up car for " + nextParticipantId);
                    //    // 3
                    //    Vector3 carStartPoint = new Vector3(_startingPoint.x, _startingPoint.y + (i * _startingPointYOffset), 0);
                    //    if (nextParticipantId == _myParticipantId)
                    //    {
                    //        // 4
                    //        myCar.GetComponent<CarController>().SetCarChoice(i + 1, true);
                    //        myCar.transform.position = carStartPoint;
                    //    }
                    //    else
                    //    {
                    //        // 5
                    //        GameObject opponentCar = (Instantiate(opponentPrefab, carStartPoint, Quaternion.identity) as GameObject);
                    //        OpponentCarController opponentScript = opponentCar.GetComponent<OpponentCarController>();
                    //        opponentScript.SetCarNumber(i + 1);
                    //        // 6
                    //        _opponentScripts[nextParticipantId] = opponentScript;
                    //    }
                    //}
                    //// 7
                    //_lapsRemaining = 3;
                    //_timePlayed = 0;
                    //guiObject.SetLaps(_lapsRemaining);
                    //guiObject.SetTime(_timePlayed);

                    //for (int i = 0; i < allPlayers.Count; i++)
                    //{
                    //    string nextParticipantId = allPlayers[i].ParticipantId;
                    //    Debug.Log("Setting up for " + nextParticipantId);


                    //    // 나의 식별 ID일때...
                    //    if (nextParticipantId == _MyParticipantId)
                    //    {
                    //        // 4
                    //        if (MyCharacter == null)
                    //        {

                    //            MyCharacter = GameObject.Find("GamePlayObj").transform.Find("PlayerCharacter").gameObject;

                    //        }
                    //        else
                    //        {

                    //            MyCharacter = GameObject.Find("GamePlayObj").transform.Find("PlayerCharacter").gameObject;

                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (EnemyCharacter == null)
                    //        {
                    //            //EnemyCharacter = GameObject.Find("Enemy_Character");
                    //            EnemyCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").gameObject;
                    //            OpponentPlayerCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").GetComponent<EnemyMove>();
                    //            //MyCharacterPos.transform.position = EnemyCharacter.transform.position;

                    //            //EnemyCharacter.transform.position = EnemyCharacterPos.transform.position;

                    //            EnemyMove opponentScript = OpponentPlayerCharacter;//EnemyCharacter.GetComponent<EnemyMove>();
                    //            _EnemyParticipantId = nextParticipantId;
                    //            _opponentScripts[nextParticipantId] = opponentScript;

                    //        }
                    //        else
                    //        {
                    //            EnemyCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").gameObject;
                    //            OpponentPlayerCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").GetComponent<EnemyMove>();
                    //            //MyCharacterPos.transform.position = EnemyCharacter.transform.position;

                    //            EnemyMove opponentScript = OpponentPlayerCharacter;//EnemyCharacter.GetComponent<EnemyMove>();
                    //            _EnemyParticipantId = nextParticipantId;
                    //            _opponentScripts[nextParticipantId] = opponentScript;
                    //        }

                    //        // 5
                    //        //GameObject opponentCar = (Instantiate(opponentPrefab, carStartPoint, Quaternion.identity) as GameObject);

                    //    }
                    //}

                    ////for (int i = 0; i < allPlayers.Count; i++)
                    ////{
                    ////    string nextParticipantId = allPlayers[i].ParticipantId;
                    ////    Debug.Log("Setting up car for " + nextParticipantId);
                    ////    // 3
                    ////    Vector3 carStartPoint = new Vector3(_startingPoint.x, _startingPoint.y + (i * _startingPointYOffset), 0);
                    ////    if (nextParticipantId == _myParticipantId)
                    ////    {
                    ////        // 4
                    ////        myCar.GetComponent<CarController>().SetCarChoice(i + 1, true);
                    ////        myCar.transform.position = carStartPoint;
                    ////    }
                    ////    else
                    ////    {
                    ////        // 5
                    ////        GameObject opponentCar = (Instantiate(opponentPrefab, carStartPoint, Quaternion.identity) as GameObject);
                    ////        OpponentCarController opponentScript = opponentCar.GetComponent<OpponentCarController>();
                    ////        opponentScript.SetCarNumber(i + 1);
                    ////        // 6
                    ////        _opponentScripts[nextParticipantId] = opponentScript;
                    ////    }
                    ////}
                    ////// 7
                    ////_lapsRemaining = 3;
                    ////_timePlayed = 0;
                    ////guiObject.SetLaps(_lapsRemaining);
                    ////guiObject.SetTime(_timePlayed);

                    //if (allPlayers[0].ParticipantId == _MyParticipantId)
                    //{
                    //    MyCharacter.transform.position = MyCharacterPos.transform.position;
                    //    EnemyCharacter.transform.position = EnemyCharacterPos.transform.position;

                    //    //PlayerName.text = GPGSManager.GetInstance.GetOtherNameGPGS(1);//_opponentScripts[_MyParticipantId].name;//GPGSManager.GetInstance.GetOtherNameGPGS(0);
                    //    //EnemyName.text = GPGSManager.GetInstance.GetOtherNameGPGS(0);

                    //    MyPlayerNick = GPGSManager.GetInstance.GetOtherNameGPGS(0);
                    //    OpponentPlayerNick = GPGSManager.GetInstance.GetOtherNameGPGS(1);
                    //}
                    //else
                    //{
                    //    MyCharacter.transform.position = EnemyCharacterPos.transform.position;
                    //    EnemyCharacter.transform.position = MyCharacterPos.transform.position;

                    //    MyPlayerNick = GPGSManager.GetInstance.GetOtherNameGPGS(1);
                    //    OpponentPlayerNick = GPGSManager.GetInstance.GetOtherNameGPGS(0);
                    //}

                    //_multiplayerReady = true;
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    Debug.Log("MultiGameModeState : " + MultiGameModeState);

                    // 현재 남아있는 플레이어들의 수를 구한다.
                    LeftPlayerCount = (allPlayers.Count - 1);

                    Debug.logger.Log("SVM", "Left Player Count : " + LeftPlayerCount);
                    Debug.Log("LeftPlayerCount : " + LeftPlayerCount);

                    // 서바이벌 모드에서 쓰일 정보들을 갱신 및 초기화
                    if (_SurvivalOpponentWeaponNumber == null)
                    {
                        _SurvivalOpponentWeaponNumber = new Dictionary<string, int>(allPlayers.Count - 1);
                        //_SurvivalOpponentWeaponNumber.Clear();
                    }
                    else
                    {
                        _SurvivalOpponentWeaponNumber.Clear();
                    }

                    if (_SurvivalOpponentWaitSignals == null)
                    {
                        _SurvivalOpponentWaitSignals = new Dictionary<string, bool>(allPlayers.Count - 1);
                        //_SurvivalOpponentWaitSignals.Clear();
                    }
                    else
                    {
                        _SurvivalOpponentWaitSignals.Clear();
                    }

                    if (_SurvivalOpponentSelectSignals == null)
                    {
                        _SurvivalOpponentSelectSignals = new Dictionary<string, bool>(allPlayers.Count - 1);
                        //_SurvivalOpponentSelectSignals.Clear();
                    }
                    else
                    {
                        _SurvivalOpponentSelectSignals.Clear();
                    }

                    if(_SurvivalPlayersID == null)
                    {
                        _SurvivalPlayersID = new Dictionary<int, string>(allPlayers.Count);
                    }
                    else
                    {
                        _SurvivalPlayersID.Clear();
                    }

                    if(_SurvivalPlayersRank == null)
                    {
                        _SurvivalPlayersRank = new Dictionary<string, int>(allPlayers.Count);
                    }
                    else
                    {
                        _SurvivalPlayersRank.Clear();
                    }

                    _opponentScripts = new Dictionary<string, EnemyMove>(allPlayers.Count - 1);
                    PlayerCharacters = new Dictionary<string, GameObject>(allPlayers.Count);
                    PlayerCharacters_Nick = new Dictionary<string, string>(allPlayers.Count);
                    PlayerCharacters_Pos = new List<Vector3>();

                    for (int i = 0; i < allPlayers.Count; i++)
                    {
                        string nextParticipantId = allPlayers[i].ParticipantId;
                        PlayerCharacters_Nick[nextParticipantId] = allPlayers[i].DisplayName;

                        _SurvivalPlayersID[i] = allPlayers[i].ParticipantId;



                        // 위치값을 하나씩 받아온다.
                        string OpponentPosObjectName = ("StartPos" + i).ToString();



                        PlayerCharacters_Pos.Add((GameObject.Find("SceneInit").transform.Find(OpponentPosObjectName).gameObject.transform.position));



                        Debug.logger.Log("SVM", "Setting up for " + nextParticipantId);
                        Debug.logger.Log("SVM", "Player[" + i + "] Nick : " + PlayerCharacters_Nick[nextParticipantId]);

                        Debug.Log("Setting up for " + nextParticipantId);
                        Debug.Log("Player[" + i + "] Nick : " + PlayerCharacters_Nick[nextParticipantId]);

                        // 나의 식별 ID일때...
                        if (nextParticipantId == _MyParticipantId)
                        {
                            //// 4
                            //if (MyCharacter == null)
                            //{
                            //    //MyCharacter = GameObject.Find("GamePlayObj").transform.Find("PlayerCharacter").gameObject;
                            //    PlayerCharacters[nextParticipantId] = GameObject.Find("GamePlayObj").transform.Find("PlayerCharacter").gameObject;
                            //    PlayerCharacters_Pos[i] = GameObject.Find("SceneInit").transform.Find("StartPos_Player").gameObject.transform.position;
                            //}
                            //else
                            //{


                            //}
                            //MyCharacter = GameObject.Find("GamePlayObj").transform.Find("PlayerCharacter").gameObject;
                            
                            // 자기자신의 아이디 순차를 저장한다.
                            _MyParticipantId_Index = i;
                            EnemyIndexChecker = 1;

                            _SurvivalPlayersRank[nextParticipantId] = (allPlayers.Count);
                            PlayerCharacters[nextParticipantId] = GameObject.Find("GamePlayObj").transform.Find("PlayerCharacter").gameObject;
                            //PlayerCharacters_Pos[i] = GameObject.Find("SceneInit").transform.Find(OpponentPosObjectName).gameObject.transform.position;

                            //Debug.logger.Log("SVM", "Survival Wait Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentWaitSignals[_SurvivalPlayersID[i]]);
                            //Debug.logger.Log("SVM", "Survival Select Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentSelectSignals[_SurvivalPlayersID[i]]);
                            //Debug.logger.Log("SVM", "Survival Weapon Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentWeaponNumber[_SurvivalPlayersID[i]]);
                            //Debug.logger.Log("SVM", "Survival Char Map ID : " + allPlayers[i].ParticipantId + " Value : " + MyCharNumber);

                            //Debug.Log("Survival Wait Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentWaitSignals[_SurvivalPlayersID[i]]);
                            //Debug.Log("Survival Select Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentSelectSignals[_SurvivalPlayersID[i]]);
                            //Debug.Log("Survival Weapon Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentWeaponNumber[_SurvivalPlayersID[i]]);
                            Debug.Log("Survival Char Map ID : " + allPlayers[i].ParticipantId + " Value : " + MyCharNumber);
                        }
                        else
                        {
                            // Dictionary안에 있는 값들을 초기화 해준다.
                            _SurvivalOpponentWeaponNumber[_SurvivalPlayersID[i]] = 0;
                            _SurvivalOpponentSelectSignals[_SurvivalPlayersID[i]] = false;
                            _SurvivalOpponentWaitSignals[_SurvivalPlayersID[i]] = false;
                            _SurvivalPlayersRank[nextParticipantId] = (allPlayers.Count);

                            string OpponentObjectName = ("EnemyCharacter" + (i - EnemyIndexChecker)).ToString();

                            PlayerCharacters[nextParticipantId] = GameObject.Find("GamePlayObj").transform.Find(OpponentObjectName).gameObject;
                            //PlayerCharacters_Pos[i] = GameObject.Find("SceneInit").transform.Find(OpponentPosObjectName).gameObject.transform.position;
                            _opponentScripts[nextParticipantId] = GameObject.Find("GamePlayObj").transform.Find(OpponentObjectName).GetComponent<EnemyMove>();

                            Debug.logger.Log("SVM", "Survival Wait Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentWaitSignals[_SurvivalPlayersID[i]]);
                            Debug.logger.Log("SVM", "Survival Select Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentSelectSignals[_SurvivalPlayersID[i]]);
                            Debug.logger.Log("SVM", "Survival Weapon Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentWeaponNumber[_SurvivalPlayersID[i]]);
                            Debug.logger.Log("SVM", "Survival Char Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentCharacterNumber[_SurvivalPlayersID[i]]);

                            Debug.Log("Survival Wait Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentWaitSignals[_SurvivalPlayersID[i]]);
                            Debug.Log("Survival Select Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentSelectSignals[_SurvivalPlayersID[i]]);
                            Debug.Log("Survival Weapon Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentWeaponNumber[_SurvivalPlayersID[i]]);
                            Debug.Log("Survival Char Map ID : " + allPlayers[i].ParticipantId + " Value : " + _SurvivalOpponentCharacterNumber[_SurvivalPlayersID[i]]);
                            //if (EnemyCharacter == null)
                            //{
                            //    //EnemyCharacter = GameObject.Find("Enemy_Character");

                            //    //EnemyCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").gameObject;

                            //    string OpponentObjectName = ("EnemyCharacter" + i).ToString();
                            //    string OpponentPosObjectName = ("StartPos_Enemy" + i).ToString();

                            //    PlayerCharacters[nextParticipantId] = GameObject.Find("GamePlayObj").transform.Find(OpponentObjectName).gameObject;
                            //    PlayerCharacters_Pos[i] = GameObject.Find("SceneInit").transform.Find(OpponentPosObjectName).gameObject.transform.position;
                            //    _opponentScripts[nextParticipantId] = GameObject.Find("GamePlayObj").transform.Find(OpponentObjectName).GetComponent<EnemyMove>();


                            //    //OpponentPlayerCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").GetComponent<EnemyMove>();
                            //    //MyCharacterPos.transform.position = EnemyCharacter.transform.position;

                            //    //EnemyCharacter.transform.position = EnemyCharacterPos.transform.position;

                            //    //EnemyMove opponentScript = OpponentPlayerCharacter;//EnemyCharacter.GetComponent<EnemyMove>();
                            //    //_EnemyParticipantId = nextParticipantId;


                            //}
                            //else
                            //{
                            //    string OpponentObjectName = ("EnemyCharacter" + i).ToString();
                            //    string OpponentPosObjectName = ("StartPos_Enemy" + i).ToString();

                            //    PlayerCharacters[nextParticipantId] = GameObject.Find("GamePlayObj").transform.Find(OpponentObjectName).gameObject;
                            //    PlayerCharacters_Pos[i] = GameObject.Find("SceneInit").transform.Find(OpponentPosObjectName).gameObject.transform.position;
                            //    _opponentScripts[nextParticipantId] = GameObject.Find("GamePlayObj").transform.Find(OpponentObjectName).GetComponent<EnemyMove>();

                            //    ////EnemyCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").gameObject;
                            //    //PlayerCharacters[nextParticipantId] = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").gameObject;
                            //    //OpponentPlayerCharacter = GameObject.Find("GamePlayObj").transform.Find("EnemyCharacter").GetComponent<EnemyMove>();
                            //    ////MyCharacterPos.transform.position = EnemyCharacter.transform.position;

                            //    //EnemyMove opponentScript = OpponentPlayerCharacter;//EnemyCharacter.GetComponent<EnemyMove>();
                            //    //_EnemyParticipantId = nextParticipantId;
                            //    //_opponentScripts[nextParticipantId] = opponentScript;
                            //}

                            // 5
                            //GameObject opponentCar = (Instantiate(opponentPrefab, carStartPoint, Quaternion.identity) as GameObject);

                        }
                    }

                    Debug.logger.Log("SVM", "Survival Wait Map Count: " + _SurvivalOpponentWaitSignals.Count);
                    Debug.logger.Log("SVM", "Survival Select Map Count: " + _SurvivalOpponentSelectSignals.Count);
                    Debug.logger.Log("SVM", "Survival Weapon Map Count: " + _SurvivalOpponentWeaponNumber.Count);
                    Debug.logger.Log("SVM", "Survival Char Map Count: " + _SurvivalOpponentCharacterNumber.Count);

                    Debug.Log("Survival Wait Map Count : " + _SurvivalOpponentWaitSignals.Count);
                    Debug.Log("Survival Select Map Count : " + _SurvivalOpponentSelectSignals.Count);
                    Debug.Log("Survival Weapon Map Count : " + _SurvivalOpponentWeaponNumber.Count);
                    Debug.Log("Survival Char Map Count : " + _SurvivalOpponentCharacterNumber.Count);

                    //for (int i = 0; i < allPlayers.Count; i++)
                    //{
                    //    string nextParticipantId = allPlayers[i].ParticipantId;
                    //    Debug.Log("Setting up car for " + nextParticipantId);
                    //    // 3
                    //    Vector3 carStartPoint = new Vector3(_startingPoint.x, _startingPoint.y + (i * _startingPointYOffset), 0);
                    //    if (nextParticipantId == _myParticipantId)
                    //    {
                    //        // 4
                    //        myCar.GetComponent<CarController>().SetCarChoice(i + 1, true);
                    //        myCar.transform.position = carStartPoint;
                    //    }
                    //    else
                    //    {
                    //        // 5
                    //        GameObject opponentCar = (Instantiate(opponentPrefab, carStartPoint, Quaternion.identity) as GameObject);
                    //        OpponentCarController opponentScript = opponentCar.GetComponent<OpponentCarController>();
                    //        opponentScript.SetCarNumber(i + 1);
                    //        // 6
                    //        _opponentScripts[nextParticipantId] = opponentScript;
                    //    }
                    //}
                    //// 7
                    //_lapsRemaining = 3;
                    //_timePlayed = 0;
                    //guiObject.SetLaps(_lapsRemaining);
                    //guiObject.SetTime(_timePlayed);

                    Debug.logger.Log("SVM", "Init Player Count : " + PlayerCharacters.Count);
                    Debug.logger.Log("SVM", "Init Players ID Count : " + _SurvivalPlayersID.Count);
                    Debug.Log("SVM Init Player Count : " + PlayerCharacters.Count);
                    Debug.Log("SVM Init Players ID Count : " + _SurvivalPlayersID.Count);

                    for(int i =0; i<PlayerCharacters.Count; i++)
                    {
                        if (allPlayers[i].ParticipantId == _MyParticipantId)
                        {

                            PlayerCharacters[_MyParticipantId].transform.position = PlayerCharacters_Pos[i];
                            Debug.logger.Log("SVM", "Player ID : " + allPlayers[i].ParticipantId);
                            Debug.Log("SVM Player ID : " + allPlayers[i].ParticipantId);
                        }
                        else
                        {
                            //MyCharacter.transform.position = EnemyCharacterPos.transform.position;
                            PlayerCharacters[allPlayers[i].ParticipantId].transform.position = PlayerCharacters_Pos[i];
                            Debug.logger.Log("SVM", "Player ID : " + allPlayers[i].ParticipantId);
                            Debug.Log("SVM Player ID : " + allPlayers[i].ParticipantId);
                        }
                    }

                    _multiplayerReady = true;

                    

                }
                break;
        }

        #endregion

        Debug.Log("Init Multi Ready : " + _multiplayerReady);

        /* 
        * 유니티 엔진 사용 시 입력을 하지 않으면 모바일 장치의 화면이 어두워지다가 잠기게 되는데,
        * 그러면 플레이어는 잠김을 다시 풀어야 해서 불편합니다. 따라서 화면 잠금 방지 기능 추가는 필수적이고,
        * Screen.sleepTimeout를 아래처럼 설정하면 그걸 할 수 있습니다. 
        */
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 지정해 주면 고정비로 빌드가 되어 단말에서 지정 해상도로 출력이 된다.	
        Screen.SetResolution(1280, 720, true); // 1280 x 720 으로 조정

        //Screen.SetResolution(1920, 1080, true); // 1920 x 1080 으로 조정

        //Screen.SetResolution(Screen.width, (Screen.width / 2) * 3 ); // 2:3 비율로 개발시

        //Screen.SetResolution(Screen.width, Screen.width * 16 / 9,  true); // 16:9 로 개발시

        

    }

    // 어플리케이션이 Home 키가 눌려졌을때의 콜백되는 함수
    void OnApplicationPause(bool pause)
    {
        
        if (pause)
        {
            bPaused = true;
            Debug.Log("OnApplicationPause : " + pause);

            ThisGameIsEnd = true;

            EndGameAndLeaveRoom(0.5f);
        }
    }

    // PVP에서 사용하는 데드아이 총알의 시작 위치를 결정해준다.
    public int GetPVPDeadEyeStartIndex()
    {
        DeadEyeStartIndex = ((My_DeadEYeSTartIndex + _OpponentDeadEyeStartIndex) / 2);

        Debug.Log("DeadEyeStartIndex : " + DeadEyeStartIndex);
        return DeadEyeStartIndex;
    }

    // 현재 PVP 모드에서 적의 캐릭터 번호를 가지고 온다
    public int GetPVPOpponentCharNumber()
    {
        return OppenentCharNumber;
    }

    // 현재 PVP 모드에서 적의 총 번호를 가지고 온다
    public int GetPVPOpponentGunNumber()
    {
        return OpponentGunNumber;
    }
    
    // 현재 PVP 모드에서 적의 총 번호를 가지고 온다
    public void SetPVPOpponentGunNumber(int number = 100)
    {
        if(number > 100)
        {
            OpponentGunNumber = 100;
        }
        else if(number < 0)
        {
            OpponentGunNumber = 0;
        }
        else
        {
            OpponentGunNumber = number;
        }
        
    }

    // 현재 서바이벌 모드에서 나의 랭크
    public int GetMySurvivalRankNumber()
    {
        return MySurvivalRank;
    }
    
    // 내 자신의 캐릭터 고유 번호를 반환
    public int GetMyCharNumber()
    {
        return MyCharNumber;
    }

    // 내 자신의 총 고유 번호를 반환
    public int GetMyGunNumber()
    {
        return MyGunNumber;
    }

    // 내 자신의 총 고유 번호를 설정
    public void SetMyGunNumber(int number = 100)
    {
        if(number < 0)
        {
            MyGunNumber = 0;
        }
        else if(number > 100)
        {
            MyGunNumber = 100;
        }
        else
        {
            MyGunNumber = number;
        }
        
    }

    #region Survival Function

    // 서바이벌 모드에서 사용하는 상대방의 Wait 신호 정보들..
    public Dictionary<string, bool> GetSurvivalOpponentWaitSignals()
    {
        Debug.logger.Log("SVM", "Survival Wait Map Count : " + _SurvivalOpponentWaitSignals.Count);
        Debug.Log("SVM Survival Wait Map Count : " + _SurvivalOpponentWaitSignals.Count);

        return _SurvivalOpponentWaitSignals;
    }

    // 서바이벌 모드에서 사용하는 상대방의 Select 신호 정보들..
    public Dictionary<string, bool> GetSurvivalOpponentSelectSignals()
    {
        Debug.logger.Log("SVM", "Survival Select Map Count : " + _SurvivalOpponentSelectSignals.Count);
        Debug.Log("SVM Survival Select Map Count : " + _SurvivalOpponentSelectSignals.Count);

        return _SurvivalOpponentSelectSignals;
    }

    // 서바이벌 모드에서 사용하는 상대방의 무기 번호들
    public Dictionary<string, int> GetSurvivalOpponentWeaponNumbers()
    {
        Debug.logger.Log("SVM", "Survival Weapon Map Count : " + _SurvivalOpponentWeaponNumber.Count);
        Debug.Log("Survival Weapon Map Count : " + _SurvivalOpponentWeaponNumber.Count);

        return _SurvivalOpponentWeaponNumber;
    }

    // 서바이벌 모드에서 사용하는 상대방의 캐릭터 번호들
    public Dictionary<string, int> _SurvivalOpponentCharacterNumbers()
    {
        Debug.logger.Log("SVM", "Survival Char Map Count : " + _SurvivalOpponentCharacterNumber.Count);
        Debug.Log("Survival Char Map Count : " + _SurvivalOpponentCharacterNumber.Count);

        return _SurvivalOpponentCharacterNumber;
    }

    // 서바이벌 모드에서 사용하는 모든 플레이어들의 ID를 반환
    public Dictionary<int, string> __SurvivalPlayersID_Dictionary()
    {
        return _SurvivalPlayersID;
    }

    // 서바이벌 모드에서 사용하는 모든 플레이어들의 랭크 정보를 반환
    public Dictionary<string, int> _SurvivalPlayers_Rank_Number()
    {
        return _SurvivalPlayersRank;
    }

    // 서바이벌 모드에서 사용하는 모든 플레이어들의 닉네임
    public Dictionary<string, string> _SurvivalPlayer_Nick()
    {
        return PlayerCharacters_Nick;
    }



    // 서바이벌 모드에서 사용하는 상대방의 Wait 신호 카운트를 불(Bool)형으로 반환해준다.
    // 반환 여부에 따라 다른 플레이어들이 모두 준비 되었는지를 알 수 있다.
    public bool GetSurvivalOpponentWaitSignals_Ready()
    {
        bool All_Wait_Ready = true;

        Dictionary<string, bool> WaitDiction = _SurvivalOpponentWaitSignals;
        IDictionaryEnumerator WaitIter = WaitDiction.GetEnumerator();

        while(WaitIter.MoveNext())
        {
            if(WaitDiction[WaitIter.Key.ToString()] == false)
            {
                All_Wait_Ready = false;
                break;
            }
        }

        if (All_Wait_Ready)
        {
            Debug.logger.Log("SVM", "Survival Wait Signals Ready : " + All_Wait_Ready);
            Debug.Log("Survival Wait Signals Ready : " + All_Wait_Ready);
        }


        return All_Wait_Ready;
    }

    // 서바이벌 모드에서 사용하는 상대방의 Select 신호 카운트를 불(Bool)형으로 반환해준다.
    // 반환 여부에 따라 다른 플레이어들이 모두 준비 되었는지를 알 수 있다.
    public bool GetSurvivalOpponentSelectSignals_Ready()
    {
        bool All_Select_Ready = true;

        Dictionary<string, bool> Select_Diction = _SurvivalOpponentSelectSignals;
        IDictionaryEnumerator Select_Iter = Select_Diction.GetEnumerator();

        while (Select_Iter.MoveNext())
        {
            if (Select_Diction[Select_Iter.Key.ToString()] == false)
            {
                All_Select_Ready = false;
                break;
            }
        }

        if(All_Select_Ready)
        {
            Debug.logger.Log("SVM", "Survival Select Signals Ready: " + All_Select_Ready);
            Debug.Log("Survival Select Signals Ready : " + All_Select_Ready);
        }
        

        return All_Select_Ready;
    }

    /// <summary>
    ///  서바이벌 모드에서 사용하는 상대방의 무기 번호를 정수형으로 반환 해준다.
    ///  index 값에 따라서 현재 방 안에 존재하는 플레이어(자기자신을 제외한)의 무기 번호를 알 수 있다.
    ///  디폴트 값 0
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int GetSurvivalOpponentWeaponNumber(int index = 0)
    {
        int WeaponNumber = 0;

        if(index >= (allPlayers.Count-1))
        {
            index = (allPlayers.Count-1);
        }
        else if(index <= 0)
        {
            index = 0;
        }

        if(_SurvivalOpponentWeaponNumber.ContainsKey(allPlayers[index].ParticipantId))
        {
            WeaponNumber = _SurvivalOpponentWeaponNumber[allPlayers[index].ParticipantId];
        }
        else
        {
            WeaponNumber = 0;
        }

        Debug.logger.Log("SVM", "Survival Weapon " + index + " Num: " + WeaponNumber);
        Debug.Log("Survival Weapon " + index + " Num : " + WeaponNumber);

        return WeaponNumber;
    }

    /// <summary>
    /// 서바이벌 모드에서 사용하는 상대방의 캐릭터 번호를 정수형으로 반환 해 준다.
    /// index 값에 따라서 현재 방 안에 존재하는 플레이어(자기자신을 제외한)의 캐릭터 번호를 알 수 있다.
    /// 디폴트 값 0
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int GetSurvivalOpponentCharacterNumber(int index = 0)
    {
        int CharNumber = 0;

        if (index >= (allPlayers.Count-1))
        {
            index = (allPlayers.Count-1);
        }
        else if (index <= 0)
        {
            index = 0;
        }

        if (_SurvivalOpponentCharacterNumber.ContainsKey(allPlayers[index].ParticipantId))
        {
            CharNumber = _SurvivalOpponentCharacterNumber[allPlayers[index].ParticipantId];
        }
        else
        {
            CharNumber = 0;
        }

        Debug.logger.Log("SVM", "Survival Char " + index + " Num : " + CharNumber);
        Debug.Log("SVM Survival Char " + index + " Num : " + CharNumber);

        return CharNumber;
    }

    // 서바이벌 모드에서 사용하는 랭킹 번호를 반환한다.
    // ID를 입력하면 해당하는 플레이어의 번호 정보를 저장한다.
    public int GetSurvivalPlayersRankNumber(string ID)
    {
        int RankNumber = 0;

        if (_SurvivalPlayersRank.ContainsKey(ID))
        {
            RankNumber = _SurvivalOpponentCharacterNumber[ID];
            Debug.Log("Players Rank ID : " + ID + " Num : " + RankNumber);
        }


        return RankNumber;
    }

    // 서바이벌 모드에서 ID에 해당하는 플레이어의 닉네임을 알 수 있다.
    public string GetSurvivalPlayerCharacters_Nick(string ID)
    {
        string Nick = "";

        if(PlayerCharacters_Nick.ContainsKey(ID))
        {
            Nick = PlayerCharacters_Nick[ID];
            Debug.Log("Players Nick ID : " + ID + " Num : " + Nick);
        }

        return Nick;
    }

    /// <summary>
    /// 서바이벌 모드에 접속해 있는 플레이어들의 수를 알 수 있다.
    /// 자기 자신을 포함한 수이다.
    /// </summary>
    /// <returns></returns>
    public int GetSurvivalPlayers_Count()
    {
        //Debug.Log("Player Count : " + allPlayers.Count);

        return (allPlayers.Count);

    }
    
    /// <summary>
    /// 서바이벌 모드에 접속해 있는 플레이어들의 ID 값을 알 수 있다.
    /// 매개변수는 순차번호 간의 ID값을 반환해준다.
    /// </summary>
    /// <returns></returns>
    public string GetSurvivalPlayersID(int index = 0)
    {
        string ID = "";

        if (index >= allPlayers.Count)
        {
            index = allPlayers.Count;
        }
        else if(index <= 0)
        {
            index = 0;
        }


        ID = _SurvivalPlayersID[index];

        return ID;
    }

    /// <summary>
    /// 서바이벌 모드에 접속해 있는 플레이어들의 고유번호를 알 수 있다.
    /// 매개변수는 해당 플레이어의 ID값을 기반으로 고유 번호를 반환한다.
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public int GetSurvivalPlayersID_Number(string ID)
    {
        int Number = -1;

        Dictionary<int, string> PlayerID_Diction = _SurvivalPlayersID;
        IDictionaryEnumerator PlayerID_Iter = PlayerID_Diction.GetEnumerator();

        while (PlayerID_Iter.MoveNext())
        {
            if(PlayerID_Iter.Value.Equals(ID))
            {
                Number = (int)PlayerID_Iter.Key;
                break;
            }
        }


        return Number;
    }

    /// <summary>
    /// 서바이벌 모드에 접속한 나의 고유 번호를 반환해준다.
    /// </summary>
    /// <returns></returns>
    public int GetMyPlayerIDNumber()
    {
        return _MyParticipantId_Index;
    }

    #endregion

    //void OnGUI()
    //{
    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(w / 2, h - 500, 100, 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = 30;
    //    style.normal.textColor = new Color(0.0f, 0.0f, 1.5f, 1.5f);

    //    //string text = string.Format("HP : {0}", HP);
    //    string text = string.Format("MyCharNum : {0}\nEnemyNum : {1}\nMultiReady : {2}\nEnemyWeapon : {3}\nMyWeapon : {4}\nSelect : {5}\nConfirm : {6}", MyCharNumber, OppenentCharNumber, _multiplayerReady
    //        , OpponentGunNumber, MyGunNumber, SelectSignal, GPGSManager.GetInstance.IsAuthenticated());

    //    GUI.Label(rect, text, style);
    //}


    // 현재 게임이 끝났는지의 여부를 리턴해 준다.
    // true면 끝, false면 끝나지 않음
    public bool GetEndGameState()
    {
        return ThisGameIsEnd;
    }



    // 게임 종료 메시지
    // 플레이어가 임의로 종료 하거나 게임이 끝나면 호출 해주면 된다.
    // float 값은 최소 1.5초, 최대 5.0초로 로비 씬으로 나가기까지의 대기 시간이다.
    // 디폴트는 2.0초로 되어있다.
    public void EndGameAndLeaveRoom(float dTime = 2.0f)
    {
        if(MultiStartChecker == false)
        {
            MultiStartChecker = true;

            GPGSManager.GetInstance.LeaveGame();
            //GPGSManager.GetInstance.LeftRoomInit();
            GPGSManager.GetInstance.updateListener = null;
            GPGSManager.GetInstance.LBListener = null;

            ThisGameIsEnd = true;

            switch (MultiGameModeState)
            {
                case HY.MultiGameModeState.NONE:
                    {
                        //GPGSManager.GetInstance.ReMatchingInit(0);
                        

                        if (dTime <= 0.5f)
                        {
                            Invoke("StartLobbyScene", 0.5f);
                        }
                        else if (dTime >= 5.0f)
                        {
                            Invoke("StartLobbyScene", 5.0f);
                        }
                        else
                        {
                            Invoke("StartLobbyScene", dTime);
                        }
                    }
                    break;

                case HY.MultiGameModeState.PVP:
                    {
                        //GPGSManager.GetInstance.ReMatchingInit(1);

                        if (dTime <= 0.5f)
                        {
                            Invoke("StartLobbyScene", 0.5f);
                        }
                        else if (dTime >= 5.0f)
                        {
                            Invoke("StartLobbyScene", 5.0f);
                        }
                        else
                        {
                            Invoke("StartLobbyScene", dTime);
                        }
                    }
                    break;

                case HY.MultiGameModeState.SURVIVAL:
                    {
                        //GPGSManager.GetInstance.ReMatchingInit(2);

                        if (dTime <= 1.5f)
                        {
                            Invoke("StartLobbyScene", 0.5f);
                        }
                        else if (dTime >= 5.0f)
                        {
                            Invoke("StartLobbyScene", 5.0f);
                        }
                        else
                        {
                            Invoke("StartLobbyScene", dTime);
                        }
                    }
                    break;
            }

            //if (dTime <= 1.5f)
            //{
            //    Invoke("StartLobbyScene", 1.5f);
            //}
            //else if (dTime >= 5.0f)
            //{
            //    Invoke("StartLobbyScene", 5.0f);
            //}
            //else
            //{
            //    Invoke("StartLobbyScene", dTime);
            //}

        }

    }

    // 게임 리 매치 함수
    // 게임이 끝나고 다시 재매치를 하고 싶다면 호출해준다.
    // float 값은 최소 1.5초, 최대 5.0초로 리매치 로딩 씬으로 나가기까지의 대기 시간이다.
    // 디폴트는 2.0초로 되어있다.
    public void EndGameRematchingGame(float dTime = 2.0f)
    {
        if (MultiStartChecker == false)
        {
            MultiStartChecker = true;

            GPGSManager.GetInstance.LeaveGame();
            GPGSManager.GetInstance.updateListener = null;
            GPGSManager.GetInstance.LBListener = null;

            ThisGameIsEnd = true;

            switch (MultiGameModeState)
            {
                case HY.MultiGameModeState.NONE:
                    {
                        GPGSManager.GetInstance.ReMatchingInit(0);

                        if (dTime <= 1.5f)
                        {
                            Invoke("StartRemathcingScene", 1.5f);
                        }
                        else if (dTime >= 5.0f)
                        {
                            Invoke("StartRemathcingScene", 5.0f);
                        }
                        else
                        {
                            Invoke("StartRemathcingScene", dTime);
                        }
                    }
                    break;

                case HY.MultiGameModeState.PVP:
                    {
                        GPGSManager.GetInstance.ReMatchingInit(1);

                        if (dTime <= 1.5f)
                        {
                            Invoke("StartRemathcingScene", 1.5f);
                        }
                        else if (dTime >= 5.0f)
                        {
                            Invoke("StartRemathcingScene", 5.0f);
                        }
                        else
                        {
                            Invoke("StartRemathcingScene", dTime);
                        }
                    }
                    break;

                case HY.MultiGameModeState.SURVIVAL:
                    {
                        GPGSManager.GetInstance.ReMatchingInit(2);

                        if (dTime <= 1.5f)
                        {
                            Invoke("StartRemathcingScene", 1.5f);
                        }
                        else if (dTime >= 5.0f)
                        {
                            Invoke("StartRemathcingScene", 5.0f);
                        }
                        else
                        {
                            Invoke("StartRemathcingScene", dTime);
                        }
                    }
                    break;
            }
        }

       

    }

    void StartLobbyScene()
    {
        AutoFade.LoadLevel("WaitingRoom", 0.2f, 0.2f, Color.black);
    }

    void StartRemathcingScene()
    {
        AutoFade.LoadLevel("ReMatchingRoom", 0.2f, 0.2f, Color.black);
    }

    // PVP 게임 재시작 코루틴
    IEnumerator StartPVPMultiGame()
    {
        yield return new WaitForSeconds(2.0f);

        AutoFade.LoadLevel("GameScene", 0.1f, 0.1f, Color.black);
    }

    // 서바이벌 게임 재시작 코루틴
    IEnumerator StartSurvivalMultiGame()
    {
        yield return new WaitForSeconds(2.0f);

        AutoFade.LoadLevel("GameScene", 0.1f, 0.1f, Color.black);
    }

    // GPGSManager(서버)에서 받은 메시지를 매니저에게 줄때 사용한다.
    // [서버]->[클라이언트]
    #region GPGS_CallBack_Interface

    // 맨 처음 게임 접속 대기 상태를 받는 콜백 함수이다.
    // 
    public void MultiStateWaitReceived(string participantId, bool Wait)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        WaitSignal = Wait;
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if(_multiplayerReady)
                    {
                        _SurvivalOpponentWaitSignals[participantId] = Wait;
                    }
                }
                break;
        }

        //if (_multiplayerReady)
        //{
        //    WaitSignal = Wait;
        //}
    }

    public void MultiStateSelectReceived(string participantId, bool Select)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        SelectSignal = Select;
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if(_multiplayerReady)
                    {
                        _SurvivalOpponentSelectSignals[participantId] = Select;
                    }
                }
                break;
        }

        //if (_multiplayerReady)
        //{
        //    SelectSignal = Select;
        //}
    }
    
    // 상대방이 고른 캐릭터의 고유 번호를 받는다
    //public void CharacterSelectStateReceived(int CharacterNumber)
    //{
    //    switch (MultiGameModeState)
    //    {
    //        case HY.MultiGameModeState.NONE:
    //            {

    //            }
    //            break;

    //        case HY.MultiGameModeState.PVP:
    //            {
    //                if (_multiplayerReady)
    //                {
    //                    //if(CharacterNumber < 0)
    //                    //{
    //                    //    OppenentCharNumber = 100;
    //                    //}
    //                    //else if(CharacterNumber > 100)
    //                    //{
    //                    //    OppenentCharNumber = 100;
    //                    //}
    //                    //else
    //                    //{
    //                    //    OppenentCharNumber = CharacterNumber;
    //                    //}
                        
    //                    //EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

    //                    //if (opponent != null)
    //                    //{
    //                    //    opponent.SetCharacterSelectState(CharacterNumber);
    //                    //}
    //                }


    //                if(CharacterNumber < 0)
    //                {
    //                    OppenentCharNumber = 100;
    //                }
    //                else if(CharacterNumber > 100)
    //                {
    //                    OppenentCharNumber = 100;
    //                }
    //                else
    //                {
    //                    OppenentCharNumber = CharacterNumber;
    //                }
    //            }
    //            break;

    //        case HY.MultiGameModeState.SURVIVAL:
    //            {

    //            }
    //            break;
    //    }

    //    //if (_multiplayerReady)
    //    //{
    //    //    OppenentCharNumber = CharacterNumber;

    //    //    //EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

    //    //    //if (opponent != null)
    //    //    //{
    //    //    //    opponent.SetCharacterSelectState(CharacterNumber);
    //    //    //}
    //    //}

    //}

    // 상대방이 선택한 무기의 고유 번호를 받는다.
    public void WeaponSelectStateReceived(string participantId, int WeaponNumber)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        //if (WeaponNumber < 0)
                        //{
                        //    OpponentGunNumber = 100;
                        //}
                        //else if (WeaponNumber > 100)
                        //{
                        //    OpponentGunNumber = 100;
                        //}
                        //else
                        //{
                        //    OpponentGunNumber = WeaponNumber;
                        //}

                        //EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

                        //if (opponent != null)
                        //{
                        //    opponent.SetWeaponSelectState(WeaponNumber);
                        //}
                    }

                    if (WeaponNumber < 0)
                    {
                        OpponentGunNumber = 100;
                    }
                    else if (WeaponNumber >= 100)
                    {
                        OpponentGunNumber = 100;
                    }
                    else
                    {
                        OpponentGunNumber = WeaponNumber;
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {
                        _SurvivalOpponentWeaponNumber[participantId] = WeaponNumber;

                        Debug.logger.Log("SVM", "Survival ID : " + participantId + " Weapon : " + _SurvivalOpponentWeaponNumber[participantId]);
                        Debug.Log("SVM Survival ID : " + participantId + " Weapon : " + _SurvivalOpponentWeaponNumber[participantId]);
                        //ItemCount++;
                    }
                }
                break;
        }

        //if (_multiplayerReady)
        //{
        //    if (WeaponNumber < 0)
        //    {
        //        OppenentCharNumber = 0;
        //    }
        //    else if (WeaponNumber > 100)
        //    {
        //        OppenentCharNumber = 100;
        //    }
        //    else
        //    {
        //        OpponentGunNumber = WeaponNumber;
        //    }


        //    //EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

        //    //if (opponent != null)
        //    //{
        //    //    opponent.SetWeaponSelectState(WeaponNumber);
        //    //}
        //}
    }

    // 끝내기 메시지를 보내주는 녀석이다.
    public void FinishedReceived(string participantId, bool GameOver)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {

                    if (_multiplayerReady)
                    {
                        if (ThisGameIsEnd == false)
                        {

                            ThisGameIsEnd = GameOver;

                            EnemyMove opponent = _opponentScripts[participantId];

                            if (opponent != null)
                            {
                                opponent.SetEndGameInformation(GameOver);
                            }

                        }

                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {
                        if (ThisGameIsEnd == false)
                        {

                            //ThisGameIsEnd = GameOver;

                            EnemyMove opponent = _opponentScripts[participantId];

                            if (opponent != null)
                            {
                                opponent.SetEndGameInformation(GameOver);
                            }

                            if(LeftPlayerCount <= 0)
                            {
                                ThisGameIsEnd = true;
                            }
                            else
                            {
                                LeftPlayerCount--;
                            }

                            Debug.Log("Now Player Count : " + LeftPlayerCount);
                        }

                    }
                }
                break;
        }



    }

    // 업데이트를 해줄 정보들...
    public void UpdatePositionReceived(string participantId, int messageNum, float posX, float posY, float posZ, float rotY)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[participantId];

                        if (opponent != null)
                        {


                            opponent.SetTransformInformation(messageNum, posX, posY, posZ, rotY);
                        }

                        ItemCount++;
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[participantId];

                        if (opponent != null)
                        {


                            opponent.SetTransformInformation(messageNum, posX, posY, posZ, rotY);
                        }

                        //ItemCount++;
                    }
                }
                break;
        }


    }

    // 아이템들의 정보들을 업데이트 해준다.
    public void ItemStateReceived(string participantId, int Index, bool GetItem)
    {
        if (_multiplayerReady)
        {
            // 아이템 처리를 해주세요...
        }
    }

    // 사격 상태를 업데이트 해준다.
    // bool 값이 true면 발사를 성공한것입니다.
    public void ShootStateReceived(string participantId, bool ShootSuccess)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

                        if (opponent != null)
                        {
                            opponent.SetShootStateReceived(ShootSuccess);
                        }
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[participantId];

                        if (opponent != null)
                        {
                            opponent.SetShootStateReceived(ShootSuccess);
                        }
                    }
                }
                break;
        }

    }

    // 상대방의 총알 방향을 알려주는 메시지입니다.
    // 각각의 좌표에 총알 방향의 노말 값을 넣어주면 됩니다.
    public void ShootVectorReceived(string participantId, float x, float y, float z)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

                        if (opponent != null)
                        {
                            opponent.SetShootVectorReceived(x, y, z);
                        }
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[participantId];

                        if (opponent != null)
                        {
                            opponent.SetShootVectorReceived(x, y, z);
                        }
                    }
                }
                break;
        }

    }

    // 현재 데드아이 상태를 업데이트 해준다.
    // true면 데드 아이 발동을 의미
    // false면 데드 아이 발동이 끝남 혹은 안됨..
    public void DeadEyeStateReceived(string participantId, bool DeadEyeActive, int DeadEyeRespawnIndex)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        _DeadEyeChecker = DeadEyeActive;
                        _DeadEyeRespawnIndex = DeadEyeRespawnIndex;

                        EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

                        if (opponent != null)
                        {
                            opponent.SetDeadEyeStateReceived(_DeadEyeChecker);
                        }
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    //if (_multiplayerReady)
                    //{
                    //    _DeadEyeChecker = DeadEyeActive;

                    //    EnemyMove opponent = _opponentScripts[participantId];

                    //    if (opponent != null)
                    //    {
                    //        opponent.SetDeadEyeStateReceived(_DeadEyeChecker);
                    //    }
                    //}
                }
                break;
        }

    }

    // 현재 데드아이 상태를 업데이트 해준다.
    // float는 데드 아이 경과 시간을 의미한다.
    public void DeadEyeTimerStateReceived(string participantId, float DeadEyeTimer)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        _DeadEyeTimer = DeadEyeTimer;

                        EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

                        if (opponent != null)
                        {
                            opponent.SetDeadEyeTimerReceived(_DeadEyeTimer);
                        }
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    //if (_multiplayerReady)
                    //{
                    //    _DeadEyeTimer = DeadEyeTimer;

                    //    EnemyMove opponent = _opponentScripts[participantId];

                    //    if (opponent != null)
                    //    {
                    //        opponent.SetDeadEyeTimerReceived(_DeadEyeTimer);
                    //    }
                    //}
                }
                break;
        }


    }

    // 데드아이 아이템이 리스폰 되어야할 위치를 받는다.
    // index는 위치로써, 서버 쪽에서 난수 값으로 0~4까지 만든 후 보내준다.
    public void DeadEyeRespawnIndexReceived(string participantId, int index)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        //_DeadEyeRespawnIndex = index;
                        //_DeadEyeStartIndexMap[participantId] = index;
                        _OpponentDeadEyeStartIndex = index;
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {

                }
                break;
        }


    }

    // 서바이벌 모드 랭크 번호를 받는 콜백 메시지 
    public void SurvivalRankMessageReceived(string participantId, int RankNumber)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {

                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {
                        _SurvivalPlayersRank[participantId] = RankNumber;
                        Debug.Log("Now Rank Received ID : " + participantId + " Num : " + RankNumber);
                    }
                }
                break;
        }
    }

    #region Survival Boss Received Callback

    // 서바이벌 모드에서 쓰일 보스 이벤트 리시버

    // 보스의 HP 값을 받는다
    public void BossHPStateReceived(int HPState)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {

                    }
                }
                break;
        }
    }

    // 보스의 애니메이션 값을 받는다
    public void BossAnimStateReceived(int AnimState)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {

                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {

                    }
                }
                break;
        }
    }

    // 보스의 사망 이벤트를 받는다
    public void BossDeadEventReceived(bool BossDead)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {

                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {

                    }
                }
                break;
        }
    }

    // 보스의 위치값 x, y, z, 그리고 y축 회전값을 받는다.
    public void BossPosReceived(int messageNum, float x, float y, float z, float rotY)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {

                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {

                    }
                }
                break;
        }
    }

    public void BossRaidAlarmReceived(bool Alarm)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {

                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {
                        BossEvent = Alarm;
                    }
                }
                break;
        }
        
    }

    #endregion

    // 데드아이 타이머를 반환 시켜준다.
    public float GetDeadEyeTimer()
    {
        return _DeadEyeTimer;
    }

    // 데드아이가 발동 됬는지의 여부
    public bool GetDeadEyeChecker()
    {
        return _DeadEyeChecker;
    }

    // 데드아이 총알 아이템의 인덱스 위치 여부
    public int GetDeadEyeRespawnIndex()
    {
        return _DeadEyeRespawnIndex;
    }

    // 현재 애니메이션 상태 값을 갱신 시켜준다.
    // AniState의 값을 다시 변환 시켜서 넘겨준다.
    public void AniStateReceived(string participantId, int AniState)
    {
        //플레이어 상태
        //enum PlayerState
        //{
        //    IDLE,
        //    DASH_SLOW,
        //    DASH_SOFT,
        //    DASH_HARD,
        //    SHOT_READY,
        //    SHOT_FIRE,
        //    DAMAGE,
        //    DEADEYE,
        //    REROAD
        //}

        ////총 상태(추후 추가)
        //enum GunState
        //{
        //    Revolver=0,
        //    ShotGun
        //}

        switch(AniState)
        {
            // IDLE
            case 0:
                {
                    m_state = LSD.PlayerState.IDLE;
                }
                break;

            // DASH_SLOW
            case 1:
                {
                    m_state = LSD.PlayerState.DASH_SLOW;
                }
                break;
            
            // DASH_SOFT
            case 2:
                {
                    m_state = LSD.PlayerState.DASH_SOFT;
                }
                break;

                // DASH_HARD
            case 3:
                {
                    m_state = LSD.PlayerState.DASH_HARD;
                }
                break;

            // SHOT_READY
            case 4:
                {
                    m_state = LSD.PlayerState.SHOT_READY;
                }
                break;

                // SHOOT_FIRE
            case 5:
                {
                    m_state = LSD.PlayerState.SHOT_FIRE;
                }
                break;

                // DAMEGE
            case 6:
                {
                    m_state = LSD.PlayerState.DAMAGE;
                }
                break;

                // DEADEYE
            case 7:
                {
                    m_state = LSD.PlayerState.DEADEYE;
                }
                break;

                // REROAD
            case 8:
                {
                    m_state = LSD.PlayerState.REROAD;
                }
                break;

            case 9:
                {
                    m_state = LSD.PlayerState.ROLL;
                }
                break;

            case 10:
                {
                    m_state = LSD.PlayerState.DEAD;
                }
                break;

            case 11:
                {
                    m_state = LSD.PlayerState.WIN;
                }
                break;
        }

        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

                        if (opponent != null)
                        {
                            opponent.SetAniStateReceived(AniState);
                        }
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[participantId];

                        if (opponent != null)
                        {
                            opponent.SetAniStateReceived(AniState);
                        }
                    }
                }
                break;
        }


    }

    // HP의 정보를 동기화 시키는 역할
    // HPState에 넘어가는 값에 따라 상대방에게 보여지는 HP가 틀려진다.
    public void HPStateReceived(string participantId, int HPState)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

                        if (opponent != null)
                        {
                            opponent.SetHPStateReceived(HPState);
                        }
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[participantId];

                        if (opponent != null)
                        {
                            opponent.SetHPStateReceived(HPState);
                        }
                    }
                }
                break;
        }

    }

    // 게임이 끝날시 호출되는 리스너 함수.
    public void LeftRoomConfirmed()
    {
        //GPGSManager.GetInstance.updateListener = null;

        //AutoFade.LoadLevel("WaitingRoom", 0.2f, 0.2f, Color.black);

        
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    ThisGameIsEnd = true;
                    GPGSManager.GetInstance.updateListener = null;
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    //if(LeftPlayerCount > 0)
                    //{
                    //    LeftPlayerCount--;
                    //}
                    //else
                    //{
                    //    ThisGameIsEnd = true;
                    //}

                    Debug.Log("Player Left Room");

                    if(ThisGameIsEnd == false)
                    {
                        ThisGameIsEnd = true;

                        SendEndGameMssage(true);
                        
                    }
                    

                    //GPGSManager.GetInstance.updateListener = null;

                }
                break;
        }


    }

    // 타임 아웃 체크
    void CheckForTimeOuts()
    {
        foreach (string participantId in _opponentScripts.Keys)
        {
            //// We can skip anybody who's finished.
            //if (_finishTimes[participantId] < 0)
            //{
            //    if (_opponentScripts[participantId].lastUpdateTime < Time.time - timeOutThreshold)
            //    {
            //        // Haven't heard from them in a while!
            //        Debug.Log("Haven't heard from " + participantId + " in " + timeOutThreshold +
            //                  " seconds! They're outta here!");

            //        PlayerLeftRoom(participantId);
            //    }
            //}

            if (_opponentScripts[participantId].lastUpdateTime < Time.time - timeOutThreshold)
            {
                // Haven't heard from them in a while!
                Debug.Log("Haven't heard from " + participantId + " in " + timeOutThreshold +
                          " seconds! They're outta here!");

                PlayerLeftRoom(participantId);
            }
        }
    }

    // 게임을 임의적으로 종료됬을때 호출되는 리스너 함수
    // 주로 강제 종료될때 호출된다.
    public void PlayerLeftRoom(string participantId)
    {
        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {

                        ThisGameIsEnd = true;

                        EnemyMove opponent = _opponentScripts[participantId];

                        if (opponent != null)
                        {
                            opponent.GameOutInformation();
                            Debug.Log("Player ID : " + participantId + " Left Game");
                        }

                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {


                    if (_multiplayerReady)
                    {
                        EnemyMove opponent = _opponentScripts[participantId];

                        if (opponent != null)
                        {
                            opponent.GameOutInformation();
                            Debug.Log("Player ID : " + participantId + " Left Game");
                        }


                        if (LeftPlayerCount > 0)
                        {
                            LeftPlayerCount--;

                        }
                        else
                        {
                            ThisGameIsEnd = true;
                        }

                        Debug.Log("Now Player Count : " + LeftPlayerCount);

                        //ThisGameIsEnd = true;
                    }



                }
                break;
        }

    }

    #endregion GPGS_CallBack_Interface

    // 다른 스크립트에서 서버로 보낼 메시지를 호출하고 싶을때 보낼 콜 함수들...
    #region Call_Function

    //// 아이템을 먹을시 이것을 GPGSManager의 아이템 메시지 함수를 호출해준다.
    //// 사실상 이건 테스트 용이니 다른걸...
    //public void CallSendItemStateMessage()
    //{
    //    GPGSManager.GetInstance.SendItemStateMessage(0, true);
    //}

    //// 아이템을 먹을시 이것을 GPGSManager의 아이템 메시지 함수를 호출해준다.
    //// Index = 메모리 풀을 만들었을때 호출될 아이템 인덱스 값
    //// ItemGetOn = true일시 아이템을 먹었다고 판단하고 메시지를 전부 보내준다.
    //public void CallSendItemStateMessage(int index, bool ItemGetOn)
    //{
    //    GPGSManager.GetInstance.SendItemStateMessage(index, ItemGetOn);
    //}

    // 맨 처음 접속했을때 상대방의 접속 여부를 의미한다.
    // Wait 값이 true면 접속을 완료했다는 것
    public void SendMultiWaitStateMessage(bool Wait)
    {
        GPGSManager.GetInstance.SendStateWaitMesssage(Wait);
    }

    // 서로가 총을 고를때 카운트를 세기 시작한다
    // 동기화를 위한 함수로써 Select 값이 true면 접속을 완료했다는 것
    public void SendMultiSelectStateMessage(bool Select)
    {
        GPGSManager.GetInstance.SendStateSelectMesssage(Select);
    }

    // 자기자신의 캐릭터 고유 번호를 상대방에게 전송해준다.
    public void SendCharacterNumberMessage(int CharacterNumber)
    {
        GPGSManager.GetInstance.SendCharacterSelectNumber(CharacterNumber);

    }

    // 자기자신의 무기 고유 번호를 상대방에게 전송해준다.
    public void SendWeaponNumberMessage(int WeaponNumber)
    {
        GPGSManager.GetInstance.SendWeaponSelectNumber(WeaponNumber);

    }

    // 자기 자신의 HP값을 보내기 위해 사용된다
    // Int 값은 현재 자신의 HP
    public void SendHPStateMessage(int HP)
    {
        GPGSManager.GetInstance.SendCharacterHP(HP);
    }

    // 상대방의 총알 방향을 보내는 메시지다.
    // 각각의 좌표에 총알 방향 노말 벡터를 넣어주면 된다.
    public void SendShootVectorMessage(float x, float y, float z)
    {
        GPGSManager.GetInstance.SendShootVectorMessage(x, y, z);
    }

    // 상대방의 총알 방향을 보내는 메시지다.
    // 각각의 좌표에 총알 방향 노말 벡터를 넣어주면 된다.
    public void SendShootVectorMessage(Vector3 vec)
    {
        GPGSManager.GetInstance.SendShootVectorMessage(vec);
    }

    // 자신의 위치를 서버에 전송한다.
    // Position X, Y, Z, Rotation Y값
    public void SendMyPositionUpdate()
    {
        // In a multiplayer game, time counts up!
        //_timePlayed += Time.deltaTime;
        //guiObject.SetTime(_timePlayed);

        if(ThisGameIsEnd == false)
        {
            // 0.16초마다 보냄으로써 네트워크 트래픽 최적화를 할 수 있다.
            if (Time.time > _nextBroadcastTime)
            {
                // We will be doing more here
                GPGSManager.GetInstance.SendMyPositionUpdate(MyCharacter.transform.position.x,
                                                        MyCharacter.transform.position.y,
                                                        MyCharacter.transform.position.z,
                                                        MyCharacter.transform.rotation.eulerAngles.y);

                //_nextBroadcastTime = Time.time + 0.16f;
                _nextBroadcastTime = Time.time +  0.13f;
            }
        }



    }

    // 게임이 끝났음을 서버에 전송한다.
    // bool 값을 전송
    // true일시 게임이 끝났다고 전송된다.
    public void SendEndGameMssage(bool GameEnd)
    {

        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (ThisGameIsEnd == false)
                    {
                        ThisGameIsEnd = GameEnd;

                        GPGSManager.GetInstance.SendFinishMessage(ThisGameIsEnd);
                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (ThisGameIsEnd == false)
                    {
                        ThisGameIsEnd = GameEnd;

                        GPGSManager.GetInstance.SendFinishMessage(ThisGameIsEnd);
                    }
                }
                break;
        }

        


    }

    // 총알 발사 여부를 알려준다.
    // bool 값을 true면 발사를 성공 한 것입니다.
    public void SendShootMessage(bool ShootSuccess)
    {
        GPGSManager.GetInstance.SendShootMessage(ShootSuccess);
    }

    // 데드 아이 메시지를 보낸다.
    // bool 값으로 작동 여부를 보내줄 수 있다
    public void SendDeadEyeMessage(bool DeadEyeActive)
    {
        GPGSManager.GetInstance.SendDeadEyeMessage(DeadEyeActive);
    }

    // 데드 아이 메시지를 보낸다.
    // float 값으로 어느정도 시간이 지났는지 알 수 있다.
    public void SendDeadEyeTimerMessage(float DeadEyeTimer)
    {
        GPGSManager.GetInstance.SendDeadEyeTimerMessage(DeadEyeTimer);
    }


    // 데드 아이 아이템을 먹을 경우 메시지를 보낸다.
    // 다시 리스폰 되는 위치를 서버쪽에서 난수로 지정해준다.
    // 현재는 임시로 사용하지 않음
    private void SendDeadEyeRespawnIndexMessage()
    {
        int index = Random.Range(1, 4);
        GPGSManager.GetInstance.SendDeadEyeIndexMessage(index);
    }

    // 애니메이션 값을 넣어준다.
    // int값으로 바꿔줘야 한다
    public void SendAniStateMessage(int AniStateNumber)
    {
        if (AniStateNumber <= 0)
        {
            //AniStateNumber = 0;
            GPGSManager.GetInstance.SendAniStateMessage(0);
        }
        else if(AniStateNumber >= (int)LSD.PlayerState.MAX)
        {
            GPGSManager.GetInstance.SendAniStateMessage(9);
        }
        else
        {
            GPGSManager.GetInstance.SendAniStateMessage(AniStateNumber);
        }
        
    }

    // 서바이벌에서 사용하는 나의 랭크 번호를 전송한다.
    public void SendMyRankNumberMessage()
    {
        switch(MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {

                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    MySurvivalRank = ((allPlayers.Count) - ((allPlayers.Count) - (LeftPlayerCount)));
                    GPGSManager.GetInstance.SendSurvivalMyRankNumber(MySurvivalRank);
                }
                break;
        }
        
    }

    #region Survival Boss Send Event

    // 서바이벌 모드에서 보스 이벤트를 발동 시킬때 사용한다.
    public void SendBossAlarmMessage(bool Alarm)
    {
        GPGSManager.GetInstance.SendBossAlertEvent(Alarm);
    }

    public void SendBossAnimMessage(int AnimNumber)
    {
        GPGSManager.GetInstance.SendBossAnimation(AnimNumber);
    }

    public void SendBossDeadMessage(bool IsDead)
    {
        GPGSManager.GetInstance.SendBossDeadState(IsDead);

    }

    public void SendBossPosition(float posX, float posY, float posZ, float rotY)
    {
        if (ThisGameIsEnd == false)
        {
            if (BossEvent == true)
            {
                GPGSManager.GetInstance.SendBossPosition(posX, posY, posZ, rotY);
            }
        }
    }

    #endregion
    //// 애니메이션 값을 넣어준다.
    //// int값으로 바꿔줘야 한다
    //public void SendAniStateMessage(LSD.PlayerState AniState)
    //{
    //    switch (AniState)
    //    {
    //        // IDLE
    //        case LSD.PlayerState.IDLE:
    //            {
    //                //m_state = LSD.PlayerState.IDLE;
    //                GPGSManager.GetInstance.SendAniStateMessage(0);
    //            }
    //            break;

    //        // DASH_SLOW
    //        case LSD.PlayerState.DASH_SLOW:
    //            {
    //                //m_state = LSD.PlayerState.DASH_SLOW;
    //                GPGSManager.GetInstance.SendAniStateMessage(1);
    //            }
    //            break;

    //        // DASH_SOFT
    //        case LSD.PlayerState.DASH_SOFT:
    //            {
    //                //m_state = LSD.PlayerState.DASH_SOFT;
    //                GPGSManager.GetInstance.SendAniStateMessage(2);
    //            }
    //            break;

    //        // DASH_HARD
    //        case LSD.PlayerState.DASH_HARD:
    //            {
    //                //m_state = LSD.PlayerState.DASH_HARD;
    //                GPGSManager.GetInstance.SendAniStateMessage(3);
    //            }
    //            break;

    //        // SHOT_READY
    //        case LSD.PlayerState.SHOT_READY:
    //            {
    //                //m_state = LSD.PlayerState.SHOT_READY;
    //                GPGSManager.GetInstance.SendAniStateMessage(4);
    //            }
    //            break;

    //        // SHOOT_FIRE
    //        case LSD.PlayerState.SHOT_FIRE:
    //            {
    //                //m_state = LSD.PlayerState.SHOT_FIRE;
    //                GPGSManager.GetInstance.SendAniStateMessage(5);
    //            }
    //            break;

    //        // DAMEGE
    //        case LSD.PlayerState.DAMAGE:
    //            {
    //                //m_state = LSD.PlayerState.DAMAGE;
    //                GPGSManager.GetInstance.SendAniStateMessage(6);
    //            }
    //            break;

    //        // DEADEYE
    //        case LSD.PlayerState.DEADEYE:
    //            {
    //                //m_state = LSD.PlayerState.DEADEYE;
    //                GPGSManager.GetInstance.SendAniStateMessage(7);
    //            }
    //            break;

    //        // REROAD
    //        case LSD.PlayerState.REROAD:
    //            {
    //                //m_state = LSD.PlayerState.REROAD;
    //                GPGSManager.GetInstance.SendAniStateMessage(8);
    //            }
    //            break;
    //    }


    //}

    #endregion Call_Function

    // Update is called once per frame
    void Update ()
    {

        // 현재 게임 상태에 따라 다르게 진행을 해준다.
        //MultiGameModeState = GPGSManager.GetInstance.GetMultiGameModeState();

        switch (MultiGameModeState)
        {
            case HY.MultiGameModeState.NONE:
                {
                    NetText.text = "Net Info : 현재는 멀티 플레이 모드가 아닙니다.\nNet Mode : " + MultiGameModeState;
                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    if (_multiplayerReady)
                    {

                        if (ThisGameIsEnd == false)
                        {
                            NetText.text = "Net Info : " + MultiGameModeState;//GPGSManager.GetInstance.GetNetMessage().ToString();

                        }
                        else
                        {
                            NetText.text = "Net Info : 상대방이 연결을 해제했습니다.";
                        }

                        //PlayerName.text = "PlayerNick : " + MyPlayerNick + " / PlayerGun : " + MyGunNumber + " / PlayerChar : " + GPGSManager.GetInstance.GetMyCharacterNumber();
                        //EnemyName.text = "OppentNick : " + OpponentPlayerNick + " / OppenentGun : " + OpponentGunNumber + " / OppenentChar : " + GPGSManager.GetInstance.GetPVPOpponentCharNumber();

                        //MyInfoText.text = "MyCharacter Name : " + MyCharacter.transform.name + " / WaitSignal : " + WaitSignal + " / SelectSignal : " + SelectSignal;
                        //EnemyInfoText.text = "Enemy Name : " + EnemyCharacter.transform.name + " / ReMatchingOn : " + ReMatchingOn + " / GameMode : " + MultiGameModeState;//EnemyCharacterPos.GetComponent<EnemyMove>().m_DebugPlayerState;//EnemyCharacterPos.transform.position;
                        //ItemGetCount.text = "MessageCount : " + ItemCount + " / EndMsg : " + ThisGameIsEnd + " / Index : " + _DeadEyeRespawnIndex;

                        PlayerName.text = "";
                        EnemyName.text = "";

                        MyInfoText.text = "";
                        EnemyInfoText.text = "";
                        ItemGetCount.text = "";

                    }
                    else
                    {
                        NetText.text = "Net Info : " + MultiGameModeState;//GPGSManager.GetInstance.GetNetMessage().ToString();
                        PlayerName.text = "Player"; //GPGSManager.GetInstance.GetNameGPGS();
                        EnemyName.text = "Enemy";

                        MyInfoText.text = "Player Info : " + MyCharacter.transform.position;
                        EnemyInfoText.text = "Enemy Info : " + EnemyCharacter.transform.position;//EnemyCharacterPos.transform.position;
                        //NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();
                        ItemGetCount.text = "MessageCount : " + ItemCount + " / EndMsg : " + ThisGameIsEnd + " / Index : " + _DeadEyeRespawnIndex;
                    }

                    // 적의 타임아웃 체크
                    if (ThisGameIsEnd == false)
                    {
                        if (Time.time > _nextTimeoutCheck)
                        {

                            CheckForTimeOuts();
                            _nextTimeoutCheck = Time.time + _timeOutCheckInterval;
                        }
                    }

                    // 플레이어의 위치 동기화
                    SendMyPositionUpdate();
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (_multiplayerReady)
                    {

                        if (ThisGameIsEnd == false)
                        {
                            NetText.text = "Net Info : " + MultiGameModeState;// GPGSManager.GetInstance.GetNetMessage().ToString();

                        }
                        else
                        {
                            NetText.text = "Net Info : 상대방이 연결을 해제했습니다.";
                        }

                        //PlayerName.text = "PlayerNick : " + MyPlayerNick + " / PlayerGun : " + MyGunNumber + " / PlayerChar : " + GPGSManager.GetInstance.GetMyCharacterNumber();
                        //EnemyName.text = "Players Count : " + allPlayers.Count;

                        PlayerName.text = "";
                        EnemyName.text = "";

                        //EnemyName.text = "OppentNick[0] : " + allPlayers[0].ParticipantId + " / OppenentGun[0] : " + _SurvivalOpponentWeaponNumber[allPlayers[0].ParticipantId].ToString() + " / OppenentChar[0] : " + _SurvivalOpponentCharacterNumber[allPlayers[0].ParticipantId].ToString()
                        //    + "\nOppentNick[1] : " + allPlayers[1].ParticipantId + " / OppenentGun[1] : " + _SurvivalOpponentWeaponNumber[allPlayers[1].ParticipantId].ToString() + " / OppenentChar[1] : " + _SurvivalOpponentCharacterNumber[allPlayers[1].ParticipantId].ToString()
                        //    + "\nOppentNick[2] : " + allPlayers[2].ParticipantId + " / OppenentGun[2] : " + _SurvivalOpponentWeaponNumber[allPlayers[2].ParticipantId].ToString() + " / OppenentChar[2] : " + _SurvivalOpponentCharacterNumber[allPlayers[2].ParticipantId].ToString()
                        //    + "\nOppentNick[3] : " + allPlayers[3].ParticipantId + " / OppenentGun[3] : " + _SurvivalOpponentWeaponNumber[allPlayers[3].ParticipantId].ToString() + " / OppenentChar[3] : " + _SurvivalOpponentCharacterNumber[allPlayers[3].ParticipantId].ToString()
                        //    + "\nOppentNick[4] : " + allPlayers[4].ParticipantId + " / OppenentGun[4] : " + _SurvivalOpponentWeaponNumber[allPlayers[4].ParticipantId].ToString() + " / OppenentChar[4] : " + _SurvivalOpponentCharacterNumber[allPlayers[4].ParticipantId].ToString()
                        //    + "\nOppentNick[5] : " + allPlayers[5].ParticipantId + " / OppenentGun[5] : " + _SurvivalOpponentWeaponNumber[allPlayers[5].ParticipantId].ToString() + " / OppenentChar[5] : " + _SurvivalOpponentCharacterNumber[allPlayers[5].ParticipantId].ToString()
                        //    + "\nOppentNick[6] : " + allPlayers[6].ParticipantId + " / OppenentGun[6] : " + _SurvivalOpponentWeaponNumber[allPlayers[6].ParticipantId].ToString() + " / OppenentChar[6] : " + _SurvivalOpponentCharacterNumber[allPlayers[6].ParticipantId].ToString()
                        //    + "\nOppentNick[7] : " + allPlayers[7].ParticipantId + " / OppenentGun[7] : " + _SurvivalOpponentWeaponNumber[allPlayers[7].ParticipantId].ToString() + " / OppenentChar[7] : " + _SurvivalOpponentCharacterNumber[allPlayers[7].ParticipantId].ToString();

                        // 남은 플레이어 체크
                        if(LeftPlayerCountTimer <= 0.0f)
                        {
                            LeftPlayerCountTimer = 1.5f;
                            Debug.Log("Now LeftPlayerCount : " + LeftPlayerCount);
                        }
                        else
                        {
                            LeftPlayerCountTimer -= Time.deltaTime;
                        }

                        // 적의 타임아웃 체크
                        if (ThisGameIsEnd == false)
                        {
                            if (Time.time > _nextTimeoutCheck)
                            {

                                CheckForTimeOuts();
                                _nextTimeoutCheck = Time.time + _timeOutCheckInterval;
                            }
                        }

                        // 나의 랭크를 지속적으로 체크한다.
                        MySurvivalRank = ((allPlayers.Count) - ((allPlayers.Count) - (LeftPlayerCount)));

                        // 플레이어의 위치 동기화
                        SendMyPositionUpdate();

                        // 보스가 출현했다면 보스의 위치도 동기화 시켜준다.
                        if (BossEvent == true)
                        {
                            //SendBossPositionUpdate();
                        }
                    }
                    else
                    {
                        //NetText.text = "Net Info : " + MultiGameModeState;//GPGSManager.GetInstance.GetNetMessage().ToString();
                        //PlayerName.text = "Player"; //GPGSManager.GetInstance.GetNameGPGS();
                        //EnemyName.text = "Enemy";

                        //MyInfoText.text = "Player Info : " + MyCharacter.transform.position;
                        //EnemyInfoText.text = "Enemy Info : " + EnemyCharacter.transform.position;//EnemyCharacterPos.transform.position;
                        ////NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();
                        //ItemGetCount.text = "MessageCount : " + ItemCount + " / EndMsg : " + ThisGameIsEnd + " / Index : " + _DeadEyeRespawnIndex;

                        NetText.text = "";
                        PlayerName.text = "";
                        EnemyName.text = "";

                        MyInfoText.text = "";
                        EnemyInfoText.text = "";
                        //NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();
                        ItemGetCount.text = "";


                    }


                }
                break;
        }

        //if (_multiplayerReady)
        //{

        //    if (ThisGameIsEnd == false)
        //    {
        //        NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();

        //    }
        //    else
        //    {
        //        NetText.text = "Net Info : 상대방이 연결을 해제했습니다.";
        //    }

        //    PlayerName.text = "PlayerNick : " + MyPlayerNick + " / PlayerGun : " + MyGunNumber + " / PlayerChar : " + MyCharNumber;
        //    EnemyName.text = "OppentNick : " + OpponentPlayerNick + " / OppenentGun : " + OpponentGunNumber + " / OppenentChar : " + OppenentCharNumber;

        //    //MyInfoText.text = "Player Info : " + MyCharacterPos.GetComponent<CharMove>().m_DebugPlayerState;//MyCharacterPos.transform.position;
        //    //EnemyInfoText.text = "Enemy Info : " + EnemyCharacterPos.GetComponent<EnemyMove>().m_DebugPlayerState;//EnemyCharacterPos.transform.position;
        //    MyInfoText.text = "MyCharacter Name : " + MyCharacter.transform.name + " / WaitSignal : " + WaitSignal + " / SelectSignal : " + SelectSignal;

        //    EnemyInfoText.text = "Enemy Name : " + EnemyCharacter.transform.name + " / ReMatchingOn : " + ReMatchingOn + " / GameMode : " + MultiGameModeState;//EnemyCharacterPos.GetComponent<EnemyMove>().m_DebugPlayerState;//EnemyCharacterPos.transform.position;

        //    ItemGetCount.text = "MessageCount : " + ItemCount + " / EndMsg : " + ThisGameIsEnd + " / Index : " + _DeadEyeRespawnIndex;


        //}
        //else
        //{
        //    PlayerName.text = "Player"; //GPGSManager.GetInstance.GetNameGPGS();
        //    EnemyName.text = "Enemy";

        //    MyInfoText.text = "Player Info : " + MyCharacter.transform.position;
        //    EnemyInfoText.text = "Enemy Info : " + EnemyCharacter.transform.position;//EnemyCharacterPos.transform.position;
        //    NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();
        //    ItemGetCount.text = "MessageCount : " + ItemCount + " / EndMsg : " + ThisGameIsEnd + " / Index : " + _DeadEyeRespawnIndex;
        //}

        //// 적의 타임아웃 체크
        //if (ThisGameIsEnd == false)
        //{
        //    if (Time.time > _nextTimeoutCheck)
        //    {

        //        CheckForTimeOuts();
        //        _nextTimeoutCheck = Time.time + _timeOutCheckInterval;
        //    }
        //}


        //// 플레이어의 위치 동기화
        //SendMyPositionUpdate();

        #region ReMatchingTestZone

        //if (ReMatchingOn == false)
        //{
        //    if (_multiplayerReady)
        //    {
        //        //if (GPGSManager.GetInstance.GetOtherNameGPGS(1) == _MyParticipantId)
        //        //{
        //        //    PlayerName.text = GPGSManager.GetInstance.GetOtherNameGPGS(1);//_opponentScripts[_MyParticipantId].name;//GPGSManager.GetInstance.GetOtherNameGPGS(0);
        //        //    EnemyName.text = GPGSManager.GetInstance.GetOtherNameGPGS(0);
        //        //}
        //        //else
        //        //{
        //        //    PlayerName.text = GPGSManager.GetInstance.GetOtherNameGPGS(0);//GPGSManager.GetInstance.GetOtherNameGPGS(0);
        //        //    EnemyName.text = GPGSManager.GetInstance.GetOtherNameGPGS(1);
        //        //}


        //        if (ThisGameIsEnd == false)
        //        {
        //            NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();

        //        }
        //        else
        //        {
        //            NetText.text = "Net Info : 상대방이 연결을 해제했습니다.";
        //        }

        //        PlayerName.text = MyPlayerNick;
        //        EnemyName.text = OpponentPlayerNick;

        //        //MyInfoText.text = "Player Info : " + MyCharacterPos.GetComponent<CharMove>().m_DebugPlayerState;//MyCharacterPos.transform.position;
        //        //EnemyInfoText.text = "Enemy Info : " + EnemyCharacterPos.GetComponent<EnemyMove>().m_DebugPlayerState;//EnemyCharacterPos.transform.position;
        //        MyInfoText.text = "WaitSignal : " + WaitSignal + " / SelectSignal : " + SelectSignal + " / GameMode : " + MultiGameModeState;

        //        EnemyInfoText.text = "Enemy Info : " + EnemyCharacterPos.GetComponent<EnemyMove>().m_DebugPlayerState;//EnemyCharacterPos.transform.position;

        //        ItemGetCount.text = "MessageCount : " + ItemCount + " / EndMsg : " + ThisGameIsEnd + " / Index : " + _DeadEyeRespawnIndex;


        //    }
        //    else
        //    {
        //        PlayerName.text = "Player"; //GPGSManager.GetInstance.GetNameGPGS();
        //        EnemyName.text = "Enemy";

        //        MyInfoText.text = "Player Info : " + MyCharacterPos.transform.position;
        //        EnemyInfoText.text = "Enemy Info : " + EnemyCharacterPos.transform.position;
        //        NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();
        //        ItemGetCount.text = "MessageCount : " + ItemCount + " / EndMsg : " + ThisGameIsEnd + " / Index : " + _DeadEyeRespawnIndex;
        //    }

        //    // 적의 타임아웃 체크
        //    if (ThisGameIsEnd == false)
        //    {
        //        if (Time.time > _nextTimeoutCheck)
        //        {

        //            CheckForTimeOuts();
        //            _nextTimeoutCheck = Time.time + _timeOutCheckInterval;
        //        }
        //    }


        //    // 플레이어의 위치 동기화
        //    SendMyPositionUpdate();
        //}
        //else
        //{
        //    if (GPGSManager.GetInstance.IsConnected() == true)
        //    {
        //        if (MultiStartChecker == false)
        //        {
        //            MultiStartChecker = true;

        //            switch(MultiGameModeState)
        //            {
        //                case HY.MultiGameModeState.NONE:
        //                    {

        //                    }
        //                    break;


        //                case HY.MultiGameModeState.PVP:
        //                    {
        //                        StartCoroutine(StartPVPMultiGame());
        //                    }
        //                    break;

        //                case HY.MultiGameModeState.SURVIVAL:
        //                    {
        //                        StartCoroutine(StartSurvivalMultiGame());
        //                    }
        //                    break;

        //                default:
        //                    {

        //                    }
        //                    break;
        //            }

        //        }

        //    }
        //}
        #endregion


        #region BackUpState

        //	switch(MultiState)
        //       {
        //           case HY.MultiGameState.START:
        //               {
        //                   switch(PlayerState)
        //                   {
        //                       case HY.MultiPlayerState.START:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.LIVE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.RELOAD:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOT:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOTCANCEL:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOTCOMPLETE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.EVENT:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEADEYESTART:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEADEYEACTIVE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEAD:
        //                           {

        //                           }
        //                           break;
        //                   }
        //               }
        //               break;

        //           case HY.MultiGameState.PLAY:
        //               {
        //                   switch (PlayerState)
        //                   {
        //                       case HY.MultiPlayerState.START:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.LIVE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.RELOAD:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOT:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOTCANCEL:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOTCOMPLETE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.EVENT:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEADEYESTART:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEADEYEACTIVE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEAD:
        //                           {

        //                           }
        //                           break;
        //                   }
        //               }
        //               break;

        //           case HY.MultiGameState.GAMEWIN:
        //               {
        //                   switch (PlayerState)
        //                   {
        //                       case HY.MultiPlayerState.START:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.LIVE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.RELOAD:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOT:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOTCANCEL:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOTCOMPLETE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.EVENT:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEADEYESTART:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEADEYEACTIVE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEAD:
        //                           {

        //                           }
        //                           break;
        //                   }
        //               }
        //               break;

        //           case HY.MultiGameState.GAMEOVER:
        //               {
        //                   switch (PlayerState)
        //                   {
        //                       case HY.MultiPlayerState.START:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.LIVE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.RELOAD:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOT:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOTCANCEL:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOTCOMPLETE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.EVENT:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEADEYESTART:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEADEYEACTIVE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEAD:
        //                           {

        //                           }
        //                           break;
        //                   }
        //               }
        //               break;

        //           case HY.MultiGameState.CONNECTOUT:
        //               {
        //                   switch (PlayerState)
        //                   {
        //                       case HY.MultiPlayerState.START:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.LIVE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.RELOAD:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOT:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOTCANCEL:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.SHOOTCOMPLETE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.EVENT:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEADEYESTART:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEADEYEACTIVE:
        //                           {

        //                           }
        //                           break;

        //                       case HY.MultiPlayerState.DEAD:
        //                           {

        //                           }
        //                           break;
        //                   }
        //               }
        //               break;
        //       }
        #endregion
    }


}
