using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieVomit_Cam : MonoBehaviour {

    [SerializeField]
    GameObject[] ZombieVomitEffects;

    public void EffectOnVomit()
    {
        for(int i = 0; i < ZombieVomitEffects.Length;i++)
        {
            if(ZombieVomitEffects[i].activeSelf != true)
            {
                ZombieVomitEffects[i].SetActive(true);
                break;
            }
        }

    }
}
