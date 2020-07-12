import React, { useEffect, useContext, useState } from "react";
import { userContext } from "../../store/UserContext";
import "./Login.scss";

import logo from "../../assets/images/logo.svg";
import logo2 from "../../assets/images/logo.png";
import dots from "../../assets/images/Dots.svg";
import eye from "../../assets/images/eye.svg";
import { Link, withRouter } from "react-router-dom";
import { motion } from "framer-motion";
import { useSnackbar } from "react-simple-snackbar";

import TwitterLogin from "react-twitter-auth";
import FacebookLogin from "react-facebook-login/dist/facebook-login-render-props";
import { GoogleLogin } from "react-google-login";

import authServices from "../../services/authServices";
import Loader from "../../components/loader/Loader";

import * as config from "../../config.json";

export default withRouter(function Login(props) {
  const context = useContext(userContext);
  const [email, setemail] = useState("");
  const [hasError, sethasError] = useState(false);
  const [password, setpassword] = useState("");
  const [viewPwrd, setViewPwrd] = useState(false);
  const [onLogin, setonLogin] = useState(true);
  const [sentEmail, setSentEmail] = useState(false);
  const options = {
    position: "top-right",
  };
  const [openSnackbar, closeSnackbar] = useSnackbar(options);

  const {
    updateLoader,
    updateUser,
    updateLoggedInStatus,
    updateUserCourses,
    updateAwards,
    updateSubjects,
    loading,
    awards,
    pageTransitions,
  } = context;

  const pwrdType = viewPwrd ? "text" : "password";

  useEffect(() => {
    authServices
      .getAwards()
      .then((res) => {
        //console.log(res);
        updateAwards(res.data);
      })
      .catch((err) => {
        console.log(err);
      });

    authServices
      .getAllCourses()
      .then((res) => {
        let subjects = res.data.map((d) => {
          return {
            ...d,
            selected: false,
          };
        });
        updateSubjects(subjects);
      })
      .catch((err) => {
        console.log(err);
      });
  }, []);

  const showError = (err) => {
    openSnackbar(err, 5000);
  };

  const login = () => {
    if (!email || !password) {
      sethasError(true);
      return;
    }
    sethasError(false);
    let data = {
      email,
      password,
    };
    updateLoader(true);
    authServices
      .login(data)
      .then((res) => {
        let user = res.data;
        localStorage.setItem("studymate-token", user.token);
        updateUser(user);
        updateLoggedInStatus(true);
        authServices
          .getUserCourses(user.token, user.id)
          .then((res) => {
            console.log({ res });
            let user_courses = res.data.userCourses;
            let sievedCourses = Object.values(
              user_courses.reduce((agg, curr) => {
                agg[curr.courseId] = curr;
                return agg;
              }, {})
            );
            updateUserCourses(sievedCourses);
            updateLoader(false);
            setTimeout(() => {
              props.history.push("/dashboard");
            }, 500);
          })
          .catch((err) => {
            console.log({ err });
            updateLoader(false);
            showError("An error occured. Please try again");
          });

        authServices
          .getUserAward(user.id)
          .then((res) => {
            // console.log(res);
          })
          .catch((err) => {
            console.log({ err });
          });

        authServices
          .getLeaderboard(user.id, awards[0].id)
          .then((res) => {
            // console.log(res);
          })
          .catch((err) => {
            console.log({ err });
          });
      })
      .catch(function (err) {
        console.log({ err });
        updateLoader(false);
        let {message: errMsg = undefined } = err.response.data;
        showError(
          errMsg ? "Incorrect username or password" : "An error occured"
        );
      });
  };

  const forgotPwrd = () => {};

  const keyUP = (e) => {
    e.key === "Enter" && login();
  };

  const twitterResponse = (e) => {};

  const facebookResponse = (e) => {
    console.log(e);
  };

  const googleResponse = (e) => {
    console.log(e);
    let data = {
      idToken: e.accessToken,
      role: 0,
    };
    authServices
      .googleLogin(data)
      .then((res) => {
        console.log(res);
      })
      .catch((err) => {
        console.log({ err });
      });
  };
  const onFailure = (error) => {
    // alert(error);
  };

  return (
    <>
      {loading && <Loader />}
      <motion.div
        className="login"
        initial={{ opacity: 0, x: "-5vw" }}
        animate={{ opacity: 1, x: "0vw" }}
        exit={{ opacity: 0, x: 0 }}
        transition={{ duration: 0.6 }}
      >
        <div className="left-side">
          <img
            src={logo}
            alt=""
            style={{ width: 170, position: "absolute", top: 50, left: 50 }}
          />

          <img src={dots} className="left-dot" alt="" />
          <img src={dots} className="right-dot" alt="" />
          <div className="student"></div>
          <div className="teacher"></div>
        </div>

        {!onLogin ? (
          <div className="right-side">
            <p style={{ fontSize: 24, fontWeight: "600", marginTop: 100 }}>
              Forgot Password
            </p>
            <div className="form">
              {!sentEmail ? (
                <>
                  <span className="label">Email Address</span>
                  <input
                    className={hasError && !email ? "has-error" : ""}
                    type="text"
                    onChange={(e) => setemail(e.target.value)}
                  />
                  <button className="main-btn mt30" onClick={forgotPwrd}>
                    Continue
                  </button>
                </>
              ) : (
                <div>
                  <p>
                    We have sent an email to {email} with a link to reset your
                    password
                  </p>
                  <p>
                    If you do not receive a mail after a few mninutes, please
                    check your span folder
                  </p>
                  <button
                    className="main-btn mt30"
                    onClick={() => setSentEmail(false)}
                  >
                    Reset password
                  </button>
                </div>
              )}
              <span
                className="no-acct"
                style={{
                  fontSize: 12,
                  position: "relative",
                  top: 10,
                  cursor: "pointer",
                  float: "right",
                }}
                onClick={() => setonLogin(true)}
              >
                Back to <span className="blue--text">login</span>
              </span>
            </div>
            <p className="cc" style={{ position: "absolute", bottom: 20 }}>
              Copyright &copy; 2020 InfoMall Ltd. All rights reserved.
            </p>
          </div>
        ) : (
          <div className="right-side">
            <div className="mobileLogo">
              <img src={logo2} alt="" />
            </div>

            <p
              className="welcome mt50"
              onClick={() => showError("Incorrect email or username")}
            >
              Good to see you again!
            </p>

            <div className="buttons">
              <FacebookLogin
                appId={config.FACEBOOK_APP_ID}
                autoLoad={false}
                callback={facebookResponse}
                render={(renderProps) => (
                  <button onClick={renderProps.onClick} className="fb"></button>
                )}
              />
              <button className="tw"></button>
              <GoogleLogin
                clientId={config.GOOGLE_CLIENT_ID}
                buttonText="Login"
                onSuccess={googleResponse}
                onFailure={onFailure}
                render={(renderProps) => (
                  <button onClick={renderProps.onClick} className="gg"></button>
                )}
              />
            </div>
            <p className="or">
              <span>or</span>
            </p>
            <div className="form">
              <span className="label">Email Address</span>
              <input
                className={hasError && !email ? "has-error" : ""}
                type="text"
                onChange={(e) => setemail(e.target.value)}
                onKeyDown={(e) => keyUP(e)}
              />

              <div className="inp-wrap">
                <span className="label">Password</span>
                <input
                  className={hasError && !password ? "has-error" : ""}
                  type={pwrdType}
                  onChange={(e) => setpassword(e.target.value)}
                  onKeyDown={(e) => keyUP(e)}
                />
                <img
                  src={eye}
                  onClick={() => setViewPwrd(!viewPwrd)}
                  alt=""
                  className="eye"
                />
              </div>

              <span
                className="forgot blue--text"
                onClick={() => setonLogin(false)}
                style={{ cursor: "pointer" }}
              >
                Forgot password?
              </span>

              <button className="main-btn mt30" onClick={login}>
                Log in
              </button>

              <span className="no-acct">
                <Link to="/signup">
                  Don’t have a StudyMate account?{" "}
                  <span className="blue--text">Create an Account</span>
                </Link>
              </span>
            </div>

            <p className="cc">
              Copyright &copy; 2020 InfoMall Ltd. All rights reserved.
            </p>
          </div>
        )}
      </motion.div>
    </>
  );
});
