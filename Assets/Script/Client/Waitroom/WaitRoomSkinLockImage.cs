using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitRoomSkinLockImage : MonoBehaviour {

    public int SkinIndex;
    public int CharIndex;
    int LockIndex;
    // Use this for initialization
    void Start()
    {
        int Mask = 1; // 00000001
        int select = Mask << SkinIndex;
        int LockCheck = (GameInfoManager.LockCode[CharIndex] & select);

        if (LockCheck > 0)
        {
            gameObject.SetActive(false);
        }

        LockIndex = GameInfoManager.LockCode[CharIndex];
    }

    void Update()
    {
        if (LockIndex != GameInfoManager.LockCode[CharIndex])
        {
            LockIndex = GameInfoManager.LockCode[CharIndex];
            int Mask = 1; // 00000001
            int select = Mask << SkinIndex;
            int LockCheck = (GameInfoManager.LockCode[CharIndex] & Mask);

            if (LockCheck > 0)
            {
                gameObject.SetActive(false);
            }
        }

    }
}
