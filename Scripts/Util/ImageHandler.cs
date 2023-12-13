using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marvrus.Util
{
    public class ImageHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        Vector2 screenGap = Vector2.zero;
        public void OnBeginDrag(PointerEventData eventData)
        {
            screenGap = new Vector2(eventData.position.x - (Screen.width / 2), eventData.position.y - (Screen.height / 2));
        }

        public void OnDrag(PointerEventData eventData)
        {
            bool scale = Input.touchCount > 1;

            if (scale is true)
            {
                // TODO :: 나중에 폰에서 확인해서 개발해야 함.
                Debug.Log(Input.GetTouch(0).position - Input.GetTouch(1).position);
            }
            else
            {
                Vector2 pos = new Vector2(eventData.position.x - (Screen.width / 2), eventData.position.y - (Screen.height / 2)) - screenGap;
                transform.localPosition = new Vector3(pos.x, pos.y, transform.position.z);

                // TODO :: mask 범위 벗어나지 않게 수정.

            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            screenGap = Vector2.zero;
        }

        float minValue = 0.5f;
        float maxValue = 2f;
        float delta = 0.1f;

        public void Update()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                var multiplier = Input.mouseScrollDelta.y * delta;

                transform.localScale += new Vector3(1, 1, 0) * multiplier;

                if (transform.localScale.x < 0.5f)
                    transform.localScale = new Vector3(minValue, minValue, 1);

                if (transform.localScale.x > 2f)
                    transform.localScale = new Vector3(maxValue, maxValue, 1);
            }
        }
    }
}