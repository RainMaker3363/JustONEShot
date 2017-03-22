using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    //총알
    [SerializeField]
    Bullet[] Bullets;

    [SerializeField]
    GameObject[] Effects;

    //총 위치
    public Transform m_GunTransform;

    public Transform m_CharPositon;

    int m_BulletIndex = 0;

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
                break;
            }
            else
            {
                m_BulletIndex++;
            }
            
        }
    }
}
