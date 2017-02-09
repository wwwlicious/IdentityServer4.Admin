/// <reference path="../Libs/angular.min.js" />
/// <reference path="../Libs/angular-route.min.js" />

(function (angular) {

    var app = angular.module("ttidAdmIdentityResources", ['ngRoute', 'ttidAdm', 'ttidAdmUI', 'ui.bootstrap']);
    function config($routeProvider, PathBase) {
        $routeProvider
            .when("/identityresources/list/:filter?/:page?",
            {
                controller: 'ListIdentityResourcesCtrl',
                resolve: { identityResources: "idAdmIdentityResources" },
                templateUrl: PathBase + '/assets/Templates.identityresources.list.html'
            });
        //.when("/scopes/create", {
        //    controller: 'NewScopeCtrl',
        //    resolve: {
        //        api: function (idAdmApi) {
        //            return idAdmApi.get();
        //        }
        //    },
        //    templateUrl: PathBase + '/assets/Templates.scopes.new.html'
        //})
        //.when("/scopes/edit/:subject", {
        //    controller: 'EditScopeCtrl',
        //    resolve: { scopes: "idAdmScopes" },
        //    templateUrl: PathBase + '/assets/Templates.scopes.edit.html'
        //});
    }
    config.$inject = ["$routeProvider", "PathBase"];
    app.config(config);

    function ListIdentityResourcesCtrl($scope, idAdmIdentityResources, idAdmPager, $routeParams, $location) {
        var model = {
            message: null,
            identityResources: null,
            pager: null,
            waiting: true,
            filter: $routeParams.filter,
            page: $routeParams.page || 1
        };
        $scope.model = model;

        $scope.search = function (filter) {
            var url = "/identityresoures/list";
            if (filter) {
                url += "/" + filter;
            }
            $location.url(url);
        };

        var itemsPerPage = 10;
        var startItem = (model.page - 1) * itemsPerPage;

        idAdmIdentityResources.getIdentityResources(model.filter, startItem, itemsPerPage).then(function(result) {
            $scope.model.waiting = false;

            $scope.model.identityResources = result.data.items;
            if (result.data.items && result.data.items.length) {
                $scope.model.pager = new idAdmPager(result.data, itemsPerPage);
            }
        }, function(error) {
            $scope.model.message = error;
            $scope.model.waiting = false;
        });
    }
    ListIdentityResourcesCtrl.$inject = ["$scope", "idAdmIdentityResources", "idAdmPager", "$routeParams", "$location"];
    app.controller("ListIdentityResourcesCtrl", ListIdentityResourcesCtrl);

    //function NewScopeCtrl($scope, idAdmScopes, api, ttFeedback) {
    //    var feedback = new ttFeedback();
    //    $scope.feedback = feedback;
    //    if (!api.links.createScope) {
    //        feedback.errors = "Create Not Supported";
    //        return;
    //    }
    //    else {
    //        var properties = api.links.createScope.meta
    //            .map(function (item) {
    //                return {
    //                    meta: item,
    //                    data: item.dataType === 5 ? false : undefined
    //                };
    //            });
    //        $scope.properties = properties;
    //        $scope.create = function (properties) {
    //            var props = properties.map(function (item) {
    //                return {
    //                    type: item.meta.type,
    //                    value: item.data
    //                };
    //            });
    //            idAdmScopes.createScope(props)
    //                .then(function (result) {
    //                    $scope.last = result;
    //                    feedback.message = "Create Success";
    //                }, feedback.errorHandler);
    //        };
    //    }
    //}
    //NewScopeCtrl.$inject = ["$scope", "idAdmScopes", "api", "ttFeedback"];
    //app.controller("NewScopeCtrl", NewScopeCtrl);

    //function EditScopeCtrl($scope, idAdmScopes, $routeParams, ttFeedback, $location) {
    //    var feedback = new ttFeedback();
    //    $scope.feedback = feedback;

    //    function loadScope() {
    //        return idAdmScopes.getScope($routeParams.subject)
    //            .then(function (result) {
    //                $scope.scope = result;
    //                if (!result.data.properties) {
    //                    $scope.tab = 1;
    //                }

    //            }, feedback.errorHandler);
    //    };
    //    loadScope();

    //    $scope.setProperty = function (property) {
    //        idAdmScopes.setProperty(property)
    //            .then(function () {
    //                if (property.meta.dataType !== 1) {
    //                    feedback.message = property.meta.name + " Changed to: " + property.data;
    //                }
    //                else {
    //                    feedback.message = property.meta.name + " Changed";
    //                }
    //                loadScope();
    //            }, feedback.errorHandler);
    //    };

    //    $scope.deleteScope = function (scope) {
    //        idAdmScopes.deleteScope(scope)
    //               .then(function () {
    //                   feedback.message = "Scope Deleted";
    //                   $scope.scope = null;
    //                   $location.path('/scopes/list');
    //               }, feedback.errorHandler);
    //    };

    //    //Claims
    //    $scope.addScopeClaim = function (scopeClaims, scopeClaim) {
    //        idAdmScopes.addScopeClaim(scopeClaims, scopeClaim)
    //            .then(function () {
    //                feedback.message = "Scope Claim Added : " + scopeClaim.name + ", " + scopeClaim.description;
    //                loadScope().then(function () {
    //                    $scope.claim = scopeClaim.data;
    //                });
    //                loadScope();
    //            }, feedback.errorHandler);
    //    };
    //    $scope.removeScopeClaim = function (scopeClaim) {
    //        idAdmScopes.removeScopeClaim(scopeClaim)
    //            .then(function () {
    //                feedback.message = "Scope Claim Removed : " + scopeClaim.data.name + ", " + scopeClaim.data.description;
    //                loadScope().then(function () {
    //                    $scope.claim = scopeClaim.data;
    //                });
    //            }, feedback.errorHandler);
    //    };

    //    $scope.availableHashes = {
    //        chosenHash: "SHA-512",
    //        choices: [
    //        {
    //            id: "SHA-256",
    //            text: "SHA-256",
    //            isDefault: "false"
    //        }, {
    //            id: "SHA-512",
    //            text: "SHA-512",
    //            isDefault: "true"
    //        }
    //        ]
    //    };
    //    function calculateScopeScretHash(clientSecret) {
    //        var hashObj = new jsSHA(
	//			$scope.availableHashes.chosenHash,
	//			"TEXT",
	//			{ numRounds: parseInt(1, 10) }
	//		);
    //        hashObj.update(clientSecret.value);
    //        clientSecret.value = hashObj.getHash("B64");
    //    }

    //    //Datepicker

    //    $scope.calendar = {
    //        isopen: {},
    //        dateFormat: "yyyy/MM/dd hh:MM",
    //        dateOptions: {},
    //        open: function ($event, index) {
    //            $event.preventDefault();
    //            $event.stopPropagation();
    //            $scope.calendar.isopen[index] = true;
    //        }
    //    };
    //    $scope.dateSelected = function (secret) {
    //        var value = $("[data-dateid='" + secret.data.id + "']").val();
    //        secret.data.expiration = value;

    //    }
    //    //Secrets
    //    $scope.addScopeSecret = function (scopeSecrets, scopeSecret) {
    //        calculateScopeScretHash(scopeSecret);
    //        idAdmScopes.addScopeSecret(scopeSecrets, scopeSecret)
    //            .then(function () {
    //                feedback.message = "Scope Secret Added : " + scopeSecret.type;
    //                loadScope().then(function () {
    //                    $scope.secret = scopeSecret.data;
    //                });
    //                loadScope();
    //            }, feedback.errorHandler);
    //    };
    //    $scope.updateScopeClaim = function (claim) {
    //        idAdmScopes.updateScopeClaim(claim)
    //                  .then(function () {
    //                      feedback.message = "Scope claim updated : " + claim.data.name;
    //                      loadScope().then(function () {
    //                          $scope.claim = claim.data;
    //                      });
    //                  }, feedback.errorHandler);
    //    }
    //    $scope.updateScopeSecret = function (scopeSecret) {
    //        idAdmScopes.updateScopeSecret(scopeSecret)
    //            .then(function () {
    //                feedback.message = "Scope Secret updated : " + scopeSecret.data.type;
    //                loadScope().then(function () {
    //                    $scope.secret = scopeSecret.data;
    //                });
    //            }, feedback.errorHandler);
    //    };
    //    $scope.removeScopeSecret = function (scopeSecret) {
    //        idAdmScopes.removeScopeSecret(scopeSecret)
    //            .then(function () {
    //                feedback.message = "Scope Secret Removed : " + scopeSecret.data.type;
    //                loadScope().then(function () {
    //                    $scope.secret = scopeSecret.data;
    //                });
    //            }, feedback.errorHandler);
    //    };

    //}
    //EditScopeCtrl.$inject = ["$scope", "idAdmScopes", "$routeParams", "ttFeedback", "$location"];
    //app.controller("EditScopeCtrl", EditScopeCtrl);

})(angular);