using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UIPopup_MyInfo : UIPopup
    {
        [Header("Default")]
        public TextMeshProUGUI nicknameText;
        public Button nicknameBtn;
        public TextMeshProUGUI idText;
        public GameObject snsTrf;
        public TextMeshProUGUI emailText;
        public Button emailBtn;
        public TextMeshProUGUI phoneNumberText;
        public Button phoneNumberBtn;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI birthDayText;
        public TextMeshProUGUI genderText;

        [Header("Protector Info")]
        public TextMeshProUGUI p_phoneNumberText;
        public TextMeshProUGUI p_nameText;
        public TextMeshProUGUI p_birthDayText;
        public TextMeshProUGUI p_genderText;

        public override void Awake()
        {
            base.Awake();

            nicknameBtn.onClick.AddListener(() =>
            {
                UIManager.s.OpenPopupWithData<UIPopup_MyInfo_NicknameModify>(true);
            });

            emailBtn.onClick.AddListener(() =>
            {
            });

            phoneNumberBtn.onClick.AddListener(() =>
            {
            });
        }

        public override void UpdateData(params object[] args)
        {
            var data = DC.data_login.data.MarvrusUser;

            nicknameText.text = data.nickname;
            idText.text = data.id;
            snsTrf.SetActive(false);
            emailText.text = data.emailAddress;
            phoneNumberText.text = data.phoneNo;
            //nameText.text = data
            birthDayText.text = data.birthDay;
            genderText.text = data.gender;

            if (data.parentAgreement.Equals("N"))
                return;
        }
    }
}