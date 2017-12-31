using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
public class IAP_Button : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector]
    private bool ButtonChecker;

    [HideInInspector]
    private InAppPurchaser m_IAP_Ins;

    public GameObject IAP_Object;

    // Use this for initialization
    void Awake () {

        // VIP 유저가 아니라면..
        if (GameInfoManager.m_iVIPUser != 3434)
        {
            ButtonChecker = false;
        }

        

        if (m_IAP_Ins == null)
        {
            m_IAP_Ins = GameObject.FindGameObjectWithTag("IAP").GetComponent<InAppPurchaser>();
        }

        if(IAP_Object == null)
        {
            IAP_Object = GameObject.Find("CashInfo");
        }
    }

    private void OnEnable()
    {
        // VIP 유저가 아니라면..
        if (GameInfoManager.m_iVIPUser != 3434)
        {
            ButtonChecker = false;
        }

        

        if (m_IAP_Ins == null)
        {
            m_IAP_Ins = GameObject.FindGameObjectWithTag("IAP").GetComponent<InAppPurchaser>();
        }

        if (IAP_Object == null)
        {
            IAP_Object = GameObject.Find("CashInfo");
        }
    }

    // Update is called once per frame
    void Update () {

    }

    // 터치를 하고 있을 대 발생하는 함수
    public virtual void OnPointerDown(PointerEventData ped)
    {
        // 구글 ID가 로그인 됬을때만 반응한다.
        if (GPGSManager.GetInstance.IsAuthenticated() == true)
        {
            // VIP 유저가 아니라면..
            if(GameInfoManager.m_iVIPUser != 3434)
            {
                if(ButtonChecker == false)
                {
                    ButtonChecker = true;

                    if(m_IAP_Ins != null)
                    {
                        m_IAP_Ins.BuyProductID(InAppPurchaser.Pro_Upgrade_productId1);
                    }

                    if (IAP_Object != null)
                    {
                        IAP_Object.SetActive(false);
                    }
                }
            }

        }
        else
        {
            if (ButtonChecker == false)
            {
                ButtonChecker = true;

                if (IAP_Object != null)
                {
                    IAP_Object.SetActive(false);
                }
            }
        }


    }

    public void ReturnButtonProtocol()
    {
        if (ButtonChecker == false)
        {
            ButtonChecker = true;

            if (IAP_Object != null)
            {
                IAP_Object.SetActive(false);
            }
        }
    }
}
