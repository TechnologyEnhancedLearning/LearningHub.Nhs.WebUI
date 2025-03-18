console.log("Running webpack build...")

const path = require('path');
const webpack = require('webpack');
const VueLoaderPlugin = require('vue-loader/lib/plugin');
const appBasePath = './Scripts/vuesrc/';

module.exports = {
    entry: {
        nhsuk: './Scripts/nhsuk.ts',
        header: './Scripts/vuesrc/header.ts',
        maincontent: './Scripts/vuesrc/maincontent.ts',
        tray: './Scripts/vuesrc/tray.ts',
        mycontributions: './Scripts/vuesrc/mycontributions/mycontributions.ts',
        registrationcontainer: './Scripts/vuesrc/registration/registrationcontainer.ts',
        resourcecontainer: './Scripts/vuesrc/resource/resourcecontainer.ts',
        contributecontainer: './Scripts/vuesrc/contribute/contributecontainer.ts',
        contributeresourcecontainer: './Scripts/vuesrc/contribute-resource/contributeresourcecontainer.ts',
        cataloguecontainer: './Scripts/vuesrc/catalogue/cataloguecontainer.ts',
        forgotpassword: './Scripts/vuesrc/forgotpassword.ts',
        certificateCheck: './Scripts/vuesrc/certificateCheck.ts',
        notificationcontainer: './Scripts/vuesrc/notification/notificationcontainer.ts',
        mylearningcontainer: './Scripts/vuesrc/mylearning/mylearningcontainer.ts',
        catalogueslistcontainer: './Scripts/vuesrc/dashboard/catalogueslistcontainer.ts',
        roadmapcontainer: './Scripts/vuesrc/roadmap/roadmapcontainer.ts',
        scormcontainer: './Scripts/vuesrc/learningsessions/scormcontainer.ts',
        scormtracecontainer: './Scripts/vuesrc/learningsessions/scormtracecontainer.ts',
        profilecontainer: './Scripts/vuesrc/profile/profilecontainer.ts'
    },
    output: {
        path: path.resolve(__dirname, './wwwroot/js/bundle/'),
        publicPath: '/wwwroot/js/bundle/',
        filename: '[name].js'
    },
    resolve: {
        extensions: ['.ts', '.js', '.cjs', '.vue', '.json'],
        alias: {
            'vue$': 'vue/dist/vue.esm.js',
            '@': path.join(__dirname, appBasePath)
        }
    },
    externals: {
        jquery: 'jQuery'
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                loader: 'ts-loader',
                exclude: /node_modules/,
                options: {
                    appendTsSuffixTo: [/\.vue$/],
                }
            },
            {
                test: /\.vue$/,
                use: [
                    {
                        loader: 'vue-loader'
                    },
                ],
            },
            {
                test: /\.(js|cjs)$/,
                loader: 'babel-loader',
                exclude: function (path) {
                    if (path.indexOf("node_modules") > -1) {
                        if ( path.indexOf("ckeditor") > -1
                            || path.indexOf("resize-detector") > -1
                            || path.indexOf("vue-clamp") > -1
                            || path.indexOf("nanoid") > -1
                            || path.indexOf("postcss") > -1
                            || path.indexOf("nhse-tel-frontend") > -1
                            || path.indexOf("sanitize-html") > -1) {
                            return false;
                        }
                        return true;
                    }
                    console.log(path);
                    return false;
                },               
                include: [
                    path.resolve('node_modules/resize-detector'),
                    path.resolve('node_modules/vue-clamp'),
                    path.resolve('node_modules/postcss'),
                    path.resolve('node_modules/nanoid'),
                    path.resolve('node_modules/nhse-tel-frontend'),
                    path.resolve('node_modules/sanitize-html'),
                ],
                options: {
                    "presets": [
                        [
                            "@vue/babel-preset-app",
                            {
                                "targets": {
                                    "browsers": [
                                        "> 1%",
                                        "last 2 versions",
                                        "not ie <= 8"
                                    ]
                                },
                                "useBuiltIns": "entry"
                            }
                        ]
                    ]
                }
            },
            {
                test: /\.scss$/,
                use: [
                    {
                        loader: 'style-loader'
                    },
                    {
                        loader: 'css-loader',
                        options: {
                            url: false,
                        },
                    },
                    {
                        loader: 'sass-loader',
                        options: {
                            implementation: require("sass"),
                        },
                    },
                ],
            },
            {
                test: /\.css$/,
                use: [
                    {
                        loader: 'style-loader'
                    },
                    {
                        loader: 'css-loader'
                    },
                ],
            },
            {
                test: /\.(eot|svg|ttf|woff|woff2)(\?\S*)?$/,
                loader: 'file-loader'
            },
            {
                test: /\.(png|jpe?g|gif|svg)(\?\S*)?$/,
                loader: 'file-loader',
                options: {
                    name: '[name].[ext]?[hash]'
                }
            }
        ]
    },
    plugins: [
        new VueLoaderPlugin(),
    ],
    devServer: {
        proxy: {
            '*': {
                target: 'http://localhost:53306',
                changeOrigin: true
            }
        }
    },
};

if (process.env.NODE_ENV === 'production') {
    module.exports.mode = 'production';
} else {
    module.exports.mode = 'development';
    module.exports.devtool = 'source-map';
}