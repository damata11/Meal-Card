using CommunityToolkit.Maui.Alerts;

namespace Meal_Card.Controls
{
    public static class NotificationToast
    {
        public async static Task MostarToastL(this string message)
        {
            var notification = Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Long);
            await notification.Show();
        }

        public async static Task MostarToast(this string message)
        {
            var notification = Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Short);
            await notification.Show();
        }
    }
}
