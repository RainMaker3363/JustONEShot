using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MultiTitleGameSurvivalButton : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public Text NetText;
    public Text NetReadyText;

    public GameObject Survival_Matching_UI;

    private bool ButtonChecker;
    //private bool MultiStartChecker;

    // Use this for initialization
    void Start()
    {
        //GPGSManager.GetInstance.InitializeGPGS(); // 초기화

        //GPGSManager.GetInstance.LoginGPGS();

        ButtonChecker = false;

        Survival_Matching_UI.SetActive(false);

        //MultiStartChecker = false;
    }

    IEnumerator StartMultiGame()
    {
        yield return new WaitForSeconds(1.0f);

        Survival_Matching_UI.SetActive(true);

        //AutoFade.LoadLevel("GameScene", 0.2f, 0.2f, Color.black);
    }

    // Update is called once per frame
    void Update()
    {

        //if (GPGSManager.GetInstance.IsAuthenticated())
        //{
        //    NetText.text = "구글 계정이 연결 되었습니다.";
        //}
        //else
        //{
        //    NetText.text = "구글 계정이 아직 연결되지 않았습니다.";
        //}

        //if (GPGSManager.GetInstance.IsConnected() == true)
        //{


        //    NetReadyText.text = "잠시후 멀티 게임을 시작합니다.";

        //    if(MultiStartChecker == false)
        //    {
        //        MultiStartChecker = true;

        //        StartCoroutine(StartMultiGame());
        //    }

        //}
        //else
        //{


        //    NetReadyText.text = "아직 멀티 게임을 준비 중입니다...";
        //}
    }

    // 터치가 드래그(Drag) 했을때 호출 되는 함수
    public virtual void OnDrag(PointerEventData ped)
    {

    }


    // 터치를 하고 있을 대 발생하는 함수
    public virtual void OnPointerDown(PointerEventData ped)
    {
        if (GPGSManager.GetInstance.IsAuthenticated() == true)
        {
            //PVP_Matching_UI.SetActive(true);

            //if (ButtonChecker == false)
            //{
            //    ButtonChecker = true;

            //    GPGSManager.GetInstance.SignInAndStartMPGame();
            //}

            if (ButtonChecker == false)
            {
                ButtonChecker = true;

                // Survival 모드로 세팅해준다.
                GPGSManager.GetInstance.SetMultiGameModeState(2);

                Survival_Matching_UI.SetActive(true);


            }



            //AutoFade.LoadLevel("TestMultiScene", 0.2f, 0.2f, Color.black);
        }


    }

    // 터치에서 손을 땠을때 발생하는 함수
    public virtual void OnPointerUp(PointerEventData ped)
    {

    }
}
