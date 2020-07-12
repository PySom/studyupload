import React, { useState } from "react";
import "./Days.scss";
export default function Days(props) {
  const [days, setdays] = useState([
    { day: "Mon", came: true, rep: 1 },
    { day: "Tue", came: true, rep: 2 },
    { day: "Wed", came: false, rep: 3 },
    { day: "Thu", came: false, rep: 4 },
    { day: "Fri", came: false, rep: 5 },
    { day: "Sat", came: false, rep: 6 },
    { day: "Sun", came: false, rep: 0 },
  ]);
  return (
    <div className="days">
      {days.map((day) => (
        <div className="day" key={day.day}>
          <div className={`ball ${props.days && props.days.some(d => d === day.rep) ? "present" : ""}`}></div>
          <span className={props.days && props.days.some(d => d === day.rep) ? "came" : ""}>{day.day}</span>
        </div>
      ))}
    </div>
  );
}
