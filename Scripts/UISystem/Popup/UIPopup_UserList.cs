using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_UserList : UIPopup
    {
        public Button topBarBtn;
        public GameObject title_like;
        public GameObject title_Comment;
        public Scroll.RecycleScroll_UserList scroll;

        public override void Awake()
        {
            base.Awake();

            topBarBtn.onClick.AddListener(OnClickTopBarBtn);
        }

        public override void UpdateData(params object[] args)
        {
            bool isLike = (bool)args[0];

            title_like.SetActive(isLike);
            title_Comment.SetActive(!isLike);

            scroll.Refresh();
            scroll.ListStart(Data.DataContainer.data_userList.Length);
        }

        void OnClickTopBarBtn()
        {
            scroll.ResetPosition(true);
        }
    }
}