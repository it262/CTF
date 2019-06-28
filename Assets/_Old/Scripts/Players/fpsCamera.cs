using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fpsCamera : MonoBehaviour {
    public Texture2D cursor;
    public float mouse_move_x;
    public float mouse_move_y;
    public Quaternion vRotation;
    public Quaternion hRotation;

    public PlayerScript ps;

    // Use this for initialization
    void Start()
    {
		
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);

        vRotation = Quaternion.identity;
        //hRotation = Quaternion.identity;
        ps = GetComponent<PlayerScript>();
    }

	// Update is called once per frame
	void Update () {

		if (!ps.pd.isPlayer)
			return;
		
        float sensitivity = 5f; // いわゆるマウス感度
        mouse_move_x = Input.GetAxis("Mouse X") * sensitivity;
        mouse_move_y = Input.GetAxis("Mouse Y") * sensitivity;

        hRotation *= Quaternion.Euler(0, mouse_move_x, 0);
        //vRotation *= Quaternion.Euler(-mouse_move_y, 0, 0);
		vRotation *= Quaternion.Euler(0,mouse_move_y, 0);

        //transform.rotation = hRotation * vRotation;
		//Vector3 v = hRotation * vRotation;


        if (Cursor.lockState != CursorLockMode.Locked)
        {
            // マウスの左ボタンが押されている
            if (Input.GetMouseButtonDown(0))
            {
                // 画面中央固定で非表示にする
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
