using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiMatching_UI : MonoBehaviour {

    public Text Matching_Text;

    private bool MultiStartChecker;

    // Use this for initialization
    void Start () {
        Matching_Text.text = "상대방을 찾는 중입니다...";

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
            Matching_Text.text = "잠시 후 게임이 시작됩니다.";

            if (MultiStartChecker == false)
            {
                MultiStartChecker = true;

                StartCoroutine(StartMultiGame());
            }

        }
    }
}
