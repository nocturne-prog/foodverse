using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_Modal_Default : UIPopup
    {
        public enum BUTTON_TYPE
        {
            ONE = 0,
            TWO
        }

        public Button leftBtn;
        public Button rightBtn;
        public TextMeshProUGUI leftText;
        public TextMeshProUGUI rightText;
        public TextMeshProUGUI title;
        public TextMeshProUGUI desc;

        private BUTTON_TYPE type = BUTTON_TYPE.ONE;
        private Action ok;
        private Action cancel;

        public override void UpdateData(params object[] args)
        {
            UIManager.s.Dim = true;

            type = (BUTTON_TYPE)args[0];
            title.text = (string)args[1];
            desc.text = (string)args[2];

            if(type == BUTTON_TYPE.ONE)
            {
                leftBtn.SetActive(false);

                ok = (Action)args[3];
                if (!string.IsNullOrEmpty(args[4].ToString())) rightText.text = args[4].ToString();
            }
            else
            {
                leftBtn.SetActive(true);

                cancel = (Action)args[3];
                ok = (Action)args[4];
                if (!string.IsNullOrEmpty(args[5].ToString())) leftText.text = args[5].ToString();
                if (!string.IsNullOrEmpty(args[6].ToString())) rightText.text = args[6].ToString();
            }

            leftBtn.onClick.AddListener(() =>
            {
                Close();
                cancel?.Invoke();
            });

            rightBtn.onClick.AddListener(() =>
            {
                Close();
                ok?.Invoke();
            });
        }

        public override void Close()
        {
            base.Close();

            UIManager.s.Dim = false;

            leftBtn.onClick.RemoveAllListeners();
            rightBtn.onClick.RemoveAllListeners();
        }
    }
}