using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.Util
{
    public class SafeArea : MonoBehaviour
    {
        public CanvasScaler scaler;
        public float ratio
        {
            get;
            private set;
        }

        private void Start()
        {
            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            if (scaler is null)
            {
                scaler = Extension.FindParent<CanvasScaler>(this.transform);

                if (scaler is null)
                {
                    Debug.LogError("SafeArea :: can't find CanvasScaler..");
                    return;
                }
            }

            Vector2 reference = scaler.referenceResolution;
            ratio = reference.x / Screen.width;

            RectTransform trf = GetComponent<RectTransform>();

            Vector2 safeAreaToCanvas = new Vector2(Screen.safeArea.width * ratio, Screen.safeArea.height * ratio);
            trf.sizeDelta = safeAreaToCanvas;

            float posX = (trf.sizeDelta.x / 2) - (safeAreaToCanvas.x / 2);
            float posY = (trf.sizeDelta.y / 2) - (safeAreaToCanvas.y / 2);
            float posYGap = ((Screen.height - Screen.safeArea.height - Screen.safeArea.y) - Screen.safeArea.y) / 2 * ratio;
            trf.localPosition = new Vector2(posX, posY - posYGap);
        }

#if UNITY_EDITOR
        private void FixedUpdate()
        {
            ApplySafeArea();
        }
#endif
    }
}