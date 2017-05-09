using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiMatching_UI : MonoBehaviour {

    public Text Matching_Text;

    private int MultiGameModeNumber;
    private bool MultiStartChecker;

    // Use this for initialization
    void Start () {
        Matching_Text.text = "Player Searching...";

        MultiStartChecker = false;

        GPGSManager.GetInstance.SignInAndStartMPGame();

        //StartCoroutine(StartMultiGame());

    }

    IEnumerator StartMultiGame()
    {
        yield return new WaitForSeconds(2.0f);

        //switch(MultiGameModeNumber)
        //{
        //    // HY.MultiGameModeState.NONE
        //    case 0:
        //        {

        //        }
        //        break;

        //    // HY.MultiGameModeState.PVP
        //    case 1:
        //        {
        //            AutoFade.LoadLevel("GameScene", 0.1f, 0.1f, Color.black);
        //        }
        //        break;

        //    // HY.MultiGameModeState.SURVIVAL
        //    case 2:
        //        {

        //        }
        //        break;
        //}

        AutoFade.LoadLevel("GameScene", 0.1f, 0.1f, Color.black);
    }

    // Update is called once per frame
    void Update () {

        MultiGameModeNumber = MultiTitleManager.MultiGameModeNumber;

        if (GPGSManager.GetInstance.IsConnected() == true)
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
