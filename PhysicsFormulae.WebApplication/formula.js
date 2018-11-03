
application.controller("FormulaController", ["$scope", "$routeParams", "dataService", "$rootScope", "metaService", function FormulaController($scope, $routeParams, dataService, $rootScope, metaService) {

    $rootScope.metaService = metaService;

    $scope.formulae = null;
    $scope.constants = null;

    dataService.getData().then(function (data) {
        $scope.formulae = data.Formulae;
        $scope.constants = data.Constants;

        for (var i = 0; i < $scope.formulae.length; i++) {
            var formula = $scope.formulae[i];

            if (formula.URLReference == $routeParams.reference) {
                $scope.formula = formula;

                for (var j = 0; j < $scope.formula.Identifiers.length; j++) {
                    var identifier = $scope.formula.Identifiers[j];
                    var matches = $scope.constants.filter(c => c.Reference == identifier.Reference);

                    if (matches.length > 0) {
                        identifier.Link = "/#/constant/" + matches[0].URLReference;
                    }
                    else {
                        identifier.Link = "";
                    }
                }

                $rootScope.metaService.set(formula.Title + " - Physics Formulae", formula.Interpretation, formula.Tags.join(", "));
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

        return "<mathematics content-type=\"latex\" content=\"\\displaystyle " + $scope.formula.Content + "\"></mathematics>";
    }

    $scope.getVariant = function (content) {
        return "<mathematics content-type=\"latex\" content=\"\\displaystyle " + content + "\"></mathematics>";
    }

    $scope.replaceMathematicsMarkers = function (text) {
        if (!text) {
            return "";
        }

        var re = /\$(.+?)\$/gi;
        var textWithKaTeX = text.replace(re, "<mathematics content-type=\"latex\" content=\"$1\"></mathematics>")

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
        misc.howPublished.value = "\\url{" + "http://www.physicsformulae.com/#/formula/" + $scope.formula.URLReference + "}"
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
        online.url.value = "http://www.physicsformulae.com/#/formula/" + $scope.formula.URLReference;
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