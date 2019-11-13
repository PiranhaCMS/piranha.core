/// <binding BeforeBuild='min:js, min:css' />
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
//var output = "wwwroot/assets/";

var css = [
    "assets/src/scss/slim.scss",
    "assets/src/scss/full.scss"
];

var fonts = [
    "node_modules/@fortawesome/fontawesome-free/webfonts/*.*",
    "assets/src/fonts/*.*"
];

var js = [
    {
        name: "piranha-deps-dev.js",
        items: [
            "node_modules/jquery/dist/jquery.slim.js",
            "node_modules/popper.js/dist/umd/popper.js",
            "node_modules/bootstrap/dist/js/bootstrap.js",
            "node_modules/vue/dist/vue.js",
            "node_modules/html5sortable/dist/html5sortable.js",
            "node_modules/nestable2/dist/jquery.nestable.min.js",
            "node_modules/dropzone/dist/dropzone.js",
            "node_modules/select2/dist/js/select2.js",
            "node_modules/vuejs-datepicker/dist/vuejs-datepicker.min.js",
            "node_modules/simplemde/dist/simplemde.min.js"
        ]
    },
    {
        name: "piranha-deps.js",
        items: [
            "node_modules/jquery/dist/jquery.slim.js",
            "node_modules/popper.js/dist/umd/popper.js",
            "node_modules/bootstrap/dist/js/bootstrap.js",
            "node_modules/vue/dist/vue.min.js",
            "node_modules/html5sortable/dist/html5sortable.js",
            "node_modules/nestable2/dist/jquery.nestable.min.js",
            "node_modules/dropzone/dist/dropzone.js",
            "node_modules/select2/dist/js/select2.js",
            "node_modules/vuejs-datepicker/dist/vuejs-datepicker.min.js",
            "node_modules/simplemde/dist/simplemde.min.js"
        ]
    },
    {
        name: "piranha.js",
        items: [
            "assets/src/js/piranha.accessibility.js",
            "assets/src/js/piranha.alert.js",
            "assets/src/js/piranha.dropzone.js",
            "assets/src/js/piranha.permissions.js",
            "assets/src/js/piranha.utils.js",
            "assets/src/js/piranha.blockpicker.js",
            "assets/src/js/piranha.notifications.js",
            "assets/src/js/piranha.mediapicker.js",
            "assets/src/js/piranha.pagepicker.js",
            "assets/src/js/piranha.postpicker.js",
            "assets/src/js/piranha.preview.js",
            "assets/src/js/piranha.resources.js",
            "assets/src/js/piranha.editor.js",
            "assets/src/js/components/page-item.js"
        ]
    },
    {
        name: "piranha.alias.js",
        items: [
            "assets/src/js/piranha.alias.js"
        ]
    },
    {
        name: "piranha.config.js",
        items: [
            "assets/src/js/piranha.config.js"
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
        name: "piranha.contentedit.js",
        items: [
            "assets/src/js/components/region.js",
            "assets/src/js/components/post-archive.js",
            "assets/src/js/components/block-group.js",
            "assets/src/js/components/block-group-horizontal.js",
            "assets/src/js/components/block-group-vertical.js",

            "assets/src/js/components/blocks/html-block.js",
            "assets/src/js/components/blocks/html-column-block.js",
            "assets/src/js/components/blocks/image-block.js",
            "assets/src/js/components/blocks/quote-block.js",
            "assets/src/js/components/blocks/separator-block.js",
            "assets/src/js/components/blocks/text-block.js",
            "assets/src/js/components/blocks/missing-block.js",
            "assets/src/js/components/blocks/audio-block.js",
            "assets/src/js/components/blocks/video-block.js",

            "assets/src/js/components/fields/audio-field.js",
            "assets/src/js/components/fields/checkbox-field.js",
            "assets/src/js/components/fields/date-field.js",
            "assets/src/js/components/fields/document-field.js",
            "assets/src/js/components/fields/html-field.js",
            "assets/src/js/components/fields/image-field.js",
            "assets/src/js/components/fields/markdown-field.js",
            "assets/src/js/components/fields/media-field.js",
            "assets/src/js/components/fields/missing-field.js",
            "assets/src/js/components/fields/number-field.js",
            "assets/src/js/components/fields/page-field.js",
            "assets/src/js/components/fields/post-field.js",
            "assets/src/js/components/fields/readonly-field.js",
            "assets/src/js/components/fields/string-field.js",
            "assets/src/js/components/fields/text-field.js",
            "assets/src/js/components/fields/video-field.js",
            "assets/src/js/components/fields/select-field.js",
        ]
    },
    {
        name: "piranha.pageedit.js",
        items: [
            "assets/src/js/piranha.pageedit.js"
        ]
    },
    {
        name: "piranha.pagelist.js",
        items: [
            "assets/src/js/components/pagecopy-item.js",
            "assets/src/js/components/sitemap-item.js",
            "assets/src/js/piranha.pagelist.js"
        ]
    },
    {
        name: "piranha.postedit.js",
        items: [
            "assets/src/js/piranha.postedit.js"
        ]
    },
    {
        name: "piranha.siteedit.js",
        items: [
            "assets/src/js/piranha.siteedit.js"
        ]
    },
    {
        name: "signalr.min.js",
        items: [
            "node_modules/@aspnet/signalr/dist/browser/signalr.js"

        ]
    }
];

//
// Compile & minimize less files
//
gulp.task("min:css", function () {
    // Minimize and combine styles
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

    // Copy fonts
    for (var n = 0; n < fonts.length; n++)
    {
        gulp.src(fonts[n])
            .pipe(gulp.dest(output + "webfonts"));
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
            .pipe(uglify().on('error', function (e) {
                console.log(e);
            }))
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
