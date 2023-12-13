using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_BottomSheet_SignUp : UIPopup
    {
        public Button overButton;
        public Button underButton;

        public override void Awake()
        {
            base.Awake();

            overButton.onClick.AddListener(OnClickOverSignUp);
            underButton.onClick.AddListener(OnClickUnderSignUp);
        }

        public override void UpdateData(params object[] args)
        {
            UIManager.s.Dim = true;
        }

        public override void Close()
        {
            base.Close();
            UIManager.s.Dim = false;
        }

        private void OnClickOverSignUp()
        {
            Close();

            object[] data = new object[] {
                UIPopup_AcceptTerms.SIGNUP_TYPE.MARVRUS_OVER
            };
            UIManager.s.OpenPopupWithData<UIPopup_AcceptTerms>(true, data);
        }

        private void OnClickUnderSignUp()
        {
            Close();

            object[] data = new object[] {
                UIPopup_AcceptTerms.SIGNUP_TYPE.MARVRUS_UNDER
            };
            UIManager.s.OpenPopupWithData<UIPopup_AcceptTerms>(true, data);
        }
    }
}
