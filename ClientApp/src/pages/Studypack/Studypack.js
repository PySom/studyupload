import React, { useContext, useEffect, useState } from "react";
import "./Studypack.scss";
import ProgressBar from "../../components/progress-bar/ProgressBar";
import Nav from "../../components/nav/Nav";
import Lesson from "../../components/lesson/Lesson";
import Quiz from "../../components/quiz/Quiz";
import Loader from "../../components/loader/Loader";
import students from "../../assets/images/students.png";
import backArrow from "../../assets/images/back.svg";
import { withRouter, useParams } from "react-router-dom";
import { userContext } from "../../store/UserContext";
import { retrieveItem } from "../../services/ls";
import authServices from "../../services/authServices";
// import students from "../../assets/images/students.png";

export default withRouter(function Studypack({ history }) {
  const {subject} = useParams();
  const context = useContext(userContext);
  const { userCourses, studyPacks, loading, selectedSubject: {id, ...selectedSubject}, updateLoader } = context;
  console.log({userCourses, studyPacks, selectedSubject})

  const [total, setTotal] = useState();
  const back = () => {
    history.push("/dashboard/");
  };

  useEffect(() => {
    updateLoader(true);
    authServices
      .getQuizCount(id)
      .then((res) => {
        console.log({ testes: res.data });
        setTotal(res.data.score)
        updateLoader(false);
      })
      .catch((err) => {
        console.log({ err });
        updateLoader(false);
      });
  }, [id])

  
  const getUserTests = (id) => {
    const usertests = retrieveItem("usertests");
    console.log({usertests, id})
    if(usertests){
      const ut = usertests.filter((ut) => ut.testId === id);
      console.log({ut})
      return ut;
    }
    return []
  }

  const getTotalScore = () => {
    const usertests = retrieveItem("usertests");
    let score = 0;
    if(usertests){
      score = usertests.reduce((agg, cur) => {
        agg += cur.score;
        return agg;
      }, 0);
    }
    return score;
  }


  const getCourseId = () => {
    if(userCourses){
      const u = userCourses.find(uc => uc.course.name.toLowerCase() === subject.toLowerCase())
      if(u){
        return u.id;
      }
    }
  }

  useEffect(() => {
    console.log(studyPacks);
  }, []);
  return (
    <>
      {loading && <Loader />}
      <div className="studypack">
        <Nav />
        <div className="banner">
          <span className="backArrow">
            <img
              src={backArrow}
              height="25"
              className="mr10"
              alt=""
              onClick={back}
              style={{ position: "relative", top: 5, cursor: "pointer" }}
            />
            Study pack
          </span>
          <span onClick={back} className="mobile-title-text">
            HOME
          </span>
        </div>
        <div className="sub-banner">
          <div className="small">
            <span>Possible points:</span>
            <p>{total}</p>
          </div>
          <div className="wide">
            <div className="progresses">
              <div className="progress-wrap">
                <ProgressBar />
                <span>
                  Accumulated Score
                  ({getTotalScore()} study points)
                </span>
              </div>
            </div>
          </div>
        </div>

        <div className="contents">
          <div className="small">
            <p className="header">Study Pack</p>

            <span className="desc">
              Our Study Packs are advanced tests created to assess your
              proficiency in various subjects and to improve your learning
              progress. <br />
              They can be experienced in three mediums based on your preference.{" "}
              <br />
              <p>- Learn mode</p>
              <p>- Time mode</p>
              <p>- Free form mode.</p>
            </span>
            <img src={students} alt="" />
          </div>
          <div className="wide list">
            {studyPacks &&
              studyPacks.length &&
              studyPacks.map((quiz, i) => {
                return (
                  <Quiz
                    type="study"
                    usercourseid={getCourseId()}
                    userquizzes={getUserTests(quiz.id)}
                    quiz={quiz.id}
                    open={true}
                    quizId={quiz.id}
                    name={`Study pack ${i + 1}`}
                    shortDescription={quiz.shortDescription}
                    quizType="studypack"
                    quizPackDetails={quiz}
                  />
                );
              })}
          </div>
        </div>
      </div>
    </>
  );
});
