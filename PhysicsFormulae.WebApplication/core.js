
function makeSearchableString(array) {
    return array.join(", ");
}

function stringContains(string1, string2) {
    return (string1.indexOf(string2) >= 0);
}

function stringIsNullOrEmpty(string) {
    return (!string || /^\s*$/.test(string));
}

function extractTags(text) {
    var re = /#[A-Za-z0-9]+/g;
    var tags = [];

    var m;
    while ((m = re.exec(text)) !== null) {
        tags.push(m[0].substring(1));
    }

    text = text.replace(re, " ");

    return [text, tags];
}

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