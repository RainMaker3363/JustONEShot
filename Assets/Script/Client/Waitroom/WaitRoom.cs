using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitRoom : MonoBehaviour {

    [SerializeField]
    GameObject[] PlayerChar;

    static int SelectIndex = 0;
    GameObject SelectChar;
    public Transform CharPos;

    GPGSManager m_GPGSManager;
    MultiTitleManager m_MultiTitleManager;
    private IEnumerator SendRoutine;

    // Use this for initialization
    void Start () {
        m_GPGSManager = GameObject.Find("GPGSManager").GetComponent<GPGSManager>();

        if(m_MultiTitleManager == null)
        {
            m_MultiTitleManager = GameObject.Find("MultiTitleManager").GetComponent<MultiTitleManager>();
        }

        SelectChar = Instantiate(PlayerChar[SelectIndex]);
        SelectChar.transform.position = CharPos.position;
        SelectChar.transform.rotation = CharPos.rotation;
        SelectChar.transform.SetParent(CharPos);

        m_GPGSManager.SetMyCharacterNumber(SelectIndex);//GPGS캐릭터 인덱스 설정
        SendRoutine = SendCharacterRoutine();

        StopCoroutine(SendRoutine);
        StartCoroutine(SendRoutine);
    }
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator SendCharacterRoutine()
    {
        while(true)
        {
            if (GPGSManager.GetInstance.IsAuthenticated())
            {
                if (GPGSManager.GetInstance.IsConnected())
                {
                    if (m_MultiTitleManager.GetOpponentCharNumber() == 100)
                    {
                        yield return new WaitForSeconds(0.5f);

                        m_MultiTitleManager.SendCharacterNumber(SelectIndex);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }


        
    }

    public void ChangeChar(int Index)  //대기실은 끊겨도되니 GC를 고려하지 않고 삭제후 생성합니다
    {
        if (Index < 2)  //캐릭터 최대개수
        {
            if (SelectIndex != Index)   //전에 선택한것과 다를경우
            {
                SelectIndex = Index;    //선택한것으로 바꿈
                Destroy(SelectChar);    //전에 있던 캐릭터는 파기
                SelectChar = Instantiate(PlayerChar[SelectIndex]);  //현재 선택한거로 다시 생성
                SelectChar.transform.position = CharPos.position;
                SelectChar.transform.rotation = CharPos.rotation;
                SelectChar.transform.SetParent(CharPos);
                if (GPGSManager.GetInstance.IsAuthenticated())
                {
                    m_GPGSManager.SetMyCharacterNumber(SelectIndex);//GPGS캐릭터 인덱스 설정
                }
            }
        }
    }
}
