using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffectsManager : MonoBehaviour {

    static BloodEffectsManager Manager;

    public GameObject BloodEffect;

    [SerializeField]
    GameObject[] BloodEffects;

    BloodEffectsManager()
    {
        if (Manager == null)
        {
            Manager = this;
        }
    }

    public static BloodEffectsManager GetInstance()
    {
        if (Manager == null)
        {
            Manager = new BloodEffectsManager();
        }
        return Manager;
    }

    void Awake()
    {
        Manager = this;
    }


    // Use this for initialization
    void Start () {
        GameObject BloodEffectManager = GameObject.Find("BloodEffectsManager");
        for(int i = 0; i<BloodEffects.Length;i++)
        {
            BloodEffects[i] = Instantiate(BloodEffect);
            BloodEffects[i].name = "BloodEffect" + i.ToString();
            BloodEffects[i].transform.SetParent(BloodEffectManager.transform);
        }

    }
	
	public bool BloodEffectOn(GameObject Obj)
    {
        GameObject Effect = null;
        bool EffectOn = false;
        for (int i = 0; i < BloodEffects.Length; i++)
        {
            if(!BloodEffects[i].activeSelf)
            {
                Effect = BloodEffects[i];
                Effect.transform.position = Obj.transform.position;
                Effect.transform.SetParent(Obj.transform);
                Effect.SetActive(true);
                EffectOn = true;
                break;
            }
        }

        return EffectOn;
    }
}
