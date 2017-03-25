using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class UseGun
{
    public int MaxBullet_Gun;  //탄창 탄알 최대수
    public int MaxBullet_Hand; // 소지 탄알 최대수
    public int Bullet_Gun;      //탄창 탄알
    public int Bullet_Hand;    //소지 탄알
    public int Bullet_Use;      //소모 탄알

    abstract public void UseBullet();
    public void ReloadBullet()  //탄창이 찼을경우는 charmove에서 체크중
    {
        Bullet_Hand--;
        Bullet_Gun++;
    }
    public void GetBullet()
    {
        if(MaxBullet_Hand> Bullet_Hand)
             Bullet_Hand++;        
    }
}

class Gun_Revolver : UseGun
{
    public Gun_Revolver()
    {
        MaxBullet_Gun = 6;
        MaxBullet_Hand = 10;
        Bullet_Gun = 6;
        Bullet_Hand = 10;
        Bullet_Use = 1;
    }

    public override void UseBullet()
    {
      Bullet_Gun -= Bullet_Use;
    }


}


public class Gun : MonoBehaviour {

    //총알
    [SerializeField]
    Bullet[] Bullets;

    [SerializeField]
    GameObject[] Effects;

    Animator anim;

    //총 위치
    public Transform m_GunTransform;

    public Transform m_CharPositon;

    int m_BulletIndex = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void SetBullet()
    {
        while(true)
        { 
            if (!Bullets[m_BulletIndex].m_Use)
            {
                Bullets[m_BulletIndex].transform.position = m_GunTransform.position;
                Bullets[m_BulletIndex].transform.rotation = this.transform.rotation;
                //Bullets[i].DistanceInit();
                Bullets[m_BulletIndex].m_Use = true;
                Bullets[m_BulletIndex].gameObject.SetActive(true);

                Effects[m_BulletIndex].transform.position = m_GunTransform.position;
                Effects[m_BulletIndex].transform.rotation = this.transform.rotation;
                Effects[m_BulletIndex].SetActive(true);
                m_BulletIndex++;
                if (m_BulletIndex == 3)
                {
                    m_BulletIndex = 0;
                }

                CharMove.m_UseGun.UseBullet();
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
        anim.SetBool("GunFire",false);
    }

    void Reload()
    {
        CharMove.m_UseGun.ReloadBullet();
    }

}
