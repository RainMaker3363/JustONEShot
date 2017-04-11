﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class UseGun
{
    public int MaxBullet_Gun;  //탄창 탄알 최대수
    public int MaxBullet_Hand; // 소지 탄알 최대수
    public int Bullet_Gun;      //탄창 탄알
    public int Bullet_Hand;    //소지 탄알
    public int Bullet_Use;      //소모 탄알
    public int Damage;      //총의 데미지

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
            Bullet_Hand++;
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
        Damage = 10;
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

    [SerializeField]
    GameObject[] Effects;

    [SerializeField]
    GameObject[] Effects_Bullet;

    [SerializeField]
    GameObject[] UI_SillinderBullets;

    [SerializeField]
    Image[] UI_HandsBullets;

    GameObject UI_Silliner;

    float SillinerRotate = 0;
    Animator anim;

    //총 위치
    public Transform m_GunTransform;
    //탄피 위치
    public Transform m_BulletTransform;

    int m_BulletIndex = 0;

    //int m_SillinderBulletIndex = 0;
    int m_HandsBulletIndex = 0;

    void Start()
    {
        anim = GetComponent<Animator>();

        Debug.Log(UI_HandsBullets.Length);
    }

    void Update()
    {
        if (gameObject.tag.Equals("Player"))
        {
            if (CharMove.m_UseGun.Bullet_Hand > m_HandsBulletIndex)
            {
                UI_HandsBullets[m_HandsBulletIndex].color = Color.white;
                m_HandsBulletIndex++;

            }
        }
    }
    void SetBullet()
    {

        while (true)
        {
            if (!Bullets[m_BulletIndex].m_Use)
            {
                Debug.Log(gameObject.tag);
                switch (gameObject.tag)
                {
                    case "Player":
                        {
                            
                            Bullets[m_BulletIndex].Damage = CharMove.m_UseGun.Damage;
                            CharMove.m_UseGun.UseBullet();
                            UI_SillinderBullets[CharMove.m_UseGun.Bullet_Gun].gameObject.SetActive(false);

                            break;
                        }
                    case "Enemy":
                        {
                            Bullets[m_BulletIndex].Damage = EnemyMove.m_EnemyUseGun.Damage;
                            EnemyMove.m_EnemyUseGun.UseBullet();
                            break;
                        }

                    default:
                        break;
                }

                //총알
                Bullets[m_BulletIndex].transform.position = m_GunTransform.position;
                Bullets[m_BulletIndex].transform.rotation = this.transform.rotation;
                //Bullets[i].DistanceInit();

                Bullets[m_BulletIndex].m_Use = true;
                Bullets[m_BulletIndex].gameObject.SetActive(true);

                //이펙트
                Effects[m_BulletIndex].transform.position = m_GunTransform.position;
                Effects[m_BulletIndex].transform.rotation = this.transform.rotation;
                Effects[m_BulletIndex].SetActive(true);
                //탄피
                Effects_Bullet[m_BulletIndex].transform.position = m_BulletTransform.position;
                Effects_Bullet[m_BulletIndex].transform.rotation = m_BulletTransform.rotation;
                Effects_Bullet[m_BulletIndex].SetActive(true);
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
        m_HandsBulletIndex--;
        UI_SillinderBullets[CharMove.m_UseGun.Bullet_Gun].SetActive(true);
        UI_HandsBullets[m_HandsBulletIndex].color = Color.black;
       
        Debug.Log("m_HandsBulletIndex : " + m_HandsBulletIndex);
        CharMove.m_UseGun.ReloadBullet();
       
    }

    void CharDamageEnd()
    {
        anim.SetBool("Damaged", false);
    }


}
