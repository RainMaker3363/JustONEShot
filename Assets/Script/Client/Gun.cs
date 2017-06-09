using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class UseGun
{
    public int NowUseGun; //현재 사용하는총 0.리볼버 1.샷건 2.머스캣   
    public int MaxBullet_Gun;  //탄창 탄알 최대수
    public int MaxBullet_Hand; // 소지 탄알 최대수
    public int Bullet_Gun;      //탄창 탄알
    public int Bullet_Hand;    //소지 탄알
    public int Bullet_Use;      //소모 탄알
    public int Damage;      //총의 데미지
    public Vector3 Sight;   //총에 따른 카메라위치
    public int GetBulletQuantity =1;
    public float ReloadSpeed = 1;
    public int DamageUpgrade = 0;

    abstract public void UseBullet();
    public void ReloadBullet()  //탄창이 찼을경우는 charmove에서 체크중
    {
        Bullet_Hand--;
        Bullet_Gun++;

    }
    public void GetBullet()
    {
        if (MaxBullet_Hand > Bullet_Hand)
        {
            Bullet_Hand+= GetBulletQuantity;
            if(MaxBullet_Hand< Bullet_Hand)
            {
                Bullet_Hand = MaxBullet_Hand;
            }
        }
    }

}

class Gun_Revolver : UseGun
{
    public Gun_Revolver()
    {
        MaxBullet_Gun = 6;
        MaxBullet_Hand = 10;
        Bullet_Gun = 0;
        Bullet_Hand = 0;
        Bullet_Use = 1;
        Damage = 25;
        NowUseGun = 0;
        Sight = new Vector3(0, 6.6f, -3.9f);
    }

    public override void UseBullet()
    {
        Bullet_Gun -= Bullet_Use;
    }


}
class Gun_ShotGun : UseGun
{
    public Gun_ShotGun()
    {
        MaxBullet_Gun = 2;
        MaxBullet_Hand = 10;
        Bullet_Gun = 0;
        Bullet_Hand = 0;
        Bullet_Use = 2;
        Damage = 35;
        NowUseGun = 1;
        Sight = new Vector3(0, 5.1f, -3);
    }

    public override void UseBullet()
    {
        Bullet_Gun -= Bullet_Use;
    }


}
class Gun_Musket : UseGun
{
    public Gun_Musket()
    {
        MaxBullet_Gun = 1;
        MaxBullet_Hand = 10;
        Bullet_Gun = 0;
        Bullet_Hand = 0;
        Bullet_Use = 1;
        Damage = 45;
        NowUseGun = 2;
        Sight = new Vector3(0, 9, -5.4f);
    }

    public override void UseBullet()
    {
        Bullet_Gun -= Bullet_Use;
    }


}


public class Gun : MonoBehaviour
{

    //총알
    [SerializeField]
    Bullet[] Bullets;

    public Bullet_ShotGun ShotGunBullet;

    [SerializeField]
    GameObject[] Effects;

    [SerializeField]
    GameObject[] Effects_Bullet;

    [SerializeField]
    GameObject[] UI_SillinderBullets;

    [SerializeField]
    GameObject[] UI_ShotGunBullets;

    [SerializeField]
    GameObject UI_MusketBullets;

    [SerializeField]
    Image[] UI_HandsBullets;

    [SerializeField]
    GameObject UI_Sillinder;

    [SerializeField]
    GameObject UI_ShotGun;

    [SerializeField]
    GameObject UI_Musket;

    float SillinerRotate = 0;
    Animator anim;
    public Animator camAni;

    //총 위치
    [SerializeField]
    Transform[] m_GunTransform;
    //탄피 위치
    [SerializeField]
    Transform[] m_BulletTransform;

    int m_BulletIndex = 0;

    //int m_SillinderBulletIndex = 0;
    int m_HandsBulletIndex = 0;
    Quaternion Rotate;

    public GameObject CharRollEffect;

    public GameObject BulletObj;
 
