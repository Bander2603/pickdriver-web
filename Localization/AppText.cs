namespace PickDriverWeb.Localization;

public sealed class AppText
{
    public string this[string text] => AppStrings.Translate(text);

    public string Translate(string text) => AppStrings.Translate(text);

    public string Format(string template, params object[] args) => AppStrings.Format(template, args);

    public string TranslateApiMessage(string? message, string fallback) => AppStrings.TranslateApiMessage(message, fallback);

    public string CurrentLanguageCode => AppStrings.CurrentLanguageCode;

    public IReadOnlyList<LanguageOption> SupportedLanguages => AppStrings.SupportedLanguages;
}

