﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_ShotGun : MonoBehaviour {

    int m_Movespeed = 30;    //지속시간감소
    [SerializeField]
    int m_Distance ;    //지속시간

    //[SerializeField]
    int m_DistanceInit = 1000;

    //총알 사용중 여부
    public bool m_Use = false;

    public int Damage;


    //RaycastHit HitObj;

	// Use this for initialization
	void Start () {
        

    }
	
    void OnEnable()
    {
        m_Distance = m_DistanceInit;
        gameObject.GetComponent<MeshCollider>().enabled = true;
    }

	// Update is called once per frame
	void Update () {
        
    }
    void FixedUpdate()
    {

        //Debug.DrawLine(transform.position, transform.position + Vector3.forward * 5, Color.blue);
        if (m_Distance > 0)
        {
            //Debug.DrawLine(transform.position, transform.position + transform.forward * 1, Color.blue);
            //transform.Translate(Vector3.forward * m_Movespeed * Time.deltaTime);
            m_Distance -= m_Movespeed;
        }
        else
        {
            m_Use = false;
            gameObject.SetActive(false);
        }


      
    }

    void OnTriggerEnter(Collider HitObj)
    {
        if(HitObj.gameObject.tag == "Player")
            {
            Debug.Log("Hit");
            Debug.Log("Hit" + Damage);
            HitObj.gameObject.GetComponent<CharMove>().Damaged(Damage, transform.forward);
            // m_Distance = 0;
        }

        if (HitObj.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit");

            HitObj.gameObject.GetComponent<EnemyMove>().Damaged(Damage, transform.forward);
            // m_Distance = 0;
        }
        //m_Distance = 0;
        gameObject.GetComponent<MeshCollider>().enabled = false;
    }
   //public void DistanceInit()
   // {
   //     m_Distance = m_DistanceInit;
   // }
}
