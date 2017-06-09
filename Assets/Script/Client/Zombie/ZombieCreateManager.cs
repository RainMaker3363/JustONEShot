using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class ZombieCreateManager : MonoBehaviour {

    public GameObject GameplayObj;
   // [SerializeField]
   public GameObject ZombieObj;

    [SerializeField]
    Transform[] ZombiePoint;

    GameObject Temp;
    public static int ZombieCount = 0;
    [SerializeField]
    int Stage=0;
    int Level = 0;
    int LevelDamage = 0;

    bool StageInit;
    bool LevelUPStatSelect;

    public TMPro.TextMeshProUGUI UI_Stage;  //남은 좀비와 스테이지 현황표시
    public GameObject LevelUP_UI;
    public GameObject Main_UI;

    public DeathZone m_DeathZone;
    

    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
        UI_Stage.text = Stage.ToString() + " STAGE " + ZombieCount.ToString() + " ALIVE";

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
        GameObject.Find("PlayerCharacter").GetComponent<CharMove>().HPRecovery(30);

        LevelUP_UI.SetActive(false);
        Main_UI.SetActive(true);
        LevelUPStatSelect = true;
        Time.timeScale = 1;
    }

    void DamageUP()
    {
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
    void ZombieCreate()
    {
        for (int i = 0; i < 4; i++)
        {
            Temp = Instantiate(ZombieObj);
            Temp.transform.position = ZombiePoint[i].position;
            Temp.GetComponentInChildren<ZombieMove>().HP = 50;
            switch (Level)
            {
                case 0:
                    {
                        LevelDamage = 10;
                        break;
                    }
                case 1:
                    {
                        LevelDamage = 20;
                        break;
                    }
                case 2:
                    {
                        LevelDamage = 50;
                        break;
                    }
                default:
                    break;
            }

            Temp.GetComponentInChildren<ZombieMove>().AttackDamge = LevelDamage;
            Temp.transform.SetParent(GameplayObj.transform);
            ZombieCount++;
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
                    //Debug.Log("Create");
                       for (int i = 0; i < Stage; i++)    //좀비 소환
                        {
                            ZombieCreate();
                            yield return new WaitForSeconds(2);
                        }
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
