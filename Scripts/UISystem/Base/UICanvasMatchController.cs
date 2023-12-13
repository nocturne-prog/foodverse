using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasMatchController : MonoBehaviour
{
    private CanvasScaler scaler;
    private float ratio;

    private void Start()
    {
        scaler = GetComponent<CanvasScaler>();
        SetMatch();
    }

#if UNITY_EDITOR
    private void Update()
    {
        SetMatch();
    }
#endif

    private void SetMatch()
    {
        ratio = (float)Screen.width / Screen.height;

        /// 19 : 9 해상도를 넘어갈 경우
        if (ratio > 2.1)
            scaler.matchWidthOrHeight = 1;
        else
            scaler.matchWidthOrHeight = 0.5f;
    }
}
