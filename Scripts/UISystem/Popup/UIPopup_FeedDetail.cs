using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UIPopup_FeedDetail : UIPopup
    {
        [Header("DoTween Ani")]
        public DOTweenAnimation ani;

        [Header("ScrollSnap")]
        public ScrollRectDragExit scroll;

        [Header("Thumbnail")]
        public Scroll.ScrollSnap_Thumbnail thumbnail;

        [Header("UserInfo")]
        public Util.ProfileImageItem userIcon;
        public TextMeshProUGUI userText;
        public TextMeshProUGUI userDate;
        public Button userMoreBtn;

        [Header("Content")]
        public TextMeshProUGUI content;

        [Header("Like")]
        public Toggle likeBtn;
        public TextMeshProUGUI likeCount;
        private Button likeConuntBtn;

        [Header("Comment")]
        public Button commentBtn;
        public TextMeshProUGUI commentCount;
        public TMP_InputField commentInputField;
        public GameObject commentUploadBtnBlock;
        public Button commentUploadBtn;
        public Scroll.ScrollRect_Comment_Item[] commentList;
        public Button commentMoreBtn;
        public Button guestUploadBlock;

        public override void Awake()
        {
            base.Awake();

            userMoreBtn.onClick.AddListener(OnClickUserMore);

            likeBtn.onValueChanged.AddListener((isOn) => OnClickLike(isOn));
            likeConuntBtn = likeCount.GetComponent<Button>();
            likeConuntBtn.onClick.AddListener(OnClickLikeCount);

            commentBtn.onClick.AddListener(OnClickComment);
            commentMoreBtn.onClick.AddListener(OnClickComment);
            commentUploadBtn.onClick.AddListener(OnClickCommentUpload);
            commentInputField.onEndEdit.AddListener(OnEndEditComment);
            commentInputField.onValueChanged.AddListener(OnValueChangedComment);
            commentInputField.onSelect.AddListener(OnSelectComment);

            thumbnail.Init();
            scroll.ExitEvent = () =>
            {
                ani.DOPlayBackwardsAllById(Const.DG_KEY_UIPOPUP_FEEDDETAIL);
                this.Hide();
            };

            guestUploadBlock.onClick.AddListener(OnClickGuestBlock);
        }

        private void FixedUpdate()
        {
            safeAreaRect.anchoredPosition =
                commentInputField.isFocused ?
                new Vector2(0, PPM.KeyboardHeight * safeArea.ratio) :
                Vector2.zero;
        }

        public override void UpdateData(params object[] args)
        {
            DC.SelectedFeed = (Feed)args[0];
            Refresh();
        }

        private void Refresh()
        {
            var data = DC.SelectedFeed;

            thumbnail.ItemCount = data.medias.Length;
            thumbnail.UpdateThumbnails(data);

            if (string.IsNullOrEmpty(data.picture_url) is true)
            {
                userIcon.texture = null;
            }
            else
            {
                Util.FileCache.s.GetImage(data.picture_url, Const.IMAGE_SIZE_MY, (texture) =>
                {
                    userIcon.texture = texture;
                });
            }

            userText.text = $"{data.username}";
            userDate.text = Extension.CalculateDate(data.created_at);
            content.text = $"{data.content}";
            likeBtn.SetIsOnWithoutNotify(data.liked);
            likeCount.text = data.like_count < 1 ? "" : $"{data.like_count}";
            commentCount.text = data.comment_count < 1 ? "" : $"{data.comment_count}";
            guestUploadBlock.SetActive(PPM.GuestLogin is true);

            foreach (var v in commentList)
                v.Clear();

            NetworkManager.s.GetCommentList(data.id, 0, (result) =>
            {
                int count = Mathf.Min(result.comments.Length, 2);

                for (int i = 0; i < count; i++)
                {
                    Comment c = result.comments[i];
                    commentList[i].UpdateData(c, false);
                }

                commentMoreBtn.SetActive(result.comments.Length > 2);
            }, (error) =>
            {
                Debug.LogError($"GetComment List :: {error}");
            }, _pageSize: 20);

            commentUploadBtnBlock.SetActive(true);
            ClearCommentInputField();
        }

        private void OnClickUserMore()
        {
            Debug.Log("OnClickUserMore !!");
            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_More>(true, UIPopup_BottomSheet_More.TYPE.Feed, DC.SelectedFeed);
        }

        private void OnClickLike(bool _isOn)
        {
            if (PPM.GuestLogin is true)
            {
                UIManager.s.OpenSelectLogin();
                likeBtn.SetIsOnWithoutNotify(false);
                return;
            }

            Debug.Log($"OnClickLike :: {_isOn}");
            NetworkManager.s.Like(_isOn, DC.SelectedFeed.id, (result) =>
            {
                likeCount.text = result.like_count < 1 ? "" : $"{result.like_count}";

                var feed = DC.GetFeed(DC.SelectedFeed.id);

                feed.like_count = result.like_count;
                feed.liked = _isOn;

                UIManager.s.FindPopup<UIPopup_Feed>().Repaint();
            });
        }

        private void OnClickLikeCount()
        {
            Debug.Log("OnClickLikeCount !!");
            NetworkManager.s.GetLikeUserList(DC.SelectedFeed.id, (result) =>
            {
                UIManager.s.OpenPopupWithData<UIPopup_UserList>(true, true);
            });
        }

        private void OnClickComment()
        {
            Debug.Log("OnClickComment !!");
            UIManager.s.OpenPopupWithData<UIPopup_Comments>(true, DC.SelectedFeed);
        }

        private void OnClickCommentUpload()
        {
            if (string.IsNullOrEmpty(commentInputField.text))
                return;

            if (PPM.GuestLogin is true)
            {
                UIManager.s.OpenSelectLogin();
                return;
            }

            NetworkManager.s.WriteComment(DC.SelectedFeed.id, commentInputField.text, (result) =>
            {
                UIManager.s.OpenToast(Const.TOAST_COMMENT_WRITE_COMPLETE);
                ClearCommentInputField();

                DC.SelectedFeed.comment_count++;
                UIManager.s.FindPopup<UIPopup_Feed>().Repaint();
                Refresh();
            }, (error) =>
            {
                UIManager.s.OpenToast(error);
            });
        }

        private void OnClickGuestBlock()
        {
            if (PPM.GuestLogin is true)
            {
                UIManager.s.OpenSelectLogin();
                return;
            }
        }

        private void ClearCommentInputField()
        {
            commentInputField.text = "";
            commentUploadBtnBlock.SetActive(true);
        }

        private void OnSelectComment(string _text)
        {
            commentUploadBtnBlock.SetActive(true);
        }

        private void OnValueChangedComment(string _text)
        {
            int size = Encoding.UTF8.GetByteCount(_text);

            if (size >= Const.MAX_CONTENTS_SIZE)
            {
                UIManager.s.OpenToast(Const.TOAST_EXCESS_TEXT);
            }
        }

        private void OnEndEditComment(string _text)
        {
            if (string.IsNullOrEmpty(_text))
                return;

            int size = Encoding.UTF8.GetByteCount(_text);

            if (size >= Const.MAX_CONTENTS_SIZE)
            {
                return;
            }

            commentUploadBtnBlock.SetActive(false);
        }

        public void SetText(string _text)
        {
            content.text = _text;
        }
    }
}