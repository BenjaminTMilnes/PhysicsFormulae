# PhysicsFormulae

This repository contains a concept project for a formulary of physics web app.

'Formulary' is a word that's not used often (you can look up the etymology of it here https://www.etymonline.com/word/formulary). As such, its meaning in modern English is somewhat vague and unknown to most people. For this project, however, it's used to mean 'a collection of formulae'. A 'formulary of physics' is essentially a dictionary of physics formulae - except rather than giving the pronunciations and meanings of words in a language, it gives the different forms of various well-known equations, and their interpretations, in physics.

This repository contains both the data for a formulary of physics, and a web app to display and navigate that data. A live version of the web app can be found at http://www.physicsformulae.com

## How the data is stored

This repository contains both the code for the Physics Formulae web app, and the data for it. Why, you might ask, is the data stored in the code repository and not in a database?

The information about each formula and constant is stored in the PhysicsFormulae.Formulae folder. The information about formulae is stored in .formula files, and the information about constants is stored in .constant files. Each .formula file contains information about 1 formula.

The .formula and .constant files are just plain text files, with a specific structure. There's actually not that much in each file. By storing the data in plain text files in the repository, it's very easy to keep track of changes to them. By not having an SQL (or other) database, it makes the app far simpler to create, and there's no need to take back-ups of the database.

The format of the .formula and .constant files has been designed to be incredibly easy to edit. The files have almost no markup (which is why they're not just XML or JSON files) - anyone could read them and understand them with no difficulty.

In order to make all of the data stored in the .formula and .constant files usable by the web app, there is a compiler. The compiler reads all of these files, and outputs the data as a single JSON file - all of the data used by the site can be found in PhysicsFormulae.Formulae/Compiled.json . The JSON file can then be very easily understood by the JavaScript-based web app.