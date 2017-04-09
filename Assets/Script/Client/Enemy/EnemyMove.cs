﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour {

    //public MoveJoyStick m_MoveJoyStickControl;  //움직임 전용 조이스틱
    public JoyStickCtrl m_ShotJoyStickControl;  //샷 전용 조이스틱

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
    LSD.GunState m_GunState;

    //캐릭터 총
    public static UseGun m_EnemyUseGun;

    //캐릭터 stat
    int Stamina = 1000;
    [SerializeField]
    int HP = 100;


    //스테미나가 전부 소모된 상태
    bool m_Exhausted = false;

    //총 쐈는지 여부
    //bool GunFire = false;

    ////데미지모션 여부
    //bool m_Damaged = false;

    void Awake()
    {
        m_GunState = LSD.GunState.Revolver; //현재는 고정 추후 받아오게함
    }

    // Use this for initialization
    void Start()
    {
        m_CharCtr = GetComponent<CharacterController>();

        //카메라 기본위치 설정
       // CamPos = cam.transform.position;

        //플레이어 선택 총
        switch (m_GunState)
        {
            case LSD.GunState.Revolver:
                {
                    m_EnemyUseGun = new Gun_Revolver();
                    break;
                }
            case LSD.GunState.ShotGun:
                {
                    break;
                }
            default:
                break;
        }

        // 플레이어 이동 방향 초기화
        m_MoveVector = Vector3.zero;
        m_MoveSpeed = 2.0f;
        m_PlayerDir = Vector3.zero;
        m_PlayerPosBack = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

        // Debug.Log("VectorForce: " + m_MoveJoyStickControl.GetVectorForce());
        Debug.Log("PlayerState: " + m_PlayerState);

        if (m_PlayerState != LSD.PlayerState.REROAD && m_PlayerState != LSD.PlayerState.DAMAGE && m_ShotJoyStickControl.GetTouch())
        {
            //    if(m_PlayerState != LSD.PlayerState.SHOT_FIRE)        
            m_PlayerState = LSD.PlayerState.SHOT_READY;
        }

        switch (m_PlayerState)
        {
            case LSD.PlayerState.IDLE:
                {
                    anim.SetInteger("DashLevel", 0);
                    Update_IDLE();
                    break;
                }
            //case LSD.PlayerState.DASH_SLOW:
            //    {
            //        anim.SetInteger("DashLevel", 1);
            //       // m_FirstTouch.color = Color.red;
            //        Update_DASH_SLOW();
            //        break;
            //    }
            //case LSD.PlayerState.DASH_SOFT:
            //    {
            //        anim.SetInteger("DashLevel", 2);
            //        //m_FirstTouch.color = Color.green;
            //        Update_DASH_SOFT();
            //        break;
            //    }
            //case LSD.PlayerState.DASH_HARD:
            //    {
            //        anim.SetInteger("DashLevel", 3);
            //        //m_FirstTouch.color = Color.blue;
            //        Update_DASH_HARD();
            //        break;
            //    }
            case LSD.PlayerState.SHOT_READY:
                {
                    anim.Play("Shot_Ready");
                    anim.SetBool("ShotReady", true);

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
            //case LSD.PlayerState.DEADEYE:
            //    {
            //        Update_DEADEYE();
            //        break;
            //    }
            //case LSD.PlayerState.REROAD:
            //    {
            //        // anim.Play("Reloading");
            //        anim.SetBool("Reloading", true);
            //        Update_REROAD();
            //        break;
            //    }

            default:
                {
                    break;
                }
        }
    }


    void Update_IDLE()
    {

        if (!m_Exhausted)//탈진상태가 아니라면
        {
            //if (m_MoveJoyStickControl.GetVectorForce() > 0)
            //{
            //    m_PlayerState = LSD.PlayerState.DASH_SOFT;
            //}
            //else if (m_MoveJoyStickControl.GetVectorForce() > 0.5f)
            //{
            //    m_PlayerState = LSD.PlayerState.DASH_HARD;
            //}
        }
        else //탈진상태라면
        {
            //if (m_MoveJoyStickControl.GetVectorForce() > 0)
            //{
            //    m_PlayerState = LSD.PlayerState.DASH_SLOW;
            //}
        }


    }

    //void Update_DASH_SLOW()
    //{
    //    if (m_MoveJoyStickControl.GetVectorForce() > 0) // 계속 조작중인경우
    //    {

    //        if (!m_Exhausted) //탈진상태에서 회복됬다면
    //        {
    //            m_PlayerState++;    //Player상태를 DASH_SOFT로 변경
    //                                //m_PlayerState = LSD.PlayerState.DASH_SOFT;
    //        }
    //    }
    //    else //조작을 멈출경우
    //    {
    //        m_PlayerState--;//Player상태를 IDLE로 변경
    //                        //m_PlayerState = LSD.PlayerState.IDLE;
    //    }
    //}

    //void Update_DASH_SOFT()
    //{
    //    if (!m_Exhausted)//탈진상태가 아니라면
    //    {

    //        if (m_MoveJoyStickControl.GetVectorForce() > 0.5f)
    //        {
    //            m_PlayerState = LSD.PlayerState.DASH_HARD;
    //        }
    //        else if (m_MoveJoyStickControl.GetVectorForce() > 0)
    //        {
    //            m_PlayerState = LSD.PlayerState.DASH_SOFT;
    //        }
    //        else //조작을 멈출경우
    //        {
    //            m_PlayerState = LSD.PlayerState.IDLE;
    //        }

    //    }
    //    else //탈진상태라면
    //    {
    //        if (m_MoveJoyStickControl.GetVectorForce() > 0)
    //        {
    //            m_PlayerState = LSD.PlayerState.DASH_SLOW;
    //        }
    //    }
    //}

    //void Update_DASH_HARD()
    //{
    //    if (!m_Exhausted)//탈진상태가 아니라면
    //    {

    //        if (m_MoveJoyStickControl.GetVectorForce() > 0.5f)
    //        {
    //            m_PlayerState = LSD.PlayerState.DASH_HARD;
    //        }
    //        else if (m_MoveJoyStickControl.GetVectorForce() > 0)
    //        {
    //            m_PlayerState = LSD.PlayerState.DASH_SOFT;
    //        }
    //        else //조작을 멈출경우
    //        {
    //            m_PlayerState = LSD.PlayerState.IDLE;
    //        }
    //    }
    //    else //탈진상태라면
    //    {
    //        if (m_MoveJoyStickControl.GetVectorForce() > 0)
    //        {
    //            m_PlayerState = LSD.PlayerState.DASH_SLOW;
    //        }
    //    }
    //}

    void Update_SHOT_READY()
    {
        //조준중에 손을 땠다면
        if (!m_ShotJoyStickControl.GetTouch())
        {
            //if (m_UseGun.Bullet_Gun > 0)
            //{
            anim.SetBool("GunFire", true);

            m_PlayerState = LSD.PlayerState.SHOT_FIRE;
            // }

        }

    }

    void Update_SHOT_FIRE()
    {
        if (!anim.GetBool("GunFire"))
        {
            m_PlayerState = LSD.PlayerState.IDLE;
        }
        else
        {

        }
    }

    void Update_DAMAGE()
    {
        if (!anim.GetBool("Damaged"))
        {
            m_PlayerState = LSD.PlayerState.IDLE;
        }
    }

    //void Update_DEADEYE()
    //{

    //}

    //void Update_REROAD()
    //{
    //    if (m_MoveJoyStickControl.GetVectorForce() > 0)
    //    {
    //        anim.SetBool("Reloading", false);
    //        m_PlayerState = LSD.PlayerState.IDLE;
    //    }

    //    if (m_UseGun.Bullet_Hand > 0 && m_UseGun.MaxBullet_Gun > m_UseGun.Bullet_Gun)//탄알이 있고 탄창이 안찼을때
    //    {

    //    }
    //    else
    //    {
    //        anim.SetBool("Reloading", false);
    //        m_PlayerState = LSD.PlayerState.IDLE;
    //    }
    //}

    //public void OnReroadButton()
    //{
    //    if (m_PlayerState == LSD.PlayerState.IDLE)
    //    {
    //        m_PlayerState = LSD.PlayerState.REROAD;
    //    }
    //}

    public void Damaged(int Damage, Vector3 vec) //데미지 모션, 매개변수로 데미지와 방향벡터를 가져옴
    {
        Debug.Log(Damage);
        Debug.Log("Damaged");
        anim.SetTrigger("Damage");
        anim.SetBool("Damaged", true);
        HP -= Damage;
        Vector3 DamageVec = -vec; //forword를 가져오므로 반대방향을볼수있게 -를 붙임
        DamageVec.y = 0; //위아래로는 움직이지 않게합니다

        transform.rotation = Quaternion.LookRotation(DamageVec);
        m_PlayerState = LSD.PlayerState.DAMAGE;
    }


    void FixedUpdate()
    {
        switch (m_PlayerState)
        {
            case LSD.PlayerState.IDLE:
                {
                    FixedUpdate_IDLE();
                    break;
                }
            //case LSD.PlayerState.DASH_SLOW:
            //    {
            //        FixedUpdate_DASH_SLOW();
            //        break;
            //    }
            //case LSD.PlayerState.DASH_SOFT:
            //    {
            //        FixedUpdate_DASH_SOFT();
            //        break;
            //    }
            //case LSD.PlayerState.DASH_HARD:
            //    {
            //        FixedUpdate_DASH_HARD();
            //        break;
            //    }
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
    //void FixedUpdate_DASH_SLOW()    //탈진 달리기
    //{
    //    m_MoveSpeed = 1;
    //    Stamina += 7;
    //    PlayerMove();
    //    StaminaCheck();
    //}
    //void FixedUpdate_DASH_SOFT()    // 천천히 달리기
    //{
    //    m_MoveSpeed = 5;
    //    Stamina += 10;
    //    PlayerMove();
    //    StaminaCheck();
    //}
    //void FixedUpdate_DASH_HARD()
    //{
    //    m_MoveSpeed = 8;
    //    Stamina -= 4;
    //    PlayerMove();
    //    StaminaCheck();
    //}
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

    //    cam.transform.position = CamPos + transform.position;
    //}

    void PlayerAiming()
    {
        transform.rotation = m_ShotJoyStickControl.GetRotateVector();
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
}
