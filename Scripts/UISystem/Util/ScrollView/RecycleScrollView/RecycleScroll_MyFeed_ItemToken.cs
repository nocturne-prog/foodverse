using System.Collections;
using System.Collections.Generic;
using Marvrus.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.Scroll
{
    public class RecycleScroll_MyFeed_ItemToken : MonoBehaviour
    {
        public RawImage thumbnail;
        public Button btn;

        private Feed data;

        private void Awake()
        {
            btn.onClick.AddListener(OnClickButton);
        }

        public void UpdateData(Feed _data)
        {
            data = _data;

            gameObject.SetActive(false);

            Util.FileCache.s.GetImage(_data.medias[0].thumbnail_url, Const.IMAGE_SIZE_MY, (texture) =>
            {
                thumbnail.texture = texture;
                gameObject.SetActive(true);
            });
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnClickButton()
        {
            UIManager.s.OpenPopupWithData<UIPopup_FeedDetail>(_addtive: true, _args: data);
        }
    }
}