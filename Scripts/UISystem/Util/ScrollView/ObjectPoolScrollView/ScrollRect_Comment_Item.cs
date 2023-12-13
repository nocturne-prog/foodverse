using System.Collections;
using System.Collections.Generic;
using Marvrus.Util;
using Marvrus.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.Scroll
{
    public class ScrollRect_Comment_Item : ObjectPoolScrollView_Item
    {
        [Header("User Profile")]
        public ProfileImageItem profile;
        public TextMeshProUGUI userName;
        public TextMeshProUGUI createAt;

        [Header("Comment")]
        public TextMeshProUGUI comment;

        [Header("More Info")]
        public Button moreBtn;

        [Header("Like")]
        public Toggle likeBtn;
        public TextMeshProUGUI likeCount;

        [Header("ChildRoot")]
        public Transform childRoot;

        private Comment data;

        private void Awake()
        {
            moreBtn.onClick.AddListener(OnClickMore);
        }

        public override void UpdateItem(int _index)
        {
            if (_index >= DC.data_totalComment.Count)
                return;

            data = DC.data_totalComment[_index];

            UpdateData(data, true);
        }

        public void UpdateData(Comment _data, bool showMoreBtn = true)
        {
            data = _data;

            profile.texture = null;

            if (string.IsNullOrEmpty(data.picture_url) is false)
            {
                FileCache.s.GetImage(data.picture_url, Const.IMAGE_SIZE_MY, (texture) =>
                {
                    profile.texture = texture;
                });
            }

            bool isMine = DC.IsMine(_data.username);

            userName.text = data.username;
            createAt.text = Extension.CalculateDate(data.created_at);
            comment.text = data.content;
            likeBtn.SetActive(false);
            likeCount.SetActive(false);
            gameObject.SetActive(true);

            SetMoreBtn(showMoreBtn);
        }

        public void OnClickMore()
        {
            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_More>(true, UIPopup_BottomSheet_More.TYPE.Comment, data);
        }

        public void SetMoreBtn(bool _active)
        {
            moreBtn.SetActive(_active);
        }

        public override void Clear()
        {
            profile.texture = null;
            userName.text = string.Empty;
            createAt.text = string.Empty;
            comment.text = string.Empty;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            profile.texture = null;
            userName.text = string.Empty;
            createAt.text = string.Empty;
            comment.text = string.Empty;
        }
    }
}