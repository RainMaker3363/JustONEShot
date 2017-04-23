using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    int m_Movespeed = 30;    //이동속도
    [SerializeField]
    int m_Distance ;    //이동거리

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
        m_Distance = m_DistanceInit;
        
    }

	// Update is called once per frame
	void Update () {
        
    }
    void FixedUpdate()
    {

        if (Physics.Raycast(transform.position, transform.forward, out HitObj, 1.0f))
        {
            
            if (HitObj.collider.gameObject.tag == "Player")
            {
                Debug.Log("Hit");
                Debug.Log("Hit"+ Damage);
                HitObj.collider.gameObject.GetComponent<CharMove>().Damaged(Damage, transform.forward);
               // m_Distance = 0;
            }

            if (HitObj.collider.gameObject.tag == "Enemy")
            {
                Debug.Log("Hit");
                HitObj.collider.gameObject.GetComponent<EnemyMove>().Damaged(Damage, transform.forward);
               // m_Distance = 0;
            }
            m_Distance = 0;
        }
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
