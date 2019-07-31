
application.controller("TagsController", ["$scope", "$routeParams", "dataService", "$rootScope", "metaService", function TagsController($scope, $routeParams, dataService, $rootScope, metaService) {

    $rootScope.metaService = metaService;

    dataService.getData().then(function (data) {
        var database = new Database(data);

        $scope.tags = database.getAllTags();

        $rootScope.metaService.set("All Tags - Physics Formulae", "", "", "", "");
    });

}]);