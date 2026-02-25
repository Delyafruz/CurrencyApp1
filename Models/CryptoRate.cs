namespace CurrencyApp1.Models;

public class CryptoRate
{
    public string Id { get; set; } = "";
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public decimal PriceUsd { get; set; }
    public decimal PriceKzt { get; set; }
    public decimal Change24h { get; set; }
    public decimal MarketCapUsd { get; set; }
    public decimal Volume24hUsd { get; set; }
}

// Ответ CoinGecko API
public class CoinGeckoResponse
{
    public string Id { get; set; } = "";
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public CoinGeckoMarketData Market_Data { get; set; } = new();
}

public class CoinGeckoMarketData
{
    public Dictionary<string, decimal> Current_Price { get; set; } = new();
    public Dictionary<string, decimal> Price_Change_Percentage_24h_In_Currency { get; set; } = new();
    public Dictionary<string, decimal> Market_Cap { get; set; } = new();
    public Dictionary<string, decimal> Total_Volume { get; set; } = new();
}