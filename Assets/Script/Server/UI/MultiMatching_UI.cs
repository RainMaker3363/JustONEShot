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
        Matching_Text.text = "Player Searching...";

        MultiStartChecker = false;

        MultiGameModeNumber = MultiTitleManager.NowMultiGameModeNumber;


        if(TitleManager == null)
        {
            TitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
        }

        GPGSManager.GetInstance.SignInAndStartMPGame();

        //StartCoroutine(StartMultiGame());

    }

    IEnumerator StartMultiGame()
    {
        yield return new WaitForSeconds(0.2f);

        //switch (MultiGameModeNumber)
        //{
        //    // HY.MultiGameModeState.NONE
        //    case HY.MultiGameModeState.NONE:
        //        {

        //        }
        //        break;

        //    // HY.MultiGameModeState.PVP
        //    case HY.MultiGameModeState.PVP:
        //        {
        //            AutoFade.LoadLevel("GameScene", 0.1f, 0.1f, Color.black);
        //        }
        //        break;

        //    // HY.MultiGameModeState.SURVIVAL
        //    case HY.MultiGameModeState.SURVIVAL:
        //        {

        //        }
        //        break;
        //}

        AutoFade.LoadLevel("GameScene", 0.1f, 0.1f, Color.black);
    }


    // Update is called once per frame
    void Update () {



        MultiGameModeNumber = MultiTitleManager.NowMultiGameModeNumber;

        if (GPGSManager.GetInstance.IsConnected() == true)
        {
            if(TitleManager.GetOpponentCharNumber() != 100)
            {
                Matching_Text.text = "Join the Session.";

                if (MultiStartChecker == false)
                {
                    MultiStartChecker = true;

                    StartCoroutine(StartMultiGame());
                }
            }


        }
    }
}
