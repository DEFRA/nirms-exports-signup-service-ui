﻿using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces
{
    public interface ICheckAnswersService
    {
        string GetBusinessDetailsProgress(TradePartyDTO tradeParty);
        string GetContactDetailsProgress(TradePartyDTO tradeParty);
        string GetAuthorisedSignatoryProgress(TradePartyDTO tradeParty);
    }
}