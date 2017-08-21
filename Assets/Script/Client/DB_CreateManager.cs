using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_CreateManager : MonoBehaviour {

    [SerializeField]
    GameObject[] RespawnPoint;

    static DB_CreateManager Manager;

    bool BulletCreate = true;   //생성이 필요한경우
    public bool Request = false;    //서버에 난수생성 요청 필요 여부
    int ServerIndex = -1;    //서버로 받을 난수
    int CreateIndex = -1;    //총알이 생성된 인덱스 

    public MultiGameManager Mul_Manager;

    DB_CreateManager()
    {
        if (Manager == null)
        {
            Manager = this;
        }
    }

    public static DB_CreateManager GetInstance()
    {
        if (Manager == null)
        {
            Manager = new DB_CreateManager();
        }
        return Manager;
    }

    void Awake()
    {
        if (Manager == null)
        {
            Manager = this;
        }
    }

    void Start()
    {
       // ServerIndex = Mul_Manager.GetPVPDeadEyeStartIndex();
    }

    void Update()
    {
        
        //if (Request)   //생성이 안되어있는경우
        //{
        //    //Mul_Manager.SendDeadEyeRespawnIndexMessage();//서버에 난수를 요청한다
        //    Request = false;
        //}

        if(Mul_Manager.GetDeadEyeRespawnIndex() > -1)
             ServerIndex = Mul_Manager.GetDeadEyeRespawnIndex(); //서버의 인덱스는 계속 참조를한다

        if (ServerIndex != -1 && CreateIndex != ServerIndex) //생성이 필요할경우 또는 만들었던 인덱스가 다를경우
        {
            if(CreateIndex != -1 && RespawnPoint[CreateIndex].GetComponent<DeadEyeBulletRespawn>().CreateAble)
            {
                RespawnPoint[CreateIndex].GetComponent<DeadEyeBulletRespawn>().BulletInit();    //사용하지않게 초기화
            }
            BulletCreate = true;    //생성을 하게한다
        }

        if (BulletCreate)  //서버에서 난수를 받았고 생성이 필요하다면
        {
            if (RespawnPoint[ServerIndex].activeSelf)//켜져있음 == 데스존에 잠기지않음
            {
                BulletCreate = false;
                RespawnPoint[ServerIndex].GetComponent<DeadEyeBulletRespawn>().CreateAble = true;
                CreateIndex = ServerIndex;
             }
            else // 데스존에 잠겨있다면
            {
                BulletCreate = false;
                //Mul_Manager.SendDeadEyeRespawnIndexMessage();//서버에 난수 재생성 요구
            }
        }
    }

    //void OnGUI()
    //{
    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(w / 2, h-100, 100, 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = 30;
    //    style.normal.textColor = new Color(0.0f, 0.0f, 1.5f, 1.5f);

    //    //string text = string.Format("HP : {0}", HP);
    //    string text = string.Format("ServerIndex : {0}\nCreateIndex : {1}", ServerIndex, CreateIndex);

    //    GUI.Label(rect, text, style);
    //}
}
