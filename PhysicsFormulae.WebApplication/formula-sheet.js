

application.controller("FormulaSheetController", ["$scope", "$routeParams", "dataService", "$rootScope", "metaService", function FormulaSheetController($scope, $routeParams, dataService, $rootScope, metaService) {

    $rootScope.metaService = metaService;

    dataService.getData().then(function (data) {
        var database = new Database(data);

        $scope.formulaSheet = database.getFormulaSheetWithURLReference($routeParams.reference);

        $rootScope.metaService.set($scope.formulaSheet.Title + " - Physics Formulae", "", [], "", "http://www.physicsformulae.com/#/formula-sheet/" + $scope.formulaSheet.URLReference);

    });

    $scope.getFormulaContent = (content) => { return createMathematicsTag(content, true); }

    $scope.replaceMathematicsMarkers = replaceMathematicsMarkers;

    $scope.getColourOfWord = getColourOfWord;

    $scope.getHTMLLink = () => { if (!$scope.formulaSheet) { return ""; } return "<a href=\"http://www.physicsformulae.com/#/formula-sheet/" + $scope.formulaSheet.URLReference + "\" title=\"" + $scope.formulaSheet.Title + " - Physics Formulae\">" + $scope.formulaSheet.Title + " - Physics Formulae</a>"; }

    $scope.getCitationKeyForReference = getCitationKeyForReference;
    $scope.getAuthorsString = getAuthorsString;

    new ClipboardJS(".copybutton");

}]);