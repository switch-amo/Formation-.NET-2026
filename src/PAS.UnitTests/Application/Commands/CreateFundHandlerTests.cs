using FluentAssertions;
using NSubstitute;
using PAS.Application.Commands.Funds.CreateFund;
using PAS.Application.Dtos;
using PAS.Domain.Abstractions;
using PAS.Domain.Funds;
using PAS.Domain.Repositories;

namespace PAS.UnitTests.Application.Commands.Funds.CreateFund;
public class CreateFundHandlerTests {
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateFund() {
        // Arrange
        var repository = Substitute.For<IFundRepository>();

        var handler = new CreateFundHandler(repository);
        var command = new CreateFundCommand("Global Equity", "FR0000120271", "EUR");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        await repository
            .Received(1)
            .AddAsync(
                Arg.Any<Fund>(),
                Arg.Any<CancellationToken>());

        await repository
            .Received(1)
            .SaveChangesAsync(
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidName_ShouldThrowException() {
        // Arrange
        var repository = Substitute.For<IFundRepository>();

        var handler = new CreateFundHandler(repository);
        var command = new CreateFundCommand("", "FR0000120271", "EUR");

        // Act
        var action = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<DomainException>();

        await repository
            .DidNotReceive()
            .AddAsync(
                Arg.Any<Fund>(),
                Arg.Any<CancellationToken>());
    }
}