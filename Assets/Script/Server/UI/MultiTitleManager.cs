using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MultiTitleManager : MonoBehaviour
{
    //public Text NetText;
    //public Text NetReadyText;

    //private bool ButtonChecker;
    //private bool MultiStartChecker;
    public static HY.MultiGameModeState NowMultiGameModeNumber;

    // Use this for initialization
    void Start () {

        GPGSManager.GetInstance.InitializeGPGS(); // 초기화

        GPGSManager.GetInstance.LoginGPGS();


        GPGSManager.GetInstance.SetMultiGameModeState(0);
        NowMultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();

        //ButtonChecker = false;
        //MultiStartChecker = false;
    }

    void Update()
    {
        NowMultiGameModeNumber = GPGSManager.GetInstance.GetMultiGameModeState();

        Debug.Log("MultiGameModeNumber : " + NowMultiGameModeNumber);
    }

    //IEnumerator StartMultiGame()
    //{
    //    yield return new WaitForSeconds(1.0f);

    //    AutoFade.LoadLevel("TestMultiScene", 0.2f, 0.2f, Color.black);
    //}

    // Update is called once per frame
    //void Update () {

    //    if (GPGSManager.GetInstance.IsAuthenticated())
    //    {
    //        NetText.text = "구글 계정이 연결 되었습니다.";
    //    }
    //    else
    //    {
    //        NetText.text = "구글 계정이 아직 연결되지 않았습니다.";
    //    }

    //    if (GPGSManager.GetInstance.IsConnected() == true)
    //    {


    //        NetReadyText.text = "잠시후 멀티 게임을 시작합니다.";

    //        if(MultiStartChecker == false)
    //        {
    //            MultiStartChecker = true;

    //            StartCoroutine(StartMultiGame());
    //        }
            
    //    }
    //    else
    //    {


    //        NetReadyText.text = "아직 멀티 게임을 준비 중입니다...";
    //    }
    //}

    // 터치가 드래그(Drag) 했을때 호출 되는 함수
    //public virtual void OnDrag(PointerEventData ped)
    //{

    //}


    // 터치를 하고 있을 대 발생하는 함수
    //public virtual void OnPointerDown(PointerEventData ped)
    //{
    //    if (GPGSManager.GetInstance.IsAuthenticated() == true)
    //    {
    //        if(ButtonChecker == false)
    //        {
    //            ButtonChecker = true;

    //            GPGSManager.GetInstance.SignInAndStartMPGame();
    //        }
            



    //        //AutoFade.LoadLevel("TestMultiScene", 0.2f, 0.2f, Color.black);
    //    }


    //}

    // 터치에서 손을 땠을때 발생하는 함수
    //public virtual void OnPointerUp(PointerEventData ped)
    //{

    //}
}