    public TMPro.TextMeshProUGUI UI_HandsBulletsText;

    void Awake()
    {
        GunInit();
    }

    void Start()
    {
        anim = GetComponent<Animator>();

        Debug.Log(UI_HandsBullets.Length);
        Rotate = Quaternion.identity;


        if (gameObject.tag.Equals("Player"))
        {
            switch (CharMove.m_UseGun.NowUseGun)
            {
                case 0:
                    {
                        UI_Sillinder.SetActive(true);
                        break;
                    }
                case 1:
                    {
                        UI_ShotGun.SetActive(true);
                        break;
                    }
                case 2:
                    {
                        UI_Musket.SetActive(true);
                        break;
                    }

                default:
                    break;
            }

        }

        BulletObj.transform.SetParent(null);
    }

    void Update()
    {
        if (gameObject.tag.Equals("Player"))
        {
           
            if (CharMove.m_UseGun.Bullet_Hand > m_HandsBulletIndex)
            {
                //UI_HandsBullets[m_HandsBulletIndex].color = Color.white;
                m_HandsBulletIndex++;
                
            }
            UI_HandsBulletsText.text = CharMove.m_UseGun.Bullet_Hand.ToString()+"/" + CharMove.m_UseGun.MaxBullet_Hand.ToString();
        }

        if (UI_Sillinder != null)
        {
            Rotate.eulerAngles = new Vector3(0, 0, SillinerRotate);
            UI_Sillinder.transform.rotation = Quaternion.Slerp(UI_Sillinder.transform.rotation, Rotate, Time.deltaTime * 5.0f);
        }

    }
    void SetBullet()
    {

        while (true)
        {
            if (!Bullets[m_BulletIndex].m_Use)
            {
                Debug.Log(gameObject.tag);
                int UseGun=100;

                switch (gameObject.tag)
                {
                    case "Player":
                        {
                            UseGun = CharMove.m_UseGun.NowUseGun;
                            switch (UseGun)
                            {
                                case 0:
                                    {
                                        Bullets[m_BulletIndex].Damage = CharMove.m_UseGun.Damage;
                                        Bullets[m_BulletIndex].m_Movespeed = 30;
                                       CharMove.m_UseGun.UseBullet();
                                        UI_SillinderBullets[5 - CharMove.m_UseGun.Bullet_Gun].gameObject.SetActive(false);
                                        SillinerRotate -= 60;
                                        break;
                                    } 
                                case 2:
                                    {
                                        Bullets[m_BulletIndex].Damage = CharMove.m_UseGun.Damage;
                                        Bullets[m_BulletIndex].m_Movespeed = 60;
                                        Bullets[m_BulletIndex].m_Distance += 500;
                                        CharMove.m_UseGun.UseBullet();
                                        UI_MusketBullets.SetActive(false);
                                        
                                        break;
                                    }

                                default:
                                    break;
                            }
                            
                            

                            //UI_Sillinder.transform.Rotate(new Vector3(0, 0, 1), 60);//Mathf.Deg2Rad*SillinerRotate);
                            break;
                        }
                    case "Enemy":
                        {
                            Bullets[m_BulletIndex].Damage = EnemyMove.m_EnemyUseGun.Damage;
                            EnemyMove.m_EnemyUseGun.UseBullet();
                            UseGun = EnemyMove.m_EnemyUseGun.NowUseGun;

                            switch (UseGun)
                            {
                                case 0:
                                    {
                                        Bullets[m_BulletIndex].Damage = EnemyMove.m_EnemyUseGun.Damage;
                                        Bullets[m_BulletIndex].m_Movespeed = 30;
                                        break;
                                    }
                                case 2:
                                    {
                                        Bullets[m_BulletIndex].Damage = EnemyMove.m_EnemyUseGun.Damage;
                                        Bullets[m_BulletIndex].m_Movespeed = 60;
                                        Bullets[m_BulletIndex].m_Distance += 500;
                                        Bullets[m_BulletIndex].Penetrate = true;
                                        break;
                                    }

                                default:
                                    break;
                            }
                            break;
                        }

                    default:
                        break;
                }

              

                //총알
                Bullets[m_BulletIndex].transform.position = m_GunTransform[UseGun].position;
                Bullets[m_BulletIndex].transform.rotation = this.transform.rotation;
                //Bullets[i].DistanceInit();

                Bullets[m_BulletIndex].m_Use = true;
                Bullets[m_BulletIndex].gameObject.SetActive(true);

                //이펙트
                Effects[m_BulletIndex].transform.position = m_GunTransform[UseGun].position;
                Effects[m_BulletIndex].transform.rotation = this.transform.rotation;
                Effects[m_BulletIndex].SetActive(true);
                //탄피
                Effects_Bullet[m_BulletIndex].transform.position = m_BulletTransform[UseGun].position;
                Effects_Bullet[m_BulletIndex].transform.rotation = m_BulletTransform[UseGun].rotation;
                Effects_Bullet[m_BulletIndex].SetActive(true);

                camAni.SetTrigger("Shot");
                m_BulletIndex++;
                if (m_BulletIndex == 3)
                {
                    m_BulletIndex = 0;
                }



                break;
            }
            else
            {
                m_BulletIndex++;
            }

        }
    }

