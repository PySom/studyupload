import React from "react";
import "./Badges.scss";

import b1 from "../../assets/images/b1.svg";
import b2 from "../../assets/images/b2.svg";
import b3 from "../../assets/images/b3.svg";
import b4 from "../../assets/images/b4.svg";
import b5 from "../../assets/images/b5.svg";
import coins from "../../assets/images/coins.svg";

export default function Badges() {
  return (
    <div className="badges">
      <p className="heading">
        Badge counts
        <span className="coins">
          <img src={coins} alt="" /> coins
        </span>
      </p>
      <div className="medals">
        <div className="medal">
          <img src={b1} alt="" />
          <span>0</span>
        </div>
        <div className="medal">
          <img src={b2} alt="" />
          <span>0</span>
        </div>
        <div className="medal">
          <img src={b3} alt="" />
          <span>0</span>
        </div>
        <div className="medal">
          <img src={b4} alt="" />
          <span>0</span>
        </div>
        <div className="medal">
          <img src={b5} alt="" />
          <span>0</span>
        </div>
      </div>
    </div>
  );
}
