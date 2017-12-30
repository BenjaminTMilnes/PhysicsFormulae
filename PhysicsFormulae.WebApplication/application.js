var application = angular.module("PhysicsFormulae", ["ngRoute", "ngSanitize"]);


function makeSearchableString(array) {
    return array.join(", ");
}

function stringContains(string1, string2) {
    return (string1.indexOf(string2) >= 0);
}

function stringIsNullOrEmpty(string) {
    return (!string || /^\s*$/.test(string));
}

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

application.filter("searchFormulae", function () {
    return function (formulae, text) {
        if (stringIsNullOrEmpty(text)) {
            return formulae;
        }
        else {
            var matchingFormulae = [];

            for (var i = 0; i < formulae.length; i++) {
                var formula = formulae[i];
                var formulaText = formula.Title + ", " + formula.Interpretation + ", " + formula.Content + ", " + makeSearchableString(formula.Fields) + ", " + makeSearchableString(formula.Tags);

                if (stringContains(formulaText.toLowerCase(), text.toLowerCase())) {
                    matchingFormulae.push(formula);
                }
            }

            return matchingFormulae;
        }
    }
});

application.factory("formulae", ["$http", function ($http) {
    var formulae = { data: [] };

    $http.get("formulae.json").then(function (response) {
        formulae.data = response.data;
    });

    return formulae;
}]);

application.controller("SearchController", ["$scope", "formulae", function SearchController($scope, formulae) {

    $scope.formulae = formulae;

}]);

application.controller("FormulaController", ["$scope", "$routeParams", "formulae", function FormulaController($scope, $routeParams, formulae) {

    $scope.formulae = formulae;

    $scope.$watch("formulae.data", function () {
        $scope.showFormula();
    });
    
    $scope.showFormula = function () {
        for (var i = 0; i < $scope.formulae.data.length; i++) {
            if ($scope.formulae.data[i].Reference == $routeParams.reference) {
                $scope.formula = $scope.formulae.data[i];
            }
        }
    }
    
    $scope.getFormulaContent = function () {
        return "<katex latex=\"\\displaystyle " + $scope.formula.Content + "\"></katex>";
    }

    $scope.replaceMathematicsMarkers = function (text) {

        var re = /\$(.+?)\$/gi;
        var textWithKaTeX = text.replace(re, "<katex latex=\"$1\"></katex>")

        return textWithKaTeX;

    }

    $scope.getLaTeXForEntireFormula = function () {

        var latex = "";

        latex += "\\begin{align*}\n";
        latex += $scope.formula.Content + "\n";
        latex += "\\end{align*}\n\n";
        latex += "where ";

        for (var i = 0; i < $scope.formula.Identifiers.length; i++) {
            if (i > 0 && $scope.formula.Identifiers.length > 2) {
                latex += ", ";
            }
            else if ($scope.formula.Identifiers.length <= 2) {
                latex += " ";
            }
            if (i == $scope.formula.Identifiers.length - 1) {
                latex += "and ";
            }

            var identifier = $scope.formula.Identifiers[i];

            latex += "$" + identifier.Content + "$ ";
            latex += identifier.Definition;
        }

        latex += ".";

        return latex;

    }

    $scope.getAuthorsString = function (authors) {

        var authorsString = "";

        for (var i = 0; i < authors.length; i++) {
            if (i > 0 && authors.length > 2) {
                authorsString += ", ";
            }
            else if (authors.length <= 2) {
                authorsString += " ";
            }
            if (i == authors.length - 1) {
                authorsString += "and ";
            }

            authorsString += authors[i];
        }

        return authorsString;

    }

    new Clipboard(".copybutton");

}]);