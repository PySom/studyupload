import React, { useContext, useState } from "react";
import "./Subject.scss";
import ProgressBar from "../../components/progress-bar/ProgressBar";
import Nav from "../../components/nav/Nav";
import Loader from "../../components/loader/Loader";
import Lesson from "../../components/lesson/Lesson";
import Quiz from "../../components/quiz/Quiz";
import students from "../../assets/images/students.png";
import dots from "../../assets/images/Dots.svg";
import backArrow from "../../assets/images/back.svg";
import { withRouter } from "react-router-dom";
import { userContext } from "../../store/UserContext";
import authServices from "../../services/authServices";
import { useEffect } from "react";
import { saveItem } from "../../services/ls";
// import students from "../../assets/images/students.png";

export default withRouter(function Subject({ history }) {
  const context = useContext(userContext);
  const {
    selectedSubject,
    updateLoader,
    updateStudyPack,
    loading,
    user,
  } = context;
  const [usertests, setusertests] = useState(null);
  const [linkIndex, setlinkIndex] = useState(0);
  const [testId, settestId] = useState(0);
  const [usercourseid, setusercourseid] = useState(0);
  const [pageLoaded, setpageLoaded] = useState(false);
  const [links, setlinks] = useState([
    { text: "Beginner" },
    { text: "Intermediate" },
    { text: "Advanced" },
  ]);

  useEffect(() => {
    updateLoader(true);
    authServices
      .getUcourseWithTests(user.id, selectedSubject.id)
      .then((res) => {
        console.log({ testes: res.data });
        const defaultTestId =
          selectedSubject &&
          selectedSubject.tests &&
          selectedSubject.tests.length
            ? selectedSubject.tests[0].id
            : 0;
        settestId(defaultTestId);
        setusercourseid(res.data.id);
        setusertests(res.data.userTests);
        updateLoader(false);
        setpageLoaded(true);
      })
      .catch((err) => {
        console.log({ err });
        updateLoader(false);
      });
    console.log(selectedSubject);
  }, [pageLoaded]);

  const back = () => {
    history.push("/dashboard/");
  };

  const gotoPacks = () => {
    console.log(selectedSubject);
    updateLoader(true);
    authServices
      .getCourseByName(selectedSubject.name)
      .then((res) => {
        console.log(res);
        updateStudyPack(res.data.tests);
        updateLoader(false);
        saveItem("usertests", usertests)
        history.push(`/studypack/${selectedSubject.name}`);
      })
      .catch((err) => {
        console.log({ err });
        updateLoader(false);
      });
  };

  const toCourses = () => {
    history.push(`/dashboard/mobile-courses`);
  };

  const pickLevel = (i) => {
    setlinkIndex(i);
  };

  const generateLevelTest = (id, tests) => {
    const test = tests.find((t) => t.id === id);
    console.log({ test });
    if (test) {
      const beginnerQuiz = test.quizes.filter((q) => q.level === 0);
      const intermediateQuiz = test.quizes.filter((q) => q.level === 1);
      const advancedQuiz = test.quizes.filter((q) => q.level === 2);
      const fourQuiz = test.quizes.filter((q) => q.level === 3);
      const fiveQuiz = test.quizes.filter((q) => q.level === 4);
      const sixQuiz = test.quizes.filter((q) => q.level === 5);
      const sevenQuiz = test.quizes.filter((q) => q.level === 6);
      const eightQuiz = test.quizes.filter((q) => q.level === 7);
      const nineQuiz = test.quizes.filter((q) => q.level === 8);
      const tenQuiz = test.quizes.filter((q) => q.level === 8);

      const beginnerVideo = test.videos.filter((q) => q.level === 0);
      const intermediateVideo = test.videos.filter((q) => q.level === 1);
      const advancedVideo = test.videos.filter((q) => q.level === 2);
      const fourVideo = test.videos.filter((q) => q.level === 3);
      const fiveVideo = test.videos.filter((q) => q.level === 4);
      const sixVideo = test.videos.filter((q) => q.level === 5);
      const sevenVideo = test.videos.filter((q) => q.level === 6);
      const eightVideo = test.videos.filter((q) => q.level === 7);
      const nineVideo = test.videos.filter((q) => q.level === 8);
      const tenVideo = test.videos.filter((q) => q.level === 9);

      const returnCandidate = [
        {
          level: 0,
          id: test.id,
          take: !!beginnerVideo.length,
          quizzes: beginnerQuiz,
          videos: beginnerVideo,
        },
        {
          level: 1,
          id: test.id,
          take: !!intermediateVideo.length,
          quizzes: intermediateQuiz,
          videos: intermediateVideo,
        },
        {
          level: 2,
          id: test.id,
          take: !!advancedVideo.length,
          quizzes: advancedQuiz,
          videos: advancedVideo,
        },
        {
          level: 3,
          id: test.id,
          take: !!fourVideo.length,
          quizzes: fourQuiz,
          videos: fourVideo,
        },
        {
          level: 4,
          id: test.id,
          take: !!fiveVideo.length,
          quizzes: fiveQuiz,
          videos: fiveVideo,
        },
        {
          level: 5,
          id: test.id,
          take: !!sixVideo.length,
          quizzes: sixQuiz,
          videos: sixVideo,
        },
        {
          level: 6,
          id: test.id,
          take: !!sevenVideo.length,
          quizzes: sevenQuiz,
          videos: sevenVideo,
        },
        {
          level: 7,
          id: test.id,
          take: !!eightVideo.length,
          quizzes: eightQuiz,
          videos: eightVideo,
        },
        {
          level: 8,
          id: test.id,
          take: !!nineVideo.length,
          quizzes: nineQuiz,
          videos: nineVideo,
        },
        {
          level: 9,
          id: test.id,
          take: !!tenVideo.length,
          quizzes: tenQuiz,
          videos: tenVideo,
        },
      ];

      console.log({ returnCandidate });

      return returnCandidate.filter((r) => r.take);
    }
    return [];

    
  };

  const getPreviousQuiz = (usertest, quizzes, level) => {
    console.log({level})
    if (usertest && quizzes) {
      const intersection = usertest.userQuizzes.filter((value) =>
        quizzes.some((q) => q.id === value.quizId && q.level === level)
      );
      console.log({quizzes, ut: usertest.userQuizzes})
      return intersection;
    }
    return [];
  };

  const getUserVideos = (data) => {
    if(data && data.length){
      return data[0].userVideos
    }
    return []
  }

  const totalScore = (tests) => {
    if(tests){
      const score = tests.filter(t => t.studyType === 1).reduce((agg, cur) => {
        agg += cur.quizes.length
        return agg;
      }, 0)
      return score;
    }
    return 0;
  }
  

  return (
    <>
      {loading && <Loader />}
      <div className="subject">
        <Nav />
        <div className="banner">
          <span onClick={toCourses} className="mobile-title-text">
            Courses
          </span>
          <span className="back" style={{ textTransform: "capitalize" }}>
            <img
              src={backArrow}
              height="25"
              className="mr10"
              alt=""
              onClick={back}
              style={{ position: "relative", top: 5, cursor: "pointer" }}
            />
            {selectedSubject.name}
          </span>
        </div>
        <div className="sub-banner">
          <div className="small">
            <span>Possible points:</span>
            <p>{totalScore(selectedSubject.tests)}</p>
          </div>
          <div className="wide">
            <div className="progresses">
              {(usertests &&
                usertests.length) ?
                usertests.map((utest) => {
                  return (
                    <div key={utest.id} className="progress-wrap">
                      <ProgressBar />
                      <span>
                        {selectedSubject.tests && selectedSubject.tests.find(
                          (t) => t.id == utest.testId
                        ) &&
                          selectedSubject.tests.find(
                            (t) => t.id == utest.testId
                          ).year}
                        ({utest.score} study points)
                      </span>
                    </div>
                  );
                }): <span>Start quizzes to acquire points</span>}
            </div>
          </div>
        </div>

        <div className="contents">
          <div className="small">
            <p className="header">Study Levels</p>
            {selectedSubject &&
              selectedSubject.tests &&
              selectedSubject.tests.map((test, i) => (
                <div
                  key={test.id}
                  onClick={() => settestId(test.id)}
                  className={`level ${testId === test.id ? "active" : ""}`}
                >
                  <div className="band">
                    <div className="inner"></div>
                  </div>
                  <span>{test.year}</span>
                </div>
              ))}

            <img src={students} alt="" />
            <img
              src={dots}
              alt=""
              style={{
                width: 60,
                float: "left",
                position: "relative",
                top: 230,
                left: "-250px",
              }}
            />
          </div>
          <div className="wide big">
            {selectedSubject &&
              selectedSubject.tests &&
              generateLevelTest(testId, selectedSubject.tests).map(
                (test, i) => (
                  <React.Fragment key={"lesson_" + i}>
                    <p className="heading">Lessons</p>
                    <div className="lessons-wrap mb30">
                      <div className="lessons">
                        {test.videos.length ? (
                          test.videos.map((video, i) => (
                            <Lesson
                              usertests={
                                usertests && usertests.length
                                  ? usertests.filter((ut) => ut.testId === test.id)
                                  : []
                              }
                              uservideos={
                                getUserVideos(
                                  (usertests && usertests.length)
                                    && usertests.filter((ut) => ut.testId === test.id)
                                )
                              }
                              key={"video" + video.id + "" + i}
                              usercourseid={usercourseid}
                              video={video}
                              grade={linkIndex}
                              disableClick={false}
                            />
                          ))
                        ) : (
                          <div
                            className="blue--text"
                            style={{
                              padding: "12px 30px",
                              margin: 0,
                              fontSize: 12,
                            }}
                          >
                            There are no video lessons in this pack. Kindly
                            check back later.
                          </div>
                        )}
                      </div>
                    </div>
                    {test.quizzes.length ? (
                      <Quiz
                        open={true}
                        usertests={
                          usertests && usertests.length
                            ? usertests.filter((ut) => ut.testId === test.id)
                            : []
                        }
                        userquizzes={getPreviousQuiz(
                          usertests &&
                            usertests.find((ut) => ut.testId === test.id),
                          test.quizzes, test.level
                        )}
                        type="main"
                        usercourseid={usercourseid}
                        quizType="normal"
                        quiz={test.quizzes}
                      />
                    ) : null}
                  </React.Fragment>
                )
              )}

            {selectedSubject.hasStudyPack && (
              <div className="pack">
                <div className="half">
                  <p className="title">Advance your learning</p>
                  <div className="desc">
                    <p style={{ fontSize: 12 }}>
                      Our Study Packs are test questions created for advanced
                      Senior Secondary levels. They are simulated for exam
                      purposes and further learning.
                    </p>
                    <p style={{ fontSize: 12 }}>
                      Note: The Lesson Packs above must have been completed
                      before proceeding to the Study Packs.
                    </p>
                  </div>
                  <div className="duration">
                    <span>Approximately 12 Study packs</span>
                  </div>
                  <button onClick={gotoPacks} className="tw-btn">
                    Lets get started
                  </button>
                </div>
                <div className="half bg"></div>
              </div>
            )}
          </div>
          <div className="wide narrow">
            {selectedSubject &&
              selectedSubject.tests &&
              selectedSubject.tests.map((test, i) => (
                <React.Fragment key={"lesson_" + i}>
                  <p className="heading">{test.year}</p>
                  <div className="lessons-wrap mb30">
                    <div className="lessons">
                      {test.videos.length ? (
                        test.videos.map((video, i) => (
                          <Lesson
                            usertests={
                              usertests && usertests.length
                                ? usertests.filter((ut) => ut.testId === test.id)
                                : []
                            }
                            uservideos={
                              getUserVideos(
                                (usertests && usertests.length)
                                  && usertests.filter((ut) => ut.testId === test.id)
                              )
                            }
                            key={"video" + video.id + "" + i}
                            usercourseid={usercourseid}
                            video={video}
                            grade={linkIndex}
                            disableClick={false}
                          />
                        ))
                      ) : (
                        <div
                          className="blue--text"
                          style={{
                            padding: "12px 30px",
                            margin: 0,
                            fontSize: 12,
                          }}
                        >
                          There are no video lessons in this pack. Kindly check
                          back later.
                        </div>
                      )}
                    </div>
                  </div>
                  {test.quizes.length ? (
                    <Quiz
                      open={true}
                      usertests={
                        usertests && usertests.length
                          ? usertests.filter((ut) => ut.testId === test.id)
                          : []
                      }
                      userquizzes={getPreviousQuiz(
                        usertests &&
                          usertests.find((ut) => ut.testId === test.id),
                        test.quizzes
                      )}
                      type="main"
                      usercourseid={usercourseid}
                      quizType="normal"
                      quiz={test.quizzes}
                    />
                  ) : null}
                </React.Fragment>
              ))}

            {selectedSubject.hasStudyPack && (
              <div className="pack">
                <div className="half">
                  <p className="title">Advance your learning</p>
                  <div className="desc">
                    <p style={{ fontSize: 12 }}>
                      Our Study Packs are test questions created for advanced
                      Senior Secondary levels. They are simulated for exam
                      purposes and further learning.
                    </p>
                    <p style={{ fontSize: 12 }}>
                      Note: The Lesson Packs above must have been completed
                      before proceeding to the Study Packs.
                    </p>
                  </div>
                  <div className="duration">
                    <span>Approximately 12 Study packs</span>
                  </div>
                  <button onClick={gotoPacks} className="tw-btn">
                    Lets get started
                  </button>
                </div>
                <div className="half bg"></div>
              </div>
            )}
          </div>
        </div>
      </div>
    </>
  );
});
