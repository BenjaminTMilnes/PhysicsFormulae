
application.controller("FormulaOfTheDayController", ["$scope", "$routeParams", "dataService", "$rootScope", "metaService", function FormulaOfTheDayController($scope, $routeParams, dataService, $rootScope, metaService) {

    $rootScope.metaService = metaService;

    $scope.formulae = null;

    dataService.getData().then(function (data) {
        $scope.formulae = data.Formulae;

        var now = new Date();
        var startOfTheYear = new Date(now.getFullYear(), 0, 0);
        var delta = now - startOfTheYear;
        var dayOfYear = Math.floor(delta / (24 * 60 * 60 * 1000));
        var i = dayOfYear % $scope.formulae.length;

        console.log(i);

        $scope.formula = $scope.formulae[i];

        $rootScope.metaService.set("Formula of the Day - Physics Formulae", $scope.formula.Title + " - " + $scope.formula.Interpretation, $scope.formula.Tags.join(", "));
    });


    $scope.getFormulaContent = function () {
        return (!$scope.formula) ? "" : createMathematicsTag($scope.formula.Content, true);
    }

    $scope.getVariant = function (content) {
        return createMathematicsTag(content, true);
    }

    $scope.replaceMathematicsMarkers = function (text) {
        if (!text) {
            return "";
        }

        var re = /\$(.+?)\$/gi;
        var textWithKaTeX = text.replace(re, "<mathematics content-type=\"latex\" content=\"$1\"></mathematics>")

        return textWithKaTeX;
    }
}]);