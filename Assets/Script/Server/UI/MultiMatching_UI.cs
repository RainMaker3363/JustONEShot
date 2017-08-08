using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiMatching_UI : MonoBehaviour {

    public Text Matching_Text;
    

    private HY.MultiGameModeState MultiGameModeNumber;
    public MultiTitleManager TitleManager;

    private bool MultiLogChecker;
    private bool MultiStartChecker;

    // Use this for initialization
    void Start () {
        //Matching_Text.text = "Player Searching...";

        MultiStartChecker = false;
        MultiLogChecker = false;

        MultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();//MultiTitleManager.NowMultiGameModeNumber;


        if (TitleManager == null)
        {
            TitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
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
                    GPGSManager.GetInstance.ShowRoomUI();
                }
                break;
        }

        GPGSManager.GetInstance.SignInAndStartMPGame();

        //StartCoroutine(StartMultiGame());

    }

    private void OnEnable()
    {
        //Matching_Text.text = "Player Searching...";

        MultiStartChecker = false;
        MultiLogChecker = false;

        MultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();//MultiTitleManager.NowMultiGameModeNumber;


        if (TitleManager == null)
        {
            TitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
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
                    GPGSManager.GetInstance.ShowRoomUI();
                }
                break;
        }

        GPGSManager.GetInstance.SignInAndStartMPGame();
    }


    IEnumerator StartMultiGame()
    {
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
                    AutoFade.LoadLevel("GameScene", 0.1f, 0.1f, Color.black);
                }
                break;

            // HY.MultiGameModeState.SURVIVAL
            case HY.MultiGameModeState.SURVIVAL:
                {
                    //Debug.Log("Survival Game Mode Connect Test End");

                    AutoFade.LoadLevel("Survival Scene", 0.1f, 0.1f, Color.black);
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

                    if (GPGSManager.GetInstance.IsConnected() == true)
                    {
                        Matching_Text.text = "PVP Room Connect...";//\nCharNum : " + TitleManager.GetOpponentCharNumber();

                        if (MultiLogChecker == false)
                        {
                            MultiLogChecker = true;

                            Dictionary<string, int> PlayersInfo = GPGSManager.GetInstance.GetSurvivalOpponentCharNumbers();
                            IDictionaryEnumerator Iter = PlayersInfo.GetEnumerator();


                            while(Iter.MoveNext())
                            {
                                Debug.Log("Player ID : " + Iter.Key);
                                Debug.Log("Player Char Num : " + Iter.Value);
                            }

                            Debug.Log("Players Count : " + GPGSManager.GetInstance.GetAllPlayers().Count);
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
                    

                    if (GPGSManager.GetInstance.IsConnected() == true)
                    {
                        Matching_Text.text = "Survival Room Connect...";
                        
                        if(MultiLogChecker == false)
                        {
                            MultiLogChecker = true;

                            for (int i = 0; i < GPGSManager.GetInstance.GetAllPlayers().Count; i++)
                            {
                                Debug.Log("Player[" + i + "] Name" + " : " + GPGSManager.GetInstance.GetOtherNameGPGS(i));
                            }

                            Debug.Log("Players Count : " + GPGSManager.GetInstance.GetAllPlayers().Count);
                        }


                        if (TitleManager.GetSurvivalOpoonentCharNumbers() >= (GPGSManager.GetInstance.GetAllPlayers().Count - 1))
                        {


                            Matching_Text.text = "Join the Survival Session.";

                            if (MultiStartChecker == false)
                            {
                                Debug.Log("SurvivalOpponentCharNumber : " + TitleManager.GetSurvivalOpoonentCharNumbers());

                                for (int i = 0; i < GPGSManager.GetInstance.GetAllPlayers().Count; i++)
                                {
                                    Debug.Log("Player[" + i + "] Name" + " : " + GPGSManager.GetInstance.GetOtherNameGPGS(i));
                                }

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
