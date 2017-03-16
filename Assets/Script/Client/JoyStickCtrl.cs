using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class JoyStickCtrl : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    // 조이스틱의 기판
    public Image Joystick_Pad;
    // 조이스틱의 스틱(Stick)
    public Image joystick_Stick;
    // 조이스틱의 이동 벡터 값
    private Vector3 inputVector;
    // 터치중인지 체크
    private bool Touch;

	// Use this for initialization
	void Start () 
    {
        if(Joystick_Pad == null)
        {
            Joystick_Pad = GetComponent<Image>();
        }

        if(joystick_Stick == null)
        {
            joystick_Stick = transform.GetChild(0).GetComponent<Image>();
        }
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // 터치가 드래그(Drag) 했을때 호출 되는 함수
    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 Pos;

        // 터치된 로컬 좌표값을 Pos에 할당하고 Joystick_Pad 직사각형의 sizedelta 값으로 나누어
        // Pos.X는 0~1, Pos.Y는 0~1 사이의 값으로 만듭니다.
        // Joystick_Stick을 기준으로 좌우로 움직였을때 Pos.X는 -1~1 사이, 상하라면 Pos.Y를 -1~1의 값으로 변환하기 위해
        // Pos.x * 2 + 1, Pos.y * 2 - 1 처리를 합니다.
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(Joystick_Pad.rectTransform, ped.position, ped.pressEventCamera, out Pos))
        {
            Pos.x = (Pos.x / Joystick_Pad.rectTransform.sizeDelta.x);
            Pos.y = (Pos.y / Joystick_Pad.rectTransform.sizeDelta.y);

            inputVector = new Vector3(Pos.x * 2 ,Pos.y * 2, 0);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // 조이스틱이 움직인다면..
            // joystick_Stick의 이미지를 터치한 좌표값으로 움직여준다.
            joystick_Stick.rectTransform.anchoredPosition = new Vector3(inputVector.x * (joystick_Stick.rectTransform.sizeDelta.x / 2),
                 inputVector.y * (joystick_Stick.rectTransform.sizeDelta.y / 2));
        }
    }

    // 터치를 하고 있을 대 발생하는 함수
    public virtual void OnPointerDown(PointerEventData ped)
    {
        Touch = true;
        OnDrag(ped);
    }

    // 터치에서 손을 땠을때 발생하는 함수
    public virtual void OnPointerUp(PointerEventData ped)
    {
        Touch = false;
        inputVector = Vector3.zero;
        joystick_Stick.rectTransform.anchoredPosition = Vector3.zero;
    }
  
    public Quaternion GetRotateVector()
    {
        Vector3 vec = new Vector3(inputVector.x, 0, inputVector.y);

        return Quaternion.LookRotation(vec);
    }
    public void InitInputVector()
    {
        inputVector = Vector3.zero;
        joystick_Stick.rectTransform.anchoredPosition = Vector2.zero;
    }

    public bool GetTouch()
    {
        return Touch;
    }
}
