using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.Scroll
{
    [RequireComponent(typeof(ScrollRect))]
    public abstract class ObjectPoolScrollView : MonoBehaviour
    {
        private ScrollRect scrollRect;
        public ObjectPoolScrollView_Item itemBase;

        public int initItemCount = 20;
        private int itemCount = 0;
        private List<ObjectPoolScrollView_Item> itemList = new();

        bool checkEndPoint = false;
        bool isInit = false;

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            itemBase.SetActive(false);

            itemList = new List<ObjectPoolScrollView_Item>();
            Instantiate(initItemCount);

            scrollRect.onValueChanged.AddListener(OnValueChangedScroll);
        }

        public void ListStart(int _count = 0)
        {
            if (isInit is true)
                Clear();

            if (_count == 0)
                _count = initItemCount;

            if (_count > itemList.Count)
            {
                Instantiate(initItemCount);
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                if (i >= _count)
                {
                    itemList[i].SetActive(false);
                    continue;
                }

                UpdateItem(i);
            }

            isInit = true;
        }

        public void UpdateItem(int _index)
        {
            int i = _index;

            itemList[i].UpdateItem(i);
            itemCount++;
        }

        public void Instantiate(int _count)
        {
            for (int i = 0; i < _count; i++)
            {
                itemList.Add(Instantiate(itemBase, scrollRect.content));
            }
        }

        public void Clear()
        {
            itemCount = 0;

            for (int i = initItemCount; i < itemList.Count; i++)
            {
                Destroy(itemList[i].gameObject);
            }

            itemList.RemoveRange(initItemCount, itemList.Count - initItemCount);

            foreach (var v in itemList)
            {
                v.Clear();
            }

            ResetPosition();
        }

        public void ResetPosition()
        {
            scrollRect.content.anchoredPosition = Vector2.zero;
        }

        private void OnValueChangedScroll(Vector2 _pos)
        {
            if (checkEndPoint is true || IsNextPage() is false)
                return;

            if (_pos.y < 0)
            {
                checkEndPoint = true;

                NetworkAction((result) =>
                {
                    checkEndPoint = false;

                    for (int i = 0; i < result; i++)
                    {
                        if (itemCount >= itemList.Count)
                        {
                            Instantiate(initItemCount);
                        }

                        UpdateItem(itemCount);
                    }
                });
            }
        }

        protected abstract bool IsNextPage();
        protected abstract void NetworkAction(Action<int> _callback);

        private void OnDisable()
        {
            Clear();
        }
    }
}
