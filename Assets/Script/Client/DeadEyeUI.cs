using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEyeUI : MonoBehaviour {

    [SerializeField]
    DeadEyeBullet[] Bullets;

    int[] BulletOrders = { 0,1,2,3,4,5 };

    int BulletOrderIndex = 0;

    public Camera cam;
   // public Camera maincam;
    public GameObject m_ScreenShot;
    private DeadEyeBullet target;

    public static Animator GunRollAnim;

    float DeadEyeStartTime;
    float DeadEyeEndTime;

    bool DeadEyeComplete = false;

    // Use this for initialization
    void Start () {
        GunRollAnim = transform.Find("ObjectAnimation").transform.Find("GunRoll").GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            if (GetClickedObject() != null)
            {
                target = GetClickedObject().GetComponent<DeadEyeBullet>();

                if (target.Active)  //순서가 맞다면
                {

                    target.BulletIn();

                    BulletOrderIndex++;
                    if (BulletOrderIndex >= Bullets.Length)
                    {
                        DeadEyeEndTime = Time.time - DeadEyeStartTime;
                        CharMove.m_DeadEyeTimer = DeadEyeEndTime;
                        DeadEyeComplete = true;
                        BulletOrderIndex = 0;
                    }
                    else
                    {
                        Bullets[BulletOrders[BulletOrderIndex]].Active = true;
                    }

                }
            }
        }
    }

    void OnEnable()
    {
        
        int temp;
        int RandomIndex;
        for (int i = 0; i < Bullets.Length; i++)
        {
            Bullets[i].Active = false;
            RandomIndex = Random.Range(i, Bullets.Length);

            temp = BulletOrders[RandomIndex];
            BulletOrders[RandomIndex] = BulletOrders[i];
            BulletOrders[i] = temp;
        }
        BulletOrderIndex = 0;
        Bullets[BulletOrders[BulletOrderIndex]].Active = true;

        DeadEyeStartTime = Time.time;
        DeadEyeComplete = false;
     // m_ScreenShot.SetActive(true);
     //maincam.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (!DeadEyeComplete)
        {
            CharMove.m_DeadEyeTimer = 10;
        }
    }


    private GameObject GetClickedObject()
    {
        RaycastHit hit;
        GameObject target = null;


        Ray ray = cam.ScreenPointToRay(Input.mousePosition); //마우스 포인트 근처 좌표를 만든다. 


        if (true == (Physics.Raycast(ray.origin, ray.direction * 10, out hit)))   //마우스 근처에 오브젝트가 있는지 확인
        {
            //있으면 오브젝트를 저장한다.
            target = hit.collider.gameObject;
        }

        return target;
    }

 
}
