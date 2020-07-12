import React, { useContext, useState } from "react";
import { useSnackbar } from "react-simple-snackbar";
// import { userContext } from "../../store/UserContext";
import "./Signup.scss";

import eye from "../../assets/images/eye.svg";
import { Link, withRouter } from "react-router-dom";
import { Carousel } from "react-responsive-carousel";
import { motion } from "framer-motion";
import "react-responsive-carousel/lib/styles/carousel.min.css";

import flex from "../../assets/images/flex1.png";
import flex2 from "../../assets/images/Slide-Image-2.png";
import flex3 from "../../assets/images/Slide-Image-3.png";
import logo from "../../assets/images/logo.svg";
import logo2 from "../../assets/images/logo.png";
import { userContext } from "../../store/UserContext";

import TwitterLogin from "react-twitter-auth";
import FacebookLogin from "react-facebook-login/dist/facebook-login-render-props";
import { GoogleLogin } from "react-google-login";
import * as config from "../../config.json";

import authServices from "../../services/authServices";
import Loader from "../../components/loader/Loader";

export default withRouter(function Signup(props) {
  const context = useContext(userContext);
  const [surName, setsurName] = useState("");
  const [firstName, setfirstName] = useState("");
  const [phoneNumber, setPhone] = useState("");
  const [email, setemail] = useState("");
  const [password, setpassword] = useState("");
  const [viewPwrd, setViewPwrd] = useState(false);
  const [hasError, sethasError] = useState(false);
  const [image, setimage] = useState("");
  const options = {
    position: "top-right",
  };
  const [openSnackbar, closeSnackbar] = useSnackbar(options);

  const pwrdType = viewPwrd ? "text" : "password";

  const {
    updateLoader,
    updateUser,
    updateLoggedInStatus,
    updateUserCourses,
    loading,
    pageTransitions,
  } = context;

  const showError = (err) => {
    console.log(err);
    openSnackbar(err, 5000);
  };

  const submit = (payload) => {
    if (!payload && (!surName || !firstName || !email || !password)) {
      sethasError(true);
      return;
    }

    let data = {
      surName,
      firstName,
      phoneNumber,
      email,
      password,
      image,
      confirmPassword: password,
    };
    let submitData = payload ? payload : data;
    updateLoader(true);
    authServices
      .signup(data)
      .then((res) => {
        console.log(res);
        let user = res.data;
        updateUser(user);
        updateLoggedInStatus(true);
        setsurName("");
        setfirstName("");
        setPhone("");
        setemail("");
        setpassword("");
        localStorage.setItem("studymate-token", user.token);
        authServices
          .getUserCourses(user.token, user.id)
          .then((res) => {
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
      })
      .catch((err) => {
        console.log({ err });
        updateLoader(false);
        showError(err.response.data.message);
      });
  };

  const twitterResponse = (e) => {};

  const facebookResponse = (e) => {
    console.log(e);
    let data = {
      email: e.email,
      surName: e.name.split(" ")[0],
      firstName: e.name.split(" ")[1],
      image: e.picture.data.url,
      password: e.userID,
    };
    // submit(data);
  };

  const googleResponse = (e) => {
    console.log(e);
  };

  const onFailure = (error) => {
    // alert(error);
  };

  return (
    <>
      {loading && <Loader />}
      <motion.div
        className="signup"
        initial={{ opacity: 0, x: "5vw" }}
        animate={{ opacity: 1, x: "0vw" }}
        exit={{ opacity: 0, x: 0 }}
        transition={{ duration: 0.6 }}
      >
        <div className="left-side">
          <img
            src={logo}
            alt=""
            style={{ width: 170, position: "absolute", top: 60, left: 100 }}
          />
          <Carousel
            showArrows={false}
            showThumbs={false}
            showStatus={false}
            infiniteLoop={true}
            transitionTime={10}
            interval={4000}
            autoPlay={true}
          >
            <div className="myslide">
              <p className="main-text">Choose your study mate wisely</p>
              <p className="smalls">
                Get ahead of your peers in knowledge with over 1,000 Junior and
                Senior Secondary School video lessons on all statutory subjects
                at your convenience
              </p>
              <img src={flex} alt="" style={{ bottom: "-110px" }} />
            </div>
            <div className="myslide">
              <p className="main-text">24hour access to quality learning</p>
              <p className="smalls">
                While access to classrooms remain limited, StudyMate ensures you
                continue learning with over 10,000 practice questions and mini
                lessons across all approved subjects.
              </p>
              <img src={flex2} alt="" style={{ bottom: "-110px" }} />
            </div>
            <div className="myslide">
              <p className="main-text">Access quality content every time</p>
              <p className="smalls">
                Join thousands of students who continue learning on our always-
                available quality educational contents.
              </p>
              <img src={flex3} alt="" />
            </div>
          </Carousel>
        </div>

        <div className="right-side">
          <div className="mobileLogo">
            <img src={logo2} alt="" />
          </div>

          <p className="welcome mt50">Sign up to StudyMate</p>

          <div className="buttons">
            <FacebookLogin
              appId={config.FACEBOOK_APP_ID}
              autoLoad={false}
              callback={facebookResponse}
              fields="name,email,picture"
              scope="profile"
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
            <div className="half">
              <span className="label">First Name</span>
              <input
                className={hasError && !firstName ? "has-error" : ""}
                type="text"
                onChange={(e) => setfirstName(e.target.value)}
              />
            </div>
            <div className="half f-right">
              <span className="label">Last Name</span>
              <input
                className={hasError && !surName ? "has-error" : ""}
                type="text"
                onChange={(e) => setsurName(e.target.value)}
              />
            </div>

            <span className="label">Email Address</span>
            <input
              className={hasError && !email ? "has-error" : ""}
              type="text"
              onChange={(e) => setemail(e.target.value)}
            />

            <span className="label">Phone Number</span>
            <input
              className={hasError && !phoneNumber ? "has-error" : ""}
              type="text"
              onChange={(e) => setPhone(e.target.value)}
            />

            <div className="inp-wrap">
              <span className="label">Password</span>
              <input
                className={hasError && !password ? "has-error" : ""}
                type={pwrdType}
                onChange={(e) => setpassword(e.target.value)}
              />
              <img
                src={eye}
                onClick={() => setViewPwrd(!viewPwrd)}
                alt=""
                className="eye"
              />
            </div>

            <span className="no-acct">
              Creating an account means you are ok with our{" "}
              <Link to="/terms">
                <span className="blue--text">Terms of Service, </span>{" "}
              </Link>
              <Link to="/privacy">
                <span className="blue--text">Privacy Policy </span>
              </Link>
              and{" "}
              <span className="blue--text">Default Notification Settings.</span>
            </span>

            <button className="main-btn mt20" onClick={() => submit(null)}>
              Create account
            </button>

            <span className="no-acct">
              <Link to="/login">
                Have an account? <span className="blue--text">Login</span>
              </Link>
            </span>
          </div>
        </div>
      </motion.div>
    </>
  );
});
