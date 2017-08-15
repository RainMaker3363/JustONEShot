using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;
using System.Collections.Generic;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi;

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

    // 멀티 게임 모드의 상태
    public enum MultiGameModeState
    {
        NONE = 0,
        PVP,
        SURVIVAL
    }
}


public class GPGSManager : Singleton<GPGSManager>, RealTimeMultiplayerListener
{
    private uint minimumOpponents = 1;
    private uint maximumOpponents = 1;
    private uint gameVariation = 0;
    private byte _protocolVersion = 1;

    // Byte + Byte + 3 floats for position  + 1 float for rotY + 1 Interget for MessageNumber
    private int _updateMessageLength = 22;
    // Byte + Byte + 1 boolen for finish Game
    private int _finishMessageLength = 3;
    // Byte + Byte + 1 boolen for Item State + 1 Interger
    private int _itemStateMessageLength = 7;
    // Byte + Byte + 1 boolen
    private int _shootMessageLength = 3;
    // Byte + Byte + 1 Float
    private int _deadeyeTimerMessageLength = 6;
    // Byte + Byte + 1 Boolean
    private int _deadeyeMessageLength = 3;
    // Byte + Byte + 1 Interger
    private int _deadeyeRespawnMessageLength = 6;
    // Byte + Byte + 1 Interger
    private int _animMessageLength = 6;
    // Byte + Byte + 1 Interget
    private int _healthMessageLength = 6;
    // Byte + Byte + 1 Boolean
    private int _gameStateWaitMesageLength = 3;
    // Byte + Byte + 1 Boolean
    private int _gameStateSelectMesageLength = 3;
    // Byte + Byte + 1 Interger
    private int _CharacterSelectMessageLength = 6;
    // Byte + Byte + 1 Interger
    private int _WeaponSelectMessageLength = 6;
    // Byte + Byte + 1 Vector
    private int _shootVectorMesageLength = 14;

    // Byte + Byte + Byte + 3 floats for position  + 1 float for rotY + 1 Interget for MessageNumber
    private int _BossAlarmMesageLength = 23;
    // Byte + Byte + Byte + 1 Boolean
    private int _BossDeadEventMessageLength = 4;
    // Byte + Byte + Byte + 1 Boolean
    private int _BossAnimMessageLength = 4;
    // Byte + Byte + Byte + 1 Interger
    private int _BossPosMessageLength = 7;
    // Byte + Byte + Byte + 1 interger
    private int _BossHPStateMessageLength = 7;

    // 메시지 도착 순서를 제어할 변수
    // 네트워크 도착 순서가 무작위로 된다면 동기화가 이상하게 될 가능성이 있기에
    // 이것을 보정해줄 변수이다.
    private int _myMessageNum;
    private int _BossPosMessageNum;

    private List<byte> _StateWaitMessage;
    private List<byte> _StateSelectMessage;
    private List<byte> _CharacterSelectMessage;
    private List<byte> _WeaponSelectMessage;

    private List<byte> _updateMessage;
    private List<byte> _endMessage;
    private List<byte> _itemstateMessage;
    private List<byte> _ShootMessage;
    private List<byte> _ShootVectorMessage;
    private List<byte> _DeadEyeMessage;
    private List<byte> _DeadEyeTimerMessage;
    private List<byte> _DeadEyeRespawnMessage;
    private List<byte> _AnimMessage;
    private List<byte> _HealthMessage;

    // 보스 이벤트
    private List<byte> _BossAlarmMessage;
    private List<byte> _BossPosMessage;
    private List<byte> _BossAnimMessage;
    private List<byte> _BossDeadEventMessage;
    private List<byte> _BossHPStateMeesage;

    // 현재 세션에 접속한 플레이어들의 정보들이다.
    //private List<Participant> allPlayers;

    // 현재 나의 캐릭터 정보를 가지고 있는다.
    // 기본값은 100이다.
    private int MyCharacterNumber = 100;

    // 적의 캐릭터 정보를 가지고 있는다.
    // PVP용으로써 서바이벌 모드에선 다른 로직을...
    // 기본값은 100이다.
    private Dictionary<string, int> SurvivalOpponentCharNumbers;
    // 나를 포함한 모든 플레이어들의 캐릭터 번호를 가지고 있는다.
    private Dictionary<string, int> PlayerCharacters_Number;
    // 서바이벌 모드에서 사용하는 나의 인덱스 번호
    private int MySurvival_ID_Index;
    private int OppenentCharNumber = 100;

    // 난수 발생시 동기화를 위한 변수
    // 맨 처음의 발생되는 리스폰 난수는 초기화할때 해준다.
    private int StartRandomEncounter;


    private bool IsConnectedOn = false;
    private bool IsMatchingNow = false;
    private bool showingWaitingRoom = false;
    private static bool IsInitEnd = false;

    private string ReceiveMessage = " ";
    private string SendMessage = " ";
    private string NetMessage = " ";

    private HY.MultiGameModeState NowMultiGameMode;
    public MPUpdateListener updateListener;
    public LBUpdateListener LBListener;

    // 현재 로그인 중인지 체크
    public bool bLogin
    {
        get;
        set;
    }

