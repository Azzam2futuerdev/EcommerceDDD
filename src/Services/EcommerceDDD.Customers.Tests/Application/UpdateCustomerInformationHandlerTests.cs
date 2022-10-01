using Moq;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Customers.Domain.Commands;
using EcommerceDDD.Customers.Api.Application.UpdatingCustomerInformation;

namespace EcommerceDDD.Customers.Tests.Application;

public class UpdateCustomerInformationHandlerTests
{
    [Fact]
    public async Task Update_WithCommand_ShouldUpdateCustomer()
    {
        // Given
        string email = "email@test.com";
        string name = "UserTest";
        string password, confirmation;
        password = confirmation = "p4ssw0rd";
        string address = "Rue XYZ";
        decimal creditLimit = 1000;

        _checker.Setup(p => p.IsUnique(It.IsAny<string>()))
            .Returns(true);
        
        var customerWriteRepository = new DummyEventStoreRepository<Customer>();

        var registerCommand = RegisterCustomer.Create(email, password, confirmation, name, address, creditLimit);
        var customerData = new CustomerData(email, name, address, creditLimit);
        var customer = Customer.Create(customerData);
        await customerWriteRepository.AppendEventsAsync(customer);

        var newName = "New Name";
        var newAddress = "New Address";
        var updateCommand = UpdateCustomerInformation.Create(customer.Id, newName, newAddress, creditLimit);
        var commandHandler = new UpdateCustomerInformationHandler(customerWriteRepository);

        // When
        await commandHandler.Handle(updateCommand, CancellationToken.None);
        var updatedCustomer = await customerWriteRepository.FetchStreamAsync(customer.Id.Value);

        // Then
        updatedCustomer.Name.Should().Be(newName);
        updatedCustomer.ShippingAddress.Should().Be(Address.Create(newAddress));
    }

    private Mock<IEmailUniquenessChecker> _checker = new();
}