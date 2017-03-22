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
        if (m_Distance > 0)
        {
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
