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

    // 플레이어의 정보
    public GameObject MyCharacter;
    public GameObject MyCharacterPos;

    // 적의 정보
    public GameObject EnemyCharacter;
    public GameObject EnemyCharacterPos;
    private Dictionary<string, MulEnemy> _opponentScripts;

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
    public float timeOutThreshold = 5.0f;
    private float _timeOutCheckInterval = 1.0f;
    private float _nextTimeoutCheck = 0.0f;


    // Use this for initialization
    void Awake () {
		
        // 임시로 만든 상태 값이므로 추후에 수정해주세요
        MultiState = HY.MultiGameState.START;
        PlayerState = HY.MultiPlayerState.LIVE;

        ThisGameIsEnd = false;

        // 네트워크 트래픽 최적화 변수 초기화
        _nextBroadcastTime = 0;

        // 타임 아웃 정보 초기화
        timeOutThreshold = 10.0f;
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
    public void UpdatePositionReceived(string participantId, float posX, float posY, float posZ, float rotY)
    {
        if (_multiplayerReady)
        {
            MulEnemy opponent = _opponentScripts[participantId];

            if (opponent != null)
            {
                opponent.SetTransformInformation(posX, posY, posZ, rotY);
            }
        }

    }

    // 게임이 끝날시 호출되는 리스너 함수.
    public void LeftRoomConfirmed()
    {
        GPGSManager.GetInstance.updateListener = null;

        //AutoFade.LoadLevel("MainTitle", 0.2f, 0.2f, Color.black);
        ThisGameIsEnd = true;
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

    //public void LeaveGame()
    //{
    //    GPGSManager.GetInstance.LeaveGame();
    //}


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

            MyInfoText.text = "Player Info : " + MyCharacterPos;
            EnemyInfoText.text = "Enemy Info : " + EnemyCharacterPos;
            NetText.text = "Enemy Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();


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
            NetText.text = "Enemy Info : " + GPGSManager.GetInstance.GetNetMessage().ToString();

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
