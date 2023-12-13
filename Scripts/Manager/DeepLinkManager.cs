using System;
using UnityEngine;
using OB = Marvrus.Util.OpenBrowser;

namespace Marvrus
{
    public class DeepLinkManager : Singleton<DeepLinkManager>
    {
        private WebViewObject webviewobj = null;

        protected override void Awake()
        {
            base.Awake();

            Application.deepLinkActivated += onDeepLinkActivated;
            if (!String.IsNullOrEmpty(Application.absoluteURL))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                onDeepLinkActivated(Application.absoluteURL);
            }

            Debug.Log("DeepLinkManager Awake");
        }

        private void onDeepLinkActivated(string url)
        {
            Debug.Log($"DeepLink Manager :: {url}");

            string[] dataSplit = url.Split("?");

            string typeString = dataSplit[0];

            OB.HostType hostType = OB.GetHostType(typeString);

            if (hostType == OB.HostType.None)
            {
                Debug.LogError($"Unknown Host :: {url}");
                return;
            }

            if (dataSplit.Length > 1)
            {
                OB.callback(dataSplit[1]);
            }
            else
            {
                OB.callback(hostType.ToString());
            }
        }
    }
}