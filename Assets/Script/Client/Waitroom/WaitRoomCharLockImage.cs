using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitRoomCharLockImage : MonoBehaviour {

    public int CharIndex;
    int LockIndex;
	// Use this for initialization
	void Start () {
        int Mask = 1; // 00000001
        int select = Mask << CharIndex;
        int LockCheck = (GameInfoManager.CharLock & select);

        if(LockCheck>0)
        {
            gameObject.SetActive(false);
        }
        LockIndex = GameInfoManager.CharLock;
    }

	void Update()
    {
        if(LockIndex != GameInfoManager.CharLock)
        {
            LockIndex = GameInfoManager.CharLock;
            int Mask = 1; // 00000001
            int select = Mask << CharIndex;
            int LockCheck = (GameInfoManager.CharLock & select);

            if (LockCheck > 0)
            {
                gameObject.SetActive(false);
            }
        }

    }
}
