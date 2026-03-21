using System.Globalization;
using System.Text;

namespace PickDriverWeb.Localization;

public static class AppStrings
{
    private static readonly Dictionary<string, string> SpanishToEnglish = new(StringComparer.Ordinal)
    {
        ["Mi perfil"] = "My profile",
        ["Gestiona usuario, seguridad y ajustes de cuenta."] = "Manage your user, security, and account settings.",
        ["Redirigiendo al login..."] = "Redirecting to login...",
        ["Cargando perfil..."] = "Loading profile...",
        ["Cuenta"] = "Account",
        ["Activa"] = "Active",
        ["Usuario"] = "Username",
        ["Cambiar nombre de usuario"] = "Change username",
        ["Guardando..."] = "Saving...",
        ["Guardar"] = "Save",
        ["Seguridad"] = "Security",
        ["Cambiar password"] = "Change password",
        ["Para cuentas sociales (Google/Apple) usa la opcion de email de restablecimiento."] = "For social accounts (Google/Apple), use the password reset email option.",
        ["Password actual"] = "Current password",
        ["Nueva password"] = "New password",
        ["Confirmar password"] = "Confirm password",
        ["Actualizando..."] = "Updating...",
        ["Actualizar password"] = "Update password",
        ["Restablecer por email"] = "Reset by email",
        ["Recibiras un enlace para definir una nueva password."] = "You'll receive a link to set a new password.",
        ["Info legal"] = "Legal info",
        ["Informacion y condiciones del contenido de PickDriver."] = "Information and terms for PickDriver content.",
        ["Licencias"] = "Licenses",
        ["Sesion y cuenta"] = "Session and account",
        ["Cerrar sesion"] = "Log out",
        ["Eliminando..."] = "Deleting...",
        ["Eliminar cuenta"] = "Delete account",
        ["Esta accion es permanente y no se puede deshacer."] = "This action is permanent and cannot be undone.",
        ["Perderas tu perfil y todos los datos asociados a tu cuenta en PickDriver."] = "You will lose your profile and all data associated with your PickDriver account.",
        ["Cancelar"] = "Cancel",
        ["Si, eliminar"] = "Yes, delete",
        ["No se pudo cargar el perfil."] = "Couldn't load the profile.",
        ["No se pudo actualizar el usuario."] = "Couldn't update the username.",
        ["Nombre de usuario actualizado."] = "Username updated.",
        ["No se pudo actualizar la password."] = "Couldn't update the password.",
        ["Password actualizada correctamente."] = "Password updated successfully.",
        ["Espera {0} antes de reenviar el email."] = "Wait {0} before resending the email.",
        ["No se encontro un email para esta cuenta."] = "No email was found for this account.",
        ["No se pudo enviar el email de restablecimiento."] = "Couldn't send the password reset email.",
        ["Se envio el email de restablecimiento si la cuenta existe."] = "If the account exists, the password reset email was sent.",
        ["Enviando..."] = "Sending...",
        ["Reenviar en {0}"] = "Resend in {0}",
        ["Enviar email de restablecimiento"] = "Send password reset email",
        ["No se pudo eliminar la cuenta."] = "Couldn't delete the account.",
        ["El usuario es obligatorio."] = "Username is required.",
        ["El usuario debe tener entre {0} y {1} caracteres."] = "Username must be between {0} and {1} characters.",
        ["El usuario solo puede tener letras, numeros, puntos, guiones y guiones bajos."] = "Username can only contain letters, numbers, dots, hyphens, and underscores.",
        ["La nueva password es obligatoria."] = "New password is required.",
        ["La password debe tener al menos {0} caracteres."] = "Password must be at least {0} characters long.",
        ["Las passwords no coinciden."] = "Passwords do not match.",
        ["Iniciar sesion"] = "Log in",
        ["Ingresando..."] = "Logging in...",
        ["Ingresar"] = "Log in",
        ["Olvide mi password"] = "I forgot my password",
        ["Tu cuenta aun no esta verificada."] = "Your account is not verified yet.",
        ["Iniciar sesion con Google"] = "Log in with Google",
        ["Login con Google no esta configurado."] = "Google login is not configured.",
        ["No tienes cuenta?"] = "Don't have an account?",
        ["Crear cuenta"] = "Create account",
        ["No se pudo iniciar sesion."] = "Couldn't log in.",
        ["Espera {0} para volver a reenviar la verificacion."] = "Wait {0} before resending verification again.",
        ["Indica tu email para reenviar la verificacion."] = "Enter your email to resend verification.",
        ["Si la cuenta existe y esta pendiente, se envio un email de verificacion."] = "If the account exists and is pending, a verification email was sent.",
        ["No se pudo reenviar el email de verificacion."] = "Couldn't resend the verification email.",
        ["No se pudo iniciar sesion con Google."] = "Couldn't log in with Google.",
        ["Reenviar email de verificacion"] = "Resend verification email",
        ["Registro completado. Revisa tu email para verificar la cuenta antes de iniciar sesion."] = "Registration completed. Check your email to verify the account before logging in.",
        ["No se pudo confirmar el envio del email de verificacion. Puedes solicitarlo de nuevo."] = "Couldn't confirm the verification email was sent. You can request it again.",
        ["Creando..."] = "Creating...",
        ["Registrarme con Google"] = "Sign up with Google",
        ["Ya tienes cuenta?"] = "Already have an account?",
        ["No se pudo completar el registro."] = "Couldn't complete registration.",
        ["Si la cuenta existe y esta pendiente, se envio otro email de verificacion."] = "If the account exists and is pending, another verification email was sent.",
        ["El email es obligatorio."] = "Email is required.",
        ["Email invalido."] = "Invalid email.",
        ["La password es obligatoria."] = "Password is required.",
        ["El email ha sido verificado correctamente. Ya puedes iniciar sesion."] = "Your email has been verified successfully. You can now log in.",
        ["Ir al login"] = "Go to login",
        ["Recuperar password"] = "Recover password",
        ["Introduce tu email y te enviaremos instrucciones para restablecer tu password."] = "Enter your email and we'll send you instructions to reset your password.",
        ["Volver al login"] = "Back to login",
        ["Si la cuenta existe, te hemos enviado un email con instrucciones."] = "If the account exists, we sent you an email with instructions.",
        ["No se pudo iniciar la recuperacion de password."] = "Couldn't start password recovery.",
        ["Enviar instrucciones"] = "Send instructions",
        ["Restablecer password"] = "Reset password",
        ["El enlace de recuperacion no es valido o no contiene token."] = "The recovery link is invalid or doesn't contain a token.",
        ["Introduce tu nueva password para completar el cambio."] = "Enter your new password to complete the update.",
        ["No se pudo restablecer la password."] = "Couldn't reset the password.",
        ["Dashboard"] = "Dashboard",
        ["Resumen general de tu temporada."] = "General summary of your season.",
        ["Cargando datos..."] = "Loading data...",
        ["Ir al calendario"] = "Go to calendar",
        ["Proxima carrera"] = "Next race",
        ["No hay carrera cargada."] = "No race loaded.",
        ["Circuito {0}"] = "{0} circuit",
        ["Ir a ligas"] = "Go to leagues",
        ["Tus ligas"] = "Your leagues",
        ["Gestiona tus ligas y su estado actual."] = "Manage your leagues and their current state.",
        ["Unirse a liga"] = "Join league",
        ["Crear liga"] = "Create league",
        ["Cargando ligas..."] = "Loading leagues...",
        ["Todavia no estas en ninguna liga."] = "You're not in any league yet.",
        ["No se pudo calcular la posicion en todas las ligas."] = "Couldn't calculate the position in all leagues.",
        ["Liga"] = "League",
        ["Status"] = "Status",
        ["Pos."] = "Pos.",
        ["Ir a resultados"] = "Go to results",
        ["Resultados F1"] = "F1 standings",
        ["Top 5 pilotos"] = "Top 5 drivers",
        ["Sin datos."] = "No data.",
        ["Piloto"] = "Driver",
        ["Equipo"] = "Team",
        ["Pts."] = "Pts.",
        ["Top 3 equipos"] = "Top 3 teams",
        ["No se pudieron cargar las carreras."] = "Couldn't load the races.",
        ["Calendario"] = "Calendar",
        ["Carreras de la temporada."] = "Season races.",
        ["Cargando carreras..."] = "Loading races...",
        ["Sin carreras disponibles."] = "No races available.",
        ["Longitud del circuito"] = "Circuit length",
        ["Distancia de carrera"] = "Race distance",
        ["Vueltas"] = "Laps",
        ["Primer Gran Premio"] = "First Grand Prix",
        ["Record de vuelta"] = "Lap record",
        ["Horarios"] = "Schedule",
        ["Tipo de horario"] = "Schedule type",
        ["Mi hora"] = "My time",
        ["Hora circuito"] = "Track time",
        ["Sin horarios confirmados."] = "No confirmed times.",
        ["Por confirmar"] = "To be confirmed",
        ["Resultados actuales de la temporada."] = "Current season standings.",
        ["Tipo de resultados"] = "Standings type",
        ["Pilotos"] = "Drivers",
        ["Equipos"] = "Teams",
        ["Cargando resultados..."] = "Loading standings...",
        ["Pais"] = "Country",
        ["No se pudieron cargar los resultados."] = "Couldn't load the standings.",
        ["Resultados de equipos"] = "Team standings",
        ["Resultados de pilotos"] = "Driver standings",
        ["Reglas del juego"] = "Game rules",
        ["Resumen rapido de como jugar en PickDriver."] = "Quick summary of how to play PickDriver.",
        ["Objetivo"] = "Objective",
        ["Elige un piloto de Formula 1 por carrera, suma puntos segun sus resultados reales y gana la liga con el mayor puntaje acumulado en la temporada."] = "Pick one Formula 1 driver per race, score points based on real results, and win the league with the highest season total.",
        ["Ligas"] = "Leagues",
        ["Necesitas una temporada activa para crear o unirte a una liga."] = "You need an active season to create or join a league.",
        ["El creador es el owner y define el maximo de jugadores."] = "The creator is the owner and sets the maximum number of players.",
        ["La liga debe estar completa para iniciar el draft."] = "The league must be full before the draft can start.",
        ["Solo el owner puede iniciar el draft o eliminar la liga antes de empezar."] = "Only the owner can start the draft or delete the league before it begins.",
        ["Equipos (opcional)"] = "Teams (optional)",
        ["Si la liga habilita equipos, todos deben estar asignados antes del draft."] = "If the league enables teams, everyone must be assigned before the draft.",
        ["Un jugador solo puede estar en un equipo y no se permiten equipos de un solo jugador."] = "A player can only belong to one team and single-player teams are not allowed.",
        ["El sistema busca equipos balanceados segun la cantidad total de jugadores."] = "The system tries to balance teams based on the total number of players.",
        ["Draft y orden de picks"] = "Draft and pick order",
        ["El owner puede fijar el orden de picks; si no, se sortea."] = "The owner can set the pick order; otherwise it's randomized.",
        ["El orden rota en cada carrera para repartir el beneficio del turno."] = "The order rotates each race to spread the turn advantage.",
        ["Si hay picks espejo, la segunda mitad del orden se invierte."] = "If mirror picks are enabled, the second half of the order is reversed.",
        ["Fechas limite"] = "Deadlines",
        ["Hay dos cortes por carrera: 36 horas antes de la primera practica (FP1) y al inicio de la FP1."] = "There are two race cutoffs: 36 hours before the first practice (FP1) and at the start of FP1.",
        ["Despues de esos cortes, no se pueden hacer picks para esa carrera."] = "After those cutoffs, picks can't be made for that race.",
        ["Picks y autopick"] = "Picks and autopick",
        ["Solo el jugador de turno puede pickear; en ligas con equipos, un companero puede ayudar en la ultima hora."] = "Only the current player can pick; in team leagues, a teammate can help during the last hour.",
        ["No puedes elegir un piloto ya tomado ni uno que hayas baneado."] = "You can't choose a driver who is already taken or one you banned.",
        ["Puedes cargar una lista de autopick: si se vence tu turno, se elige el primer piloto disponible."] = "You can set an autopick list: if your turn expires, the first available driver is chosen.",
        ["Bans (opcional)"] = "Bans (optional)",
        ["Solo puedes banear el pick inmediatamente anterior."] = "You can only ban the immediately previous pick.",
        ["No se puede banear al ultimo jugador del orden (salvo que tambien sea el primero)."] = "You can't ban the last player in the order unless they're also the first.",
        ["Sin equipos: 2 bans por usuario. Con equipos: 3 bans por equipo."] = "Without teams: 2 bans per user. With teams: 3 bans per team.",
        ["Por carrera solo se usa 1 ban por usuario o equipo; un piloto no puede ser baneado dos veces en la misma carrera."] = "Only 1 ban per user or team can be used per race; a driver can't be banned twice in the same race.",
        ["Cuando un pick es baneado, el jugador debe elegir otro piloto."] = "When a pick is banned, the player must choose another driver.",
        ["Puntuacion y standings"] = "Scoring and standings",
        ["Solo cuentan los picks no baneados."] = "Only non-banned picks count.",
        ["Los autopicks valen el 50% de los puntos del piloto."] = "Autopicks are worth 50% of the driver's points.",
        ["Los standings se calculan solo con carreras completadas."] = "Standings are calculated using only completed races.",
        ["Notificaciones"] = "Notifications",
        ["Recibes avisos cuando es tu turno de pickear, cuando se publica el resultado de una carrera y cuando hay cambios relevantes en el draft."] = "You receive notifications when it's your turn to pick, when race results are published, and when there are relevant draft changes.",
        ["Disclaimer"] = "Disclaimer",
        ["Informacion importante sobre uso de la app y exactitud de datos."] = "Important information about app usage and data accuracy.",
        ["Uso informativo y recreativo"] = "Informational and recreational use",
        ["PickDriver es una aplicacion de entretenimiento. El contenido, predicciones, estadisticas y clasificaciones se ofrece con fines recreativos y puede contener errores, retrasos o cambios."] = "PickDriver is an entertainment app. Content, predictions, statistics, and standings are provided for recreational purposes and may contain errors, delays, or changes.",
        ["Marcas y contenido de terceros"] = "Third-party trademarks and content",
        ["Formula 1, nombres de equipos, circuitos y marcas relacionadas pertenecen a sus respectivos propietarios. PickDriver no esta afiliada oficialmente con Formula 1 ni con los equipos participantes."] = "Formula 1, team names, circuits, and related trademarks belong to their respective owners. PickDriver is not officially affiliated with Formula 1 or the participating teams.",
        ["Cuenta y datos"] = "Account and data",
        ["Si eliminas tu cuenta, el proceso es irreversible y podras perder acceso a datos de perfil, ligas y progreso asociado."] = "If you delete your account, the process is irreversible and you may lose access to profile data, leagues, and related progress.",
        ["PickDriver"] = "PickDriver",
        ["Primary"] = "Primary",
        ["Resultados"] = "Results",
        ["Password"] = "Password",
        ["Idioma"] = "Language",
        ["El idioma por defecto sigue el del sistema y puedes cambiarlo aqui."] = "The default language follows the system language and you can change it here.",
        ["Round {0}"] = "Round {0}",
        ["Reenviando..."] = "Resending...",
        ["Reenviar verificacion"] = "Resend verification",
        ["Redirigiendo al dashboard..."] = "Redirecting to dashboard...",
        ["Inicia sesion o crea una cuenta para empezar a jugar."] = "Log in or create an account to start playing.",
        ["Apps moviles"] = "Mobile apps",
        ["Seleccionar plataforma movil"] = "Select mobile platform",
        ["App iOS nativa en Beta abierta en TestFlight"] = "Native iOS app in open beta on TestFlight",
        ["Codigo QR de PickDriver iOS en TestFlight"] = "QR code for PickDriver iOS on TestFlight",
        ["Escanea este codigo para instalar la beta."] = "Scan this code to install the beta.",
        ["App Android nativa en beta cerrada"] = "Native Android app in closed beta",
        ["Unete al"] = "Join the",
        ["grupo de testers"] = "testers group",
        ["y descarga la app escaneando este codigo QR."] = "and download the app by scanning this QR code.",
        ["Codigo QR de PickDriver Android en Google Play"] = "QR code for PickDriver Android on Google Play",
        ["Escanea este codigo para abrir la app en Google Play."] = "Scan this code to open the app on Google Play.",
        ["liga"] = "league",
        ["Registro"] = "Sign up",
        ["Email verificado"] = "Email verified",
        ["seconds."] = "seconds.",
        ["Failed to rejoin."] = "Failed to rejoin.",
        ["Please retry or reload the page."] = "Please retry or reload the page.",
        ["Failed to resume the session."] = "Failed to resume the session.",
        ["Reload"] = "Reload",
        ["An unhandled error has occurred."] = "An unhandled error has occurred.",
        ["Rejoining the server..."] = "Rejoining the server...",
        ["Rejoin failed... trying again in <span id=\"components-seconds-to-next-attempt\"></span> seconds."] = "Rejoin failed... trying again in <span id=\"components-seconds-to-next-attempt\"></span> seconds.",
        ["Failed to rejoin.<br />Please retry or reload the page."] = "Failed to rejoin.<br />Please retry or reload the page.",
        ["Retry"] = "Retry",
        ["The session has been paused by the server."] = "The session has been paused by the server.",
        ["Failed to resume the session.<br />Please retry or reload the page."] = "Failed to resume the session.<br />Please retry or reload the page.",
        ["Resume"] = "Resume",
        ["Resumen de licencias y componentes usados por PickDriver Web."] = "Summary of licenses and components used by PickDriver Web.",
        ["Software principal"] = "Core software",
        ["xUnit y bUnit para tests"] = "xUnit and bUnit for tests",
        ["El codigo de esta aplicacion se distribuye bajo licencia MIT."] = "This app's code is distributed under the MIT license.",
        ["Archivos legales del proyecto"] = "Project legal files",
        ["Consulta los archivos LICENSE y NOTICE.md del repositorio para el detalle completo."] = "See the LICENSE and NOTICE.md files in the repository for full details.",
        ["Nota: algunos assets visuales de F1 tienen restricciones de redistribucion y se documentan en NOTICE.md."] = "Note: some F1 visual assets have redistribution restrictions and are documented in NOTICE.md.",
        ["Not Found"] = "Not Found",
        ["Sorry, the content you are looking for does not exist."] = "Sorry, the content you are looking for does not exist.",
        ["Request ID:"] = "Request ID:",
        ["Development Mode"] = "Development Mode",
        ["Swapping to Development environment will display more detailed information about the error that occurred."] = "Switching to the Development environment will display more detailed information about the error that occurred.",
        ["The Development environment shouldn't be enabled for deployed applications. It can result in displaying sensitive information from exceptions to end users. For local debugging, enable the Development environment by setting the ASPNETCORE_ENVIRONMENT environment variable to Development and restarting the app."] = "The Development environment shouldn't be enabled for deployed applications. It can result in displaying sensitive information from exceptions to end users. For local debugging, enable the Development environment by setting the ASPNETCORE_ENVIRONMENT environment variable to Development and restarting the app.",
        ["Error."] = "Error.",
        ["An error occurred while processing your request."] = "An error occurred while processing your request.",
        ["Hora local"] = "Local time",
        ["Hora circuito (UTC)"] = "Track time (UTC)",
        ["Hora circuito ({0})"] = "Track time ({0})",
        ["Practice 1"] = "Practice 1",
        ["Practice 2"] = "Practice 2",
        ["Practice 3"] = "Practice 3",
        ["Qualifying"] = "Qualifying",
        ["Sprint Qualifying"] = "Sprint Qualifying",
        ["Sprint"] = "Sprint",
        ["Race"] = "Race",
        ["Reuniendo servidor..."] = "Rejoining the server...",
        ["active"] = "active",
        ["pending"] = "pending",
        ["owner"] = "owner",
        ["member"] = "member",
        ["No se pudo conectar con la API."] = "Couldn't connect to the API.",
        ["La solicitud a la API expiro."] = "The API request timed out.",
        ["Respuesta inesperada del servidor."] = "Unexpected response from the server.",
        ["Credenciales invalidas"] = "Invalid credentials",
        ["Email no verificado"] = "Email not verified",
        ["Token expirado"] = "Expired token",
        ["If the account exists and is pending verification, a verification email has been sent."] = "If the account exists and is pending verification, a verification email has been sent.",
        ["If the account exists, password reset instructions have been sent."] = "If the account exists, password reset instructions have been sent.",
        ["Password updated successfully."] = "Password updated successfully.",
        ["Missing driverID"] = "Missing driver ID",
        ["Your turn is no longer active"] = "Your turn is no longer active",
        ["Driver no longer available"] = "Driver no longer available",
        ["Mock endpoint not implemented."] = "Mock endpoint not implemented."
    };

