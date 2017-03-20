using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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
        IDLE,
        DASH_SLOW,
        DASH_SOFT,
        DASH_HARD,
        SHOT_READY,
        SHOT_FIRE,
        DAMAGE,
        DEADEYE,
        REROAD,
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

public class MultiGameManager : MonoBehaviour, MPUpdateListener {

    // 전체적으로 관리될 게임, 캐릭터 상태 값
    public static HY.MultiGameState MultiState;
    public static HY.MultiPlayerState PlayerState;

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

    public Text PlayerInfo;
    public Text EnemyInfo;
    public Text NetMessageInfo;
    public Text NetInfo1;
    public Text NetInfo2;

    // Use this for initialization
    void Awake () {
		
        // 임시로 만든 상태 값이므로 추후에 수정해주세요
        MultiState = HY.MultiGameState.START;
        PlayerState = HY.MultiPlayerState.IDLE;

        GPGSManager.GetInstance.TrySilentSignIn();
        GPGSManager.GetInstance.InitMessager();

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
                    MyCharacter = GameObject.Find("Lincoin_Body");
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
                    EnemyCharacter = GameObject.Find("EnemyLincoin_Body");
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

    public void UpdateTransReceived(string participantId, float posX, float posY, float posZ, float rotY)
    {
        if (_multiplayerReady)
        {
            MulEnemy opponent = _opponentScripts[participantId];

            if (opponent != null)
            {
                opponent.SetTransformInformation(posX, posY, posZ, rotY);
            }

            //EnemyCharacter.GetComponent<MulEnemy>().SetTransformInformation(posX, posY, velX, velY, rotZ);
        }


        //if (_multiplayerReady)
        //{
        //    MulEnemy opponent = _opponentScripts[senderId];

        //    if (opponent != null)
        //    {
        //        opponent.SetTransformInformation(posX, posY, velX, velY, rotZ);
        //    }


        //    //EnemyCharacter.GetComponent<MulEnemy>().SetTransformInformation(posX, posY, velX, velY, rotZ);
        //}

    }

    // Update is called once per frame
    void Update () {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                {
                    if (Input.GetKey(KeyCode.Escape))
                    {

                        // 할꺼 하셈

                        // Application.Quit();
                    }

                }
                break;

            default:
                {
                    if (Input.GetKey(KeyCode.Escape))
                    {

                        // 할꺼 하셈

                        // Application.Quit();

                    }
                }
                break;
        }

        if(_multiplayerReady == true)
        {
            PlayerInfo.text = "MyCharacter Pos : " + MyCharacter.transform.position;
            EnemyInfo.text = "Enemy Pos : " + EnemyCharacter.transform.position;
            NetMessageInfo.text = GPGSManager.GetInstance.GetReceiveMessage();
            NetInfo1.text = "My ID : " + GPGSManager.GetInstance.GetMyParticipantId();
            NetInfo2.text = "Enemy ID : " + _EnemyParticipantId.ToString();

            Pos_Multiplayer_Update();
        }
        else
        {

            PlayerInfo.text = "MultiPlayer is not Ready";
            EnemyInfo.text = "MultiPlayer is not Ready";
            NetMessageInfo.text = GPGSManager.GetInstance.GetReceiveMessage();
            NetInfo1.text = "My ID : " + GPGSManager.GetInstance.GetMyParticipantId();
            NetInfo2.text = "Enemy ID : " + _EnemyParticipantId.ToString();

        }

        switch (MultiState)
        {
            case HY.MultiGameState.START:
                {

                    switch (PlayerState)
                    {
                        case HY.MultiPlayerState.IDLE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_SLOW:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_SOFT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_HARD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOT_READY:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOT_FIRE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DAMAGE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYE:
                            {

                            }
                            break;
                            
                        case HY.MultiPlayerState.REROAD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEAD:
                            {

                            }
                            break;
                    }
                }
                break;
                
            case HY.MultiGameState.PLAY:
                {
                    switch (PlayerState)
                    {
                        case HY.MultiPlayerState.IDLE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_SLOW:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_SOFT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_HARD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOT_READY:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOT_FIRE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DAMAGE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.REROAD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEAD:
                            {

                            }
                            break;
                    }
                }
                break;

            case HY.MultiGameState.GAMEWIN:
                {
                    switch (PlayerState)
                    {
                        case HY.MultiPlayerState.IDLE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_SLOW:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_SOFT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_HARD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOT_READY:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOT_FIRE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DAMAGE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.REROAD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEAD:
                            {

                            }
                            break;
                    }
                }
                break;

            case HY.MultiGameState.GAMEOVER:
                {
                    switch (PlayerState)
                    {
                        case HY.MultiPlayerState.IDLE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_SLOW:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_SOFT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_HARD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOT_READY:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOT_FIRE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DAMAGE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.REROAD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEAD:
                            {

                            }
                            break;
                    }
                }
                break;

            case HY.MultiGameState.CONNECTOUT:
                {
                    switch (PlayerState)
                    {
                        case HY.MultiPlayerState.IDLE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_SLOW:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_SOFT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DASH_HARD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOT_READY:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOT_FIRE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DAMAGE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.REROAD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEAD:
                            {

                            }
                            break;
                    }
                }
                break;
        }
	}

    void StartNewGame()
    {
        AutoFade.LoadLevel("MultiPlayScene", 0.2f, 0.2f, Color.black);
    }


    void Pos_Multiplayer_Update()
    {
        // In a multiplayer game, time counts up!
        //_timePlayed += Time.deltaTime;
        //guiObject.SetTime(_timePlayed);


        // We will be doing more here
        GPGSManager.GetInstance.SendMyUpdate(MyCharacter.transform.position.x,
                                                MyCharacter.transform.position.y,
                                                MyCharacter.transform.position.z,
                                                MyCharacter.transform.rotation.eulerAngles.y);
    }
}
