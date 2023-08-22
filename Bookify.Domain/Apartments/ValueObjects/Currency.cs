namespace Bookify.Domain.Apartments.ValueObjects;

public record Currency
{
    //Internal por que não é para expor esse valor, apenas para uso interno em Domain
    internal static readonly Currency None = new("");
    //Currency que podem ser utilizadas no projeto
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");
    public static readonly Currency Real = new("REAL");

    private Currency(string code) => Code = code;

    public string Code { get; init; }

    //Método para retornar uma instância de Currency de acordo com uma string
    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ??
            throw new ApplicationException("The currency code is invalid");
    }

    //Expondo propriedade com todas currencies disponíveis
    public static readonly IReadOnlyCollection<Currency> All = new[] { Usd, Eur, Real };
}
