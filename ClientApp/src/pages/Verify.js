import React, { useContext, useEffect } from "react";
import Loader from "../components/loader/Loader";
import { userContext } from "../store/UserContext";
import { withRouter } from "react-router-dom";
import authServices from "../services/authServices";
import { useSnackbar } from "react-simple-snackbar";

export default withRouter(function Verify({ history }) {
  let context = useContext(userContext);
  const { updateLoader, loading } = context;
  const options = {
    position: "top-right",
  };
  const [openSnackbar, closeSnackbar] = useSnackbar(options);

  useEffect(() => {
    let location = window.location.hash;
    console.log({ location });
    if (location.includes("verifyemail")) {
      let code = location.split("=")[1];
      verifyEmail(code);
      updateLoader(true);
    }
  }, []);

  const verifyEmail = (code) => {
    authServices
      .verifyEmail({ code })
      .then((res) => {
        console.log(res);
        openSnackbar("Email verified successfully", 5000);
        updateLoader(false);
        let token = localStorage.getItem("studymate-token");
        token ? history.push("/dashboard") : history.push("/login");
      })
      .catch((err) => {
        console.log({ err });
        updateLoader(false);
        openSnackbar(err.response.data.message, 5000);
        history.push("/login");
      });
  };

  return <div>{loading && <Loader />}</div>;
});
