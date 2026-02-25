using System.Xml.Linq;
using CurrencyApp1.Models;

namespace CurrencyApp1.Services;

public class CurrencyService
{
    private readonly HttpClient _http;

    // НБ РК XML через бесплатный CORS-прокси allorigins.win
    private const string NbrkUrl =
        "https://corsproxy.io/?url=https://nationalbank.kz/rss/rates_all.xml";

    public static readonly Dictionary<string, string> CurrencyFlags = new()
    {
        ["USD"] = "🇺🇸",
        ["EUR"] = "🇪🇺",
        ["RUB"] = "🇷🇺",
        ["GBP"] = "🇬🇧",
        ["JPY"] = "🇯🇵",
        ["CHF"] = "🇨🇭",
        ["CNY"] = "🇨🇳",
        ["CAD"] = "🇨🇦",
        ["AUD"] = "🇦🇺",
        ["SEK"] = "🇸🇪",
        ["NOK"] = "🇳🇴",
        ["DKK"] = "🇩🇰",
        ["KGS"] = "🇰🇬",
        ["UZS"] = "🇺🇿",
        ["TJS"] = "🇹🇯",
        ["AZN"] = "🇦🇿",
        ["GEL"] = "🇬🇪",
        ["BYN"] = "🇧🇾",
        ["UAH"] = "🇺🇦",
        ["TRY"] = "🇹🇷",
        ["SGD"] = "🇸🇬",
        ["HKD"] = "🇭🇰",
        ["KRW"] = "🇰🇷",
        ["PLN"] = "🇵🇱",
        ["CZK"] = "🇨🇿",
        ["HUF"] = "🇭🇺",
        ["INR"] = "🇮🇳",
        ["BRL"] = "🇧🇷",
        ["MXN"] = "🇲🇽",
        ["ZAR"] = "🇿🇦",
        ["AED"] = "🇦🇪",
        ["SAR"] = "🇸🇦",
        ["QAR"] = "🇶🇦",
        ["KWD"] = "🇰🇼",
        ["MNT"] = "🇲🇳",
    };

    public static readonly Dictionary<string, string> CurrencyNames = new()
    {
        ["USD"] = "Доллар США",
        ["EUR"] = "Евро",
        ["RUB"] = "Российский рубль",
        ["GBP"] = "Фунт стерлингов",
        ["JPY"] = "Японская йена",
        ["CHF"] = "Швейцарский франк",
        ["CNY"] = "Китайский юань",
        ["CAD"] = "Канадский доллар",
        ["AUD"] = "Австралийский доллар",
        ["SEK"] = "Шведская крона",
        ["NOK"] = "Норвежская крона",
        ["DKK"] = "Датская крона",
        ["KGS"] = "Киргизский сом",
        ["UZS"] = "Узбекский сум",
        ["TJS"] = "Таджикский сомони",
        ["AZN"] = "Азербайджанский манат",
        ["GEL"] = "Грузинский лари",
        ["BYN"] = "Белорусский рубль",
        ["UAH"] = "Украинская гривна",
        ["TRY"] = "Турецкая лира",
        ["SGD"] = "Сингапурский доллар",
        ["HKD"] = "Гонконгский доллар",
        ["KRW"] = "Южнокорейская вона",
        ["PLN"] = "Польский злотый",
        ["CZK"] = "Чешская крона",
        ["HUF"] = "Венгерский форинт",
        ["INR"] = "Индийская рупия",
        ["BRL"] = "Бразильский реал",
        ["MXN"] = "Мексиканское песо",
        ["ZAR"] = "Южноафриканский рэнд",
        ["AED"] = "Дирхам ОАЭ",
        ["SAR"] = "Саудовский риял",
        ["QAR"] = "Катарский риял",
        ["KWD"] = "Кувейтский динар",
        ["MNT"] = "Монгольский тугрик",
    };

    public CurrencyService(HttpClient http)
    {
        _http = http;
    }

    public async Task<(List<NbrkRate> Rates, string Date)> GetRatesAsync()
    {
        var xml = await _http.GetStringAsync(NbrkUrl);

        var doc = XDocument.Parse(xml);

        // Дата публикации из канала
        var pubDate = doc.Descendants("pubDate").FirstOrDefault()?.Value ?? "";

        var result = new List<NbrkRate>();

        foreach (var item in doc.Descendants("item"))
        {
            var title = item.Element("title")?.Value?.Trim().ToUpper() ?? "";
            var desc = item.Element("description")?.Value ?? "";
            var quantStr = item.Element("quant")?.Value ?? "1";
            var change = item.Element("change")?.Value ?? "0";

            if (string.IsNullOrEmpty(title)) continue;

            if (!decimal.TryParse(desc.Replace(",", "."),
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture,
                out var rate)) continue;

            int.TryParse(quantStr, out var quant);
            if (quant <= 0) quant = 1;

            decimal.TryParse(change.Replace(",", "."),
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture,
                out var changeVal);

            result.Add(new NbrkRate
            {
                Code = title,
                Rate = rate,
                Quantity = quant,
                Change = changeVal,
                Name = CurrencyNames.GetValueOrDefault(title, title),
                Flag = CurrencyFlags.GetValueOrDefault(title, "🌍"),
            });
        }

        return (result, pubDate);
    }
}
