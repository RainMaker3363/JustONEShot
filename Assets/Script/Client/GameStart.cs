using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour {

    public Animator Selectanim;
    public Camera cam;
    public GameObject MainUI;
    public CharMove Char;
    public EnemyMove Enemy;
    public MultiGameManager Mul_Manager;
    public Animator CountDown;
    bool CountDownBool = false;
    public GameObject Waiting;
    public GameObject Select;

    public GameObject GamePlayObj;

    private float WaitOverTime;
    private float WaitTime = 15;

    public TMPro.TextMeshPro SelectCount;
    int SelectTime = 10;

    public GameObject GunInfo;


    void Awake()
    {
       
    }

    // Use this for initialization
    void Start ()
    {
        Char = GamePlayObj.transform.Find("PlayerCharacter").GetComponent<CharMove>();
        Enemy = GamePlayObj.transform.Find("EnemyCharacter").GetComponent<EnemyMove>();
        WaitOverTime = Time.time + WaitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Char == null)
        {
            Char = GamePlayObj.transform.Find("PlayerCharacter").GetComponent<CharMove>();
        }


        if (Enemy == null)
        {
            Enemy = GamePlayObj.transform.Find("EnemyCharacter").GetComponent<EnemyMove>();
        }

        if (Char != null && Enemy != null)
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
                if (EnemyMove.m_SelectGun != 100 && CharMove.m_GunSelect)//둘다 총을 골랐을경우
                {
                    if (!CountDownBool)
                    {
                        Waiting.SetActive(false);
                        CountDownBool = true;
                        StartCoroutine(CountDownStart());
                    }
                }
                else if (CharMove.m_GunSelect)   //플레이어는 총을 골랐지만 상대가 고르지 않은경우
                {
                    if (!Waiting.activeSelf)
                    {
                        if (Selectanim.GetBool("PlayEnd"))
                        {
                            Waiting.SetActive(true);
                        }
                    }

                    //Waiting.SetActive(true); //현재 애니메이션에서 켜줍니다.
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
                    DB_CreateManager.GetInstance().DeadEyeCreateTime();
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
