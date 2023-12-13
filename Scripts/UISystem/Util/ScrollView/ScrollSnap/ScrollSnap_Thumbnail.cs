using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Marvrus.Util;

namespace Marvrus.Scroll
{
    public class ScrollSnap_Thumbnail : ScrollSnap
    {
        public int ItemCount
        {
            get { return itemCount; }
            set { itemCount = value; }
        }

        [Header("Thumbnail")]
        public RawImage thumbnailBase;
        private List<RawImage> thumbnailList = new List<RawImage>();
        private List<Button> thumbnailButtonList = new List<Button>();

        [Header("Indicator")]
        public IndicatorItem indicatorBase;
        private List<IndicatorItem> indicatorList = new List<IndicatorItem>();

        string[] url_path;

        protected override void Awake()
        {
            scrollRect = gameObject.GetComponent<ScrollRect>();
            thumbnailBase.SetActive(false);
            indicatorBase.SetActive(false);

            OnValueChanged = (_index) =>
            {
                for (int i = 0; i < indicatorList.Count; i++)
                {
                    indicatorList[i].On = i == _index;
                }
            };
        }

        public void Init()
        {
            for (int i = 0; i < Const.MAX_MEDIA_COUNT; i++)
            {
                GameObject thumbnail = Instantiate(thumbnailBase.gameObject, thumbnailBase.transform.parent);
                thumbnailList.Add(thumbnail.GetComponent<RawImage>());
                thumbnail.transform.SetAsLastSibling();

                Button thumbnailButton = thumbnail.GetComponent<Button>();

                if (thumbnailButton != null)
                {
                    int buttonIndex = thumbnailButtonList.Count;
                    thumbnailButton.onClick.AddListener(() => OnClickImage(buttonIndex));
                    thumbnailButtonList.Add(thumbnailButton);
                }

                GameObject indicator = Instantiate(indicatorBase.gameObject, indicatorBase.transform.parent);
                indicatorList.Add(indicator.GetComponent<IndicatorItem>());
                indicator.transform.SetAsLastSibling();
            }
        }

        public void UpdateThumbnails(Feed _data)
        {
            string[] data = new string[_data.medias.Length];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = _data.medias[i].thumbnail_url;
            }

            UpdateThumbnails(data);
        }

        public void UpdateThumbnails(string[] _data, bool _local = false)
        {
            url_path = _data;

            for (int i = 0; i < thumbnailList.Count; i++)
            {
                int index = i;

                thumbnailList[index].texture = null;

                if (i >= url_path.Length || string.IsNullOrEmpty(url_path[i]))
                {
                    thumbnailList[index].SetActive(false);
                    indicatorList[index].SetActive(false);
                }
                else
                {
                    thumbnailList[index].SetActive(true);
                    indicatorList[index].SetActive(true);
                    indicatorList[index].On = i == 0;

                    if (_local is true)
                    {
                        FileCache.s.LoadImage(url_path[index], Const.IMAGE_SIZE_IMAGE_DETAIL, (texture) =>
                        {
                            thumbnailList[index].texture = texture;
                        });
                    }
                    else
                    {
                        FileCache.s.GetImage(url_path[index], Const.IMAGE_SIZE_IMAGE_DETAIL, (texture) =>
                        {
                            thumbnailList[index].texture = texture;
                        });
                    }
                }
            }

            if(_data.Length == 1)
            {
                for(int i =0; i< indicatorList.Count; i++)
                {
                    indicatorList[i].SetActive(false);
                }
            }
        }

        public void Clear()
        {
            Refresh();

            for (int i = 0; i < thumbnailList.Count; i++)
            {
                thumbnailList[i].SetActive(false);
                indicatorList[i].SetActive(false);
            }
        }

        private void OnClickImage(int _index)
        {
            Debug.Log($"OnClickImage :: {_index}");
            UIManager.s.OpenPopupWithData<UI.UIPopup_FullScreenPhoto>(true, url_path);
        }
    }
}