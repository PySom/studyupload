import React from "react";
import spinner from "../../assets/images/spinner.svg";

export default function Loader() {
  return (
    <div className="loader-wrap">
      <img src={spinner} alt="" />
    </div>
  );
}
