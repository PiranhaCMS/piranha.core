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
        "assets/js/polyfill.js",
        "assets/lib/jquery/dist/jquery.js",
        "assets/lib/bootstrap/dist/js/bootstrap.js",
        "assets/lib/jasny-bootstrap/dist/js/jasny-bootstrap.js",
        "assets/lib/moment/min/moment.min.js",
        "assets/lib/bootstrap-datetimepicker-3/build/js/bootstrap-datetimepicker.min.js",
        "assets/lib/jquery-nestable/jquery.nestable.js",
        "assets/lib/jquery.ns-autogrow/dist/jquery.ns-autogrow.js",
        "assets/lib/select2/dist/js/select2.js",
        "assets/lib/dropzone/dist/dropzone.js",
        "assets/lib/simplemde/dist/simplemde.min.js",
        "assets/lib/object.assign-polyfill/object.assign.js",
        "assets/js/html5sortable.js",
        "assets/js/piranha.notifications.js",
        "assets/js/piranha.media.js",
        "assets/js/piranha.page.js",
        "assets/js/piranha.post.js",
        "assets/js/ui.js"
    ],
    jsDest: "assets/js/script.js",
    signalJs: [
        "node_modules/@aspnet/signalr/dist/browser/signalr.min.js"
    ],
    signalDest: "assets/js/script.signalr.js",
    editorLess: "assets/less/editor.less",
    less: "assets/less/style.less",
    css: ["assets/css/*.css", "!assets/css/*.min.css"],
    cssDest: "assets/css"
};

//
// Compile & minimize less files
//
gulp.task("min:css", function () {
  return gulp.src(paths.less)
    .pipe(less({
        relativeUrls: true
    }))
    .pipe(gulp.dest(paths.cssDest))
    .pipe(cssmin())
    .pipe(rename({
        suffix: ".min"
    }))
    .pipe(gulp.dest(paths.cssDest));
});

//
// Compile & minimize editor less file
//
gulp.task("min:editor", function () {
    return gulp.src(paths.editorLess)
        .pipe(less({
            relativeUrls: true
        }))
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
// Combine & minimze js files
//
gulp.task("min:signalr", function () {
    return gulp.src(paths.signalJs, { base: "." })
        .pipe(concat(paths.signalDest))
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
gulp.task("serve", ["min:css", "min:editor", "min:js", "min:signalr"]);
gulp.task("default", ["serve"]);
