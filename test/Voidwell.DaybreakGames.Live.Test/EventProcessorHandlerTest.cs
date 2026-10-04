using System.Text.Json;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Live.CensusStream;
using Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Live.Mappers;
using Xunit;
using DataPlayerLogin = Voidwell.DaybreakGames.Data.Models.Planetside.Events.PlayerLogin;

namespace Voidwell.DaybreakGames.Live.Test;

public class EventProcessorHandlerTest
{
    private readonly Mock<IEventRepository> _eventRepository = new();
    private readonly Mock<IPlayerMonitor> _playerMonitor = new();
    private readonly Mock<IEventValidator> _validator = new();
    private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.AddProfile<CensusToDataMappingProfile>(), NullLoggerFactory.Instance).CreateMapper();

    private PlayerLoginProcessor CreateProcessor()
    {
        _eventRepository.Setup(a => a.AddAsync(It.IsAny<DataPlayerLogin>())).Returns(Task.CompletedTask);
        _playerMonitor.Setup(a => a.SetOnlineAsync(It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync((Domain.Models.OnlineCharacter)null);

        return new PlayerLoginProcessor(_eventRepository.Object, _playerMonitor.Object, _validator.Object, _mapper);
    }

    private static EventProcessorHandler CreateSut(PlayerLoginProcessor processor)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IEventProcessor<PlayerLogin>>(processor);

        return new EventProcessorHandler(services.BuildServiceProvider());
    }

    [Fact]
    public async Task TryProcessAsync_KnownEvent_DeserializesPayloadAndInvokesProcessor()
    {
        _validator.Setup(a => a.Validiate(It.IsAny<PlayerLogin>(), It.IsAny<Func<PlayerLogin, string>>(), It.IsAny<Func<PlayerLogin, bool>>())).ReturnsAsync(true);
        var sut = CreateSut(CreateProcessor());
        var payload = JsonDocument.Parse("""{"character_id":"c1","event_name":"PlayerLogin","timestamp":"1700000000","world_id":"17"}""").RootElement;

        var handled = await sut.TryProcessAsync("PlayerLogin", payload);

        handled.Should().BeTrue();
        _eventRepository.Verify(a => a.AddAsync(It.Is<DataPlayerLogin>(e => e.CharacterId == "c1" && e.WorldId == 17)), Times.Once);
        _playerMonitor.Verify(a => a.SetOnlineAsync("c1", new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc)), Times.Once);
    }

    [Fact]
    public async Task TryProcessAsync_UnknownEvent_ReturnsFalse()
    {
        var sut = CreateSut(CreateProcessor());
        var payload = JsonDocument.Parse("""{"event_name":"Nothing"}""").RootElement;

        (await sut.TryProcessAsync("Nothing", payload)).Should().BeFalse();
    }

    [Fact]
    public async Task PlayerLoginProcessor_DuplicateEvent_IsIgnored()
    {
        _validator.Setup(a => a.Validiate(It.IsAny<PlayerLogin>(), It.IsAny<Func<PlayerLogin, string>>(), It.IsAny<Func<PlayerLogin, bool>>())).ReturnsAsync(false);
        var processor = CreateProcessor();

        await processor.Process(new PlayerLogin { CharacterId = "c1", Timestamp = DateTime.UtcNow, WorldId = 17 });

        _eventRepository.Verify(a => a.AddAsync(It.IsAny<DataPlayerLogin>()), Times.Never);
        _playerMonitor.Verify(a => a.SetOnlineAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task PlayerLoginProcessor_ValidatesByCharacterId()
    {
        string capturedKey = null;
        _validator.Setup(a => a.Validiate(It.IsAny<PlayerLogin>(), It.IsAny<Func<PlayerLogin, string>>(), It.IsAny<Func<PlayerLogin, bool>>()))
            .Callback((PlayerLogin ev, Func<PlayerLogin, string> key, Func<PlayerLogin, bool> cleanup) => capturedKey = key(ev))
            .ReturnsAsync(false);
        var processor = CreateProcessor();

        await processor.Process(new PlayerLogin { CharacterId = "c1", Timestamp = DateTime.UtcNow, WorldId = 17 });

        capturedKey.Should().Be("c1");
    }

    [Fact]
    public void CensusToDataMappingProfile_AllMapsCompile()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<CensusToDataMappingProfile>(), NullLoggerFactory.Instance);

        var act = () => configuration.CompileMappings();

        act.Should().NotThrow();
    }
}
