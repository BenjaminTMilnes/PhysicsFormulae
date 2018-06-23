class Subscript {
    constructor() {
        this.type = "subscript";
        this.content = {};
    }
}

class Superscript {
    constructor() {
        this.type ="superscript";
        this.content = {};
    }
}

class Bold {
    constructor() {
        this.type = "bold";
        this.content = {};
    }
}

class RomanText {
    constructor() {
        this.type = "romantext";
        this.content = {};
    }
}

class Symbol {
    constructor() {
        this.type = "symbol";
        this.latex = "";
        this.htmlEntity = "";
    }
}

class Text {
    constructor() {
        this.type = "text";
        this.content = "";
    }
}


function getMathsObjectFromLaTeX(latex) {

}

function convertMathsObjectToHTML(mathsObject) {

    if (mathsObject.type == "subscript") {
        return "<sub>" + convertMathsObjectToHTML(mathsObject.content) + "</sub>";
    }

    if (mathsObject.type == "superscript") {
        return "<sup>" + convertMathsObjectToHTML(mathsObject.content) + "</sup>";
    }

    if (mathsObject.type == "bold") {
        return "<strong>" + convertMathsObjectToHTML(mathsObject.content) + "</strong>";
    }

    if (mathsObject.type == "romantext") {
        return "<span style='font-style: normal;'>" + convertMathsObjectToHTML(mathsObject.content) + "</span>";
    }

    if (mathsObject.type == "text") {
        return mathsObject.content;
    }

}