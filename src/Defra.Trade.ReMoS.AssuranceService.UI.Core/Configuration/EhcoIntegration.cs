using Microsoft.Azure.Management.AppService.Fluent.WebAppAuthentication.Definition;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EhcoIntegration
    {
        public string ValidIssuer { get; set; } = string.Empty;
        public string ValidAudience { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
    }
}
