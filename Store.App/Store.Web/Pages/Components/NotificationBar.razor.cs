using Microsoft.AspNetCore.Components;
using Store.Web.Models;

namespace Store.Web.Pages.Components
{
    public partial class NotificationBar
    {
        [Parameter]
        public Noticiation? noticiation { get; set; } = default!;

        public Queue<Noticiation>? Notifications { get; set; } = new Queue<Noticiation> ();

        protected override async Task OnParametersSetAsync()
        {
            if (noticiation != null)
            {
                Notifications.Enqueue(noticiation);
                await ClearNotification();
            }
        }

        private async Task ClearNotification()
        {
            if (Notifications.Count > 0)
            {
                await Task.Delay(2000);
                Notifications.Dequeue();
            }
        }
    }
}
