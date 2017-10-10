using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine;


public class ZombieCreateManager : MonoBehaviour
{

    public GameObject GameplayObj;
    [SerializeField]
    GameObject[] ZombieObj;

    [SerializeField]
    GameObject[] Zombies;

    [SerializeField]
    GameObject[] Zombies_Boom;

    [SerializeField]
    GameObject[] Zombies_Vomit;

    [SerializeField]
    GameObject[] Zombies_Big;

    [SerializeField]
    GameObject[] Zombies_Speed;

    [SerializeField]
    GameObject[] Zombies_Transparency;

    int Index_Zombie;
    int Index_Zombie_Boom;
    int Index_Zombie_Vomit;
    int Index_Zombie_Big;
    int Index_Zombie_Speed;
    int Index_Zombie_Transparency;

    int[,] ZombieStagearr;

    int ZombeCreateCode;
    [SerializeField]
    int ZombeInfinityCreateIndex = 0;

    [SerializeField]
    Transform[] ZombiePoint;

    GameObject Temp;
    public static int ZombieCount = 0;
    [SerializeField]
    public static int Stage = 0;
    int Level = 0;
    int LevelDamage = 0;
    int LevelHP = 0;
    int ZombieStatUP = 0;

    bool StageInit;
    bool LevelUPStatSelect;
    bool Infinity;

    public TMPro.TextMeshProUGUI UI_Stage;  //남은 좀비와 스테이지 현황표시
    public GameObject LevelUP_UI;
    public GameObject Main_UI;
    public GameObject Skill_UI;

    GameObject Player;

    public DeathZone m_DeathZone;

    public AudioClip SelectSound;
    AudioSource m_AudioSource;

    // Use this for initialization
    void Start()
    {
        m_AudioSource = gameObject.GetComponent<AudioSource>();
        ZombieIndexInit();
        ZombieStageInit();
        Infinity  = GameInfoManager.GetInstance().ZombieInfinityMode;
        Level = GameInfoManager.GetInstance().ZombieLevel;
    }

