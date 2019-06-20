

application.controller("FormulaSetController", ["$scope", "$routeParams", "dataService", "$rootScope", "metaService", function FormulaSetController($scope, $routeParams, dataService, $rootScope, metaService) {

    $rootScope.metaService = metaService;

    dataService.getData().then(function (data) {
        var database = new Database(data);

        $scope.formulaSet = database.getFormulaSetWithURLReference($routeParams.reference);

        $rootScope.metaService.set($scope.formulaSet.Title + " - Physics Formulae", "", $scope.formulaSet.Tags.join(", "), "", "http://www.physicsformulae.com/#/formula-set/" + $scope.formulaSet.URLReference);

    });
    
    $scope.getFormulaContent = (content) => { return  createMathematicsTag(  content, true); }

    $scope.replaceMathematicsMarkers = replaceMathematicsMarkers;
  
    $scope.getColourOfWord = getColourOfWord;
    
    $scope.getHTMLLink = () => { if (!$scope.formulaSet) { return ""; } return "<a href=\"http://www.physicsformulae.com/#/formula-set/" + $scope.formulaSet.URLReference + "\" title=\"" + $scope.formulaSet.Title + " - Physics Formulae\">" + $scope.formulaSet.Title + " - Physics Formulae</a>"; }

    $scope.getCitationKeyForReference = getCitationKeyForReference;
    $scope.getAuthorsString = getAuthorsString;

    new ClipboardJS(".copybutton");

}]);