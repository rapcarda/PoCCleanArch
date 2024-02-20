namespace Bookify.Application.Exceptions;
public sealed class ConcurrencyException : Exception
{
    //Esta classe será usada no tratamento de concorrencia do banco de dados. Criado uma classe a parte para não expor para o usuário a exception
    public ConcurrencyException(string message, Exception innerException): base(message, innerException)
    {
    }
}
