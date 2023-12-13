using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UIPopup_MyInfo_EmailModify : UIPopup
    {
        public TMP_InputField emailInputField;
        public Button saveBtn;
        public GameObject block;

        public void OnClickSave()
        {
        }

        public void OnEndEditEmail()
        {
        }
    }
}