using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class UIPopup_BottomSheet_More : UIPopup
    {
        [Header("Edit")]
        public Button btn_Edit;
        [Header("Delete")]
        public Button btn_Delete;
        [Header("Report")]
        public Button btn_Report;

        private Feed feed;
        private Comment comment;
        private UIPopup_Comments commentsPopup;

        public enum TYPE
        {
            Feed = 0,
            Comment
        }

        private TYPE type = TYPE.Feed;

        public override void Awake()
        {
            base.Awake();

            btn_Edit.onClick.AddListener(OnClickEdit);
            btn_Delete.onClick.AddListener(OnClickDelete);
            btn_Report.onClick.AddListener(OnClickReport);
        }

        /// <summary>
        /// args[0] : type 0 = feed, 1 = comment
        /// </summary>
        /// <param name="args"></param>
        public override void UpdateData(params object[] args)
        {
            type = (TYPE)args[0];

            bool isMine = false;
            bool isMyFeed = false;

            switch (type)
            {
                case TYPE.Feed:
                    feed = (Feed)args[1];

                    isMine = DC.IsMine(feed.username);

                    btn_Edit.SetActive(isMine);
                    btn_Delete.SetActive(isMine);
                    btn_Report.SetActive(!isMine);
                    break;

                case TYPE.Comment:
                    feed = DC.SelectedFeed;
                    comment = (Comment)args[1];

                    isMine = DC.IsMine(comment.username);
                    isMyFeed = DC.IsMine(feed.username);

                    btn_Edit.SetActive(isMine);
                    btn_Delete.SetActive(isMine || isMyFeed);
                    btn_Report.SetActive(!isMine);

                    commentsPopup = UIManager.s.FindPopup<UIPopup_Comments>();
                    break;
            }

            UIManager.s.Dim = true;
        }

        public override void Close()
        {
            this.Hide();
            UIManager.s.Dim = false;
        }

        private void OnClickReport()
        {
            if (PPM.GuestLogin is true)
            {
                Close();
                UIManager.s.OpenSelectLogin();
                return;
            }

            long feedId = feed is null ? 0 : feed.id;
            long commentId = comment is null ? 0 : comment.id;

            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_Report>(true, type, feedId, commentId);
            this.Close();
        }

        private void OnClickEdit()
        {
            switch (type)
            {
                case TYPE.Feed:
                    //UIManager.s.ClosePopup<UIPopup_FeedDetail>();
                    UIManager.s.OpenPopupWithData<UIPopup_UploadFeed>(_addtive: true, _args: true);
                    break;

                case TYPE.Comment:
                    commentsPopup.SetEditComment(comment.id, comment.content);
                    this.Close();
                    break;
            }

            this.Close();
        }

        private void OnClickDelete()
        {
            this.Close();

            switch (type)
            {
                case TYPE.Feed:

                    UIManager.s.OpenTwoButton(Const.TOAST_FEED_DELETE_TITLE, Const.TOAST_FEED_DELETE_DESC,
                        _left: () =>
                        {

                        },
                        _right: () =>
                        {
                            NetworkManager.s.DeleteFeed(DC.SelectedFeed.id, () =>
                            {
                                UIManager.s.OpenToast(Const.TOAST_FEED_DELETE_COMPLETE);
                                UIManager.s.RefreshFeed();
                                UIManager.s.ClosePopup<UIPopup_FeedDetail>();

                                var popup_my = UIManager.s.FindValue<UIPopup_My>();

                                if (popup_my.isShow)
                                {
                                    UIPopup_My popup = (UIPopup_My)popup_my.popup;
                                    popup.GetMyFeedListRpeat();
                                }

                            }, (error) =>
                            {
                                UIManager.s.OpenToast(error);

                            });
                        },
                        _leftText: Const.CANCEL,
                        _rightText: Const.DELTE);

                    break;

                case TYPE.Comment:

                    UIManager.s.OpenTwoButton(Const.TOAST_COMMENT_DELETE_TITLE, Const.TOAST_COMMENT_DELETE_DESC,
                        _left: () =>
                        {
                        },
                        _right: () =>
                        {
                            NetworkManager.s.DeleteComment(DC.SelectedFeed.id, comment.id, (result) =>
                            {
                                UIManager.s.OpenToast(Const.TOAST_COMMENT_DELETE_COMPLETE);
                                commentsPopup.Refresh();

                                var feed = DC.GetFeed(DC.SelectedFeed.id);
                                feed.comment_count--;
                                UIManager.s.FindPopup<UIPopup_Feed>().Repaint();
                            }, (error) =>
                            {
                                UIManager.s.OpenToast(error);
                            });
                        },
                        _leftText: Const.CANCEL,
                        _rightText: Const.DELTE);

                    break;
            }
        }
    }
}