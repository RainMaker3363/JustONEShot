using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LSD
{
    //플레이어 상태
    enum PlayerState
    {
        IDLE,
        DASH_SLOW,
        DASH_SOFT,
        DASH_HARD,
        SHOT_READY,
        SHOT_FIRE,
        DAMAGE,
        DEADEYE,
        REROAD,
        ROLL,
        DEAD,
        WIN,
        MAX
    }

    //총 상태(추후 추가)
    enum GunState
    {
        Revolver = 0,
        ShotGun,
        Musket

    }

    enum GameMode
    {
        PVP =0,
        Survivel,
        Zombie,
        Tutorial              

    }
}

abstract public class UseChar
{
    public float MaxStamina;
    public float Stamina;
    public int MaxHP;
    public int HP;
    public float Speed;  //퍼센트기준 1==100%
    public float SteminaRecovery; //퍼센트기준 1==100%

}

class Char_00 : UseChar
{

    public Char_00()
    {
        MaxStamina = Stamina = 1000;
        MaxHP = HP = 100;
        Speed = 1;
        SteminaRecovery = 1;
    }

}

class Char_01 : UseChar
{

    public Char_01()
    {
        MaxStamina = Stamina = 800;
        MaxHP = HP = 80;
        Speed = 1.2f;
        SteminaRecovery = 1.2f;
    }

}

class Char_02 : UseChar
{

    public Char_02()
    {
        MaxStamina = Stamina = 850;
        MaxHP = HP = 150;
        Speed = 0.8f;
        SteminaRecovery = 0.8f;
    }

}

class Char_03 : UseChar
{

    public Char_03()
    {
        MaxStamina = Stamina = 1200;
        MaxHP = HP = 120;
        Speed = 0.9f;
        SteminaRecovery = 0.9f;
    }

}

public class CharMove : MonoBehaviour
{


    public MoveJoyStick m_MoveJoyStickControl;  //움직임 전용 조이스틱
    public JoyStickCtrl m_ShotJoyStickControl;  //샷 전용 조이스틱

    public Animator anim;

    //이동 속도?
    public Image m_FirstTouch;

    public GameObject cam;
    Animator camAni;
    private Vector3 CamPos;
    public Transform CamLook;
    public GameObject UI_Main;
    public GameObject UI_GameOver;
    public Animator Result; //게임 결과 연출
    //public Text UI_GameOverText;
    public GameObject UI_DamageEffect;

    // 플레이어의 움직임
    private float m_MoveSpeed;
    public static float m_RollSpeed;
    private Vector3 m_MoveVector;
    private Vector3 m_PlayerDir;
    //private Vector3 PlayerDeadEyePosBack;
    private Vector3 m_PlayerPosBack;
    public Transform m_GroundCheck;
    private CharacterController m_CharCtr;

    LSD.PlayerState m_PlayerState;
    LSD.PlayerState m_PlayerBeforeState;
    LSD.GunState m_GunState;

    public int m_DebugPlayerState;

    int CharIndex = 0; //캐릭터 선택 인덱스

    //캐릭터 총
    public static UseGun m_UseGun;

    //캐릭터 stat
    public static UseChar CharStat;

    bool StaminaRecovery = true;

    public static bool Invincibility = false;


    public static bool Skill_Fastgun = false;
    public static bool Skill_Hide = false;
    public static bool Skill_BloodBullet = false;
    public static bool Skill_Invincibility = false;
    bool Button_SkillOn = false;

    public SkinnedMeshRenderer WomenSkin;
    SkinnedMeshRenderer WomenGun;
    public Material WomenCamo;
    public GameObject CamoEffect;
    public GameObject InvincibilityEffect;

    //float Stamina = 1000;   
    //[SerializeField]
    //int HP = 100;
    //int Speed = 1;

    bool ShotAble = true;   //조준 가능여부

    //스테미나가 전부 소모된 상태
    bool m_Exhausted = false;

    static bool DeadEyeStart = false;
    public static bool DeadEyeEnd = false;

    bool m_AniPlay = false;

    public Transform EnemyPos;  //  적 캐릭터 위치 추후변경예상

    public Image HP_bar;
    public Image Stamina_bar;

    public MultiGameManager Mul_Manager;

    public static float m_DeadEyeTimer;   //플레이어 데드아이 시간
    float EnemyDeadEyeTimer;    //적 데드아이 시간
    bool DeadEyecomplete = false;
    public static bool DeadEyeSuccess;
    bool DeadEyeSuccessCheck = false;

    public Transform DeathZone;
    bool DeathZoneDealay = false;

    public static bool GameEnd = false;

    float Debug_DeadEyeTimer_Player = 0;
    float Debug_DeadEyeTimer_Enemy = 0;

    public GameObject Revlolver;
    public GameObject ShotGun;
    public GameObject Musket;
    public static bool m_GunSelect = false;
    public GameObject GunPoint;


    private float PlayerUpdateTime;
    private float PlayerUpdateDelay = 0.11f;

    public bool m_ZombieClear = false;

    public Sprite DeadImage;
    public Sprite ClearImage;
    [SerializeField]
    Sprite[] RankImage;

    AudioSource m_AudioSource;
    public AudioClip CharHitSound;


    public SkinnedMeshRenderer Skin;

    string NowSceneName;
    TMPro.TextMeshProUGUI TutorialText;
    bool DeadEyeTutorial = false;

    GameObject Button_Roll;
    //void OnEnable()
    //{

    //    //플레이어 선택 총
    //    switch (m_GunState)
    //    {
    //        case LSD.GunState.Revolver:
    //            {

    //                break;
    //            }
    //        case LSD.GunState.ShotGun:
    //            {
    //                    break;
    //            }
    //        case LSD.GunState.Musket:
    //            {
    //                         break;
    //            }
    //        default:
    //            break;
    //    }
    //}

    void Awake()
    {
        m_GunState = LSD.GunState.ShotGun; //현재는 고정 추후 받아오게함
        CharIndex = GameInfoManager.GetInstance().SelectIndex;
        CharInit();
        m_ZombieClear = false;
        m_GunSelect = false;
        gameObject.SetActive(false);
        m_AudioSource = gameObject.transform.GetComponentInChildren<AudioSource>();
       
    }