    // Update is called once per frame
    void Update()
    {
        UI_Stage.text = Stage.ToString() + " stage " + ZombieCount.ToString() + " zombie";

        if (LevelUP_UI.activeSelf)
        {
            Player.GetComponent<CharMove>().CharAllInit();
            
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
        //Level = 0;
        StageInit = true;

        LevelUP_UI.transform.Find("Bullet_Button").GetComponent<Button>().onClick.AddListener(BulletUP);
        LevelUP_UI.transform.Find("BulletMax_Button").GetComponent<Button>().onClick.AddListener(BulletMaxUP);
        LevelUP_UI.transform.Find("HP_Button").GetComponent<Button>().onClick.AddListener(HPUP);
        LevelUP_UI.transform.Find("Damage_Button").GetComponent<Button>().onClick.AddListener(DamageUP);
        LevelUP_UI.transform.Find("Reload_Button").GetComponent<Button>().onClick.AddListener(ReloadUP);

        LevelUPStatSelect = true;

        Player = GameplayObj.transform.Find("PlayerCharacter").gameObject;

        StartCoroutine(StageSetup());
    }

    //public void SelectLevel(int SelectLevel)
    //{
    //    Level = SelectLevel;
    //}
    //public void SelectMode(bool Selectmode)
    //{
    //    Infinity = Selectmode;
    //}

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
            LevelUP_UI.transform.Find("BulletMax_Button").Find("state").GetComponent<Image>().fillAmount = (CharMove.m_UseGun.MaxBullet_Hand - 10) / 20.0f;
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

    void ZombieIndexInit()
    {
        Index_Zombie = 0;
        Index_Zombie_Boom = 0;
        Index_Zombie_Vomit = 0;
        Index_Zombie_Big = 0;
        Index_Zombie_Speed = 0;
        Index_Zombie_Transparency = 0;
    }
    void ZombieCreate(int count)//0 일반좀비
    {
        for (int i = 0; i < count; i++)
        {
            Zombies[Index_Zombie].SetActive(true);
            if(count == 2)
            {
                Zombies[Index_Zombie].transform.position = ZombiePoint[i+1].position;
            }
            else
            {
                Zombies[Index_Zombie].transform.position = ZombiePoint[i].position;
            }
            

            Zombies[Index_Zombie].GetComponent<NavMeshAgent>().enabled = true;
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

            if (Infinity)
            {
                float HP = LevelHP;
                float Damage = LevelDamage;

                for (int j = 0; j < ZombieStatUP; j++)
                {
                    HP *=  1.25f;
                    Damage *= 1.25f;

                }
                LevelHP = (int)HP;
                LevelDamage = (int)Damage;
            }

            Zombies[Index_Zombie].GetComponentInChildren<ZombieMove>().HP = LevelHP;
            Zombies[Index_Zombie].GetComponentInChildren<ZombieMove>().AttackDamge = LevelDamage;
            StartCoroutine(Zombies[Index_Zombie].GetComponentInChildren<ZombieMove>().ZombieFastMoving());

           

            //Zombies[Index_Zombie].transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Index_Zombie++;
            //Temp = null;
        }
    }
    void Zombie_BoomCreate(int count)//1 폭탄좀비
    {
        for (int i = 0; i < count; i++)
        {
            Zombies_Boom[Index_Zombie_Boom].SetActive(true);
            if (count == 2)
            {
                Zombies_Boom[Index_Zombie_Boom].transform.position = ZombiePoint[i + 1].position;
            }
            else
            {
                Zombies_Boom[Index_Zombie_Boom].transform.position = ZombiePoint[i].position;
            }

            Zombies_Boom[Index_Zombie_Boom].GetComponent<NavMeshAgent>().enabled = true;
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

            Zombies_Boom[Index_Zombie_Boom].GetComponentInChildren<ZombieMove_Boom>().HP = LevelHP;
            Zombies_Boom[Index_Zombie_Boom].GetComponentInChildren<ZombieMove_Boom>().AttackDamge = LevelDamage;
            StartCoroutine(Zombies_Boom[Index_Zombie_Boom].GetComponentInChildren<ZombieMove_Boom>().ZombieFastMoving());
            //Zombies_Boom[Index_Zombie_Boom].transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Index_Zombie_Boom++;
            //Temp = null;
        }
    }

    void Zombie_VomitCreate(int count)//2 구토좀비
    {
        for (int i = 0; i < count; i++)
        {
            Zombies_Vomit[Index_Zombie_Vomit].SetActive(true);
            if (count == 2)
            {
                Zombies_Vomit[Index_Zombie_Vomit].transform.position = ZombiePoint[i + 1].position;
            }
            else
            {
                Zombies_Vomit[Index_Zombie_Vomit].transform.position = ZombiePoint[i].position;
            }

            Zombies_Vomit[Index_Zombie_Vomit].GetComponent<NavMeshAgent>().enabled = true;
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

            Zombies_Vomit[Index_Zombie_Vomit].GetComponentInChildren<ZombieMove_Vomit>().HP = LevelHP;
            Zombies_Vomit[Index_Zombie_Vomit].GetComponentInChildren<ZombieMove_Vomit>().AttackDamge = LevelDamage;
            StartCoroutine(Zombies_Vomit[Index_Zombie_Vomit].GetComponentInChildren<ZombieMove_Vomit>().ZombieFastMoving());
            //Zombies_Vomit[Index_Zombie_Vomit].transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Index_Zombie_Vomit++;
            //Temp = null;
        }
    }

    void Zombie_SpeedCreate(int count)//3 스피드좀비
    {
        for (int i = 0; i < count; i++)
        {
            Zombies_Speed[Index_Zombie_Speed].SetActive(true);
            if (count == 2)
            {
                Zombies_Speed[Index_Zombie_Speed].transform.position = ZombiePoint[i + 1].position;
            }
            else
            {
                Zombies_Speed[Index_Zombie_Speed].transform.position = ZombiePoint[i].position;
            }


            Zombies_Speed[Index_Zombie_Speed].GetComponent<NavMeshAgent>().enabled = true;
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

            Zombies_Speed[Index_Zombie_Speed].GetComponentInChildren<ZombieMove>().HP = LevelHP;
            Zombies_Speed[Index_Zombie_Speed].GetComponentInChildren<ZombieMove>().AttackDamge = LevelDamage;
            StartCoroutine(Zombies_Speed[Index_Zombie_Speed].GetComponentInChildren<ZombieMove>().ZombieFastMoving());
            //Zombies_Speed[Index_Zombie_Speed].transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Index_Zombie_Speed++;
            //Temp = null;
        }
    }

    void Zombie_HideCreate(int count)//4 투명좀비
    {
        for (int i = 0; i < count; i++)
        {
            Zombies_Transparency[Index_Zombie_Transparency].SetActive(true);
            if (count == 2)
            {
                Zombies_Transparency[Index_Zombie_Transparency].transform.position = ZombiePoint[i + 1].position;
            }
            else
            {
                Zombies_Transparency[Index_Zombie_Transparency].transform.position = ZombiePoint[i].position;
            }

            Zombies_Transparency[Index_Zombie_Transparency].GetComponent<NavMeshAgent>().enabled = true;
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

            Zombies_Transparency[Index_Zombie_Transparency].GetComponentInChildren<ZombieMove>().HP = LevelHP;
            Zombies_Transparency[Index_Zombie_Transparency].GetComponentInChildren<ZombieMove>().AttackDamge = LevelDamage;
            StartCoroutine(Zombies_Transparency[Index_Zombie_Transparency].GetComponentInChildren<ZombieMove>().ZombieFastMoving());
            //Zombies_Transparency[Index_Zombie_Transparency].transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Index_Zombie_Transparency++;
            //Temp = null;
        }
    }

    void Zombie_BigCreate(int count)//5 초대형좀비
    {
        for (int i = 0; i < count; i++)
        {
            Zombies_Big[Index_Zombie_Big].SetActive(true);
            if (count == 2)
            {
                Zombies_Big[Index_Zombie_Big].transform.position = ZombiePoint[i + 1].position;
            }
            else
            {
                Zombies_Big[Index_Zombie_Big].transform.position = ZombiePoint[i].position;
            }

            Zombies_Big[Index_Zombie_Big].GetComponent<NavMeshAgent>().enabled = true;
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

            Zombies_Big[Index_Zombie_Big].GetComponentInChildren<ZombieMove_Big>().HP = LevelHP;
            Zombies_Big[Index_Zombie_Big].GetComponentInChildren<ZombieMove_Big>().AttackDamge = LevelDamage;
            StartCoroutine(Zombies_Big[Index_Zombie_Big].GetComponentInChildren<ZombieMove_Big>().ZombieFastMoving());
            //Zombies_Big[Index_Zombie_Big].transform.SetParent(GameplayObj.transform);
            ZombieCount++;
            Index_Zombie_Big++;
            //Temp = null;
        }
    }
    IEnumerator StageSetup()
    {
        int Order=0;

        while (true)
        {

            if (!LevelUPStatSelect)
            {
                if (!m_DeathZone.Reset)
                    m_DeathZone.Reset = true;
                yield return new WaitForEndOfFrame();
                continue;
            }
            else
            {
                Stage++;
                ZombeInfinityCreateIndex++;
                LevelUPStatSelect = false;

                //Debug.Log("Zombie");
            }
            yield return new WaitForSeconds(10);
            
            while (true)    //플레이중
            {
                if (StageInit)
                {

                    //Debug.Log("ZombieCode : " + ZombieStagearr[Stage, Order]);\
                    if (Infinity)
                    {
                        if(ZombeInfinityCreateIndex>10)
                        {
                            ZombeInfinityCreateIndex = 5;
                            ZombieStatUP++;
                        }
                        for (int i = 0; i < ZombeInfinityCreateIndex; i++)
                        {
                            ZombieStageCreate(14);
                            yield return new WaitForSeconds(2);
                        }

                        StageInit = false;
                        ZombieIndexInit();//생성이 완료됬으니 인덱스 초기화
                    }
                    else
                    {
                        int ZombieCode = 0;
                        if (Order < 13)
                        {
                            ZombieCode = ZombieStagearr[Stage - 1, Order];
                        }

                        if (ZombieCode != 0)
                        {
                            ZombieStageCreate(ZombieCode);
                            Order++;
                            yield return new WaitForSeconds(2);
                        }
                        else
                        {
                            Order = 0;
                            StageInit = false;
                            ZombieIndexInit();//생성이 완료됬으니 인덱스 초기화
                        }

                    }
                    

                    //}
                    

                }
                else if (ZombieCount <= 0 && !StageInit)
                {
                    StageInit = true;
                    if (Stage < 20 || Infinity)
                    {
                        Skill_UI.SetActive(true);
                        LevelUP_UI.SetActive(true);
                        Main_UI.SetActive(false);
                        GameInfoManager.GetInstance().Pause = true;
                        Time.timeScale = 0;
                    }
                    else
                    {
                        GameObject.Find("PlayerCharacter").GetComponent<CharMove>().m_ZombieClear = true;
                        yield break;
                    }
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

        }
        yield return null;
    }

    void ZombieStageInit()
    {
        ZombieStagearr = new int[20, 13]
            {
                {14,0,0,0,0,0,0,0,0,0,0,0,0 },  //1
                {14,14,0,0,0,0,0,0,0,0,0,0,0 },  //2
                {14,14,22,0,0,0,0,0,0,0,0,0,0 },  //3
                {14,14,24,0,0,0,0,0,0,0,0,0,0 },  //4
                {14,14,32,14,0,0,0,0,0,0,0,0,0 },  //5
                {14,14,34,14,0,0,0,0,0,0,0,0,0 },  //6
                {14,14,22,14,32,0,0,0,0,0,0,0,0 },  //7
                {14,14,22,14,14,32,0,0,0,0,0,0,0 },  //8
                {14,14,24,14,14,34,0,0,0,0,0,0,0 },  //9
                {14,14,24,14,14,34,61,0,0,0,0,0,0 },  //10
                {14,14,22,14,14,42,14,0,0,0,0,0,0 },  //11
                {14,14,24,14,14,42,14,0,0,0,0,0,0 },  //12
                {14,14,32,14,14,42,14,0,0,0,0,0,0 },  //13
                {14,14,34,14,14,42,14,14,0,0,0,0,0 },  //14
                {14,14,22,14,14,32,14,14,44,0,0,0,0 },  //15
                {14,14,22,14,14,32,14,14,52,0,0,0,0 },  //16
                {14,14,22,14,14,42,14,14,52,14,0,0,0 },  //17
                {14,14,32,14,14,42,14,14,52,14,0,0,0 },  //18
                {14,14,22,14,14,32,14,14,42,14,54,0,0 },  //19
                {14,14,24,14,14,34,14,14,44,14,14,54,62 },  //20
            };
        
    }

    void ZombieStageCreate(int Code)
    {
        int Count = Code % 10;
        int Kind = Code / 10;

        switch (Kind)
        {
            case 1:
                {
                    ZombieCreate(Count);
                    break;
                }
            case 2:
                {
                    Zombie_HideCreate(Count);
                    break;
                }
            case 3:
                {
                    Zombie_BoomCreate(Count);
                    break;
                }
            case 4:
                {
                    Zombie_VomitCreate(Count);
                    break;
                }
            case 5:
                {
                    Zombie_SpeedCreate(Count);
                    break;
                }
            case 6:
                {
                    Zombie_BigCreate(Count);
                    break;
                }

            default:
                break;
        }
    }
}