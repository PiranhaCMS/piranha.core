/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

var gulp = require("gulp"),
    less = require("gulp-less"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    rename = require("gulp-rename"),
    uglify = require("gulp-uglify");

var project = require('./project.json');

var paths = {
    js: [
        "assets/vendor/jquery.js",
        "assets/vendor/bootstrap/js/bootstrap.js",
        "assets/vendor/jquery-sortable.js",
        "assets/vendor/jquery-nestable.js",
        "assets/js/ui.js"
    ],
    jsDest: "assets/js/script.js",
    less: "assets/css/style.less",
    css: ["assets/css/*.css", "!assets/css/*.min.css"],
    cssDest: "assets/css"
};

//
// Compile & minimize less files
//
gulp.task("min:css", function () {
  return gulp.src(paths.less)
    .pipe(less())
    .pipe(gulp.dest(paths.cssDest))
    .pipe(cssmin())
    .pipe(rename({
        suffix: ".min"
    }))
    .pipe(gulp.dest(paths.cssDest));
});

//
// Combine & minimze js files
//
gulp.task("min:js", function () {
    return gulp.src(paths.js, { base: "." })
        .pipe(concat(paths.jsDest))
        .pipe(gulp.dest("."))
        .pipe(uglify())
        .pipe(rename({
            suffix: ".min"
        }))
        .pipe(gulp.dest("."));
});  

//
// Default tasks
//
gulp.task("serve", ["min:css", "min:js"]);
gulp.task("default", ["serve"]);
