using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GooglePlayGames.BasicApi.Multiplayer;

namespace HY
{
    // 멀티 게임 상태
    public enum MultiGameState
    {
        WAIT = 0,
        SELECT,
        PLAY,
        GAMEOVER,
        GAMEWIN,
        CONNECTOUT,
        MAX
    }

    // 멀티 게임 플레이어의 상태
    public enum MultiPlayerState
    {
        START = 0,
        LIVE,
        RELOAD,
        SHOOT,
        SHOOTCANCEL,
        SHOOTCOMPLETE,
        EVENT,
        DEADEYESTART,
        DEADEYEACTIVE,
        DEAD
    }

    // 멀티 플레이어 캐릭터 상태
    public enum MultiCharacterState
    {
        CHAR_001 = 0,
        CHAR_002,
        CHAR_003
    }

    // 멀티 플레이어 총 상태
    public enum MultiGunState
    {
        REVOLVER = 0,
        SHOTGUN,
        MUSKET
    }
}

public class MultiGameManager : MonoBehaviour, MPUpdateListener
{

    // 전체적으로 관리될 게임, 캐릭터 상태 값
    public static HY.MultiGameState MultiState;
    public static HY.MultiPlayerState PlayerState;

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
    public GameObject MyCharacterPos;
    private string MyPlayerNick;

    // 적의 정보
    public GameObject EnemyCharacter;
    public GameObject EnemyCharacterPos;
    private string OpponentPlayerNick;
    private Dictionary<string, EnemyMove> _opponentScripts;

    // 상대방이 갖고 있을 애니메이션 값
    private LSD.PlayerState m_state;

    private bool _multiplayerReady = false;
    private string _MyParticipantId;
    private string _EnemyParticipantId;
    private Vector2 _startingPoint;

    // 네트워크 최적화 부분
    private float _nextBroadcastTime;

    // 게임이 끝났는지의 여부
    private bool ThisGameIsEnd;

    // 타임 아웃 정보
    [HideInInspector]
    public float timeOutThreshold;
    private float _timeOutCheckInterval;
    private float _nextTimeoutCheck;

    //데드아이 체크
    private float _DeadEyeTimer;
    private bool _DeadEyeChecker;
    private int _DeadEyeRespawnIndex;

    // Use this for initialization
    void Awake() {

        // 임시로 만든 상태 값이므로 추후에 수정해주세요
        MultiState = HY.MultiGameState.WAIT;
        PlayerState = HY.MultiPlayerState.LIVE;

        // 네트워크 체크 변수들
        ThisGameIsEnd = false;
        ItemCount = 0;
        _DeadEyeTimer = 0.0f;
        _DeadEyeChecker = false;
        _DeadEyeRespawnIndex = -1;
        WaitSignal = false;
        SelectSignal = false;

        // 네트워크 트래픽 최적화 변수 초기화
        _nextBroadcastTime = 0;

        // 타임 아웃 정보 초기화
        timeOutThreshold = 20.0f;
        _timeOutCheckInterval = 1.0f;
        _nextTimeoutCheck = 0.0f;

        MyPlayerNick = "";
        OpponentPlayerNick = "";

        SetupMultiplayerGame();
    }

