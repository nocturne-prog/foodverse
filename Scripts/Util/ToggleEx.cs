using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleEx : Button
{
    public GameObject on;
    public GameObject off;

    public bool isOn
    {
        get
        {
            return toggle;
        }
        set
        {
            SetOn(value);
        }
    }

    private bool toggle = false;

    public override void OnPointerClick(PointerEventData eventData)
    {
        isOn = !toggle;
        base.OnPointerClick(eventData);
    }

    public void SetOn(bool _isOn)
    {
        on.SetActive(_isOn);
        off.SetActive(!_isOn);

        toggle = _isOn;
    }
}
