using FluentAssertions;
using NSubstitute;
using PAS.Application.Queries.Funds.GetFundList;
using PAS.Domain.Funds;
using PAS.Domain.Funds.ValueObjects;
using PAS.Domain.Repositories;

namespace PAS.UnitTests.Application.Queries.GetFundList;

public class GetFundListHandlerTests {
    [Fact]
    public async Task Handle_ShouldReturnFunds() {
        // Arrange
        var repository = Substitute.For<IFundRepository>();

        var fund = Fund.Create(
            "Global Equity",
            Isin.Create("FR0000120271"),
            Currency.Create("EUR"));


        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Fund> { fund });

        var handler = new GetFundListHandler(repository);

        // Act
        var result = await handler.Handle(
            new GetFundListQuery(),
            CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);

        result.First()
            .Name
            .Should()
            .Be("Global Equity");


        await repository
            .Received(1)
            .GetAllAsync(
                Arg.Any<CancellationToken>());
    }
}