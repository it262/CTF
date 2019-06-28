using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{

	[SerializeField] Material skybox;
	private float rot = 0;
    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
		rot = (rot > 360) ? 0 : rot + 0.1f;
		RenderSettings.skybox.SetFloat ("_Rotation", rot);
    }
}
