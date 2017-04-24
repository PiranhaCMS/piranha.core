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

var paths = {
    js: [
        "assets/lib/jquery/dist/jquery.js",
        "assets/lib/bootstrap/dist/js/bootstrap.js"
    ],
    jsDest: "wwwroot/assets/js/script.min.js",
    less: "assets/less/style.less",
    cssDest: "wwwroot/assets/css"
};

//
// Compile & minimize less files
//
gulp.task("min:css", function () {
  return gulp.src(paths.less)
    .pipe(less({
        relativeUrls: true
    }))
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
        .pipe(uglify())
        .pipe(gulp.dest("."));
});  

//
// Default tasks
//
gulp.task("serve", ["min:css", "min:js"]);
gulp.task("default", ["serve"]);
