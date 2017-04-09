using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeadEyeBullet : MonoBehaviour{

    public bool Active = false;
    MeshRenderer m_MeshRenderer;
    Animator anim;
  

 
    // Use this for initialization
    void Start () {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            m_MeshRenderer.material.SetColor("_RimColor", Color.yellow);
        }       

     
    }

    public void BulletIn()
    {
        Debug.Log("Click");


        Active = false;
        m_MeshRenderer.material.SetColor("_RimColor", Color.black);
        anim.SetTrigger("IN");
    }

    public void SpinRoll()
    {
        DeadEyeUI.GunRollAnim.SetTrigger("IN");
    }



}
