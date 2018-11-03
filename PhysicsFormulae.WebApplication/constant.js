
application.controller("ConstantController", ["$scope", "$routeParams", "dataService", "$rootScope", "metaService", function ConstantController($scope, $routeParams, dataService, $rootScope, metaService) {

    $rootScope.metaService = metaService;

    $scope.constants = null;

    dataService.getData().then(function (data) {
        $scope.constants = data.Constants;
        $scope.formulae = data.Formulae;

        for (var i = 0; i < $scope.constants.length; i++) {
            var constant = $scope.constants[i];

            if (constant.URLReference == $routeParams.reference) {
                $scope.constant = constant;
                $scope.usedInFormulae = [];

                for (var j = 0; j < $scope.constant.UsedInFormulae.length; j++) {
                    $scope.usedInFormulae = $scope.usedInFormulae.concat($scope.formulae.filter(f => f.Reference == $scope.constant.UsedInFormulae[j]));
                }

                $rootScope.metaService.set(constant.Title + " - Physics Formulae", constant.Interpretation, constant.Tags.join(", "));

                $scope.getValues();
            }
        }
    });

    $scope.replaceMathematicsMarkers = function (text) {
        if (!text) {
            return "";
        }

        var re = /\$(.+?)\$/gi;
        var textWithKaTeX = text.replace(re, "<mathematics content-type=\"latex\" content=\"$1\"></mathematics>")

        return textWithKaTeX;
    }

    $scope.getSymbolContent = function () {
        if (!$scope.constant) {
            return "";
        }

        return "<mathematics content-type=\"latex\" content=\"\\displaystyle " + $scope.constant.Symbol + "\"></mathematics>";
    }

    $scope.getUsedInFormula = function (content) {
        return "<mathematics content-type=\"latex\" content=\"\\displaystyle " + content + "\"></mathematics>";
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

            if (value.Exponent == 0) {
                number = value.Coefficient;
            }

            var units = value.Units;
            var latex = value.Coefficient + " \\times 10^{" + value.Exponent + "} \\,\\mathrm{" + value.Units + "}";

            listedValues.push({ "type": "Precise Value", "significand": value.Coefficient, "number": number, "units": units, "latex": latex });

            var numberTo3SF = Number.parseFloat(value.Coefficient).toPrecision(3) + " &times; 10<sup>" + changeHyphensToMinusSigns(value.Exponent) + "</sup>";

            if (value.Exponent == 0) {
                numberTo3SF = Number.parseFloat(value.Coefficient).toPrecision(3);
            }

            var latexTo3SF = Number.parseFloat(value.Coefficient).toPrecision(3) + " \\times 10^{" + value.Exponent + "} \\,\\mathrm{" + value.Units + "}";

            listedValues.push({ "type": "To 3 s.f.", "significand": Number.parseFloat(value.Coefficient).toPrecision(3), "number": numberTo3SF, "units": units, "latex": latexTo3SF });
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
        misc.howPublished.value = "\\url{" + "http://www.physicsformulae.com/#/constant/" + $scope.constant.URLReference + "}"
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
        online.url.value = "http://www.physicsformulae.com/#/constant/" + $scope.constant.URLReference;
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