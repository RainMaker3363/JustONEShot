using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

enum ZombieState
{
    IDLE,
    WALK,
    ATTACK,
    DAMAGE,
    DEATH
}

public class ZombieMove : MonoBehaviour {

    public Transform GamePlayObj;
    public Transform PlayerPos;
    public GameObject ParentObj;
    NavMeshAgent NvAgent;
    [SerializeField]
    float m_MoveDealy;
    
    float AttackTime;
    [SerializeField]
    float AttackDealy;

    ZombieState Z_State;
    float Distance;
    bool MotionPlay;

  Animator anim;
    

   public int HP;
   public int AttackDamge;

    public bool Die = false;  //에디터 테스트용


    // Use this for initialization
    void Start () {
        GamePlayObj = GameObject.Find("GamePlayObj").transform;
        PlayerPos = GamePlayObj.transform.Find("PlayerCharacter");
        NvAgent = gameObject.GetComponentInParent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        MotionPlay = false;
        AttackDealy = 5;
        StartCoroutine(ZombieMoveSystem(m_MoveDealy));
    }
	
	// Update is called once per frame
	void Update () {
        Distance = Vector3.Distance(transform.position, PlayerPos.position);
        if (!MotionPlay)
        {
            if (HP <= 0)
            {
                Z_State = ZombieState.DEATH;
                ZombieCreateManager.ZombieCount--;
                Destroy(ParentObj);
            }
            else if (Distance > 2.5f)
            {
                Z_State = ZombieState.WALK;
                anim.SetTrigger("Walk");
                NvAgent.Resume();
            }
            else if(Distance <= 2.5f && AttackTime<Time.time)
            {
                AttackTime = Time.time + AttackDealy;
                Z_State = ZombieState.ATTACK;
                transform.LookAt(PlayerPos.position);
                anim.SetTrigger("Attack");
                NvAgent.Stop();
                MotionPlay = true;
            }
            else if (Distance <= 2.5f && AttackTime > Time.time)
            {
                Z_State = ZombieState.IDLE;
                anim.SetTrigger("Idle");
                NvAgent.Stop();
               
            }
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
            // m_Distance = 0;
        }
    }
   
    public void ZombieDamage(int Damage)
    {
        HP -= Damage;
        if(HP<=0)
        {
            Z_State = ZombieState.DEATH;
            
            anim.SetTrigger("Death");
        }
        else
        {
            Z_State = ZombieState.DAMAGE;
            anim.SetTrigger("Damage");
        }
        NvAgent.Stop();
        MotionPlay = true;
    }

    public void MotionEnd()
    {
        MotionPlay = false;
    }

    IEnumerator ZombieMoveSystem(float MoveDealy)
    {
        while(true)
        {
            yield return new WaitForSeconds(MoveDealy);
            NvAgent.SetDestination(PlayerPos.position);
        }

        yield return null;
    }

}
