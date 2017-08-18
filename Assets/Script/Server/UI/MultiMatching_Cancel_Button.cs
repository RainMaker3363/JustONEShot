using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MultiMatching_Cancel_Button : MonoBehaviour, IPointerDownHandler
{
    public MultiTitleManager _MultiTitleManager;

    public GameObject MultiMatching_UI_Obj;
    public GameObject MultiMatching_UI_Ready_Obj;
    public GameObject MultiMatching_UI_Main_Obj;

    private bool ButtonChecker;
    private bool Cancel_Button_On;

    // Use this for initialization
    void Start () {
        ButtonChecker = false;

        if(_MultiTitleManager == null)
        {
            _MultiTitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
        }
        
    }

    private void OnEnable()
    {
        ButtonChecker = false;

        if (_MultiTitleManager == null)
        {
            _MultiTitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
        }
    }

    public void Initialize()
    {
        ButtonChecker = false;

        if (_MultiTitleManager == null)
        {
            _MultiTitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
        }
    }

    private void Update()
    {
        if(GPGSManager.GetInstance.IsMatching())
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        
    }

    // 멀티를 취소할때 사용하는 함수
    public void MultiMatching_Cancel()
    {
        if (ButtonChecker == false)
        {
            ButtonChecker = true;

            MultiMatching_UI_Main_Obj.SetActive(true);
            MultiMatching_UI_Ready_Obj.SetActive(false);
            MultiMatching_UI_Obj.SetActive(false);

            // Survival 모드로 세팅해준다.
            GPGSManager.GetInstance.SetMultiGameModeState(HY.MultiGameModeState.NONE);

            // 방을 나간다.
            GPGSManager.GetInstance.OnRoomConnected(false);
        }
    }

    // 터치를 하고 있을 대 발생하는 함수
    public virtual void OnPointerDown(PointerEventData ped)
    {

        if (GPGSManager.GetInstance.IsAuthenticated() == true)
        {
            if(GPGSManager.GetInstance.IsMatching() == false)
            {
                if (ButtonChecker == false)
                {
                    ButtonChecker = true;

                    MultiMatching_UI_Main_Obj.SetActive(true);
                    MultiMatching_UI_Ready_Obj.SetActive(false);
                    MultiMatching_UI_Obj.SetActive(false);

                    // NONE 모드로 세팅해준다.
                    //GPGSManager.GetInstance.SetMultiGameModeState(HY.MultiGameModeState.NONE);

                    // 방을 나간다.
                    GPGSManager.GetInstance.LeaveRoom();

                    // 초기화를 다시 시켜준다.
                    _MultiTitleManager.MultiMatching_Cancel_Initilize();
                }
            }

        }


    }
}
