using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class ZombieMove_Boom : Zombie
{

    public Transform GamePlayObj;
    public Transform PlayerPos;
    public GameObject ParentObj;
    NavMeshAgent NvAgent;
    [SerializeField]
    float m_MoveDealy;

    ZombieState Z_State;
    [SerializeField]
    float Distance;
    bool MotionPlay;

    Animator anim;
    Animator CamAnim;

    IEnumerator Dealy_Coroutine;

    public int HP;
    public int AttackDamge;

    public bool Die = false;  //에디터 테스트용

    public SkinnedMeshRenderer shader;
    public GameObject BoomEffect;
    public AudioClip HitSound;
    public AudioClip BoomSound;
    public AudioClip DeadSound;
    //public AudioClip IdleSound;
    public AudioClip BloodSound;

    AudioSource m_AudioSource;

    public GameObject MiniMapPoint;

    // Use this for initialization
    void Awake()
    {
        GamePlayObj = GameObject.Find("GamePlayObj").transform;
        NvAgent = gameObject.GetComponentInParent<NavMeshAgent>();
        PlayerPos = GamePlayObj.transform.Find("PlayerCharacter");
    }

    void Start()
    {
        
        anim = gameObject.GetComponent<Animator>();
        MotionPlay = false;

       

        m_AudioSource = gameObject.GetComponentInParent<AudioSource>();

        CamAnim = GameObject.Find("CameraPos").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR    //유니티에디터에서 실행시킬경우 이쪽코드를 실행
        if (Input.GetKeyDown(KeyCode.Space))
            Die = true;
#endif
        Distance = Vector3.Distance(transform.position, PlayerPos.position);
        if (!MotionPlay)
        {
            if (HP <= 0)
            {
                Z_State = ZombieState.DEATH;
                ZombieCreateManager.ZombieCount--;
                // Destroy(ParentObj);
                ParentObj.SetActive(false);
            }
            else if (Distance > 3.5f)
            {
                Z_State = ZombieState.WALK;
                anim.SetTrigger("Walk");
                NvAgent.Resume();
                //m_AudioSource.PlayOneShot(IdleSound);
            }
            else if (Distance <= 3.5f && Distance > 2.5f)
            {

                if(Z_State != ZombieState.ATTACK)
                {
                    Z_State = ZombieState.ATTACK;
                    transform.LookAt(PlayerPos.position);
                    anim.SetTrigger("Dash");
                    NvAgent.speed = 3.99f;  //기본이 2.1f 기준 40%증가
                    //StartCoroutine(ZombieBoom());
                }
                
                //NvAgent.Stop();
                //MotionPlay = true;
                m_AudioSource.PlayOneShot(BoomSound);
            }
            else if (Distance <= 2.5f)
            {
                ZombieDamage(100);
            }

            }
        if (CharMove.CharStat.HP <= 0)
        {
            m_AudioSource.enabled = false;
            Z_State = ZombieState.IDLE;
            NvAgent.Stop();
            MotionPlay = true;
        }

        if (Die)    //에디터 테스트용
        {
            Die = false;
            ZombieDamage(100);
        }
        switch (Z_State)
        {
            case ZombieState.IDLE:
                {

                }
                break;
            case ZombieState.WALK:
                {

                }
                break;
            case ZombieState.ATTACK:
                {

                }
                break;
            case ZombieState.DAMAGE:
                {

                }
                break;
            case ZombieState.DEATH:
                {

                }
                break;
            default:
                break;
        }


    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            //거리가 1일때 40, 3.5일때 5;
            //거리가 현재 거리가 5일때 최소 1일때 최대
            int Damage = 40;
            if (Distance>3.5f)
            {
                Damage = 5;
            }
            else if (Distance>0 && Distance <= 1.9f)
            {
                //Damage = 40;//55 25
                Damage = AttackDamge;   //attackdamge = 40(보통기준)
            }
            else if (Distance > 1.9f && Distance <= 2.8f)
            {
                //Damage = 25;//40 10
                Damage = AttackDamge-15;
            }
            else if (Distance > 2.8 && Distance <= 3.5f)
            {
                //Damage = 10; //25 5
                Damage = Mathf.Abs(AttackDamge-30);

            }
            // 
            Debug.Log("BoomDamage : " + Damage);
            col.gameObject.GetComponent<CharMove>().Damaged(Damage, transform.forward);
            // m_Distance = 0;
        }
    }

    void OnEnable()
    {
        Dealy_Coroutine = ZombieMoveSystem(m_MoveDealy);
        StartCoroutine(Dealy_Coroutine);
    }

    void OnDisable()
    {
        shader.material.SetFloat("_DissolveEdgeRange", 0);
        StopCoroutine(Dealy_Coroutine);
        NvAgent.enabled = false;
        GetComponent<CapsuleCollider>().enabled = true;
        MiniMapPoint.SetActive(false);
    }

    override public void ZombieDamage(int Damage)
    {
        HP -= Damage;
        if (HP <= 0)
        {
            Z_State = ZombieState.DEATH;
            MotionPlay = true;
            anim.SetTrigger("Death");
            m_AudioSource.PlayOneShot(DeadSound);
        }
        else
        {
            Z_State = ZombieState.DAMAGE;
            anim.SetTrigger("Damage");
            m_AudioSource.PlayOneShot(HitSound);
            m_AudioSource.PlayOneShot(BloodSound);
        }
        StartCoroutine(ZombieDamageEffect());
        NvAgent.Stop();
       
    }

    public void MotionEnd()
    {
        MotionPlay = false;
    }

    public void BoomEffectOn()
    {
        BoomEffect.SetActive(true);
        CamAnim.SetTrigger("Effect");
        NvAgent.Stop();
    }

    IEnumerator ZombieMoveSystem(float MoveDealy)
    {
        while (true)
        {
            yield return new WaitForSeconds(MoveDealy);
            if (!CharMove.Skill_Hide)
                NvAgent.SetDestination(PlayerPos.position);
        }

        yield return null;
    }

    //IEnumerator ZombieBoom()
    //{
        
    //    yield return new WaitForSeconds(3);
    //    ZombieDamage(100);

    //    yield return null;
    //}
    IEnumerator ZombieDamageEffect()
    {
        shader.material.SetFloat("_DissolveEdgeRange", 1);
        yield return new WaitForSeconds(0.1f);
        shader.material.SetFloat("_DissolveEdgeRange", 0);
        yield return null;
    }

    public IEnumerator ZombieFastMoving()
    {
        Distance = Vector3.Distance(transform.position, PlayerPos.position);
        float speed = NvAgent.speed;

        NvAgent.speed = FastMoveSpeed;
        while (!ZombieFastMoveCheck(Distance))
        {

            yield return new WaitForEndOfFrame();
        }

        NvAgent.speed = speed;
        MiniMapPoint.SetActive(true);
        // yield return null;
    }

    public override void Bleeding()
    {
        if (HP > 0)
        {
            StartCoroutine(BleedingDamage());
        }
    }

    IEnumerator BleedingDamage()
    {
        BloodEffectsManager.GetInstance().BloodEffectOn(gameObject);

        for (int i = 0; i < 5; i++)
        {
            HP -= 4;
            if (HP <= 0)
            {
                Z_State = ZombieState.DEATH;

                anim.SetTrigger("Death");
                m_AudioSource.PlayOneShot(DeadSound);
                break;
            }
            yield return new WaitForSeconds(1);
        }

        yield return null;

    }
}
