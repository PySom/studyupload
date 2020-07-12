import React, { useState, useEffect, useContext } from "react";
import { userContext } from "../../store/UserContext";
import authServices from "../../services/authServices";
import Modal from "react-responsive-modal";
import banner1 from "../../assets/images/banner1.svg";
import banner2 from "../../assets/images/Subjects.svg";
import { useSnackbar } from "react-simple-snackbar";

export default function AddCourses(props) {
  const context = useContext(userContext);
  const {
    userCourses,
    subjects,
    user,
    updateUserCourses,
    updateUser,
    updateLoader,
  } = context;
  const { open, onCloseModal } = props;
  const [subjects_, setsubjects_] = useState([]);
  const [levelSubjects, setLevelSubjects] = useState([]);
  const [level, setlevel] = useState(null);
  const [newSelectedCourses, setnewSelectedCourses] = useState([]);
  const [toBeRemoved, settoBeRemoved] = useState([]);
  const [step, setStep] = useState(1);
  const [loader, setloader] = useState(false);
  const [junior, setjunior] = useState(false);
  const [senior, setsenior] = useState(false);
  const options = {
    position: "top-right",
  };
  const [openSnackbar, closeSnackbar] = useSnackbar(options);
  let modalWidth = window.innerWidth > 1024 ? 75 : 100;

  const preSelectedCourses = userCourses.reduce((agg, curr) => {
    agg[curr.courseId] = curr;
    return agg;
  }, {});

  useEffect(() => {
    setjunior(user.level === 3);
    setsenior(user.level === 4);
    console.log({ sub: subjects });
    let refinedSubjects = subjects.map((s) => {
      return {
        ...s,
        preSelected: preSelectedCourses[s.id] ? true : false,
      };
    });
    setsubjects_(refinedSubjects);
    console.log({ refinedSubjects });
    setlevel(user.level);
    let levelCourses = refinedSubjects.filter(
      (s) => s.studyLevel === user.level
    );
    setLevelSubjects(levelCourses);
    setStep(user.level ? 2 : 1);
  }, []);

  const pickLevel = (n) => {
    updateLevel(n);
    if (n === 3) {
      setjunior(true);
      setsenior(false);
    } else {
      setjunior(false);
      setsenior(true);
    }

    let levelCourses = subjects_.filter((s) => s.studyLevel === n);
    setLevelSubjects(levelCourses);
  };

  const jumpStep = () => {
    if (!level) return;
    step < 2 ? setStep(step + 1) : selectCourse();
  };

  const getCurrentUser = () => {
    authServices
      .getCurrentUser()
      .then((res) => {
        let user = res.data;
        updateUser(user);
      })
      .catch((err) => {
        console.log(err);
      });
  };

  const selectCourse = () => {
    if (newSelectedCourses.length || toBeRemoved.length) {
      setloader(true);
      updateLoader(true);
    } else {
      onCloseModal();
      getCurrentUser();
    }

    newSelectedCourses.forEach((id) => {
      authServices
        .updateUserCourses({
          userId: user.id,
          courseId: id,
        })
        .then((res) => {
          fetchUserCourses();
          setloader(false);
        })
        .catch((err) => {
          console.log({ err });
          openSnackbar(
            err.response.data ? err.response.data.message : "An error occured",
            5000
          );
          onCloseModal();
          setloader(false);
          updateLoader(false);
        });
    });

    toBeRemoved.forEach((id) => {
      console.log({toBeRemoved})
      authServices
        .deleteUserCourse(id, user.id)
        .then((res) => {
          fetchUserCourses();
          setloader(false);
        })
        .catch((err) => {
          console.log({ err });
          openSnackbar(
            err.response.data ? err.response.data.message : "An error occured",
            5000
          );
          getCurrentUser();
          onCloseModal();
          updateLoader(false);
          setloader(false);
        });
    });
  };

  const fetchUserCourses = () => {
    authServices
      .getUserCourses(null, user.id)
      .then((res) => {
        onCloseModal();
        getCurrentUser();
        openSnackbar("Courses updated sucessfully", 5000);
        let user_courses = res.data.userCourses;
        let sievedCourses = Object.values(
          user_courses.reduce((agg, curr) => {
            agg[curr.courseId] = curr;
            return agg;
          }, {})
        );
        updateLoader(false);
        updateUserCourses(sievedCourses);
      })
      .catch((err) => {
        updateLoader(false);
        console.log({ err });
      });
  };

  const updateLevel = (level) => {
    setlevel(level);
    let data = [
      {
        value: level === 3 ? "junior" : "senior",
        op: "add",
        path: "/level",
      },
    ];

    authServices
      .updateUserData(data, user.id)
      .then((res) => {
        console.log(res);
        getCurrentUser();
      })
      .catch((err) => {
        console.log({ err });
      });
  };

  const pickSubject = (i) => {
    let subject = levelSubjects[i];
    console.log(subject);
    if (!subject) return;
    if (subject.preSelected) {
      settoBeRemoved([...toBeRemoved, subject.id]);
      setnewSelectedCourses(
        [...newSelectedCourses].filter((n) => n !== subject.id)
      );
    } else {
      setnewSelectedCourses([...newSelectedCourses, subject.id]);
      settoBeRemoved([...toBeRemoved].filter((n) => n !== subject.id));
    }
    let newSubjectArr = levelSubjects.map((sub, j) => {
      return i === j
        ? {
            ...sub,
            preSelected: !sub.preSelected,
          }
        : { ...sub };
    });
    setLevelSubjects(newSubjectArr);
  };

  return (
    <div>
      <Modal
        open={open}
        onClose={onCloseModal}
        center
        styles={{ modal: { width: `${modalWidth}%` } }}
        closeOnOverlayClick={false}
      >
        <div
          className="banner"
          style={{
            backgroundImage: step === 1 ? `url(${banner1})` : `url(${banner2})`,
          }}
        >
          <p className="main-text">Let us help you personalize your learning</p>
          <p className="sub-text">
            {step === 1
              ? "What grade are you in?"
              : "What courses can we help you learn?"}
          </p>
        </div>
        {step === 1 ? (
          <div className="modal-data">
            <p className="blue--text">Secondary / Basic</p>

            <button className="class bg_1" onClick={() => pickLevel(3)}>
              <input
                type="checkbox"
                name=""
                id=""
                onChange={() => pickLevel(3)}
                checked={junior}
              />
              Junior Secondary
            </button>

            <button className="class bg_2" onClick={() => pickLevel(4)}>
              <input
                type="checkbox"
                name=""
                id=""
                onChange={() => pickLevel(4)}
                checked={senior}
              />
              Senior Secondary
            </button>

            <div className="coming">
              <em>coming soon</em>
              <p className="blue--text">Primary / Elementary</p>
              <p className="blue--text">University / Adult learner</p>
            </div>
          </div>
        ) : (
          <div className="modal-data">
            {!levelSubjects.length && (
              <p className="blue--text">
                There are no courses for your selected level
              </p>
            )}
            {levelSubjects.map((subject, i) => {
              return (
                <button
                  className={`subject bg_${i + 1}`}
                  onClick={() => pickSubject(i)}
                  key={`subject-${subject.name}`}
                >
                  <input
                    type="checkbox"
                    name=""
                    id=""
                    style={{ transform: "scale(1.02)" }}
                    checked={subject.preSelected}
                    onChange={() => pickSubject(i)}
                  />
                  {subject.name}
                </button>
              );
            })}
          </div>
        )}
        <div className="modal-footer">
          {step === 2 && (
            <em
              style={{ cursor: "pointer" }}
              className="f-left blue--text"
              onClick={() => setStep(1)}
            >
              Select learning grade
            </em>
          )}
          <button className="tw-btn" onClick={jumpStep} disabled={loader}>
            {step === 1 ? "NEXT" : "Start Learning"}
          </button>
        </div>
      </Modal>
    </div>
  );
}
