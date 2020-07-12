import React, { useState, useContext } from "react";
import "./Sidebar.scss";
import students from "../../assets/images/students.png";
import { useRouteMatch, Link, withRouter } from "react-router-dom";

import Links from "./Links";
import { userContext } from "../../store/UserContext";
import UserPhoto from "../user-photo/UserPhoto";

export default withRouter(function Sidebar({ history }) {
  const context = useContext(userContext);
  const fullname = `${context.user.firstName} ${context.user.surName}`;

  return (
    <div className="sidebar">
      <div className="upper">
        <UserPhoto />
        <p className="user-details">
          {fullname}
          <span>
            {context.user.level === 3 ? "Junior secondary" : "Senior secondary"}
          </span>
        </p>
        <Links />
      </div>

      <img src={students} alt="" className="students" />
    </div>
  );
});