    // Use this for initialization
    void Start()
    {
        NowSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        m_CharCtr = GetComponent<CharacterController>();

        //카메라 기본위치 설정
        CamPos = cam.transform.position;
        if (m_UseGun.Sight != null)
            CamPos = m_UseGun.Sight;

        camAni = cam.GetComponent<Animator>();


        // 플레이어 이동 방향 초기화
        m_MoveVector = Vector3.zero;
        m_MoveSpeed = 2.0f;
        m_PlayerDir = Vector3.zero;
        m_PlayerPosBack = Vector3.zero;

        switch (CharIndex)
        {
            case 0:
                {
                    CharStat = new Char_00();
                    break;
                }
            case 1:
                {
                    CharStat = new Char_01();
                    break;
                }
            case 2:
                {
                    CharStat = new Char_02();
                    break;
                }
            case 3:
                {
                    CharStat = new Char_03();
                    break;
                }
            default:
                break;
        }

        if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
        {
            Mul_Manager.SendHPStateMessage(CharStat.HP);
        }

        //플레이어 UI초기화
        HP_bar.fillAmount = 1;
        Stamina_bar.fillAmount = 1;

        cam.transform.position = CamPos + transform.position;

        StartCoroutine(ServerUpdate());
        PlayerUpdateTime = Time.time + PlayerUpdateDelay;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.K))
        {
            Damaged(10000, Vector2.zero);
        }
