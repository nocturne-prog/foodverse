using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;
using DF = Marvrus.Util.Define;
using OB = Marvrus.Util.OpenBrowser;
using DC = Marvrus.Data.DataContainer;
using DG.Tweening;
using TMPro;
using Marvrus.Util;

namespace Marvrus.UI
{
    public class UIPopup_Settings : UIPopup
    {
        [System.Serializable]
        public struct Item
        {
            public Button button;
            public Transform arrow;
            public RectTransform list;
        }

        [Header("TopBarBtn")]
        public Button topBarBtn;

        [Header("Scroll content")]
        public RectTransform scrollContent;

        [Header("Vertical Layout Group")]
        public VerticalLayoutGroup layoutGroup;

        [Header("Push")]
        public Item push;
        public ToggleEx pushTotalToggle;
        public ToggleEx pushNightToggle;
        public ToggleEx pushMarketingToggle;
        private bool isOnPush = true;

        [Header("Account")]
        public Item account;
        public Button accountEditProfileBtn;
        public Button accountChangePwdBtn;
        public Button accountLogoutBtn;
        public Button accountWidthdrawal;
        private bool isOnAccount = true;

        [Header("Service Info")]
        public Item serviceInfo;
        public Button serviceTermsOfSereviceBtn;
        public Button servicePrivacyPolicyBtn;
        public TextMeshProUGUI serviceVersionInfoText;
        private bool isOnService = true;

        [Header("Service Center")]
        public Item serviceCenter;
        public Button serviceCenterFAQ;
        public Button serviceCenterCS;
        private bool isOnServiceCenter = true;


        private const int itemSize = 180;
        private float itemWidth = 0;

        public override void Awake()
        {
            itemWidth = push.list.sizeDelta.x;

            base.Awake();

            topBarBtn.onClick.AddListener(OnClickTopBarBtn);

            push.button.onClick.AddListener(() =>
            {
                isOnPush = !isOnPush;
                OnClickListItem(push, isOnPush);
            });

            pushTotalToggle.onClick.AddListener(OnClickPushTotal);
            pushMarketingToggle.onClick.AddListener(OnClickPushMarketing);
            pushNightToggle.onClick.AddListener(OnClickPushNight);

            account.button.onClick.AddListener(() =>
            {
                isOnAccount = !isOnAccount;
                OnClickListItem(account, isOnAccount);
            });

            accountEditProfileBtn.onClick.AddListener(OnClickAccountEditProfile);
            accountChangePwdBtn.onClick.AddListener(OnClickAccountChangePwd);
            accountLogoutBtn.onClick.AddListener(OnClickAccountLogout);
            accountWidthdrawal.onClick.AddListener(OnClickWithdrawal);

            serviceInfo.button.onClick.AddListener(() =>
            {
                isOnService = !isOnService;
                OnClickListItem(serviceInfo, isOnService);
            });

            servicePrivacyPolicyBtn.onClick.AddListener(OnClickServicePrivacyPolicy);
            serviceTermsOfSereviceBtn.onClick.AddListener(OnClickServiceTermsOfSerevice);
            serviceVersionInfoText.text = $"{Application.version}";

            serviceCenter.button.onClick.AddListener(() =>
            {
                isOnServiceCenter = !isOnServiceCenter;
                OnClickListItem(serviceCenter, isOnServiceCenter);
            });
        }

        public override void UpdateData(params object[] args)
        {
            base.UpdateData(args);

            pushTotalToggle.isOn = PPM.ReceptionNotice;
            pushMarketingToggle.isOn = PPM.ReceptionMarketing;
            pushNightToggle.isOn = PPM.ReceptionNight;

            bool isGuest = PPM.GuestLogin;
            bool isSNS = PPM.SNS_Login;

            accountChangePwdBtn.SetActive(isSNS is false && isGuest is false);
            accountWidthdrawal.SetActive(isGuest is false);

            //if (isOnPush is true)
            //{
            //    OnClickListItem(push, isOnPush);
            //}

            if (isOnAccount is true)
            {
                OnClickListItem(account, isOnAccount);
            }

            //if (isOnService is true)
            //{
            //    OnClickListItem(serviceInfo, isOnService);
            //}

            //if (isOnServiceCenter is true)
            //{
            //    OnClickListItem(serviceCenter, isOnServiceCenter);
            //}
        }

        void OnClickTopBarBtn()
        {
            scrollContent.DOAnchorPosY(0, Const.SCROLL_RESET_DURATION);
        }