    // GPGS를 초기화 합니다
    public void InitializeGPGS()
    {
        bLogin = false;
        IsConnectedOn = false;
        IsMatchingNow = false;

        MyCharacterNumber = 100;
        OppenentCharNumber = 100;
        StartRandomEncounter = UnityEngine.Random.Range(0, 5);

        if (SurvivalOpponentCharNumbers == null)
        {
            SurvivalOpponentCharNumbers = new Dictionary<string, int>(8);
            //SurvivalOpponentCharNumbers.Clear();
        }
        else
        {
            SurvivalOpponentCharNumbers.Clear();
        }

        if(PlayerCharacters_Number == null)
        {
            PlayerCharacters_Number = new Dictionary<string, int>(8);
        }
        else
        {
            PlayerCharacters_Number.Clear();
        }

        MySurvival_ID_Index = 0;

        if (IsInitEnd == false)
        {
            //IsInitEnd = true;

            minimumOpponents = 1;
            maximumOpponents = 1;
            gameVariation = 0;
            _protocolVersion = 1;

            // Byte + Byte + 3 floats for position  + 1 float for rotY + 1 Interget for MessageNumber
            _updateMessageLength = 22;
            // Byte + Byte + 1 boolen for finish Game
            _finishMessageLength = 3;
            // Byte + Byte + 1 boolen for Item State + 1 Interger
            _itemStateMessageLength = 7;
            // Byte + Byte + 1 boolen
            _shootMessageLength = 3;
            // Byte + Byte + 1 Float
            _deadeyeTimerMessageLength = 6;
            // Byte + Byte + 1 Boolean
            _deadeyeMessageLength = 3;
             // Byte + Byte + 1 Interger
             _deadeyeRespawnMessageLength = 6;
            // Byte + Byte + 1 Interger
            _animMessageLength = 6;
            // Byte + Byte + 1 Interget
            _healthMessageLength = 6;
            // Byte + Byte + 1 Boolean
            _gameStateWaitMesageLength = 3;
            // Byte + Byte + 1 Boolean
            _gameStateSelectMesageLength = 3;
            // Byte + Byte + 1 Interger
            _CharacterSelectMessageLength = 6;
            // Byte + Byte + 1 Interger
            _WeaponSelectMessageLength = 6;
             // Byte + Byte + 1 Vector
            _shootVectorMesageLength = 14;
            
            
            // Byte + Byte + Byte + 3 floats for position  + 1 float for rotY + 1 Interget for MessageNumber
            _BossAlarmMesageLength = 23;
            // Byte + Byte + Byte + 1 Boolean
            _BossDeadEventMessageLength = 4;
            // Byte + Byte + Byte + 1 Boolean
            _BossAnimMessageLength = 4;
            // Byte + Byte + Byte + 1 Interger
            _BossPosMessageLength = 7;
            // Byte + Byte + Byte + 1 interger
            _BossHPStateMessageLength = 7;

            //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            //    // 클라우드 게임 데이터 저장을 위해서 사용
            //    .EnableSavedGames()
            //    .Build();

            //PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }
        

        if (_updateMessage == null)
        {
            _updateMessage = new List<byte>(_updateMessageLength);
        }

        if(_endMessage == null)
        {
            _endMessage = new List<byte>(_finishMessageLength);
        }

        if(_itemstateMessage == null)
        {
            _itemstateMessage  = new List<byte>(_itemStateMessageLength);
        }

        if (_ShootMessage == null)
        {
            _ShootMessage = new List<byte>(_shootMessageLength);
        }

        if(_ShootVectorMessage == null)
        {
            _ShootVectorMessage = new List<byte>(_shootVectorMesageLength);
        }

        if (_DeadEyeMessage == null)
        {
            _DeadEyeMessage = new List<byte>(_deadeyeMessageLength);
        }

        if(_DeadEyeTimerMessage == null)
        {
            _DeadEyeTimerMessage = new List<byte>(_deadeyeTimerMessageLength);
        }

        if(_DeadEyeRespawnMessage == null)
        {
            _DeadEyeRespawnMessage = new List<byte>(_deadeyeRespawnMessageLength);
        }


        if (_AnimMessage == null)
        {
            _AnimMessage = new List<byte>(_animMessageLength);
        }

        if(_HealthMessage == null)
        {
            _HealthMessage = new List<byte>(_healthMessageLength);
        }

        if (_StateWaitMessage == null)
        {
            _StateWaitMessage = new List<byte>(_gameStateWaitMesageLength);
        }

        if (_StateSelectMessage == null)
        {
            _StateSelectMessage = new List<byte>(_gameStateSelectMesageLength);
        }

        if (_WeaponSelectMessage == null)
        {
            _WeaponSelectMessage = new List<byte>(_WeaponSelectMessageLength);
        }

        if (_CharacterSelectMessage == null)
        {
            _CharacterSelectMessage = new List<byte>(_CharacterSelectMessageLength);
        }

        // 보스 이벤트
        if(_BossAlarmMessage == null)
        {
            _BossAlarmMessage = new List<byte>(_BossAlarmMesageLength);
        }

        if(_BossPosMessage == null)
        {
            _BossPosMessage = new List<byte>(_BossPosMessageLength);
        }

        if(_BossAnimMessage == null)
        {
            _BossAnimMessage = new List<byte>(_BossAnimMessageLength);
        }

        if(_BossDeadEventMessage == null)
        {
            _BossDeadEventMessage = new List<byte>(_BossDeadEventMessageLength);
        }

        if(_BossHPStateMeesage == null)
        {
            _BossHPStateMeesage = new List<byte>(_BossHPStateMessageLength);
        }

        NowMultiGameMode = HY.MultiGameModeState.NONE;

        _BossPosMessageNum = 0;
        _myMessageNum = 0;
    }

    // P2P 방식으로 상대방을 검색하기 시작한다.
    public void StartMatchMaking()
    {

        // 게임모드가 선택되어 있지 않다면 아무것도 하지 않는다
        switch (NowMultiGameMode)
        {
            case HY.MultiGameModeState.NONE:
                {
                    // 최소 수용 인원
                    minimumOpponents = 1;
                    // 최대 수용 인원
                    maximumOpponents = 1;
                    // 게임 모드 (기본값)
                    gameVariation = 0;

                    // 최소 수용 인원
                    // 최대 수용 인원
                    //PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    // 최소 수용 인원
                    minimumOpponents = 1;
                    // 최대 수용 인원
                    maximumOpponents = 1;
                    // 게임 모드 (기본값)
                    gameVariation = 0;

                    //PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();

                    Debug.Log("Mode : " + NowMultiGameMode + " Min : " + minimumOpponents + " Max : " + maximumOpponents + " Variation : " + gameVariation);

                    // 최소 수용 인원
                    // 최대 수용 인원
                    PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {
                    // 최소 수용 인원
                    minimumOpponents = 3;
                    // 최대 수용 인원
                    maximumOpponents = 5;
                    // 게임 모드
                    gameVariation = 1;

                    //PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();

                    Debug.Log("Mode : " + NowMultiGameMode + " Min : " + minimumOpponents + " Max : " + maximumOpponents + " Variation : " + gameVariation);

                    // 최소 수용 인원
                    // 최대 수용 인원
                    PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
                }
                break;

            //default:
            //    {
            //        // 최소 수용 인원
            //        minimumOpponents = 1;
            //        // 최대 수용 인원
            //        maximumOpponents = 1;

            //        // 최소 수용 인원
            //        // 최대 수용 인원
            //        PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
            //    }
            //    break;
        }


        // 최소 수용 인원
        // 최대 수용 인원
        //PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
        //PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();
    }

    public void ShowRoomUI()
    {
        PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();
    }

    public bool IsConnected()
    {
        return IsConnectedOn;
    }

    public bool IsMatching()
    {
        return IsMatchingNow;
    }

    public string GetNetMessage()
    {
        return NetMessage;
    }

    public bool IsShowingWaitingRoom()
    {
        return showingWaitingRoom;
    }

    public string GetReceiveMessage()
    {
        return ReceiveMessage;
    }

    public string GetSendMessage()
    {
        return SendMessage;
    }

    // 0~5의 값을 전달해준다.
    public int GetStartRandomSeed()
    {
        return StartRandomEncounter;
    }

    // 현재 PVP 모드에서 적의 캐릭터 번호를 가지고 온다
    public int GetPVPOpponentCharNumber()
    {
        return OppenentCharNumber;
    }

    // 서바이벌 모드에서 적들의 캐릭터 번호를 알 수 있습니다.
    public Dictionary<string, int> GetSurvivalOpponentCharNumbers()
    {
        return SurvivalOpponentCharNumbers;
    }

    // 서바이벌 모드에서 나의 인덱스 번호를 알 수 있다.
    public int GetMySurvival_ID_Index()
    {
        //allPlayers = GetAllPlayers();
        string My_ID = GetMyParticipantId();

        for (int i =0; i < GetAllPlayers().Count; i++)
        {
            if(GetAllPlayers()[i].ParticipantId.Equals(My_ID))
            {
                MySurvival_ID_Index = i;
                break;
            }
        }

        return MySurvival_ID_Index;
    }

    // 서바이벌 모드에서 모든 플레이어들의 캐릭터 번호를 알 수 있습니다.
    public int GetSurvivalAllPlayerCharacterNumber(string ID)
    {
        int Character_Number = 0;
        IDictionaryEnumerator Iter = PlayerCharacters_Number.GetEnumerator();

        while(Iter.MoveNext())
        {
            if(Iter.Key.Equals(ID))
            {
                Character_Number = (int)Iter.Value;
                break;
            }
        }

        return Character_Number;
    }

    // 서바이벌 모드에서 모든 플레이어들의 ID값을 알아낸다.
    public string GetSurvivalAllPlayerCharacterID(int index = 0)
    {
        //allPlayers = GetAllPlayers();

        if (index <= 0)
        {
            index = 0;
        }
        else if(index >= GetAllPlayers().Count)
        {
            index = GetAllPlayers().Count;
        }

        return GetAllPlayers()[index].ParticipantId;
    }

    //// 서바이벌 모드에서 적들의 캐릭터 번호를 설정할 때 사용합니다.
    //public void SetPlayerCharacters_Number(int CharNumber = 0)
    //{
    //    PlayerCharacters_Number[GetMyParticipantId()] = CharNumber;
    //}

    // 현재 내가 선택한 캐릭터에 대한 정보를 설정해준다.
    // 기본값은 100
    public void SetMyCharacterNumber(int number = 100)
    {
        if(number < 0)
        {
            MyCharacterNumber = 0;
        }
        else if(number > 100)
        {
            MyCharacterNumber = 100;
        }
        else
        {
            MyCharacterNumber = number;

        }

        switch(NowMultiGameMode)
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
                    PlayerCharacters_Number[GetMyParticipantId()] = MyCharacterNumber;
                }
                break;
        }
    }

