using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;

public class TradeParty
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    //Also in LDM
    //Role Code
}

public record AddTradePartyContract(
    string? Name);