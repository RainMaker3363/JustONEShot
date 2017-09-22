using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : MonoBehaviour
{

    public Animator anim;

    //LSD.PlayerState m_PlayerState;

    public UnityEngine.UI.Image Hp_Bar;
    private Vector3 HP_BarPos;

    AudioSource m_AudioSource;
    public AudioClip CharHitSound;

    bool m_DeadEyePlay;
    static bool PlayerDeadEyeStart = false;

    bool Heal;
    [SerializeField]
    int HP = 100;
    int MaxHP = 100;
    bool bleeding = false;

    public GameObject DamageEffect;

    public Transform PlayerPos;

    // Use this for initialization
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();

        PlayerPos = GameObject.Find("GamePlayObj").transform.Find("PlayerCharacter");
        if (PlayerPos != null)
            PlayerPos.GetComponent<CharMove>().EnemyPos = this.gameObject.transform;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damaged(int Damage) //데미지 모션, 매개변수로 데미지와 방향벡터를 가져옴
    {
        //if (!Invincibility)
        //{
        //Vector3 DamageVec = -vec; //forword를 가져오므로 반대방향을볼수있게 -를 붙임
        //DamageVec.y = 0; //위아래로는 움직이지 않게합니다
        //transform.rotation = Quaternion.LookRotation(DamageVec);

        //m_PlayerState = LSD.PlayerState.DAMAGE;

        HP -= Damage;

        Hp_Bar.fillAmount = (float)HP / MaxHP;

        DeadCheck();

        anim.SetTrigger("Damage");
        if (GameInfoManager.GetInstance().EffectSoundUse)
        {
            m_AudioSource.PlayOneShot(CharHitSound);
        }



       // Handheld.Vibrate();
        // }

    }

    public void DeadEyeDamaged(int Damage) //데미지 모션, 매개변수로 데미지와 방향벡터를 가져옴
    {
        //Vector3 DamageVec = -vec; //forword를 가져오므로 반대방향을볼수있게 -를 붙임
        //DamageVec.y = 0; //위아래로는 움직이지 않게합니다
        //transform.rotation = Quaternion.LookRotation(DamageVec);

        //if (!CharMove.DeadEyeSuccess) //데드아이 피격시 성공했다면
        //{
        //    HP -= Damage;
        //    DeadCheck();

        //    anim.SetTrigger("Damage");
        //    //    anim.SetBool("Damaged", true);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌 

        //}
        //else
        //{
        HP -= (Damage + 45);
        DeadCheck();
        Hp_Bar.fillAmount = (float)HP / MaxHP;
        anim.SetTrigger("Damage");
        // anim.SetBool("Damaged", true);  //gun 에 있는 함수가 매카님에서 false로 바꿔줌               

        // }

        //HP_bar.fillAmount = HP / 100;

        //if (GPGSManager.GetInstance.IsAuthenticated())
        //{
        //    Mul_Manager.SendMyPositionUpdate();
        //    Mul_Manager.SendAniStateMessage((int)m_PlayerState);//서버 전송
        //}
        if (GameInfoManager.GetInstance().EffectSoundUse)
        {
            m_AudioSource.PlayOneShot(CharHitSound);
        }
        //m_PlayerState = LSD.PlayerState.DAMAGE;
    }

    //public static void PlayerDeadEye()    //데드아이 총알을 먹었을경우
    //{
    //    PlayerDeadEyeStart = true;

    //}

    //public void DeadEyeCheck()
    //{
    //    if (m_DeadEyePlay)
    //    {
    //        //transform.LookAt(PlayerPos.position);
    //        //anim.SetInteger("DashLevel", 0);
    //        m_PlayerState = LSD.PlayerState.DEADEYE;
    //        //anim.SetTrigger("DeadEye");
    //        m_DeadEyePlay = false;
    //    }

    //    if (PlayerDeadEyeStart)
    //    {
    //        anim.Play("Idle");
    //        anim.SetInteger("DashLevel", 0);
    //        m_PlayerState = LSD.PlayerState.DEADEYE;
    //        PlayerDeadEyeStart = false;
    //    }
    //}

    public bool DeadCheck()
    {
        if (HP <= 0)
        {
            HP = 0;
            if (!Heal)
            {
                Heal = true;
                //회복실행
                StartCoroutine(Healling());
            }

            //anim.SetBool("Death", true);
            return true;
        }
        return false;
    }
    public IEnumerator BleedingDamage()
    {
        BloodEffectsManager.GetInstance().BloodEffectOn(gameObject);

        //for (int i = 0; i < 5; i++)
        //{

        yield return new WaitForSeconds(5);
        //}
        //if (GPGSManager.GetInstance.IsAuthenticated())
        //    Mul_Manager.SendPlayerBleedOutMessage(false);
        bleeding = false;
        yield return null;

    }

    IEnumerator Healling()
    {
        while (HP < 100)
        {
            Debug.Log("Heal");
            HP++;
            Hp_Bar.fillAmount = (float)HP / MaxHP;
            yield return new WaitForEndOfFrame();
        }
        Heal = false;
    }
}
