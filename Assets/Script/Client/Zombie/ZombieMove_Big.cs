﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;



public class ZombieMove_Big : Zombie
{
    public Transform GamePlayObj;
    public Transform PlayerPos;
    public GameObject ParentObj;
    public NavMeshAgent NvAgent;
    [SerializeField]
    float m_MoveDealy;

    float AttackTime;
    [SerializeField]
    float AttackDealy;

    ZombieState Z_State;
    float Distance;
    bool MotionPlay;

    Animator anim;
    Animator CamAnim;

    IEnumerator Dealy_Coroutine;

    public int HP;
    public int AttackDamge;

    public bool Die = false;  //에디터 테스트용

    public SkinnedMeshRenderer shader;

    public AudioClip HitSound;
   // public AudioClip BumpSound;
    public AudioClip DeadSound;
    //public AudioClip IdleSound;
    public AudioClip BloodSound;

    public AudioSource m_AudioSource;

    public GameObject EarthEffect;

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
        AttackDealy = 5;
        

        //m_AudioSource = gameObject.GetComponentInParent<AudioSource>();
        CamAnim = GameObject.Find("CameraPos").GetComponent<Animator>();
        EarthEffect.transform.SetParent(null);  //이펙트를 빼냄
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
            else if (Distance <= 3.5f && AttackTime < Time.time)
            {
                AttackTime = Time.time + AttackDealy;
                Z_State = ZombieState.ATTACK;
                transform.LookAt(PlayerPos.position);
                anim.SetTrigger("Attack");
                NvAgent.Stop();
                MotionPlay = true;
               // m_AudioSource.PlayOneShot(BumpSound);
            }
            else if (Distance <= 3.5f && AttackTime > Time.time)
            {
                Z_State = ZombieState.IDLE;
                anim.SetTrigger("Idle");
                NvAgent.Stop();

            }
        }
        if (CharMove.CharStat.HP <= 0)
        {
            m_AudioSource.enabled = false;
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
            col.gameObject.GetComponent<CharMove>().Damaged(AttackDamge, transform.forward);
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

            anim.SetTrigger("Death");
            m_AudioSource.PlayOneShot(DeadSound);
            MotionPlay = true;
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

    public void EarthQuake()
    {
        CamAnim.SetTrigger("Effect");
        EarthEffect.transform.position = this.transform.position;
        EarthEffect.transform.rotation = this.transform.rotation;
        EarthEffect.SetActive(true);
    }
    //void OnDestroy()
    //{
    //    Destroy(EarthEffect);
    //}

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

