var path = require('path');

module.exports = {
  entry: './UI/App.jsx',
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
              exclude: /(node_modules|bower_components)/,
              use: {
                  loader: 'babel-loader',
                  options: {
                      presets: ['env', 'react']
                  }
              }
          }
      ]
  }
};