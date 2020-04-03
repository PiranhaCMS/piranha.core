/// <binding BeforeBuild='min' />
/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

var gulp = require("gulp"),
    sass = require('gulp-sass'),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    rename = require("gulp-rename"),
    uglifyes = require('uglify-es'),
    composer = require('gulp-uglify/composer'),
    uglify = composer(uglifyes, console);

var output = "assets/dist/";

var resources = [
    "node_modules/summernote/dist/**/*.*",
    "node_modules/codemirror/lib/*.*",
    "node_modules/codemirror/mode/xml/*.*",
    "node_modules/codemirror/addon/hint/show-hint.css",
    "node_modules/codemirror/addon/hint/show-hint.js",
    "node_modules/codemirror/addon/hint/xml-hint.js",
    "node_modules/codemirror/addon/hint/html-hint.js"
];

var styles = [
    "assets/src/piranha.summernote.scss",
];

var scripts = [
    "assets/src/piranha.summernote.js",
    "assets/src/piranha.editor.js"
];

gulp.task("min", function (done) {
    // Copy resources
    var n;
    for (n = 0; n < resources.length; n++)
    {
        gulp.src(resources[n])
            .pipe(gulp.dest(output));
    }

    // Compile scss
    for (n = 0; n < styles.length; n++)
    {
        gulp.src(styles[n])
            .pipe(sass().on("error", sass.logError))
            .pipe(cssmin())
            .pipe(rename({
                suffix: ".min"
            }))
            .pipe(gulp.dest(output));
    }

    // Compile js
    gulp.src(scripts, { base: "." })
        .pipe(concat(output + "piranha.summernote.js"))
        .pipe(gulp.dest("."))
        .pipe(uglify().on('error', function (e) {
            console.log(e);
        }))
        .pipe(rename({
            suffix: ".min"
        }))
        .pipe(gulp.dest("."));
    done();
});

//
// Default tasks
//
gulp.task("serve", gulp.series("min"));
gulp.task("default", gulp.series("serve"));
