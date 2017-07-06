using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine;


public class ZombieCreateManager : MonoBehaviour {

    public GameObject GameplayObj;
   [SerializeField]
    GameObject[] ZombieObj;

    [SerializeField]
    Transform[] ZombiePoint;

    GameObject Temp;
    public static int ZombieCount = 0;
    [SerializeField]
    int Stage=0;
    int Level = 0;
    int LevelDamage = 0;
    int LevelHP=0;

    bool StageInit;
    bool LevelUPStatSelect;

    public TMPro.TextMeshProUGUI UI_Stage;  //남은 좀비와 스테이지 현황표시
    public GameObject LevelUP_UI;
    public GameObject Main_UI;

    public DeathZone m_DeathZone;

    public AudioClip SelectSound;
    AudioSource m_AudioSource;

    // Use this for initialization
    void Start () {
        m_AudioSource = gameObject.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        UI_Stage.text = Stage.ToString() + " stage " + ZombieCount.ToString() + " zombie";

        if(LevelUP_UI.activeSelf)
        {
            if (CharMove.CharStat.HP >= CharMove.CharStat.MaxHP)    //max로 찍었으면 추후찍지못하게 합니다
            {
                LevelUP_UI.transform.Find("HP_Button").Find("BackWhite").gameObject.SetActive(false);
                LevelUP_UI.transform.Find("HP_Button").Find("state").gameObject.SetActive(false);
                LevelUP_UI.transform.Find("HP_Button").Find("Max").gameObject.SetActive(true);
                LevelUP_UI.transform.Find("HP_Button").GetComponent<Button>().enabled = false;
            }
            else
            {
                LevelUP_UI.transform.Find("HP_Button").Find("BackWhite").gameObject.SetActive(true);
                LevelUP_UI.transform.Find("HP_Button").Find("state").gameObject.SetActive(true);
                LevelUP_UI.transform.Find("HP_Button").Find("Max").gameObject.SetActive(false);
                LevelUP_UI.transform.Find("HP_Button").GetComponent<Button>().enabled = true;
                LevelUP_UI.transform.Find("HP_Button").Find("state").GetComponent<Image>().fillAmount = (float)CharMove.CharStat.HP / (float)CharMove.CharStat.MaxHP;
            }
        }
    }

    void OnEnable()
    {
        Stage = 0;
        ZombieCount = 0;
        Level = 0;
        StageInit = true;

        LevelUP_UI.transform.Find("Bullet_Button").GetComponent<Button>().onClick.AddListener(BulletUP);
        LevelUP_UI.transform.Find("BulletMax_Button").GetComponent<Button>().onClick.AddListener(BulletMaxUP);
        LevelUP_UI.transform.Find("HP_Button").GetComponent<Button>().onClick.AddListener(HPUP);
        LevelUP_UI.transform.Find("Damage_Button").GetComponent<Button>().onClick.AddListener(DamageUP);
        LevelUP_UI.transform.Find("Reload_Button").GetComponent<Button>().onClick.AddListener(ReloadUP);
        
        LevelUPStatSelect = true;

      
        StartCoroutine(StageSetup());
    }

    void BulletUP()
    {
        m_AudioSource.PlayOneShot(SelectSound);
        CharMove.m_UseGun.GetBulletQuantity++;
        if (CharMove.m_UseGun.GetBulletQuantity >= 6)    //max로 찍었으면 추후찍지못하게 합니다
        {
            LevelUP_UI.transform.Find("Bullet_Button").Find("BackWhite").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("Bullet_Button").Find("state").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("Bullet_Button").Find("Max").gameObject.SetActive(true);
            LevelUP_UI.transform.Find("Bullet_Button").GetComponent<Button>().enabled = false;
        }
        else
        {
            //LevelUP_UI.transform.Find("Bullet_Button").Find("BackWhite").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("Bullet_Button").Find("state").GetComponent<Image>().fillAmount = CharMove.m_UseGun.GetBulletQuantity / 6.0f;
        }

        LevelUP_UI.SetActive(false);
        Main_UI.SetActive(true);
        LevelUPStatSelect = true;
        Time.timeScale = 1;
    }
    void BulletMaxUP()
    {
        m_AudioSource.PlayOneShot(SelectSound);
        CharMove.m_UseGun.MaxBullet_Hand++;
        if (CharMove.m_UseGun.MaxBullet_Hand >= 20f)    //max로 찍었으면 추후찍지못하게 합니다
        {
            LevelUP_UI.transform.Find("BulletMax_Button").Find("BackWhite").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("BulletMax_Button").Find("state").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("BulletMax_Button").Find("Max").gameObject.SetActive(true);
            LevelUP_UI.transform.Find("BulletMax_Button").GetComponent<Button>().enabled = false;
        }
        else
        {
           // LevelUP_UI.transform.Find("BulletMax_Button").Find("BackWhite").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("BulletMax_Button").Find("state").GetComponent<Image>().fillAmount = (CharMove.m_UseGun.MaxBullet_Hand-10) / 20.0f;
        }

        LevelUP_UI.SetActive(false);
        Main_UI.SetActive(true);
        LevelUPStatSelect = true;
        Time.timeScale = 1;
    }
    void HPUP()
    {
        m_AudioSource.PlayOneShot(SelectSound);
        GameObject.Find("PlayerCharacter").GetComponent<CharMove>().HPRecovery(30);

        LevelUP_UI.SetActive(false);
        Main_UI.SetActive(true);
        LevelUPStatSelect = true;
        Time.timeScale = 1;
    }

