using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi_Item_Bullet : MonoBehaviour {

    private CapsuleCollider cap_col;
    public MultiGameManager Mul_Manager;

    // Use this for initialization
    void Start ()
    {
	    if(cap_col == null)
        {
            cap_col = GetComponent<CapsuleCollider>();
            cap_col.enabled = true;
        }	

	}

    void OnEnable()
    {
        if (cap_col == null)
        {
            cap_col = GetComponent<CapsuleCollider>();
            cap_col.enabled = true;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player") == true)
        {
            //Debug.Log("ASDAFAWDASDAWD");

            if (Mul_Manager == true)
            {
                Mul_Manager.CallSendItemStateMessage();
            }
        }
    }
    
}
