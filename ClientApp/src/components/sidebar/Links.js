import React, { useState } from "react";
import { useRouteMatch, Link, withRouter } from "react-router-dom";
import "./Sidebar.scss";

import Learn from "../../assets/images/Learn.svg";
import Progress from "../../assets/images/Progress.svg";
import Profile from "../../assets/images/Profile.svg";

export default withRouter(function Links({ history }) {
  let match = useRouteMatch();
  const [links, setlinks] = useState([
    { text: "Learn", to: "", icon: Learn },
    { text: "Progress", to: "progress", icon: Progress },
    { text: "My Profile", to: "profile", icon: Profile },
  ]);

  const [linkIndex, setlinkIndex] = useState(0);

  const navigate = (i, to) => {
    setlinkIndex(i);
    console.log(to);
    history.push(`/dashboard/${to}`);
  };

  return (
    <ul className="links-list">
      {links.map((link, i) => {
        return (
          <li
            onClick={() => navigate(i, link.to)}
            className={linkIndex === i ? "active" : ""}
            key={link.text}
          >
            <img src={link.icon} className="mr20" alt="" />
            <span>{link.text}</span>

            <svg
              version="1.1"
              x="0px"
              y="0px"
              width="12px"
              height="12px"
              className="f-right mt5 mr0"
              viewBox="0 0 512.005 512.005"
            >
              <g>
                <g>
                  <path
                    d="M388.418,240.923L153.751,6.256c-8.341-8.341-21.824-8.341-30.165,0s-8.341,21.824,0,30.165L343.17,256.005
                        L123.586,475.589c-8.341,8.341-8.341,21.824,0,30.165c4.16,4.16,9.621,6.251,15.083,6.251c5.461,0,10.923-2.091,15.083-6.251
                        l234.667-234.667C396.759,262.747,396.759,249.264,388.418,240.923z"
                  />
                </g>
              </g>
            </svg>
          </li>
        );
      })}
    </ul>
  );
});
