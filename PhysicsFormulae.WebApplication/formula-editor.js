
application.controller("FormulaEditorController", ["$scope", "dataService", "$rootScope", "metaService", function FormulaEditorController($scope, dataService, $rootScope, metaService) {

    $scope.possibleIdentifierTypes = [];
    $scope.possibleIdentifierObjectTypes = [];
    $scope.possibleFields = [];

    $scope.formulaTitle = "";
    $scope.formulaInterpretation = "";
    $scope.formulaContent = "";
    $scope.formulaIdentifiers = [];
    $scope.formulaFields = [];

    $rootScope.metaService = metaService;

    dataService.getData().then(function (data) {
        var database = new Database(data);

        $rootScope.metaService.set("Formula Editor", "", "");

        database.formulae.forEach(f => {
            f.Identifiers.forEach(i => {
                var a = false;
                var b = false;

                $scope.possibleIdentifierTypes.forEach(t => {
                    if (i.Type == t) {
                        a = true;
                    }
                });

                $scope.possibleIdentifierObjectTypes.forEach(t => {
                    if (i.ObjectType == t) {
                        b = true;
                    }
                });

                if (!a) {
                    $scope.possibleIdentifierTypes.push(i.Type);
                }

                if (!b) {
                    $scope.possibleIdentifierObjectTypes.push(i.ObjectType);
                }
            });

            f.Fields.forEach(g => {

                var c = false;

                $scope.possibleFields.forEach(h => {
                    if (g == h) {
                        c = true;
                    }
                });

                if (!c) {
                    $scope.possibleFields.push(g);
                }
            });

        });
    });

    $scope.addOrRemoveField = function (field) {
        var i = $scope.formulaFields.indexOf(field);

        if (i > -1) {
            $scope.formulaFields.splice(i, 1);
        }
        else {
            $scope.formulaFields.push(field);
        }
    }

    $scope.addNewIdentifier = function () {
        $scope.formulaIdentifiers.push({ "content": "", "type": "", "objectType": "", "reference": "", "dimensions": "", "units": "", "interpretation": "" });
    }

    $scope.addNewIdentifier();

    $scope.getAbbreviation = function (type) {
        if (type == "Variable") {
            return "var.";
        }
        if (type == "Constant") {
            return "const.";
        }
        if (type == "Scalar") {
            return "scal.";
        }
        if (type == "Vector") {
            return "vec.";
        }
    }

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
            if (i > 0 && reference.substr(i - 1, 1) == " ") {
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
        $scope.formulaFileName = $scope.makeFormulaReference($scope.formulaTitle) + ".formula";

        $scope.formulaFileText = $scope.makeFormulaReference($scope.formulaTitle);
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += $scope.formulaTitle;
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += $scope.formulaInterpretation;
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += $scope.formulaContent;
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "where:";
        $scope.formulaFileText += "\n\n";

        $scope.formulaIdentifiers.forEach(i => {
            if (i.content != "" && i.type != "" && i.objectType != "" && i.reference != "" && i.interpretation != "") {

                $scope.formulaFileText += i.content + " [" + i.type + " " + i.objectType + " " + i.reference;

                if (i.dimensions != "") {
                    $scope.formulaFileText += ", " + i.dimensions;
                }

                if (i.units != "") {
                    $scope.formulaFileText += ", " + i.units;
                }

                $scope.formulaFileText += "] " + i.interpretation + "\n";
            }
        });

        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "variants:";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "\n\n";
        $scope.formulaFileText += "fields:";
        $scope.formulaFileText += "\n\n";

        $scope.formulaFields.forEach(f => {
            $scope.formulaFileText += f + "\n";
        })

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

    $scope.$watch("formulaIdentifiers", function (oldValue, newValue) {
        $scope.updateFileText();
    }, true);

    $scope.$watch("formulaFields", function (oldValue, newValue) {
        $scope.updateFileText();
    }, true);
}]);