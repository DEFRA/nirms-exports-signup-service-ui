using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

public class TradeAddressAddUpdateDTO
{
    public string? LineOne { get; set; }
    public string? LineTwo { get; set; }
    public string? CityName { get; set; }
    public string? PostCode { get; set; }
}
