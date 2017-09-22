﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiMatching_UI : MonoBehaviour {

    public Text Matching_Text;
    

    private HY.MultiGameModeState MultiGameModeNumber;
    public MultiTitleManager TitleManager;
    public MultiMatching_Cancel_Button MultiMatching_Cancel_Button_obj;

    private bool MultiLogChecker;
    private bool MultiStartChecker;
    private bool MultiSeedSenderChecker;

    // 매칭시 타임아웃을 체크한다.
    private float TimeOutChecker;

    // Use this for initialization
    void Start () {
        //Matching_Text.text = "Player Searching...";

        MultiStartChecker = false;
        MultiLogChecker = false;
        MultiSeedSenderChecker = false;

        // 매칭 시 타임아웃을 체크할 시간
        TimeOutChecker = 60.0f;

        MultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();//MultiTitleManager.NowMultiGameModeNumber;

        if (MultiMatching_Cancel_Button_obj != null)
        {
            MultiMatching_Cancel_Button_obj.Initialize();
        }
        else
        {
            MultiMatching_Cancel_Button_obj = GameObject.Find("Cancel_Button").GetComponent<MultiMatching_Cancel_Button>();
            MultiMatching_Cancel_Button_obj.Initialize();
        }

        if (TitleManager == null)
        {
            TitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();

            //// 맵, 데드아이 위치에 대한 난수값을 던진다.
            //TitleManager.SendSelectedMapSeed();
            //TitleManager.SendPVPDeadEyeSeed();
        }
        else
        {
            //// 맵, 데드아이 위치에 대한 난수값을 던진다.
            //TitleManager.SendSelectedMapSeed();
            //TitleManager.SendPVPDeadEyeSeed();
        }


        Matching_Text.text = "Matching The Player...\nMode : " + MultiGameModeNumber.ToString();

        Debug.Log("MultiGameModeNumber : " + MultiGameModeNumber);

        switch(MultiGameModeNumber)
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
                    //GPGSManager.GetInstance.ShowRoomUI();
                }
                break;
        }

        GPGSManager.GetInstance.SignInAndStartMPGame();

        //StartCoroutine(StartMultiGame());
        //Debug.Log("MultiMatching_UI Start");
    }

    private void OnEnable()
    {
        //Matching_Text.text = "Player Searching...";

        //MultiStartChecker = false;
        //MultiLogChecker = false;

        //// 매칭 시 타임아웃을 체크할 시간
        //TimeOutChecker = 60.0f;

        //MultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();//MultiTitleManager.NowMultiGameModeNumber;

        //if(MultiMatching_Cancel_Button_obj != null)
        //{
        //    MultiMatching_Cancel_Button_obj.Initialize();
        //}
        //else
        //{
        //    MultiMatching_Cancel_Button_obj = GameObject.Find("Cancel_Button").GetComponent<MultiMatching_Cancel_Button>();
        //    MultiMatching_Cancel_Button_obj.Initialize();
        //}

        //if (TitleManager == null)
        //{
        //    TitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
        //}

        //Matching_Text.text = "Matching The Player...\nMode : " + MultiGameModeNumber.ToString();


        //Debug.Log("MultiGameModeNumber : " + MultiGameModeNumber);


        //switch (MultiGameModeNumber)
        //{
        //    case HY.MultiGameModeState.NONE:
        //        {

        //        }
        //        break;

        //    case HY.MultiGameModeState.PVP:
        //        {

        //        }
        //        break;

        //    case HY.MultiGameModeState.SURVIVAL:
        //        {
        //            //GPGSManager.GetInstance.ShowRoomUI();
        //        }
        //        break;
        //}

        //GPGSManager.GetInstance.SignInAndStartMPGame();

        //Debug.Log("MultiMatching_UI OnEnable");
    }

    public void StartMatchingRestart()
    {
        MultiStartChecker = false;
        MultiLogChecker = false;
        MultiSeedSenderChecker = false;

        // 매칭 시 타임아웃을 체크할 시간
        TimeOutChecker = 60.0f;

        MultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();//MultiTitleManager.NowMultiGameModeNumber;

        if (MultiMatching_Cancel_Button_obj != null)
        {
            MultiMatching_Cancel_Button_obj.Initialize();
        }
        else
        {
            MultiMatching_Cancel_Button_obj = GameObject.Find("Cancel_Button").GetComponent<MultiMatching_Cancel_Button>();
            MultiMatching_Cancel_Button_obj.Initialize();
        }

        if (TitleManager == null)
        {
            TitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();

            //// 맵, 데드아이 위치에 대한 난수값을 던진다.
            //TitleManager.SendSelectedMapSeed();
            //TitleManager.SendPVPDeadEyeSeed();
        }
        else
        {
            //// 맵, 데드아이 위치에 대한 난수값을 던진다.
            //TitleManager.SendSelectedMapSeed();
            //TitleManager.SendPVPDeadEyeSeed();
        }

        Matching_Text.text = "Matching The Player...\nMode : " + MultiGameModeNumber.ToString();

        Debug.Log("MultiGameModeNumber : " + MultiGameModeNumber);

        switch (MultiGameModeNumber)
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
                    //GPGSManager.GetInstance.ShowRoomUI();
                }
                break;
        }

        GPGSManager.GetInstance.SignInAndStartMPGame();
    }


    IEnumerator StartMultiGame()
    {
        //티켓사용
        WaitRoom.TicketUse();

        yield return new WaitForSeconds(0.2f);

        switch (MultiGameModeNumber)
        {
            // HY.MultiGameModeState.NONE
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            // HY.MultiGameModeState.PVP
            case HY.MultiGameModeState.PVP:
                {
                    if(TitleManager != null)
                    {
                        int Map = TitleManager.GetSelectedMapSeed();

                        switch (Map)
                        {
                            case 0:
                                {
                                    AutoFade.LoadLevel("GameScene0", 0.1f, 0.1f, Color.black);
                                }
                                break;

                            case 1:
                                {
                                    AutoFade.LoadLevel("GameScene1", 0.1f, 0.1f, Color.black);
                                }
                                break;
                        }
                    }

                }
                break;

            // HY.MultiGameModeState.SURVIVAL
            case HY.MultiGameModeState.SURVIVAL:
                {
                    if (TitleManager != null)
                    {
                        int Map = TitleManager.GetSelectedMapSeed();

                        switch (Map)
                        {
                            case 0:
                                {
                                    AutoFade.LoadLevel("SurvivalScene0", 0.1f, 0.1f, Color.black);
                                }
                                break;

                            case 1:
                                {
                                    AutoFade.LoadLevel("SurvivalScene1", 0.1f, 0.1f, Color.black);
                                }
                                break;
                        }
                    }
                    //Debug.Log("Survival Game Mode Connect Test End");


                }
                break;
        }


    }


    // Update is called once per frame
    void Update () {



        //MultiGameModeNumber = MultiTitleManager.NowMultiGameModeNumber;
        MultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();

        switch(MultiGameModeNumber)
        {
            case HY.MultiGameModeState.NONE:
                {

                }
                break;

            case HY.MultiGameModeState.PVP:
                {
                    //if(TimeOutChecker > 0.0f)
                    //{
                    //    TimeOutChecker -= Time.deltaTime;
                    //}
                    //else
                    //{
                    //    if(MultiMatching_Cancel_Button_obj != null)
                    //    {
                    //        MultiMatching_Cancel_Button_obj.MultiMatching_Cancel();
                    //    }
                    //}

                    if (GPGSManager.GetInstance.IsConnected() == true)
                    {
                        Matching_Text.text = "PVP Room Connect...";//\nCharNum : " + TitleManager.GetOpponentCharNumber();

                        if (MultiLogChecker == false)
                        {
                            MultiLogChecker = true;

                            //Dictionary<string, int> PlayersInfo = GPGSManager.GetInstance.GetSurvivalOpponentCharNumbers();
                            //IDictionaryEnumerator Iter = PlayersInfo.GetEnumerator();


                            //while(Iter.MoveNext())
                            //{
                            //    Debug.Log("Player ID : " + Iter.Key);
                            //    Debug.Log("Player Char Num : " + Iter.Value);
                            //}

                            Debug.Log("Players Count : " + GPGSManager.GetInstance.GetAllPlayers().Count);
                        }
                        
                        if(MultiSeedSenderChecker == false)
                        {
                            MultiSeedSenderChecker = true;

                            // 맵, 데드아이 위치에 대한 난수값을 던진다.
                            TitleManager.SendSelectedMapSeed();
                            TitleManager.SendPVPDeadEyeSeed();
                        }

                        if (TitleManager.GetOpponentCharNumber() != 100)
                        {
                            Matching_Text.text = "Join the PVP Session.";//\nCharNum : " + TitleManager.GetOpponentCharNumber();

                            if (MultiStartChecker == false)
                            {
                                MultiStartChecker = true;



                                StartCoroutine(StartMultiGame());
                            }
                        }


                    }
                }
                break;

            case HY.MultiGameModeState.SURVIVAL:
                {

                    //if (TimeOutChecker > 0.0f)
                    //{
                    //    TimeOutChecker -= Time.deltaTime;
                    //}
                    //else
                    //{
                    //    if (MultiMatching_Cancel_Button_obj != null)
                    //    {
                    //        MultiMatching_Cancel_Button_obj.MultiMatching_Cancel();
                    //    }
                    //}

                    if (GPGSManager.GetInstance.IsConnected() == true)
                    {
                        Matching_Text.text = "Survival Room Connect...";
                        
                        if(MultiLogChecker == false)
                        {
                            MultiLogChecker = true;

                            for (int i = 0; i < GPGSManager.GetInstance.GetAllPlayers().Count; i++)
                            {
                                Debug.Log("Survival Ready Player[" + i + "] Name" + " : " + GPGSManager.GetInstance.GetOtherNameGPGS(i));
                            }

                            Debug.Log("Players Count : " + GPGSManager.GetInstance.GetAllPlayers().Count);
                        }

                        if (MultiSeedSenderChecker == false)
                        {
                            MultiSeedSenderChecker = true;

                            // 맵, 데드아이 위치에 대한 난수값을 던진다.
                            TitleManager.SendSelectedMapSeed();
                            TitleManager.SendPVPDeadEyeSeed();
                        }

                        if (TitleManager.GetSurvivalOpoonentCharNumbers() >= (GPGSManager.GetInstance.GetAllPlayers().Count - 1))
                        {


                            Matching_Text.text = "Join the Survival Session.";

                            if (MultiStartChecker == false)
                            {
                                Debug.Log("SurvivalOpponentCharNumber : " + TitleManager.GetSurvivalOpoonentCharNumbers());

                                Dictionary<string, int> Diction = TitleManager.GetSurvivalOpponentCharNumber();
                                IDictionaryEnumerator Iter = Diction.GetEnumerator();

                                int Counter = 0;

                                while(Iter.MoveNext())
                                {
                                    if (Diction.ContainsKey(Iter.Key.ToString()))
                                    {
                                        Debug.Log("Session Player[" + Counter + "] ID : " + Iter.Key + " Char : " + Iter.Value);
                                        Counter++;
                                    }

                                    
                                }
                                

                                //for (int i = 0; i < GPGSManager.GetInstance.GetAllPlayers().Count; i++)
                                //{
                                //    Debug.Log("Session Player[" + i + "] Name : " GPGSManager.GetInstance.GetOtherNameGPGS(i) + " Char : ");
                                //}

                                MultiStartChecker = true;

                                StartCoroutine(StartMultiGame());
                            }
                        }


                    }
                }
                break;
        }
       
    }
}
