import React, { useContext, useState } from "react";
import "./faq.scss";
import { motion } from "framer-motion";
import Nav from "../../components/nav/Nav";
import Loader from "../../components/loader/Loader";
import backArrow from "../../assets/images/back.svg";
import plus from "../../assets/images/more.svg";
import { userContext } from "../../store/UserContext";
import { withRouter } from "react-router-dom";

export default withRouter(function FAQ({ history }) {
  const context = useContext(userContext);
  const { user, loading } = context;

  const [faqs, setfaqs] = useState([
    {
      title: "Q. What are Badges?",
      text: `
      Incentives used to encourage students for mastery and practice, completion of missions, as well as for participating positively in the StudyMate community`,
      open: false,
    },
    {
      title: "Q. What do I expect to see in my profile?",
      text: `
      This is where a student goes to take pride in viewing all of their progress on StudyMate! Provides a dashboard of progress, points, badges, leaderboard and more. Students also find their active subscription here`,
      open: false,
    },
    {
      title: "Q. What is a Streak? ",
      text: `
      It is a reward for persistence in studying, using our platform!  StudyMate rewards students with points for a streak of questions answered correctly, and continuous days of work.`,
      open: false,
    },
    {
      title: "Q. How many learning Videos does StudyMate have?",
      text: `
      StudyMate has 1,000+ instructional videos, including over 100 math videos tightly aligned with the NERDC curriculum, covering early math to senior secondary.`,
      open: false,
    },
    {
      title: "Q. What are Study Packs?",
      text: `
      StudyMate designed its practice questions according to packs. Each pack is designed with over 50 unique questions`,
      open: false,
    },
    {
      title: "Q. What happens in the Test section?",
      text: `
      StudyMate allows users to test their skills with over 10,000 practice questions in the test section. These practice questions can help you develop on your learning outcome and show you your progress`,
      open: false,
    },
    {
      title: "Q. Are there levels for different learners on StudyMate?",
      text: `
      Yes.
        There is the beginner, intermediate and advanced level, each level with different video packs and questions following.`,
      open: false,
    },
  ]);

  const back = () => {
    history.push("/edit-profile/");
  };

  const openFaq = (i) => {
    setfaqs(
      faqs.map((f, j) => {
        return i === j
          ? {
              ...f,
              open: !f.open,
            }
          : {
              ...f,
            };
      })
    );
  };

  return (
    <motion.div
      className="faq"
      initial={{ opacity: 0, x: "-5vw" }}
      animate={{ opacity: 1, x: "0vw" }}
      exit={{ opacity: 0, x: 0 }}
      transition={{ duration: 0.6 }}
    >
      {loading && <Loader />}
      <Nav />
      <div className="edit-contents">
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
            Support
          </span>
          <span onClick={back} className="mobile-title-text">
            HOME
          </span>
        </div>
        <div className="contents">
          <p className="title">How can we help you?</p>
          <input type="text" />
          <textarea name="" id="" cols="30" rows="10"></textarea>
          <button className="tw-btn submit">SUBMIT</button>
          <br />
          <div className="questions">
            <p className="title blue--text">
              <b>Frequently asked questions(FAQ's)</b>
            </p>
            <p className="sub">
              Frequently askedquestions (FAQ) or Questions and Answers (Q & A),
              are listed questions and answers , all supposed to be asked in
              some context.
            </p>

            {faqs.map((faq, i) => (
              <div className="accordion" key={faq.text}>
                <div className="top" onClick={() => openFaq(i)}>
                  <p
                    style={{
                      display: "inline-block",
                      width: "90%",
                      lineHeight: "20px",
                    }}
                  >
                    {faq.title}
                  </p>
                  <img
                    src={plus}
                    width="20"
                    style={{
                      float: "right",
                      position: "relative",
                      right: "15px",
                      top: "25px",
                      display: "inline-block",
                    }}
                    alt=""
                  />
                </div>
                <div className={`btm ${faq.open ? "open" : ""}`}>
                  {faq.text}
                </div>
              </div>
            ))}
          </div>
        </div>
        <div className="band">
          2020 All right reserved. StudyMate
          <p style={{ margin: "4px 0" }}>Powered by infomall</p>
        </div>
      </div>
    </motion.div>
  );
});
