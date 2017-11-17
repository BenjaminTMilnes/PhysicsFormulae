var application = angular.module("PhysicsFormulae", []);

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

application.controller("SearchController", ["$scope", "$http", function SearchController($scope, $http) {

    $scope.formulae = [];

    $http.get("formulae.json").then(function (response) {
        $scope.formulae = response.data;
    });

}])