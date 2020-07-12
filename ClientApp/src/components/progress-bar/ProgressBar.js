import React from "react";
import "./Progressbar.scss";

export default function ProgressBar({ width }) {
  return (
    <div className="progress-bar">
      <div className="indicator" style={{ width: `${width * 100}%` }}></div>
    </div>
  );
}
