using Microsoft.AspNetCore.Components;
using Store.Web.Models;
using Store.Web.Services;
using static Store.Web.Models.Noticiation;

namespace Store.Web.Components
{
    public partial class NotificationBase : ComponentBase, IDisposable
    {
        [Inject]
        public NotificationService NotificationService { get; set; } = default!;

        protected string Message { get; set; }

        protected bool IsVisible { get; set; }

        protected string BackgroundCssClass { get; set; }

        protected string IconCssClass { get; set; }

        protected override void OnInitialized()
        {
            NotificationService.OnShow += ShowNotification;
            NotificationService.OnHide += HideNotification;
        }

        private void ShowNotification(Noticiation noticiation)
        {
            BuildNoticationSettings(noticiation);
            IsVisible = true;
            StateHasChanged();
        }

        private void HideNotification()
        {
            IsVisible = false;
            StateHasChanged();
        }

        private void BuildNoticationSettings(Noticiation noticiation)
        {
            switch (noticiation.Level)
            {
                case NoticiationLevel.Success:
                    BackgroundCssClass = "info";
                    IconCssClass = "info";
                    break;
                case NoticiationLevel.Danger:
                    BackgroundCssClass = "success";
                    IconCssClass = "check";
                    break;
                case NoticiationLevel.Warning:
                    BackgroundCssClass = "warning";
                    IconCssClass = "exclamation";
                    break;
                case NoticiationLevel.Info:
                    BackgroundCssClass = "danger";
                    IconCssClass = "times";
                    break;
            }

            Message = noticiation.Name;
        }

        public void Dispose()
        {
            NotificationService.OnShow -= ShowNotification;
        }
    }
}
