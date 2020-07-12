import React, { useEffect, useContext } from "react";
import BackendUI from "./components/BackendUI"
import Main from "./Main";

import { userContext } from "./store/UserContext";
import { Route, useHistory, Redirect } from "react-router-dom";

export default function App() {
    let history = useHistory();
    console.log({ history })
    const { user: { id = undefined, ...user } = {} } = useContext(userContext);
    return (
        <>
            <Route path='/'>
                <Main id={id} />
            </Route>
            <Route exact path='/thankyouforyourwork'>
                {
                    id
                        ? <BackendUI />
                        : <Redirect to="/login" />
                }

            </Route>
        </>
      
  );
}


