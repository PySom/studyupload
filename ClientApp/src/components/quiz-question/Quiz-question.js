import React, { useState, useEffect, useContext } from "react";
import "./Quiz-question.scss";
import ProgressBar from "../progress-bar/ProgressBar";
import close from "../../assets/images/close.svg";
import streak from "../../assets/images/repeat.svg";
import Passage from "../../assets/images/Passage.svg";
import caret from "../../assets/images/down-arrow.svg";
import { appUrl, audioUrl } from "../../services/urls";
import { userContext } from "../../store/UserContext";
import beep from "../../assets/audio/beep1.mp3";

import MathJax from "react-mathjax";
import Parser from "../content-display/Parser";
import useStudy from "../../custom-hooks/usestudy";
import authServices from "../../services/authServices";
import Loader from "../loader/Loader";

export default function QuizQuestion(props) {
  const { loading, updateLoader } = useContext(userContext);
  let hr = props.time ? parseFloat(props.time.hour) * 60 : 0;
  let min = parseFloat(props.time.minutes);
  let duration_ = (hr + min) * 60;

  const [passage, setpassage] = useState(false);
  const [beepSound, setBeepSound] = useState(true);
  const [feedback, setFeedback] = useState(false);
  const [answered, setanswered] = useState(false);
  const [wrongAnswer, setwrongAnswer] = useState(false);
  const [currentQuestion, setcurrentQuestion] = useState(0);
  const [timeUp, setTimeUp] = useState(false);
  const [tryAttempt, settryAttempt] = useState(false);
  const [duration, setduration] = useState(duration_);
  const [answers, setanswers] = useState([]);
  const [timeAnswers, settimeAnswers] = useState(1);
  const [buttonText, setbuttonText] = useState("TRY AGAIN");
  const [remains, setremains] = useState({ hr: 0, min: 0, sec: 0 });
  const [optionSelected, setoptionSelected] = useState(false);
  const [showAlert, setshowAlert] = useState(true);
  const [stopper, setStopper] = useState(0);
  const [readInstructions, setreadInstructions] = useState(false);
  //const [showExplanation, setshowExplanation] = useState(false);
  const { selectedQuizMode, questions } = props;
  let activeQuestion = questions[currentQuestion];

  const getThisTestForUser = () => {
    if (props.usertests) {
      const data = props.usertests.find(
        (ut) => ut.testId === activeQuestion.testId
      );
      if (data) return data.userQuizzes;
    }
    return [];
  };
  console.log({ dataIn: props.userquizzes });
  const {
    userScore,
    updateThisAnswer,
    userAnswers,
    userAnswersToAdd,
    userAnswersToUpdate,
    answer,
    onTryAgain,
    updateAnswers,
    setShowAlert,
    hideShowAlert,
    willTryAgain,
    willSubmit,
    willMoveNext,
    willShowAlert,
    answerIsCorrect,
    lockState,
    correctClassName,
    wrongClassName,
    selectedClassName,
    optionIsCorrect,
    userAnswersCount,
    showExplanation,
    lockAllAnswers,
    lockThisAnswer,
  } = useStudy(
    props.userquizzes || [],
    activeQuestion && activeQuestion.id,
    props.selectedQuizMode
  );

  console.log(selectedQuizMode)

  console.log({ userAnswersToAdd, userAnswersToUpdate });
  //const qid = activeQuestion && activeQuestion.quizId;
  const uid = answer ? answer.userOptionId : 0;

  const [userOption, setuserOption] = useState(uid);
  console.log({ usercid: props.usercourseid, tests: props.usertests });
  useEffect(() => {
    if (selectedQuizMode === "Learn Mode") {
      console.log("beep");
      setBeepSound(true);
    }
  }, [beepSound]);

  const modeConverter = () => {
    if (selectedQuizMode === "Learn Mode") return 0;
    else if (selectedQuizMode === "Time Mode") return 1;
    else if (selectedQuizMode === "Free Form Mode") return 2;
  };

  const sendUserAnswersToStore = (callback) => {
    //add a user's test and then add the courses
    //check if user answered
    if (userAnswersToAdd.length) {
      console.log("quiz commenced adding...");
      let userTestId = 0;
      if (props.usertests && props.usertests.length) {
        userTestId = props.usertests[0].id;
      }
      if (userTestId) {
        const addAnswers = userAnswersToAdd.map((ua) => ({
          ...ua,
          userOption: ua.userOptionId,
          mode: modeConverter(),
          correctOption: ua.correctOptionId,
          userTestId,
        }));
        updateLoader(true);
        sendUserQuizToServer(addAnswers, callback, false);
      } else {
        const data = {
          userCourseId: props.usercourseid,
          testId: activeQuestion.testId,
          currentLevel: activeQuestion.level,
          score: selectedQuizMode === "Free Form Mode" ? userScore : 0,
        };
        updateLoader(true);
        sendUserTestToServer(data, callback);
      }
    } else {
      console.log("quiz nothing to add");
      callback();
    }
    if(userAnswersToUpdate.length){
      const userTestId = props.usertests[0].id;
      if(userTestId){
        const data = [
          {
            value: userScore,
            op: "add",
            path: "/score",
          },
        ];
        updateLoader(true);
        authServices
          .updateUserTestData(data, userTestId)
          .then((res) => {
            console.log({ usertest: res.data, mode: modeConverter() });

            const data = userAnswersToAdd.map((ua) => ({
              ...ua,
              userOption: ua.userOptionId,
              correctOption: ua.correctOptionId,
              userTestId: res.data.id,
            }));
            updateUserQuizInServer(data, callback, false);
          })
          .catch((err) => {
            console.log({ err });
            callback();
            updateLoader(false);
          });

        console.log("user test id", userTestId)
      }
      console.log({userScore});
    } else {
      console.log("quiz nothing to update");
      callback();
    }
  };

  const sendUserTestToServer = (data, callback) => {
    authServices
      .addUserTest(data)
      .then((res) => {
        console.log({ usertest: res.data, mode: modeConverter() });

        const data = userAnswersToAdd.map((ua) => ({
          ...ua,
          userOption: ua.userOptionId,
          mode: modeConverter(),
          correctOption: ua.correctOptionId,
          userTestId: res.data.id,
        }));
        sendUserQuizToServer(data, callback, false);
      })
      .catch((err) => {
        console.log({ err });
        callback();
        updateLoader(false);
      });
  };

  const sendUserQuizToServer = (data, callback, loadstate) => {
    authServices
      .addMultipleUserQuizzes(data)
      .then((res) => {
        console.log({ usertest: res.data });
        callback();
        updateLoader(loadstate);
      })
      .catch((err) => {
        console.log({ err });
        callback();
        updateLoader(loadstate);
      });
  };

  const updateUserQuizInServer = (data, callback, loadstate) => {
    authServices
      .updateMultipleUserQuiz(data)
      .then((res) => {
        console.log({ usertestupdate: res.data });
        callback();
        updateLoader(loadstate);
      })
      .catch((err) => {
        console.log({ err });
        callback();
        updateLoader(loadstate);
      });
  };

  // console.log({ myquestion: questions });
  const [options, setoptions] = useState([]);
  useEffect(() => {
    getOptions(0);

    if (selectedQuizMode === "Time Mode") {
      // console.log({ effect: { userScore, userAnswers } });
      const timer = window.setInterval(() => {
        const prevTime = duration;
        // console.log({ prevTime });
        if (prevTime <= 0) {
          console.log("to clear interval");
          window.clearInterval(timer);
          submit();
        } else {
          // console.log("to clear intervalss");
          let dur = prevTime - 1;
          let hoursRemaining = dur > 3600 ? Math.floor(dur / 3600) : 0;
          let minutesRemaining = Math.floor(dur / 60 - 60 * hoursRemaining);
          let secondsRemaining =
            dur - (hoursRemaining * 3600 + minutesRemaining * 60);
          let data = {
            hr: hoursRemaining,
            min:
              minutesRemaining >= 10
                ? minutesRemaining
                : `0${minutesRemaining}`,
            sec:
              secondsRemaining >= 10
                ? secondsRemaining
                : `0${secondsRemaining}`,
          };
          setremains(data);
          // console.log({ dur });
          setduration(dur);
          setStopper(() => stopper + 1);
        }
      }, 1000);
      return () => {
        window.clearInterval(timer);
      };
    }
  }, [stopper]);

  const nextQuestion = () => {
    let no = currentQuestion + 1;
    updateAnswers();
    if (no >= questions.length) {
      submit();
      return;
    }
    setcurrentQuestion(no);
    setanswered(false);
    setwrongAnswer(false);
    setoptionSelected(false);
    settryAttempt(false);
    setshowAlert(false);
    setbuttonText("TRY AGAIN");
    setreadInstructions(false);
    // console.log({ activeQuestion });
  };

  const prevQuestion = () => {
    let no = currentQuestion - 1;
    updateAnswers();
    setcurrentQuestion(no);
    getOptions(no);
    setanswered(false);
    setwrongAnswer(false);
    setoptionSelected(false);
    setreadInstructions(false);
  };

  const pickAnswerOne = (i) => {
    setuserOption(i);
    setoptionSelected(true);
    if (selectedQuizMode !== "Learn Mode") {
      updateThisAnswer({
        quizId: activeQuestion.id,
        userOptionId: i,
        mode: modeConverter(),
        correctOptionId: activeQuestion.answerId,
        alert: true,
      });
    }
  };

  const openPassage = () => {
    setpassage(!passage);
    setFeedback(!setFeedback);
  };

  const openFeedback = () => {
    setpassage(false);
    setFeedback(!feedback);
  };

  const getOptions = (no) => {
    console.log({ no });
    if (no >= questions.length) {
      submit();
      return;
    }
  };

  const submit = () => {
    console.log("Submitting");
    sendUserAnswersToStore(submitAction);
  };

  const submitAction = () => {
    let score = userScore;
    console.log({ userScore, userAnswers });
    let count = questions.length;
    let percent = (score * 100) / count;
    //setcurrentQuestion(0);
    props.completeTest({ score, percent, count });
  };

  const tryAgain = () => {
    if (buttonText === "TRY AGAIN") {
      setbuttonText("SUBMIT");
      setuserOption(0);
      settryAttempt(false);
      onTryAgain({ userOptionId: 0, alert: false });
      return;
    } else {
      submitLearingAnswer();
    }
  };

  const submitLearingAnswer = () => {
    if (userOption) {
      setoptionSelected(false);
      settryAttempt(true);
      updateThisAnswer({
        quizId: activeQuestion.id,
        userOptionId: userOption,
        mode: modeConverter(),
        correctOption: activeQuestion.answerId,
        userOption: userOption,
        correctOptionId: activeQuestion.answerId,
        alert: true,
      });
      setoptions(
        options.map((option, j) => {
          return userOption === j
            ? {
                ...option,
                picked: true,
              }
            : {
                ...option,
                picked: false,
              };
        })
      );
    }
  };

  if (
    answerIsCorrect &&
    selectedQuizMode === "Learn Mode" &&
    beepSound &&
    !lockState
  ) {
    const snd = new Audio(beep);
    snd.play();
    setBeepSound(false);
  }

  const viewExplanation = () => {
    showExplanation();
  };

  const setImageUrl = (str) => {
    if (!str) return;
    return str
      .replace(`<img src='assets`, `<img src='${appUrl}/assets`)
      .replace(`<img src="assets`, `<img src="${appUrl}/assets`)
      .replace("\\", "/");
  };

  if (duration <= 0 && selectedQuizMode === "Time Mode") {
    console.log("inside life");
    submit();
  }

  const onClose = () => {
    props.onSaveProgress(() => sendUserAnswersToStore(() => {}));
    props.onClose();
  };

  return (
    <>
      {loading && <Loader />}
      {activeQuestion && (
        <div className="quiz-quuestion">
          <span className="close">
            <img src={close} alt="" onClick={onClose} />
          </span>
          <div className="upper">
            <div className="upper-bar">
              {
                ((activeQuestion.isFirstSection && readInstructions) ||
                  !activeQuestion.isFirstSection) && (
                  // (selectedQuizMode === "Time Mode" && (
                  <div className="instruction">
                    <p>
                      {activeQuestion && activeQuestion.section ? (
                        <span
                          style={{ maxWidth: "80%" }}
                          dangerouslySetInnerHTML={{
                            __html: setImageUrl(activeQuestion.section),
                          }}
                        ></span>
                      ) : (
                        <span>&nbsp;</span>
                      )}{" "}
                      {selectedQuizMode === "Time Mode" && (
                        <span className="time">
                          {remains.hr}: {remains.min}: {remains.sec}
                        </span>
                      )}
                    </p>
                  </div>
                )
                // ))
              }
              {answer && answer.alert && selectedQuizMode === "Learn Mode" && (
                <div className="alert">
                  <img src={streak} className="badge" alt="" />
                  <div className="streak-text">
                    <p className="top">
                      {answerIsCorrect ? "Excellent." : "Not quite yet ..."}
                      <img
                        src={close}
                        alt=""
                        onClick={() => hideShowAlert()}
                        className="ex"
                      />
                    </p>
                    <p>
                      {answerIsCorrect
                        ? "Great work! Well done."
                        : "Your answer is incorrect."}
                    </p>
                    <p>
                      <span className="blue--text" onClick={viewExplanation}>
                        View explanation
                      </span>{" "}
                      or{" "}
                      <span className="blue--text" onClick={nextQuestion}>
                        Move on
                      </span>
                    </p>
                  </div>
                </div>
              )}
              <ProgressBar width={userAnswersCount / (questions.length - 1)} />
            </div>
            {activeQuestion.isFirstSection && !readInstructions ? (
              <>
                <div className="content content_">
                  <Parser
                    isMathJax={activeQuestion.isQuestionMathJax}
                    question={activeQuestion.section}
                  />
                </div>
                <div className="passage passage_" id="passage">
                  <img src={Passage} alt="" />
                </div>
              </>
            ) : (
              <>
                <div className="content">
                  <Parser
                    className="question"
                    isMathJax={activeQuestion.isQuestionMathJax}
                    question={activeQuestion.question}
                  />

                  <div className="questions">
                    {activeQuestion.options.map((option, i) => {
                      return (
                        <div
                          key={option.text}
                          className={`answer
                              ${
                                ((answer && lockState) || tryAttempt) &&
                                selectedQuizMode === "Learn Mode"
                                  ? "lock"
                                  : ""
                              }
                              ${
                                answer &&
                                (answer.userOptionId === option.id ||
                                  userOption === option.id)
                                  ? "correct_"
                                  : ""
                              }
                              ${
                                answer &&
                                correctClassName(option.id) &&
                                (!!answer.userOptionId || userOption) &&
                                !optionSelected &&
                                selectedQuizMode === "Learn Mode"
                                  ? "correct"
                                  : ""
                              }
                              ${
                                answer &&
                                wrongClassName(option.id) &&
                                (!!answer.userOptionId || userOption) &&
                                !optionSelected &&
                                selectedQuizMode === "Learn Mode"
                                  ? "wrong"
                                  : ""
                              }
                            `}
                          onClick={() =>
                            ((answer && lockState) || tryAttempt) &&
                            selectedQuizMode === "Learn Mode"
                              ? () => {}
                              : pickAnswerOne(option.id)
                          }
                        >
                          <span className="label">
                            {String.fromCharCode(65 + i)}
                          </span>

                          <p>
                            <Parser
                              className=""
                              inline={true}
                              isMathJax={option.isMathJax}
                              question={option.content}
                            />
                            <br />
                            {answer &&
                              answer.showExplanation &&
                              optionIsCorrect(option.id) &&
                              selectedQuizMode === "Learn Mode" && (
                                <p
                                  style={{
                                    fontSize: 14,
                                    color: "#afafaf",
                                    position: "relative",
                                    top: "10px",
                                    marginLeft: 0,
                                  }}
                                >
                                  <p style={{ margin: "10px 0 20px" }}>
                                    <Parser
                                      className=""
                                      inline={true}
                                      isMathJax={
                                        activeQuestion.answerUrl &&
                                        activeQuestion.answerUrl.includes("\\(")
                                      }
                                      question={activeQuestion.answerUrl}
                                    />
                                  </p>
                                  <audio controls controlsList="nodownload">
                                    <source
                                      src={`${audioUrl}${
                                        activeQuestion.audioUrl &&
                                        activeQuestion.audioUrl.replace(
                                          "\\",
                                          "/"
                                        )
                                      }`}
                                      type="audio/mpeg"
                                    />
                                  </audio>
                                </p>
                              )}
                          </p>
                          {/* {selectedQuizMode !== "Learn Mode" &&
                  answered &&
                  option.id === activeQuestion.answerId && (
                    <h4 className="caveat">correct answer</h4>
                  )} */}
                          {selectedQuizMode === "Learn Mode" &&
                            answer &&
                            correctClassName(option.id) &&
                            (!!answer.userOptionId || userOption) &&
                            !optionSelected && (
                              <h4 className="caveat">Correct answer</h4>
                            )}

                          {selectedQuizMode === "Learn Mode" &&
                            answer &&
                            wrongClassName(option.id) &&
                            (!!answer.userOptionId || userOption) &&
                            !optionSelected && (
                              <h4 className="caveat">Incorrect answer</h4>
                            )}
                        </div>
                      );
                    })}
                  </div>

                  {answer && !answer.showExplanation && (
                    <p
                      className="feedback blue--text"
                      onClick={() => openFeedback()}
                    >
                      <em>Feedback</em>
                    </p>
                  )}
                </div>
                {passage && (
                  <div
                    className="passage"
                    id="passage"
                    dangerouslySetInnerHTML={{
                      __html: setImageUrl(activeQuestion.passage),
                    }}
                  ></div>
                )}
                {feedback && (
                  <div className="passage formm">
                    <p className="top">Report a mistake in this question</p>
                    <span className="sub">
                      Give an applicable feedback on this Question
                    </span>

                    <form>
                      <span className="radio mt30">
                        <input type="radio" name="type" id="" />
                        The answer is wrong
                      </span>
                      {/* <span className="radio">
                <input type="radio" name="type" id="" />I caught a typo
              </span>
              <span className="radio">
                <input type="radio" name="type" id="" />
                The question or hit are confusing
              </span> */}
                      <span className="radio">
                        <input type="radio" name="type" id="" />
                        The image is not clear
                      </span>
                    </form>

                    <p className="sub">
                      Alternatively, you can report any technical problems you
                      may be experiencing.
                    </p>

                    <span className="sub">Description of issue:</span>
                    <textarea name="" id="" cols="30" rows="10"></textarea>
                    <button className="tw-btn">Submit issue</button>
                    <span onClick={openFeedback} className="cancel">
                      Cancel
                    </span>
                  </div>
                )}
              </>
            )}
          </div>

          {activeQuestion.isFirstSection && !readInstructions ? (
            <div className="footer">
              <b
                style={{
                  position: "relative",
                  top: 13,
                }}
              >
                {currentQuestion + 1} of {questions.length}
              </b>

              <button
                className="tw-btn"
                onClick={() => setreadInstructions(true)}
              >
                Continue
              </button>
              {currentQuestion > 0 && (
                <button className="blue-btn mr15" onClick={prevQuestion}>
                  Previous
                </button>
              )}
            </div>
          ) : selectedQuizMode === "Learn Mode" ? (
            <div className="footer">
              <b
                style={{
                  position: "relative",
                  top: 13,
                }}
              >
                {currentQuestion > 0 && (
                  <img
                    src={caret}
                    width="12"
                    alt=""
                    style={{
                      transform: "rotate(90deg)",
                      marginRight: "10px",
                      cursor: "pointer",
                    }}
                    onClick={prevQuestion}
                  />
                )}
                {currentQuestion + 1} of {questions.length}
                {currentQuestion < questions.length - 1 && (
                  <img
                    src={caret}
                    width="12"
                    alt=""
                    style={{
                      transform: "rotate(-90deg)",
                      marginLeft: "10px",
                      cursor: "pointer",
                    }}
                    onClick={nextQuestion}
                  />
                )}
              </b>

              {willSubmit && (
                <button
                  className="tw-btn"
                  onClick={() => submitLearingAnswer()}
                  style={{ marginLeft: 10 }}
                >
                  SUBMIT
                </button>
              )}

              {willTryAgain && (
                <button
                  className="tw-btn"
                  onClick={() => tryAgain()}
                  style={{ marginLeft: 10 }}
                >
                  {buttonText}
                </button>
              )}

              {willMoveNext && (
                <button
                  className="tw-btn"
                  onClick={() => nextQuestion()}
                  style={{ marginLeft: 10 }}
                >
                  NEXT QUESTION
                </button>
              )}

              {activeQuestion && activeQuestion.passage && (
                <button className="blue-btn" onClick={() => openPassage()}>
                  {passage ? "Close passage" : "Open passage"}
                </button>
              )}
            </div>
          ) : (
            <div className="footer">
              <b
                style={{
                  position: "relative",
                  top: 13,
                }}
              >
                {currentQuestion + 1} of {questions.length}
              </b>
              {currentQuestion === questions.length - 1 && (
                <button
                  className="tw-btn"
                  onClick={submit}
                  style={{ marginLeft: 10 }}
                >
                  SUBMIT
                </button>
              )}

              {currentQuestion < questions.length - 1 && (
                <button
                  className="blue-btn"
                  onClick={nextQuestion}
                  style={{ marginLeft: 10 }}
                >
                  NEXT
                </button>
              )}

              {currentQuestion > 0 && (
                <button
                  className="bare-btn"
                  onClick={prevQuestion}
                  style={{ marginLeft: 10 }}
                >
                  PREVIOUS
                </button>
              )}
              {activeQuestion && activeQuestion.passage && (
                <button className="blue-btn" onClick={() => openPassage()}>
                  {passage ? "Close passage" : "Open passage"}
                </button>
              )}
            </div>
          )}
        </div>
      )}
    </>
  );
}
