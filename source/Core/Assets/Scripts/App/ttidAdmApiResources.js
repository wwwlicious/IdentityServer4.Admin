/// <reference path="../Libs/angular.min.js" />
/// <reference path="../Libs/angular-route.min.js" />

(function (angular) {

    var app = angular.module("ttidAdmApiResources", ['ngRoute', 'ttidAdm', 'ttidAdmUI', 'ui.bootstrap']);
    function config($routeProvider, PathBase) {
        $routeProvider
            .when("/apiresources/list/:filter?/:page?", {
                controller: 'ListApiResourcesCtrl',
                resolve: { apiResources: "idAdmApiResources" },
                templateUrl: PathBase + '/assets/Templates.apiresources.list.html'
            })
            .when("/apiresources/create", {
                controller: 'NewApiResourceCtrl',
                resolve: {
                    api: function (idAdmApi) {
                        return idAdmApi.get();
                    }
                },
                templateUrl: PathBase + '/assets/Templates.apiresources.new.html'
            })
            .when("/apiresources/edit/:subject", {
                controller: 'EditApiResourceCtrl',
                resolve: { apiResources: "idAdmApiResources" },
                templateUrl: PathBase + '/assets/Templates.apiresources.edit.html'
            });
    }
    config.$inject = ["$routeProvider", "PathBase"];
    app.config(config);

    function ListApiResourcesCtrl($scope, idAdmApiResources, idAdmPager, $routeParams, $location) {
        var model = {
            message: null,
            apiResources: null,
            pager: null,
            waiting: true,
            filter: $routeParams.filter,
            page: $routeParams.page || 1
        };
        $scope.model = model;

        $scope.search = function (filter) {
            var url = "/apiresources/list";
            if (filter) {
                url += "/" + filter;
            }
            $location.url(url);
        };

        var itemsPerPage = 10;
        var startItem = (model.page - 1) * itemsPerPage;

        idAdmApiResources.getApiResources(model.filter, startItem, itemsPerPage).then(function (result) {
            $scope.model.waiting = false;

            $scope.model.apiResources = result.data.items;            
            if (result.data.items && result.data.items.length) {
                    $scope.model.pager = new idAdmPager(result.data, itemsPerPage);
            }
        }, function (error) {
                $scope.model.message = error;
                $scope.model.waiting = false;
        });
    }
    ListApiResourcesCtrl.$inject = ["$scope", "idAdmApiResources", "idAdmPager", "$routeParams", "$location"];
    app.controller("ListApiResourcesCtrl", ListApiResourcesCtrl);

    function NewApiResourceCtrl($scope, idAdmApiResources, api, ttFeedback) {
        var feedback = new ttFeedback();
        $scope.feedback = feedback;
        if (!api.links.createApiResource) {
            feedback.errors = "Create Not Supported";
            return;
        }
        else {
            var properties = api.links.createApiResource.meta
                .map(function (item) {
                    return {
                        meta: item,
                        data: item.dataType === 5 ? false : undefined
                    };
                });
            $scope.properties = properties;
            $scope.create = function (properties) {
                var props = properties.map(function (item) {
                    return {
                        type: item.meta.type,
                        value: item.data
                    };
                });
                idAdmApiResources.createApiResource(props)
                    .then(function (result) {
                        $scope.last = result;
                        feedback.message = "Create Success";
                    }, feedback.errorHandler);
            };
        }
    }
    NewApiResourceCtrl.$inject = ["$scope", "idAdmApiResources", "api", "ttFeedback"];
    app.controller("NewApiResourceCtrl", NewApiResourceCtrl);

    function EditApiResourceCtrl($scope, idAdmApiResources, $routeParams, ttFeedback, $location) {
        var feedback = new ttFeedback();
        $scope.feedback = feedback;

        function loadApiResource() {
            return idAdmApiResources.getApiResource($routeParams.subject)
                .then(function(result) {
                    $scope.apiResource = result;
                    if (!result.data.properties) {
                        $scope.tab = 1;
                    }
                }, feedback.errorHandler);
        }
        loadApiResource();

        $scope.setProperty = function (property) {
            idAdmApiResources.setProperty(property)
                .then(function () {
                    if (property.meta.dataType !== 1) {
                        feedback.message = property.meta.name + " Changed to: " + property.data;
                    }
                    else {
                        feedback.message = property.meta.name + " Changed";
                    }
                    loadApiResource();
                }, feedback.errorHandler);
            };

        $scope.deleteApiResource = function(apiResource) {
            idAdmApiResources.deleteApiResource(apiResource)
                .then(function() {
                    feedback.message = "Api Resource Deleted";
                    $scope.apiResource = null;
                    $location.path('/apiresources/list');
                }, feedback.errorHandler);
        };

        //Claims
        $scope.addApiResourceClaim = function(claims, claim) {
            idAdmApiResources.addClaim(claims, claim)
                .then(function () {
                    feedback.message = "Api Resource Claim Added : " + claim.type;
                    loadApiResource().then(function () {
                        $scope.claim = claim.data;
                    });
                }, feedback.errorHandler);
        };

        $scope.removeApiResourceClaim = function(claim) {
            idAdmApiResources.removeClaim(claim)
                .then(function () {
                    feedback.message = "Api Resource Claim Removed : " + claim.data.type;
                    loadApiResource().then(function () {
                        $scope.claim = claim.data;
                    });
                }, feedback.errorHandler);
        };

        $scope.availableHashes = {
            chosenHash: "SHA-512",
            choices: [
            {
                id: "SHA-256",
                text: "SHA-256",
                isDefault: "false"
            }, {
                id: "SHA-512",
                text: "SHA-512",
                isDefault: "true"
            }
            ]
        };
        function calculateSecretHash(clientSecret) {
            var hashObj = new jsSHA(
				$scope.availableHashes.chosenHash,
				"TEXT",
				{ numRounds: parseInt(1, 10) }
			);
            hashObj.update(clientSecret.value);
            clientSecret.value = hashObj.getHash("B64");
        }

        //Datepicker
        $scope.calendar = {
            isopen: {},
            dateFormat: "yyyy/MM/dd hh:MM",
            dateOptions: {},
            open: function ($event, index) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.calendar.isopen[index] = true;
            }
        };
        $scope.dateSelected = function (secret) {
            var value = $("[data-dateid='" + secret.data.id + "']").val();
            secret.data.expiration = value;
        };
        //Secrets
        $scope.addApiResourceSecret = function (secrets, secret) {
            calculateSecretHash(secret);
            idAdmApiResources.addSecret(secrets, secret)
                .then(function () {
                    feedback.message = "Api Resource Secret Added : " + secret.type;
                    loadApiResource().then(function() {
                        $scope.secret = secret.data;
                    });
                }, feedback.errorHandler);
        };
        $scope.updateApiResourceSecret = function (secret) {
            idAdmApiResources.updateSecret(secret)
                .then(function() {
                    feedback.message = "Api Resource Secret Updated : " + secret.data.type;
                    loadApiResource().then(function() {
                        $scope.secret = secret.data;
                    });
                }, feedback.errorHandler);
        };

        $scope.removeApiResourceSecret = function (secret) {
            idAdmApiResources.removeSecret(secret)
                .then(function () {
                    feedback.message = "Api Resource Secret Removed : " + secret.data.type;
                    loadApiResource().then(function () {
                        $scope.secret = secret.data;
                    });
                }, feedback.errorHandler);
        };

        // Scopes
        $scope.addApiResourceScope = function(scopes, scope) {
            idAdmApiResources.addScope(scopes, scope)
                .then(function() {
                    feedback.message = "Api Resource Scope Added : " + scope.name;
                    loadApiResource().then(function() {
                        $scope.scope = scope.data;
                    });
                }, feedback.errorHandler);
        }

        $scope.updateApiResourceScope = function (scope) {
            idAdmApiResources.updateScope(scope)
                .then(function () {
                    feedback.message = "Api Resource Scope Updated : " + scope.data.name;
                    loadApiResource().then(function () {
                        $scope.scope = scope.data;
                    });
                }, feedback.errorHandler);
        };

        $scope.removeApiResourceScope = function (scope) {
            idAdmApiResources.removeSecret(scope)
                .then(function () {
                    feedback.message = "Api Resource Scope Removed : " + scope.data.name;
                    loadApiResource().then(function () {
                        $scope.scope = scope.data;
                    });
                }, feedback.errorHandler);
        };

        // Scope Claims
        $scope.addApiResourceScopeClaim = function(scope, claim) {
            idAdmApiResources.addScopeClaim(scope, claim)
                .then(function () {
                    feedback.message = "Api Resource Scope : " + scope.data.name + ", added claim : " + claim.type;
                    loadApiResource().then(function () {
                        $scope.scope = scope.data;
                    });
                }, feedback.errorHandler);
        };

        $scope.removeApiResourceScopeClaim = function(scope, claim) {
            idAdmApiResources.removeScopeClaim(claim)
                .then(function() {
                    feedback.message = "Api Resource Scope : " + scope.data.name + ", removed claim : " + claim.data.type;
                    loadApiResource().then(function () {
                        $scope.scope = scope.data;
                    });
                }, feedback.errorHandler);
        };
    }
    EditApiResourceCtrl.$inject = ["$scope", "idAdmApiResources", "$routeParams", "ttFeedback", "$location"];
    app.controller("EditApiResourceCtrl", EditApiResourceCtrl);

})(angular);