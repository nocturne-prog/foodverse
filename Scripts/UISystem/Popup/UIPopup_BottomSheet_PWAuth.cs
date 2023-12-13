using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class UIPopup_BottomSheet_PWAuth : UIPopup
    {
        public ScrollRectDragExit exitScroll;
        public TMP_InputField pwdInputField;
        public GameObject okBtnBlock;
        public Button okBtn;
        public GameObject missMatchMsg;

        private string pwd = string.Empty;

        public override void Awake()
        {
            base.Awake();

            exitScroll.ExitEvent = () =>
            {
                this.Hide();
            };

            pwdInputField.onEndEdit.AddListener(OnEndEditPwd);
            okBtn.onClick.AddListener(OnClickConfirm);
        }

        public override void UpdateData(params object[] args)
        {
            missMatchMsg.SetActive(false);
            okBtnBlock.SetActive(true);
            UIManager.s.Dim = true;
        }

        public override void Close()
        {
            this.Hide();
            UIManager.s.Dim = false;
        }

        public void OnEndEditPwd(string text)
        {
            pwd = text;
            okBtnBlock.SetActive(string.IsNullOrEmpty(text));
            missMatchMsg.SetActive(false);
        }

        public void OnClickConfirm()
        {
            NetworkManager.s.CheckPassword(pwd, () =>
            {
                Close();
                UIManager.s.OpenPopupWithData<UIPopup_Withdrawal>(true);
            }, (error) =>
            {
                missMatchMsg.SetActive(true);
            });
        }
    }
}