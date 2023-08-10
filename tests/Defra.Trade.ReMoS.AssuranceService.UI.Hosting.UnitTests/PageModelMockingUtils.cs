using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests
{
    internal class PageModelMockingUtils
    {
        public PageContext MockPageContext()
        {
            var claimsIdentity = new ClaimsIdentity(It.IsAny<IEnumerable<Claim>>());
            var principle = new ClaimsPrincipal(claimsIdentity);

            var httpContext = new DefaultHttpContext()
            {
                User = principle
            };

            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);

            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };

            return pageContext;
        }
    }
}
