#Styling
* [Dart Sass](https://sass-lang.com/dart-sass)

##Structure
* **Abstracts/** - variables, mixins
* **Components/** - styles for components such as buttons, modals etc
* **Layout/** - styles necessary for the layout of the site
* **Pages/** - styles for specific pages
* **Sections/** - styles for specific areas such as catalogues

##Compilation
All the *.scss files are compiled to *.css files (minified) in wwwroot/css/ except for the partials (files with names that start with _).

`npm run watch:sass` will watch the source files for changes and re-compile css each time you save your Sass.

`npm run build:sass` compiles the files without watching the changes.