import React from 'react';
import ReactDOM from 'react-dom';
import Devices from './components/Devices';
import { Provider } from "react-redux";
import { combineReducers, createStore, applyMiddleware, compose } from 'redux';
import thunkMiddleware from "redux-thunk";
import { createLogger } from 'redux-logger';
import app from './reducers';

let middleWare;
if (process.env.NODE_ENV === 'production') {
    middleWare = applyMiddleware(thunkMiddleware);
}
else {
    const loggerMiddleware = createLogger();
    middleWare = compose(
        applyMiddleware(thunkMiddleware, loggerMiddleware),
        window.devToolsExtension ? window.devToolsExtension() : f => f
    )
}

const store = createStore(
    combineReducers(app),
    middleWare
);

ReactDOM.render(
    <Provider store={store}>
      <Devices/>
    </Provider>,
  document.getElementById('root')
);