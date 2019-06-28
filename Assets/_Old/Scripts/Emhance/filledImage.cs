using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class filledImage : MonoBehaviour
{
    Image image;
    float nowAmount = 0.5f;
    float targetAmount = 0.5f;
    

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = Mathf.Lerp(nowAmount, targetAmount, 0.8f);
        nowAmount = Mathf.Lerp(nowAmount, targetAmount, 0.8f);
    }

    public void changeTargetAmount(float statasValue)
    {
        targetAmount = statasValue / 100;
    }
}
