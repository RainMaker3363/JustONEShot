using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int m_Movespeed = 0;    //이동속도
   // [SerializeField]
    public int m_Distance=0 ;    //이동거리

    //[SerializeField]
    int m_DistanceInit = 500;

    //총알 사용중 여부
    public bool m_Use = false;

    public int Damage;


    RaycastHit HitObj;

	// Use this for initialization
	void Start () {
        

    }
	
    void OnEnable()
    {
        m_Distance += m_DistanceInit;
        
    }

	// Update is called once per frame
	void Update () {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Hit");
            Debug.Log("Hit" + Damage);
            col.gameObject.GetComponent<CharMove>().Damaged(Damage, transform.forward);
            // m_Distance = 0;
        }

        if (col.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit");

            col.gameObject.GetComponent<EnemyMove>().Damaged(Damage, transform.forward);
            // m_Distance = 0;
        }

        m_Distance = 0;
    }
    void FixedUpdate()
    {

        //Debug.DrawLine(transform.position, transform.position + Vector3.forward * 5, Color.blue);
        if (m_Distance > 0)
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 1, Color.blue);
            transform.Translate(Vector3.forward * m_Movespeed * Time.deltaTime);
            m_Distance -= m_Movespeed;
        }
        else
        {
            m_Use = false;
            gameObject.SetActive(false);
        }


      
    }

   //public void DistanceInit()
   // {
   //     m_Distance = m_DistanceInit;
   // }
}
