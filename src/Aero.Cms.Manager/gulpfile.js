/// <binding BeforeBuild='min:js, min:css' />
/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/Aerocms/Aero.core
 *
 */
// Gulp task to compile .vue files into Vue.Components(...)
var path = require('path'),
    vueCompiler = require('vue-template-compiler'),
    babel = require("@babel/core"),
    babelTemplate = require("@babel/template").default,
    codeFrameColumns = require('@babel/code-frame').codeFrameColumns,
    babelTypes = require("@babel/types"),
    through2 = require('through2'),
    rtlcss = require('gulp-rtlcss');

function vueCompile() {
    return through2.obj(function (file, _, callback) {
        var relativeFile = path.relative(file.cwd, file.path);
        var ext = path.extname(file.path);
        if (ext === '.vue') {
            var getComponent;
            getComponent = function (ast, sourceCode) {
                const ta = ast.program.body[0]
                if (!babelTypes.isExportDefaultDeclaration(ta)) {
                    var msg = 'Top level declaration in file ' + relativeFile + ' must be "export default {" \n' + codeFrameColumns(sourceCode, { start: ta.loc.start }, { highlightCode: true });
                    throw msg;
                }
                return ta.declaration;
            }

            var compile;
            compile = function (componentName, content) {
                var component = vueCompiler.parseComponent(content, []);
                if (component.styles.length > 0) {
                    component.styles.forEach(s => {
                        const linesToStyle = content.substr(0, s.start).split(/\r?\n/).length;
                        var msg = 'WARNING: <style> tag in ' + relativeFile + ' is ignored\n' + codeFrameColumns(content, { start: { line: linesToStyle } }, { highlightCode: true });
                        console.warn(msg);
                    });
                }

                var ast = babel.parse(component.script.content, {
                    parserOpts: {
                        sourceFilename: file.path
                    }
                });

                var vueComponent = getComponent(ast, component.script.content);
                vueComponent.properties.push(babelTypes.objectProperty(babelTypes.identifier('template'), babelTypes.stringLiteral(component.template.content)))

                var wrapInComponent = babelTemplate("Vue.component(NAME, COMPONENT);");
                var componentAst = wrapInComponent({
                    NAME: babelTypes.stringLiteral(componentName),
                    COMPONENT: vueComponent
                })

                ast.program.body = [componentAst]

                babel.transformFromAst(ast, null, null, function (err, result) {
                    if (err) {
                        callback(err, null)
                    }
                    else {
                        file.contents = Buffer.from(result.code);
                        callback(null, file)
                    }
                });
            }
            var componentName = path.basename(file.path, ext);
            if (file.isBuffer()) {
                compile(componentName, file.contents.toString());
            }
            else if (file.isStream()) {
                var chunks = [];
                file.contents.on('data', function (chunk) {
                    chunks.push(chunk);
                });
                file.contents.on('end', function () {
                    compile(componentName, Buffer.concat(chunks).toString());
                });
            }
        } else {
            callback(null, file);
        }
    });
}

// Gulp build script
var gulp = require("gulp"),
    sass = require('gulp-sass')(require('sass')),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    rename = require("gulp-rename"),
    uglify = require('gulp-uglify-es').default;

