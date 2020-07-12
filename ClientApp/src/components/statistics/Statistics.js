import React, { useEffect, useState } from "react";
import { Doughnut } from "react-chartjs-2";
import "./Statistics.scss";

export default function Statistics({first, second, third, ...props}){
  const [data, setData] = useState()
  useEffect(() => {
    first = Number(first)
    second = Number(second)
    third = Number(third)
    const total = first + second + third;
    let firstPercent = 0, secondPercent = 0, thirdPercent = 0;
    if(total > 0){
      firstPercent = (first / total) * 100
      secondPercent = (second / total) * 100
      thirdPercent = (third / total) * 100
    }
    console.log({first, second, third, firstPercent, secondPercent, thirdPercent})

    const data = {
      labels: ["one", "two", "three"],
      datasets: [
        {
          label: "# of Votes",
          data: [firstPercent, secondPercent, thirdPercent],
          backgroundColor: [
            "rgb(0, 41, 132)",
            "rgb(16, 163, 255)",
            "rgb(247, 174, 74)",
          ],
          borderWidth: 0,
        },
      ],
    };
    const options = {
      legend: {
        display: false,
      },
      tooltips: {
        enabled: false,
      },
    };

    setData({data, options})
  }, [first, second, third]);
  console.log({first, second, third})
  return data ? (<div className="statistics" style={{ padding: "15px" }}>
      <Doughnut data={data.data} options={data.options} />
      <div className="legends">
        <div className="legend vids">
          <p>
            <span
              style={{
                backgroundColor: data.data.datasets[0]
                  .backgroundColor[0],
              }}
            ></span>{" "}
            Completed Videos
          </p>
          <span>{first}%</span>
        </div>
        <div className="legend left">
          <p>
            <span
              style={{
                backgroundColor: data.data.datasets[0]
                  .backgroundColor[1],
              }}
            ></span>{" "}
            Quiz
          </p>
          <span>{second}%</span>
        </div>
        <div className="legend left">
          <p>
            <span
              style={{
                backgroundColor: data.data.datasets[0]
                  .backgroundColor[2],
              }}
            ></span>{" "}
            Mastery
          </p>
          <span>{third}%</span>
        </div>
      </div>
    </div>
) 
: <span>No data yet</span>
}