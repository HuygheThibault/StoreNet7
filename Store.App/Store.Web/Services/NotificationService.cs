using Store.Web.Models;
using System.Timers;

namespace Store.Web.Services
{
    public class NotificationService : IDisposable
    {
        public event Action<Noticiation> OnShow;

        public event Action OnHide;

        private System.Timers.Timer? Countdown { get; set; }

        public void ShowNotification(Noticiation notification)
        {
            OnShow?.Invoke(notification);
            startCountdown();
        }

        private void startCountdown()
        {
            SetCountdown();
            if (Countdown.Enabled)
            {
                Countdown.Stop();
                Countdown.Start();
            }
            else
            {
                Countdown.Start();
            }
        }

        private void SetCountdown()
        {
            if (Countdown == null)
            {
                Countdown = new System.Timers.Timer(5000);
                Countdown.Elapsed += HideNoticiation;
                Countdown.AutoReset = false;
            }
        }

        private void HideNoticiation(object source, ElapsedEventArgs args)
        {
            OnHide?.Invoke();
        }

        public void Dispose()
        {
            Countdown?.Dispose();
        }
    }
}
