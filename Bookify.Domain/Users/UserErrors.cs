using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;
public static class UserErrors
{
    public static Error NotFoud = new(
        "User.NotFound",
        "The user with the specified indentifier was not foud");
}
