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

        /* 
        * 유니티 엔진 사용 시 입력을 하지 않으면 모바일 장치의 화면이 어두워지다가 잠기게 되는데,
        * 그러면 플레이어는 잠김을 다시 풀어야 해서 불편합니다. 따라서 화면 잠금 방지 기능 추가는 필수적이고,
        * Screen.sleepTimeout를 아래처럼 설정하면 그걸 할 수 있습니다. 
        */
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 지정해 주면 고정비로 빌드가 되어 단말에서 지정 해상도로 출력이 된다.	
        //Screen.SetResolution(1280, 720, true); // 1280 x 720 으로 조정

        //Screen.SetResolution(1920, 1080, true); // 1920 x 1080 으로 조정

        //Screen.SetResolution(Screen.width, (Screen.width / 2) * 3 ); // 2:3 비율로 개발시

        //Screen.SetResolution(Screen.width, Screen.width * 16 / 9,  true); // 16:9 로 개발시
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
