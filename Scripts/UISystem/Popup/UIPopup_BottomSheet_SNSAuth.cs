using Facebook.Unity;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class SocialLoginData
    {
        public string accessToken;
        public string userId;
        public string userEmail;
    }

    public class UIPopup_BottomSheet_SNSAuth : UIPopup
    {
        public Button naverBtn;
        public Button kakaoBtn;
        public Button facebookBtn;

        private string userEmail = null;

        private AndroidJavaObject kakaoAndroidObject;
        private AndroidJavaObject naverAndroidObject;

        public override void Awake()
        {
            base.Awake();

            naverBtn.onClick.AddListener(OnClickNaver);
            kakaoBtn.onClick.AddListener(OnClickKakao);
            facebookBtn.onClick.AddListener(OnClickFaceBook);
        }

        public override void UpdateData(params object[] args)
        {
            UIManager.s.Dim = true;

            /// type
            /// 0 : Naver
            /// 1 : Kakao
            /// 2 : Facebook

            int type = (int)args[0];
            naverBtn.SetActive(type == 0);
            kakaoBtn.SetActive(type == 1);
            facebookBtn.SetActive(type == 2);

            switch (type)
            {
                case 0:
                    {
                        naverAndroidObject = new AndroidJavaObject("com.marvrus.foodverse.UnityNaverLogin");
                        break;
                    }
                case 1:
                    {
                        kakaoAndroidObject = new AndroidJavaObject("com.marvrus.foodverse.UnityKakaoLogin");
                        break;
                    }
                case 2:
                    {
                        // FACEBOOK 초기화
                        if (!FB.IsInitialized)
                        {
                            // Initialize the Facebook SDK
                            FB.Init(FacebookInitCallback, OnHideUnity);
                        }
                        else
                        {
                            // Already initialized, signal an app activation App Event
                            FB.ActivateApp();
                        }
                        break;
                    }
            }
        }

        public override void Close()
        {
            base.Close();
            UIManager.s.Dim = false;

            naverBtn.enabled = true;
            kakaoBtn.enabled = true;
            facebookBtn.enabled = true;
        }

        #region naver login
        public void OnClickNaver()
        {
            naverBtn.enabled = false;

            Debug.Log("OnClick Naver Login");
            naverAndroidObject.Call("NaverLogin");
        }

        public void NaverLoginAuthCallback(string _loginData)
        {
            Debug.Log(_loginData);

            var data = JsonConvert.DeserializeObject<SocialLoginData>(_loginData);
            CheckSocialLogin("NAVER", data.accessToken, data.userId, data.userEmail);
        }

        // 네이버 로그인이 실패하는 경우, 재로그인 시도하도록 로그인 입력창 활성화
        public void NaverLoginFailedCallback(string _error)
        {
            Debug.Log(_error);
        }
        #endregion

        #region kakao login
        private void OnClickKakao()
        {
            kakaoBtn.enabled = false;

            Debug.Log("OnClick Kakao Login");
            kakaoAndroidObject.Call("kakaoLogin");
        }

        private void KakaoLoginAuthCallback(string _loginData)
        {
            Debug.Log(_loginData);

            var data = JsonConvert.DeserializeObject<SocialLoginData>(_loginData);
            CheckSocialLogin("KAKAO", data.accessToken, data.userId, data.userEmail);
        }

        private void KakaoLoginFailedCallback(string _error)
        {
            Debug.Log(_error);
        }
        #endregion

        #region facebook login
        private void FacebookInitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                Debug.Log("FB.IsInitialized Success");
                FB.ActivateApp();
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnClickFaceBook()
        {
            facebookBtn.enabled = false;

            Debug.Log("OnClick Facebook Login");
            var perms = new List<string>() { "public_profile", "email" };
            FB.LogInWithReadPermissions(perms, FacebookLoginAuthCallback);
        }

        private void FacebookLoginAuthCallback(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                Debug.Log("AccessToken :: " + aToken.TokenString);
                Debug.Log("UserID :: " + aToken.UserId);

                foreach (string perm in aToken.Permissions)
                {
                    Debug.Log(perm);
                }

                FB.API("/me?fields=email", HttpMethod.GET, GetFacebookInfo, new Dictionary<string, string>() { });

                CheckSocialLogin("FACEBOOK", aToken.TokenString, aToken.UserId);
            }
            else
            {
                Debug.Log("User cancelled login");
                Close();
            }
        }

        private void GetFacebookInfo(IResult result)
        {
            if (result.Error == null)
            {
                if(result.RawResult.Contains("email"))
                {
					if (!string.IsNullOrEmpty(result.ResultDictionary["email"].ToString()))
						userEmail = result.ResultDictionary["email"].ToString();
				}
                
            }
            else
            {
                Debug.LogError($"GetFacebookInfo Error : {result.Error}");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
        #endregion

        private void CheckSocialLogin(string _provider, string _fbToken, string _fbUserID, string _email = null)
        {
            if (string.IsNullOrEmpty(_email)) _email = userEmail;

            Protocol.Request.SocialLogin socialLoginData = new Protocol.Request.SocialLogin(
                    _provider, _fbToken, _fbUserID, PPM.ClientID
                );

            NetworkManager.s.SocialLogin(socialLoginData, (result) =>
            {
                UIManager.s.Dim = false;

                PPM.AutoLogin = true;
                PPM.ID = result.data.id;

                UIManager.s.OpenFeed();
            }, (error) =>
            {
                if (error.Contains("NOT_FOUND"))
                {
                    Close();

                    object[] data = new object[] {
                        UIPopup_AcceptTerms.SIGNUP_TYPE.SNS,
                        socialLoginData,
                        _email
                    };
                    UIManager.s.OpenPopupWithData<UIPopup_AcceptTerms>(true, data);
                }
            });
        }
    }
}