var output = "assets/dist/";

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
        name: "Aero-deps-dev.js",
        items: [
            "node_modules/jquery/dist/jquery.js",
            "node_modules/popper.js/dist/umd/popper.js",
            "node_modules/bootstrap/dist/js/bootstrap.js",
            "node_modules/vue/dist/vue.js",
            "node_modules/html5sortable/dist/html5sortable.js",
            "node_modules/nestable2/dist/jquery.nestable.min.js",
            "node_modules/dropzone/dist/dropzone.js",
            "node_modules/select2/dist/js/select2.js",
            "node_modules/vuejs-datepicker/dist/vuejs-datepicker.min.js",
            "node_modules/simplemde/dist/simplemde.min.js",
            "node_modules/dompurify/dist/purify.min.js"
        ]
    },
    {
        name: "Aero-deps.js",
        items: [
            "node_modules/jquery/dist/jquery.js",
            "node_modules/popper.js/dist/umd/popper.js",
            "node_modules/bootstrap/dist/js/bootstrap.js",
            "node_modules/vue/dist/vue.min.js",
            "node_modules/html5sortable/dist/html5sortable.js",
            "node_modules/nestable2/dist/jquery.nestable.min.js",
            "node_modules/dropzone/dist/dropzone.js",
            "node_modules/select2/dist/js/select2.js",
            "node_modules/vuejs-datepicker/dist/vuejs-datepicker.min.js",
            "node_modules/simplemde/dist/simplemde.min.js",
            "node_modules/dompurify/dist/purify.min.js"
        ]
    },
    {
        name: "Aero.js",
        items: [
            "assets/src/js/Aero.eventbus.js",
            "assets/src/js/Aero.accessibility.js",
            "assets/src/js/Aero.alert.js",
            "assets/src/js/Aero.archivepicker.js",
            "assets/src/js/Aero.dropzone.js",
            "assets/src/js/Aero.permissions.js",
            "assets/src/js/Aero.utils.js",
            "assets/src/js/Aero.blockpicker.js",
            "assets/src/js/Aero.notifications.js",
            "assets/src/js/Aero.contentpicker.js",
            "assets/src/js/Aero.mediapicker.js",
            "assets/src/js/Aero.pagepicker.js",
            "assets/src/js/Aero.postpicker.js",
            "assets/src/js/Aero.preview.js",
            "assets/src/js/Aero.languageedit.js",
            "assets/src/js/Aero.resources.js",
            "assets/src/js/Aero.editor.js",
            "assets/src/js/components/page-item.vue"
        ]
    },
    {
        name: "Aero.alias.js",
        items: [
            "assets/src/js/Aero.alias.js"
        ]
    },
    {
        name: "Aero.comment.js",
        items: [
            "assets/src/js/Aero.comment.js"
        ]
    },
    {
        name: "Aero.config.js",
        items: [
            "assets/src/js/Aero.config.js"
        ]
    },
    {
        name: "Aero.media.js",
        items: [
            "assets/src/js/components/folder-item.vue",
            "assets/src/js/Aero.media.js"
        ]
    },
    {
        name: "Aero.module.js",
        items: [
            "assets/src/js/Aero.module.js"
        ]
    },
    {
        name: "Aero.components.js",
        items: [
            "assets/src/js/components/region.vue",
            "assets/src/js/components/post-archive.vue",
            "assets/src/js/components/block-group.vue",
            "assets/src/js/components/block-group-horizontal.vue",
            "assets/src/js/components/block-group-vertical.vue",
            "assets/src/js/components/generic-block.vue",

            "assets/src/js/components/blocks/audio-block.vue",
            "assets/src/js/components/blocks/content-block.vue",
            "assets/src/js/components/blocks/html-block.vue",
            "assets/src/js/components/blocks/html-column-block.vue",
            "assets/src/js/components/blocks/image-block.vue",
            "assets/src/js/components/blocks/markdown-block.vue",
            "assets/src/js/components/blocks/missing-block.vue",
            "assets/src/js/components/blocks/page-block.vue",
            "assets/src/js/components/blocks/post-block.vue",
            "assets/src/js/components/blocks/quote-block.vue",
            "assets/src/js/components/blocks/separator-block.vue",
            "assets/src/js/components/blocks/text-block.vue",
            "assets/src/js/components/blocks/video-block.vue",

            "assets/src/js/components/fields/archivepage-field.vue",
            "assets/src/js/components/fields/audio-field.vue",
            "assets/src/js/components/fields/checkbox-field.vue",
            "assets/src/js/components/fields/color-field.vue",
            "assets/src/js/components/fields/content-field.vue",
            "assets/src/js/components/fields/data-select-field.vue",
            "assets/src/js/components/fields/date-field.vue",
            "assets/src/js/components/fields/document-field.vue",
            "assets/src/js/components/fields/html-field.vue",
            "assets/src/js/components/fields/image-field.vue",
            "assets/src/js/components/fields/markdown-field.vue",
            "assets/src/js/components/fields/media-field.vue",
            "assets/src/js/components/fields/missing-field.vue",
            "assets/src/js/components/fields/number-field.vue",
            "assets/src/js/components/fields/page-field.vue",
            "assets/src/js/components/fields/post-field.vue",
            "assets/src/js/components/fields/readonly-field.vue",
            "assets/src/js/components/fields/select-field.vue",
            "assets/src/js/components/fields/string-field.vue",
            "assets/src/js/components/fields/text-field.vue",
            "assets/src/js/components/fields/video-field.vue",
        ]
    },
    {
        name: "Aero.contentlist.js",
        items: [
            "assets/src/js/Aero.contentlist.js"
        ]
    },
    {
        name: "Aero.contentedit.js",
        items: [
            "assets/src/js/Aero.contentedit.js"
        ]
    },
    {
        name: "Aero.pageedit.js",
        items: [
            "assets/src/js/Aero.pageedit.js"
        ]
    },
    {
        name: "Aero.pagelist.js",
        items: [
            "assets/src/js/components/pagecopy-item.vue",
            "assets/src/js/components/sitemap-item.vue",
            "assets/src/js/Aero.pagelist.js"
        ]
    },
    {
        name: "Aero.postedit.js",
        items: [
            "assets/src/js/Aero.postedit.js"
        ]
    },
    {
        name: "Aero.siteedit.js",
        items: [
            "assets/src/js/Aero.siteedit.js"
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
// Compile & minimize & rtl less files
//
gulp.task("rtl:min:css", function (done) {
    // Minimize and combine styles
    for (var n = 0; n < css.length; n++)
    {
        gulp.src(css[n])
            .pipe(sass().on("error", sass.logError))
            .pipe(cssmin())
            .pipe(rtlcss()) // Convert to RTL.
            .pipe(rename({
                suffix: ".rtl.min"
            }))
            .pipe(gulp.dest(output + "css"));
    }

    // Copy fonts
    for (var n = 0; n < fonts.length; n++)
    {
        gulp.src(fonts[n])
            .pipe(gulp.dest(output + "webfonts"));
    }
    done();
});

//
// Compile & minimize less files
//
gulp.task("min:css", function (done) {
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
    done();
});

//
// Compile & minimize less files
//
gulp.task("min:js", function (done) {
    for (var n = 0; n < js.length; n++)
    {
        gulp.src(js[n].items, { base: "." })
            .pipe(vueCompile())
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
    done();
});

//
// Default tasks
//
gulp.task("serve", gulp.parallel(["min:css", "min:js", "rtl:min:css"]));
gulp.task("default", gulp.series("serve"));
