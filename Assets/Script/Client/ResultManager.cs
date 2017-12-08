using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour {

    //GameObject UI_GameOver;
    GameObject GamePlayObj;
    TMPro.TextMeshProUGUI UI_Coin;
    TMPro.TextMeshProUGUI UI_Score;

    int PlayTime = 0;

    LSD.GameMode m_GameMode;

    public GameObject ZombieSound;

    // Use this for initialization
    void Start () {
        string NowScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        switch (NowScene)
        {
            case "GameScene0":
                {
                    m_GameMode = LSD.GameMode.PVP;
                    break;
                }
            case "GameScene1":
                {
                    m_GameMode = LSD.GameMode.PVP;
                    break;
                }
            case "GameScene2":
                {
                    m_GameMode = LSD.GameMode.PVP;
                    break;
                }
            case "SurvivalScene0":
                {
                    m_GameMode = LSD.GameMode.Survivel;
                    break;
                }
            case "SurvivalScene1":
                {
                    m_GameMode = LSD.GameMode.Survivel;
                    break;
                }
            case "SurvivalScene2":
                {
                    m_GameMode = LSD.GameMode.Survivel;
                    break;
                }

            case "ZombieScene":
                {
                    m_GameMode = LSD.GameMode.Zombie;
                    break;
                }
            case "TutorialScene":
                {
                    m_GameMode = LSD.GameMode.Tutorial;
                    break;
                }
            default:
                break;
        }

        GamePlayObj = GameObject.Find("GamePlayObj");
        //UI_GameOver = GameObject.Find("GamePlayObj/UI_GameOver");
        UI_Coin = GamePlayObj.transform.Find("UI_GameOver/Image/Coin Text/Coin Text pro").GetComponent<TMPro.TextMeshProUGUI>();
        UI_Score = GamePlayObj.transform.Find("UI_GameOver/Image/Prize Text/PrizeText pro").GetComponent<TMPro.TextMeshProUGUI>();
        StartCoroutine(PlayTimeCheck());
        PlayTime = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
        if(GameInfoManager.GetInstance().GameOver)
        {
            GameInfoManager.GetInstance().GameOver = false;

            if(GameObject.Find("BGM")!=null)
                GameObject.Find("BGM").GetComponent<AudioSource>().mute = true;

            int Score;
            switch (m_GameMode)
            {
                case LSD.GameMode.PVP:
                    {                        
                        if(CharMove.CharStat.HP>0)
                        {
                            UI_Coin.text = "100";
                            GameInfoManager.GetInstance().GoldAdd(100);

                            Score = GameInfoManager.GetInstance().PVPScoreAdd(100);
                            

                        }
                        else
                        {
                            UI_Coin.text = "10";
                            GameInfoManager.GetInstance().GoldAdd(10);
                            Score = GameInfoManager.GetInstance().PVPScoreAdd(-100);
                        }

                        UI_Score.text = Score.ToString();
                        GPGSManager.GetInstance.Send_LeaderBoard_Score("leaderboard_pvp_1_vs_1", Score);
                        break;
                    }
                case LSD.GameMode.Survivel:
                    {
                        if (CharMove.CharStat.HP > 0)
                        {
                            UI_Coin.text = "100";
                            GameInfoManager.GetInstance().GoldAdd(100);
                            Score = GameInfoManager.GetInstance().SurvivalScoreAdd(100);

                        }
                        else
                        {
                            UI_Coin.text = "10";
                            GameInfoManager.GetInstance().GoldAdd(10);
                            Score = GameInfoManager.GetInstance().SurvivalScoreAdd(-100);
                        }

                        UI_Score.text = Score.ToString();
                        GPGSManager.GetInstance.Send_LeaderBoard_Score("leaderboard_pvp_survival_1_vs_m", Score);
                        break;
                    }
                case LSD.GameMode.Zombie:
                    {
                        if(ZombieSound != null)
                            ZombieSound.SetActive(false);

                        if (CharMove.CharStat.HP > 0)   //클리어했을때
                        {
                            int Gold = 0;
                            //int Mask = 1; // 00000001
                            int CharIndex = 0;
                            int select = 1; //보상 캐릭터가 열려있는지 체크
                            switch (GameInfoManager.GetInstance().ZombieLevel)
                            {
                                case 0:
                                    {
                                        Gold = 250;
                                        //select = Mask << 2;
                                        // GameInfoManager.CharLock += select;
                                        CharIndex = 2;
                                        break;
                                    }
                                case 1:
                                    {
                                        Gold = 500;
                                        //select = Mask << 3;
                                        CharIndex = 3;
                                        break;
                                    }
                                case 2:
                                    {
                                        Gold = 1000;
                                        //select = Mask << 1;
                                        CharIndex = 1;
                                        break;
                                    }

                                default:
                                    break;
                            }

                            UI_Coin.text = Gold.ToString();
                            GameInfoManager.GetInstance().GoldAdd(Gold);
                            int LockCheck = (GameInfoManager.LockCode[CharIndex] & select);

                            if (LockCheck > 0)
                            {

                            }
                            else //보상 캐릭이 잠겨있다면 해제
                            {
                                GameInfoManager.LockCode[CharIndex] += 1;   //00000001
                                PlayerPrefs.SetString("LockCode" + CharIndex.ToString(), System.Convert.ToString(GameInfoManager.LockCode[CharIndex], 2));
                            }

                        }
                        else //패배했을때
                        {
                            int Leveladd = 0;

                            switch (GameInfoManager.GetInstance().ZombieLevel)
                            {
                                case 0:
                                    {
                                        Leveladd = 5;                                           
                                        break;
                                    }
                                case 1:
                                    {
                                        Leveladd = 10;
                                        break;
                                    }
                                case 2:
                                    {
                                        Leveladd = 20;
                                        break;
                                    }

                                default:
                                    break;
                            }
                            int Gold = 0;

                            if (GameInfoManager.GetInstance().ZombieInfinityMode)    //무한모드일때
                            {
                                Gold = Leveladd * ZombieCreateManager.Stage; //스테이지 * 난이도 점수
                            }
                            //유한모드일때 골드 보상 없음(패배했으니..)

                            UI_Coin.text = Gold.ToString();
                            GameInfoManager.GetInstance().GoldAdd(Gold);
                        }
                        int Scoreadd = 0;

                        switch (GameInfoManager.GetInstance().ZombieLevel)
                        {
                            case 0:
                                {
                                    Scoreadd = 1000;
                                    break;
                                }
                            case 1:
                                {
                                    Scoreadd = 3000;
                                    break;
                                }
                            case 2:
                                {
                                    Scoreadd = 5000;
                                    break;
                                }

                            default:
                                break;
                        }
                        Score = (Scoreadd * ZombieCreateManager.Stage) - PlayTime - GameInfoManager.GetInstance().Accumulated_Get();
                        UI_Score.text = Score.ToString();

                        switch (GameInfoManager.GetInstance().ZombieLevel)
                        {
                            case 0:
                                {
                                   if(GameInfoManager.GetInstance().ZombieInfinityMode)
                                    {
                                        GPGSManager.GetInstance.Send_LeaderBoard_Score("leaderboard_zombie_survival_easy", Score);
                                    }
                                    else
                                    {
                                        GPGSManager.GetInstance.Send_LeaderBoard_Score("leaderboard_zombie_survival_endless_easy", Score);
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (GameInfoManager.GetInstance().ZombieInfinityMode)
                                    {
                                        GPGSManager.GetInstance.Send_LeaderBoard_Score("leaderboard_zombie_survival_normal", Score);
                                    }
                                    else
                                    {
                                        GPGSManager.GetInstance.Send_LeaderBoard_Score("leaderboard_zombie_survival_endless_normal", Score);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (GameInfoManager.GetInstance().ZombieInfinityMode)
                                    {
                                        GPGSManager.GetInstance.Send_LeaderBoard_Score("leaderboard_zombie_survival_hard", Score);
                                    }
                                    else
                                    {
                                        GPGSManager.GetInstance.Send_LeaderBoard_Score("leaderboard_zombie_survival_endless_hard", Score);
                                    }
                                    break;
                                }

                            default:
                                break;
                        }

                        
                        GameInfoManager.GetInstance().Accumulated_Reset();

                        break;
                    }
                case LSD.GameMode.Tutorial:
                    {
                        UI_Coin.text = "0";
                        //GameInfoManager.GetInstance().GoldAdd(10);
                        Score = 0;

                        UI_Score.text = Score.ToString();
                        break;
                    }
                default:
                    break;
            }
        }

	}

    IEnumerator PlayTimeCheck()
    {
        while(true)
        {
            if(!GameInfoManager.GetInstance().Pause)
                PlayTime++;

            if (GameInfoManager.GetInstance().GameOver)
                break;

            yield return new WaitForSeconds(1);

        }
       
    }
}
