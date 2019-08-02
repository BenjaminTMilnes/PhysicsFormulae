function extractField(text) {
    var re = /field\s+=\s+'([A-Za-z0-9 ]*)'/;
    var remainingText = text.replace(re, "");
    var matches = text.match(re);

    if (matches) {
        return [remainingText, matches[1]];
    }

    return [remainingText, ""];
}

application.filter("paginate", function () {
    return function (a, pageNumber, numberOfItemsPerPage) {
        if (a == null || a == undefined) {
            return [];
        }

        if (pageNumber == null) {
            pageNumber = 1;
        }

        if (numberOfItemsPerPage == null) {
            numberOfItemsPerPage = 10;
        }

        var p = (pageNumber - 1) * numberOfItemsPerPage;
        var q = (pageNumber) * numberOfItemsPerPage;

        return a.slice(p, q);
    }
});

application.filter("searchFormulae", function () {
    return function (formulae, text) {
        if (formulae == null || formulae == undefined) {
            return [];
        }

        if (stringIsNullOrEmpty(text)) {
            return formulae;
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

            formulae.forEach(formula => {

                if (text != "") {
                    if (stringContains(formula.FormulaText, text.toLowerCase())) {
                        matchingFormulae.push(formula);
                    }
                }
                else {
                    tags.forEach(t => {
                        if (stringContains(formula.TagsText, t.toLowerCase())) {
                            matchingFormulae.push(formula);
                        }
                    });

                    if (stringContains(formula.FieldsText, field.toLowerCase())) {
                        matchingFormulae.push(formula);
                    }
                }

            });

            return matchingFormulae;
        }
    }
});

application.filter("searchConstants", function () {
    return function (constants, text) {
        if (constants == null || constants == undefined) {
            return [];
        }

        if (stringIsNullOrEmpty(text)) {
            return constants;
        }
        else {
            var a = extractTags(text);
            text = a[0];
            var tags = a[1];

            var matchingConstants = [];

            constants.forEach(constant => {

                if (text != "") {
                    if (stringContains(constant.ConstantText, text.toLowerCase())) {
                        matchingConstants.push(constant);
                    }
                }
                else {
                    tags.forEach(t => {
                        if (stringContains(constant.TagsText, t.toLowerCase())) {
                            matchingConstants.push(constant);
                        }
                    });
                }

            });

            return matchingConstants;
        }
    }
});

var defaultTitle = "Physics Formulae - Look up equations and constants that are commonly used in physics";
var defaultDescription = "Physics Formulae is a website where you can look up the equations and constants that are commonly used in physics. You can also copy the LaTeX for each formula to use in academic papers.";
var defaultKeywords = "physics, science, maths, equations, equation, formula, formulae, formulary, constants, LaTeX, BibTeX, academia, academic, papers, reports, journals, theses, dissertations";

application.controller("SearchController", ["$scope", "$rootScope", "$routeParams", "dataService", "metaService", "$filter", function SearchController($scope, $rootScope, $routeParams, dataService, metaService, $filter) {

    $scope.getColourOfWord = getColourOfWord;

    $scope.pageNumber = 1;
    $scope.numberOfFormulaePerPage = 10;
    $scope.numberOfPages = 1;
    $scope.pages = [];

    $scope.formulaSearchResults = [];
    $scope.currentPageFormulaSearchResults = [];

    $scope.constantSearchResults = [];
    $scope.currentPageConstantSearchResults = [];

    $scope.searchTerms = "";

    if (!stringIsNullOrEmpty($routeParams.fieldName)) {
        $scope.searchTerms = "field = '" + $routeParams.fieldName + "'";
    }

    if (!stringIsNullOrEmpty($routeParams.tagName)) {
        $scope.searchTerms = "#" + $routeParams.tagName;
    }

    $rootScope.metaService = metaService;

    dataService.getData().then(function (data) {
        $scope.formulae = data.FormulaIndexEntries;
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

        $scope.updateSearchResults($scope.searchTerms);

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
        if (n <= 0 || n > $scope.numberOfPages) {
            return;
        }

        $scope.pageNumber = n;
        $scope.setPageNumberRange();
    }

    $scope.setPageNumberRange = function () {

        if ($scope.formulaSearchResults == null || $scope.formulaSearchResults == undefined) {
            return;
        }

        $scope.currentPageFormulaSearchResults = $filter("paginate")($scope.formulaSearchResults, $scope.pageNumber, $scope.numberOfFormulaePerPage);

        $scope.currentPageConstantSearchResults = $filter("paginate")($scope.constantSearchResults, $scope.pageNumber, $scope.numberOfFormulaePerPage);

        $scope.numberOfPages = Math.ceil($scope.formulaSearchResults.length / $scope.numberOfFormulaePerPage);
        $scope.pages = [];

        var addEllipses = 0;

        for (var i = 0; i < $scope.numberOfPages ; i++) {
            if (i > 0 && i < $scope.numberOfPages - 1 && (i < $scope.pageNumber - 3 || i > $scope.pageNumber + 1)) {
                if (addEllipses == 0) {
                    $scope.pages.push({ "pageNumber": -1, "class": "pagenumber-ellipses" });
                }
                addEllipses = 1;
            }
            else {
                $scope.pages.push({ "pageNumber": i + 1, "class": (i == $scope.pageNumber - 1) ? "pagenumber-selected" : "pagenumber-unselected" });
                addEllipses = 0;
            }
        }

    }

    $scope.updateSearchResults = function (searchTerms) {
        $scope.formulaSearchResults = $filter("orderBy")($filter("searchFormulae")($scope.formulae, searchTerms), "formula.Title");

        $scope.constantSearchResults = $filter("orderBy")($filter("searchConstants")($scope.constants, searchTerms), "constant.Title");

        $scope.setPageNumberRange();
    }

    $scope.$watch("searchTerms", function (newValue, oldValue) {
        $scope.updateSearchResults(newValue);
    });

}]);