    void SetupMultiplayerGame()
    {

        GPGSManager.GetInstance.updateListener = this;

        // 1
        _MyParticipantId = GPGSManager.GetInstance.GetMyParticipantId();

        // 2
        List<Participant> allPlayers = GPGSManager.GetInstance.GetAllPlayers();
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
                    MyCharacter = GameObject.Find("Character");
                    MyCharacter.transform.position = MyCharacterPos.transform.position;
                }
                else
                {
                    MyCharacter.transform.position = MyCharacterPos.transform.position;
                }
            }
            else
            {
                if (EnemyCharacter == null)
                {
                    EnemyCharacter = GameObject.Find("Enemy_Character");
                    EnemyCharacter.transform.position = EnemyCharacterPos.transform.position;

                    EnemyMove opponentScript = EnemyCharacter.GetComponent<EnemyMove>();
                    _EnemyParticipantId = nextParticipantId;
                    _opponentScripts[nextParticipantId] = opponentScript;

                }
                else
                {
                    EnemyCharacter.transform.position = EnemyCharacterPos.transform.position;

                    EnemyMove opponentScript = EnemyCharacter.GetComponent<EnemyMove>();
                    _EnemyParticipantId = nextParticipantId;
                    _opponentScripts[nextParticipantId] = opponentScript;
                }
                // 5
                //GameObject opponentCar = (Instantiate(opponentPrefab, carStartPoint, Quaternion.identity) as GameObject);

            }
        }

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

        if (allPlayers[0].ParticipantId == _MyParticipantId)
        {
            MyCharacter.transform.position = MyCharacterPos.transform.position;
            EnemyCharacter.transform.position = EnemyCharacterPos.transform.position;

            //PlayerName.text = GPGSManager.GetInstance.GetOtherNameGPGS(1);//_opponentScripts[_MyParticipantId].name;//GPGSManager.GetInstance.GetOtherNameGPGS(0);
            //EnemyName.text = GPGSManager.GetInstance.GetOtherNameGPGS(0);
        }
        else
        {
            MyCharacter.transform.position = EnemyCharacterPos.transform.position;
            EnemyCharacter.transform.position = MyCharacterPos.transform.position;
        }

        MyPlayerNick = GPGSManager.GetInstance.GetOtherNameGPGS(0);
        OpponentPlayerNick = GPGSManager.GetInstance.GetOtherNameGPGS(1);

        _multiplayerReady = true;

    }



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
        GPGSManager.GetInstance.LeaveGame();
        GPGSManager.GetInstance.updateListener = null;

        ThisGameIsEnd = true;

        if(dTime <= 1.5f)
        {
            Invoke("StartLobbyScene", 1.5f);
        }
        else if(dTime >= 5.0f)
        {
            Invoke("StartLobbyScene", 5.0f);
        }
        else
        {
            Invoke("StartLobbyScene", dTime);
        }
        
    }

    void StartLobbyScene()
    {
        AutoFade.LoadLevel("WaitingRoom", 0.2f, 0.2f, Color.black);
    }

    // GPGSManager(서버)에서 받은 메시지를 매니저에게 줄때 사용한다.
    // [서버]->[클라이언트]
    #region GPGS_CallBack_Interface

    // 맨 처음 게임 접속 대기 상태를 받는 콜백 함수이다.
    // 
    public void MultiStateWaitReceived(bool Wait)
    {
        if (_multiplayerReady)
        {
            WaitSignal = Wait;
        }
    }

    public void MultiStateSelectReceived(bool Select)
    {
        if (_multiplayerReady)
        {
            SelectSignal = Select;
        }
    }

    // 끝내기 메시지를 보내주는 녀석이다.
    public void FinishedReceived(string participantId, bool GameOver)
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

    // 업데이트를 해줄 정보들...
    public void UpdatePositionReceived(string participantId, int messageNum, float posX, float posY, float posZ, float rotY)
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

    // 아이템들의 정보들을 업데이트 해준다.
    public void ItemStateReceived(int Index, bool GetItem)
    {
        if (_multiplayerReady)
        {
            // 아이템 처리를 해주세요...
        }
    }

    // 사격 상태를 업데이트 해준다.
    // bool 값이 true면 발사를 성공한것입니다.
    public void ShootStateReceived(bool ShootSuccess)
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

    // 상대방의 총알 방향을 알려주는 메시지입니다.
    // 각각의 좌표에 총알 방향의 노말 값을 넣어주면 됩니다.
    public void ShootVectorReceived(float x, float y, float z)
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

    // 현재 데드아이 상태를 업데이트 해준다.
    // true면 데드 아이 발동을 의미
    // false면 데드 아이 발동이 끝남 혹은 안됨..
    public void DeadEyeStateReceived(bool DeadEyeActive)
    {
        if (_multiplayerReady)
        {
            _DeadEyeChecker = DeadEyeActive;
            
            EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

            if (opponent != null)
            {
                opponent.SetDeadEyeStateReceived(_DeadEyeChecker);
            }
        }
    }

    // 현재 데드아이 상태를 업데이트 해준다.
    // float는 데드 아이 경과 시간을 의미한다.
    public void DeadEyeTimerStateReceived(float DeadEyeTimer)
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

    // 데드아이 아이템이 리스폰 되어야할 위치를 받는다.
    // index는 위치로써, 서버 쪽에서 난수 값으로 0~4까지 만든 후 보내준다.
    public void DeadEyeRespawnIndexReceived(int index)
    {
        if (_multiplayerReady)
        {
            _DeadEyeRespawnIndex = index;
        }
    }

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
    public void AniStateReceived(int AniState)
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

        if (_multiplayerReady)
        {
            EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

            if (opponent != null)
            {
                opponent.SetAniStateReceived(AniState);
            }
        }
    }

    // HP의 정보를 동기화 시키는 역할
    // HPState에 넘어가는 값에 따라 상대방에게 보여지는 HP가 틀려진다.
    public void HPStateReceived(int HPState)
    {
        if(_multiplayerReady)
        {
            EnemyMove opponent = _opponentScripts[_EnemyParticipantId];

            if (opponent != null)
            {
               opponent.SetHPStateReceived(HPState);
            }
        }
    }

    // 게임이 끝날시 호출되는 리스너 함수.
    public void LeftRoomConfirmed()
    {
        //GPGSManager.GetInstance.updateListener = null;

        //AutoFade.LoadLevel("WaitingRoom", 0.2f, 0.2f, Color.black);
        ThisGameIsEnd = true;
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
        if (_multiplayerReady)
        {

            ThisGameIsEnd = true;

            EnemyMove opponent = _opponentScripts[participantId];

            if (opponent != null)
            {
                opponent.GameOutInformation();
            }

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


        if (ThisGameIsEnd == false)
        {
            ThisGameIsEnd = GameEnd;

            GPGSManager.GetInstance.SendFinishMessage(ThisGameIsEnd);
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
    public void SendDeadEyeRespawnIndexMessage()
    {
        GPGSManager.GetInstance.SendDeadEyeIndexMessage();
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
        //switch(MultiState)
        //{
        //    case HY.MultiGameState.WAIT:
        //        {

        //        }
        //        break;

        //    case HY.MultiGameState.SELECT:
        //        {

        //        }
        //        break;

        //    case HY.MultiGameState.PLAY:
        //        {


        //        }
        //        break;

        //    case HY.MultiGameState.CONNECTOUT:
        //        {

        //        }
        //        break;
        //}

        if (_multiplayerReady)
        {
            //if (GPGSManager.GetInstance.GetOtherNameGPGS(1) == _MyParticipantId)
            //{
            //    PlayerName.text = GPGSManager.GetInstance.GetOtherNameGPGS(1);//_opponentScripts[_MyParticipantId].name;//GPGSManager.GetInstance.GetOtherNameGPGS(0);
            //    EnemyName.text = GPGSManager.GetInstance.GetOtherNameGPGS(0);
            //}
            //else
            //{
            //    PlayerName.text = GPGSManager.GetInstance.GetOtherNameGPGS(0);//GPGSManager.GetInstance.GetOtherNameGPGS(0);
            //    EnemyName.text = GPGSManager.GetInstance.GetOtherNameGPGS(1);
            //}


            if (ThisGameIsEnd == false)
            {
                NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();

            }
            else
            {
                NetText.text = "Net Info : 상대방이 연결을 해제했습니다.";
            }

            PlayerName.text = MyPlayerNick;
            EnemyName.text = OpponentPlayerNick;

            //MyInfoText.text = "Player Info : " + MyCharacterPos.GetComponent<CharMove>().m_DebugPlayerState;//MyCharacterPos.transform.position;
            //EnemyInfoText.text = "Enemy Info : " + EnemyCharacterPos.GetComponent<EnemyMove>().m_DebugPlayerState;//EnemyCharacterPos.transform.position;
            MyInfoText.text = "WaitSignal : " + WaitSignal + " / SelectSignal : " + SelectSignal;

            EnemyInfoText.text = "Enemy Info : " + EnemyCharacterPos.GetComponent<EnemyMove>().m_DebugPlayerState;//EnemyCharacterPos.transform.position;

            ItemGetCount.text = "MessageCount : " + ItemCount + " / EndMsg : " + ThisGameIsEnd + " / Index : " + _DeadEyeRespawnIndex;


        }
        else
        {
            PlayerName.text = "Player"; //GPGSManager.GetInstance.GetNameGPGS();
            EnemyName.text = "Enemy";

            MyInfoText.text = "Player Info : " + MyCharacterPos.transform.position;
            EnemyInfoText.text = "Enemy Info : " + EnemyCharacterPos.transform.position;
            NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();
            ItemGetCount.text = "MessageCount : " + ItemCount + " / EndMsg : " + ThisGameIsEnd + " / Index : " + _DeadEyeRespawnIndex;
        }

        // 적의 타임아웃 체크
        if(ThisGameIsEnd == false)
        {
            if (Time.time > _nextTimeoutCheck)
            {

                CheckForTimeOuts();
                _nextTimeoutCheck = Time.time + _timeOutCheckInterval;
            }
        }


        // 플레이어의 위치 동기화
        SendMyPositionUpdate();


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
    }


}
