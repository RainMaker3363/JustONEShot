using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour {

    //GameObject UI_GameOver;
    GameObject GamePlayObj;
    TMPro.TextMeshProUGUI UI_Coin;
    TMPro.TextMeshProUGUI UI_Score;

    int PlayTime;

    LSD.GameMode m_GameMode;

    // Use this for initialization
    void Start () {
        string NowScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        switch (NowScene)
        {
            case "GameScene":
                {
                    m_GameMode = LSD.GameMode.PVP;
                    break;
                }
            case "Survival Scene":
                {
                    m_GameMode = LSD.GameMode.Survivel;
                    break;
                }

            case "ZombieScene":
                {
                    m_GameMode = LSD.GameMode.Zombie;
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
    }
	
	// Update is called once per frame
	void Update () {
		
        if(GameInfoManager.GetInstance().GameOver)
        {
            GameInfoManager.GetInstance().GameOver = false;

            switch (m_GameMode)
            {
                case LSD.GameMode.PVP:
                    {                        
                        if(CharMove.CharStat.HP>0)
                        {
                            UI_Coin.text = "100";
                            GameInfoManager.GetInstance().GoldAdd(100);
                            UI_Score.text = GameInfoManager.GetInstance().ScoreAdd(100).ToString();

                        }
                        else
                        {
                            UI_Coin.text = "10";
                            GameInfoManager.GetInstance().GoldAdd(10);
                            UI_Score.text = GameInfoManager.GetInstance().ScoreAdd(-100).ToString();
                        }
                        break;
                    }
                case LSD.GameMode.Survivel:
                    {
                        if (CharMove.CharStat.HP > 0)
                        {
                            UI_Coin.text = "100";
                            GameInfoManager.GetInstance().GoldAdd(100);
                            UI_Score.text = GameInfoManager.GetInstance().ScoreAdd(100).ToString();

                        }
                        else
                        {
                            UI_Coin.text = "10";
                            GameInfoManager.GetInstance().GoldAdd(10);
                            UI_Score.text = GameInfoManager.GetInstance().ScoreAdd(-100).ToString();
                        }
                        break;
                    }
                case LSD.GameMode.Zombie:
                    {
                        if (CharMove.CharStat.HP > 0)   //클리어했을때
                        {
                            int Gold = 0;

                            switch (GameInfoManager.GetInstance().ZombieLevel)
                            {
                                case 0:
                                    {
                                        Gold = 250;
                                        break;
                                    }
                                case 1:
                                    {
                                        Gold = 500;
                                        break;
                                    }
                                case 2:
                                    {
                                        Gold = 1000;
                                        break;
                                    }

                                default:
                                    break;
                            }

                            UI_Coin.text = Gold.ToString();
                            GameInfoManager.GetInstance().GoldAdd(Gold);

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
                        int Score = (Scoreadd * ZombieCreateManager.Stage) - PlayTime - GameInfoManager.GetInstance().Accumulated_Get(); ;
                        UI_Score.text = Score.ToString();

                        GameInfoManager.GetInstance().Accumulated_Reset();

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
