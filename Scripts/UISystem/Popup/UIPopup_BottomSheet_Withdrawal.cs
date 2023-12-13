using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UIPopup_BottomSheet_Withdrawal : UIPopup
    {
        public Toggle[] toggles;
        public Button btn;
        public InputField inputField;

        string reason;
        string reasonDetail;

        public override void Awake()
        {
            btn.onClick.AddListener(OnClickBtn);

            for(int i = 0; i < toggles.Length; i++)
            {
                toggles[i].onValueChanged.AddListener(OnValueChangedToggles);
            }

            inputField.onEndEdit.AddListener(OnEditEndInputField);

            base.Awake();
        }

        public override void UpdateData(params object[] args)
        {
            base.UpdateData(args);
        }

        void OnClickBtn()
        {
            NetworkManager.s.Withdrawal(reason, reasonDetail, (result) =>
            {
                UIManager.s.CloseAll();
                PPM.Clear();
                DC.Clear();
                UIManager.s.OpenPopupWithData<UIPopup_Withdrawal_Completed>(true);
            }, (error) =>
            {
                Debug.LogError(error);
            });
        }

        void OnValueChangedToggles(bool _isOn)
        {
            CheckReason();
        }

        void OnEditEndInputField(string _text)
        {
            if (string.IsNullOrEmpty(_text))
                return;

            for(int i = 0; i < toggles.Length - 1; i++)
            {
                toggles[i].SetIsOnWithoutNotify(false);
            }

            toggles[toggles.Length - 1].isOn = true;
            reasonDetail = _text;
        }

        void CheckReason()
        {
            for(int i = 0; i < toggles.Length; i++)
            {
                if(toggles[i].isOn is true)
                {
                    reason = Const.WHITHDRAWAL_REASON[i];
                    break;
                }
            }

        }
    }
}
