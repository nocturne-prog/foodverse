using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marvrus.UI
{
    public class UIPopup_PrivacyPolicy_Under14 : UIPopup
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void Close()
        {
            base.Close();
            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_PhoneAuth_Protector>(true);
        }
    }
}
