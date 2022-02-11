using System.Collections.Generic;
using BinanceBotApp.Data;

namespace BinanceBotApp.DataInternal.Deserializers
{
    public class AccountBalanceInfo
    {
        public int MakerComission { get; set; }
        public int TakerComission { get; set; }
        public int BuyerComission { get; set; }
        public int SellerComission { get; set; }
        public bool CanTrade { get; set; }
        public bool CanWithdraw { get; set; }
        public bool CanDeposit { get; set; }
        public long UpdateTime { get; set; }
        public string AccountType { get; set; }
        public IEnumerable<CoinAmountDto> Balances { get; set; }
    }
}