    // 현재 내가 선택한 캐릭터에 대한 정보를 반환시켜준다.
    public int GetMyCharacterNumber()
    {
        return MyCharacterNumber;
    }



    // 현재 멀티게임 모드의 상태를 반환해준다.
    public HY.MultiGameModeState GetMultiGameModeState()
    { 
        return NowMultiGameMode;
    }

    // 현재 멀티게임모드의 상태를 갱신시켜준다
    // 0 : None
    // 1 : PVP
    // 2 : Survival
    public void SetMultiGameModeState(int ModeNumber)
    {
        if(ModeNumber <= 0)
        {
            ModeNumber = 0;
        }
        else if(ModeNumber >= (int)HY.MultiGameModeState.SURVIVAL)
        {
            ModeNumber = (int)HY.MultiGameModeState.SURVIVAL;
        }

        switch(ModeNumber)
        {
            case 0:
                {
                    NowMultiGameMode = HY.MultiGameModeState.NONE;
                }
                break;

            case 1:
                {
                    NowMultiGameMode = HY.MultiGameModeState.PVP;
                }
                break;

            case 2:
                {
                    NowMultiGameMode = HY.MultiGameModeState.SURVIVAL;
                }
                break;

            default:
                {
                    NowMultiGameMode = HY.MultiGameModeState.NONE;
                }
                break;
        }
    }

    // 현재 멀티게임모드의 상태를 갱신시켜준다
    public void SetMultiGameModeState(HY.MultiGameModeState Mode)
    {
        NowMultiGameMode = Mode;
    }

    public void InitMessager()
    {
        if (_updateMessage == null)
        {
            _updateMessage = new List<byte>(_updateMessageLength);
        }

        if (_endMessage == null)
        {
            _endMessage = new List<byte>(_finishMessageLength);
        }

        if (_itemstateMessage == null)
        {
            _itemstateMessage = new List<byte>(_itemStateMessageLength);
        }

        if (_ShootMessage == null)
        {
            _ShootMessage = new List<byte>(_shootMessageLength);
        }

        if (_ShootVectorMessage == null)
        {
            _ShootVectorMessage = new List<byte>(_shootVectorMesageLength);
        }

        if (_DeadEyeMessage == null)
        {
            _DeadEyeMessage = new List<byte>(_deadeyeMessageLength);
        }

        if (_DeadEyeTimerMessage == null)
        {
            _DeadEyeTimerMessage = new List<byte>(_deadeyeTimerMessageLength);
        }

        if (_DeadEyeRespawnMessage == null)
        {
            _DeadEyeRespawnMessage = new List<byte>(_deadeyeRespawnMessageLength);
        }

        if (_AnimMessage == null)
        {
            _AnimMessage = new List<byte>(_animMessageLength);
        }

        if (_HealthMessage == null)
        {
            _HealthMessage = new List<byte>(_healthMessageLength);
        }

        if (_StateWaitMessage == null)
        {
            _StateWaitMessage = new List<byte>(_gameStateWaitMesageLength);
        }

        if (_StateSelectMessage == null)
        {
            _StateSelectMessage = new List<byte>(_gameStateSelectMesageLength);
        }

        if (_WeaponSelectMessage == null)
        {
            _WeaponSelectMessage = new List<byte>(_WeaponSelectMessageLength);
        }

        if (_CharacterSelectMessage == null)
        {
            _CharacterSelectMessage = new List<byte>(_CharacterSelectMessageLength);
        }

        // 보스 이벤트
        if (_BossAlarmMessage == null)
        {
            _BossAlarmMessage = new List<byte>(_BossAlarmMesageLength);
        }

        if (_BossPosMessage == null)
        {
            _BossPosMessage = new List<byte>(_BossPosMessageLength);
        }

        if (_BossAnimMessage == null)
        {
            _BossAnimMessage = new List<byte>(_BossAnimMessageLength);
        }

        if (_BossDeadEventMessage == null)
        {
            _BossDeadEventMessage = new List<byte>(_BossDeadEventMessageLength);
        }

        if (_BossHPStateMeesage == null)
        {
            _BossHPStateMeesage = new List<byte>(_BossHPStateMessageLength);
        }

        _BossPosMessageNum = 0;
        _myMessageNum = 0;
    }

    public List<byte> GetUpdateMessage()
    {
        return _updateMessage;
    }

    // 현재 상태를 디버깅 로그로 보여주는 함수
    private void ShowMPStatus(string message)
    {
        Debug.Log(message);

        NetMessage = message;

        //if (mainMenuManager != null)
        //{
        //    //mainMenuManager.SetLobbyStatusMessage(message);
        //}
    }

    // 멀티플레이 방이 얼마나 셋업이 되었는지 보여주는 리스너 함수
    public void OnRoomSetupProgress(float percent)
    {
        ShowMPStatus("We are " + percent + "% done with setup");

        IsMatchingNow = true;

        if (!showingWaitingRoom)
        {
            showingWaitingRoom = true;
            //PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();
        }
    }