    void SetBullet_ShotGun()
    {


        if (!ShotGunBullet.m_Use)
        {
            Debug.Log(gameObject.tag);
            int UseGun = 100;

            switch (gameObject.tag)
            {
                case "Player":
                    {

                        ShotGunBullet.Damage = CharMove.m_UseGun.Damage;
                        CharMove.m_UseGun.UseBullet();
                        UI_ShotGunBullets[0].gameObject.SetActive(false);
                        UI_ShotGunBullets[1].gameObject.SetActive(false);
                        UseGun = CharMove.m_UseGun.NowUseGun;

                        // UI_SillinderBullets[5 - CharMove.m_UseGun.Bullet_Gun].gameObject.SetActive(false);
                        //SillinerRotate -= 60;


                        //UI_Sillinder.transform.Rotate(new Vector3(0, 0, 1), 60);//Mathf.Deg2Rad*SillinerRotate);
                        break;
                    }
                case "Enemy":
                    {
                        ShotGunBullet.Damage = EnemyMove.m_EnemyUseGun.Damage;
                        EnemyMove.m_EnemyUseGun.UseBullet();
                        UseGun = EnemyMove.m_EnemyUseGun.NowUseGun;
                        break;
                    }

                default:
                    break;
            }

           

            //총알
            ShotGunBullet.transform.position = m_GunTransform[UseGun].position;
            ShotGunBullet.transform.rotation = this.transform.rotation;
            //Bullets[i].DistanceInit();

            ShotGunBullet.m_Use = true;
            ShotGunBullet.gameObject.SetActive(true);

            //이펙트
            Effects[m_BulletIndex].transform.position = m_GunTransform[UseGun].position;
            Effects[m_BulletIndex].transform.rotation = this.transform.rotation;
            Effects[m_BulletIndex].SetActive(true);
            //탄피
            Effects_Bullet[m_BulletIndex].transform.position = m_BulletTransform[UseGun].position;
            Effects_Bullet[m_BulletIndex].transform.rotation = m_BulletTransform[UseGun].rotation;
            Effects_Bullet[m_BulletIndex].SetActive(true);

            camAni.SetTrigger("ShotGun");
            m_BulletIndex++;
            if (m_BulletIndex == 3)
            {
                m_BulletIndex = 0;
            }


        }
    }

    void DelaySet()
    {
        anim.SetBool("ShotDelay", true);
    }

    void DelayOver()    //애니메이터상 딜레이 해제
    {
        anim.SetBool("ShotDelay", false);
    }

    void ShotEnd()
    {
        anim.SetBool("GunFire", false);
        anim.SetBool("Shot", false);
    }

