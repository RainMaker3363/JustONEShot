using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimation : MonoBehaviour {

    Animator anim;
    public GameObject NextObject;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        anim.SetBool("PlayEnd", false);
    }
	
	public void PlayEnd()
    {
        anim.SetBool("PlayEnd", true);
        if (NextObject != null)
            NextObject.SetActive(true);
    }
}
