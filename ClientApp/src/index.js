import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import * as serviceWorker from './serviceWorker';
import { HashRouter } from 'react-router-dom';
import UserContextProvider from "./store/UserContext";
import SnackbarProvider from "react-simple-snackbar";

ReactDOM.render(
    <React.StrictMode>
        <HashRouter>
            <SnackbarProvider>
                <div className="App">
                    <UserContextProvider>
                        <App />
                    </UserContextProvider>
                </div>
            </SnackbarProvider>
        </HashRouter>
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
