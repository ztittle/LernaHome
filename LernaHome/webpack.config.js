var path = require('path');
var webpack = require('webpack');

module.exports = {
  entry: ['babel-polyfill', './UI/app.jsx'],
  output: {
    path: path.resolve(__dirname, 'wwwroot/dist'),
    filename: 'app.js'
  },
  devtool: "source-map",
  resolve: {
      extensions: ['.js', '.jsx']
  },
  module: {
      rules: [
          {
              test: /\.jsx?$/,
              exclude: /node_modules/,
              use: {
                  loader: 'babel-loader',
                  options: {
                      presets: ['env', 'es2015', 'react', 'stage-0']
                  }
              }
          }
      ]
  },
  plugins: [
    new webpack.ContextReplacementPlugin(/heracles/, 'heracles.js')
  ]
};