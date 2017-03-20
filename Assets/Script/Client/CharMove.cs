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
        REROAD

    }
}


public class CharMove : MonoBehaviour {

   
    public MoveJoyStick m_MoveJoyStickControl;  //움직임 전용 조이스틱
    public JoyStickCtrl m_ShotJoyStickControl;  //샷 전용 조이스틱

    public Animator anim;

    //이동 속도?
    public Image m_FirstTouch;

    public Camera cam;
    private Vector3 CamPos;

    // 플레이어의 움직임
    private float m_MoveSpeed;
    private Vector3 m_MoveVector;
    private Vector3 m_PlayerDir;
    //private Vector3 PlayerDeadEyePosBack;
    private Vector3 m_PlayerPosBack;
    public Transform m_GroundCheck;
    private CharacterController m_CharCtr;

    LSD.PlayerState m_PlayerState;

    //캐릭터 stat
    int Stamina = 1000;


    //스테미나가 전부 소모된 상태
    bool m_Exhausted = false;

    // Use this for initialization
    void Start () {

        m_CharCtr = GetComponent<CharacterController>();

        //카메라 기본위치 설정
        CamPos = cam.transform.position;

        // 플레이어 이동 방향 초기화
        m_MoveVector = Vector3.zero;
        m_MoveSpeed = 2.0f;
        m_PlayerDir = Vector3.zero;
        m_PlayerPosBack = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {

        // Debug.Log("VectorForce: " + m_MoveJoyStickControl.GetVectorForce());
        Debug.Log("PlayerState: " + m_PlayerState);
        
        if(m_ShotJoyStickControl.GetTouch())
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
            case LSD.PlayerState.DASH_SLOW:
                {
                    anim.SetInteger("DashLevel", 1);
                    m_FirstTouch.color = Color.red;
                    Update_DASH_SLOW();
                    break;
                }
            case LSD.PlayerState.DASH_SOFT:
                {
                    anim.SetInteger("DashLevel", 2);
                    m_FirstTouch.color = Color.green;
                    Update_DASH_SOFT();
                    break;
                }
            case LSD.PlayerState.DASH_HARD:
                {
                    anim.SetInteger("DashLevel", 3);
                    m_FirstTouch.color = Color.blue;
                    Update_DASH_HARD();
                    break;
                }
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
            //case LSD.PlayerState.DAMAGE:
            //    {
            //        Update_DAMAGE();
            //        break;
            //    }
            //case LSD.PlayerState.DEADEYE:
            //    {
            //        Update_DEADEYE();
            //        break;
            //    }
            //case LSD.PlayerState.REROAD:
            //    {
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

            if (!m_Exhausted) //탈진상태에서 회복됬다면
            {
                m_PlayerState++;    //Player상태를 DASH_SOFT로 변경
                                    //m_PlayerState = LSD.PlayerState.DASH_SOFT;
            }          
        }
        else //조작을 멈출경우
        {
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
                m_PlayerState = LSD.PlayerState.DASH_HARD;
            }
            else if(m_MoveJoyStickControl.GetVectorForce() > 0)
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

    void Update_SHOT_READY()
    {
        //조준중에 손을 땠다면
        if(!m_ShotJoyStickControl.GetTouch())
        {
            m_PlayerState = LSD.PlayerState.SHOT_FIRE;
            
        }
    }

    void Update_SHOT_FIRE()
    {
        m_PlayerState = LSD.PlayerState.IDLE;
    }

    //void Update_DAMAGE()
    //{

    //}

    //void Update_DEADEYE()
    //{

    //}

    //void Update_REROAD()
    //{

    //}




    void FixedUpdate()
    {
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
        m_MoveSpeed = 2;
        Stamina += 7;
        PlayerMove();
        StaminaCheck();
    }
    void FixedUpdate_DASH_SOFT()    // 천천히 달리기
    {
        m_MoveSpeed = 4;
        Stamina += 10;
        PlayerMove();
        StaminaCheck();
    }
    void FixedUpdate_DASH_HARD()
    {
        m_MoveSpeed = 8;
        Stamina -= 4;
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

    }
    void FixedUpdate_REROAD()
    {

    }

    void PlayerMove()
    {

        transform.rotation = m_MoveJoyStickControl.GetRotateVector();
        //transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
        m_CharCtr.Move((transform.forward +Physics.gravity) * m_MoveSpeed * Time.deltaTime);
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

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(w / 2, 0, 100, 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 10;
        style.normal.textColor = new Color(0.0f, 0.0f, 1.5f, 1.5f);

        string text = string.Format("Stamina : {0}", Stamina);

        GUI.Label(rect, text, style);
    }
}
