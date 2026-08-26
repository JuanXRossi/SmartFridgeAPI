namespace SmartFridgeAPI.Service
{
    public static class EmailTemplates
    {
        private const string Wrapper = @"
            <div style=""font-family: Arial, sans-serif; max-width: 480px; margin: 0 auto; padding: 24px; color: #1f2937;"">
                <h2 style=""color:#0f766e;"">SmartFridge</h2>
                {0}
                <p style=""margin-top:32px; font-size:12px; color:#6b7280;"">
                    Si no realizaste esta acción, podés ignorar este correo.
                </p>
            </div>";

        public static string ConfirmationTemplate(string name, string link) => string.Format(Wrapper, $@"
            <p>Hola {name},</p>
            <p>Gracias por registrarte en SmartFridge. Confirmá tu dirección de correo para activar tu cuenta:</p>
            <p style=""text-align:center; margin:32px 0;"">
                <a href=""{link}"" style=""background:#0f766e; color:#fff; padding:12px 24px; border-radius:8px; text-decoration:none;"">
                    Confirmar mi cuenta
                </a>
            </p>
            <p>O copiá y pegá este enlace en tu navegador:</p>
            <p style=""word-break:break-all; font-size:13px;"">{link}</p>
            <p>Este enlace expira en 24 horas.</p>");

        public static string PasswordResetTemplate(string name, string link) => string.Format(Wrapper, $@"
            <p>Hola {name},</p>
            <p>Recibimos una solicitud para restablecer tu contraseña.</p>
            <p style=""text-align:center; margin:32px 0;"">
                <a href=""{link}"" style=""background:#0f766e; color:#fff; padding:12px 24px; border-radius:8px; text-decoration:none;"">
                    Restablecer contraseña
                </a>
            </p>
            <p>O copiá y pegá este enlace en tu navegador:</p>
            <p style=""word-break:break-all; font-size:13px;"">{link}</p>
            <p>Este enlace expira en 1 hora.</p>");
    }
}