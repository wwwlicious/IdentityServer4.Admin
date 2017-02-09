namespace IdentityAdmin.Api.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core;
    using Core.ApiResource;
    using Extensions;
    using Filters;
    using Models.ApiResource;
    using Resources;

    [RoutePrefix(Constants.ApiResourcesRoutePrefix)]
    [NoCache]
    public class ApiResourceController : ApiController
    {
        private readonly IApiResourceService _service;

        public ApiResourceController(IApiResourceService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            _service = service;
        }

        ApiResourceMetaData _metadata;

        async Task<ApiResourceMetaData> GetCoreMetaDataAsync()
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

        [HttpGet, Route("", Name = Constants.RouteNames.GetApiResources)]
        public async Task<IHttpActionResult> GetApiResourcesAsync(string filter = null, int start = 0, int count = 100)
        {
            var result = await _service.QueryAsync(filter, start, count);
            if (result.IsSuccess)
            {
                var meta = await GetCoreMetaDataAsync();
                var resource = new ApiResourceQueryResultResource(result.Result, Url, meta);
                return Ok(resource);
            }
            return BadRequest(result.ToError());
        }

        [HttpGet, Route("{subject}", Name = Constants.RouteNames.GetApiResource)]
        public async Task<IHttpActionResult> GetApiResourceAsync(string subject)
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
                return Ok(new ApiResourceDetailResource(result.Result, Url, meta));
            }

            return BadRequest(result.ToError());
        }

        [HttpPost, Route("", Name = Constants.RouteNames.CreateApiResource)]
        public async Task<IHttpActionResult> CreateApiResourceAsync(PropertyValue[] properties)
        {
            return BadRequest("");
        }

        [HttpDelete, Route("{subject}", Name = Constants.RouteNames.DeleteApiResource)]
        public async Task<IHttpActionResult> DeleteApiResourceAsync(string subject)
        {
            return BadRequest("");
        }
    }
}
