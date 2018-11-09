function extractField(text) {
    var re = /field\s+=\s+'([A-Za-z0-9 ]*)'/;
    var remainingText = text.replace(re, "");
    var matches = text.match(re);

    if (matches) {
        return [remainingText, matches[1]];
    }

    return [remainingText, ""];
}

application.filter("searchFormulae", function () {
    return function (formulae, text, pageNumber, numberOfFormulaePerPage) {
        if (formulae == undefined) {
            return [];
        }

        if (pageNumber == null) {
            pageNumber = 1;
        }

        if (numberOfFormulaePerPage == null) {
            numberOfFormulaePerPage = 10;
        }

        var p = (pageNumber - 1) * numberOfFormulaePerPage;
        var q = (pageNumber) * numberOfFormulaePerPage;

        if (stringIsNullOrEmpty(text)) {
            return formulae.slice(p, q);
        }
        else {
            var a = extractTags(text);
            text = a[0];
            var tags = a[1];

            console.log(tags);

            var b = extractField(text);
            text = b[0];
            var field = b[1];

            var matchingFormulae = [];

            for (var i = 0; i < formulae.length; i++) {
                var formula = formulae[i];
                var formulaText = formula.Title + ", " + formula.Interpretation + ", " + formula.Content;
                var tagsText = makeSearchableString(formula.Tags);

                if (text != "") {
                    if (stringContains(formulaText.toLowerCase(), text.toLowerCase())) {
                        matchingFormulae.push(formula);
                        continue;
                    }
                }

                for (var j = 0; j < tags.length; j++) {
                    if (stringContains(tagsText, tags[j])) {
                        matchingFormulae.push(formula);
                    }
                }

                for (var k = 0; k < formula.Fields.length; k++) {
                    if (formula.Fields[k] == field) {
                        matchingFormulae.push(formula);
                    }
                }
            }

            return matchingFormulae.slice(p, q);
        }
    }
});

application.filter("searchConstants", function () {
    return function (constants, text, pageNumber, numberOfConstantsPerPage) {
        if (constants == undefined) {
            return [];
        }

        if (pageNumber == null) {
            pageNumber = 1;
        }

        if (numberOfConstantsPerPage == null) {
            numberOfConstantsPerPage = 10;
        }

        var p = (pageNumber - 1) * numberOfConstantsPerPage;
        var q = (pageNumber) * numberOfConstantsPerPage;

        if (stringIsNullOrEmpty(text)) {
            return constants.slice(p, q);
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

            return matchingConstants.slice(p, q);
        }
    }
});

var defaultTitle = "Physics Formulae - Look up equations and constants that are commonly used in physics";
var defaultDescription = "Physics Formulae is a website where you can look up the equations and constants that are commonly used in physics. You can also copy the LaTeX for each formula to use in academic papers.";
var defaultKeywords = "physics, science, maths, equations, equation, formula, formulae, formulary, constants, LaTeX, BibTeX, academia, academic, papers, reports, journals, theses, dissertations";

application.controller("SearchController", ["$scope", "$rootScope", "$routeParams", "dataService", "metaService", "$filter", function SearchController($scope, $rootScope, $routeParams, dataService, metaService, $filter) {

    $scope.pageNumber = 1;
    $scope.numberOfFormulaePerPage = 10;
    $scope.pages = [];

    if (!stringIsNullOrEmpty($routeParams.fieldName)) {
        $scope.searchTerms = "field = '" + $routeParams.fieldName + "'";
    }

    if (!stringIsNullOrEmpty($routeParams.tagName)) {
        $scope.searchTerms = "#" + $routeParams.tagName;
    }

    $rootScope.metaService = metaService;

    dataService.getData().then(function (data) {
        $scope.formulae = data.Formulae;
        $scope.constants = data.Constants;

        if (!stringIsNullOrEmpty($routeParams.fieldName)) {
            $rootScope.metaService.set($routeParams.fieldName + " - Physics Formulae", defaultDescription, defaultKeywords);
        }
        else if (!stringIsNullOrEmpty($routeParams.tagName)) {
            $rootScope.metaService.set("'" + $routeParams.tagName + "' - " + defaultTitle, defaultDescription, defaultKeywords);
        }
        else {
            $rootScope.metaService.set(defaultTitle, defaultDescription, defaultKeywords);
        }

        var numberOfPages = Math.ceil($scope.formulae.length / $scope.numberOfFormulaePerPage);

        for (var i = 0; i < numberOfPages; i++) {
            $scope.pages.push({ "pageNumber": i + 1 });
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

    $scope.setPageNumber = function (n) {
        $scope.pageNumber = n;
    }


    $scope.$watch("searchTerms", function (newValue, oldValue) {
        if (newValue != "") {
            var searchResults = $filter("searchFormulae")($scope.formulae, newValue, $scope.pageNumber, $scope.numberOfFormulaePerPage);
        }
        else {
            var searchResults = $scope.formulae;
        }

        var numberOfPages = Math.ceil(searchResults.length / $scope.numberOfFormulaePerPage);
        $scope.pages = [];

        for (var i = 0; i < numberOfPages; i++) {
            $scope.pages.push({ "pageNumber": i + 1 });
        }
    });
}]);