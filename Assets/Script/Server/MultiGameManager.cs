using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HY
{
    // 멀티 게임 상태
    public enum MultiGameState
    {
        START = 0,
        PLAY,
        GAMEOVER,
        GAMEWIN,
        CONNECTOUT
    }

    // 멀티 게임 플레이어의 상태
    public enum MultiPlayerState
    {
        START = 0,
        LIVE,
        RELOAD,
        SHOOT,
        SHOOTCANCEL,
        SHOOTCOMPLETE,
        EVENT,
        DEADEYESTART,
        DEADEYEACTIVE,
        DEAD
    }

    // 멀티 플레이어 캐릭터 상태
    public enum MultiCharacterState
    {
        CHAR_001 = 0,
        CHAR_002,
        CHAR_003
    }

    // 멀티 플레이어 총 상태
    public enum MultiGunState
    {
        REVOLVER = 0,
        SHOTGUN,
        MUSKET
    }
}

public class MultiGameManager : MonoBehaviour {

    // 전체적으로 관리될 게임, 캐릭터 상태 값
    public static HY.MultiGameState MultiState;
    public static HY.MultiPlayerState PlayerState;

	// Use this for initialization
	void Awake () {
		
        // 임시로 만든 상태 값이므로 추후에 수정해주세요
        MultiState = HY.MultiGameState.START;
        PlayerState = HY.MultiPlayerState.LIVE;
	}
	
	// Update is called once per frame
	void Update () {
		switch(MultiState)
        {
            case HY.MultiGameState.START:
                {
                    switch(PlayerState)
                    {
                        case HY.MultiPlayerState.START:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.LIVE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.RELOAD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOTCANCEL:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOTCOMPLETE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.EVENT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYESTART:
                            {

                            }
                            break;
                            
                        case HY.MultiPlayerState.DEADEYEACTIVE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEAD:
                            {

                            }
                            break;
                    }
                }
                break;
                
            case HY.MultiGameState.PLAY:
                {
                    switch (PlayerState)
                    {
                        case HY.MultiPlayerState.START:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.LIVE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.RELOAD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOTCANCEL:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOTCOMPLETE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.EVENT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYESTART:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYEACTIVE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEAD:
                            {

                            }
                            break;
                    }
                }
                break;

            case HY.MultiGameState.GAMEWIN:
                {
                    switch (PlayerState)
                    {
                        case HY.MultiPlayerState.START:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.LIVE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.RELOAD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOTCANCEL:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOTCOMPLETE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.EVENT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYESTART:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYEACTIVE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEAD:
                            {

                            }
                            break;
                    }
                }
                break;

            case HY.MultiGameState.GAMEOVER:
                {
                    switch (PlayerState)
                    {
                        case HY.MultiPlayerState.START:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.LIVE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.RELOAD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOTCANCEL:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOTCOMPLETE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.EVENT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYESTART:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYEACTIVE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEAD:
                            {

                            }
                            break;
                    }
                }
                break;

            case HY.MultiGameState.CONNECTOUT:
                {
                    switch (PlayerState)
                    {
                        case HY.MultiPlayerState.START:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.LIVE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.RELOAD:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOTCANCEL:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.SHOOTCOMPLETE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.EVENT:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYESTART:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEADEYEACTIVE:
                            {

                            }
                            break;

                        case HY.MultiPlayerState.DEAD:
                            {

                            }
                            break;
                    }
                }
                break;
        }
	}
}
