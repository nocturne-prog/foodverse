using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;
using System.Collections.Generic;
using System.Text;
using PPM = Marvrus.Util.PlayerPrefsManager;
using Marvrus.Scroll;
using Marvrus.Util;

namespace Marvrus.UI
{
    public class UIPopup_Comments : UIPopup
    {
        public Button topBarBtn;
        public GameObject noCommentObj;
        public ScrollRect_Comment commentScroll;
        public RectTransform commentInputFieldParent;
        public TMP_InputField commentInputField;
        public Button commentUploadBtn;
        public GameObject commentUploadBtnBlock;
        public Button guestUploadBlock;

        Feed data;
        bool commentEditMode = false;

        public override void Awake()
        {
            topBarBtn.onClick.AddListener(OnClickTopBarBtn);
            commentUploadBtn.onClick.AddListener(OnClickCommentUpload);
            commentInputField.onEndEdit.AddListener(OnEndEditComment);
            commentInputField.onValueChanged.AddListener(OnValueChangedComment);
            commentInputField.onSelect.AddListener(OnSelectComment);
            guestUploadBlock.onClick.AddListener(OnClickGuestBlock);
            base.Awake();
        }

        private void FixedUpdate()
        {
            commentInputFieldParent.anchoredPosition =
                commentInputField.isFocused ?
                new Vector2(0, PPM.KeyboardHeight * safeArea.ratio) :
                Vector2.zero; 
        }

        public override void UpdateData(params object[] args)
        {
            data = (Feed)args[0];

            Comment[] comments = DC.data_commentList.comments;
            noCommentObj.SetActive(comments.Length == 0);
            commentUploadBtnBlock.SetActive(true);
            commentInputField.text = "";

            Refresh();

            base.UpdateData(args);
        }

        public void Refresh()
        {
            DC.data_totalComment = new List<Comment>();

            NetworkManager.s.GetCommentList(DC.SelectedFeed.id, 0, (result) =>
            {
                commentScroll.ListStart(result.comments.Length);
                noCommentObj.SetActive(result.comments.Length == 0);
            }, (error) =>
            {
                Debug.LogError($"GetComment List :: {error}");
            });

            commentUploadBtnBlock.SetActive(true);
            commentInputField.text = "";
            guestUploadBlock.SetActive(PPM.GuestLogin is true);
        }

        private void OnClickCommentUpload()
        {
            if (string.IsNullOrEmpty(commentInputField.text))
                return;

            if (PPM.GuestLogin is true)
            {
                return;
            }

            if (commentEditMode is false)
            {
                NetworkManager.s.WriteComment(data.id, commentInputField.text, (result) =>
                {
                    UIManager.s.OpenToast(Const.TOAST_COMMENT_WRITE_COMPLETE);
                    Refresh();

                    var feed = DC.GetFeed(data.id);
                    feed.comment_count++;
                    UIManager.s.FindPopup<UIPopup_Feed>().Repaint();
                }, (error) =>
                {
                    UIManager.s.OpenToast(error);
                });
            }
            else
            {
                commentEditMode = false;

                NetworkManager.s.EditComment(data.id, selectedCommentId, commentInputField.text, (result) =>
                {
                    UIManager.s.OpenToast(Const.TOAST_COMMENT_EDIT_COMPLETE);
                    Refresh();
                }, (error) =>
                {
                    UIManager.s.OpenToast(error);
                    Refresh();
                });
            }
        }

        void OnClickTopBarBtn()
        {
            commentScroll.ResetPosition();
        }

        void OnClickGuestBlock()
        {
            if (PPM.GuestLogin is true)
            {
                UIManager.s.OpenSelectLogin();
                return;
            }
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

        private long selectedCommentId = 0;
        public void SetEditComment(long _id, string _text)
        {
            commentEditMode = true;
            commentInputField.text = _text;
            selectedCommentId = _id;
            TouchScreenKeyboard.Open(_text, TouchScreenKeyboardType.Default);
        }
    }
}