    private static readonly Dictionary<string, string> EnglishToSpanish = BuildReverseMap(SpanishToEnglish);

    public static IReadOnlyList<LanguageOption> SupportedLanguages { get; } =
    [
        new("en", "English", "English"),
        new("es", "Spanish", "Español")
    ];

    public static string CurrentLanguageCode => NormalizeCulture(CultureInfo.CurrentUICulture.Name);

    public static string Translate(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        return CurrentLanguageCode == "es"
            ? TranslateToSpanish(text)
            : TranslateToEnglish(text);
    }

    public static string Format(string template, params object[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, Translate(template), args);
    }

    public static string TranslateApiMessage(string? message, string fallback)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return Translate(fallback);
        }

        return Translate(message.Trim());
    }

    public static string NormalizeCulture(string? culture)
    {
        if (string.IsNullOrWhiteSpace(culture))
        {
            return "en";
        }

        var normalized = culture.Trim().ToLowerInvariant();
        return normalized.StartsWith("es", StringComparison.Ordinal) ? "es" : "en";
    }

    private static string TranslateToEnglish(string text)
    {
        return SpanishToEnglish.TryGetValue(text, out var translation)
            ? translation
            : text;
    }

    private static string TranslateToSpanish(string text)
    {
        return EnglishToSpanish.TryGetValue(text, out var translation)
            ? translation
            : text;
    }

    private static Dictionary<string, string> BuildReverseMap(Dictionary<string, string> source)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var pair in source)
        {
            result.TryAdd(pair.Value, pair.Key);
        }

        return result;
    }
}

public sealed record LanguageOption(string Code, string Name, string NativeName);
