using CommunityToolkit.Maui.Alerts;

namespace Meal_Card.Controls
{
    public static class DataControllerUser
    {
        public static string? hora;

        public static string DataAppUser()
        {

            DateTime agora = DateTime.UtcNow;
            var hora_Atual = agora.Hour;
            try
            {
                if (hora_Atual >= 1 || hora_Atual == 12)
                {
                    return "Ola, Bom dia ☀️";
                }
                else if (hora_Atual > 12 || hora_Atual <= 18)
                {
                    return "Ola, Boa tarde ☀️";
                }
                else
                {
                    return "Ola, Boa noite 🌙";
                }
            }
            catch (System.Exception ex)
            {
                var errorMessage = $"Erro inesperado: {ex.Message}";
                var notification = Toast.Make("Aconteceu em erro, tente novamente mais tarde...", CommunityToolkit.Maui.Core.ToastDuration.Short);
                notification.Show();

                return " Ola 😎";
            }

        }
    }
}
