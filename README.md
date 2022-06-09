# Physics Formulae

Physics Formulae is a web-based formulary of physics.

'Formulary' is a word that's not used often (you can look up the etymology of it here https://www.etymonline.com/word/formulary). As such, its meaning in modern English is somewhat vague and unknown to most people. For this project, however, it's used to mean 'a collection of formulae'. A 'formulary of physics' is essentially a dictionary of physics formulae - except rather than giving the pronunciations and meanings of words in a language, it gives the different forms of various well-known equations, and their interpretations, in physics.

This project is live, and can be viewed here: http://www.physicsformulae.com

## Table of Contents

- [Principles of Design](#principles-of-design)
    - [Storing the data](#storing-the-data)
    - [The front-end app](#the-front-end-app)
- [Status and Planned Changes](#status-and-planned-changes)

## Principles of Design

### Storing the data

For this project, I wanted to avoid using an SQL (or other) database to store the data. SQL databases can take a while to set up properly, and can be tricky to update. Adding and editing large quantities of data within them can also be time-consuming. So instead, I decided to store the data for this web app in the code repository itself. Each formula and constant is stored as a single file. These files are just plain text files with a specific format. This specific format means that very little markup is needed. When a formula or constant is updated, some code is run, and these plain text files are read and converted into a JSON format, which can then be used by the front-end web app.

This approach has several advantages. It's very easy to review the data - having minimal markup makes it easy to read. It's very easy to edit the data, as it's just plain text files. As it's in the repository, a complete edit history is available. The JSON file that is produced by the compiler code, while large for a JSON file, is very small compared to a lot of other data objects that are transferred across the internet today, such as images and videos. Since this JSON file contains the entire data set, compiled and ready to use, once it has been downloaded, the web app will continue to function even if the internet connection is lost, providing a smoother experience. Furthermore, deploying the app simply involves transferring static files, with no further setup. Making back-ups is also very straightforward.

The .formula and .constant files can be found in the PhysicsFormulae.Formulae folder.

### The front-end app

The front-end app is an AngularJS, single-page web app. When the app is first opened, it downloads the compiled data (Compiled.json). Using the app is then essentially just 'navigating' this data.

The app allows you to search through the formulae and constants, view detailed information about them, copy the LaTeX for them, see any variants or related formulae or constants, and sometimes, in the case of formulae, see a derivation of them.

## Status and Planned Changes

The app is currently live and working well. Most of the work left to do is simply adding more formulae and constants - this will be done gradually over time.

Among the technical features to be added and improved are:

- Improving the mathematics rendering engine that's used to make the PNG images of the formulae
- Adding a feature to allow the user to enter all but one of the terms in a formula, and have the final term be calculated
- Clear delineation between the different levels of interpretation of a formula
