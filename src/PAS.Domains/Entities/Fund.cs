using PAS.Domain.Entities.Enums;

namespace PAS.Domain.Entities {
    public sealed class Fund {

        public Fund(string id, string name, string isin, string currency, FundStatusEnum status) {
            Id = id;
            Name = name;
            Isin = isin;
            Currency = currency;
            Status = status;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Isin { get; set; }
        public string Currency { get; set; }
        public FundStatusEnum Status { get; set; }
    }
}
