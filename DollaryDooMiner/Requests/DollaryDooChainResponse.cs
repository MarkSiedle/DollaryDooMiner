using System.Collections.Generic;

namespace DollaryDooMiner.Requests
{
    public class DollaryDooChainResponse
    {
        public int index { get; set; }
        public int proof { get; set; }
        public string previous_hash { get; set; }
        public string timestamp { get; set; }
        public List<DollaryDooChainTransaction> transactions { get; set; }
    }

    public class DollaryDooChainTransaction
    {
        public string from { get; set; }
        public string to { get; set; }
        public int index { get; set; }
    }
}
