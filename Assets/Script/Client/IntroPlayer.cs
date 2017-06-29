//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class IntroPlayer : MonoBehaviour {

//    Coroutine nextscene;
//    // Use this for initialization
//    void Start () {
//#if UNITY_ANDROID
//        Handheld.PlayFullScreenMovie("Intro.mp4", Color.black,FullScreenMovieControlMode.CancelOnInput);
//#elif UNITY_IPHONE
//        iPhoneUtils.PlayMovie(path, Color.black, iPhoneMovieControlMode.CancelOnTouch, iPhoneMovieScalingMode.Fill);
//#endif
//        nextscene = StartCoroutine(NextScene());
//    }
	
//	// Update is called once per frame
//	void Update () {
//		if(Input.GetMouseButtonDown(0))
//        {
//            StopCoroutine(nextscene);
//            UnityEngine.SceneManagement.SceneManager.LoadScene("WaitingRoom");
//        }
//	}

//    IEnumerator NextScene()
//    {
//        yield return new WaitForSeconds(27);
//        UnityEngine.SceneManagement.SceneManager.LoadScene("WaitingRoom");
//    }
//}
using UnityEngine;
using System.Collections;
using System.IO;
 
[RequireComponent(typeof(AudioSource))]
public class VideoPlayer : MonoBehaviour
{
    Object[] movie_stills;
    int number_of_stills = 0;
    public bool loop = false;
    public bool playOnStart = false;
    public int fps = 60;
    public AudioClip sound;
    public string resourceSubfolder = "";

    private int stills = 0;
    private bool play = false;
    private bool loaded = false;

    void Update()
    {

        if (!resourceSubfolder.Equals("")&&!loaded) {
            StartCoroutine(ImportVideo());
        }

        if (fps > 0)
        {
            if (play == true)
            {
                StartCoroutine(Player());
            }
        }
        else
        {
            Debug.LogError("'fps' must be set to a value greater than 0.");
        }
    }

    IEnumerator Player()
    {
        play = false;
        if (loop)
        {
            Debug.Log("looped. stills: " + stills + ", length: " + movie_stills.Length);
            if (stills >= movie_stills.Length)
            {
                GetComponent<AudioSource>().Stop();
                GetComponent<AudioSource>().clip = sound;
                GetComponent<AudioSource>().Play();
                stills = 0;
                Debug.Log("restarting. stills: " + stills);
            }
        }
        else
        {
            if (stills > movie_stills.Length)
            {
                GetComponent<AudioSource>().Stop();
                stills -= 1;
            }
        }

        if (stills >= 0&&stills < movie_stills.Length) {
            Texture2D MainTex = movie_stills[stills] as Texture2D;
            GetComponent<Renderer>().material.SetTexture("_MainTex", MainTex);
            stills += 1;
            int fps_fixer = fps * 3;
            float wait_time = 1.0f / fps_fixer;
            yield return new WaitForSeconds(wait_time);
            if (!GetComponent<AudioSource>().clip)
            {
                if (sound)
                {
                    GetComponent<AudioSource>().clip = sound;
                    GetComponent<AudioSource>().Play();
                }
            }
            play = true;
        }
    }

    public void Play() { play = true; }
    public void Pause() { play = false; }

    void Start()
    {
        GetComponent<AudioSource>().loop = false;
        number_of_stills -= 1;
    }


    IEnumerator ImportVideo()
    {
        movie_stills = Resources.LoadAll(resourceSubfolder, typeof(Texture2D));
        loaded = true;
        if (playOnStart)
            play = true;
        yield return null;
    }

    public void UnloadFromMemory()
    {
        play = false;
        GetComponent<AudioSource>().Stop();

        foreach (Object o in movie_stills) Destroy(o);
    }
}