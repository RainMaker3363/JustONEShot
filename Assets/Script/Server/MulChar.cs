using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MulChar : MonoBehaviour {

    public Camera cam;
    private Vector3 CamPos;
    private CharacterController m_CharCtr;

    public MoveJoyStick m_MoveJoyStickControl;  //움직임 전용 조이스틱
    private float m_MoveSpeed = 4.5f;
    private float MoveDir;

    private int ps;

    // Use this for initialization
    void Start () {
        //카메라 기본위치 설정
        CamPos = cam.transform.position;

        MoveDir = 0.0f;

        if(m_CharCtr == null)
        {
            m_CharCtr = GetComponent<CharacterController>();
        }

    }
	
	// Update is called once per frame
	void Update () {

        MoveDir = m_MoveJoyStickControl.GetVectorForce();

        if (m_MoveJoyStickControl.GetVectorForce() > 0)
        {
            PlayerMove();
        }
        

        
    }

    void PlayerMove()
    {

        transform.rotation = m_MoveJoyStickControl.GetRotateVector();
        //transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
        //m_CharCtr.Move((transform.forward + Physics.gravity) * m_MoveSpeed * Time.deltaTime);
        m_CharCtr.Move((transform.forward + Physics.gravity) * m_MoveSpeed * Time.deltaTime);
        //this.transform.Translate((transform.forward) * MoveDir * m_MoveSpeed * Time.deltaTime);
        //RaycastHit Ground;
        //if (Physics.Raycast(m_GroundCheck.position, Vector3.down, out Ground, 5f))
        //{
        //    //Debug.Log("HitPosition : " + Ground.point);
        //    //Debug.Log("Hitname : " + Ground.transform.name);
        //    transform.position = new Vector3(transform.position.x, Ground.point.y, transform.position.z);
        //}

        cam.transform.position = CamPos + transform.position;
    }
}
