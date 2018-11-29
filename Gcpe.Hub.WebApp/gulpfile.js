/// <binding AfterBuild='default' Clean='clean' ProjectOpened='watch' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var del = require('del');

var paths = {
    scripts: ['scripts/**/*.js', 'scripts/**/*.ts', 'scripts/**/*.map', '!scripts/**/*.d.ts']
    //scripts: ['Scripts/**/*.js'],
};

gulp.task('clean', function () {
    return del(['wwwroot/scripts/**/*']);
});

gulp.task('default', function () {
    gulp.src(paths.scripts).pipe(gulp.dest('wwwroot/ts'))
});

gulp.task('watch', function () {
    return gulp.watch(paths.scripts, ['default']);
});