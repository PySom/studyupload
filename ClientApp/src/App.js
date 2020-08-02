import React from "react";
import BackendUI from "./components/BackendUI"

import { Route, useHistory } from "react-router-dom";

export default function App() {
    let history = useHistory();
    console.log({ history })
    return (
        <>
            <Route exact path='/'>
                <BackendUI />
            </Route>
        </>
      
  );
}


