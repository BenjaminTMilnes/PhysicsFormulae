
application.controller("FormulaOfTheDayController", ["$scope", "dataService", "$rootScope", "metaService", function FormulaOfTheDayController($scope, dataService, $rootScope, metaService) {

    $rootScope.metaService = metaService;

    dataService.getData().then(function (data) {
        var database = new Database(data);

        var now = new Date();
        var startOfTheYear = new Date(now.getFullYear(), 0, 0);
        var delta = now - startOfTheYear;
        var dayOfYear = Math.floor(delta / (24 * 60 * 60 * 1000));
        var i = dayOfYear % database.formulae.length;
        
        $scope.formula = database.formulae[i];

        $rootScope.metaService.set("Formula of the Day - Physics Formulae", $scope.formula.Title + " - " + $scope.formula.Interpretation, $scope.formula.Tags.join(", "));
    });

    $scope.getFormulaContent = () => { return (!$scope.formula) ? "" : createMathematicsTag($scope.formula.Content, true); }
    $scope.getVariant = (content) => { return createMathematicsTag(content, true); }
    $scope.replaceMathematicsMarkers = replaceMathematicsMarkers;

}]);