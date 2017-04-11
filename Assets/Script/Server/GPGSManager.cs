using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi;

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
    // Byte + Byte + 1 Vector
    private int _shootMessageLength = 14;
    // Byte + Byte + 1 Boolean
    private int _deadeyeMessageLength = 3;
    // Byte + Byte + 1 Interger
    private int _animMessageLength = 6;
    // Byte + Byte + 1 Interget
    private int _healthMessageLength = 6;

    // 메시지 도착 순서를 제어할 변수
    // 네트워크 도착 순서가 무작위로 된다면 동기화가 이상하게 될 가능성이 있기에
    // 이것을 보정해줄 변수이다.
    private int _myMessageNum;

    private List<byte> _updateMessage;
    private List<byte> _endMessage;
    private List<byte> _itemstateMessage;
    private List<byte> _ShootMessage;
    private List<byte> _DeadEyeMessage;
    private List<byte> _AnimMessage;
    private List<byte> _HealthMessage;

    private bool IsConnectedOn = false;
    private bool showingWaitingRoom = false;

    private string ReceiveMessage = " ";
    private string SendMessage = " ";
    private string NetMessage = " ";

    public MPUpdateListener updateListener;

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

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

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

        if (_DeadEyeMessage == null)
        {
            _DeadEyeMessage = new List<byte>(_deadeyeMessageLength);
        }

        if (_AnimMessage == null)
        {
            _AnimMessage = new List<byte>(_animMessageLength);
        }

        if(_HealthMessage == null)
        {
            _HealthMessage = new List<byte>(_healthMessageLength);
        }

        _myMessageNum = 0;
    }

    // P2P 방식으로 상대방을 검색하기 시작한다.
    public void StartMatchMaking()
    {
        // 최소 수용 인원
        // 최대 수용 인원
        PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
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

        if (_DeadEyeMessage == null)
        {
            _DeadEyeMessage = new List<byte>(_deadeyeMessageLength);
        }

        if (_AnimMessage == null)
        {
            _AnimMessage = new List<byte>(_animMessageLength);
        }


        if (_HealthMessage == null)
        {
            _HealthMessage = new List<byte>(_healthMessageLength);
        }
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

        if (!showingWaitingRoom)
        {
            showingWaitingRoom = true;
            PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();
        }
    }

    // 멀티플레이 방이 연결되었는지의 여부
    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            ShowMPStatus("We are connected to the room! I would probably start our game now.");
            IsConnectedOn = true;
            _myMessageNum = 0;
        }
        else
        {
            ShowMPStatus("Uh-oh. Encountered some error connecting to the room.");
            IsConnectedOn = false;
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
    public void SendItemStateMessage(int Index, bool GetItem)
    {
        _itemstateMessage.Clear();
        _itemstateMessage.Add(_protocolVersion);
        _itemstateMessage.Add((byte)'I');
        _itemstateMessage.AddRange(System.BitConverter.GetBytes(GetItem));
        _itemstateMessage.AddRange(System.BitConverter.GetBytes(Index));

        byte[] ItemStateMessageToSend = _itemstateMessage.ToArray();

        Debug.Log("Sending my update message  " + ItemStateMessageToSend + " to all players in the room");

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, ItemStateMessageToSend);
    }

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

    // 상대방 총알의 궤도를 보내는 메시지
    public void SendShootMessage(float x, float y, float z)
    {
        _ShootMessage.Clear();
        _ShootMessage.Add(_protocolVersion);
        _ShootMessage.Add((byte)'S');
        _ShootMessage.AddRange(System.BitConverter.GetBytes(x));
        _ShootMessage.AddRange(System.BitConverter.GetBytes(y));
        _ShootMessage.AddRange(System.BitConverter.GetBytes(z));

        byte[] ShotMessageToSend = _ShootMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, ShotMessageToSend);
    }

    // 상대방 총알의 궤도를 보내는 메시지
    // 
    public void SendShootMessage(Vector3 vec)
    {
        _ShootMessage.Clear();
        _ShootMessage.Add(_protocolVersion);
        _ShootMessage.Add((byte)'S');
        _ShootMessage.AddRange(System.BitConverter.GetBytes(vec.x));
        _ShootMessage.AddRange(System.BitConverter.GetBytes(vec.y));
        _ShootMessage.AddRange(System.BitConverter.GetBytes(vec.z));

        byte[] ShotMessageToSend = _ShootMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, ShotMessageToSend);
    }

    // 데드아이 상태값을 보내는 메시지
    public void SendDeadEyeMessage(bool DeadEyeOn)
    {
        _DeadEyeMessage.Clear();
        _DeadEyeMessage.Add(_protocolVersion);
        _DeadEyeMessage.Add((byte)'D');
        _DeadEyeMessage.AddRange(System.BitConverter.GetBytes(DeadEyeOn));

        byte[] DeadEyeMessageToSend = _DeadEyeMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, DeadEyeMessageToSend);
    }

    // HP 값을 보내는 메시지
    public void SendCharacterHP(int HPState)
    {
        _HealthMessage.Clear();
        _HealthMessage.Add(_protocolVersion);
        _HealthMessage.Add((byte)'H');
        _HealthMessage.AddRange(System.BitConverter.GetBytes(HPState));

        byte[] HealthMessageToSend = _HealthMessage.ToArray();

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, HealthMessageToSend);
    }

    // 상대 ID로부터 메시지를 받았을때 호출되는 리스너 함수
    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        ShowMPStatus("We have received some gameplay messages from participant ID:" + senderId);

        // We'll be doing more with this later...
        byte messageVersion = (byte)data[0];
        // Let's figure out what type of message this is.
        char messageType = (char)data[1];

        //if (messageType == 'U' && data.Length == _updateMessageLength)
        if (messageType == 'U')
        {
            int messageNum = System.BitConverter.ToInt32(data, 2);
            float posX = System.BitConverter.ToSingle(data, 6);
            float posY = System.BitConverter.ToSingle(data, 10);
            float posZ = System.BitConverter.ToSingle(data, 14);
            float rotY = System.BitConverter.ToSingle(data, 18);
            //float rotZ = System.BitConverter.ToSingle(data, 18);
            Debug.Log("Player " + senderId + " is at (" + posX + ", " + posY + ") rotation " + rotY);

            ReceiveMessage = ByteToString(data);
            // We'd better tell our GameController about this.
            //updateListener.UpdateReceived(senderId, posX, posY, velX, velY, rotZ);

            if (updateListener != null)
            {
                updateListener.UpdatePositionReceived(senderId, messageNum, posX, posY, posZ, rotY);
            }

        }
        else if(messageType == 'F')
        {
            bool GameOver = System.BitConverter.ToBoolean(data, 2);

            Debug.Log("This Game Is End");

            ReceiveMessage = ByteToString(data);

            if(updateListener != null)
            {
                updateListener.FinishedReceived(senderId, GameOver);
            }
        }
        else if(messageType == 'I')
        {
            bool ItemGet = System.BitConverter.ToBoolean(data, 2);
            int Index = System.BitConverter.ToInt32(data, 3);

            Debug.Log("Item Get");

            ReceiveMessage = ByteToString(data);

            if (updateListener != null)
            {
                //updateListener.ItemStateReceived(Index, ItemGet);
                updateListener.ItemStateReceived(0, ItemGet);
            }
        }
        else if(messageType == 'S')
        {
            float shoot_x = System.BitConverter.ToSingle(data, 2);
            float shoot_y = System.BitConverter.ToSingle(data, 6);
            float shoot_z = System.BitConverter.ToSingle(data, 10);

            Debug.Log("Shoot Get");

            ReceiveMessage = ByteToString(data);

            if (updateListener != null)
            {
                //updateListener.ItemStateReceived(Index, ItemGet);
                updateListener.ShootStateReceived(shoot_x, shoot_y, shoot_z);
            }
        }
        else if(messageType == 'D')
        {
            bool DeadEyeActive = System.BitConverter.ToBoolean(data, 2);

            Debug.Log("DeadEye Get");

            ReceiveMessage = ByteToString(data);

            if (updateListener != null)
            {
                //updateListener.ItemStateReceived(Index, ItemGet);
                updateListener.DeadEyeStateReceived(DeadEyeActive);
            }
        }
        else if(messageType == 'A')
        {
            int AniStateNumber = System.BitConverter.ToInt32(data, 2);

            Debug.Log("AniState Get");

            ReceiveMessage = ByteToString(data);

            if (updateListener != null)
            {
                //updateListener.ItemStateReceived(Index, ItemGet);
                updateListener.AniStateReceived(AniStateNumber);
            }
        }
        else if(messageType == 'H')
        {
            int HPState = System.BitConverter.ToInt32(data, 2);

            Debug.Log("HPState Get");

            ReceiveMessage = ByteToString(data);

            if (updateListener != null)
            {
                //updateListener.ItemStateReceived(Index, ItemGet);
                updateListener.HPStateReceived(HPState);
            }
        }
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


    public List<Participant> GetAllPlayers()
    {
        return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
    }

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

        if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[index] != null)
        {
            return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()[index].DisplayName;
        }
        else
        {
            return null;
        }
    }
}
