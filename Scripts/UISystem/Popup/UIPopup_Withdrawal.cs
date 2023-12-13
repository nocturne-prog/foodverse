using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_Withdrawal : UIPopup
    {
        public Toggle toggle1;
        public Toggle toggle2;
        public GameObject block;
        public Button btn;

        public override void Awake()
        {
            toggle1.onValueChanged.AddListener(OnValueChangedToggle);
            toggle2.onValueChanged.AddListener(OnValueChangedToggle);
            btn.onClick.AddListener(OnClickBtn);

            base.Awake();
        }

        public override void UpdateData(params object[] args)
        {
            toggle1.isOn = false;
            toggle2.isOn = false;
            block.SetActive(true);
            btn.SetActive(false);

            base.UpdateData(args);
        }

        void OnValueChangedToggle(bool _isOn)
        {
            bool agree = CheckAgree();

            block.SetActive(agree is false);
            btn.SetActive(agree is true);
        }

        void OnClickBtn()
        {
            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_Withdrawal>(true);

            //UIManager.s.OpenTwoButton("", Const.POPUP_WITHDRAWAL_DESC, null, () =>
            //{
                

            //}, Const.CANCEL, Const.WITHDRAWAL);
        }

        bool CheckAgree()
        {
            return toggle1.isOn && toggle2.isOn;
        }
    }
}