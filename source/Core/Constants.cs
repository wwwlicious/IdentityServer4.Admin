/*
 * Copyright 2014 Dominick Baier, Brock Allen, Bert Hoorne
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace IdentityAdmin
{
    using Newtonsoft.Json.Serialization;

    public class Constants
    {
        public const string LocalAuthenticationType = "idAdmin.Local";
        public const string CookieAuthenticationType = "Cookies";
        public const string BearerAuthenticationType = "Bearer";

        public const string AuthorizePath = "/authorize";
        public const string CallbackFragment = "/#/callback/";
        public const string IdAdmMgrClientId = "idAdmin";
        public const string IdAdminScope = "idAdmin";
        public const string AdminRoleName = "IdentityAdminAdministrator";

        internal const string AutofacScope = "idAdmin:AutofacScope";

        public static readonly TimeSpan DefaultTokenExpiration = TimeSpan.FromHours(10);

        public class ClaimTypes
        {
            public const string Subject = "sub";
            public const string Username = "username";
            public const string Name = "name";
            public const string Password = "password";
            public const string Email = "email";
            public const string Phone = "phone";
            public const string Role = "role";
            public const string BootstrapToken = "bootstrap-token";
        }

        public const string RoutePrefix = "api";
        public const string MetadataRoutePrefix = RoutePrefix + "";
        public const string ScopeRoutePrefix = RoutePrefix + "/scopes";
        public const string ClientsRoutePrefix = RoutePrefix + "/clients";
        public const string IdentityResourcesRoutePrefix = RoutePrefix + "/identityresources";
        public const string ApiResourcesRoutePrefix = RoutePrefix + "/apiresources";

        public class RouteNames
        {
           
            public const string GetClients = "GetClients";
            public const string GetClient = "GetClient";
            public const string CreateClient = "CreateClient";
            public const string DeleteClient = "DeleteClient";
            public const string UpdateClientProperty = "UpdateClientProperty";
            public const string AddClientSecret = "AddClientSecret";
            public const string RemoveClientSecret = "RemoveClientSecret";
            public const string AddClientClaim = "AddClientClaim";
            public const string RemoveClientClaim = "RemoveClientClaim";
            public const string AddClientIdPRestriction = "AddClientIdPRestriction";
            public const string RemoveClientIdPRestriction = "RemoveClientIdPRestriction";
            public const string AddClientPostLogoutRedirectUri = "AddClientPostLogoutRedirectUri";
            public const string RemoveClientPostLogoutRedirectUri = "RemoveClientPostLogoutRedirectUri";
            public const string AddClientRedirectUri = "AddClientRedirectUri";
            public const string RemoveClientRedirectUri = "RemoveClientRedirectUri";
            public const string AddClientCorsOrigin = "AddClientCorsOrigin";
            public const string RemoveClientCorsOrigin = "RemoveClientCorsOrigin";
            public const string AddClientCustomGrantType = "AddClientCustomGrantType";
            public const string RemoveClientCustomGrantType = "RemoveClientCustomGrantType";
            public const string AddClientScope = "AddClientScope";
            public const string RemoveClientScope = "RemoveClientScope";

            public const string GetScopes = "GetScopes";
            public const string GetScope = "GetScope";
            public const string CreateScope = "CreateScope";
            public const string DeleteScope = "DeleteScope";
            public const string UpdateScopeProperty = "UpdateScopeProperty";
            public const string AddScopeClaim = "AddScopeClaim";
            public const string RemoveScopeClaim = "RemoveScopeClaim";
            public const string UpdateScopeClaim = "UpdateScopeClaim";
            public const string AddScopeSecret = "AddScopeSecret";
            public const string RemoveScopeSecret = "RemoveScopeSecret";
            public const string UpdateScopeSecret = "UpdateScopeSecret";

            public const string GetIdentityResources = "GetIdentityResources";
            public const string GetIdentityResource = "GetIdentityResource";
            public const string CreateIdentityResource = "CreateIdentityResource";
            public const string DeleteIdentityResource = "DeleteIdentityResource";
            public const string UpdateIdentityResourceProperty = "UpdateIdentityResourceProperty";
            public const string RemoveIdentityResourceClaim = "RemoveIdentityResourceClaim";
            public const string AddIdentityResourceClaim = "AddIdentityResourceClaim";

            public const string GetApiResources = "GetApiResources";
            public const string GetApiResource = "GetApiResource";
            public const string CreateApiResource = "CreateApiResource";
            public const string DeleteApiResource = "DeleteApiResource";
            public const string UpdateApiResourceProperty = "UpdateApiResourceProperty";
            public const string AddApiResourceClaim = "AddApiResourceClaim";
            public const string RemoveApiResourceClaim = "RemoveApiResourceClaim";
            public const string UpdateApiResourceSecret = "UpdateApiResourceSecret";
            public const string AddApiResourceSecret = "AddApiResourceSecret";
            public const string RemoveApiResourceSecret = "RemoveApiResourceSecret";

            public const string UpdateApiResourceScope = "UpdateApiResourceScope";
            public const string AddApiResourceScope = "AddApiResourceScope";
            public const string RemoveApiResourceScope = "RemoveApiResourceScope";
            public const string AddApiResourceScopeClaim = "AddApiResourceScopeClaim";
            public const string RemoveApiResourceScopeClaim = "RemoveApiResourceScopeClaim";

            public const string Home = "Home";
            public const string Logout = "Logout";
            public const string OAuthFrameCallback = "FrameCallback";
        }
    }
}
