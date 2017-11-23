var application = angular.module("PhysicsFormulae", ["ngRoute", "ngSanitize"]);

application.config(function ($routeProvider) {
    $routeProvider
        .when("/", { templateUrl: "search.html", controller: "SearchController" })
        .when("/formula/:reference", { templateUrl: "formula.html", controller: "FormulaController" });
});

application.directive("katex", function () {
    return {
        restrict: "E",
        link: function (scope, element, attributes) {
            var latex = attributes.latex;
            if (typeof (katex) === "undefined") {
                require(["katex"], function (katex) {
                    katex.render(latex, element[0]);
                });
            }
            else {
                katex.render(latex, element[0]);
            }
        }
    }
});


application.directive("compile", ["$compile", function ($compile) {
    return function (scope, element, attributes) {
        scope.$watch(function (scope) {
            return scope.$eval(attributes.compile);
        }, function (value) {
            element.html(value);
            $compile(element.contents())(scope);
        });
    };
}]);

application.controller("SearchController", ["$scope", "$http", function SearchController($scope, $http) {

    $scope.formulae = [];

    $http.get("formulae.json").then(function (response) {
        $scope.formulae = response.data;
    });

}]);


application.controller("FormulaController", ["$scope", "$routeParams", "$http", function FormulaController($scope, $routeParams, $http) {

    $scope.formulae = [];

    $http.get("formulae.json").then(function (response) {
        $scope.formulae = response.data;

        for (var i = 0; i < $scope.formulae.length; i++) {
            if ($scope.formulae[i].Reference == $routeParams.reference) {
                $scope.formula = $scope.formulae[i];
            }
        }
    });

    $scope.getFormulaContent = function () {
        return "<katex latex=\"\\displaystyle " + $scope.formula.Content + "\"></katex>";
    }

}]);