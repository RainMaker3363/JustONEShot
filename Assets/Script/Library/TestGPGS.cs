using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using GooglePlayGames.BasicApi;

public class TestGPGS : MonoBehaviour {


    /// 현재 로그인 중인지 체크
    public bool bLogin
    {
        get;
        set;
    }

	// Use this for initialization
	void Start () {

        bLogin = false;

        PlayGamesPlatform.Activate();

        // 로그인이 안되어 있으면
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(LoginCallBackGPGS);
        }
            

    }

    /// GPGS Login Callback
    public void LoginCallBackGPGS(bool result)
    {

        bLogin = result;

    }


    
	
	// Update is called once per frame
	void Update () {

	}
}
