/// <binding BeforeBuild='all' />
module.exports = function (grunt) {
    grunt.initConfig({
        clean: ["wwwroot/lib/*"],
        copy: {
            main: {
                expand: true,
                cwd: 'node_modules/',
                src: [
                    'bootstrap/**/*',
                    'jquery/dist/**/*'
                ],
                dest: 'wwwroot/lib/',
            }
        }
    });

    grunt.loadNpmTasks("grunt-contrib-clean");
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-uglify');

    grunt.registerTask("all", ['clean', 'copy']);
};