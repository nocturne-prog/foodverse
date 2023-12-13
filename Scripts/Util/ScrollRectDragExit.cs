using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectDragExit : ScrollRect
{
    public Action ExitEvent;
    private Vector2 start, end;
    private bool a, b;


    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        start = eventData.position;
        a = verticalNormalizedPosition == 1 || verticalNormalizedPosition == 0;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        end = eventData.position;
        b = verticalNormalizedPosition == 1 || verticalNormalizedPosition == 0;

        if (a && b)
        {

            if ((start.y - end.y) > Const.SWIPE_DRAG_SIZE)
            {
                ExitEvent?.Invoke();
            }
        }
    }
}