        #region Push
        public void OnClickPushTotal()
        {
            PPM.ReceptionNotice = pushTotalToggle.isOn;
            UIManager.s.OpenToast(pushTotalToggle.isOn ? Const.TOAST_PUSH_ABLE : Const.TOAST_PUSH_DISABLE);
        }

        public void OnClickPushNight()
        {
            PPM.ReceptionNight = pushNightToggle.isOn;
            UIManager.s.OpenToast(pushNightToggle.isOn ? Const.TOAST_NIGHT_ABLE : Const.TOAST_NIGHT_DISABLE);
        }

        public void OnClickPushMarketing()
        {
            PPM.ReceptionMarketing = pushMarketingToggle.isOn;
            UIManager.s.OpenToast(pushMarketingToggle.isOn ? Const.TOAST_MARKETING_ABLE : Const.TOAST_MARKETING_DISABLE);
        }
        #endregion

        #region Account
        public void OnClickAccountEditProfile()
        {
            if (PPM.GuestLogin is true)
            {
                UIManager.s.OpenSelectLogin();
            }
            else
            {
                UIManager.s.OpenPopupWithData<UIPopup_MyInfo_ProfileModify>(true);
            }
        }

        public void OnClickAccountChangePwd()
        {
            OB.MyProfile((result) =>
            {
            });
        }

        public void OnClickAccountLogout()
        {
            if (DF.platform == DF.Platform.UnityEditor)
            {
                string title = "", desc = "";

                if (PPM.GuestLogin is true)
                {
                    title = Const.POPUP_LOGOUT_DESC;
                    desc = Const.POPUP_LOGOUT_DESC_GUEST;
                }
                else
                {
                    desc = Const.POPUP_LOGOUT_DESC;
                }

                UIManager.s.OpenTwoButton(title, desc, null, Logout, Const.CANCEL, Const.LOGOUT);
            }
            else
            {
                OB.LogOut((result) =>
                {
                    PPM.Clear();
                    DC.Clear();
                    UIManager.s.OpenPopupWithData<UIPopup_Splash>();
                });
            }
        }

        private void Logout()
        {
            NetworkManager.s.Logout((result) =>
            {
                PPM.Clear();
                DC.Clear();
                UIManager.s.OpenPopupWithData<UIPopup_Splash>();
            }, (error) =>
            {
                Debug.LogError(error);
            });
        }

        private void OnClickWithdrawal()
        {
            if (DF.platform == DF.Platform.UnityEditor)
            {
                UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_PWAuth>(true);
            }
            else
            {
                OB.MyProfile((result) =>
                {
                   if (result.Contains("profile_withdrawal"))
                   {
                       PPM.Clear();
                       DC.Clear();
                       UIManager.s.OpenPopupWithData<UIPopup_Splash>();
                   }
                });
            }
        }
        #endregion

        #region Service Info
        public void OnClickServiceTermsOfSerevice()
        {
            UIManager.s.OpenPopup<UIPopup_TermsofService>(true);
        }
        public void OnClickServicePrivacyPolicy()
        {
            UIManager.s.OpenPopup<UIPopup_PrivacyPolicy>(true);
        }
        public void OnClickServiceVersionInfo() { }

        #endregion

        #region Service Center
        public void OnClickServiceCenterFnQ()
        {
        }

        public void OnClickServiceCenterCS()
        {
        }
        #endregion

        #region DoTween
        void OnClickListItem(Item _item, bool _isOpen)
        {
            if (_isOpen is true)
                OpenItem(_item);
            else
                CloseItem(_item);
        }

        void OpenItem(Item _item)
        {
            int x = (int)itemWidth;
            int y = GetActiveChildCount(_item.list.transform) * itemSize;

            _item.list.DOSizeDelta(new Vector2(x, y), 0.3f)
                .SetEase(Ease.OutBack)
                .onUpdate += () =>
                {
                    UpdateLayoutGroup();
                };

            _item.arrow.DORotate(new Vector3(0, 0, 90), 0.3f);
        }

        void CloseItem(Item _item)
        {
            int x = (int)itemWidth;
            int y = 0;

            _item.list.DOSizeDelta(new Vector2(x, y), 0.2f)
                .SetEase(Ease.InExpo)
                .onUpdate += () =>
                {
                    UpdateLayoutGroup();
                };

            _item.arrow.DORotate(new Vector3(0, 0, -90), 0.2f);
        }

        int GetActiveChildCount(Transform _trf)
        {
            int count = 0;

            for (int i = 0; i < _trf.childCount; i++)
            {
                if (_trf.GetChild(i).IsActive() is true)
                    count++;
            }

            return count;
        }

        void UpdateLayoutGroup()
        {
            layoutGroup.SetLayoutVertical();
        }
        #endregion
    }
}
