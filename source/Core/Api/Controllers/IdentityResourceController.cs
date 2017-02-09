namespace IdentityAdmin.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Filters;

    [RoutePrefix(Constants.IdentityResourcesRoutePrefix)]
    [NoCache]
    public class IdentityResourceController : ApiController
    {
        public IdentityResourceController()
        {
            //if (identityAdminService == null) throw new ArgumentNullException("identityAdminService");
            //_identityAdminService = identityAdminService;
        }

        [HttpGet, Route("", Name = Constants.RouteNames.GetIdentityResources)]
        public async Task<IHttpActionResult> GetIdentityResourcesAsync(string filter = null, int start = 0, int count = 100)
        {
            //var result = await _identityAdminService.QueryScopesAsync(filter, start, count);
            //if (result.IsSuccess)
            //{
            //    var meta = await GetCoreMetaDataAsync();
            //    var resource = new ScopeQueryResultResource(result.Result, Url, meta.ScopeMetaData);
            //    return Ok(resource);
            //}

            return BadRequest("");
        }
    }
}
