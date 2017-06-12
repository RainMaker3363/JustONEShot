using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_RespawnManager : MonoBehaviour {

    [SerializeField]
    GameObject[] Item_Bullets;

    static DB_RespawnManager Manager;

    DB_RespawnManager()
    {
        if(Manager==null)
        {
            Manager = this;
        }
    }

    public static DB_RespawnManager GetInstance()
    {
        if (Manager == null)
        {
            Manager = new DB_RespawnManager();
        }
        return Manager;
    }

    void Awake()
    {
        Manager = this;
    }

    public int CreateItemBullet(Transform RespawnPoint)
    {
        int BulletIndex = 0;
        for(int i = 0; i<Item_Bullets.Length;i++)
        {
            if(!Item_Bullets[i].activeSelf)
            {
                Item_Bullets[i].transform.position = RespawnPoint.transform.position;
                Item_Bullets[i].SetActive(true);
                BulletIndex = i;
                break;
            }
        }
        return BulletIndex;
    }

    public void DeleteItemBullet(int BulletIndex)
    {
        if(BulletIndex>-1)
            Item_Bullets[BulletIndex].SetActive(false);
    }

}
