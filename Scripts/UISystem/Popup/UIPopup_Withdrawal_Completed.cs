using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_Withdrawal_Completed : UIPopup
    {
        public Button btn;

        public override void Awake()
        {
            btn.onClick.AddListener(Close);
        }

        public override void UpdateData(params object[] args)
        {
            base.UpdateData(args);
        }

        public override void Close()
        {
            UIManager.s.OpenPopupWithData<UIPopup_Splash>();
        }
    }
}