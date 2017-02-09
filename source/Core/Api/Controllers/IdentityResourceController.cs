namespace IdentityAdmin.Api.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core;
    using Core.IdentityResource;
    using Extensions;
    using Filters;
    using Models.IdentityResource;
    using Resources;

    [RoutePrefix(Constants.IdentityResourcesRoutePrefix)]
    [NoCache]
    public class IdentityResourceController : ApiController
    {
        private readonly IIdentityResourceService _service;

        public IdentityResourceController(IIdentityResourceService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            _service = service;            
        }

        IdentityResourceMetaData _metadata;

        async Task<IdentityResourceMetaData> GetCoreMetaDataAsync()
        {
            if (_metadata == null)
            {
                _metadata = await _service.GetMetadataAsync();
                if (_metadata == null) throw new InvalidOperationException("IdentityResourceMetaData returned null");
                _metadata.Validate();

                return _metadata;
            }

            return _metadata;
        }

        public IHttpActionResult BadRequest<T>(T data)
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, data));
        }

        [HttpGet, Route("", Name = Constants.RouteNames.GetIdentityResources)]
        public async Task<IHttpActionResult> GetIdentityResourcesAsync(string filter = null, int start = 0, int count = 100)
        {
            var result = await _service.QueryAsync(filter, start, count);
            if (result.IsSuccess)
            {
                var meta = await GetCoreMetaDataAsync();
                var resource = new IdentityResourceQueryResultResource(result.Result, Url, meta);
                return Ok(resource);
            }
            return BadRequest(result.ToError());
        }

        [HttpGet, Route("{subject}", Name = Constants.RouteNames.GetIdentityResource)]
        public async Task<IHttpActionResult> GetIdentityResourceAsync(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToError());
            }

            var result = await _service.GetAsync(subject);
            if (result.IsSuccess)
            {
                if (result.Result == null)
                {
                    return NotFound();
                }

                var meta = await GetCoreMetaDataAsync();
                return Ok(new IdentityResourceDetailResource(result.Result, Url, meta));
            }

            return BadRequest(result.ToError());
        }

        [HttpPost, Route("", Name = Constants.RouteNames.CreateIdentityResource)]
        public async Task<IHttpActionResult> CreateIdentityResourceAsync(PropertyValue[] properties)
        {
            //var coreMetadata = await GetCoreMetaDataAsync();
            //if (!coreMetadata.ClientMetaData.SupportsCreate)
            //{
            //    return MethodNotAllowed();
            //}
            //if (properties == null)
            //{
            //    ModelState.AddModelError("", Messages.ClientDataRequired);
            //}

            //var errors = ValidateCreateProperties(coreMetadata.ClientMetaData, properties);
            //foreach (var error in errors)
            //{
            //    ModelState.AddModelError("", error);
            //}

            //if (ModelState.IsValid)
            //{
            //    var result = await _identityAdminService.CreateClientAsync(properties);
            //    if (result.IsSuccess)
            //    {
            //        var url = Url.RelativeLink(Constants.RouteNames.GetClient, new { subject = result.Result.Subject });
            //        var resource = new
            //        {
            //            Data = new { subject = result.Result.Subject },
            //            Links = new { detail = url }
            //        };
            //        return Created(url, resource);
            //    }

            //    ModelState.AddErrors(result);
            //}

            return BadRequest("");
        }

        [HttpDelete, Route("{subject}", Name = Constants.RouteNames.DeleteIdentityResource)]
        public async Task<IHttpActionResult> DeleteIdentityResourceAsync(string subject)
        {
            return BadRequest("");
        }

        //public const string DeleteIdentityResource = "DeleteIdentityResource";
    }
}
