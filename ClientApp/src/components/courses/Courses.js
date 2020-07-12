import React, { useContext, useEffect } from "react";
import { userContext } from "../../store/UserContext";

import "./Courses.scss";
import neww from "../../assets/images/New.svg";
import authServices from "../../services/authServices";
import { withRouter } from "react-router-dom";

export default withRouter(function Courses(props) {
  const context = useContext(userContext);
  const {
    userCourses,
    updateLoader,
    selectedSubject,
    saveSelectedSubject,
    user,
  } = context;
  let sortesubjects = userCourses
    .map((subject, i) => {
      return {
        ...subject,
        width:
          subject.course.name.length <= 7
            ? 1
            : subject.course.name.length > 7 && subject.course.name.length <= 12
            ? 2
            : 3,
        new: false,
      };
    })
    .filter((c) =>
      user.level === 4
        ? c.course.studyLevel === 0 || c.course.studyLevel === user.level
        : c.course.studyLevel === user.level
    )
    .sort((a, b) => {
      if (a.width < b.width) return -1;
      if (a.width > b.width) return 1;
      return 0;
    });

  const selectCourse = (course) => {
    updateLoader(true);
    authServices
      .getSubjectQuiz(course.courseId)
      .then((res) => {
        saveSelectedSubject(res.data);
        updateLoader(false);
        props.moveToCourse &&
          props.history.push(`/subject/${selectedSubject.id}`);
      })
      .catch((err) => {
        console.log({ err });
        updateLoader(false);
      });
  };

  useEffect(() => {
    sortesubjects.length &&
      !selectedSubject.name &&
      selectCourse(sortesubjects[0]);
  }, []);

  return (
    <div className="courses-wrap">
      <div className="courses">
        <div className="flex-grid-thirds">
          {sortesubjects.map((course, i) => {
            return (
              <div
                key={course.name + i}
                onClick={() => selectCourse(course)}
                style={{ textTransform: "uppercase" }}
                className={`bg_${(i % 17) + 2} course pLR`}
              >
                {course.course.name}{" "}
                {course.new && <img src={neww} className="new" alt="" />}
              </div>
            );
          })}
        </div>
      </div>
      <button onClick={props.onOpenModal} className="mt25 tw-btn">
        <b>Add more courses +</b>
      </button>
    </div>
  );
});
