using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.ResponseTypes;


public class ResponseTradePartyCollectionType
{
    public List<TradeParty> TradeParties { get; set; } = default!;
}
public class ResponseTradePartyType
{
    public TradeParty TradeParty { get; set; } = default!;
}
