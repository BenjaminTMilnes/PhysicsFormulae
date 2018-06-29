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
        .when("/tag/:tagName", { templateUrl: "search.html", controller: "SearchController" })
        .when("/field/:fieldName", { templateUrl: "search.html", controller: "SearchController" })
        .when("/formula/:reference", { templateUrl: "formula.html", controller: "FormulaController" })
        .when("/constant/:reference", { templateUrl: "constant.html", controller: "ConstantController" });
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

function extractTags(text) {
    var re = /#[A-Za-z0-9]+/g;
    var tags = [];

    var m;
    while ((m = re.exec(text)) !== null) {
        tags.push(m[0]);
    }

    text = text.replace(re, " ");

    return [text, tags];
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
                    if (tagsText.toLowerCase() == tags[j].toLowerCase()) {
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
                    if (tagsText.toLowerCase() == tags[j].toLowerCase()) {
                        matchingConstants.push(constant);
                    }
                }
            }

            return matchingConstants;
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

application.controller("SearchController", ["$scope", "$routeParams", "dataService", function SearchController($scope, $routeParams, dataService) {

    $scope.pageNumber = 1;
    $scope.numberOfFormulaePerPage = 10;

    dataService.getData().then(function (data) {
        $scope.formulae = data.Formulae;
        $scope.constants = data.Constants;
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

function convertLaTeXToHTML(latex) {
    latex = latex.replace(/\^\{(\-?[0-9]+)\}/g, "<sup>$1</sup>");
    latex = latex.replace(/\^(\-?[0-9]+)/g, "<sup>$1</sup>");
    latex = latex.replace(/\_\{(\-?[0-9]+)\}/g, "<sub>$1</sub>");
    latex = latex.replace(/\_(\-?[0-9]+)/g, "<sub>$1</sub>");

    return latex;
}

function changeHyphensToMinusSigns(html) {
    return html.replace(/\-/g, "&minus;");
}

application.controller("ConstantController", ["$scope", "$routeParams", "dataService", function ConstantController($scope, $routeParams, dataService) {

    $scope.constants = null;

    dataService.getData().then(function (data) {
        $scope.constants = data.Constants;

        for (var i = 0; i < $scope.constants.length; i++) {
            var constant = $scope.constants[i];

            if (constant.Reference == $routeParams.reference) {
                $scope.constant = constant;

                $scope.getValues();
            }
        }
    });

    $scope.getSymbolContent = function () {
        if (!$scope.constant) {
            return "";
        }

        return "<katex latex=\"\\displaystyle " + $scope.constant.Symbol + "\"></katex>";
    }

    $scope.getValues = function () {
        if (!$scope.constant) {
            return [];
        }

        var constant = $scope.constant;
        var listedValues = [];

        for (var i = 0; i < constant.Values.length; i++) {
            var value = constant.Values[i];

            var number = value.Coefficient + " &times; 10<sup>" + changeHyphensToMinusSigns(value.Exponent) + "</sup>";
            var units = value.Units;
            var latex = value.Coefficient + " \\times 10^{" + value.Exponent + "} \\,\\mathrm{" + value.Units + "}";

            listedValues.push({ "type": "Precise Value", "number": number, "units": units, "latex": latex });

            var numberTo3SF = Number.parseFloat(value.Coefficient).toPrecision(3) + " &times; 10<sup>" + changeHyphensToMinusSigns(value.Exponent) + "</sup>";
            var latexTo3SF = Number.parseFloat(value.Coefficient).toPrecision(3) + " \\times 10^{" + value.Exponent + "} \\,\\mathrm{" + value.Units + "}";

            listedValues.push({ "type": "To 3 s.f.", "number": numberTo3SF, "units": units, "latex": latexTo3SF });
        }

        $scope.listedValues = listedValues;
    }

    $scope.getBibTeXForThisWebpage = function () {
        if (!$scope.constant) {
            return "";
        }

        var bibtex = "";
        var database = new BibTeXDatabase();
        var misc = new BibTeXMiscellaneous();

        misc.citationKey = "PhysicsFormulae_" + $scope.constant.Reference;
        misc.title.value = $scope.constant.Title;
        misc.howPublished.value = "\\url{" + "http://www.physicsformulae.com/#/constant/" + $scope.constant.Reference + "}"
        misc.note.value = $scope.constant.Title + " (Physics Formulae), edited by B. T. Milnes, accessed on " + $scope.getTodaysDate();

        database.entries.push(misc);

        var exporter = new BibTeXExporter();

        bibtex = exporter.convertBibTeXDatabaseToText(database).trim();

        return bibtex;
    }

    $scope.getBibLaTeXForThisWebpage = function () {
        if (!$scope.constant) {
            return "";
        }

        var biblatex = "";
        var database = new BibTeXDatabase();
        var online = new BibLaTeXOnline();

        online.citationKey = "PhysicsFormulae_" + $scope.constant.Reference;
        online.title.value = $scope.constant.Title + " (Physics Formulae)";
        online.author.value = "B. T. Milnes";
        online.url.value = "http://www.physicsformulae.com/#/constant/" + $scope.constant.Reference;
        online.urlDate.value = $scope.getTodaysDate();

        database.entries.push(online);

        var exporter = new BibTeXExporter();

        biblatex = exporter.convertBibTeXDatabaseToText(database).trim();

        return biblatex;
    }

    $scope.getTodaysDate = getTodaysDate;
    $scope.getAuthorsString = getAuthorsString;

    $scope.convertLaTeXToHTML = convertLaTeXToHTML;
    $scope.changeHyphensToMinusSigns = changeHyphensToMinusSigns;

    new ClipboardJS(".copybutton");

}]);


application.controller("FormulaController", ["$scope", "$routeParams", "dataService", function FormulaController($scope, $routeParams, dataService) {

    $scope.formulae = null;

    dataService.getData().then(function (data) {
        $scope.formulae = data.Formulae;

        for (var i = 0; i < $scope.formulae.length; i++) {
            var formula = $scope.formulae[i];

            if (formula.Reference == $routeParams.reference) {
                $scope.formula = formula;
            }
        }
    });

    $scope.getNumberOfGoodReferences = function () {
        if (!$scope.formula) {
            return 0;
        }

        var n = 0;

        for (var i = 0; i < $scope.formula.References.length; i++) {
            if ($scope.formula.References[i].Type == "Book") {
                n += 1;
            }
        }

        return n;
    }

    $scope.getFormulaContent = function () {
        if (!$scope.formula) {
            return "";
        }

        return "<katex latex=\"\\displaystyle " + $scope.formula.Content + "\"></katex>";
    }

    $scope.getVariant = function (content) {
        return "<katex latex=\"\\displaystyle " + content + "\"></katex>";
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
        var misc = new BibTeXMiscellaneous();

        misc.citationKey = "PhysicsFormulae_" + $scope.formula.Reference;
        misc.title.value = $scope.formula.Title;
        misc.howPublished.value = "\\url{" + "http://www.physicsformulae.com/#/formula/" + $scope.formula.Reference + "}"
        misc.note.value = $scope.formula.Title + " (Physics Formulae), edited by B. T. Milnes, accessed on " + $scope.getTodaysDate();

        database.entries.push(misc);

        var exporter = new BibTeXExporter();

        bibtex = exporter.convertBibTeXDatabaseToText(database).trim();

        return bibtex;
    }

    $scope.getBibLaTeXForThisWebpage = function () {
        if (!$scope.formula) {
            return "";
        }

        var biblatex = "";
        var database = new BibTeXDatabase();
        var online = new BibLaTeXOnline();

        online.citationKey = "PhysicsFormulae_" + $scope.formula.Reference;
        online.title.value = $scope.formula.Title + " (Physics Formulae)";
        online.author.value = "B. T. Milnes";
        online.url.value = "http://www.physicsformulae.com/#/formula/" + $scope.formula.Reference;
        online.urlDate.value = $scope.getTodaysDate();

        database.entries.push(online);

        var exporter = new BibTeXExporter();

        biblatex = exporter.convertBibTeXDatabaseToText(database).trim();

        return biblatex;
    }

    $scope.getTodaysDate = getTodaysDate;

    $scope.getBibTeXForOriginalReferences = function () {
        if (!$scope.formula) {
            return "";
        }

        return getBibTeXForOriginalReferences($scope.formula.References);
    }

    $scope.getCitationKeyForReference = getCitationKeyForReference;
    $scope.getAuthorsString = getAuthorsString;

    new ClipboardJS(".copybutton");

}]);


function getTodaysDate() {
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


function getBibTeXForOriginalReferences(references) {

    var bibtex = "";

    var database = new BibTeXDatabase();

    for (var i = 0; i < references.length; i++) {
        var reference = references[i];

        if (reference.Type == "Book") {
            var book = new BibTeXBook();
            book.citationKey = getCitationKeyForReference(reference);
            book.title.value = reference.Title;
            book.author.value = getAuthorsString(reference.Authors);
            book.publisher.value = reference.Publisher;

            database.entries.push(book);
        }
    }

    var exporter = new BibTeXExporter();

    bibtex = exporter.convertBibTeXDatabaseToText(database).trim();

    return bibtex;

}

function getCitationKeyForReference(reference) {

    var citationKey = "";

    citationKey = reference.Authors[0];
    citationKey = citationKey.replace(/[\s\.]/g, "");

    return citationKey;
}

function getAuthorsString(authors) {

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