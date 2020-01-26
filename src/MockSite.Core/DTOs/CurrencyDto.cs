namespace MockSite.Core.DTOs
{
    public class CurrencyDto
    {
        public string CurrencyCode { get; }

        public string CurrencyRate { get; }

        public CurrencyDto(string currencyCode, string currencyRate)
        {
            this.CurrencyCode = currencyCode;
            this.CurrencyRate = currencyRate;
        }
    }
}