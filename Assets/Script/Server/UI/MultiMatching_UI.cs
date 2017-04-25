using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiMatching_UI : MonoBehaviour {

    public Text Matching_Text;

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

        AutoFade.LoadLevel("GameScene", 0.1f, 0.1f, Color.black);
    }

    // Update is called once per frame
    void Update () {

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
