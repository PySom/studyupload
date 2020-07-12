import React, { useEffect, useState, useContext } from "react";
import "./Learn.scss";

import Badges from "../../../../components/badges/Badges";
import { motion } from "framer-motion";

import students from "../../../../assets/images/students.png";
import b1 from "../../../../assets/images/b1.svg";
import b2 from "../../../../assets/images/b2.svg";
import b3 from "../../../../assets/images/b3.svg";
import b4 from "../../../../assets/images/b4.svg";
import b5 from "../../../../assets/images/b5.svg";
import coins from "../../../../assets/images/coins.svg";
import LearnIcn from "../../../../assets/images/Learn.svg";
import Progress from "../../../../assets/images/Progress.svg";
import Profile from "../../../../assets/images/Profile.svg";
import Quiz from "../../../../components/quiz/Quiz";
import Courses from "../../../../components/courses/Courses";
import Lesson from "../../../../components/lesson/Lesson";
import Modal from "react-responsive-modal";
import { withRouter } from "react-router-dom";
import { userContext } from "../../../../store/UserContext";
import authServices from "../../../../services/authServices";
import AddCourses from "../../../../components/add-courses/AddCourses";

export default withRouter(function Learn({ history }) {
  const context = useContext(userContext);
  const {
    userCourses,
    subjects,
    user,
    updateUserCourses,
    selectedSubject,
    updateLoader,
  } = context;

  const [open, setopen] = useState(false);
  const [verified, setverified] = useState(true);

  useEffect(() => {
    !userCourses.length && setopen(true);
  }, []);

  const onOpenModal = () => {
    setopen(true);
  };

  const onCloseModal = () => setopen(false);

  const toSubject = () => {
    if (!userCourses.length) return;
    console.log({ selectedSubject });
    history.push(`/subject/${selectedSubject.id}`);
  };

  const toMobileCourses = () => {
    history.push("/dashboard/mobile-courses");
  };

  const toEdit = () => history.push("/edit-profile");

  const toProgress = () => history.push("/dashboard/progress");
  const toProfile = () => history.push("/dashboard/profile");

  return (
    <motion.div
      className="learn"
      initial={{ opacity: 0, x: "-5vw" }}
      animate={{ opacity: 1, x: "0vw" }}
      exit={{ opacity: 0, x: 0 }}
      transition={{ duration: 0.6 }}
    >
      <div className="mobile-screen">
        <div className="medals">
          <div className="medal">
            <img src={coins} alt="" />
            <span>0</span>
          </div>
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
        <div className="grid">
          <div className="half" onClick={toMobileCourses}>
            <img src={LearnIcn} alt="" />
            <span>
              Start
              <p>Learning</p>
            </span>
          </div>
          <div className="halff">
            <div className="qtr tt" onClick={toProgress}>
              <img src={Progress} alt="" />
              <span>Progress</span>
            </div>
            <div className="qtr bb" onClick={toProfile}>
              <img src={Profile} alt="" />
              <span>My Profile</span>
            </div>
          </div>
        </div>
        <p className="foot">
          2020 All right reserved. StudyMate. Powered by infomal Nigeria
        </p>
      </div>
      {!user.isVerified && (
        <div className="verify">
          <span>
            Please edit your profile to complete your registration within a week
            from registration.
          </span>
          <button onClick={toEdit}>EDIT PROFILE</button>
        </div>
      )}
      {userCourses.length ? (
        <div className="wide-side">
          <p className="heading">{selectedSubject.name}</p>
          <div className="lessons-wrap">
            <p className="sub-heading">
                          Lessons
              <button 
                   className="tw-btn f-right"
                    disabled={(selectedSubject &&
                        selectedSubject.tests &&
                        selectedSubject.tests.length) ? "" : "disbled"}
                   onClick={toSubject}>
                Start learning
              </button>
            </p>

            <div className="lessons">
              {selectedSubject &&
              selectedSubject.tests &&
              selectedSubject.tests.length ? (
                selectedSubject.tests[0].videos
                  .filter((v, i) => i < 4)
                  .map((video, i) => (
                    <Lesson
                      key={`lesson${i}`}
                      video={video}
                      disableClick={true}
                    />
                  ))
              ) : (
                <p
                  className="blue--text"
                  style={{
                    paddingTop: "30px",
                    textAlign: "center",
                  }}
                >
                  {userCourses.length
                    ? "Select a course to get started." 
                    : "There are currently no videos in this study pack. Kindly check back later" }
                </p>
              )}
            </div>
          </div>

          <div className="quizzes mt30" onClick={toSubject}>
            {selectedSubject &&
            selectedSubject.quizzes &&
            selectedSubject.quizzes.length ? (
              <Quiz
                open={false}
                quiz={selectedSubject.quizzes}
                quizType="normal"
              />
            ) : null}
          </div>
        </div>
      ) : (
        <div
          className="wide-side"
          style={{
            background: "#fff",
            height: 400,
            textAlign: "center",
            paddingTop: 130,
          }}
        >
          <p className="blue--text">Select a course to start learning</p>
        </div>
      )}
      <div className="narrow-side">
        <div className="badge-wrap">
          <Badges />
        </div>

        <div className="courses">
          <p className="title mt30">Courses</p>
          <Courses onOpenModal={onOpenModal} className="mt20" />
        </div>

        <img src={students} className="students" alt="" />

        <AddCourses open={open} onCloseModal={onCloseModal} />
      </div>
    </motion.div>
  );
});
