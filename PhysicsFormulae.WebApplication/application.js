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

application.factory("dataService", ["$http", function ($http) {
    var dataService = {
        getData: function () {
            return $http.get("formulae.json").then(function (response) {
                return response.data;
            });
        }
    };

    return dataService;
}]);

application.controller("SearchController", ["$scope", "dataService", function SearchController($scope, dataService) {

    dataService.getData().then(function (data) {
        $scope.formulae = data;
    });

}]);

application.controller("FormulaController", ["$scope", "$routeParams", "dataService", function FormulaController($scope, $routeParams, dataService) {

    $scope.formulae = null;

    dataService.getData().then(function (data) {
        $scope.formulae = data;

        for (var i = 0; i < $scope.formulae.length; i++) {
            var formula = $scope.formulae[i];

            if (formula.Reference == $routeParams.reference) {
                $scope.formula = formula;
            }
        }
    });

    $scope.getFormulaContent = function () {
        if (!$scope.formula) {
            return "";  
        }

        return "<katex latex=\"\\displaystyle " + $scope.formula.Content + "\"></katex>";
    }

    $scope.replaceMathematicsMarkers = function (text) {
        if (!text) {
            return "";
        }

        var re = /\$(.+?)\$/gi;
        var textWithKaTeX = text.replace(re, "<katex latex=\"$1\"></katex>")

        return textWithKaTeX;
    }

    $scope.getLaTeXForEntireFormula = function () {
        if (!$scope.formula) {
            return "";
        }

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

    $scope.getBibTeXForThisWebpage = function () {
        if (!$scope.formula) {
            return "";
        }

        var bibtex = "";
        var database = new BibTeXDatabase();
        var webpage = new BibTeXWebpage();

        webpage.citationKey = "PhysicsFormulae_" + $scope.formula.Reference;
        webpage.author.value = "B. T. Milnes";
        webpage.title.value = $scope.formula.Title;
        webpage.websiteTitle.value = "Physics Formulae";
        webpage.url.value = "http://www.physicsformulae.com/#/formula/" + $scope.formula.Reference;
        webpage.dateAccessed.value = $scope.getTodaysDate();

        database.entries.push(webpage);

        var exporter = new BibTeXExporter();

        bibtex = exporter.convertBibTeXDatabaseToText(database).trim();

        return bibtex;
    }

    $scope.getTodaysDate = function () {
        var today = new Date();

        var day = today.getDate();
        var month = today.getMonth() + 1;
        var year = today.getFullYear();

        if (day < 10) {
            day = "0" + day;
        }

        if (month < 10) {
            month = "0" + month;
        }

        var todaysDate = year + "/" + month + "/" + day;

        return todaysDate;
    }

    $scope.getBibTeXForOriginalReferences = function () {
        if (!$scope.formula) {
            return "";
        }

        var bibtex = "";

        var database = new BibTeXDatabase();

        for (var i = 0; i < $scope.formula.References.length; i++) {
            var reference = $scope.formula.References[i];

            if (reference.Type == "Book") {
                var book = new BibTeXBook();
                book.citationKey = $scope.getCitationKeyForReference(reference);
                book.title.value = reference.Title;
                book.author.value = $scope.getAuthorsString(reference.Authors);
                book.publisher.value = reference.Publisher;

                database.entries.push(book);
            }
        }

        var exporter = new BibTeXExporter();

        bibtex = exporter.convertBibTeXDatabaseToText(database).trim();

        return bibtex;

    }

    $scope.getCitationKeyForReference = function (reference) {

        var citationKey = "";

        citationKey = reference.Authors[0];
        citationKey = citationKey.replace(/[\s\.]/g, "");

        return citationKey;
    }

    $scope.getAuthorsString = function (authors) {

        var authorsString = "";

        for (var i = 0; i < authors.length; i++) {
            if (i > 0 && authors.length > 2) {
                authorsString += ", ";
            }
            else if (i > 0 && authors.length == 2) {
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