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
    uglify = require("gulp-uglify");

//var output = "assets/dist/";
var output = "wwwroot/assets/";

var css = [
    "assets/src/scss/slim.scss"
];

var js = [
    {
        name: "piranha.js",
        items: [
            "node_modules/html5sortable/dist/html5sortable.js",
            "node_modules/nestable2/dist/jquery.nestable.min.js",
            "assets/src/js/piranha.utils.js",
            "assets/src/js/piranha.blockpicker.js",
            "assets/src/js/piranha.notifications.js",
            "assets/src/js/piranha.mediapicker.js",
            "assets/src/js/piranha.preview.js",
            "assets/src/js/piranha.editor.js"
        ]
    },
    {
        name: "piranha.alias.js",
        items: [
            "assets/src/js/piranha.alias.js"
        ]
    },
    {
        name: "piranha.media.js",
        items: [
            "assets/src/js/piranha.media.js"
        ]
    },
    {
        name: "piranha.module.js",
        items: [
            "assets/src/js/piranha.module.js"
        ]
    },
    {
        name: "piranha.pageedit.js",
        items: [
            "assets/src/js/components/region.js",

            "assets/src/js/components/blocks/block-group.js",
            "assets/src/js/components/blocks/html-block.js",
            "assets/src/js/components/blocks/html-column-block.js",
            "assets/src/js/components/blocks/image-block.js",
            "assets/src/js/components/blocks/quote-block.js",
            "assets/src/js/components/blocks/text-block.js",
            "assets/src/js/components/blocks/missing-block.js",

            "assets/src/js/components/fields/checkbox-field.js",
            "assets/src/js/components/fields/string-field.js",
            "assets/src/js/components/fields/html-field.js",
            "assets/src/js/components/fields/image-field.js",
            "assets/src/js/components/fields/number-field.js",
            "assets/src/js/components/fields/text-field.js",
            "assets/src/js/components/fields/missing-field.js",

            "assets/src/js/piranha.pageedit.js"
        ]
    },
    {
        name: "piranha.pagelist.js",
        items: [
            "assets/src/js/components/sitemap-item.js",
            "assets/src/js/piranha.pagelist.js"
        ]
    }
];

//
// Compile & minimize less files
//
gulp.task("min:css", function () {
    for (var n = 0; n < css.length; n++)
    {
        gulp.src(css[n])
            .pipe(sass().on("error", sass.logError))
            .pipe(cssmin())
            .pipe(rename({
                suffix: ".min"
            }))
            .pipe(gulp.dest(output + "css"));
    }
});

//
// Compile & minimize less files
//
gulp.task("min:js", function () {
    for (var n = 0; n < js.length; n++)
    {
        gulp.src(js[n].items, { base: "." })
            .pipe(concat(output + "js/" + js[n].name))
            .pipe(gulp.dest("."))
            .pipe(uglify())
            .pipe(rename({
                suffix: ".min"
            }))
            .pipe(gulp.dest("."));
    }
});

//
// Default tasks
//
gulp.task("serve", ["min:css", "min:js"]);
gulp.task("default", ["serve"]);
