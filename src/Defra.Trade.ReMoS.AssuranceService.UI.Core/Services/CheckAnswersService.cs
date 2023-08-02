using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services
{
    public class CheckAnswersService : ICheckAnswersService
    {
        public string GetBusinessDetailsProgress(TradePartyDTO tradeParty)
        {
            if (tradeParty.PartyName != null && tradeParty.Address != null
                && tradeParty.Address.LineOne != null && tradeParty.Address.PostCode != null)
            {
                return TaskListStatus.COMPLETE;
            }

            if (tradeParty?.PartyName != null && (tradeParty?.Address == null
                || tradeParty?.Address?.LineOne == null || tradeParty?.Address?.PostCode == null))
            {
                return TaskListStatus.INPROGRESS;
            }

            return TaskListStatus.NOTSTART;
        }

        public string GetContactDetailsProgress(TradePartyDTO tradeParty)
        {
            if (tradeParty.Contact != null && tradeParty.Contact.PersonName != null && tradeParty.Contact.Email != null && tradeParty.Contact.TelephoneNumber != null && tradeParty.Contact.Position != null)
            {
                return TaskListStatus.COMPLETE;
            }

            if (tradeParty.Contact != null || tradeParty?.Contact?.PersonName != null || tradeParty?.Contact?.Email != null || tradeParty?.Contact?.TelephoneNumber != null || tradeParty?.Contact?.Position != null)
            {
                return TaskListStatus.INPROGRESS;
            }

            return TaskListStatus.NOTSTART;
        }

        public string GetAuthorisedSignatoryProgress(TradePartyDTO tradeParty)
        {
            if (tradeParty.AuthorisedSignatory != null && tradeParty.Contact != null)
            {
                if (tradeParty.Contact?.IsAuthorisedSignatory == true)
                {
                    return TaskListStatus.COMPLETE;
                }

                if (tradeParty.Contact?.IsAuthorisedSignatory == false && tradeParty.AuthorisedSignatory.Name != null && tradeParty.AuthorisedSignatory.Position != null && tradeParty.AuthorisedSignatory.EmailAddress != null)
                {
                    return TaskListStatus.COMPLETE;
                }

                if (tradeParty.Contact?.IsAuthorisedSignatory == false && tradeParty.AuthorisedSignatory.Name != null || tradeParty.AuthorisedSignatory.Position != null || tradeParty.AuthorisedSignatory.EmailAddress != null)
                {
                    return TaskListStatus.INPROGRESS;
                }
            }

            return TaskListStatus.NOTSTART;
        }

        public string GetEligibilityProgress(TradePartyDTO tradeParty)
        {
            if (tradeParty.Address != null)
            {
                if (tradeParty.Address!.TradeCountry != null && !string.IsNullOrEmpty(tradeParty.FboNumber) && tradeParty.RegulationsConfirmed)
                {
                    return TaskListStatus.COMPLETE;
                }
                if (tradeParty.Address!.TradeCountry == null || string.IsNullOrEmpty(tradeParty.FboNumber) || !tradeParty.RegulationsConfirmed)
                {
                    return TaskListStatus.INPROGRESS;
                }
            }
            return TaskListStatus.NOTSTART;
        }
    }
}