    void DamageUP()
    {
        m_AudioSource.PlayOneShot(SelectSound);
        CharMove.m_UseGun.Damage += 5;
        CharMove.m_UseGun.DamageUpgrade++;
        if (CharMove.m_UseGun.DamageUpgrade >= 5)    //max로 찍었으면 추후찍지못하게 합니다
        {
            LevelUP_UI.transform.Find("Damage_Button").Find("BackWhite").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("Damage_Button").Find("state").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("Damage_Button").Find("Max").gameObject.SetActive(true);
            LevelUP_UI.transform.Find("Damage_Button").GetComponent<Button>().enabled = false;
        }
        else
        {
            //LevelUP_UI.transform.Find("Damage_Button").Find("BackWhite").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("Damage_Button").Find("state").GetComponent<Image>().fillAmount = CharMove.m_UseGun.DamageUpgrade / 5.0f;
        }

        LevelUP_UI.SetActive(false);
        Main_UI.SetActive(true);
        LevelUPStatSelect = true;
        Time.timeScale = 1;
    }
    void ReloadUP()
    {
        m_AudioSource.PlayOneShot(SelectSound);
        CharMove.m_UseGun.ReloadSpeed += 0.2f;

        if (CharMove.m_UseGun.ReloadSpeed >= 2.0f)    //max로 찍었으면 추후찍지못하게 합니다
        {
            LevelUP_UI.transform.Find("Reload_Button").Find("BackWhite").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("Reload_Button").Find("state").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("Reload_Button").Find("Max").gameObject.SetActive(true);
            LevelUP_UI.transform.Find("Reload_Button").GetComponent<Button>().enabled = false;
        }
        else
        {
            //LevelUP_UI.transform.Find("Reload_Button").Find("BackWhite").gameObject.SetActive(false);
            LevelUP_UI.transform.Find("Reload_Button").Find("state").GetComponent<Image>().fillAmount = (CharMove.m_UseGun.ReloadSpeed - 1);
        }

        LevelUP_UI.SetActive(false);
        Main_UI.SetActive(true);
        LevelUPStatSelect = true;
        Time.timeScale = 1;
        
    }
    void ZombieCreate()//0 일반좀비
    {
        for (int i = 0; i < 4; i++)
        {
            Temp = Instantiate(ZombieObj[0]);
            Temp.transform.position = ZombiePoint[i].position;
        
            Temp.GetComponent<NavMeshAgent>().enabled = true;
            switch (Level)
            {
                case 0:
                    {
                        LevelHP = 25;
                        LevelDamage = 10;//*0.5
                        break;
                    }
                case 1:
                    {
                        LevelHP = 40;
                        LevelDamage = 20;//*1
                        break;
                    }
                case 2:
                    {
                        LevelHP = 70;
                        LevelDamage = 30;//*1.5
                        break;
                    }
                default:
                    break;
            }

            Temp.GetComponentInChildren<ZombieMove>().HP = LevelHP;
            Temp.GetComponentInChildren<ZombieMove>().AttackDamge = LevelDamage;
            Temp.transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Temp = null;
        }
    }
    void Zombie_BoomCreate()//1 폭탄좀비
    {
        for (int i = 0; i < 4; i++)
        {
            Temp = Instantiate(ZombieObj[1]);
            Temp.transform.position = ZombiePoint[i].position;
           
            Temp.GetComponent<NavMeshAgent>().enabled = true;
            switch (Level)  //좀비스크립트내에서 적용중(현재 Level수치는 미적용)
            {
                case 0:
                    {
                        LevelHP = 30;
                        LevelDamage = 25;//*0.5
                        break;
                    }
                case 1:
                    {
                        LevelHP = 50;
                        LevelDamage = 40;//*1
                        break;
                    }
                case 2:
                    {
                        LevelHP = 75;
                        LevelDamage = 55;//*2.5
                        break;
                    }
                default:
                    break;
            }

            Temp.GetComponentInChildren<ZombieMove_Boom>().HP = LevelHP;
            Temp.GetComponentInChildren<ZombieMove_Boom>().AttackDamge = LevelDamage;
            Temp.transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Temp = null;
        }
    }

