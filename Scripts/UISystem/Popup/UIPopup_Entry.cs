using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;
using DF = Marvrus.Util.Define;
using OB = Marvrus.Util.OpenBrowser; 
using System.Net;

namespace Marvrus.UI
{
    public class UIPopup_Entry : UIPopup
    {
        [Header("for Application")]
        public GameObject app_parent;
        public Button app_login;
        public Button app_signUp;
        public Button app_guestLogin;

        [Header("for Editor")]
        public GameObject editor_parent;
        public Button marvrusLoginBtn;
        public Button naverLoginBtn;
        public Button kakaoLoginBtn;
        public Button facebookLoginBtn;
        public Button guestLoginBtn;

        public override void Awake()
        {
            base.Awake();

            if (DF.platform == DF.Platform.UnityEditor)
            {
                app_parent.SetActive(false);
                editor_parent.SetActive(true);

                marvrusLoginBtn.onClick.AddListener(OnClickMarvrus);
                naverLoginBtn.onClick.AddListener(OnClickNaver);
                kakaoLoginBtn.onClick.AddListener(OnClickKakao);
                facebookLoginBtn.onClick.AddListener(OnClickFaceBook);
                guestLoginBtn.onClick.AddListener(OnClickGuest);
            }
            else
            {
                app_parent.SetActive(true);
                editor_parent.SetActive(false);

                app_login.onClick.AddListener(OnClickAppLogin);
                app_signUp.onClick.AddListener(OnClickAppSignUp);
                app_guestLogin.onClick.AddListener(OnClickGuest);
            }
        }

        public override void UpdateData(params object[] args)
        {
            if (DF.platform == DF.Platform.UnityEditor)
            {
                bool isAutoLogin = PPM.AutoLogin;
                bool isGuestLogin = PPM.GuestLogin;

                if (isAutoLogin || isGuestLogin)
                {
                    UIManager.s.Dim = true;
                    if (isAutoLogin) AutoLogin();
                    if (isGuestLogin) AutoGuestLogin();
                }
            }
            else
            {
                Debug.Log($"{PPM.RefreshToken} // {PPM.ID}");

                if (string.IsNullOrEmpty(PPM.RefreshToken) is false &&
                    string.IsNullOrEmpty(PPM.ID) is false)
                {
                    AutoLogin();
                }
            }
        }

        private void AutoLogin()
        {
            Protocol.Request.AutoLogin data = new()
            {
                id = PPM.ID,
                refreshToken = PPM.RefreshToken,
                clientId = PPM.ClientID
            };

            NetworkManager.s.AutoLogin(data, (result) =>
            {
                UIManager.s.Dim = false;
                UIManager.s.OpenFeed();

                if(string.IsNullOrEmpty(result.data.MarvrusUser.provider) is false)
                {
                    bool isMarvrusUser = result.data.MarvrusUser.provider.Equals(Const.PROVIDER_MARVRUS_USER);
                    PPM.SNS_Login = !isMarvrusUser;
                }

            }, (error) =>
            {
                Debug.LogError(error);
                PPM.AutoLogin = false;
                UIManager.s.Dim = false;
            });
        }

        private void AutoGuestLogin()
        {
            Protocol.Request.GuestLogin guestData = new Protocol.Request.GuestLogin(
                    _clientId: PPM.ClientID, _id: PPM.ID
                );

            NetworkManager.s.GuestLogin(guestData, (result) =>
            {
                UIManager.s.Dim = false;
                UIManager.s.OpenFeed();
            }, (error) =>
            {
                Debug.LogError(error);
                PPM.GuestLogin = false;
                UIManager.s.Dim = false;
            });
        }

        public void OnClickMarvrus()
        {
            UIManager.s.OpenPopupWithData<UIPopup_Login>(true);
        }

        public void OnClickNaver()
        {
            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_SNSAuth>(true, 0);
        }

        public void OnClickKakao()
        {
            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_SNSAuth>(true, 1);
        }

        public void OnClickFaceBook()
        {
            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_SNSAuth>(true, 2);
        }

        public void OnClickGuest()
        {
            UIManager.s.OpenTwoButton(Const.POPUP_GUEST_TITLE,
                Const.POPUP_GUEST_DESC,
                _right: () =>
                {
                    object[] data = new object[] {
                        UIPopup_AcceptTerms.SIGNUP_TYPE.GUEST
                    };
                    UIManager.s.OpenPopupWithData<UIPopup_AcceptTerms>(true, data);
                }, _leftText: Const.CANCEL, _rightText: Const.POPUP_GUEST_OK);
        }

        public void OnClickAppLogin()
        {
            Debug.Log("OnClickLogin");

            OB.SignIn((result) =>
            {
                Debug.Log("SignIn redirect");
                AppLogin(result);
            });
        }

        public void OnClickAppSignUp()
        {
            Debug.Log("OnClickSignUp");

            OB.SignUp((result) =>
            {
                Debug.Log("SignUp redirect");
                AppLogin(result);
            });
        }

        void AppLogin(string _result)
        {
            string[] parameters = _result.Split("&");
            string userId = parameters[0].Split("=")[1];
            string refreshToken = parameters[1].Split("=")[1];

            userId = WebUtility.UrlDecode(userId);
            refreshToken = WebUtility.UrlDecode(refreshToken);

            PPM.ID = userId;
            PPM.RefreshToken = refreshToken;
            AutoLogin();
        }
    }
}