﻿using System;
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
        public bool ReadyForCheckAnswers(TradePartyDTO tradeParty)
        {
            if (tradeParty == null)
            {
                return false;
            }

            if (GetEligibilityProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return false;
            }

            if (GetBusinessDetailsProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return false;
            }

            if (GetAuthorisedSignatoryProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return false;
            }

            if (GetContactDetailsProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return false;
            }

            return true;
        }

        public string GetBusinessDetailsProgress(TradePartyDTO tradeParty)
        {
            if (tradeParty.PracticeName != null
                && tradeParty.Address != null
                && tradeParty.Address.TradeCountry != null)
            {
                return TaskListStatus.COMPLETE;
            }

            if (tradeParty?.PracticeName != null
                && (tradeParty?.Address == null || tradeParty?.Address?.TradeCountry == null))
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
            if (tradeParty.Contact == null || tradeParty?.Contact?.PersonName == null || tradeParty?.Contact?.Email == null || tradeParty?.Contact?.TelephoneNumber == null || tradeParty?.Contact?.Position == null)
            {
                return TaskListStatus.CANNOTSTART;
            }

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

                if (tradeParty.Contact?.IsAuthorisedSignatory == false && tradeParty.AuthorisedSignatory?.Id != null && tradeParty.AuthorisedSignatory?.Id != Guid.Empty)
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