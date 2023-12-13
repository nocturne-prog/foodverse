using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;
using PPM = Marvrus.Util.PlayerPrefsManager;
using Marvrus.UI;
using Marvrus.Util; 
using DG.Tweening;

namespace Marvrus.Scroll
{
    public class RecycleScroll_Feed_Item : RecycleScroll_Item
    {
        [Header("Scroll Snap")]
        public ScrollSnap_Thumbnail snap;

        [Header("UserInfo")]
        public ProfileImageItem userIcon;
        public TextMeshProUGUI userText;
        public TextMeshProUGUI userDate;
        public Button userMoreBtn;

        [Header("Content")]
        public TextMeshProUGUI content;
        public Button contentBtn;

        [Header("Like")]
        public Toggle likeBtn;
        public DOTweenAnimation likeAni;
        public GameObject likeParticle;
        public TextMeshProUGUI likeCount;
        private Button likeConuntBtn;

        [Header("Comment")]
        public Button commentBtn;
        public TextMeshProUGUI commentCount;

        [Header("Loading")]
        public GameObject loading;
        private bool Loading
        {
            set { loading.SetActive(value); }
        }

        private Feed data;

        private void Start()
        {
            userMoreBtn.onClick.AddListener(OnClickUserMore);

            contentBtn = content.gameObject.GetComponent<Button>();
            contentBtn.onClick.AddListener(() => OnClickContent());

            likeBtn.onValueChanged.AddListener((isOn) => OnClickLike(isOn));
            likeConuntBtn = likeCount.GetComponent<Button>();
            likeConuntBtn.onClick.AddListener(OnClickLikeCount);

            commentBtn.onClick.AddListener(OnClickComment);
        }

        public override void Init()
        {
            snap.Init();
        }

        public override void UpdateItem(int _index)
        {
            if (_index < 0 || _index >= DC.data_totalFeed.Count)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                gameObject.SetActive(true);
            }

            data = DC.data_totalFeed[_index];
            snap.ItemCount = data.medias.Length;
            snap.UpdateThumbnails(data);

            if (string.IsNullOrEmpty(data.picture_url) is true)
            {
                userIcon.texture = null;
            }
            else
            {
                FileCache.s.GetImage(data.picture_url, Const.IMAGE_SIZE_FEED, (texture) =>
                {
                    userIcon.texture = texture;
                });
            }

            userText.text = $"{data.username}";
            userDate.text = Extension.CalculateDate(data.created_at);
            content.text = Extension.PrintContents(data.content, 50);
            likeBtn.SetIsOnWithoutNotify(data.liked);
            likeCount.text = data.like_count < 1 ? "" : $"{data.like_count}";
            commentCount.text = data.comment_count < 1 ? "" : $"{data.comment_count}";

            Loading = false;
        }

        public override void Repaint()
        {
            likeBtn.SetIsOnWithoutNotify(data.liked);
            likeCount.text = data.like_count < 1 ? "" : $"{data.like_count}";
            commentCount.text = data.comment_count < 1 ? "" : $"{data.comment_count}";
        }

        private void OnClickContent()
        {
            Debug.Log($"OnClickContent !!");
            UIManager.s.OpenPopupWithData<UIPopup_FeedDetail>(true, data);
        }

        private void OnClickUserMore()
        {
            DC.SelectedFeed = data;
            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_More>(true, UIPopup_BottomSheet_More.TYPE.Feed, data);
        }

        private void OnClickLike(bool _isOn)
        {
            if(PPM.GuestLogin is true)
            {
                UIManager.s.OpenSelectLogin();
                likeBtn.SetIsOnWithoutNotify(false);
                return;
            }

            Debug.Log($"OnClickLike :: {_isOn}");
            NetworkManager.s.Like(_isOn, data.id, (result) =>
            {
                if (_isOn is true)
                {
                    likeAni.DORestart();
                }

                likeParticle.SetActive(_isOn);
                likeCount.text = result.like_count < 1 ? "" : $"{result.like_count}";

                var feed = DC.GetFeed(data.id);

                feed.like_count = result.like_count;
                feed.liked = _isOn;
            });
        }

        private void OnClickLikeCount()
        {
            Debug.Log("OnClickLikeCount !!");
            NetworkManager.s.GetLikeUserList(data.id, (result) =>
            {
                UIManager.s.OpenPopupWithData<UIPopup_UserList>(true, true);
            });
        }

        private void OnClickComment()
        {
            //Debug.Log("OnClickComment !!");
            //NetworkManager.s.GetCommentUserList(data.id, (result) =>
            //{
            //    UIManager.s.OpenPopupWithData<UIPopup_UserList>(true, false);
            //});

            DC.SelectedFeed = data;

            NetworkManager.s.GetCommentList(data.id, 0, (result) =>
            {
                UIManager.s.OpenPopupWithData<UIPopup_Comments>(true, data);
            }, (error) =>
            {
            });
        }

        public void RefreshCommentCount()
        {
            commentCount.text = data.comment_count < 1 ? "" : $"{data.comment_count}";
        }

        //private void OnClickCommentCount()
        //{
        //    Debug.Log("OnClickCOmmentCount !!");
        //}
    }
}