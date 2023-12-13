using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.Scroll
{
    public class RecycleScroll : MonoBehaviour
    {
        [SerializeField]
        public struct Item
        {
            public RectTransform trf;
            public RecycleScroll_Item token;
        }

        [SerializeField] private RectTransform m_ItemBase;
        [SerializeField] int m_instantiateItemCount = 9;
        [SerializeField] float m_itemWidth = 1080;
        [SerializeField] float m_itemHeight = 200;
        [SerializeField] float gap = 10;
        [SerializeField] Direction direction;
        [System.NonSerialized] public List<Item> m_itemList = new List<Item>();
        protected float m_diffPreFramePosition = 0;
        [SerializeField] int m_currentItemNo = 0;
        [SerializeField] public ScrollRect scrollRect;
        [SerializeField] int dataCount = 0;
        public enum Direction
        {
            Vertical,
            Horizontal,
        }

        // cache component
        public RectTransform m_Content;

        private float AnchoredPosition
        {
            get
            {
                return (direction == Direction.Vertical) ?
                    -m_Content.anchoredPosition.y :
                    m_Content.anchoredPosition.x;
            }
        }
        private float ItemScale
        {
            get
            {
                return (direction == Direction.Vertical) ?
                    m_itemHeight :
                    m_itemWidth;
            }
        }

        public Vector2 additionalScale = Vector2.zero;

        public void ResetPosition(bool _animation = false)
        {
            if(_animation is true)
            {
                float value = m_Content.localPosition.y;

                m_Content.DOAnchorPosY(0, Const.SCROLL_RESET_DURATION).OnComplete(() =>
                {
                   Vector3 pos = m_Content.localPosition;
                   m_Content.localPosition = new Vector3(pos.x, 0, 0);
                });

            }
            else
            {
                Vector3 pos = m_Content.localPosition;
                m_Content.localPosition = new Vector3(pos.x, 0, 0);
            }
        }

        public void ListStart(int _count)
        {
            if (m_itemList.Count > 0)
            {
                /// 이미 생성을 한 후에는 포지션만 잡아주기.
                for (int i = 0; i < m_itemList.Count; i++)
                {
                    var item = m_itemList[i];

                    item.trf.anchoredPosition =
                    (direction == Direction.Vertical) ?
                    new Vector2(gap, -additionalScale.y -gap - (ItemScale + gap) * i) :
                    new Vector2((ItemScale + gap) * i + gap + additionalScale.x, -gap);

                    item.token.Init();
                    item.token.UpdateItem(i);
                }

                dataCount = _count;
                SetContentSize();
                ResetPosition();

                return;
            }

            if (m_ItemBase is null)
            {
                Debug.LogError($"{transform.name} :: item base is null");
                return;
            }

            dataCount = _count;

            //if (dataCount < m_instantiateItemCount)
            //{
            //    m_instantiateItemCount = dataCount;
            //}
            //else
            //{
            //    m_instantiateItemCount = (direction == Direction.Vertical) ?
            //        Mathf.RoundToInt(Screen.height / ItemScale) + 3 :
            //        Mathf.RoundToInt(Screen.width / ItemScale) + 3;
            //}

            // create items
            scrollRect.horizontal = direction == Direction.Horizontal;
            scrollRect.vertical = direction == Direction.Vertical;

            SetContentSize();

            scrollRect.onValueChanged.AddListener(OnValueChanged);
            //Debug.Log(m_instantiateItemCount);
            m_ItemBase.gameObject.SetActive(false);
            for (int i = 0; i < m_instantiateItemCount; i++)
            {
                var item = GameObject.Instantiate(m_ItemBase, transform) as RectTransform;
                //item.SetParent(transform, false);
                item.name = i.ToString();

                if (direction == Direction.Vertical)
                {
                    //item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_itemWidth - gap * 2);
                    item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_itemHeight);
                }
                else
                {
                    item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_itemWidth);
                    //item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_itemHeight);
                }

                item.anchoredPosition =
                    (direction == Direction.Vertical) ?
                    new Vector2(gap, -additionalScale.y -gap - (ItemScale + gap) * i) :
                    new Vector2((ItemScale + gap) * i + gap + additionalScale.x, -gap);

                var itemToken = item.GetComponent<RecycleScroll_Item>();

                m_itemList.Add(new Item()
                {
                    trf = item,
                    token = itemToken
                });

                item.gameObject.SetActive(true);
                itemToken.Init();
                itemToken.UpdateItem(i);
            }

            Init();
        }

        public void SetContentSize(int _addCount = 0)
        {
            dataCount += _addCount;

            if (direction == Direction.Vertical)
            {
                m_Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((ItemScale + gap) * dataCount) - gap + additionalScale.y);
                //m_Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_itemWidth + gap * 2);
            }
            else
            {
                m_Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((ItemScale + gap) * dataCount) - gap + additionalScale.x);
                //m_Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_itemHeight + gap * 2);
            }
        }


        public void OnValueChanged(Vector2 _pos)
        {
            // scroll up, item attach bottom  or  right
            while (AnchoredPosition - m_diffPreFramePosition < -(ItemScale + gap) * 2 - additionalScale.y)
            {
                m_diffPreFramePosition -= (ItemScale + gap);

                var item = m_itemList[0];

                var pos = additionalScale.y + (ItemScale + gap) * m_instantiateItemCount + (ItemScale + gap) * m_currentItemNo;
                bool checkEndPoint = CheckEndPoint(pos);

                if (checkEndPoint is true)
                {
                    m_diffPreFramePosition += (ItemScale + gap);
                    return;
                }

                item.trf.anchoredPosition = (direction == Direction.Vertical) ? new Vector2(gap, -pos - gap) : new Vector2(pos + gap, -gap);
                m_itemList.RemoveAt(0);
                m_itemList.Add(item);

                m_currentItemNo++;

                if (m_currentItemNo + m_instantiateItemCount - 1 < dataCount)
                {
                    item.token.UpdateItem(m_currentItemNo + m_instantiateItemCount - 1);
                }
                else
                {
                    OnEndScroll(
                        _update: () =>
                        {
                            item.token.UpdateItem(m_currentItemNo + m_instantiateItemCount - 1);
                        });
                }
            }

            // scroll down, item attach top  or  left
            while (AnchoredPosition - m_diffPreFramePosition > 0 - additionalScale.y)
            {
                if (m_currentItemNo <= 0)
                {
                    if (AnchoredPosition - m_diffPreFramePosition > 350f)
                    {
                        OnTopScroll();
                        break;
                    }

                    return;
                }

                m_diffPreFramePosition += (ItemScale + gap);

                var itemListLastCount = m_instantiateItemCount - 1;
                var item = m_itemList[itemListLastCount];
                m_itemList.RemoveAt(itemListLastCount);
                m_itemList.Insert(0, item);

                m_currentItemNo--;

                var pos = additionalScale.y + (ItemScale + gap) * m_currentItemNo + gap;
                item.trf.anchoredPosition = (direction == Direction.Vertical) ? new Vector2(gap, -pos) : new Vector2(pos, -gap);

                if (m_currentItemNo > -1)
                {
                    item.token.UpdateItem(m_currentItemNo);
                }
                else
                {
                    item.token.UpdateItem(-100);
                }
            }
        }

        protected virtual void OnTopScroll()
        {
        }

        protected virtual void OnEndScroll(Action _update)
        {
        }

        protected virtual bool CheckEndPoint(float itemPos)
        {
            return Math.Abs(itemPos) >= Math.Abs(m_Content.sizeDelta.y);
        }

        protected virtual void Init()
        {
        }

        public virtual void Refresh()
        {
            foreach (var v in m_itemList)
            {
                v.token.Refresh();
            }
        }

        public void Repaint()
        {
            foreach(var v in m_itemList)
            {
                v.token.Repaint();
            }
        }
    }
}