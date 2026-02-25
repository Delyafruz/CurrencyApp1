using System.Net.Http.Json;
using CurrencyApp1.Models;

namespace CurrencyApp1.Services;

public class CryptoService
{
    private readonly HttpClient _http;

    // CoinGecko — бесплатный API, без ключа
    // Возвращает цены сразу в USD и KZT
    private const string CoinsUrl =
        "https://api.coingecko.com/api/v3/coins/markets" +
        "?vs_currency=usd" +
        "&ids=bitcoin,ethereum,litecoin,binancecoin" +
        "&order=market_cap_desc" +
        "&price_change_percentage=24h" +
        "&sparkline=false";

    // Для конвертации в тенге берём курс USD/KZT с НБ РК через тот же прокси
    private const string UsdKztUrl =
        "https://corsproxy.io/?url=https://nationalbank.kz/rss/rates_all.xml";

    public static readonly Dictionary<string, string> CryptoIcons = new()
    {
        ["bitcoin"] = "₿",
        ["ethereum"] = "Ξ",
        ["litecoin"] = "Ł",
        ["binancecoin"] = "BNB",
    };

    public static readonly Dictionary<string, string> CryptoColors = new()
    {
        ["bitcoin"] = "#f7931a",
        ["ethereum"] = "#627eea",
        ["litecoin"] = "#bfbbbb",
        ["binancecoin"] = "#f3ba2f",
    };

    public CryptoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CryptoRate>> GetCryptoRatesAsync()
    {
        // 1. Получаем курс USD → KZT с НБ РК
        var usdToKzt = await GetUsdToKztAsync();

        // 2. Получаем крипто с CoinGecko (цены в USD)
        var coins = await _http.GetFromJsonAsync<List<CoinGeckoMarket>>(
            CoinsUrl,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (coins == null) return new();

        return coins.Select(c => new CryptoRate
        {
            Id = c.Id,
            Symbol = c.Symbol.ToUpper(),
            Name = c.Name,
            Icon = CryptoIcons.GetValueOrDefault(c.Id, "🪙"),
            PriceUsd = c.Current_Price,
            PriceKzt = c.Current_Price * usdToKzt,
            Change24h = c.Price_Change_Percentage_24h,
            MarketCapUsd = c.Market_Cap,
            Volume24hUsd = c.Total_Volume,
        }).ToList();
    }

    private async Task<decimal> GetUsdToKztAsync()
    {
        try
        {
            var xml = await _http.GetStringAsync(UsdKztUrl);
            var doc = System.Xml.Linq.XDocument.Parse(xml);

            var usdItem = doc.Descendants("item")
                .FirstOrDefault(i => i.Element("title")?.Value?.Trim().ToUpper() == "USD");

            if (usdItem != null)
            {
                var val = usdItem.Element("description")?.Value ?? "";
                if (decimal.TryParse(val.Replace(",", "."),
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var rate))
                    return rate;
            }
        }
        catch { }

        // Запасной курс если НБ РК недоступен
        return 500m;
    }
}

// Модель ответа CoinGecko /coins/markets
public class CoinGeckoMarket
{
    public string Id { get; set; } = "";
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Current_Price { get; set; }
    public decimal Price_Change_Percentage_24h { get; set; }
    public decimal Market_Cap { get; set; }
    public decimal Total_Volume { get; set; }
    public string? Image { get; set; }
}