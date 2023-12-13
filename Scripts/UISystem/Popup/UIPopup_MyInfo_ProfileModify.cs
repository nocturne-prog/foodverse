using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UIPopup_MyInfo_ProfileModify : UIPopup
    {
        public Util.ProfileImageItem pictureImg;
        public Button editPictureBtn;
        public Button editNicknameBtn;
        public TextMeshProUGUI nicknameText;
        public Button onboardingCompleteBtn;

        private bool isOnboarding = false;
        private bool isChangeProfile = false;

        public override void Awake()
        {
            base.Awake();

            editPictureBtn.onClick.AddListener(OnClickEditPicture);
            editNicknameBtn.onClick.AddListener(OnClickEditNickname);
            onboardingCompleteBtn.onClick.AddListener(OnClickOnboardingComplete);
        }

        public override void UpdateData(params object[] args)
        {
            isChangeProfile = false;

            if (args != null && args.Length > 0)
            {
                isOnboarding = (bool)args[0];
            }
            else
            {
                isOnboarding = false;
            }

            onboardingCompleteBtn.SetActive(isOnboarding);
            Refresh();
        }

        public void OnClickEditPicture()
        {
            UIManager.s.OpenPopupWithData<UIPopup_MyInfo_ProfileModify_Photo>(true);
        }

        public void OnClickEditNickname()
        {
            UIManager.s.OpenPopupWithData<UIPopup_MyInfo_UsernameModify>(true);
        }

        public void Refresh()
        {
            UserProfile myProfile = DC.data_myProfile;

            if (string.IsNullOrEmpty(myProfile.picture_url) is false)
            {
                Util.FileCache.s.GetImage(myProfile.picture_url, Const.IMAGE_SIZE_MY, (texture) =>
                {
                    pictureImg.texture = texture;
                });
            }

            nicknameText.text = DC.data_myProfile.userName;
        }

        public void OnClickOnboardingComplete()
        {
            UIManager.s.OpenToast(Const.TOAST_ONBOARDING_PROPLE);
            Close();
        }

        public override void Close()
        {
            if (isOnboarding is true)
            {
                if (isChangeProfile is true)
                {
                    UIManager.s.OpenToast(Const.TOAST_ONBOARDING_PROPLE);
                }
            }

            base.Close();
        }

        public void ChangeProfileAction()
        {
            isChangeProfile = true;
        }
    }
}