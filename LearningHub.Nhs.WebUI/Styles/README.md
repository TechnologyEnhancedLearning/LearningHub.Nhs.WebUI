#Styling
* [Dart Sass](https://sass-lang.com/dart-sass)

##Structure
* **abstracts/** - variables, mixins
* **components/** - styles for components such as buttons, modals etc
* **layout/** - styles necessary for the layout of the site
* **pages/** - styles for specific pages
  * Only the home and landing page stylesheets are imported directly, the other being included in the main stylesheet
* **sections/** - styles for main areas such as catalogues, resources
  * `_all.scss` contains some styling rules that apply to multiple parts of the site as well
* **vendors/** - for other frameworks such as bootstrap
  * To change the bootstrap version you have to overwrite the `wwwroot/lib/bootstrap` directory

##Compilation
All the *.scss files are compiled to *.css files (minified) in wwwroot/css/ except for the partials (files with names that start with _).

`npm run watch:sass` will watch the source files for changes and re-compile css each time you save your Sass.

`npm run build:sass` compiles the files without watching the changes.