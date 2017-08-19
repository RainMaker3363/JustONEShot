using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalGameStart : MonoBehaviour
{

    public Animator Selectanim;
    public Camera cam;
    public GameObject MainUI;
    public CharMove Char;
    [SerializeField]
    public EnemyMove[] Enemy;
    public MultiGameManager Mul_Manager;
    public Animator CountDown;
    bool CountDownBool = false;
    public GameObject Waiting;
    public GameObject Select;

    public GameObject GamePlayObj;

    private float WaitOverTime;
    private float WaitTime = 16;

    int EditorIndex = 4;

    public TMPro.TextMeshPro SelectCount;
    int SelectTime = 10;

    public GameObject GunInfo;

    void Awake()
    {
        
        WaitOverTime = Time.time + WaitTime;
    }

    // Use this for initialization
    void Start()
    {
        Char = GamePlayObj.transform.Find("PlayerCharacter").GetComponent<CharMove>();
#if UNITY_EDITOR

        for (int i = 0; i < EditorIndex; i++)
        {
            if (Enemy[i] == null)
            {
                Enemy[i] = GamePlayObj.transform.Find("EnemyCharacter" + i).GetComponent<EnemyMove>();
            }
        }
#else
        for (int i = 0; i < GPGSManager.GetInstance.GetAllPlayers().Count-1; i++)
        {
            if (Enemy[i] == null)
            {
                Enemy[i] = GamePlayObj.transform.Find("EnemyCharacter" + i).GetComponent<EnemyMove>();
            }
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update" + CharMove.m_GunSelect);
        if (Char == null)
        {
            Char = GamePlayObj.transform.Find("PlayerCharacter").GetComponent<CharMove>();
        }

#if UNITY_EDITOR

        for (int i = 0; i < EditorIndex; i++)
        {
            if (Enemy[i] == null)
            {
                Enemy[i] = GamePlayObj.transform.Find("EnemyCharacter" + i).GetComponent<EnemyMove>();
            }
        }
#else
        for (int i = 0; i < GPGSManager.GetInstance.GetAllPlayers().Count - 1; i++)
        {
            if (Enemy[i] == null)
            {
                Enemy[i] = GamePlayObj.transform.Find("EnemyCharacter" + i).GetComponent<EnemyMove>();
            }
        }
#endif
        if (Char != null&& Enemy != null)
        {

            if (!CharMove.m_GunSelect)
            {
                if (WaitOverTime - SelectTime < Time.time)
                {
                    SelectTime--;
                    SelectCount.text = SelectTime.ToString();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (GetClickedObject() != null)
                    {
                        SelectCount.gameObject.SetActive(false);
                        GunInfo.SetActive(false);
                        switch (GetClickedObject().gameObject.tag)
                        {
                            case "Revolver":
                                {
                                    Selectanim.SetTrigger("Revolver");
                                    Char.SelectGun_Revolver();
                                    if (GPGSManager.GetInstance.IsAuthenticated())
                                    {
                                        Mul_Manager.SendWeaponNumberMessage(0);
                                        Mul_Manager.SendMultiSelectStateMessage(true);
                                        Mul_Manager.SendMultiWaitStateMessage(true);
                                    }

                                    Select.SetActive(false);

                                    //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                                    break;
                                }
                            case "ShotGun":
                                {
                                    Selectanim.SetTrigger("ShotGun");
                                    Char.SelectGun_ShotGun();
                                    if (GPGSManager.GetInstance.IsAuthenticated())
                                    {
                                        Mul_Manager.SendWeaponNumberMessage(1);
                                        Mul_Manager.SendMultiSelectStateMessage(true);
                                        Mul_Manager.SendMultiWaitStateMessage(true);
                                    }
                                    Select.SetActive(false);
                                    //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                                    break;
                                }
                            case "Musket":
                                {
                                    Selectanim.SetTrigger("Musket");
                                    Char.SelectGun_Musket();
                                    if (GPGSManager.GetInstance.IsAuthenticated())
                                    {
                                        Mul_Manager.SendWeaponNumberMessage(2);
                                        Mul_Manager.SendMultiSelectStateMessage(true);
                                        Mul_Manager.SendMultiWaitStateMessage(true);
                                    }
                                    Select.SetActive(false);
                                    //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                                    break;
                                }


                            default:
                                break;
                        }


                    }
                }

            }

            if (!CharMove.m_GunSelect && WaitOverTime < Time.time)
            {
                int SelectGun = 0;   //캐릭터 추가시 수정 현재는 리볼버
                SelectCount.gameObject.SetActive(false);
                GunInfo.SetActive(false);
                switch (SelectGun)
                {
                    case 0:
                        {
                            Selectanim.SetTrigger("Revolver");
                            Char.SelectGun_Revolver();
                            if (GPGSManager.GetInstance.IsAuthenticated())
                            {
                                Mul_Manager.SendWeaponNumberMessage(0);
                                Mul_Manager.SendMultiSelectStateMessage(true);
                                Mul_Manager.SendMultiWaitStateMessage(true);
                            }
                            Select.SetActive(false);
                            //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                            break;
                        }
                    case 1:
                        {
                            Selectanim.SetTrigger("ShotGun");
                            Char.SelectGun_ShotGun();
                            if (GPGSManager.GetInstance.IsAuthenticated())
                            {
                                Mul_Manager.SendWeaponNumberMessage(1);
                                Mul_Manager.SendMultiSelectStateMessage(true);
                                Mul_Manager.SendMultiWaitStateMessage(true);
                            }
                            Select.SetActive(false);
                            //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                            break;
                        }
                    case 2:
                        {
                            Selectanim.SetTrigger("Musket");
                            Char.SelectGun_Musket();
                            if (GPGSManager.GetInstance.IsAuthenticated())
                            {
                                Mul_Manager.SendWeaponNumberMessage(2);
                                Mul_Manager.SendMultiSelectStateMessage(true);
                                Mul_Manager.SendMultiWaitStateMessage(true);
                            }
                            Select.SetActive(false);
                            //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                            break;
                        }
                    default:
                        break;
                }
            }

            if (!CountDown.gameObject.activeSelf)
            {
                if (CharMove.m_GunSelect && !CountDownBool)
                {
                    if (GPGSManager.GetInstance.IsAuthenticated())
                    {
                        if (Mul_Manager.GetSurvivalOpponentWaitSignals_Ready()) //전부 총을 골랐을경우
                        {
                            int MyIDNumber = Mul_Manager.GetMyPlayerIDNumber();
                            int MyIDCheck = 0;  //i 가 ,PlayerIDNumber와 같을경우 보정
                            for (int i = 0; i < Mul_Manager.GetSurvivalPlayers_Count(); i++)
                            {
                                int GunNumber = 0;
                                GunNumber = Mul_Manager.GetSurvivalOpponentWeaponNumber(i);

                                if (i == MyIDNumber)//i가 내IDNumber와 같을경우
                                {
                                    //Enemy[i].gameObject.SetActive(false);
                                    MyIDCheck++;
                                }
                                else
                                {
                                    // 무기 번호를 받음(적들)
                                    int CharNumber = Mul_Manager.GetSurvivalOpponentCharacterNumber(i);
                                    Enemy[i - MyIDCheck].SetSurvivalEnemyGun(CharNumber, GunNumber);
                                    Enemy[i - MyIDCheck].PlayerNumber = i;
                                }
                            }


                            if (!CountDownBool)
                            {
                                Waiting.SetActive(false);
                                CountDownBool = true;
                                StartCoroutine(CountDownStart());
                            }
                        }
                        else if (!Waiting.activeSelf)   //플레이어는 총을 골랐지만 상대가 고르지 않은경우
                        {
                            if (Selectanim.GetBool("PlayEnd"))
                            {
                                Waiting.SetActive(true);                                
                            }
                        }
                    }
                }                
            }

            if (CountDown.gameObject.activeSelf)
            {
                if (!Char.gameObject.activeSelf)
                    Char.gameObject.SetActive(true);

                if (CountDown.GetBool("PlayEnd"))
                {
                    MainUI.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private GameObject GetClickedObject()
    {
        RaycastHit hit;
        GameObject target = null;


        Ray ray = cam.ScreenPointToRay(Input.mousePosition); //마우스 포인트 근처 좌표를 만든다. 


        if (true == (Physics.Raycast(ray.origin, ray.direction * 10, out hit)))   //마우스 근처에 오브젝트가 있는지 확인
        {
            //있으면 오브젝트를 저장한다.
            target = hit.collider.gameObject;
        }

        return target;
    }

    IEnumerator CountDownStart()
    {
        yield return new WaitForSeconds(5);
        CountDown.gameObject.SetActive(true);
        yield return null;
    }

    //void OnGUI()
    //{
    //     int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(w / 2, h / 2, 100, 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = 30;
    //    style.normal.textColor = new Color(0.0f, 0.0f, 1.5f, 1.5f);

    //    //string text = string.Format("HP : {0}", HP);
    //    string text = string.Format("EmemyName : {0}\nPlayerName : {1}\nMultyReady : {2}", Mul_Manager.EnemyCharacter.transform.name, Mul_Manager.MyCharacter.transform.name, Mul_Manager._multiplayerReady);

    //    GUI.Label(rect, text, style);

    //}
}
