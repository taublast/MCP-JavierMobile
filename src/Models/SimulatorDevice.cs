#nullable disable
using System.Runtime.Serialization;

namespace MobileDevMcpServer.Models
{
    public class SimulatorDevice
    {
        [DataMember(Name = "udid")]
        public string Udid { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "runtime")]
        public string Runtime { get; set; }
    }
}