    // 멀티플레이 방이 연결되었는지의 여부
    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            ShowMPStatus("We are connected to the room! I would probably start our game now.");
            IsConnectedOn = true;
            //IsMatchingNow = true;
            _myMessageNum = 0;
        }
        else
        {
            ShowMPStatus("Uh-oh. Encountered some error connecting to the room.");
            IsConnectedOn = false;
            IsMatchingNow = false;
        }
    }


    //// 멀티플레이어가 나갔을때 호출되는 리스너 함수
    //public void OnLeftRoom()
    //{
    //    ShowMPStatus("We have left the room. We should probably perform some clean-up tasks.");

    //    showingWaitingRoom = false;
    //}

    // 멀티플레이어가 나갔을때 호출되는 리스너 함수
    public void OnLeftRoom()
    {
        ShowMPStatus("We have left the room.");

        if (updateListener != null)
        {
            updateListener.LeftRoomConfirmed();
        }
        
        if(LBListener != null)
        {
            LBListener.LeftRoomConfirmed();
        }

        showingWaitingRoom = false;
    }

    // 해당 플레이어의 아이디가 연결을 했을 경우 호출되는 리스너 함수
    public void OnPeersConnected(string[] participantIds)
    {
        foreach (string participantID in participantIds)
        {
            ShowMPStatus("Player " + participantID + " has joined.");
        }
    }

    // 해당 플레이어의 아이디가 연결을 끊었을 경우 호출되는 리스너 함수
    public void OnPeersDisconnected(string[] participantIds)
    {
        //foreach (string participantID in participantIds)
        //{
        //    ShowMPStatus("Player " + participantID + " has left.");
        //}

        foreach (string participantID in participantIds)
        {
            ShowMPStatus("Player " + participantID + " has left.");

            if (updateListener != null)
            {
                updateListener.PlayerLeftRoom(participantID);
            }
        }
    }

    /// Raises the participant left event.
    /// This is called during room setup if a player declines an invitation
    /// or leaves.  The status of the participant can be inspected to determine
    /// the reason.  If all players have left, the room is closed automatically.
    public void OnParticipantLeft(Participant participant)
    {
        ShowMPStatus("All Player Out! So, The Room will be Close");
        LeaveRoom();
    }

    // 방을 떠나기 및 파기
    public void LeaveRoom()
    {
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    }

    // 상대 ID로부터 메시지를 받았을때 호출되는 리스너 함수
    //public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    //{
    //    ShowMPStatus("We have received some gameplay messages from participant ID:" + senderId);
    //}

    public bool IsAuthenticated()
    {
        return PlayGamesPlatform.Instance.localUser.authenticated;
    }

    public void SignInAndStartMPGame()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("We're signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
                    StartMatchMaking();
                }
                else
                {
                    Debug.Log("Oh... we're not signed in.");
                }
            });
        }
        else
        {
            Debug.Log("You're already signed in.");
            StartMatchMaking();
        }
    }

    public void TrySilentSignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("Silently signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
                }
                else
                {
                    Debug.Log("Oh... we're not signed in.");
                }
            }, true);
        }
        else
        {
            Debug.Log("We're already signed in");
        }
    }

    // 다른 플레이어들에게 자신의 좌표 값을 보내준다.
    // X, Y, Z, Rotation Y 값..
    public void SendMyPositionUpdate(float posX, float posY, float posZ, float rotY)
    {
        _updateMessage.Clear();
        _updateMessage.Add(_protocolVersion);
        _updateMessage.Add((byte)'U');
        _updateMessage.AddRange(System.BitConverter.GetBytes(++_myMessageNum));
        _updateMessage.AddRange(System.BitConverter.GetBytes(posX));
        _updateMessage.AddRange(System.BitConverter.GetBytes(posY));
        _updateMessage.AddRange(System.BitConverter.GetBytes(posZ));
        _updateMessage.AddRange(System.BitConverter.GetBytes(rotY));
        
        byte[] messageToSend = _updateMessage.ToArray();

        Debug.Log("Sending my update message  " + messageToSend + " to all players in the room");

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, messageToSend);
        //PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend);
    }

    // 게임이 끝났을때 보내지는 메시지

    public void SendFinishMessage(bool GameEnd)
    {
        //List<byte> bytes = new List<byte>(_finishMessageLength);
        _endMessage.Clear();
        _endMessage.Add(_protocolVersion);
        _endMessage.Add((byte)'F');
        _endMessage.AddRange(System.BitConverter.GetBytes(GameEnd));

        byte[] EndMessageToSend = _endMessage.ToArray();

        Debug.Log("Sending my update message  " + EndMessageToSend + " to all players in the room");

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, EndMessageToSend);
    }

    // 아이템을 먹었을때 보내지는 메시지
    // 아이템의 인덱스 번호 (메모리 풀을 만들었다면 총알의 3개 중 하나...)
    // 아이템을 먹었는지의 여부 TRUE면 먹었다고 인식
    //public void SendItemStateMessage(int Index, bool GetItem)
    //{
    //    _itemstateMessage.Clear();
    //    _itemstateMessage.Add(_protocolVersion);
    //    _itemstateMessage.Add((byte)'I');
    //    _itemstateMessage.AddRange(System.BitConverter.GetBytes(GetItem));
    //    _itemstateMessage.AddRange(System.BitConverter.GetBytes(Index));

    //    byte[] ItemStateMessageToSend = _itemstateMessage.ToArray();

    //    Debug.Log("Sending my update message  " + ItemStateMessageToSend + " to all players in the room");

    //    PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, ItemStateMessageToSend);
    //}

    // 상대방 애니메이션 상태값을 보내는 메시지
    public void SendAniStateMessage(int State)
    {
        _AnimMessage.Clear();
        _AnimMessage.Add(_protocolVersion);
        _AnimMessage.Add((byte)'A');
        _AnimMessage.AddRange(System.BitConverter.GetBytes(State));

        byte[] AniStateMessageToSend = _AnimMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, AniStateMessageToSend);
    }

    // 상대방 총알의 발사 성공 여부를 보내는 메시지
    public void SendShootMessage(bool ShootSuccess)
    {
        _ShootMessage.Clear();
        _ShootMessage.Add(_protocolVersion);
        _ShootMessage.Add((byte)'S');
        _ShootMessage.AddRange(System.BitConverter.GetBytes(ShootSuccess));

        byte[] ShotMessageToSend = _ShootMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, ShotMessageToSend);
    }

    // 상대방에게 총알을 맞았을때 판정하는 메시지로써,
    // vector는 총알이 날아오는 방향값을 알려준다.
    public void SendShootVectorMessage(float x, float y, float z)
    {

        _ShootVectorMessage.Clear();
        _ShootVectorMessage.Add(_protocolVersion);
        _ShootVectorMessage.Add((byte)'V');
        _ShootVectorMessage.AddRange(System.BitConverter.GetBytes(x));
        _ShootVectorMessage.AddRange(System.BitConverter.GetBytes(y));
        _ShootVectorMessage.AddRange(System.BitConverter.GetBytes(z));
        

        byte[] messageToSend = _ShootVectorMessage.ToArray();

        Debug.Log("Sending my update message  " + messageToSend + " to all players in the room");

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend);
    }

    // 상대방에게 총알을 맞았을때 판정하는 메시지로써,
    // vector는 총알이 날아오는 방향값을 알려준다.
    public void SendShootVectorMessage(Vector3 vec)
    {
        _ShootVectorMessage.Clear();
        _ShootVectorMessage.Add(_protocolVersion);
        _ShootVectorMessage.Add((byte)'V');
        _ShootVectorMessage.AddRange(System.BitConverter.GetBytes(vec.x));
        _ShootVectorMessage.AddRange(System.BitConverter.GetBytes(vec.y));
        _ShootVectorMessage.AddRange(System.BitConverter.GetBytes(vec.z));


        byte[] messageToSend = _ShootVectorMessage.ToArray();

        Debug.Log("Sending my update message  " + messageToSend + " to all players in the room");

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend);
    }

    // 맨 처음 접속을 완료했을때 보내주는 메시지
    // true로 된다면 접속을 성공했다고 알려주는 것이다.
    public void SendStateWaitMesssage(bool Wait)
    {

        _StateWaitMessage.Clear();
        _StateWaitMessage.Add(_protocolVersion);
        _StateWaitMessage.Add((byte)'W');
        _StateWaitMessage.AddRange(System.BitConverter.GetBytes(Wait));

        byte[] StateMessageToSend = _StateWaitMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, StateMessageToSend);
    }

    // 서로가 무기를 고를때 완료했으면 보내주는 메시지
    // true가 된다면 고르기를 완료 한 것이다.
    public void SendStateSelectMesssage(bool Select)
    {

        _StateSelectMessage.Clear();
        _StateSelectMessage.Add(_protocolVersion);
        _StateSelectMessage.Add((byte)'Q');
        _StateSelectMessage.AddRange(System.BitConverter.GetBytes(Select));

        byte[] StateMessageToSend = _StateSelectMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, StateMessageToSend);
    }

    //// 상대방 총알의 궤도를 보내는 메시지
    //// 
    //public void SendShootMessage(Vector3 vec)
    //{
    //    _ShootMessage.Clear();
    //    _ShootMessage.Add(_protocolVersion);
    //    _ShootMessage.Add((byte)'S');
    //    _ShootMessage.AddRange(System.BitConverter.GetBytes(vec.x));
    //    _ShootMessage.AddRange(System.BitConverter.GetBytes(vec.y));
    //    _ShootMessage.AddRange(System.BitConverter.GetBytes(vec.z));

    //    byte[] ShotMessageToSend = _ShootMessage.ToArray();

    //    PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, ShotMessageToSend);
    //}

    // 데드아이 상태값을 보내는 메시지
    // 데드아이 경과 시간을 보내준다.
    public void SendDeadEyeTimerMessage(float DeadEyeSet)
    {
        _DeadEyeTimerMessage.Clear();
        _DeadEyeTimerMessage.Add(_protocolVersion);
        _DeadEyeTimerMessage.Add((byte)'E');
        _DeadEyeTimerMessage.AddRange(System.BitConverter.GetBytes(DeadEyeSet));

        byte[] DeadEyeMessageToSend = _DeadEyeTimerMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, DeadEyeMessageToSend);
    }

    // 데드아이 리스폰 위치를 보내는 메시지
    // 데드아이의 위치를 난수를 보내준다.
    public void SendDeadEyeIndexMessage()
    {
        _DeadEyeRespawnMessage.Clear();
        _DeadEyeRespawnMessage.Add(_protocolVersion);
        _DeadEyeRespawnMessage.Add((byte)'I');

        int index = UnityEngine.Random.Range(0, 5);


        _DeadEyeRespawnMessage.AddRange(System.BitConverter.GetBytes(index));

        byte[] DeadEyeMessageToSend = _DeadEyeRespawnMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, DeadEyeMessageToSend);
    }

    // 데드아이 상태값을 보내는 메시지
    // bool 값이 true라면 데드 아이가 끝난 것을 의미한다.
    public void SendDeadEyeMessage(bool DeadEyeSet)
    {
        _DeadEyeMessage.Clear();
        _DeadEyeMessage.Add(_protocolVersion);
        _DeadEyeMessage.Add((byte)'D');
        _DeadEyeMessage.AddRange(System.BitConverter.GetBytes(DeadEyeSet));

        byte[] DeadEyeMessageToSend = _DeadEyeMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, DeadEyeMessageToSend);
    }

    // HP 값을 보내는 메시지
    public void SendCharacterHP(int HPState)
    {
        _HealthMessage.Clear();
        _HealthMessage.Add(_protocolVersion);
        _HealthMessage.Add((byte)'H');
        _HealthMessage.AddRange(System.BitConverter.GetBytes(HPState));

        byte[] HealthMessageToSend = _HealthMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, HealthMessageToSend);
    }

    // 자신이 선택한 캐릭터의 고유번호를 보내는 메시지
    public void SendCharacterSelectNumber(int CharacterNumber = 100)
    {
        _CharacterSelectMessage.Clear();
        _CharacterSelectMessage.Add(_protocolVersion);
        _CharacterSelectMessage.Add((byte)'C');
        _CharacterSelectMessage.AddRange(System.BitConverter.GetBytes(CharacterNumber));

        byte[] CharacterSelectMessageToSend = _CharacterSelectMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, CharacterSelectMessageToSend);
    }

    // 자신이 선택한 무기의 고유번호를 보내는 메시지
    public void SendWeaponSelectNumber(int WeaponNumber = 100)
    {

        _WeaponSelectMessage.Clear();
        _WeaponSelectMessage.Add(_protocolVersion);
        _WeaponSelectMessage.Add((byte)'P');
        _WeaponSelectMessage.AddRange(System.BitConverter.GetBytes(WeaponNumber));

        byte[] WeaponSelectMessageToSend = _WeaponSelectMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, WeaponSelectMessageToSend);
    }

    // 서바이벌 모드에서 보스 레이드 이벤트를 보내는 메시지
    public void SendBossAlertEvent(bool Alarm)
    {

        _BossAlarmMessage.Clear();
        _BossAlarmMessage.Add(_protocolVersion);
        _BossAlarmMessage.Add((byte)'B');
        _BossAlarmMessage.Add((byte)'E');
        _BossAlarmMessage.AddRange(System.BitConverter.GetBytes(Alarm));

        byte[] _BossAlarmMessageToSend = _BossAlarmMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, _BossAlarmMessageToSend);
    }

    // 서바이벌 모드에서 보스의 위치를 동기화 하는 메시지
    public void SendBossPosition(float posX, float posY, float posZ, float rotY)
    {

        _BossPosMessage.Clear();
        _BossPosMessage.Add(_protocolVersion);
        _BossPosMessage.Add((byte)'B');
        _BossPosMessage.Add((byte)'U');
        _BossPosMessage.AddRange(System.BitConverter.GetBytes(++_BossPosMessageNum));
        _BossPosMessage.AddRange(System.BitConverter.GetBytes(posX));
        _BossPosMessage.AddRange(System.BitConverter.GetBytes(posY));
        _BossPosMessage.AddRange(System.BitConverter.GetBytes(posZ));
        _BossPosMessage.AddRange(System.BitConverter.GetBytes(rotY));

        byte[] messageToSend = _BossPosMessage.ToArray();

        Debug.Log("Sending Boss update  Position message  " + messageToSend + " to all players in the room");

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, messageToSend);
        //PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend);
    }

    // 서바이벌 모드에서 보스의 애니메이션을 동기화 하는 메시지
    public void SendBossAnimation(int AnimationNum)
    {
        _BossAlarmMessage.Clear();
        _BossAlarmMessage.Add(_protocolVersion);
        _BossAlarmMessage.Add((byte)'B');
        _BossAlarmMessage.Add((byte)'A');
        _BossAlarmMessage.AddRange(System.BitConverter.GetBytes(AnimationNum));

        byte[] _BossAlarmMessageToSend = _BossAlarmMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, _BossAlarmMessageToSend);
    }

    // 서바이벌 모드에서 보스의 HP 상태를 동기화 하는 메시지
    public void SendBossHPState(int HPState)
    {
        _BossHPStateMeesage.Clear();
        _BossHPStateMeesage.Add(_protocolVersion);
        _BossHPStateMeesage.Add((byte)'B');
        _BossHPStateMeesage.Add((byte)'H');
        _BossHPStateMeesage.AddRange(System.BitConverter.GetBytes(HPState));

        byte[] BossHPStateMessageToSend = _BossHPStateMeesage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, BossHPStateMessageToSend);
    }

    // 서바이벌 모드에서 보스의 사망 여부를 동기화 하는 메시지
    public void SendBossDeadState(bool DeadState)
    {
        _BossDeadEventMessage.Clear();
        _BossDeadEventMessage.Add(_protocolVersion);
        _BossDeadEventMessage.Add((byte)'B');
        _BossDeadEventMessage.Add((byte)'D');
        _BossDeadEventMessage.AddRange(System.BitConverter.GetBytes(DeadState));

        byte[] BossDeadStateMessageToSend = _BossDeadEventMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, BossDeadStateMessageToSend);
    }

    // 상대 ID로부터 메시지를 받았을때 호출되는 리스너 함수
    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        //ShowMPStatus("We have received some gameplay messages from participant ID:" + senderId);

        // We'll be doing more with this later...
        byte messageVersion = (byte)data[0];
        // Let's figure out what type of message this is.
        char messageType = (char)data[1];

        switch(messageType)
        {
            case 'U':
                {
                    int messageNum = System.BitConverter.ToInt32(data, 2);
                    float posX = System.BitConverter.ToSingle(data, 6);
                    float posY = System.BitConverter.ToSingle(data, 10);
                    float posZ = System.BitConverter.ToSingle(data, 14);
                    float rotY = System.BitConverter.ToSingle(data, 18);
                    //float rotZ = System.BitConverter.ToSingle(data, 18);
                    //Debug.Log("Player " + senderId + " is at (" + posX + ", " + posY + ") rotation " + rotY);

                    ReceiveMessage = ByteToString(data);
                    // We'd better tell our GameController about this.
                    //updateListener.UpdateReceived(senderId, posX, posY, velX, velY, rotZ);

                    if (updateListener != null)
                    {
                        updateListener.UpdatePositionReceived(senderId, messageNum, posX, posY, posZ, rotY);
                    }

                }
                break;

            case 'F':
                {
                    bool GameOver = System.BitConverter.ToBoolean(data, 2);

                    Debug.Log("This Game Is End");

                    ReceiveMessage = ByteToString(data);

                    if (updateListener != null)
                    {
                        updateListener.FinishedReceived(senderId, GameOver);
                    }
                }
                break;

            case 'I':
                {
                    int index = System.BitConverter.ToInt32(data, 2);

                    if (updateListener != null)
                    {
                        //switch(MultiGameMode)
                        //{
                        //    case HY.MultiGameModeState.NONE:
                        //        {

                        //        }
                        //        break;

                        //    case HY.MultiGameModeState.PVP:
                        //        {
                        //            updateListener.DeadEyeRespawnIndexReceived(index);
                        //        }
                        //        break;

                        //    case HY.MultiGameModeState.SURVIVAL:
                        //        {
                        //            updateListener.DeadEyeRespawnIndexReceived(senderId, index);
                        //        }
                        //        break;
                        //}

                        updateListener.DeadEyeRespawnIndexReceived(senderId, index);
                    }
                }
                break;

            case 'S':
                {
                    bool shoot = System.BitConverter.ToBoolean(data, 2);

                    Debug.Log("Shoot Get");

                    ReceiveMessage = ByteToString(data);

                    if (updateListener != null)
                    {
                        //updateListener.ItemStateReceived(Index, ItemGet);
                        updateListener.ShootStateReceived(senderId, shoot);
                    }
                }
                break;

            case 'D':
                {
                    bool DeadEyeActive = System.BitConverter.ToBoolean(data, 2);

                    Debug.Log("DeadEye Message!");


                    ReceiveMessage = ByteToString(data);

                    if (updateListener != null)
                    {
                        //updateListener.ItemStateReceived(Index, ItemGet);
                        updateListener.DeadEyeStateReceived(senderId, DeadEyeActive);
                    }
                }
                break;

            case 'E':
                {
                    float DeadEyeActive = System.BitConverter.ToSingle(data, 2);

                    Debug.Log("DeadEye Timer Get");

                    ReceiveMessage = ByteToString(data);

                    if (updateListener != null)
                    {
                        //updateListener.ItemStateReceived(Index, ItemGet);
                        updateListener.DeadEyeTimerStateReceived(senderId, DeadEyeActive);
                    }
                }
                break;

            case 'A':
                {
                    int AniStateNumber = System.BitConverter.ToInt32(data, 2);

                    //Debug.Log("AniState Get");

                    ReceiveMessage = ByteToString(data);

                    if (updateListener != null)
                    {
                        //updateListener.ItemStateReceived(Index, ItemGet);
                        updateListener.AniStateReceived(senderId, AniStateNumber);
                    }
                }
                break;

            case 'H':
                {
                    int HPState = System.BitConverter.ToInt32(data, 2);

                    Debug.Log("HPState Get");

                    ReceiveMessage = ByteToString(data);

                    if (updateListener != null)
                    {
                        //updateListener.ItemStateReceived(Index, ItemGet);
                        updateListener.HPStateReceived(senderId, HPState);
                    }
                }
                break;

            case 'Q':
                {
                    bool SelectOut = System.BitConverter.ToBoolean(data, 2);

                    Debug.Log("Select Complete");

                    ReceiveMessage = ByteToString(data);

                    if (updateListener != null)
                    {
                        //updateListener.ItemStateReceived(Index, ItemGet);
                        updateListener.MultiStateSelectReceived(senderId, SelectOut);
                    }
                }
                break;

            case 'W':
                {
                    bool WaitOut = System.BitConverter.ToBoolean(data, 2);

                    Debug.Log("Wait signal is over");

                    ReceiveMessage = ByteToString(data);

                    if (updateListener != null)
                    {
                        //updateListener.ItemStateReceived(Index, ItemGet);
                        updateListener.MultiStateWaitReceived(senderId, WaitOut);
                    }
                }
                break;

            case 'C':
                {
                    int CharacterNumber = System.BitConverter.ToInt32(data, 2);



                    ReceiveMessage = ByteToString(data);

                    if (LBListener != null)
                    {
                        OppenentCharNumber = CharacterNumber;

                        switch (NowMultiGameMode)
                        {
                            case HY.MultiGameModeState.PVP:
                                {

                                    Debug.Log("Character Number is : " + CharacterNumber);

                                    OppenentCharNumber = CharacterNumber;
                                    //LBListener.OpponentCharacterNumberReceive(senderId, CharacterNumber);
                                }
                                break;

                            case HY.MultiGameModeState.SURVIVAL:
                                {
                                    SurvivalOpponentCharNumbers[senderId] = CharacterNumber;
                                    //PlayerCharacters_Number[senderId] = CharacterNumber;
                                    Debug.Log("Survival Char ID : " + senderId);
                                    Debug.Log("Survival Char Number : " + SurvivalOpponentCharNumbers[senderId]);
                                    //LBListener.OpponentCharacterNumberReceive(senderId, CharacterNumber);
                                }
                                break;

                            default:
                                {
                                    OppenentCharNumber = CharacterNumber;
                                }
                                break;
                        }

                        if (LBListener != null)
                        {
                            LBListener.OpponentCharacterNumberReceive(senderId, CharacterNumber);
                        }

                    }
                    //if (updateListener != null)
                    //{
                    //    //updateListener.ItemStateReceived(Index, ItemGet);
                    //    updateListener.CharacterSelectStateReceived(CharacterNumber);
                    //}
                }
                break;

            case 'P':
                {
                    int WeaponNumber = System.BitConverter.ToInt32(data, 2);

                    Debug.Log("Weapon Number is : " + WeaponNumber);

                    ReceiveMessage = ByteToString(data);

                    if (updateListener != null)
                    {
                        //updateListener.ItemStateReceived(Index, ItemGet);
                        updateListener.WeaponSelectStateReceived(senderId, WeaponNumber);
                    }
                }
                break;

            case 'V':
                {
                    float VecX = System.BitConverter.ToSingle(data, 2);
                    float VecY = System.BitConverter.ToSingle(data, 6);
                    float VecZ = System.BitConverter.ToSingle(data, 10);

                    Debug.Log("Player " + senderId + " is at (" + VecX + ", " + VecY + ", " + VecZ + " )");

                    ReceiveMessage = ByteToString(data);
                    // We'd better tell our GameController about this.
                    //updateListener.UpdateReceived(senderId, posX, posY, velX, velY, rotZ);

                    if (updateListener != null)
                    {
                        updateListener.ShootVectorReceived(senderId, VecX, VecY, VecZ);
                    }
                }
                break;

            case 'B':
                {
                    char BossMessageType = (char)data[2];

                    switch(BossMessageType)
                    {
                        case 'A':
                            {
                               int AnimNumber = System.BitConverter.ToInt32(data, 3);

                                Debug.Log("Boss Anim : " + AnimNumber);

                                if (updateListener != null)
                                {
                                    updateListener.BossAnimStateReceived(AnimNumber);
                                }
                            }
                            break;

                        case 'D':
                            {
                                bool IsDead = System.BitConverter.ToBoolean(data, 3);

                                Debug.Log("Boss IS Dead : " + IsDead);

                                if (updateListener != null)
                                {
                                    updateListener.BossDeadEventReceived(IsDead);
                                }
                            }
                            break;

                        case 'U':
                            {
                                int messageNum = System.BitConverter.ToInt32(data, 3);
                                float posX = System.BitConverter.ToSingle(data, 7);
                                float posY = System.BitConverter.ToSingle(data, 11);
                                float posZ = System.BitConverter.ToSingle(data, 15);
                                float rotY = System.BitConverter.ToSingle(data, 19);
                                //float rotZ = System.BitConverter.ToSingle(data, 18);

                                Debug.Log("Num : " + messageNum + "Boss is at (" + posX + ", " + posY + ") rotation " + rotY);

                                ReceiveMessage = ByteToString(data);
                                // We'd better tell our GameController about this.
                                //updateListener.UpdateReceived(senderId, posX, posY, velX, velY, rotZ);

                                if (updateListener != null)
                                {
                                    updateListener.BossPosReceived(messageNum, posX, posY, posZ, rotY);
                                }
                            }
                            break;

                        case 'E':
                            {
                                bool Alarm = System.BitConverter.ToBoolean(data, 3);

                                Debug.Log("Boss Raid Event Trigger On : " + Alarm);

                                if (updateListener != null)
                                {
                                    updateListener.BossRaidAlarmReceived(Alarm);
                                }
                            }
                            break;

                        case 'H':
                            {
                                int bossHP = System.BitConverter.ToInt32(data, 3);

                                Debug.Log("Boss HP : " + bossHP);

                                if(updateListener != null)
                                {
                                    updateListener.BossHPStateReceived(bossHP);
                                }
                            }
                            break;
                    }

                }
                break;
        }

        #region Old Message if
        
        ////if (messageType == 'U' && data.Length == _updateMessageLength)
        //if (messageType == 'U')
        //{
        //    int messageNum = System.BitConverter.ToInt32(data, 2);
        //    float posX = System.BitConverter.ToSingle(data, 6);
        //    float posY = System.BitConverter.ToSingle(data, 10);
        //    float posZ = System.BitConverter.ToSingle(data, 14);
        //    float rotY = System.BitConverter.ToSingle(data, 18);
        //    //float rotZ = System.BitConverter.ToSingle(data, 18);
        //    Debug.Log("Player " + senderId + " is at (" + posX + ", " + posY + ") rotation " + rotY);

        //    ReceiveMessage = ByteToString(data);
        //    // We'd better tell our GameController about this.
        //    //updateListener.UpdateReceived(senderId, posX, posY, velX, velY, rotZ);

        //    if (updateListener != null)
        //    {
        //        updateListener.UpdatePositionReceived(senderId, messageNum, posX, posY, posZ, rotY);
        //    }

        //}
        //else if(messageType == 'F')
        //{
        //    bool GameOver = System.BitConverter.ToBoolean(data, 2);

        //    Debug.Log("This Game Is End");

        //    ReceiveMessage = ByteToString(data);

        //    if(updateListener != null)
        //    {
        //        updateListener.FinishedReceived(senderId, GameOver);
        //    }
        //}
        //else if(messageType == 'I')
        //{
        //    //bool ItemGet = System.BitConverter.ToBoolean(data, 2);
        //    //int Index = System.BitConverter.ToInt32(data, 3);

        //    //Debug.Log("Item Get");

        //    //ReceiveMessage = ByteToString(data);

        //    //if (updateListener != null)
        //    //{
        //    //    //updateListener.ItemStateReceived(Index, ItemGet);
        //    //    updateListener.ItemStateReceived(0, ItemGet);
        //    //}
        //    int index = System.BitConverter.ToInt32(data, 2);

        //    if(updateListener != null)
        //    {
        //        //switch(MultiGameMode)
        //        //{
        //        //    case HY.MultiGameModeState.NONE:
        //        //        {

        //        //        }
        //        //        break;

        //        //    case HY.MultiGameModeState.PVP:
        //        //        {
        //        //            updateListener.DeadEyeRespawnIndexReceived(index);
        //        //        }
        //        //        break;

        //        //    case HY.MultiGameModeState.SURVIVAL:
        //        //        {
        //        //            updateListener.DeadEyeRespawnIndexReceived(senderId, index);
        //        //        }
        //        //        break;
        //        //}

        //        updateListener.DeadEyeRespawnIndexReceived(senderId, index);
        //    }
        //}
        //else if(messageType == 'S')
        //{
        //    bool shoot = System.BitConverter.ToBoolean(data, 2);

        //    Debug.Log("Shoot Get");

        //    ReceiveMessage = ByteToString(data);

        //    if (updateListener != null)
        //    {
        //        //updateListener.ItemStateReceived(Index, ItemGet);
        //        updateListener.ShootStateReceived(senderId, shoot);
        //    }
        //}
        //else if(messageType == 'D')
        //{
        //    bool DeadEyeActive = System.BitConverter.ToBoolean(data, 2);

        //    Debug.Log("DeadEye Message!");
            

        //    ReceiveMessage = ByteToString(data);

        //    if (updateListener != null)
        //    {
        //        //updateListener.ItemStateReceived(Index, ItemGet);
        //        updateListener.DeadEyeStateReceived(senderId, DeadEyeActive);
        //    }
        //}
        //else if (messageType == 'E')
        //{
        //    float DeadEyeActive = System.BitConverter.ToSingle(data, 2);

        //    Debug.Log("DeadEye Timer Get");

        //    ReceiveMessage = ByteToString(data);

        //    if (updateListener != null)
        //    {
        //        //updateListener.ItemStateReceived(Index, ItemGet);
        //        updateListener.DeadEyeTimerStateReceived(senderId, DeadEyeActive);
        //    }
        //}
        //else if(messageType == 'A')
        //{
        //    int AniStateNumber = System.BitConverter.ToInt32(data, 2);

        //    Debug.Log("AniState Get");

        //    ReceiveMessage = ByteToString(data);

        //    if (updateListener != null)
        //    {
        //        //updateListener.ItemStateReceived(Index, ItemGet);
        //        updateListener.AniStateReceived(senderId, AniStateNumber);
        //    }
        //}
        //else if(messageType == 'H')
        //{
        //    int HPState = System.BitConverter.ToInt32(data, 2);

        //    Debug.Log("HPState Get");

        //    ReceiveMessage = ByteToString(data);

        //    if (updateListener != null)
        //    {
        //        //updateListener.ItemStateReceived(Index, ItemGet);
        //        updateListener.HPStateReceived(senderId, HPState);
        //    }
        //}
        //else if(messageType == 'Q')
        //{
        //    bool SelectOut = System.BitConverter.ToBoolean(data, 2);

        //    Debug.Log("Select Complete");

        //    ReceiveMessage = ByteToString(data);

        //    if (updateListener != null)
        //    {
        //        //updateListener.ItemStateReceived(Index, ItemGet);
        //        updateListener.MultiStateSelectReceived(senderId, SelectOut);
        //    }
        //}
        //else if(messageType == 'W')
        //{
        //    bool WaitOut = System.BitConverter.ToBoolean(data, 2);

        //    Debug.Log("Wait signal is over");

        //    ReceiveMessage = ByteToString(data);

        //    if (updateListener != null)
        //    {
        //        //updateListener.ItemStateReceived(Index, ItemGet);
        //        updateListener.MultiStateWaitReceived(senderId, WaitOut);
        //    }
        //}
        //else if (messageType == 'C')
        //{
        //    int CharacterNumber = System.BitConverter.ToInt32(data, 2);

            

        //    Debug.Log("Character Number is : " + CharacterNumber);

        //    ReceiveMessage = ByteToString(data);

        //    if(LBListener != null)
        //    {
        //        OppenentCharNumber = CharacterNumber;

        //        switch(NowMultiGameMode)
        //        {
        //            case HY.MultiGameModeState.PVP:
        //                {
        //                    OppenentCharNumber = CharacterNumber;
        //                    //LBListener.OpponentCharacterNumberReceive(senderId, CharacterNumber);
        //                }
        //                break;

        //            case HY.MultiGameModeState.SURVIVAL:
        //                {
        //                    SurvivalOpponentCharNumbers[senderId] = CharacterNumber;
        //                    //LBListener.OpponentCharacterNumberReceive(senderId, CharacterNumber);
        //                }
        //                break;

        //            default:
        //                {
        //                    OppenentCharNumber = CharacterNumber;
        //                }
        //                break;
        //        }

        //        if(LBListener != null)
        //        {
        //            LBListener.OpponentCharacterNumberReceive(senderId, CharacterNumber);
        //        }

        //    }
        //    //if (updateListener != null)
        //    //{
        //    //    //updateListener.ItemStateReceived(Index, ItemGet);
        //    //    updateListener.CharacterSelectStateReceived(CharacterNumber);
        //    //}
        //}
        //else if (messageType == 'P')
        //{
        //    int WeaponNumber = System.BitConverter.ToInt32(data, 2);

        //    Debug.Log("Weapon Number is : " + WeaponNumber);

        //    ReceiveMessage = ByteToString(data);

        //    if (updateListener != null)
        //    {
        //        //updateListener.ItemStateReceived(Index, ItemGet);
        //        updateListener.WeaponSelectStateReceived(senderId, WeaponNumber);
        //    }
        //}
        //else if (messageType == 'V')
        //{
        //    float VecX = System.BitConverter.ToSingle(data, 2);
        //    float VecY = System.BitConverter.ToSingle(data, 6);
        //    float VecZ = System.BitConverter.ToSingle(data, 10);
            
        //    Debug.Log("Player " + senderId + " is at (" + VecX + ", " + VecY + ", " + VecZ + " )");

        //    ReceiveMessage = ByteToString(data);
        //    // We'd better tell our GameController about this.
        //    //updateListener.UpdateReceived(senderId, posX, posY, velX, velY, rotZ);

        //    if (updateListener != null)
        //    {
        //        updateListener.ShootVectorReceived(senderId, VecX, VecY, VecZ);
        //    }

        //}
        //else if(messageType == 'B')
        //{
        //    bool Alarm = System.BitConverter.ToBoolean(data, 2);

        //    Debug.Log("Boss Raid Event Trigger On : " + Alarm);

        //    if(updateListener != null)
        //    {
        //        updateListener.BossRaidAlarm(Alarm);
        //    }
        //}
