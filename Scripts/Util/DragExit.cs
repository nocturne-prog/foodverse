using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Marvrus.UI;

public class DragExit : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public enum DragType
    {
        Top_Down,
        Bottom_Up,
        LeftToRight,
        RightToLeft
    }

    public DragType dragType = DragType.Top_Down;

    public UIPopup popup;
    public Action ExitEvent;
    private Vector2 start, end;
    private bool isSuccess = false;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        start = eventData.position;
        isSuccess = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
         end = eventData.position;

        switch (dragType)
        {
            case DragType.Top_Down:
                isSuccess = (start.y - end.y) > Const.SWIPE_DRAG_SIZE;
                break;

            case DragType.Bottom_Up:
                isSuccess = (end.y - start.y) > Const.SWIPE_DRAG_SIZE;
                break;

            case DragType.LeftToRight:
                isSuccess = (end.x - start.x) > Const.SWIPE_DRAG_SIZE;
                break;

            case DragType.RightToLeft:
                isSuccess = (start.x - end.x) > Const.SWIPE_DRAG_SIZE;
                break;
        }

        if (isSuccess is false)
            return;

        if (popup is null)
        {
            ExitEvent?.Invoke();
        }
        else
        {
            popup.Close();
        }
    }
}
