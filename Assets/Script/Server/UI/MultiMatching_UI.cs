using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiMatching_UI : MonoBehaviour {

    public Text Matching_Text;
    

    private HY.MultiGameModeState MultiGameModeNumber;
    public MultiTitleManager TitleManager;

    private bool MultiStartChecker;

    // Use this for initialization
    void Start () {
        //Matching_Text.text = "Player Searching...";

        MultiStartChecker = false;

        MultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();//MultiTitleManager.NowMultiGameModeNumber;


        if (TitleManager == null)
        {
            TitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
        }

        Matching_Text.text = "Matching The Player...\nMode : " + MultiGameModeNumber.ToString();

        Debug.Log("MultiGameModeNumber : " + MultiGameModeNumber);

        GPGSManager.GetInstance.SignInAndStartMPGame();

        //StartCoroutine(StartMultiGame());

    }

    private void OnEnable()
    {
        //Matching_Text.text = "Player Searching...";

        MultiStartChecker = false;

        MultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();//MultiTitleManager.NowMultiGameModeNumber;


        if (TitleManager == null)
        {
            TitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
        }

        Matching_Text.text = "Matching The Player...\nMode : " + MultiGameModeNumber.ToString();

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
                    AutoFade.LoadLevel("GameScene", 0.1f, 0.1f, Color.black);
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
                        Matching_Text.text = "Room Connect...\nCharNum : " + TitleManager.GetOpponentCharNumber();

                        if (TitleManager.GetOpponentCharNumber() != 100)
                        {
                            Matching_Text.text = "Join the Session.\nCharNum : " + TitleManager.GetOpponentCharNumber();

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
                    GPGSManager.GetInstance.ShowRoomUI();

                    if (GPGSManager.GetInstance.IsConnected() == true)
                    {
                        Matching_Text.text = "Room Connect...\nCharCount : " + TitleManager.GetOpponentCharNumber()
                            + "\nCharName[0] = " + GPGSManager.GetInstance.GetOtherNameGPGS(0)
                            + "\nCharName[1] = " + GPGSManager.GetInstance.GetOtherNameGPGS(1)
                            + "\nCharName[2] = " + GPGSManager.GetInstance.GetOtherNameGPGS(2)
                            + "\nCharName[3] = " + GPGSManager.GetInstance.GetOtherNameGPGS(3)
                            + "\nCharName[4] = " + GPGSManager.GetInstance.GetOtherNameGPGS(4)
                            + "\nCharName[5] = " + GPGSManager.GetInstance.GetOtherNameGPGS(5)
                            + "\nCharName[6] = " + GPGSManager.GetInstance.GetOtherNameGPGS(6);

                        if (TitleManager.GetSurvivalOpoonentCharNumbers() >= 7)
                        {
                            Matching_Text.text = "Join the Session.\nCharCount : " + TitleManager.GetOpponentCharNumber()
                            + "\nCharName[0] = " + GPGSManager.GetInstance.GetOtherNameGPGS(0)
                            + "\nCharName[1] = " + GPGSManager.GetInstance.GetOtherNameGPGS(1)
                            + "\nCharName[2] = " + GPGSManager.GetInstance.GetOtherNameGPGS(2)
                            + "\nCharName[3] = " + GPGSManager.GetInstance.GetOtherNameGPGS(3)
                            + "\nCharName[4] = " + GPGSManager.GetInstance.GetOtherNameGPGS(4)
                            + "\nCharName[5] = " + GPGSManager.GetInstance.GetOtherNameGPGS(5)
                            + "\nCharName[6] = " + GPGSManager.GetInstance.GetOtherNameGPGS(6);

                            if (MultiStartChecker == false)
                            {
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
