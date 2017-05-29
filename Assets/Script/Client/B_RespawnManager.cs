using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_RespawnManager : MonoBehaviour {

    [SerializeField]
    public GameObject[] Item_Bullets;

    //static B_RespawnManager Manager;

    //B_RespawnManager()
    //{
    //    if(Manager==null)
    //    {
    //        Manager = this;
    //    }
    //}

    //public static B_RespawnManager GetInstance()
    //{
    //    if (Manager == null)
    //    {
    //        new B_RespawnManager();
    //    }
    //    return Manager;
   // }

    public int CreateItemBullet(Transform RespawnPoint)
    {
        //Debug.Log(Item_Bullets.Length);
        int BulletIndex = 0;
        for(int i = 0; i<Item_Bullets.Length;i++)
        {
            //Debug.Log(Item_Bullets[i].activeSelf);
            if (!Item_Bullets[i].activeSelf)
            {
                Item_Bullets[i].transform.position = RespawnPoint.position;
                Item_Bullets[i].SetActive(true);
                BulletIndex = i;
                Debug.Log(Item_Bullets[i].transform.position);
                break;
            }
        }
        return BulletIndex;
    }

    public void DeleteItemBullet(int BulletIndex)
    {
        Item_Bullets[BulletIndex].SetActive(false);
    }

}
