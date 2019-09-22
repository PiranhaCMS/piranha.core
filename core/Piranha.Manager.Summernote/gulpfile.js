/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

var gulp = require("gulp");

var output = "assets/dist/";

var resources = [
    "node_modules/summernote/dist/**/*.*",
    "assets/src/*.*"
];

gulp.task("min", function () {
    // Copy resources
    for (var n = 0; n < resources.length; n++)
    {
        gulp.src(resources[n])
            .pipe(gulp.dest(output));
    }
});

//
// Default tasks
//
gulp.task("serve", ["min"]);
gulp.task("default", ["serve"]);
