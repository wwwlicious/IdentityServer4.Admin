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
            })
            .when("/identityresources/create", {
                controller: 'NewIdentityResourceCtrl',
                resolve: {
                    api: function (idAdmApi) {
                        return idAdmApi.get();
                    }
                },
                templateUrl: PathBase + '/assets/Templates.identityresources.new.html'
            })
            .when("/identityresources/edit/:subject", {
                controller: 'EditIdentityResourceCtrl',
                resolve: { identityResources: "idAdmIdentityResources" },
                templateUrl: PathBase + '/assets/Templates.identityresources.edit.html'
            });
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

    function NewIdentityResourceCtrl($scope, idAdmIdentityResources, api, ttFeedback) {
        var feedback = new ttFeedback();
        $scope.feedback = feedback;
        if (!api.links.createIdentityResource) {
            feedback.errors = "Create Not Supported";
            return;
        }
        else {
            var properties = api.links.createIdentityResource.meta
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
                idAdmIdentityResources.createIdentityResource(props)
                    .then(function (result) {
                        $scope.last = result;
                        feedback.message = "Create Success";
                    }, feedback.errorHandler);
            };
        }
    }
    NewIdentityResourceCtrl.$inject = ["$scope", "idAdmIdentityResources", "api", "ttFeedback"];
    app.controller("NewIdentityResourceCtrl", NewIdentityResourceCtrl);

    function EditIdentityResourceCtrl($scope, idAdmIdentityResources, $routeParams, ttFeedback, $location) {
        var feedback = new ttFeedback();
        $scope.feedback = feedback;

        function loadIdentityResource() {
            return idAdmIdentityResources.getIdentityResource($routeParams.subject)
                .then(function(result) {
                    $scope.identityResource = result;
                    if (!result.data.properties) {
                        $scope.tab = 1;
                    }
                }, feedback.errorHandler);
        };
        loadIdentityResource();

        $scope.setProperty = function (property) {
            idAdmIdentityResources.setProperty(property)
                .then(function () {
                    if (property.meta.dataType !== 1) {
                        feedback.message = property.meta.name + " Changed to: " + property.data;
                    }
                    else {
                        feedback.message = property.meta.name + " Changed";
                    }
                    loadIdentityResource();
                }, feedback.errorHandler);
        };

        $scope.deleteIdentityResource = function(identityResource) {
            idAdmIdentityResources.deleteIdentityResource(identityResource)
                .then(function() {
                    feedback.message = "Identity Resource Deleted";
                    $scope.identityResource = null;
                    $location.path('/identityresources/list');
                }, feedback.errorHandler);
        };

        $scope.addIdentityResourceClaim = function(claims, claim) {
            idAdmIdentityResources.addClaim(claims, claim)
                .then(function() {
                    feedback.message = "Identity Resource Claim Added : " + claim.type;
                    loadIdentityResource().then(function() {
                        $scope.claim = claim.data;
                    });
                }, feedback.errorHandler);
        };

        $scope.removeIdentityResourceClaim = function(claim) {
            idAdmIdentityResources.removeClaim(claim)
                .then(function() {
                    feedback.message = "Identity Resource Claim Removed : " + claim.data.type;
                    loadIdentityResource().then(function () {
                        $scope.claim = claim.data;
                    });
                }, feedback.errorHandler);
        };
    }
    EditIdentityResourceCtrl.$inject = ["$scope", "idAdmIdentityResources", "$routeParams", "ttFeedback", "$location"];
    app.controller("EditIdentityResourceCtrl", EditIdentityResourceCtrl);

})(angular);