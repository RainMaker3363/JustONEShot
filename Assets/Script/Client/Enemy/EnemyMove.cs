using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour {

  // public MoveJoyStick m_MoveJoyStickControl;  //움직임 전용 조이스틱
    //public JoyStickCtrl m_ShotJoyStickControl;  //샷 전용 조이스틱

    public Animator anim;

    //이동 속도?
    //public Image m_FirstTouch;

    //public Camera cam;
    //private Vector3 CamPos;

    // 플레이어의 움직임
    private float m_MoveSpeed;
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
    public static UseGun m_EnemyUseGun;

    //캐릭터 stat
    int Stamina = 1000;
    [SerializeField]
    int HP = 100;
    int BeforeHP = 100;


    //스테미나가 전부 소모된 상태
    bool m_Exhausted = false;

    Vector3 DamageVec;

    //총 쐈는지 여부
    //bool GunFire = false;

    ////데미지모션 여부
    //bool m_Damaged = false;
    
    // 게임 끝의 여부
    public bool GameEndOn;

    bool m_ShootSuccess;

    bool m_DeadEyePlay;

    bool m_AniPlay = false;

    static bool PlayerDeadEyeStart = false;
    // static bool EnemyDeadEyeStart = false;
    public static bool DeadEyeEnd = false;

    public Transform PlayerPos;  //  적 캐릭터 위치 추후변경예상

    public static float m_DeadEyeTimer;

    [SerializeField]
    public GameObject[] Guns;

    public static int m_SelectGun = 100;//서버 초기값

    public GameObject EnemyUI;
    public UnityEngine.UI.Image Hp_Bar;
    private Vector3 HP_BarPos;

    MultiGameManager Mul_Manager;

    /// //////////////////////////////////////////////////////////////////////////////////<summary>
    /// 서버
    /// ///////////////////////////////////////////////////////////////////////////////</summary>
    /// 

    // 적의 위치와 회전값을 보간해주기 위한 정보들..
    private Vector3 _startPos;
    private Vector3 _destinationPos;
    private Quaternion _startRot;
    private Quaternion _destinationRot;

    // 적의 정보를 보간시켜주기 위해 필요한 정보들..
    private float _lastUpdateTime;
    private float _timePerUpdate =  0.13f;
    private float pctDone;

    // 메시지 순서를 알아낼 변수
    private int _lastMessageNum;



    void Awake()
    {
        m_GunState = LSD.GunState.Revolver; //현재는 고정 추후 받아오게함
        Mul_Manager = GameObject.Find("MultiGameMananger").GetComponent<MultiGameManager>();
        Mul_Manager.EnemyCharacter = this.gameObject;
        if (GPGSManager.GetInstance.IsAuthenticated())  //접속중일때
        {            
            CharIndex = Mul_Manager.GetPVPOpponentCharNumber();
        }
    }

    // Use this for initialization
    void Start()
    {
        m_CharCtr = GetComponent<CharacterController>();

        if (GPGSManager.GetInstance.IsAuthenticated())  //접속중일때
        {
            StartCoroutine(WaitSelectEnemyGun());
        }

        PlayerPos = GameObject.Find("GamePlayObj").transform.Find("PlayerCharacter");
        PlayerPos.GetComponent<CharMove>().EnemyPos = this.gameObject.transform;
        //카메라 기본위치 설정
        // CamPos = cam.transform.position;

        //플레이어 선택 총
        //switch (m_GunState)
        //{
        //    case LSD.GunState.Revolver:
        //        {
        //            m_EnemyUseGun = new Gun_Revolver();
        //            break;
        //        }
        //    case LSD.GunState.ShotGun:
        //        {
        //            break;
        //        }
        //    default:
        //        break;
        //}

        // 플레이어 이동 방향 초기화
        m_MoveVector = Vector3.zero;
        m_MoveSpeed = 2.0f;
        m_PlayerDir = Vector3.zero;
        m_PlayerPosBack = Vector3.zero;

        HP_BarPos = Vector3.up;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        ///서버
        //////////////////////////////////////////////////////////////


        GameEndOn = false;

        _lastUpdateTime = Time.time;

        _lastMessageNum = 0;

        // 0.16초마다 적의 위치를 갱신시켜 준다.
        _timePerUpdate = 0.16f;

        // 초기 위치값 갱신
        _startPos = this.transform.position;
        _startRot = this.transform.rotation;

        _destinationPos = this.transform.position;
        _destinationRot = Quaternion.identity;

        EnemyUI.transform.SetParent(null);

    }

    // Update is called once per frame
    void Update()
    {


        //////////////////////////////////////////////////////////////////////////////////////
        //클라
        m_DebugPlayerState = (int)m_PlayerState;
        // Debug.Log("VectorForce: " + m_MoveJoyStickControl.GetVectorForce());
        Debug.Log("PlayerState: " + m_PlayerState);

        if (m_PlayerBeforeState != m_PlayerState)
        {
            CharAniInit();
            m_PlayerBeforeState = m_PlayerState;
        }

        DeadEyeCheck();
        DamageCheck();

        EnemyUI.transform.position = transform.position;
        //if (m_PlayerState != LSD.PlayerState.REROAD && m_PlayerState != LSD.PlayerState.DAMAGE && m_ShotJoyStickControl.GetTouch())
        //{
        //    //    if(m_PlayerState != LSD.PlayerState.SHOT_FIRE)        
        //    m_PlayerState = LSD.PlayerState.SHOT_READY;
        //}

        switch (m_PlayerState)
        {
            case LSD.PlayerState.IDLE:
                {
                    anim.SetInteger("DashLevel", 0);
                    Update_IDLE();
                    break;
                }
            case LSD.PlayerState.DASH_SLOW:
                {
                    anim.SetInteger("DashLevel", 1);
                    // m_FirstTouch.color = Color.red;
                   Update_DASH_SLOW();
                    break;
                }
            case LSD.PlayerState.DASH_SOFT:
                {
                    anim.SetInteger("DashLevel", 2);
                    //m_FirstTouch.color = Color.green;
                    Update_DASH_SOFT();
                    break;
                }
            case LSD.PlayerState.DASH_HARD:
                {
                    anim.SetInteger("DashLevel", 3);
                    //m_FirstTouch.color = Color.blue;
                    Update_DASH_HARD();
                    break;
                }
            case LSD.PlayerState.SHOT_READY:
                {
                    if (!m_AniPlay)
                    {
                        anim.Play("Shot_Ready");
                        m_AniPlay = true;   //CharAniInit에서 false로 바꿔줌
                    }
                    anim.SetBool("ShotReady", true);
                    if (!anim.GetBool("GunFire"))
                    {
                        anim.SetBool("GunFire", true);
                    }

                    Update_SHOT_READY();
                    break;
                }
            case LSD.PlayerState.SHOT_FIRE:
                {
                    
                    anim.SetBool("ShotReady", false);
                    Update_SHOT_FIRE();
                    break;
                }
            case LSD.PlayerState.DAMAGE:
                {
                    Update_DAMAGE();
                    break;
                }
            case LSD.PlayerState.DEADEYE:
                {
                    Update_DEADEYE();
                    break;
                }
            case LSD.PlayerState.REROAD:
                {
                    // anim.Play("Reloading");
                    anim.SetBool("Reloading", true);
                    Update_REROAD();
                    break;
                }
            case LSD.PlayerState.ROLL:
                {
                    if (!m_AniPlay) 
                    {
                        anim.Play("Roll");
                        m_AniPlay = true;   //CharAniInit에서 false로 바꿔줌
                        anim.SetBool("Rolling", true);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌
                    }
                    Update_Roll();
                    break;
                }
            case LSD.PlayerState.DEAD:
                {
                    if (!m_AniPlay)
                    {
                        anim.SetTrigger("Death");
                        m_PlayerBeforeState = m_PlayerState;
                        m_AniPlay = true;   //CharAniInit에서 false로 바꿔줌                       
                    }
                    break;
                }
            case LSD.PlayerState.WIN:
                {
                    if (!m_AniPlay)
                    {
                        anim.SetTrigger("Victory");
                        m_PlayerBeforeState = m_PlayerState;
                        m_AniPlay = true;   //CharAniInit에서 false로 바꿔줌                       
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    // 적의 타임아웃(Time-Out)의 여부를 확인할때 쓴다.
    // 네트워크 상태가 안좋거나 기타 여부로 체크가 안되면 종료되게 해준다.
    public float lastUpdateTime
    {
        get
        {
            return _lastUpdateTime;
        }
    }

    // 적의 위치값을 갱신시켜 준다.
    public void SetTransformInformation(int messageNum, float posX, float posY, float posZ, float rotY)
    {
        if (messageNum <= _lastMessageNum)
        {
            // Discard any out of order messages
            return;
        }

        _lastMessageNum = messageNum;

        _startPos = this.transform.position;
        _startRot = this.transform.rotation;

        _destinationPos = new Vector3(posX, posY, posZ);
        _destinationRot = Quaternion.Euler(0, rotY, 0);

        _lastUpdateTime = Time.time;
    }

    public void SetEndGameInformation(bool GameEnd)
    {
        GameEndOn = GameEnd;
    }

    // 게임을 나가면 해당 플레이어의 모습을 감춘다.
    public void GameOutInformation()
    {
        this.gameObject.SetActive(false);

        GameEndOn = true;
    }

    void CharAniInit()
    {
        m_AniPlay = false;
        if (anim.GetBool("Reloading"))
        {
            anim.SetBool("Reloading", false);
        }
    }
    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////
    /// </summary>
    #region Update_State
    void Update_IDLE()
    {


        //if (!m_Exhausted)//탈진상태가 아니라면
        //{
        //    if (m_MoveJoyStickControl.GetVectorForce() > 0)
        //    {
        //        m_PlayerState = LSD.PlayerState.DASH_SOFT;
        //    }
        //    else if (m_MoveJoyStickControl.GetVectorForce() > 0.5f)
        //    {
        //        m_PlayerState = LSD.PlayerState.DASH_HARD;
        //    }
        //}
        //else //탈진상태라면
        //{
        //    if (m_MoveJoyStickControl.GetVectorForce() > 0)
        //    {
        //        m_PlayerState = LSD.PlayerState.DASH_SLOW;
        //    }
        //}


    }

    void Update_DASH_SLOW()
    {
        //if (m_MoveJoyStickControl.GetVectorForce() > 0) // 계속 조작중인경우
        //{

        //    if (!m_Exhausted) //탈진상태에서 회복됬다면
        //    {
        //        m_PlayerState++;    //Player상태를 DASH_SOFT로 변경
        //                            //m_PlayerState = LSD.PlayerState.DASH_SOFT;
        //    }
        //}
        //else //조작을 멈출경우
        //{
        //    m_PlayerState--;//Player상태를 IDLE로 변경
        //                    //m_PlayerState = LSD.PlayerState.IDLE;
        //}
    }

    void Update_DASH_SOFT()
    {
        //if (!m_Exhausted)//탈진상태가 아니라면
        //{

        //    if (m_MoveJoyStickControl.GetVectorForce() > 0.5f)
        //    {
        //        m_PlayerState = LSD.PlayerState.DASH_HARD;
        //    }
        //    else if (m_MoveJoyStickControl.GetVectorForce() > 0)
        //    {
        //        m_PlayerState = LSD.PlayerState.DASH_SOFT;
        //    }
        //    else //조작을 멈출경우
        //    {
        //        m_PlayerState = LSD.PlayerState.IDLE;
        //    }

        //}
        //else //탈진상태라면
        //{
        //    if (m_MoveJoyStickControl.GetVectorForce() > 0)
        //    {
        //        m_PlayerState = LSD.PlayerState.DASH_SLOW;
        //    }
        //}
    }

    void Update_DASH_HARD()
    {
        //if (!m_Exhausted)//탈진상태가 아니라면
        //{

        //    if (m_MoveJoyStickControl.GetVectorForce() > 0.5f)
        //    {
        //        m_PlayerState = LSD.PlayerState.DASH_HARD;
        //    }
        //    else if (m_MoveJoyStickControl.GetVectorForce() > 0)
        //    {
        //        m_PlayerState = LSD.PlayerState.DASH_SOFT;
        //    }
        //    else //조작을 멈출경우
        //    {
        //        m_PlayerState = LSD.PlayerState.IDLE;
        //    }
        //}
        //else //탈진상태라면
        //{
        //    if (m_MoveJoyStickControl.GetVectorForce() > 0)
        //    {
        //        m_PlayerState = LSD.PlayerState.DASH_SLOW;
        //    }
        //}
    }

    void Update_SHOT_READY()
    {
        //조준중에 손을 땠다면
        //if (!m_ShotJoyStickControl.GetTouch())
        //{
        //    //if (m_UseGun.Bullet_Gun > 0)
        //    //{
        //    anim.SetBool("GunFire", true);

        //    m_PlayerState = LSD.PlayerState.SHOT_FIRE;
        //    // }

        //}
        

    }

    void Update_SHOT_FIRE()
    {
        if (anim.GetBool("GunFire"))
        {
            anim.SetBool("Shot", m_ShootSuccess);
        }
        else if (!anim.GetBool("GunFire"))
        {
            m_PlayerState = LSD.PlayerState.IDLE;
        }
        
    }

    void Update_DAMAGE()
    {
        if (!anim.GetBool("Damaged"))
        {
           // anim.SetBool("DeadEyeDamage", false);
            m_PlayerState = LSD.PlayerState.IDLE;
        }
    }

    void Update_DEADEYE()
    {
        //anim.SetBool("DeadEyeDamage", CharMove.DeadEyeSuccess);
        if (DeadEyeEnd)
        {
            DeadEyeEnd = false;
           
            m_PlayerState = LSD.PlayerState.IDLE;
        }
    }

    void Update_REROAD()
    {
        //if (m_MoveJoyStickControl.GetVectorForce() > 0)
        //{
        //    anim.SetBool("Reloading", false);
        //    m_PlayerState = LSD.PlayerState.IDLE;
        //}

        //if (m_UseGun.Bullet_Hand > 0 && m_UseGun.MaxBullet_Gun > m_UseGun.Bullet_Gun)//탄알이 있고 탄창이 안찼을때
        //{

        //}
        //else
        //{
        //    anim.SetBool("Reloading", false);
        //    m_PlayerState = LSD.PlayerState.IDLE;
        //}
    }

    void Update_Roll()
    {

        //if (!anim.GetBool("Rolling"))
        //{
        //    //StopCoroutine(StaminaRecoveryDealy());
        //    //StartCoroutine(StaminaRecoveryDealy());
        //    m_PlayerState = LSD.PlayerState.IDLE;
        //}
        //else
        //{
        //   // StaminaRecovery = false;
        //}
    }

    #endregion Update_State

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////
    /// </summary>
    //public void OnReroadButton()
    //{
    //    if (m_PlayerState == LSD.PlayerState.IDLE)
    //    {
    //        m_PlayerState = LSD.PlayerState.REROAD;
    //    }
    //}

    public void Damaged(int Damage, Vector3 vec) //데미지 모션, 매개변수로 데미지와 방향벡터를 가져옴
    {
        Vector3 DamageVec = -vec; //forword를 가져오므로 반대방향을볼수있게 -를 붙임
        DamageVec.y = 0; //위아래로는 움직이지 않게합니다

        transform.rotation = Quaternion.LookRotation(DamageVec);

        Debug.Log(Damage);
        Debug.Log("Damaged");
        if (!DeadCheck())
        {
            anim.SetTrigger("Damage");
            anim.SetBool("Damaged", true);
        }
        //HP -= Damage;

        m_PlayerState = LSD.PlayerState.DAMAGE;
    }
    public void DamageCheck()
    {
        if(BeforeHP != HP)
        {
            //if (DamageVec != Vector3.zero)
            //{
            //    transform.rotation = Quaternion.LookRotation(DamageVec);

                if (!DeadCheck())   //죽은경우 true반환
                {
                    //anim.SetTrigger("Damage");
                    //anim.SetBool("Damaged", true);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌
                }
           // }
            BeforeHP = HP;
            Hp_Bar.fillAmount = (float)HP / 100;
            DamageVec = Vector3.zero;
        }
    }
    public void DeadEyeDamaged(int Damage, Vector3 vec) //데미지 모션, 매개변수로 데미지와 방향벡터를 가져옴
    {
        Vector3 DamageVec = -vec; //forword를 가져오므로 반대방향을볼수있게 -를 붙임
        DamageVec.y = 0; //위아래로는 움직이지 않게합니다
        transform.rotation = Quaternion.LookRotation(DamageVec);

        if (!CharMove.DeadEyeSuccess) //데드아이 피격시 성공했다면
        {
            HP -= Damage;
            if (!DeadCheck())
            {
                anim.SetTrigger("Damage");
                anim.SetBool("Damaged", true);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌 
            }
        }
        else
        {
            HP -= (Damage + 45);
            if (!DeadCheck())
            {
                anim.SetTrigger("DeadEyeDamage");
                anim.SetBool("Damaged", true);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌               
            }
        }

        //HP_bar.fillAmount = HP / 100;

        //if (GPGSManager.GetInstance.IsAuthenticated())
        //{
        //    Mul_Manager.SendMyPositionUpdate();
        //    Mul_Manager.SendAniStateMessage((int)m_PlayerState);//서버 전송
        //}
        m_PlayerState = LSD.PlayerState.DAMAGE;
    }
    public static void PlayerDeadEye()    //데드아이 총알을 먹었을경우
    {
        PlayerDeadEyeStart = true;

    }
    //public static void EnemyDeadEye()    //데드아이 총알을 먹었을경우
    //{
    //    EnemyDeadEyeStart = true;

    //}

    public void DeadEyeCheck()
    {
        if (m_DeadEyePlay)
        {
            transform.LookAt(PlayerPos.position);
            anim.SetInteger("DashLevel", 0);
            m_PlayerState = LSD.PlayerState.DEADEYE;
            anim.SetTrigger("DeadEye");
            m_DeadEyePlay = false;
        }

        if (PlayerDeadEyeStart)
        {
            anim.Play("Idle");
            anim.SetInteger("DashLevel", 0);
            m_PlayerState = LSD.PlayerState.DEADEYE;
            PlayerDeadEyeStart = false;
        }
    }

    public bool DeadCheck()
    {
        if (HP <= 0)
        {
            HP = 0;
            m_PlayerState = LSD.PlayerState.DEAD;
            anim.SetBool("Death", true);   
            return true;         
        }
        return false;
    }

    void FixedUpdate()
    {
        /////////////////////////////////////////////////////////////////////////////////////
        //서버
        pctDone = (Time.time - _lastUpdateTime) / _timePerUpdate;

        if (pctDone <= 1.0f)
        {
            transform.position = Vector3.Slerp(_startPos, _destinationPos, pctDone);
            transform.rotation = Quaternion.Slerp(_startRot, _destinationRot, pctDone);
        }


        //////////////////////////////////////////////////////////////////////////////////////
        //클라

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
            //case LSD.PlayerState.REROAD:
            //    {
            //        FixedUpdate_REROAD();
            //        break;
            //    }

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
            Stamina += 7;
        }
        else
        {
            Stamina += 10;
        }
        StaminaCheck();
    }
    void FixedUpdate_DASH_SLOW()    //탈진 달리기
    {
        m_MoveSpeed = 1;
        //Stamina += 7;
        //PlayerMove();
        //StaminaCheck();
    }
    void FixedUpdate_DASH_SOFT()    // 천천히 달리기
    {
        m_MoveSpeed = 5;
        //Stamina += 10;
        //PlayerMove();
        //StaminaCheck();
    }
    void FixedUpdate_DASH_HARD()
    {
        m_MoveSpeed = 8;
        //Stamina -= 4;
        //PlayerMove();
        //StaminaCheck();
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

    }
    //void FixedUpdate_REROAD()
    //{

    //}

    //void PlayerMove()
    //{

    //    transform.rotation = m_MoveJoyStickControl.GetRotateVector();
    //    //transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
    //    m_CharCtr.Move((transform.forward + Physics.gravity) * m_MoveSpeed * Time.deltaTime);
    //    //RaycastHit Ground;
    //    //if (Physics.Raycast(m_GroundCheck.position, Vector3.down, out Ground, 5f))
    //    //{
    //    //    //Debug.Log("HitPosition : " + Ground.point);
    //    //    //Debug.Log("Hitname : " + Ground.transform.name);
    //    //    transform.position = new Vector3(transform.position.x, Ground.point.y, transform.position.z);
    //    //}

    //    // cam.transform.position = CamPos + transform.position;
    //}

    void PlayerAiming()
    {
        //transform.rotation = m_ShotJoyStickControl.GetRotateVector();
    }

    void StaminaCheck()
    {
        if (Stamina < 0)
        {
            Stamina = 0;
            m_Exhausted = true;
        }

        if (Stamina > 1000)
        {
            Stamina = 1000;
            if (m_Exhausted)
            {
                m_Exhausted = false;
            }
        }
    }

    public void SetAniStateReceived(int AniState)
    {
        m_PlayerState = (LSD.PlayerState)AniState;
    }

    public void SetShootStateReceived(bool ShootSuccess)
    {
        m_ShootSuccess = ShootSuccess;
    }
    public void SetDeadEyeStateReceived(bool DeadEyeOn)
    {
        m_DeadEyePlay = DeadEyeOn;
    }

    public void SetDeadEyeTimerReceived(float _DeadEyeTimer)
    {
        m_DeadEyeTimer = _DeadEyeTimer;
    }
    public void SetHPStateReceived(int _HPState)
    {
        HP = _HPState;
    }

    public void SetShootVectorReceived(float x, float y, float z)
    {
        DamageVec.x = x;
        DamageVec.y = y;
        DamageVec.z = z;
    }
    public void SetShootVectorReceived(Vector3 _vec)
    {
        DamageVec = _vec;
    }

    public void SetCharacterSelectState(int CharacterNumber)
    {

    }

    //void OnGUI()
    //{
        
        
    //    MultiGameManager Mul_Manager = GameObject.Find("MultiGameMananger").GetComponent<MultiGameManager>();
       

    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(w / 2,h/2, 100, 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = 30;
    //    style.normal.textColor = new Color(0.0f, 0.0f, 1.5f, 1.5f);

    //    string text;
    //    //string text = string.Format("HP : {0}", HP);

    //    if (GPGSManager.GetInstance.IsAuthenticated())
    //    {
    //        if(Mul_Manager != null)
    //        {
    //            if(Mul_Manager.MyCharacter == null)
    //            {
    //                text = string.Format("MultiGameManager Init Trouble");
    //            }
    //            else
    //            {
    //                text = string.Format("EmemyName : {0}\nPlayerName : {1}\nMultyReady : {2}", Mul_Manager.MyCharacter.transform.name, Mul_Manager.MyCharacter.transform.name, Mul_Manager._multiplayerReady);
    //            }
                
    //        }
    //        else
    //        {
    //            text = string.Format("GPGS Manager Alive, But MultiManager is Trouble");
    //        }
            
    //    }
    //    else
    //    {
    //        text = string.Format("GPGS Manager Missing");
    //    }
        

    //    GUI.Label(rect, text, style);

    //}

    IEnumerator WaitSelectEnemyGun()
    {

        while (true)
        {
            if(SetEnemyGun())
            {
                yield break;
            }

            yield return new WaitForEndOfFrame();
           
        }

    }


    public bool SetEnemyGun()
    {       
        m_SelectGun = Mul_Manager.GetPVPOpponentGunNumber(); 
        if (m_SelectGun != 100)
        {
            switch (m_SelectGun)
            {
                case 0:
                    {
                        SelectGun_Revolver();
                        break;
                    }
                case 1:
                    {
                        SelectGun_ShotGun();
                        break;
                    }
                case 2:
                    {
                        SelectGun_Musket();
                        break;
                    }
                default:
                    break;
            }
            return true;
        }

        return false;
    }

    public void SelectGun_Revolver()
    {
        m_GunState = LSD.GunState.Revolver;
        m_EnemyUseGun = new Gun_Revolver();
        Guns[0].SetActive(true);
        Guns[1].SetActive(false);
        Guns[2].SetActive(false);
        

        string Path = "Client/Resource_Art/Character/0"+ CharIndex + "/Animation/Character_BaseModel_Revolver";

        anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(Path, typeof(RuntimeAnimatorController));
        //Mul_Manager.SendWeaponNumberMessage(0);
    }

    public void SelectGun_ShotGun()
    {
        m_GunState = LSD.GunState.ShotGun;
        m_EnemyUseGun = new Gun_ShotGun();
        Guns[0].SetActive(false);
        Guns[1].SetActive(true);
        Guns[2].SetActive(false);
        
        string Path = "Client/Resource_Art/Character/0" + CharIndex + "/Animation/Character_BaseModel_ShotGun";
        anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(Path, typeof(RuntimeAnimatorController));
        //Mul_Manager.SendWeaponNumberMessage(1);
    }

    public void SelectGun_Musket()
    {
        m_GunState = LSD.GunState.Musket;
        m_EnemyUseGun = new Gun_Musket();
        Guns[0].SetActive(false);
        Guns[1].SetActive(false);
        Guns[2].SetActive(true);
        
        string Path = "Client/Resource_Art/Character/0" + CharIndex + "/Animation/Character_BaseModel_Musket";

        anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(Path, typeof(RuntimeAnimatorController));

    }

    void OnDestroy()
    {
        m_SelectGun = 100; //static이니 파괴됬을때 초기화
        m_EnemyUseGun = null;
    }
}
