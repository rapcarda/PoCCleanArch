using Bookify.Domain.Abstractions;
using Bookify.Domain.Users.Events;
using Bookify.Domain.Users.ValueObjects;

namespace Bookify.Domain.Users;

public class User : Entity
{
    private User(
        Guid id,
        FirstName firstName,
        LastName lastName,
        Email email) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public FirstName FirstName { get; private set; }
    public LastName LastName{ get; private set; }
    public Email Email{ get; private set; }

    // Uma maneira diferente, deixando o construtor privado e expondo um método que irá retornar um User.
    // Prós: Esconder o construtor da classe que pode haver alguma regra que não queira expor
    //       Poder adicionar alguma regra no método Create que não caiba no construtor default da classe
    public static User Create(FirstName firstName, LastName lastName, Email email)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        return user;
    }

}
