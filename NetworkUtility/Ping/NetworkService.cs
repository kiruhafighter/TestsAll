using NetworkUtility.DNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
 
namespace NetworkUtility.Ping
{
    public class NetworkService
    {
        private readonly IDNS _dnsService;

        public NetworkService(IDNS dnsService)
        {
            _dnsService = dnsService;
        }
        public string SendPing()
        {
            //BuildPacket()
            bool dnsSuccess = _dnsService.SendDNS();

            if (dnsSuccess) return "Success: Ping Sent!";

            return "Failed: Ping not sent!";
        }

        public int PingTimeout(int a, int b)
        {
            return a + b;
        }

        public DateTime LastPingDate()
        {
            return DateTime.Now;
        }

        public PingOptions GetPingOptions()
        {
            return new PingOptions() 
            { 
                DontFragment = true,
                Ttl = 1
            };
        }

        public IEnumerable<PingOptions> MostRecentPings()
        {
            IEnumerable<PingOptions> pings = new[]
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

            return pings;
        }
    }
}
