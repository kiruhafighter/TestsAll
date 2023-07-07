using FakeItEasy;
using FluentAssertions;
using Moq;
using NetworkUtility.DNS;
using NetworkUtility.Ping;
using System.Net.NetworkInformation;

namespace NetworkUtility.Tests.PingTests
{
    public class NetworkServiceTests
    {
        //Arrange - variables, classes, mocks
        private readonly NetworkService _pingService;
        private readonly Mock<IDNS> _dnsService;

        //private readonly IDNS _dnsService;

        public NetworkServiceTests() 
        {
            //FakeItEasy
            //_dnsService = A.Fake<IDNS>();
            //A.CallTo(() => _dnsService.SendDNS()).Returns(true);
            
            //Moq
            _dnsService = new Mock<IDNS>();
            _dnsService.Setup(ser => ser.SendDNS()).Returns(true);

            //SUT
            _pingService = new NetworkService(_dnsService.Object);
        }

        [Fact]
        public void NetworkService_SendPing_ReturnString()
        {
            //Arrange - variables, classes, mocks

            //Act
            var result = _pingService.SendPing();

            //Assertions
            //FluentAssertions
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Be("Success: Ping Sent!");
            result.Should().Contain("Success", Exactly.Once());

            //xUnit Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2, 2, 4)]
        public void NetworkService_PingTimeout_ReturnInt(int a, int b, int expected)
        {
            //Arrange

            //Act
            int result = _pingService.PingTimeout(a, b);

            //Assert
            result.Should().Be(expected);
            result.Should().NotBeInRange(-1000, 0);
            result.Should().BeGreaterThanOrEqualTo(2);

            Assert.NotEqual(10, result);
        }

        [Fact]
        public void NetworkService_LastPingDate_ReturnDate()
        {
            //Act
            var result = _pingService.LastPingDate();

            //Assert
            result.Should().NotBeBefore(new DateTime(2023, 7, 6));
            result.Should().NotBeAfter(DateTime.Now);
            result.Should().BeAfter(DateTime.Now.AddDays(-1));
        }

        [Fact]
        public void NetworkService_GetPingOptions_ReturnsObject()
        {
            //Arrange
            var expected = new PingOptions()
            {
                DontFragment = true,
                Ttl = 1
            };

            //Act
            var result = _pingService.GetPingOptions();

            //Assert WARNING : bew careful
            result.Should().BeOfType<PingOptions>();
            Assert.IsType<PingOptions>(result);
            result.Should().BeEquivalentTo(expected);
            result.Ttl.Should().Be(1);
        }

        [Fact]
        public void NetworkService_MostRecentPings_ReturnsIEnumerable()
        {
            //Arrange
            IEnumerable<PingOptions> expected = new[]
            {
                new PingOptions()
                {
                    DontFragment = true,
                    Ttl = 1
                },
                new PingOptions()
                {
                    DontFragment = false,
                    Ttl = 2
                }
            };

            var expected2 = new PingOptions()
            {
                DontFragment = true,
                Ttl = 1
            };

            IEnumerable<PingOptions> expected3 = new List<PingOptions>()
            {
                new PingOptions()
                {
                    DontFragment = true,
                    Ttl = 1
                },
                new PingOptions()
                {
                    DontFragment = false,
                    Ttl = 2
                }
            };

 
            var result = _pingService.MostRecentPings();

            //Assert
            result.Should().BeAssignableTo<IEnumerable<PingOptions>>();
            result.Should().Contain(x => x.DontFragment == false);
            result.Should().ContainEquivalentOf(expected2);
            result.Should().BeEquivalentTo(expected);
            result.Should().BeEquivalentTo(expected3);
        }
    }
}