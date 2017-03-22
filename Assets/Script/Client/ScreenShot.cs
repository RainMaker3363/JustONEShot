//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class ScreenShot : MonoBehaviour {

//    private Texture2D m_Screenshot;
//    private Image m_ScreenImage;

//    // Use this for initialization
//    void Start () {

//        m_ScreenImage = GetComponent<Image>();

//        //2d texture객체를 만드는대.. 스크린의 넓이, 높이를 선택하고 텍스쳐 포멧은 스크린샷을 찍기 위해서는 이렇게 해야 한다더군요. 
//        m_Screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);


//        //현재 화면을 픽셀단위로 읽음
//        m_Screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);


//        //적용
//        m_Screenshot.Apply();

//        m_ScreenImage = m_Screenshot;

//    }
	
//	// Update is called once per frame
//	void Update () {
		
//	}
//}
