using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DeadEye : MonoBehaviour {

    public GameObject MainUI;
    public GameObject DeadEyeUI;

    public Camera MainCam;

    public Animator Playeranim;

    public Bullet_DeadEye Bullets;   
    public GameObject Effects;
    //총 위치
    [SerializeField]
    Transform[] m_GunTransform;

    public Transform m_EnemyPos;

    //데드아이 연출용 카메라
    public GameObject DeadEyeCamera;
    public GameObject DeadEyeBulletCam;
    public GameObject DeadEyeBulletEndCam;

    public GameObject DeadEyeLoadEffect;

    public GameObject DeadEyeUIBack;

    float DeadEyeBulletEffectTime = 3.27f;  //데드아이 총알나가는 연출 시간
                                            // Use this for initialization

    public GameObject GamePlayObj;

    AudioSource m_AudioSource;
    public AudioClip DeadEyeShotSound;
    public AudioClip DeadEyeJumpSound;
    public AudioClip DeadEyeBackgroundSound;

    void Awake()
    {
        m_AudioSource = gameObject.GetComponent<AudioSource>();
    }
    void Start () {
        if(SceneManager.GetActiveScene().name != "GameScene"&& SceneManager.GetActiveScene().name != "GameScene 1")
            Destroy(gameObject.GetComponent<DeadEye>());
        else
        DeadEyeInit();
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

        //if(CharMove.GameEnd)
        //{
        //    StopCoroutine(DeadEyeEnd());
        //    CharMove.DeadEyeEnd = true;
        //    EnemyMove.DeadEyeEnd = true;
        //    //MainUI.SetActive(true);
        //    //MainCam.gameObject.SetActive(true);
        //    DeadEyeBulletEndCam.SetActive(false);
        //}

    }

    void DeadEyeStart()
    {
        
        DeadEyeCamera.SetActive(true);
        MainCam.gameObject.SetActive(false);
        MainUI.SetActive(false);

        if (GameInfoManager.GetInstance().BackgroundSoundUse)
        {
            GameObject.Find("BGM").GetComponent<AudioSource>().mute = true;
            m_AudioSource.clip = DeadEyeBackgroundSound;
            m_AudioSource.Play();
            m_AudioSource.PlayOneShot(DeadEyeJumpSound);
        }
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

        //if (CharMove.GameEnd)
        //{
        MainUI.SetActive(true);
        MainCam.gameObject.SetActive(true);
        //}
        if (GameInfoManager.GetInstance().BackgroundSoundUse)
        {
            GameObject.Find("BGM").GetComponent<AudioSource>().mute = false;
        }
        DeadEyeBulletEndCam.SetActive(false);
    }

    IEnumerator DeadEyeTime()
    {
        yield return new WaitForSeconds(10);
        //if(CharMove.m_DeadEyeTimer<=0)
        //{
        //    CharMove.m_DeadEyeTimer = 10;
        //}
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
                Bullets.transform.position = m_GunTransform[CharMove.m_UseGun.NowUseGun].position;
                Bullets.transform.LookAt(LookVec);
                Bullets.m_Movespeed = Vector3.Distance(Bullets.transform.position, LookVec)/DeadEyeBulletEffectTime;//거리/시간 = 속도
                //Bullets[i].DistanceInit();

                Bullets.m_Use = true;
                Bullets.gameObject.SetActive(true);

                //이펙트
                Effects.transform.position = m_GunTransform[CharMove.m_UseGun.NowUseGun].position;
                Effects.transform.rotation = this.transform.rotation;
                Effects.SetActive(true);

                if (GameInfoManager.GetInstance().EffectSoundUse)
                {
                    m_AudioSource.Stop();
                    m_AudioSource.clip = null;
                    m_AudioSource.PlayOneShot(DeadEyeShotSound);
                }
               
                break;
            }
          

        }
    }

    void DeadEyeInit()
    {
        GamePlayObj = GameObject.Find("GamePlayObj");


        MainUI = GamePlayObj.transform.Find("UI_Main").gameObject;
        DeadEyeUI = GamePlayObj.transform.Find("DeadEyeInfo/DeadEye").gameObject;
        MainCam = GamePlayObj.transform.Find("CameraPos/Main Camera").GetComponent<Camera>();

        Bullets = GamePlayObj.transform.Find("DeadEyeInfo/DeadEyeBullet/Bullet_DeadEye").GetComponent<Bullet_DeadEye>();
        
        Effects = GamePlayObj.transform.Find("DeadEyeInfo/DeadEyeBullet/Effect_DeadEyeFire").gameObject;

        DeadEyeBulletCam = Bullets.transform.Find("DeadEyeBulletCam").gameObject;
        DeadEyeBulletEndCam = GamePlayObj.transform.Find("DeadEyeInfo/DeadEyeBullet/BulletEndCam").gameObject;
        

        DeadEyeLoadEffect = GamePlayObj.transform.Find("DeadEyeInfo/DeadEye/ObjectAnimation/GunRoll/Effect_DeadEyeLoadComplete").gameObject;
        
        DeadEyeUIBack = GamePlayObj.transform.Find("DeadEyeInfo/DeadEye/Camera/ScreenShotCanvas/ScreenShotImage").gameObject;

        switch (gameObject.tag)
        {
            case "Player":
                {
                    m_EnemyPos = GamePlayObj.transform.Find("EnemyCharacter");
                    break;
                }
            case "Enemy":
                {
                    m_EnemyPos = GamePlayObj.transform.Find("PlayerCharacter");
                    break;
                }
            default:
                break;
        }

        //if (gameObject.tag == "Enemy")
        //{
        //    DeadEyeBulletCam.SetActive(false);
        //    DeadEyeBulletEndCam.SetActive(false);
        //    Bullets.gameObject.SetActive(false);
        //    DeadEyeLoadEffect.SetActive(false);
        //    DeadEyeUIBack.SetActive(false);
        //    DeadEyeUI.SetActive(false);
        //}
    }

}