#endif
        //if (PlayerUpdateTime<Time.time)
        //{
        //    PlayerUpdateTime = Time.time + PlayerUpdateDelay;
        //Mul_Manager.SendMultiSelectStateMessage

        m_DebugPlayerState = (int)m_PlayerState;
        // Debug.Log("VectorForce: " + m_MoveJoyStickControl.GetVectorForce());
        //Debug.Log("PlayerState: " + m_PlayerState);

        if (m_PlayerBeforeState != m_PlayerState)
        {

            if (GameEnd)   //이기거나 졌을경우 변경을 막음
            {
                //ShotAble = false;
                m_PlayerState = m_PlayerBeforeState;
            }
            else
            {
                CharAniInit();
                m_PlayerBeforeState = m_PlayerState;
            }

        }

        DeadEyeCheck();
        DeathZoneCheck();
        GameEndCheck();

        if(NowSceneName == "TutorialScene")
        {
            if(Button_SkillOn && !Skill_BloodBullet)
            {
                Button_SkillOn = false;
                UI_Main.transform.Find("Control/Button_Skill").gameObject.SetActive(true);
                TutorialText.gameObject.SetActive(false);
            }
        }

        if (ShotAble && m_ShotJoyStickControl.GetTouch())
        {
            //    if(m_PlayerState != LSD.PlayerState.SHOT_FIRE)        
            m_PlayerState = LSD.PlayerState.SHOT_READY;
        }

        switch (m_PlayerState)
        {
            case LSD.PlayerState.IDLE:
                {
                    if(Physics.GetIgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy")))
                    {
                        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
                    }
                    ShotAble = true;
                    anim.SetInteger("DashLevel", 0);
                    anim.speed = 1;
                    Update_IDLE();
                    break;
                }
            case LSD.PlayerState.DASH_SLOW:
                {
                    ShotAble = true;
                    anim.SetInteger("DashLevel", 1);
                    anim.speed = CharStat.Speed;
                    m_FirstTouch.color = Color.red;
                    Update_DASH_SLOW();
                    break;
                }
            case LSD.PlayerState.DASH_SOFT:
                {
                    ShotAble = true;
                    anim.SetInteger("DashLevel", 2);
                    anim.speed = CharStat.Speed;
                    m_FirstTouch.color = Color.green;
                    Update_DASH_SOFT();
                    break;
                }
            case LSD.PlayerState.DASH_HARD:
                {
                    ShotAble = true;
                    anim.SetInteger("DashLevel", 3);
                    anim.speed = CharStat.Speed;
                    m_FirstTouch.color = Color.blue;
                    Update_DASH_HARD();
                    break;
                }
            case LSD.PlayerState.SHOT_READY:
                {
                    if (!m_AniPlay)
                    {
                        anim.Play("Shot_Ready");
                        m_AniPlay = true;
                        GunPoint.SetActive(true);
                    }
                    anim.SetBool("ShotReady", true);

                    Update_SHOT_READY();
                    break;
                }
            case LSD.PlayerState.SHOT_FIRE:
                {
                    ShotAble = false;
                    anim.SetBool("ShotReady", false);
                    Update_SHOT_FIRE();
                    break;
                }
            case LSD.PlayerState.DAMAGE:
                {
                    m_AniPlay = true;
                    ShotAble = false;
                    Update_DAMAGE();
                    break;
                }
            case LSD.PlayerState.DEADEYE:
                {
                    ShotAble = false;
                    Update_DEADEYE();
                    break;
                }
            case LSD.PlayerState.REROAD:
                {
                    // anim.Play("Reloading");
                    ShotAble = false;
                    anim.SetBool("Reloading", true);
                    Update_REROAD();
                    break;
                }
            case LSD.PlayerState.ROLL:
                {
                    if (!m_AniPlay)
                    {
                        anim.Play("Roll");
                        m_AniPlay = true;
                        Button_Roll.SetActive(false);
                    }
                    ShotAble = false;
                    Update_Roll();
                    break;
                }
            case LSD.PlayerState.DEAD:
                {
                    ShotAble = false;
                    if (UI_Main.activeSelf)
                    {
                        UI_Main.SetActive(false);
                    }

                    break;
                }
            case LSD.PlayerState.WIN:
                {
                    ShotAble = false;
                    if (UI_Main.activeSelf)
                    {
                        UI_Main.SetActive(false);
                    }
                    break;
                }

            default:
                {
                    break;
                }
        }
        //}
    }

    void CharAniInit()
    {
        m_AniPlay = false;
        if (anim.GetBool("Reloading"))
        {
            anim.SetBool("Reloading", false);
        }
        UI_DamageEffect.SetActive(false);
    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////
    /// Update
    /// 
    /// </summary>
#region Update_State
    void Update_IDLE()
    {
        //if (anim.GetBool("Reloading"))
        //{
        //    anim.SetBool("Reloading", false);
        //}

        if (!m_Exhausted)//탈진상태가 아니라면
        {
            if (m_MoveJoyStickControl.GetVectorForce() > 0)
            {
                m_PlayerState = LSD.PlayerState.DASH_SOFT;
            }
            //else if (m_MoveJoyStickControl.GetVectorForce() > 0.5f)
            //{
            //    m_PlayerState = LSD.PlayerState.DASH_HARD;
            //}
        }
        else //탈진상태라면
        {
            if (m_MoveJoyStickControl.GetVectorForce() > 0)
            {
                m_PlayerState = LSD.PlayerState.DASH_SLOW;
            }
        }


    }

    void Update_DASH_SLOW()
    {
        if (m_MoveJoyStickControl.GetVectorForce() > 0) // 계속 조작중인경우
        {
            if(TutorialText != null)
            {
                TutorialText.gameObject.SetActive(true);
                TutorialText.text = "slow";
            }

            if (!m_Exhausted) //탈진상태에서 회복됬다면
            {
                if (TutorialText != null)
                {
                    TutorialText.gameObject.SetActive(false);
                }
                m_PlayerState++;    //Player상태를 DASH_SOFT로 변경
                                    //m_PlayerState = LSD.PlayerState.DASH_SOFT;
            }
        }
        else //조작을 멈출경우
        {
            if (TutorialText != null)
            {
                TutorialText.gameObject.SetActive(false);
            }
            m_PlayerState--;//Player상태를 IDLE로 변경
                            //m_PlayerState = LSD.PlayerState.IDLE;
        }
    }

    void Update_DASH_SOFT()
    {
        if (!m_Exhausted)//탈진상태가 아니라면
        {

            if (m_MoveJoyStickControl.GetVectorForce() > 0.5f)
            {
                m_PlayerState = LSD.PlayerState.DASH_HARD;
            }
            else if (m_MoveJoyStickControl.GetVectorForce() > 0)
            {
                m_PlayerState = LSD.PlayerState.DASH_SOFT;
            }
            else //조작을 멈출경우
            {
                m_PlayerState = LSD.PlayerState.IDLE;
            }

        }
        else //탈진상태라면
        {
            if (m_MoveJoyStickControl.GetVectorForce() > 0)
            {
                m_PlayerState = LSD.PlayerState.DASH_SLOW;
            }
        }
    }

    void Update_DASH_HARD()
    {
        if (!m_Exhausted)//탈진상태가 아니라면
        {

            if (m_MoveJoyStickControl.GetVectorForce() > 0.5f)
            {
                StaminaRecovery = false;
                m_PlayerState = LSD.PlayerState.DASH_HARD;
            }
            else if (m_MoveJoyStickControl.GetVectorForce() > 0)
            {
                StopCoroutine(StaminaRecoveryDealy());
                StartCoroutine(StaminaRecoveryDealy());
                m_PlayerState = LSD.PlayerState.DASH_SOFT;
            }
            else //조작을 멈출경우
            {
                StopCoroutine(StaminaRecoveryDealy());
                StartCoroutine(StaminaRecoveryDealy());
                m_PlayerState = LSD.PlayerState.IDLE;
            }
        }
        else //탈진상태라면
        {
            if (m_MoveJoyStickControl.GetVectorForce() > 0)
            {
                StopCoroutine(StaminaRecoveryDealy());
                StartCoroutine(StaminaRecoveryDealy());
                m_PlayerState = LSD.PlayerState.DASH_SLOW;
            }
        }
    }

    void Update_SHOT_READY()
    {
        //조준중에 손을 땠다면
        if (!m_ShotJoyStickControl.GetTouch())
        {
            anim.SetBool("GunFire", true);
            GunPoint.SetActive(false);
            if (!Skill_Fastgun)
            {
                if (m_UseGun.Bullet_Gun >= m_UseGun.Bullet_Use)
                {
                    anim.SetBool("Shot", true);

                    if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
                    {
                        Mul_Manager.SendShootMessage(true);
                    }
                    m_PlayerState = LSD.PlayerState.SHOT_FIRE;
                }
                else
                {
                    anim.SetBool("Shot", false);
                    if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
                    {
                        Mul_Manager.SendShootMessage(false);
                    }

                    if (TutorialText != null)
                    {
                        TutorialText.gameObject.SetActive(true);
                        if (m_UseGun.Bullet_Hand < 1)
                        {
                            TutorialText.text = "no ammo";
                        }
                        else if (m_UseGun.Bullet_Gun < m_UseGun.Bullet_Use)
                        {
                            TutorialText.text = "no reload";
                        }
                    }
                    m_PlayerState = LSD.PlayerState.SHOT_FIRE;
                }
            }
            else
            {
                anim.SetBool("Shot", true);
                anim.speed = 4;
                if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
                {
                    Mul_Manager.SendShootMessage(true);
                }
                m_PlayerState = LSD.PlayerState.SHOT_FIRE;
            }

        }

    }

    void Update_SHOT_FIRE()
    {
        if (!anim.GetBool("GunFire"))
        {
            if (TutorialText != null)
            {
                TutorialText.gameObject.SetActive(false);
            }
            anim.speed = 1;
            m_PlayerState = LSD.PlayerState.IDLE;
        }
        else
        {

        }
    }

    void Update_DAMAGE()
    {
        UI_DamageEffect.SetActive(true);
        if (!anim.GetBool("Damaged"))
        {
            anim.SetBool("DeadEyeDamage", false);
            m_PlayerState = LSD.PlayerState.IDLE;
        }
    }

    void Update_DEADEYE()
    {
        //if(!m_MoveJoyStickControl.TouchBegin)
        //{
        m_MoveJoyStickControl.PedInit();
        //}
        if (Mul_Manager != null)
        {
            if ((DeadEyeEnd || !Mul_Manager.GetDeadEyeChecker()) && DeadEyeSuccessCheck)
            {
                //판별했으니 초기화
                EnemyDeadEyeTimer = 0;
                m_DeadEyeTimer = 0;
                DeadEyeEnd = false;
                DeadEyeSuccessCheck = false;
                cam.transform.position = CamPos + transform.position;
                if (GPGSManager.GetInstance.IsAuthenticated())
                {
                    Mul_Manager.SendDeadEyeMessage(false);
                }
                m_PlayerState = LSD.PlayerState.IDLE;
                if (NowSceneName == "GameScene0" || NowSceneName == "SurvivalScene0" || NowSceneName == "ZombieScene")
                {
                    DeathZone.GetComponent<DeathZone>().DeadEyePlaying = false;
                }
                else
                {
                    DeathZone.parent.parent.GetComponent<DeathZone_03>().DeadEyePlaying = false;
                }
            }


            if (m_DeadEyeTimer > 0 && !DeadEyecomplete && !DeadEyeSuccessCheck)
            {
                Mul_Manager.SendDeadEyeTimerMessage(m_DeadEyeTimer);
                DeadEyecomplete = true;
            }

            EnemyDeadEyeTimer = Mul_Manager.GetDeadEyeTimer();

            if (EnemyDeadEyeTimer > 0 && DeadEyecomplete && !DeadEyeSuccessCheck)  //플레이어의 데드아이가 끝났고 상대데드아이가 끝났는가
            {
                if (EnemyDeadEyeTimer > m_DeadEyeTimer)//데드아이성공여부 판별
                {
                    DeadEyeSuccess = true;
                }
                else
                {
                    DeadEyeSuccess = false;
                }
                //디버그용
                Debug_DeadEyeTimer_Player = m_DeadEyeTimer;
                Debug_DeadEyeTimer_Enemy = EnemyDeadEyeTimer;

                DeadEyecomplete = false;
                DeadEyeSuccessCheck = true;
            }
        }
        else
        {
            if(!DeadEyeTutorial)
            {
                DeadEyeTutorial = true;
                StartCoroutine(DeadEye_Tutorial());
            }
        }
    }

    void Update_REROAD()
    {
        if (m_MoveJoyStickControl.GetVectorForce() > 0)
        {
            anim.SetBool("Reloading", false);
            m_PlayerState = LSD.PlayerState.IDLE;
        }

        if (m_UseGun.Bullet_Hand > 0 && m_UseGun.MaxBullet_Gun > m_UseGun.Bullet_Gun)//탄알이 있고 탄창이 안찼을때
        {

        }
        else
        {
            anim.SetBool("Reloading", false);
            m_PlayerState = LSD.PlayerState.IDLE;
        }
    }

    void Update_Roll()
    {


        if (!anim.GetBool("Rolling"))
        {
            StopCoroutine(StaminaRecoveryDealy());
            StartCoroutine(StaminaRecoveryDealy());
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
            
            m_PlayerState = LSD.PlayerState.IDLE;
            Button_Roll.SetActive(true);
        }
        else
        {
            
            StaminaRecovery = false;
        }
    }

#endregion Update_State

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    public void OnReroadButton()
    {
        if (m_PlayerState == LSD.PlayerState.IDLE)
        {

            m_PlayerState = LSD.PlayerState.REROAD;
            m_MoveJoyStickControl.PedInit();

        }
    }

    public void OnRollButton()
    {
        if (m_PlayerState != LSD.PlayerState.SHOT_FIRE && m_PlayerState != LSD.PlayerState.ROLL && !m_Exhausted)
        {
            if (CharStat.Stamina > 400 || Skill_Hide)
            {
                if (m_PlayerState == LSD.PlayerState.DAMAGE)
                {
                    anim.SetBool("Damaged", false);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌 중간에 캔슬된경우 여기서 바꿔줌
                }

                if (!Skill_Hide)
                    CharStat.Stamina -= 400;
                //PlayerMove();
                anim.SetBool("Rolling", true);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌
                m_PlayerState = LSD.PlayerState.ROLL;
                StaminaCheck();
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
                if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
                {
                    Mul_Manager.SendAniStateMessage((int)m_PlayerState);//서버 전송
                }
            }
        }
    }

    public void Damaged(int Damage, Vector3 vec) //데미지 모션, 매개변수로 데미지와 방향벡터를 가져옴
    {
        if (!Invincibility)
        {
            Vector3 DamageVec = -vec; //forword를 가져오므로 반대방향을볼수있게 -를 붙임
            DamageVec.y = 0; //위아래로는 움직이지 않게합니다
            transform.rotation = Quaternion.LookRotation(DamageVec);

            m_PlayerState = LSD.PlayerState.DAMAGE;

            CharStat.HP -= Damage;

            HP_bar.fillAmount = (float)CharStat.HP / CharStat.MaxHP;

            if (!DeadCheck())
            {
                anim.SetTrigger("Damage");
                anim.SetBool("Damaged", true);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌
                camAni.SetTrigger("Damage");
                GunPoint.SetActive(false);
                if (GameInfoManager.GetInstance().EffectSoundUse)
                {
                    m_AudioSource.PlayOneShot(CharHitSound);
                }
            }


            if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
            {
                Mul_Manager.SendMyPositionUpdate();
                Mul_Manager.SendAniStateMessage((int)m_PlayerState);//서버 전송
                Mul_Manager.SendHPStateMessage(CharStat.HP);
                Mul_Manager.SendShootVectorMessage(DamageVec);
            }
            Handheld.Vibrate();
        }

    }

    public void DeadEyeDamaged(int Damage, Vector3 vec) //데미지 모션, 매개변수로 데미지와 방향벡터를 가져옴
    {
        Vector3 DamageVec = -vec; //forword를 가져오므로 반대방향을볼수있게 -를 붙임
        DamageVec.y = 0; //위아래로는 움직이지 않게합니다
        transform.rotation = Quaternion.LookRotation(DamageVec);

        m_PlayerState = LSD.PlayerState.DAMAGE;

        if (DeadEyeSuccess) //데드아이 피격시 성공했다면
        {

            CharStat.HP -= Damage;
            if (!DeadCheck())
            {
                anim.SetTrigger("Damage");
                anim.SetBool("Damaged", true);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌
                camAni.SetTrigger("Damage");
            }
        }
        else
        {
            CharStat.HP -= (Damage + 45);
            if (!DeadCheck())
            {
                anim.SetTrigger("DeadEyeDamage");
                anim.SetBool("Damaged", true);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌
                camAni.SetTrigger("Damage");
            }

        }

        HP_bar.fillAmount = (float)CharStat.HP / CharStat.MaxHP;

        if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
        {
            Mul_Manager.SendMyPositionUpdate();
            Mul_Manager.SendAniStateMessage((int)m_PlayerState);//서버 전송
            Mul_Manager.SendHPStateMessage(CharStat.HP);
        }
        if (GameInfoManager.GetInstance().EffectSoundUse)
        {
            m_AudioSource.PlayOneShot(CharHitSound);
        }
        Handheld.Vibrate();
    }

    public bool DeadCheck()
    {
        if (CharStat.HP <= 0 && !GameEnd)
        {
            CharStat.HP = 0;
            m_PlayerState = LSD.PlayerState.DEAD;
            m_PlayerBeforeState = LSD.PlayerState.DEAD;
            anim.SetTrigger("Death");
            GameEnd = true;
            // Mul_Manager.SendAniStateMessage((int)m_PlayerState); //데미지부분에서도 보내줌
            if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
            {
                Mul_Manager.SendEndGameMssage(true);
            }

            UI_Main.SetActive(false);
            UI_GameOver.SetActive(true);
            cam.SetActive(false);
            gameObject.GetComponent<CharacterController>().enabled = false;

            //EnemyPos.gameObject.SetActive(false);
            if (NowSceneName == "ZombieScene" || NowSceneName == "TutorialScene")
            {
                Result.transform.Find("Effect_Result_Lose_01/Effect_WinUI/Images/Color_Text_Lose").GetComponent<SpriteRenderer>().sprite = DeadImage;
            }
            else if (NowSceneName == "SurvivalScene0"||NowSceneName == "SurvivalScene1")
            {
                int Rank = Mul_Manager.GetMySurvivalRankNumber();
                Result.transform.Find("Effect_Result_Lose_01/Effect_WinUI/Images/Color_Text_Lose").GetComponent<SpriteRenderer>().sprite = RankImage[Rank-1];

            }

            Result.SetTrigger("Lose");
            if(GameObject.Find("BGM"))
                GameObject.Find("BGM").GetComponent<AudioSource>().mute = true;

            GameInfoManager.GetInstance().GameOver = true;
            // UI_GameOverText.GetComponent<Text>().text = "Defeat";

            return true;
        }
        else if (GameEnd)
        {
            return true;
        }
        return false;
    }

    public void GameEndCheck()
    {
        if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)  //접속중일때
        {
            if (Mul_Manager.GetEndGameState() && !GameEnd)
            {
                //UI_GameOverText.GetComponent<Text>().text = "Victory";
                Mul_Manager.SendAniStateMessage((int)m_PlayerState);
                GameEnd = true;
                UI_Main.SetActive(false);
                StartCoroutine(PlayerWin());

            }
        }

        if (m_ZombieClear && !GameEnd)
        {
            GameEnd = true;
            UI_Main.SetActive(false);
            StartCoroutine(PlayerWin());
        }
    }

    public static void DeadEye()    //데드아이 총알을 먹었을경우
    {
        DeadEyeStart = true;

    }

    public void DeadEyeCheck()
    {
        if (DeadEyeStart)
        {
            transform.LookAt(EnemyPos.position);
            anim.SetInteger("DashLevel", 0);
            m_PlayerState = LSD.PlayerState.DEADEYE;
            anim.SetTrigger("DeadEye");

            DeadEyeStart = false;
            if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
            {
                Mul_Manager.SendDeadEyeMessage(true);
            }

            if (NowSceneName == "GameScene0" || NowSceneName == "SurvivalScene0" || NowSceneName == "ZombieScene")
            {
                DeathZone.GetComponent<DeathZone>().DeadEyePlaying = false;
            }
            else
            {
                DeathZone.parent.parent.GetComponent<DeathZone_03>().DeadEyePlaying = false;
            }
        }

        if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
        {
            if (Mul_Manager.GetDeadEyeChecker())
            {
                if (!m_AniPlay)
                {
                    anim.Play("Idle");
                    anim.SetInteger("DashLevel", 0);
                    m_AniPlay = true;
                }
                m_PlayerState = LSD.PlayerState.DEADEYE;
            }
        }
    }

    void DeathZoneCheck()
    {
        if (NowSceneName == "GameScene0" || NowSceneName == "SurvivalScene0" || NowSceneName == "ZombieScene")
        {
            if (DeathZone.position.y + 0.02 >= transform.position.y)
            {
                if (!DeathZoneDealay)
                {
                    DeathZoneDealay = true;
                    CharStat.HP -= DeathZone.gameObject.GetComponent<DeathZone>().Damage;
                    HP_bar.fillAmount = (float)CharStat.HP / 100;
                    DeadCheck();
                    StartCoroutine(DeathZoneDealyTime(DeathZone.gameObject.GetComponent<DeathZone>().DamageDealay));
                }
            }
        }
        else
        {
            //Debug.Log("Distance"+Vector3.Distance(Vector2.zero, new Vector3(transform.position.x, 0, transform.position.z)));
            //Debug.Log("CenterDistance" + Mathf.Abs(DeathZone.position.x));
            if (DeathZone.position.x < Vector3.Distance(Vector2.zero, new Vector3(transform.position.x, 0, transform.position.z)))
            {
                if (!DeathZoneDealay)
                {
                    DeathZoneDealay = true;
                    CharStat.HP -= DeathZone.parent.parent.gameObject.GetComponent<DeathZone_03>().Damage;
                    HP_bar.fillAmount = (float)CharStat.HP / 100;
                    DeadCheck();
                    StartCoroutine(DeathZoneDealyTime(DeathZone.parent.parent.gameObject.GetComponent<DeathZone_03>().DamageDealay));
                }
            }
        }
    }

    public void HPRecovery(int Recovery)
    {
        CharStat.HP += Recovery;
        if (CharStat.MaxHP < CharStat.HP)
        {
            CharStat.HP = CharStat.MaxHP;
        }
        HP_bar.fillAmount = (float)CharStat.HP / 100;
    }

    //public void DeadEyeUIOn()
    //{
    //   DeadEyeUI.SetActive(true);
    //}

    void FixedUpdate()
    {
        //Mul_Manager.GetAniStateMessage
        switch (m_PlayerState)
        {
            case LSD.PlayerState.IDLE:
                {
                    FixedUpdate_IDLE();
                    break;
                }
            case LSD.PlayerState.DASH_SLOW:
                {
                    FixedUpdate_DASH_SLOW();
                    break;
                }
            case LSD.PlayerState.DASH_SOFT:
                {
                    FixedUpdate_DASH_SOFT();
                    break;
                }
            case LSD.PlayerState.DASH_HARD:
                {
                    FixedUpdate_DASH_HARD();
                    break;
                }
            case LSD.PlayerState.SHOT_READY:
                {
                    FixedUpdate_SHOT_READY();
                    break;
                }
            case LSD.PlayerState.SHOT_FIRE:
                {
                    FixedUpdate_SHOT_FIRE();
                    break;
                }
            case LSD.PlayerState.DAMAGE:
                {
                    FixedUpdate_DAMAGE();
                    break;
                }
            case LSD.PlayerState.DEADEYE:
                {
                    FixedUpdate_DEADEYE();
                    break;
                }
            case LSD.PlayerState.REROAD:
                {
                    FixedUpdate_REROAD();
                    break;
                }
            case LSD.PlayerState.ROLL:
                {
                    FixedUpdate_Roll();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }


    void FixedUpdate_IDLE()
    {
        if (m_Exhausted)
        {
            CharStat.Stamina += (7 * CharStat.SteminaRecovery);
        }
        else
        {
            if (StaminaRecovery)
                CharStat.Stamina += (10 * CharStat.SteminaRecovery);
        }
        StaminaCheck();
    }
    void FixedUpdate_DASH_SLOW()    //탈진 달리기
    {
        m_MoveSpeed = 1 * CharStat.Speed;
        if (StaminaRecovery)
            CharStat.Stamina += (7 * CharStat.SteminaRecovery);
        PlayerMove();
        StaminaCheck();
    }
    void FixedUpdate_DASH_SOFT()    // 천천히 달리기
    {
        m_MoveSpeed = 5 * CharStat.Speed;
        if (StaminaRecovery)
            CharStat.Stamina += (10 * CharStat.SteminaRecovery);
        PlayerMove();
        StaminaCheck();
    }
    void FixedUpdate_DASH_HARD()
    {
        m_MoveSpeed = 8 * CharStat.Speed;
        if(!Skill_Hide)
            CharStat.Stamina -= (4 * CharStat.SteminaRecovery);
        PlayerMove();
        StaminaCheck();
    }
    void FixedUpdate_SHOT_READY()
    {
        PlayerAiming();
    }
    void FixedUpdate_SHOT_FIRE()
    {

    }
    void FixedUpdate_DAMAGE()
    {
    }
    void FixedUpdate_DEADEYE()
    {
        cam.transform.position = CamPos + CamLook.transform.position;
    }
    void FixedUpdate_REROAD()
    {

    }
    void FixedUpdate_Roll()
    {
        m_CharCtr.Move((transform.forward + Physics.gravity) * m_RollSpeed * Time.deltaTime);
        cam.transform.position = CamPos + transform.position;
    }
    void PlayerMove()
    {

        transform.rotation = m_MoveJoyStickControl.GetRotateVector();
        //transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
        
        m_CharCtr.Move((transform.forward + Physics.gravity) * m_MoveSpeed * Time.deltaTime);
        //RaycastHit Ground;
        //if (Physics.Raycast(m_GroundCheck.position, Vector3.down, out Ground, 5f))
        //{
        //    //Debug.Log("HitPosition : " + Ground.point);
        //    //Debug.Log("Hitname : " + Ground.transform.name);
        //    transform.position = new Vector3(transform.position.x, Ground.point.y, transform.position.z);
        //}

        cam.transform.position = CamPos + transform.position;
    }

    void PlayerAiming()
    {
        transform.rotation = m_ShotJoyStickControl.GetRotateVector();
    }

    void StaminaCheck()
    {
        if (CharStat.Stamina < 0)
        {
            CharStat.Stamina = 0;
            m_Exhausted = true;
        }

        if (CharStat.Stamina > CharStat.MaxStamina)
        {
            CharStat.Stamina = CharStat.MaxStamina;
            if (m_Exhausted)
            {
                m_Exhausted = false;
            }
        }

        Stamina_bar.fillAmount = CharStat.Stamina / CharStat.MaxStamina;
    }

    //public void SetDeadEyeTimer(float _DeadEyeEndTime)
    //{
    //    m_DeadEyeTimer = _DeadEyeEndTime;

    //    Mul_Manager.SendDeadEyeTimerMessage(m_DeadEyeTimer);
    //}


    //void OnGUI()
    //{
    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(w / 2, 0, 100, 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = 30;
    //    style.normal.textColor = new Color(0.0f, 0.0f, 1.5f, 1.5f);

    //    //string text = string.Format("HP : {0}", HP);
    //    string text = string.Format("My : {0}\nEnemy : {1}", Debug_DeadEyeTimer_Player,Debug_DeadEyeTimer_Enemy);

    //    GUI.Label(rect, text, style);

    //    //Rect Bulletrect = new Rect(w - 300, 0, 100, 100);

    //    //string Bullettext = string.Format("탄창 : {0}/{1}\n탄알 : {2}/{3}", m_UseGun.Bullet_Gun, m_UseGun.MaxBullet_Gun, m_UseGun.Bullet_Hand, m_UseGun.MaxBullet_Hand);


    //    //GUI.Label(Bulletrect, Bullettext, style);
    //}

    IEnumerator ServerUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.13f);
            if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
            {
                if (Mul_Manager == true)
                {
                    Mul_Manager.SendAniStateMessage((int)m_PlayerState);
                }
            }
        }
    }

    IEnumerator StaminaRecoveryDealy()
    {
        yield return new WaitForSeconds(0.5f);
        StaminaRecovery = true;

    }
    IEnumerator DeathZoneDealyTime(float Time)
    {
        yield return new WaitForSeconds(Time);
        DeathZoneDealay = false;
    }

    IEnumerator PlayerWin()
    {
        UI_GameOver.SetActive(true);
        cam.SetActive(false);
        //yield return new WaitForSeconds(1);

        Result.SetTrigger("Win");

        m_PlayerState = LSD.PlayerState.WIN;
        m_PlayerBeforeState = LSD.PlayerState.WIN;

       
        if (NowSceneName == "ZombieScene")
        {
            Result.transform.Find("Effect_Result_Win_01/Effect_WinUI/Images/Color_Text_Win").GetComponent<SpriteRenderer>().sprite = ClearImage;
        }
        else if (NowSceneName == "SurvivalScene0" || NowSceneName == "SurvivalScene1")
        {
            int Rank = Mul_Manager.GetMySurvivalRankNumber();
            Result.transform.Find("Effect_Result_Win_01/Effect_WinUI/Images/Color_Text_Win").GetComponent<SpriteRenderer>().sprite = RankImage[Rank-1];
        }

        anim.SetTrigger("Victory");

        GameObject.Find("BGM").GetComponent<AudioSource>().mute = true;
        GameInfoManager.GetInstance().GameOver = true;
        yield return null;
    }

    public void SelectGun_Revolver()
    {
        m_GunState = LSD.GunState.Revolver;
        m_UseGun = new Gun_Revolver();
        Revlolver.SetActive(true);
        string Path = "Client/Resource_Art/Character/0" + CharIndex.ToString() + "/Animation/Character_BaseModel_Revolver";
        anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(Path, typeof(RuntimeAnimatorController));
        m_GunSelect = true;
        GunPoint = transform.FindChild("Gun_Pointer/Effect_Pointer_01").gameObject;

        WomenGun = Revlolver.GetComponent<SkinnedMeshRenderer>();
    }

    public void SelectGun_ShotGun()
    {
        m_GunState = LSD.GunState.ShotGun;
        m_UseGun = new Gun_ShotGun();
        ShotGun.SetActive(true);
        string Path = "Client/Resource_Art/Character/0" + CharIndex.ToString() + "/Animation/Character_BaseModel_ShotGun";

        anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(Path, typeof(RuntimeAnimatorController));
        m_GunSelect = true;
        GunPoint = transform.FindChild("Gun_Pointer/Effect_Pointer_02").gameObject;

        WomenGun = ShotGun.GetComponent<SkinnedMeshRenderer>();
    }

    public void SelectGun_Musket()
    {
        m_GunState = LSD.GunState.Musket;
        m_UseGun = new Gun_Musket();
        Musket.SetActive(true);

        string Path = "Client/Resource_Art/Character/0" + CharIndex.ToString() + "/Animation/Character_BaseModel_Musket";
        anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(Path, typeof(RuntimeAnimatorController));
        m_GunSelect = true;
        GunPoint = transform.FindChild("Gun_Pointer/Effect_Pointer_03").gameObject;

        WomenGun = Musket.GetComponent<SkinnedMeshRenderer>();
    }


    public void OnExitButton()
    {
        Debug.Log("Exit");
        if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)  //접속중일때
        {
            Mul_Manager.EndGameAndLeaveRoom();
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "ZombieScene"|| UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TutorialScene")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("WaitingRoom");
            // UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    public void OnRematchButton()
    {
        Debug.Log("Rematch");
        if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)  //접속중일때
        {
            Mul_Manager.EndGameAndLeaveRoom();  //멀티는 일단 종료
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "ZombieScene")
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("ZombieScene");
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("ZombieScene");
            // UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    public void OnSkillButton()
    {
        Debug.Log("Skill");
        //스킬 사용 전송
        if (GPGSManager.GetInstance.IsAuthenticated())
        {
            if (NowSceneName != "ZombieScene"&& NowSceneName != "TutorialScene")
            {
                Mul_Manager.SendPlayerSkillOnMessage(true);
            }
        }

        if(NowSceneName == "TutorialScene")
        {
            TutorialText.gameObject.SetActive(true); 
        }

        switch (CharIndex)
        {
            case 0: // 링컨 스킬
                {
                    if(TutorialText != null)
                    {
                        TutorialText.text = "fire blindly for 2 seconds";
                    }

                    StartCoroutine(SkillUse(2));
                    Skill_Fastgun = true;
                    break;
                }
            case 1: // 샬롯 스킬
                {
                    if (TutorialText != null)
                    {
                        TutorialText.text = "transparent for 6 seconds";
                    }
                    StartCoroutine(SkillUse(6));
                    Skill_Hide = true;
                    StartCoroutine(Camouflage());
                    break;
                }
                case 2: // 호그 스킬
                {
                    if (TutorialText != null)
                    {
                        TutorialText.text = "invincible for 4 seconds";
                    }
                    StartCoroutine(SkillUse(4));
                    Skill_Invincibility = true;
                    StartCoroutine(InvincibilitySkill());
                    break;
                }
            case 3: // 엔젤 스킬
                {
                    if (TutorialText != null)
                    {
                        TutorialText.text = "bleeding shot";
                    }
                    //StartCoroutine(SkillUse(3));
                    Skill_BloodBullet = true;
                    Button_SkillOn = true;
                    break;
                }
            
            default:
                break;
        }

        //버튼삭제
        UI_Main.transform.Find("Control/Button_Skill").gameObject.SetActive(false);
    }

    void CharInit()
    {

        GameObject GamePlayObj = GameObject.Find("GamePlayObj");

        UI_Main = GamePlayObj.transform.Find("UI_Main").gameObject;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "ZombieScene")
        {
            //if (GPGSManager.GetInstance.GetMyCharacterNumber() != 100)
            //{
            //    CharIndex = GPGSManager.GetInstance.GetMyCharacterNumber();
            //    Debug.Log("CharIndex" + CharIndex);
            //}
            //else
            //{
            CharIndex = GameInfoManager.GetInstance().SelectIndex;
            Debug.Log("SingleCharIndex" + CharIndex);
            //}
        }
        else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TutorialScene")
        {
            TutorialText = UI_Main.transform.Find("TutorialText").GetComponent<TMPro.TextMeshProUGUI>();
        }
        else
        {
            if (GPGSManager.GetInstance.IsAuthenticated())  //접속중일때
            {
                CharIndex = GPGSManager.GetInstance.GetMyCharacterNumber();
            }
            Mul_Manager = GameObject.Find("MultiGameMananger").GetComponent<MultiGameManager>();
            Mul_Manager.MyCharacter = this.gameObject;
        }
        


        m_MoveJoyStickControl = UI_Main.GetComponentInChildren<MoveJoyStick>(); //움직임 전용 조이스틱
        m_ShotJoyStickControl = UI_Main.GetComponentInChildren<JoyStickCtrl>();  //샷 전용 조이스틱

        m_FirstTouch = m_MoveJoyStickControl.transform.Find("Joystickpad").GetComponent<Image>();
        cam = GameObject.Find("CameraPos");


        UI_GameOver = GamePlayObj.transform.Find("UI_GameOver").gameObject;
        // UI_GameOver.SetActive(false);

        //UI_GameOverText;
        UI_DamageEffect = UI_Main.transform.Find("DamageEffect").gameObject;
        // UI_DamageEffect.SetActive(false);

        //EnemyPos = GamePlayObj.transform.Find("EnemyCharacter");
        HP_bar = UI_Main.transform.Find("LifeHealthLine_Bar").GetComponent<Image>();
        Stamina_bar = UI_Main.transform.Find("Stamina_Bar").GetComponent<Image>();



        DeathZone = GameObject.Find("DeathZone").transform;

        UI_Main.transform.Find("Control/Button_Reroad").GetComponent<Button>().onClick.AddListener(OnReroadButton);
        Button_Roll = UI_Main.transform.Find("Control/Button_Roll").gameObject;
        Button_Roll.GetComponent<Button>().onClick.AddListener(OnRollButton);
        UI_Main.transform.Find("Control/Button_Skill").GetComponent<Button>().onClick.AddListener(OnSkillButton);
        string Path = "Client/UI/InGame/Skill_0" + CharIndex.ToString();
        Sprite sprite = (Sprite)Resources.Load(Path, typeof(Sprite));
        UI_Main.transform.Find("Control/Button_Skill").GetComponent<Image>().sprite = sprite;

        GamePlayObj.transform.Find("UI_GameOver/Image/Button_Exit").GetComponent<Button>().onClick.AddListener(OnExitButton);
        GamePlayObj.transform.Find("UI_GameOver/Image/Button_Rematch").GetComponent<Button>().onClick.AddListener(OnRematchButton);


        Path = "Client/InGamePrefab/Skin/0" + CharIndex.ToString()+"/"+GameInfoManager.GetInstance().SelectSkinIndex.ToString();
        Debug.Log(Path);
        Material Mat = (Material)Resources.Load(Path, typeof(Material));
        Skin.material = Mat;
        //anim;
        //UI_Main.SetActive(false);
        // gameObject.SetActive(false);
    }

    IEnumerator SkillUse(int SkillTime)
    {
        
        if(TutorialText != null)
        {
            Debug.Log("SkillStart");
            TutorialText.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(SkillTime);

        Skill_Fastgun = false;
        Skill_Hide = false;
        //Skill_BloodBullet = false;
        Skill_Invincibility = false;
        if (GPGSManager.GetInstance.IsAuthenticated())
        {
            if (NowSceneName != "ZombieScene" && NowSceneName != "TutorialScene")
            {
                Mul_Manager.SendPlayerSkillOnMessage(false);
            }
        }
            
        Debug.Log("SkillEnd");
        if (TutorialText != null)
        {
            UI_Main.transform.Find("Control/Button_Skill").gameObject.SetActive(true);
            TutorialText.gameObject.SetActive(false);
        }
        yield return null;
    }

    public IEnumerator BleedingDamage()
    {
        if (GPGSManager.GetInstance.IsAuthenticated())
            Mul_Manager.SendPlayerBleedOutMessage(true);
        BloodEffectsManager.GetInstance().BloodEffectOn(gameObject);

        for (int i = 0; i < 5; i++)
        {
            CharStat.HP -= 4;

            HP_bar.fillAmount = (float)CharStat.HP / CharStat.MaxHP;

            if (!DeadCheck())
            {
                
            }


            if (GPGSManager.GetInstance.IsAuthenticated() && Mul_Manager != null)
            {
                Mul_Manager.SendMyPositionUpdate();
                Mul_Manager.SendAniStateMessage((int)m_PlayerState);//서버 전송
                Mul_Manager.SendHPStateMessage(CharStat.HP);              
            }
            yield return new WaitForSeconds(1);
        }
        if (GPGSManager.GetInstance.IsAuthenticated())
            Mul_Manager.SendPlayerBleedOutMessage(false);
        yield return null;

    }

    public IEnumerator Camouflage()
    {
        Material Skin = WomenSkin.material;
        Material Gun = WomenGun.material;
        int layer = WomenSkin.gameObject.layer;
        WomenSkin.gameObject.layer = 0; //"Defult";

        GameObject effect = Instantiate(CamoEffect);
        effect.transform.position = gameObject.transform.position;
        m_Exhausted = false;

        WomenSkin.material = WomenCamo;
        WomenGun.material = WomenCamo;
        while(true)
        {
            if(m_PlayerState == LSD.PlayerState.DAMAGE || m_PlayerState == LSD.PlayerState.SHOT_READY)
            {
                Skill_Hide = false;
            }


            if (!Skill_Hide)
            {
                WomenSkin.material = Skin;
                WomenGun.material = Gun;
                WomenSkin.gameObject.layer = layer;
                Destroy(effect);
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    public IEnumerator InvincibilitySkill()
    {
        Invincibility = true;
        GameObject effect = Instantiate(InvincibilityEffect);
        effect.transform.position = gameObject.transform.position;
        effect.transform.SetParent(gameObject.transform);
        yield return new WaitForSeconds(4);
        Invincibility = false;       
        Destroy(effect);
    }

    public IEnumerator DeadEye_Tutorial()
    {
        yield return new WaitForSeconds(15);
        DeadEyeTutorial = false;
        cam.transform.position = CamPos + transform.position;
        m_PlayerState = LSD.PlayerState.IDLE;
    }

    public void CharAllInit()
    {
        m_MoveJoyStickControl.PedInit();
        m_ShotJoyStickControl.InitInputVector();
        m_ShotJoyStickControl.InitTouch();
        GunPoint.SetActive(false);
        anim.Play("Idle");
        m_PlayerState = LSD.PlayerState.IDLE;
    }

    void OnDestroy()
    {
        m_UseGun = null;
        m_GunSelect = false;
        GameEnd = false;
    }
}
