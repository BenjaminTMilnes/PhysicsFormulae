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

application.service("metaService", function () {
    var title = "Physics Formulae";
    var metaDescription = "";
    var metaKeywords = "";

    return {
        set: function (newTitle, newMetaDescription, newMetaKeywords) {
            title = newTitle;
            metaDescription = newMetaDescription;
            metaKeywords = newMetaKeywords;
        },
        metaTitle: function () { return title; },
        metaDescription: function () { return metaDescription; },
        metaKeywords: function () { return metaKeywords; }
    }
});

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