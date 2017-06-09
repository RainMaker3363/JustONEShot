using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneDamage : MonoBehaviour {

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.gameObject.tag);
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<CharMove>().Damaged(1000, Vector3.zero);
        }
    }

    //void OnControllerColliderHit(ControllerColliderHit col)
    //{
    //    Debug.Log(col.gameObject.tag);
    //    if (col.gameObject.tag == "Player")
    //    {
    //        col.gameObject.GetComponent<CharMove>().Damaged(1000, Vector3.zero);
    //    }
    //}
}
