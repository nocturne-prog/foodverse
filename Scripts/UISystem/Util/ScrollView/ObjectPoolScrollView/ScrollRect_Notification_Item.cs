using Marvrus.Util;
using Marvrus.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;
using System.Linq.Expressions;

namespace Marvrus.Scroll
{
    public class ScrollRect_Notification_Item : ObjectPoolScrollView_Item
    {
        private Button btn;

        public ProfileImageItem profile;
        public TextMeshProUGUI content;
        public RawImage thumbnail;

        Notification data;
        UIPopup_Notification popup;

        private void Awake()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(OnClickItem);
        }

        public override void UpdateItem(int _index)
        {
            if (_index >= DC.data_totalNotification.Count)
                return;

            data = DC.data_totalNotification[_index];

            if (string.IsNullOrEmpty(data.sender_profile.picture_url) is false)
            {
                FileCache.s.GetImage(data.sender_profile.picture_url, Const.IMAGE_SIZE_MY, (texture) =>
                {
                    profile.texture = texture;
                });
            }

            if (string.IsNullOrEmpty(data.thumbnail_url) is false)
            {
                FileCache.s.GetImage(data.thumbnail_url, Const.IMAGE_SIZE_MY_ACTION, (texture) =>
                {
                    thumbnail.texture = texture;
                });
            }

            content.text = $"{data.title}:"
                            + $"<color=#0057FF>@{data.sender_name}</color> {data.message}\n"
                            + $"<color=#6D6D6D>{Extension.CalculateDate(data.sent_at)}</color>";

            gameObject.SetActive(true);
        }

        public override void Clear()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            btn = null;
            profile = null;
            content = null;
            thumbnail = null;
            data = null;
            popup = null;
        }

        private void OnClickItem()
        {
            var feed = DC.GetFeed(data.article_id);
            UIManager.s.OpenPopupWithData<UIPopup_FeedDetail>(true, feed);
        }
    }
}