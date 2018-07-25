function extractField(text) {
    var re = /field\s+=\s+'([A-Za-z0-9 ]*)'/;
    var matches = text.match(re);

    if (matches) {
        var remainingText = text.replace(re, "");

        return [remainingText, matches[1]];
    }

    return [remainingText, ""];
}

application.filter("searchFormulae", function () {
    return function (formulae, text) {
        if (stringIsNullOrEmpty(text)) {
            return formulae;
        }
        else {
            var a = extractTags(text);
            text = a[0];
            var tags = a[1];

            var b = extractField(text);
            text = b[0];
            var field = b[1];

            var matchingFormulae = [];

            for (var i = 0; i < formulae.length; i++) {
                var formula = formulae[i];
                var formulaText = formula.Title + ", " + formula.Interpretation + ", " + formula.Content + ", " + makeSearchableString(formula.Fields);
                var tagsText = makeSearchableString(formula.Tags);

                if (text != "") {
                    if (stringContains(formulaText.toLowerCase(), text.toLowerCase())) {
                        matchingFormulae.push(formula);
                        continue;
                    }
                }

                for (var j = 0; j < tags.length; j++) {
                    if (stringContains(tagsText.toLowerCase(), tags[j].toLowerCase())) {
                        matchingFormulae.push(formula);
                    }
                }

                for (var k = 0; k < formula.Fields.length; k++) {
                    if (formula.Fields[k] == field) {
                        matchingFormulae.push(formula);
                    }
                }
            }

            return matchingFormulae;
        }
    }
});

application.filter("searchConstants", function () {
    return function (constants, text) {
        if (stringIsNullOrEmpty(text)) {
            return constants;
        }
        else {
            var a = extractTags(text);
            text = a[0];
            var tags = a[1];

            var matchingConstants = [];

            for (var i = 0; i < constants.length; i++) {
                var constant = constants[i];
                var constantText = constant.Title + ", " + constant.Interpretation + ", " + constant.Symbol;
                var tagsText = makeSearchableString(constant.Tags);

                if (text != "") {
                    if (stringContains(constantText.toLowerCase(), text.toLowerCase())) {
                        matchingConstants.push(constant);
                        continue;
                    }
                }

                for (var j = 0; j < tags.length; j++) {
                    if (stringContains(tagsText.toLowerCase(), tags[j].toLowerCase())) {
                        matchingConstants.push(constant);
                    }
                }
            }

            return matchingConstants;
        }
    }
});

var defaultTitle = "Physics Formulae - Look up equations and constants that are commonly used in physics";
var defaultDescription = "Physics Formulae is a website where you can look up the equations and constants that are commonly used in physics. You can also copy the LaTeX for each formula to use in academic papers.";
var defaultKeywords = "physics, science, maths, equations, equation, formula, formulae, formulary, constants, LaTeX, BibTeX, academia, academic, papers, reports, journals, theses, dissertations";

application.controller("SearchController", ["$scope", "$rootScope", "$routeParams", "dataService", "metaService", function SearchController($scope, $rootScope, $routeParams, dataService, metaService) {

    $scope.pageNumber = 1;
    $scope.numberOfFormulaePerPage = 10;

    if (!stringIsNullOrEmpty($routeParams.fieldName)) {
        $scope.searchTerms = "field = '" + $routeParams.fieldName + "'";
    }

    $rootScope.metaService = metaService;

    dataService.getData().then(function (data) {
        $scope.formulae = data.Formulae;
        $scope.constants = data.Constants;

        if (!stringIsNullOrEmpty($routeParams.fieldName)) {
            $rootScope.metaService.set($routeParams.fieldName + " - Physics Formulae", defaultDescription, defaultKeywords);
        }
        else {
            $rootScope.metaService.set(defaultTitle, defaultDescription, defaultKeywords);
        }
    });

    $scope.replaceMathematicsMarkers = function (text) {
        if (!text) {
            return "";
        }

        var re = /\$(.+?)\$/gi;
        var textWithKaTeX = text.replace(re, "<katex latex=\"$1\"></katex>")

        return textWithKaTeX;
    }

}]);