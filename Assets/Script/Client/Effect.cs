using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {
    float SetTime;
    public float LateTime;
    // Update is called once per frame
    void Update()
    {
        if (SetTime < Time.time)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SetTime = Time.time + LateTime;
    }
}
