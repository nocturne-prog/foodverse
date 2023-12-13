using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PC = Marvrus.Util.PermissionCheck;
using PPM = Marvrus.Util.PlayerPrefsManager;
using DF = Marvrus.Util.Define;

namespace Marvrus.UI
{
    public class UIPopup_Permission : UIPopup
    {
        public GameObject cameraTrf;
        public GameObject galleryTrf;
        public GameObject notifiTrf;

        public Button okBtn;

        public override void Awake()
        {
            base.Awake();

            okBtn.onClick.AddListener(OnClickOk);
        }

        public override void UpdateData(params object[] args)
        {
            base.UpdateData(args);
#if UNITY_ANDROID
            notifiTrf.SetActive(false);
#else
            notifiTrf.SetActive(true);
#endif
        }

        private void OnClickOk()
        {
            PPM.FirstCheckPermission = true;

            if (DF.platform == DF.Platform.UnityEditor)
            {
                UIManager.s.OpenPopupWithData<UIPopup_Entry>();
            }
            else
            {
                Util.PermissionType[] type = new Util.PermissionType[]
                {
                    Util.PermissionType.Camera,
                    Util.PermissionType.Read
                };

                int callbackCount = 0;

                PC.RequestPermissions(type, (result) =>
                {
                    callbackCount++;

                    if (callbackCount == 1)
                    {
                        // NativeCameraPermission
                        PPM.NativeCamera = PC.ParseNativePluginPermissionValue(result);
                        Debug.Log($"Camera :: {PPM.NativeCamera}");
                    }
                    else
                    {
                        // NativeGalleryPermission
                        PPM.NativeGallery = PC.ParseNativePluginPermissionValue(result);
                        Debug.Log($"Gallery :: {PPM.NativeGallery}");
                        UIManager.s.OpenPopupWithData<UIPopup_Entry>();
                    }
                });
            }
        }
    }
}