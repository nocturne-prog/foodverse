using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class SnackBar : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public Button btn;

        private void Start()
        {
            btn.onClick.AddListener(() =>
            {
                Hide();
            });
        }

        public void Open(string _text, float _duration = 2f)
        {
            CancelInvoke(nameof(Hide));

            text.text = _text;
            SetActive(true);
            Invoke(nameof(Hide), _duration);
        }

        public void SetActive(bool _active)
        {
            gameObject.SetActive(_active);
        }

        public void Hide()
        {
            SetActive(false);
        }
    }
}