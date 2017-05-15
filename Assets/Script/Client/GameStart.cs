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


    private float WaitOverTime;
    private float WaitTime = 10;

    // Use this for initialization
    void Start ()
    {
        WaitOverTime = Time.time + WaitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CharMove.m_GunSelect && Input.GetMouseButtonDown(0))
        {
            if (GetClickedObject() != null)
            {
                switch (GetClickedObject().gameObject.tag)
                {
                    case "Revolver":
                        {
                            Selectanim.SetTrigger("Revolver");
                            Char.SelectGun_Revolver();
                            

                            //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                            break;
                        }
                    case "ShotGun":
                        {
                            Selectanim.SetTrigger("ShotGun");
                            Char.SelectGun_ShotGun();
                            //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                            break;
                        }
                    case "Musket":
                        {
                            Selectanim.SetTrigger("Musket");
                            Char.SelectGun_Musket();

                            //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                            break;
                        }


                    default:
                        break;
                }
                

            }
        }

        if (!CharMove.m_GunSelect && WaitOverTime < Time.time)
        {
            int SelectGun = 0;   //캐릭터 추가시 수정 현재는 리볼버
            switch (SelectGun)
            {
                case 0:
                    {
                        Selectanim.SetTrigger("Revolver");
                        Char.SelectGun_Revolver();

                        //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                        break;
                    }
                case 1:
                    {
                        Selectanim.SetTrigger("ShotGun");
                        Char.SelectGun_ShotGun();
                        //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                        break;
                    }
                case 2:
                    {
                        Selectanim.SetTrigger("Musket");
                        Char.SelectGun_Musket();

                        //CountDown.gameObject.SetActive(true);   //현재는 상대총을 받아오지않으므로 바로 카운트를 시작합니다.
                        break;
                    }
                default:
                    break;
            }
        }

        if (!CountDown.gameObject.activeSelf)
        {
            if(EnemyMove.m_SelectGun != 100 && CharMove.m_GunSelect)//둘다 총을 골랐을경우
            {
                if (!CountDownBool)
                {
                    Waiting.SetActive(false);
                    CountDownBool = true;
                    StartCoroutine(CountDownStart());
                }
            }
            else if(CharMove.m_GunSelect)   //플레이어는 총을 골랐지만 상대가 고르지 않은경우
            {
                if(!Waiting.activeSelf)
                {
                    if(Selectanim.GetBool("PlayEnd"))
                        Waiting.SetActive(true);
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
}
