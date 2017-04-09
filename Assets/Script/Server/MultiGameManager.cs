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
        START = 0,
        PLAY,
        GAMEOVER,
        GAMEWIN,
        CONNECTOUT
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

    // 적의 정보
    public GameObject EnemyCharacter;
    public GameObject EnemyCharacterPos;
    private Dictionary<string, MulEnemy> _opponentScripts;

    // 상대방이 갖고 있을 애니메이션 값
    private LSD.PlayerState m_state;

    private bool _multiplayerReady = false;
    private string _MyParticipantId;
    private string _EnemyParticipantId;
    private Vector2 _startingPoint;

    // 네트워크 최적화 부분
    private float _nextBroadcastTime;

    // 게임이 끝났는지의 여부
    public static bool ThisGameIsEnd;

    // 타임 아웃 정보
    [HideInInspector]
    public float timeOutThreshold;
    private float _timeOutCheckInterval;
    private float _nextTimeoutCheck;


    // Use this for initialization
    void Awake () {
		
        // 임시로 만든 상태 값이므로 추후에 수정해주세요
        MultiState = HY.MultiGameState.START;
        PlayerState = HY.MultiPlayerState.LIVE;

        ThisGameIsEnd = false;
        ItemCount = 0;

        // 네트워크 트래픽 최적화 변수 초기화
        _nextBroadcastTime = 0;

        // 타임 아웃 정보 초기화
        timeOutThreshold = 20.0f;
        _timeOutCheckInterval = 1.0f;
        _nextTimeoutCheck = 0.0f;

        SetupMultiplayerGame();
    }

    void SetupMultiplayerGame()
    {

        GPGSManager.GetInstance.updateListener = this;

        // 1
        _MyParticipantId = GPGSManager.GetInstance.GetMyParticipantId();

        // 2
        List<Participant> allPlayers = GPGSManager.GetInstance.GetAllPlayers();
        _opponentScripts = new Dictionary<string, MulEnemy>(allPlayers.Count - 1);

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
                    MyCharacter = GameObject.Find("Player");
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
                    EnemyCharacter = GameObject.Find("Enemy");
                    EnemyCharacter.transform.position = EnemyCharacterPos.transform.position;

                    MulEnemy opponentScript = EnemyCharacter.GetComponent<MulEnemy>();
                    _EnemyParticipantId = nextParticipantId;
                    _opponentScripts[nextParticipantId] = opponentScript;

                }
                else
                {
                    EnemyCharacter.transform.position = EnemyCharacterPos.transform.position;

                    MulEnemy opponentScript = EnemyCharacter.GetComponent<MulEnemy>();
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

        _multiplayerReady = true;

    }

    void StartNewGame()
    {
        AutoFade.LoadLevel("MultiPlayScene", 0.2f, 0.2f, Color.black);
    }

    // GPGSManager(서버)에서 받은 메시지를 매니저에게 줄때 사용한다.
    // [서버]->[클라이언트]
    #region GPGS_CallBack_Interface

    // 끝내기 메시지를 보내주는 녀석이다.
    public void FinishedReceived(string participantId, bool GameOver)
    {
        if (_multiplayerReady)
        {
            MulEnemy opponent = _opponentScripts[participantId];

            if (opponent != null)
            {
                opponent.SetEndGameInformation(GameOver);
            }

            ThisGameIsEnd = GameOver;
        }

        
    }

    // 업데이트를 해줄 정보들...
    public void UpdatePositionReceived(string participantId, int messageNum, float posX, float posY, float posZ, float rotY)
    {
        if (_multiplayerReady)
        {
            MulEnemy opponent = _opponentScripts[participantId];

            if (opponent != null)
            {
                opponent.SetTransformInformation(messageNum, posX, posY, posZ, rotY);
            }
        }

    }

    // 아이템들의 정보들을 업데이트 해준다.
    public void ItemStateReceived(int Index, bool GetItem)
    {
        if (_multiplayerReady)
        {
            ItemCount++;
            
        }
    }

    // 사격 상태를 업데이트 해준다.
    // x,y,z는 총알의 방향벡터를 의미
    public void ShootStateReceived(float x, float y, float z)
    {
        if (_multiplayerReady)
        {

        }
    }

    // 현재 데드아이 상태를 업데이트 해준다.
    // true면 데드 아이 발동을 의미
    public void DeadEyeStateReceived(bool DeadEyeOn)
    {
        if (_multiplayerReady)
        {

        }
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
        }

        if (_multiplayerReady)
        {

        }
    }

    // 게임이 끝날시 호출되는 리스너 함수.
    public void LeftRoomConfirmed()
    {
        GPGSManager.GetInstance.updateListener = null;

        //AutoFade.LoadLevel("MainTitle", 0.2f, 0.2f, Color.black);
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
            MulEnemy opponent = _opponentScripts[participantId];

            if (opponent != null)
            {
                opponent.GameOutInformation();
            }

            ThisGameIsEnd = true;
        }
    }



    #endregion GPGS_CallBack_Interface

    // 다른 스크립트에서 서버로 보낼 메시지를 호출하고 싶을때 보낼 콜 함수들...
    #region Call_Function

    // 아이템을 먹을시 이것을 GPGSManager의 아이템 메시지 함수를 호출해준다.
    // 사실상 이건 테스트 용이니 다른걸...
    public void CallSendItemStateMessage()
    {
        GPGSManager.GetInstance.SendItemStateMessage(0, true);
    }

    // 아이템을 먹을시 이것을 GPGSManager의 아이템 메시지 함수를 호출해준다.
    // Index = 메모리 풀을 만들었을때 호출될 아이템 인덱스 값
    // ItemGetOn = true일시 아이템을 먹었다고 판단하고 메시지를 전부 보내준다.
    public void CallSendItemStateMessage(int index, bool ItemGetOn)
    {
        GPGSManager.GetInstance.SendItemStateMessage(index, ItemGetOn);
    }

    // 자신의 위치를 서버에 전송한다.
    // Position X, Y, Z, Rotation Y값
    public void SendMyPositionUpdate()
    {
        // In a multiplayer game, time counts up!
        //_timePlayed += Time.deltaTime;
        //guiObject.SetTime(_timePlayed);

        // 0.16초마다 보냄으로써 네트워크 트래픽 최적화를 할 수 있다.
        if (Time.time > _nextBroadcastTime)
        {
            // We will be doing more here
            GPGSManager.GetInstance.SendMyPositionUpdate(MyCharacter.transform.position.x,
                                                    MyCharacter.transform.position.y,
                                                    MyCharacter.transform.position.z,
                                                    MyCharacter.transform.rotation.eulerAngles.y);

            _nextBroadcastTime = Time.time + 0.16f;
        }



    }

    // 게임이 끝났음을 서버에 전송한다.
    // bool 값을 전송
    // true일시 게임이 끝났다고 전송된다.
    public void SendEndGameMssage(bool GameEnd)
    {
        ThisGameIsEnd = GameEnd;

        GPGSManager.GetInstance.SendFinishMessage(ThisGameIsEnd);


    }

    #endregion Call_Function

    // Update is called once per frame
    void Update ()
    {
        if (_multiplayerReady)
        {
            if (GPGSManager.GetInstance.GetOtherNameGPGS(1) == _MyParticipantId)
            {
                PlayerName.text = GPGSManager.GetInstance.GetOtherNameGPGS(1);//_opponentScripts[_MyParticipantId].name;//GPGSManager.GetInstance.GetOtherNameGPGS(0);
                EnemyName.text = GPGSManager.GetInstance.GetOtherNameGPGS(0);
            }
            else
            {
                PlayerName.text = GPGSManager.GetInstance.GetOtherNameGPGS(0);//GPGSManager.GetInstance.GetOtherNameGPGS(0);
                EnemyName.text = GPGSManager.GetInstance.GetOtherNameGPGS(1);
            }


            //PlayerName.gameObject.transform.position = new Vector3(_opponentScripts[_MyParticipantId].transform.position.x, _opponentScripts[_MyParticipantId].transform.position.y + 0.4f, _opponentScripts[_MyParticipantId].transform.position.z);
            //EnemyName.gameObject.transform.position = new Vector3(_opponentScripts[_EnemyParticipantId].transform.position.x, _opponentScripts[_EnemyParticipantId].transform.position.y + 0.4f, _opponentScripts[_EnemyParticipantId].transform.position.z);
            //PlayerName.gameObject.transform.position = new Vector3(MyCharacter.transform.position.x, MyCharacter.transform.position.y + 0.4f, MyCharacter.transform.position.z);
            //EnemyName.gameObject.transform.position = new Vector3(EnemyCharacter.transform.position.x, EnemyCharacter.transform.position.y + 0.4f, EnemyCharacter.transform.position.z);

            MyInfoText.text = "Player Info : " + MyCharacterPos.transform.position;
            EnemyInfoText.text = "Enemy Info : " + EnemyCharacterPos.transform.position;
            NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();
            ItemGetCount.text = "ItemCount : " + ItemCount;

        }
        else
        {
            PlayerName.text = "Player"; //GPGSManager.GetInstance.GetNameGPGS();
            EnemyName.text = "Enemy";

            //PlayerName.gameObject.transform.position = new Vector3(MyCharacter.transform.position.x, MyCharacter.transform.position.y + 0.4f, MyCharacter.transform.position.z);
            //EnemyName.gameObject.transform.position = new Vector3(EnemyCharacter.transform.position.x, EnemyCharacter.transform.position.y + 0.4f, EnemyCharacter.transform.position.z);

            //EnemyInfoText.text = "Player Info : " + MyCharacter.transform.position.ToString();//("Player Info : " + _opponentScripts[_MyParticipantId].transform.position).ToString();//GPGSManager.GetInstance.GetAllPlayers()[0].ParticipantId;
            //NetText.text = "Enemy Info : " + EnemyCharacter.transform.position.ToString();//("Enemy Info : " + _opponentScripts[_EnemyParticipantId].transform.position).ToString();

            MyInfoText.text = "Player Info : " + MyCharacterPos;
            EnemyInfoText.text = "Enemy Info : " + EnemyCharacterPos;
            NetText.text = "Net Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();
            ItemGetCount.text = "ItemCount : " + ItemCount;
        }

        // 적의 타임아웃 체크
        if (Time.time > _nextTimeoutCheck)
        {
            CheckForTimeOuts();
            _nextTimeoutCheck = Time.time + _timeOutCheckInterval;
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
