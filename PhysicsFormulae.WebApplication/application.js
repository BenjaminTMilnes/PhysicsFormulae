var application = angular.module("PhysicsFormulae", ["ngRoute", "ngSanitize"]);

application.config(function ($routeProvider) {
    $routeProvider
        .when("/", { templateUrl: "search.html", controller: "SearchController" })
        .when("/tag/:tagName", { templateUrl: "search.html", controller: "SearchController" })
        .when("/field/:fieldName", { templateUrl: "search.html", controller: "SearchController" })
        .when("/formula/:reference", { templateUrl: "formula.html", controller: "FormulaController" })
        .when("/constant/:reference", { templateUrl: "constant.html", controller: "ConstantController" })
        .when("/formula-of-the-day", { templateUrl: "formula-of-the-day.html", controller: "FormulaOfTheDayController" })
           .when("/formula-editor", { templateUrl: "formula-editor.html", controller: "FormulaEditorController"})
        .when("/about", { templateUrl: "about.html" });
});

application.directive("mathematics", function () {
    return {
        restrict: "E",
        link: function (scope, element, attributes) {
            var contentType = attributes.contentType;
            var content = attributes.content;
            if (typeof (katex) === "undefined") {
                require(["katex"], function (katex) {
                    katex.render(content, element[0]);
                });
            }
            else {
                katex.render(content, element[0]);
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

application.directive("references", function () {
    return {
        restrict: "E",
        templateUrl: "references.html",
        scope: {
            references: "=references",
            pageType: "=pageType"
        }
    };
});

application.directive("bibtex", function () {
    return {
        restrict: "E",
        templateUrl: "bibtex.html",
        scope: {
            bibtexForThisWebpage: "=bibtexForThisWebpage",
            numberOfGoodReferences: "=numberOfGoodReferences",
            bibtexForOriginalSources: "=bibtexForOriginalSources",
            biblatexForThisWebpage: "=biblatexForThisWebpage"
        }
    };
});

application.directive("seeMore", function () {
    return {
        restrict: "E",
        templateUrl: "see-more.html",
        scope: { links: "=links" }
    };
});

application.directive( "vfg", function () {
    return {
        restrict: "E",
        templateUrl: "vertical-field-group.html",
        scope: {
            label: "=label",
             value:"=value"
 }
    };
});

class Database {
    constructor(data) {
        this._data = data;
    }

    get formulae() {
        return this._data.Formulae;
    }

    get constants() {
        return this._data.Constants;
    }
          
    get references() {
        return this._data.References;
    }

    getFormulaWithReference(reference) {
        var fs = this.formulae.filter(f => f.Reference == reference);

        if (fs.length > 0) { return fs[0]; }

        return null;
    }

    getFormulaWithURLReference(urlReference) {
        var fs = this.formulae.filter(f => f.URLReference == urlReference);

        if (fs.length > 0) { return fs[0]; }

        return null;
    }

    getConstantWithReference(reference) {
        var cs = this.constants.filter(c => c.Reference == reference);

        if (cs.length > 0) { return cs[0]; }

        return null;
    }

    getConstantWithURLReference(urlReference) {
        var cs = this.constants.filter(c => c.URLReference == urlReference);

        if (cs.length > 0) { return cs[0]; }

        return null;
    }
}

application.factory("dataService", ["$http", function ($http) {
    var dataService = {
        data: null,
        getData: function () {
            var that = this;

            if (this.data != null) {
                return new Promise(function (resolve, reject) { return resolve(that.data); });
            }

            return $http.get("formulae.json").then(function (response) {
                that.data = response.data;

                return response.data;
            });
        }
    };

    return dataService;
}]);

application.service("metaService", function () {
    var title = "Physics Formulae";
    var metaDescription = "";
    var metaKeywords = "";
    var previewImageURL = "";
    var canonicalURL = "";

    return {
        set: function (newTitle, newMetaDescription, newMetaKeywords, newPreviewImageURL, newCanonicalURL) {
            title = newTitle;
            metaDescription = newMetaDescription;
            metaKeywords = newMetaKeywords;
            previewImageURL = newPreviewImageURL;
            canonicalURL = newCanonicalURL;
        },
        metaTitle: function () { return title; },
        metaDescription: function () { return metaDescription; },
        metaKeywords: function () { return metaKeywords; },
        previewImageURL: function () { return previewImageURL; },
        canonicalURL: function () { return canonicalURL; }
    }
});
