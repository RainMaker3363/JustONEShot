using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootEffect : MonoBehaviour {

    public GameObject m_FootEffect;
    GameObject temp;

    public Transform m_LeftFoot;
    public Transform m_RightFoot;

    public AudioClip StepSounds;

    AudioSource m_AudioSource;

    // Use this for initialization
    void Start () {
        m_AudioSource = gameObject.GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void StempFoot_L()
    {
        Debug.Log("LeftFoot");

        temp = Instantiate(m_FootEffect);
        temp.transform.position = m_LeftFoot.position;
        temp.transform.rotation = this.transform.rotation;
        m_AudioSource.PlayOneShot(StepSounds);
    }
    

    void StempFoot_R()
    {
        Debug.Log("RightFoot");

        temp = Instantiate(m_FootEffect);
        temp.transform.position = m_RightFoot.position;
        temp.transform.rotation = this.transform.rotation;
        m_AudioSource.PlayOneShot(StepSounds);
    }
}
