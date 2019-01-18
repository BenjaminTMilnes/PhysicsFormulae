
application.controller("FormulaEditorController", ["$scope", "dataService", "$rootScope", "metaService", function FormulaEditorController($scope, dataService, $rootScope, metaService) {

    $scope.formulaTitle = "";
    $scope.formulaInterpretation = "";
    $scope.formulaContent = "";

    $rootScope.metaService = metaService;

    dataService.getData().then(function (data) {
        var database = new Database(data);

        $rootScope.metaService.set(  "Formula Editor", "", "");
    });

    $scope.makeFormulaReference = function (title) {
        if (title == undefined) {
            return "";
        }

        var reference = title;

        for (var i = 0; i < reference.length; i++) {
            if (i == 0) {
                var c = reference.substr(i, 1);

                reference = c.toUpperCase() + reference.substr(i + 1);
            }
            if (i > 0 &&  reference.substr(i-1, 1) == " ") {
                var c = reference.substr(i, 1);

                reference = reference.substr(0, i - 1) + c.toUpperCase() + reference.substr(i + 1);
            }
        }

        if (reference.substr(0, 3) == "The") {
            reference = reference.substr(3);
        }

        return reference;
    }

    $scope.updateFileText = function () {
        $scope.formulaFileText = $scope.makeFormulaReference($scope.formulaTitle);
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += $scope.formulaTitle;
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += $scope.formulaInterpretation;
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += $scope.formulaContent;
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText +=  "where:";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "variants:";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "fields:";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "derived from:";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "references:";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "see more:";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "tags:";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "rating: *****";
        $scope.formulaFileText += "\n\n";
    }

    $scope.$watch("formulaTitle", function (oldValue, newValue) {
        $scope.updateFileText();
    });

    $scope.$watch("formulaContent", function (oldValue, newValue) {
        $scope.updateFileText();
    });

    $scope.$watch("formulaInterpretation", function (oldValue, newValue) {
        $scope.updateFileText();
    });
}]);