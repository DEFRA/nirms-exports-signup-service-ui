using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;

public class LogisticsLocation
{
    public Guid Id { get; set; }

    //public int? RemosNumber { get; set; }
    public string? Name { get; set; }
    public TradeAddress? Address { get; set; }
    public Guid TradeAddressId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public string? NI_GBFlag { get; set; }
    public virtual LogisticLocationBusinessRelationship? EstablishmentBusinessRelationship { get; set; }
}
