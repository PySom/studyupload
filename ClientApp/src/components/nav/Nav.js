import React, { useState, useContext } from "react";
import "./Nav.scss";

import userIcon from "../../assets/images/user.svg";
import logo from "../../assets/images/logo.png";

import edit from "../../assets/images/Edit-profile.svg";
import logoutIcon from "../../assets/images/logout.svg";

import caret from "../../assets/images/down-arrow.svg";
import search from "../../assets/images/search.svg";
import { withRouter } from "react-router-dom";
import { userContext } from "../../store/UserContext";
import { appUrl } from "../../services/urls";

export default withRouter(function Nav(props) {
  const context = useContext(userContext);
  const fullname = `${context.user.firstName} ${context.user.surName}`;

  const [open, setopen] = useState(false);
  const [show_nav, setshow_nav] = useState(false);

  const logout = () => {
    context.logout();
    setTimeout(() => {
      props.history.push("/login");
    }, 200);
  };

  const toProfile = () => {
    props.history.push("/edit-profile");
  };

  const goTo = (to) => {
    if (to === "courses") props.history.push("/dashboard/");
    if (to === "edit") props.history.push("/edit-profile");
    if (to === "logout") logout();
    setshow_nav(false);
  };

  return (
    <div className="nav">
      <span className="logo">
        <img src={logo} className="logo" alt="" />
      </span>

      {context.isLoggedIn && (
        <span className="user f-right">
          <div
            className="avatar"
            style={{
              backgroundImage: `url("${appUrl}${
                context.user.image && context.user.image.replace("\\", "/")
              }")`,
              backgroundSize: "cover",
              backgroundPosition: "center",
            }}
          >
            {!context.user.image && <img src={userIcon} height="30" alt="" />}
          </div>
          {fullname}
          <img
            src={caret}
            style={{
              width: 10,
              marginLeft: 12,
              transform: open ? `rotate(180deg)` : "",
            }}
            alt=""
          />
          <div className="dropdown">
            <ul>
              <li className="bb" onClick={toProfile}>
                Edit profile <img src={edit} className="f-right m15" alt="" />{" "}
              </li>
              <li onClick={logout}>
                Logout <img src={logoutIcon} className="f-right m15" alt="" />{" "}
              </li>
            </ul>
          </div>
        </span>
      )}

      {/* {context.isLoggedIn && (
        <span className="inp">
          <img src={search} alt="" />
          <input type="text" placeholder="What do you want learn?" />
        </span>
      )} */}

      <div
        id="nav-icon1"
        onClick={() => setshow_nav(!show_nav)}
        className={show_nav ? "open" : ""}
      >
        <span></span>
        <span></span>
        <span></span>
      </div>

      <div className={`mobilenav ${show_nav ? "open" : ""}`}>
        {/* <input type="text" placeholder="What do you want learn?" /> */}

        <p onClick={() => goTo("edit")}>
          <img src={edit} className="logo" alt="" />
          Edit Profile
        </p>
        <p onClick={() => goTo("logout")}>
          <img src={logoutIcon} className="logo" alt="" />
          Logout
        </p>
      </div>
    </div>
  );
});