    void Reload()
    {

        if (gameObject.tag.Equals("Player"))
        {
            m_HandsBulletIndex--;

            switch (CharMove.m_UseGun.NowUseGun)
            {
                case 0:
                    {
                        SillinerRotate += 60;
                        UI_SillinderBullets[5 - CharMove.m_UseGun.Bullet_Gun].SetActive(true);//회전때문에 뒤에거부터 활성화
                        break;
                    }
                case 1:
                    {
                        UI_ShotGunBullets[CharMove.m_UseGun.Bullet_Gun].SetActive(true);
                        break;
                    }
                case 2:
                    {
                        UI_MusketBullets.SetActive(true);
                        break;
                    }

                default:
                    break;
            }


           // UI_HandsBullets[m_HandsBulletIndex].color = Color.black;

            Debug.Log("m_HandsBulletIndex : " + m_HandsBulletIndex);
            CharMove.m_UseGun.ReloadBullet();
        }
    }

    void CharDamageEnd()
    {
        anim.SetBool("Damaged", false);
    }

    void CharRollEnd()
    {
         anim.SetBool("Rolling", false);
    }

    void CharInvincibilityStart()
    {
        CharMove.Invincibility = true;        
    }
    void CharInvincibilityEnd()
    {
        CharMove.Invincibility = false;
    }
    void CharRollEffectOn()
    {
        CharRollEffect.SetActive(true);
    }

    void SetRollSpeed(float speed) //캐릭터 구르기 속도 조절
    {
        CharMove.m_RollSpeed =10*speed;      
    }

    void SetReloadSpeed() //캐릭터 재장전 속도 조절
    {
        anim.speed = CharMove.m_UseGun.ReloadSpeed;
    }
    void SetReloadEnd()
    {
        anim.speed = 1;
    }

    void GunInit()
    {
        Debug.Log("GunInit");
        string str = null;

        GameObject GamePlayObj = GameObject.Find("GamePlayObj");

        switch (gameObject.tag)
        {
            case "Player":
                {
                    UI_Sillinder = GamePlayObj.transform.Find("UI_Main/BlackSillnder").gameObject;
                    UI_ShotGun = GamePlayObj.transform.Find("UI_Main/ShotGun_UI").gameObject;
                    UI_Musket = GamePlayObj.transform.Find("UI_Main/Musket_UI").gameObject;
                    UI_HandsBulletsText = GamePlayObj.transform.Find("UI_Main/BulletText_UI").GetComponent<TMPro.TextMeshProUGUI>();

                    UI_SillinderBullets = new GameObject[6];
                    for(int i =0; i<6;i++)
                    {
                        str = "Bullet_In_" + i;
                        UI_SillinderBullets[i] = UI_Sillinder.transform.Find(str).gameObject;
                        UI_SillinderBullets[i].SetActive(false);
                    }

                    UI_ShotGunBullets = new GameObject[2];
                    for (int i = 0; i < 2; i++)
                    {
                        str = "Bullet_ShotGunIn_" + i;
                        UI_ShotGunBullets[i] = UI_ShotGun.transform.Find(str).gameObject;
                        UI_ShotGunBullets[i].SetActive(false);
                    }

                    str = "Bullet_MusketIn";
                    UI_MusketBullets = UI_Musket.transform.Find(str).gameObject;
                    UI_MusketBullets.SetActive(false);

                    UI_Sillinder.SetActive(false);
                    UI_ShotGun.SetActive(false);
                    UI_Musket.SetActive(false);

                    UI_HandsBullets = new Image[10];
                    for (int i =0; i<10;i++)
                    {
                        str = "UI_Main/UI_Bullets/Bullet_Hands_" + i;
                        UI_HandsBullets[i] = GamePlayObj.transform.Find(str).GetComponent<Image>();
                    }


                    camAni = GamePlayObj.transform.Find("CameraPos").GetComponent<Animator>();
                    break;
                }
            case "Enemy":
                {
                    break;
                }
            default:
                break;
        }
        
    }
}
