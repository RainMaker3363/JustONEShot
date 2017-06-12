using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameChecker : MonoBehaviour {

    // 프레임 고정을 위한 변수
    private int FrameRate;
    private float deltaTime = 0.0f;

    void Awake()
    {
        //FrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //Application.targetFrameRate = 60;
    }
	
	// Update is called once per frame
	void Update () {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 1.5f, 1.5f);

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        GUI.Label(rect, text, style);
    }
}
