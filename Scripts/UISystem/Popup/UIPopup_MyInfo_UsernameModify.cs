using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UIPopup_MyInfo_UsernameModify : UIPopup
    {
        public TMP_InputField usernameInputField;
        public Button saveBtn;
        public GameObject block;
        public GameObject usernameDuplicate;

        private string changedUsername = string.Empty;

        public override void Awake()
        {
            base.Awake();
            saveBtn.onClick.AddListener(OnClickSave);
            usernameInputField.onEndEdit.AddListener(OnEndEditUsername);
        }

        public override void UpdateData(params object[] args)
        {
            usernameInputField.text = DC.data_myProfile.userName;
            block.SetActive(true);

            changedUsername = string.Empty;
    }

        public void OnClickSave()
        {
            if (string.IsNullOrEmpty(changedUsername) is true)
                return;

            string picturePath = string.Empty;

            if (string.IsNullOrEmpty(DC.data_myProfile.picture_url) is false)
            {
                picturePath = Util.FileCache.s.GetImagePath(DC.data_myProfile.picture_url);
            }

            NetworkManager.s.EditProfile(changedUsername, picturePath, (profile) =>
            {
                UIManager.s.FindPopup<UIPopup_My>().Refresh(profile);
                var myInfoProfileModify = UIManager.s.FindPopup<UIPopup_MyInfo_ProfileModify>();
                myInfoProfileModify.Refresh();
                myInfoProfileModify.ChangeProfileAction();
                UIManager.s.RefreshFeed();
                Close();
                UIManager.s.OpenToast(Const.TOAST_EDIT_NICKNAME_COMPLETE);
            },(error) =>
            {
                Debug.LogError(error);
            });
        }

        public void OnEndEditUsername(string value)
        {
            if (DC.data_myProfile.userName.Equals(value) is true)
                return;

            usernameDuplicate.SetActive(false);

            NetworkManager.s.EditProfileCheck(value, (result) =>
            {
                bool available = result.available;

                if (available is true)
                {
                    changedUsername = value;
                    block.SetActive(false);
                }

                usernameDuplicate.SetActive(!available);
            });
            
        }
    }
}