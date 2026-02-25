namespace CurrencyApp1.Models;

/// <summary>
/// Курс валюты от Национального Банка Республики Казахстан
/// </summary>
public class NbrkRate
{
    /// <summary>Код валюты (USD, EUR, RUB...)</summary>
    public string Code { get; set; } = "";

    /// <summary>Курс в тенге за Quantity единиц</summary>
    public decimal Rate { get; set; }

    /// <summary>Количество единиц (обычно 1, для некоторых валют 100)</summary>
    public int Quantity { get; set; } = 1;

    /// <summary>Изменение курса относительно предыдущего дня</summary>
    public decimal Change { get; set; }

    /// <summary>Название валюты на русском</summary>
    public string Name { get; set; } = "";

    /// <summary>Флаг страны (emoji)</summary>
    public string Flag { get; set; } = "🌍";

    /// <summary>Курс за 1 единицу валюты</summary>
    public decimal RatePerOne => Quantity > 1 ? Rate / Quantity : Rate;
}