#endregion
    }

    // GPGS를 로그인 합니다.
    public void LoginGPGS()
    {
        // 로그인이 안되어 있으면..
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(LoginCallBackGPGS);
        }
    }


    public string GetMyParticipantId()
    {
        return PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
    }

    // 방을 나갈때 호출해주면
    // 현재 멀티 플레이가 되고 있는 게임이 끝나게 된다.
    public void LeaveGame()
    {
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    }

    // 리 매칭(ReMatching)을 할때 초기화 해주는 옵션
    public void ReMatchingInit(int Option)
    {
        switch(Option)
        {
            case 0:
                {
                    bLogin = false;
                    IsConnectedOn = false;

                    NowMultiGameMode = HY.MultiGameModeState.NONE;

                    _myMessageNum = 0;

                }
                break;

            case 1:
                {
                    bLogin = false;
                    IsConnectedOn = false;

                    NowMultiGameMode = HY.MultiGameModeState.PVP;

                    _myMessageNum = 0;

                    StartMatchMaking();
                }
                break;

            case 2:
                {
                    bLogin = false;
                    IsConnectedOn = false;

                    NowMultiGameMode = HY.MultiGameModeState.SURVIVAL;

                    _myMessageNum = 0;

                    StartMatchMaking();
                }
                break;

            default:
                {
                    bLogin = false;
                    IsConnectedOn = false;

                    NowMultiGameMode = HY.MultiGameModeState.NONE;

                    _myMessageNum = 0;
                }
                break;
        }
    }

    public void LeftRoomInit()
    {
        bLogin = false;
        IsConnectedOn = false;

        NowMultiGameMode = HY.MultiGameModeState.NONE;

        _myMessageNum = 0;

    }


    public List<Participant> GetAllPlayers()
    {
        return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
    }

    #region LeaderBoard Settings

    // 리더보드 UI를 활성화 시킵니다.
    public void OpenLeaderboardUI()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
        else
        {
            Debug.Log("OpenLeaderboard : Not Loggin Google Play GameService");
        }
    }

    /// <summary>
    /// 리더보드에 점수를 보내어 서버에 갱신시키고 순위를 매겨준다.
    /// LeaderBoardID는 어느 리더보드에 저장할지의 대한 ID값이다.
    /// Score는 보낼 점수
    /// ONESHOT_Cloud 안에 있는 ID값을 써주면 된다.
    /// </summary>
    /// <param name="LeaderBoardID"></param>
    /// <param name="Score"></param>
    public void Send_LeaderBoard_Score(string LeaderBoardID, long Score)
    {
        if (Social.localUser.authenticated)
        {
            if(Score < 0)
            {
                Score = 0;
            }
            else if(Score > long.MaxValue)
            {
                Score = long.MaxValue;
            }

            Social.ReportScore(Score, LeaderBoardID, (bool success) =>
            {
                Debug.Log("Send_LeaderBoard_Score [ID] : " + LeaderBoardID + " [Score] : " + Score);
            });
        }
    }

    #endregion

    #region Acheievement Settings

    // 도전과제 UI를 활성화 시킵니다.
    public void OpenAcheieveMentUI()
    {
        if(Social.localUser.authenticated)
        {
            Social.ShowAchievementsUI();
        }
        else
        {
            Debug.Log("OpenAchievement : Not Loggin Google Play GameService");
        }
    }

    /// <summary>
    /// 해당하는 ID의 도전과제를 언락 시킵니다.
    /// ONESHOT_Cloud 안에 있는 ID값을 써주면 된다.
    /// </summary>
    /// <param name="AchieveID"></param>
    public void UnlockAcheievement(string AchieveID)
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportProgress(AchieveID, 100.0f, (bool success) =>
            {
                Debug.Log("UnlockAcheievement : " + AchieveID);
            });
        }

    }

    /// <summary>
    /// 도전과제를 언락시킬때 바로 언락이 아닌 진행도를 두어 100이 채워지면 해제 되는 식입니다.
    /// AchieveID는 언락 시킬 도전과제의 고유 ID입니다.
    /// Progress는 진행도입니다. 이것이 100이 되면 해당 도전과제는 언락됩니다.
    /// ONESHOT_Cloud 안에 있는 ID값을 써주면 된다.
    /// </summary>
    /// <param name="AchieveID"></param>
    /// <param name="Progress"></param>
    public void UnlockAcheievement(string AchieveID, int Progress)
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(AchieveID, Progress, (bool success) =>
            {
                Debug.Log("UnlockAcheievement : " + AchieveID);
            });
        }

    }

    #endregion


    // 바이트 배열을 String으로 변환 
    private string ByteToString(byte[] strByte)
    {
        string str = System.Text.Encoding.UTF8.GetString(strByte);//Encoding.Default.GetString(StrByte);
        return str;
    }

    // String을 바이트 배열로 변환 
    private byte[] StringToByte(string str)
    {

        byte[] StrByte = System.Text.Encoding.UTF8.GetBytes(str);//Encoding.UTF8.GetBytes(str);
        return StrByte;
    }


    // GPGS 로그인 콜백
    public void LoginCallBackGPGS(bool result)
    {
        bLogin = result;
    }

    // GPGS를 로그아웃 합니다.
    public void LogoutGPGS()
    {
        // 로그인이 되어 있으면
        if (Social.localUser.authenticated)
        {
            ((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
            bLogin = false;
        }
    }

    // GPGS에서 자신의 프로필 이미지를 가져옵니다.
    public Texture2D GetImageGPGS()
    {
        if (Social.localUser.authenticated)
        {
            return Social.localUser.image;
        }
        else
        {
            return null;
        }
    }

    // GPGS 에서 사용자 이름을 가져옵니다
    public string GetNameGPGS()
    {
        if (Social.localUser.authenticated)
        {
            return Social.localUser.userName;
        }
        else
        {
            return null;
        }
    }

    public string GetOtherNameGPGS(int index)
    {
        string UnknownStr;

        if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[index] != null)
        {
            return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[index].DisplayName;
        }
        else
        {
            UnknownStr = "UnKnown Player";

            return UnknownStr;
        }
    }
}
