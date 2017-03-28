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

    // Byte + Byte + 2 floats for position + 2 floats for velcocity + 1 float for rotZ
    private int _updateMessageLength = 22;
    // Byte + Byte + 1 boolen for finish Game
    private int _finishMessageLength = 3;
    private List<byte> _updateMessage;
    private List<byte> _endMessage;
    
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
        _updateMessage = new List<byte>(_updateMessageLength);
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
        }
        else
        {
            ShowMPStatus("Uh-oh. Encountered some error connecting to the room.");
            IsConnectedOn = false;
        }
    }

    // 멀티플레이어가 나갔을때 호출되는 리스너 함수
    public void OnLeftRoom()
    {
        ShowMPStatus("We have left the room. We should probably perform some clean-up tasks.");

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
        foreach (string participantID in participantIds)
        {
            ShowMPStatus("Player " + participantID + " has left.");
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

    public void SendMyUpdate(float posX, float posY, Vector2 velocity, float rotZ)
    {


        _updateMessage.Clear();
        _updateMessage.Add(_protocolVersion);
        _updateMessage.Add((byte)'U');
        _updateMessage.AddRange(System.BitConverter.GetBytes(posX));
        _updateMessage.AddRange(System.BitConverter.GetBytes(posY));
        _updateMessage.AddRange(System.BitConverter.GetBytes(velocity.x));
        _updateMessage.AddRange(System.BitConverter.GetBytes(velocity.y));
        _updateMessage.AddRange(System.BitConverter.GetBytes(rotZ));
        byte[] messageToSend = _updateMessage.ToArray();

        Debug.Log("Sending my update message  " + messageToSend + " to all players in the room");

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, messageToSend);
        //PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend);
    }

    public void SendMyUpdate(string senderId, float posX, float posY, Vector2 velocity, float rotZ)
    {
        _updateMessage.Clear();
        _updateMessage.Add(_protocolVersion);
        _updateMessage.Add((byte)'U');
        _updateMessage.AddRange(System.BitConverter.GetBytes(posX));
        _updateMessage.AddRange(System.BitConverter.GetBytes(posY));
        _updateMessage.AddRange(System.BitConverter.GetBytes(velocity.x));
        _updateMessage.AddRange(System.BitConverter.GetBytes(velocity.y));
        _updateMessage.AddRange(System.BitConverter.GetBytes(rotZ));
        byte[] messageToSend = _updateMessage.ToArray();

        SendMessage = ByteToString(messageToSend);

        Debug.Log("Sending my update message  " + messageToSend + " to all players in the room");

        PlayGamesPlatform.Instance.RealTime.SendMessage(false, senderId, messageToSend);
    }

    // 게임이 끝났을때 보내지는 메시지
    public void SendFinishMessage(bool GameEnd)
    {
        List<byte> bytes = new List<byte>(_finishMessageLength);
        bytes.Add(_protocolVersion);
        bytes.Add((byte)'F');
        bytes.AddRange(System.BitConverter.GetBytes(GameEnd));
        byte[] messageToSend = bytes.ToArray();

        Debug.Log("Sending my update message  " + messageToSend + " to all players in the room");

        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend);
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
            float posX = System.BitConverter.ToSingle(data, 2);
            float posY = System.BitConverter.ToSingle(data, 6);
            float velX = System.BitConverter.ToSingle(data, 10);
            float velY = System.BitConverter.ToSingle(data, 14);
            float rotZ = System.BitConverter.ToSingle(data, 18);
            Debug.Log("Player " + senderId + " is at (" + posX + ", " + posY + ") traveling (" + velX + ", " + velY + ") rotation " + rotZ);

            ReceiveMessage = ByteToString(data);
            // We'd better tell our GameController about this.
            //updateListener.UpdateReceived(senderId, posX, posY, velX, velY, rotZ);

            if (updateListener != null)
            {
                updateListener.UpdateReceived(senderId, posX, posY, velX, velY, rotZ);
            }

        }
        else if(messageType == 'F')
        {
            bool GameOver = System.BitConverter.ToBoolean(data, 1);

            Debug.Log("This Game Is End");

            ReceiveMessage = ByteToString(data);

            if(updateListener != null)
            {
                updateListener.FinishedReceived(senderId, GameOver);
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
