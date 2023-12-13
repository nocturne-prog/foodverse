using UnityEngine.UI;
using REPORT_TYPE = Protocol.Request.Report.REPORT_CATEGORY;
using CATEGORY_TYPE = Marvrus.UI.UIPopup_BottomSheet_More.TYPE;

namespace Marvrus.UI
{
    public class UIPopup_BottomSheet_Report : UIPopup
    {
        public Toggle spamToggle;
        public Toggle harmfulToggle;
        public Button cancelBtn;
        public Button reportBtn;

        private REPORT_TYPE reportType = REPORT_TYPE.SPAM;
        private CATEGORY_TYPE categoryType = CATEGORY_TYPE.Feed;
        private long feed_id;
        private long comment_id;

        public override void Awake()
        {
            base.Awake();

            cancelBtn.onClick.AddListener(OnClickCancel);
            reportBtn.onClick.AddListener(OnClickReport);
            spamToggle.onValueChanged.AddListener(OnClickSpam);
            harmfulToggle.onValueChanged.AddListener(OnClickHarmful);
        }

        public override void UpdateData(params object[] args)
        {
            categoryType = (CATEGORY_TYPE)args[0];
            
            feed_id = (long)args[1];
            comment_id = (long)args[2];

            spamToggle.SetIsOnWithoutNotify(true);
            harmfulToggle.SetIsOnWithoutNotify(false);
            reportType = REPORT_TYPE.SPAM;

            UIManager.s.Dim = true;
            base.UpdateData(args);
        }

        public void OnClickSpam(bool _isOn)
        {
            if (_isOn is true)
                reportType = REPORT_TYPE.SPAM;
        }

        public void OnClickHarmful(bool _isOn)
        {
            if (_isOn is true)
                reportType = REPORT_TYPE.SPAM;
        }

        public void OnClickCancel()
        {
            Close();
        }

        public override void Close()
        {
            UIManager.s.Dim = false;
            base.Close();
        }

        public void OnClickReport()
        {
            if (categoryType == CATEGORY_TYPE.Feed)
            {
                Protocol.Request.Report data = new(reportType, string.Empty);

                NetworkManager.s.ReportFeed(feed_id, data, (result) =>
                {
                    UIManager.s.OpenToast(Const.TOAST_REPORT_COMPLETE);
                    Close();
                }, (error) =>
                {
                    UIManager.s.OpenToast(error);
                    Close();
                });
            }
            else
            {
                Protocol.Request.Report data = new(reportType, string.Empty);

                NetworkManager.s.ReportComment(feed_id, comment_id, data, (result) =>
                {
                    UIManager.s.OpenToast(Const.TOAST_REPORT_COMPLETE);
                    Close();
                }, (error) =>
                {
                    UIManager.s.OpenToast(error);
                    Close();
                });
            }
        }
    }
}