    void Zombie_VomitCreate()//2 구토좀비
    {
        for (int i = 0; i < 4; i++)
        {
            Temp = Instantiate(ZombieObj[2]);
            Temp.transform.position = ZombiePoint[i].position;
           
            Temp.GetComponent<NavMeshAgent>().enabled = true;
            switch (Level)
            {
                case 0:
                    {
                        LevelHP = 25;
                        LevelDamage = 0;//*0.5
                        break;
                    }
                case 1:
                    {
                        LevelHP = 40;
                        LevelDamage = 0;//*1
                        break;
                    }
                case 2:
                    {
                        LevelHP = 70;
                        LevelDamage = 0;//*2.5
                        break;
                    }
                default:
                    break;
            }

            Temp.GetComponentInChildren<ZombieMove_Vomit>().HP = LevelHP;
            Temp.GetComponentInChildren<ZombieMove_Vomit>().AttackDamge = LevelDamage;
            Temp.transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Temp = null;
        }
    }

    void Zombie_SpeedCreate()//3 스피드좀비
    {
        for (int i = 0; i < 4; i++)
        {
            Temp = Instantiate(ZombieObj[3]);
            Temp.transform.position = ZombiePoint[i].position;
            
            Temp.GetComponent<NavMeshAgent>().enabled = true;
            switch (Level)
            {
                case 0:
                    {
                        LevelHP = 25;
                        LevelDamage = 10;//*0.5
                        break;
                    }
                case 1:
                    {
                        LevelHP = 40;
                        LevelDamage = 20;//*1
                        break;
                    }
                case 2:
                    {
                        LevelHP = 70;
                        LevelDamage = 30;//*1.5
                        break;
                    }
                default:
                    break;
            }

            Temp.GetComponentInChildren<ZombieMove>().HP = LevelHP;
            Temp.GetComponentInChildren<ZombieMove>().AttackDamge = LevelDamage;
            Temp.transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Temp = null;
        }
    }

    void Zombie_HideCreate()//4 투명좀비
    {
        for (int i = 0; i < 4; i++)
        {
            Temp = Instantiate(ZombieObj[4]);
            Temp.transform.position = ZombiePoint[i].position;
            
            Temp.GetComponent<NavMeshAgent>().enabled = true;
            switch (Level)
            {
                case 0:
                    {
                        LevelHP = 25;
                        LevelDamage = 10;//*0.5
                        break;
                    }
                case 1:
                    {
                        LevelHP = 40;
                        LevelDamage = 20;//*1
                        break;
                    }
                case 2:
                    {
                        LevelHP = 70;
                        LevelDamage = 30;//*1.5
                        break;
                    }
                default:
                    break;
            }

            Temp.GetComponentInChildren<ZombieMove>().HP = LevelHP;
            Temp.GetComponentInChildren<ZombieMove>().AttackDamge = LevelDamage;
            Temp.transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Temp = null;
        }
    }

    void Zombie_BigCreate()//5 초대형좀비
    {
        for (int i = 0; i < 4; i++)
        {
            Temp = Instantiate(ZombieObj[5]);
            Temp.transform.position = ZombiePoint[i].position;
           
            Temp.GetComponent<NavMeshAgent>().enabled = true;
            switch (Level)
            {
                case 0:
                    {
                        LevelHP = 250;
                        LevelDamage = 25;//*0.5
                        break;
                    }
                case 1:
                    {
                        LevelHP = 400;
                        LevelDamage = 50;//*1
                        break;
                    }
                case 2:
                    {
                        LevelHP = 700;
                        LevelDamage = 75;//*2.5
                        break;
                    }
                default:
                    break;
            }

            Temp.GetComponentInChildren<ZombieMove_Big>().HP = LevelHP;
            Temp.GetComponentInChildren<ZombieMove_Big>().AttackDamge = LevelDamage;
            Temp.transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Temp = null;
        }
    }
    IEnumerator StageSetup()
    {
        while (true)
        {
            
            if(!LevelUPStatSelect)
            {
                if(!m_DeathZone.Reset)
                     m_DeathZone.Reset = true;
                yield return new WaitForEndOfFrame();
                continue;
            }
            else
            {
                Stage++;
                LevelUPStatSelect = false;
                
                //Debug.Log("Zombie");
            }
            yield return new WaitForSeconds(10);
            while (true)    //플레이중
            {
                if (StageInit)
                {
                    int test = Stage;
                    if(test > 6)
                    {
                        test %= 6;
                    }

                    switch(test)
                    {
                        case 1:
                            {
                                ZombieCreate();
                                break;
                            }
                        case 2:
                            {
                                Zombie_BoomCreate();
                                break;
                            }
                        case 3:
                            {
                                Zombie_VomitCreate();
                                break;
                            }
                        case 4:
                            {
                                Zombie_SpeedCreate();
                                break;
                            }
                        case 5:
                            {
                                Zombie_HideCreate();
                                break;
                            }
                        case 6:
                            {
                                Zombie_BigCreate();
                                break;
                            }

                        default:
                            break;
                    }
                    //Debug.Log("Create");
                       //for (int i = 0; i < Stage; i++)    //좀비 소환
                       // {
                            //ZombieCreate();
                            yield return new WaitForSeconds(2);
                       // }
                        StageInit = false;
                }
                else if (ZombieCount <= 0 && !StageInit)
                {
                    StageInit = true;
                    LevelUP_UI.SetActive(true);
                    Main_UI.SetActive(false);
                    Time.timeScale = 0;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

        }
        yield return null;
    }
}
