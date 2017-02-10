namespace IdentityAdmin.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public IHttpActionResult NoContent()
        {
            return StatusCode(HttpStatusCode.NoContent);
        }

        public IHttpActionResult MethodNotAllowed()
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
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
            var metadata = await GetCoreMetaDataAsync();
            if (!metadata.SupportsCreate)
            {
                return MethodNotAllowed();
            }
            if (properties == null)
            {
                ModelState.AddModelError("", Messages.IdentityResourceDataRequired);
            }

            var errors = ValidateCreateProperties(metadata, properties);
            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                var result = await _service.CreateAsync(properties);
                if (result.IsSuccess)
                {
                    var url = Url.RelativeLink(Constants.RouteNames.GetIdentityResource, new { subject = result.Result.Subject });
                    var resource = new
                    {
                        Data = new { subject = result.Result.Subject },
                        Links = new { detail = url }
                    };
                    return Created(url, resource);
                }

                ModelState.AddErrors(result);
            }
            return BadRequest("");
        }

        [HttpDelete, Route("{subject}", Name = Constants.RouteNames.DeleteIdentityResource)]
        public async Task<IHttpActionResult> DeleteIdentityResourceAsync(string subject)
        {
            var meta = await GetCoreMetaDataAsync();
            if (!meta.SupportsDelete)
            {
                return MethodNotAllowed();
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToError());
            }

            var result = await _service.DeleteAsync(subject);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.ToError());
        }

        [HttpPut, Route("{subject}/properties/{type}", Name = Constants.RouteNames.UpdateIdentityResourceProperty)]
        public async Task<IHttpActionResult> SetPropertyAsync(string subject, string type)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            type = type.FromBase64UrlEncoded();

            string value = await Request.Content.ReadAsStringAsync();
            var meta = await GetCoreMetaDataAsync();
            ValidateUpdateProperty(meta, type, value);

            if (ModelState.IsValid)
            {
                var result = await _service.SetPropertyAsync(subject, type, value);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }


        [HttpPost, Route("{subject}/identityresourceclaim", Name = Constants.RouteNames.AddIdentityResourceClaim)]
        public async Task<IHttpActionResult> AddIdentityResourceClaimAsync(string subject, IdentityResourceClaimValue model)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (model == null)
            {
                ModelState.AddModelError("", "Model required");
            }

            if (ModelState.IsValid)
            {
                var result = await _service.AddClaimAsync(subject, model.Type);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }

        [HttpDelete, Route("{subject}/identityresourceclaim/{id}", Name = Constants.RouteNames.RemoveIdentityResourceClaim)]
        public async Task<IHttpActionResult> RemoveIdentityResourceClaimAsync(string subject, string id)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var result = await _service.RemoveClaimAsync(subject, id);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.ToError());
        }

        private IEnumerable<string> ValidateCreateProperties(IdentityResourceMetaData identityResourceMetaData, IEnumerable<PropertyValue> properties)
        {
            if (identityResourceMetaData == null) throw new ArgumentNullException(nameof(identityResourceMetaData));
            properties = properties ?? Enumerable.Empty<PropertyValue>();

            var meta = identityResourceMetaData.CreateProperties;
            return meta.Validate(properties);
        }

        private void ValidateUpdateProperty(IdentityResourceMetaData identityResourceMetaData, string type, string value)
        {
            if (identityResourceMetaData == null) throw new ArgumentNullException(nameof(identityResourceMetaData));

            if (string.IsNullOrWhiteSpace(type))
            {
                ModelState.AddModelError("", Messages.PropertyTypeRequired);
                return;
            }

            var prop = identityResourceMetaData.UpdateProperties.SingleOrDefault(x => x.Type == type);
            if (prop == null)
            {
                ModelState.AddModelError("", string.Format(Messages.PropertyInvalid, type));
            }
            else
            {
                var error = prop.Validate(value);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                }
            }
        }
    }
}
