using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_DeadEye : MonoBehaviour
{


    public float m_Movespeed;    //이동속도 DeadEye에서 거리및 시간에 비례하여 조정합니다
    [SerializeField]
    int m_Distance;    //이동거리

    //[SerializeField]
    int m_DistanceInit = 500;

    //총알 사용중 여부
    public bool m_Use = false;

    public int Damage = 20;


    RaycastHit HitObj;

    // Use this for initialization
    void Start()
    {
        Damage = 20;

    }

    void OnEnable()
    {
        m_Distance = m_DistanceInit;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {

        if (Physics.Raycast(transform.position, transform.forward, out HitObj, 0.5f))
        {

            if (HitObj.collider.gameObject.tag == "Player")
            {
                Debug.Log("Hit");
                Debug.Log("Hit" + Damage);
                HitObj.collider.gameObject.GetComponent<CharMove>().DeadEyeDamaged(Damage, transform.forward);
                m_Distance = 0;
                Debug.Log(m_Distance);
            }

            if (HitObj.collider.gameObject.tag == "Enemy")
            {
                Debug.Log("Hit");
                HitObj.collider.gameObject.GetComponent<EnemyMove>().DeadEyeDamaged(Damage, transform.forward);
                m_Distance = 0;
            }

            if (HitObj.collider.gameObject.tag == "Scarecrow")
            {
                Debug.Log("Hit");
                HitObj.collider.transform.parent.GetComponent<Scarecrow>().DeadEyeDamaged(Damage);
                m_Distance = 0;
            }
        }
        //Debug.DrawLine(transform.position, transform.position + Vector3.forward * 5, Color.blue);
       
        if (m_Distance > 0)
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 1, Color.blue);
            transform.Translate(Vector3.forward * m_Movespeed * Time.deltaTime);
           // m_Distance -= m_Movespeed;
        }
        else
        {
            m_Use = false;
            gameObject.SetActive(false);
        }



    }


}
