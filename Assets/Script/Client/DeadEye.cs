using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEye : MonoBehaviour {

    public GameObject MainUI;
    public GameObject DeadEyeUI;

    public Camera MainCam;

    public Animator Playeranim;

    public Bullet_DeadEye Bullets;   
    public GameObject Effects;
    //총 위치
    public Transform m_GunTransform;

    public Transform m_EnemyPos;

    //데드아이 연출용 카메라
    public GameObject DeadEyeCamera;
    public GameObject DeadEyeBulletCam;
    public GameObject DeadEyeBulletEndCam;

    public GameObject DeadEyeLoadEffect;

    public GameObject DeadEyeUIBack;

    float DeadEyeBulletEffectTime = 3.27f;  //데드아이 총알나가는 연출 시간
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if(!Bullets.m_Use && DeadEyeBulletCam.activeSelf)
        {
            DeadEyeBulletEndCam.transform.position = DeadEyeBulletCam.transform.position;
            DeadEyeBulletEndCam.transform.rotation = DeadEyeBulletCam.transform.rotation;
            DeadEyeBulletCam.SetActive(false);
            DeadEyeBulletEndCam.SetActive(true);
            StartCoroutine(DeadEyeEnd());
        }

    }

    void DeadEyeStart()
    {
        DeadEyeCamera.SetActive(true);
        MainCam.gameObject.SetActive(false);
        MainUI.SetActive(false);
    }

    void DeadEyePlay()
    {
        DeadEyeUI.SetActive(true);
       
        StartCoroutine(DeadEyeTime());
    }

    void DeadEyeBulletMove()
    {
        DeadEyeCamera.SetActive(false);
        DeadEyeBulletCam.SetActive(true);
        //StartCoroutine(DeadEyeBulletEffectEndTime());
    }

    IEnumerator DeadEyeEnd()
    {
        yield return new WaitForSeconds(2f);
        CharMove.DeadEyeEnd = true;
        EnemyMove.DeadEyeEnd = true;
        MainUI.SetActive(true);
        MainCam.gameObject.SetActive(true);
        DeadEyeBulletEndCam.SetActive(false);
    }

    IEnumerator DeadEyeTime()
    {
        yield return new WaitForSeconds(10);

        DeadEyeUI.SetActive(false);
        DeadEyeLoadEffect.SetActive(false); //Roll06 애니메이션에서 켜줍니다
        DeadEyeUIBack.SetActive(false);
        
        //MainCam.gameObject.SetActive(true);     //현재 스크린샷에서 꺼짐

        Playeranim.SetTrigger("DeadEyeEnd");
    }



    void SetBullet_DeadEye()
    {

        while (true)
        {
            if (!Bullets.m_Use)
            {
                //Debug.Log(gameObject.tag);
                //switch (gameObject.tag)
                //{
                //    case "Player":
                //        {
                //            Bullets.Damage = CharMove.m_UseGun.Damage;
                //            CharMove.m_UseGun.UseBullet();
                //            break;
                //        }
                //    case "Enemy":
                //        {
                //            Bullets.Damage = EnemyMove.m_EnemyUseGun.Damage;
                //            EnemyMove.m_EnemyUseGun.UseBullet();
                //            break;
                //        }

                //    default:
                //        break;
                //}

                Vector3 LookVec = m_EnemyPos.position;
                LookVec.y += 1;

                //총알
                Bullets.transform.position = m_GunTransform.position;
                Bullets.transform.LookAt(LookVec);
                Bullets.m_Movespeed = Vector3.Distance(Bullets.transform.position, LookVec)/DeadEyeBulletEffectTime;//거리/시간 = 속도
                //Bullets[i].DistanceInit();

                Bullets.m_Use = true;
                Bullets.gameObject.SetActive(true);

                //이펙트
                Effects.transform.position = m_GunTransform.position;
                Effects.transform.rotation = this.transform.rotation;
                Effects.SetActive(true);
                
               
                break;
            }
          

        }
    }

}
