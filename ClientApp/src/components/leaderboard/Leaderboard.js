import React, { useEffect, useContext, useState } from "react";
import "./Leaderboard.scss";

import b1 from "../../assets/images/b1.svg";
import b2 from "../../assets/images/b2.svg";
import b3 from "../../assets/images/b3.svg";
import best from "../../assets/images/Best.svg";
import { userContext } from "../../store/UserContext";
import authServices from "../../services/authServices";
import { appUrl } from "../../services/urls";



export default function Leaderboard() {
  const { loading, user: {id, ...user}, updateLoader } = useContext(userContext);
  const [leaderBoards, setLeaderBoards] = useState([]);

  const getClass = (index, uid) => {
    if(uid === id) return "mine"
    else if(index === 0) return "first"
    else if(index === 1) return "second"
    else if(index === 2) return "third"
    return "others"
  }

  const getImage = (pos) => {
    if(pos === 1) return b1;
    if(pos === 2) return b2;
    if(pos === 3) return b3;
  }

  useEffect(() => {
    updateLoader(true);
    authServices
      .getLeaderboard(id)
      .then((res) => {
        console.log({ testes: res.data });
        setLeaderBoards(res.data)
        updateLoader(false);
      })
      .catch((err) => {
        console.log({ err });
        updateLoader(false);
      });
  }, [id])
  return (
    <div className="leaderboard">
      <p className="heading">Leaderboard</p>
      <div className="people">
        
        {leaderBoards.map((leader, index) => (
          <>
          {leader.id === id && <p className="my">My position</p>}
          <div className={`person ${getClass(index, leader.id)}`}>
            {leader.position <= 3 && <img src={getImage(leader.position)} alt="" />}
            {leader.position > 3 && <span className="position">{leader.position}</span>}
            <img
              className="photo"
              src={`${appUrl}${leader.userImg}`}
              alt=""
            />
            <span className="name">{leader.name}</span>

            <span className="points">{leader.score} pts.</span>
            <img src={best} className="best" alt="" />
          </div>
          </>
          
        ))}
        
      </div>
    </div>
  );
}
