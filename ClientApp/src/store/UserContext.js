import React, { createContext, Component } from "react";
import axios from "axios";
export const userContext = createContext();

export default class UserContextProvider extends Component {
  state = {
    loading: false,
    isLoggedIn: false,
    user: {
      email: "",
      firstName: "",
      id: null,
      image: null,
      phoneNumber: "",
      role: 0,
      sex: 0,
      surName: "",
      level: 2,
    },
    selectedSubject: {},
    subjects: [],
    userCourses: [],
    awards: [],
    userAwards: [],
    token: "",
    studyPacks: [],
    quizzes: [],
  };

  componentWillMount() {
    let state = JSON.parse(localStorage.getItem("parentValueKey"));
    this.setState(state);
    let token = this.state.user.token || "";
    axios.defaults.headers.common["Authorization"] = `${token}`;
    setTimeout(() => {
      this.setState({ loading: false });
    }, 1000);
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.state !== prevState) {
      // Whatever storage mechanism you end up deciding to use.
      localStorage.setItem("parentValueKey", JSON.stringify(this.state));
    } else {
      localStorage.setItem("parentValueKey", JSON.stringify(prevState));
    }
  }

  saveSelectedSubject = (selectedSubject) => this.setState({ selectedSubject });

  updateLoader = (loading) => {
    this.setState({ loading });
  };

  updateLoggedInStatus = (isLoggedIn) => {
    this.setState({ isLoggedIn });
  };

  updateStudyPack = (studyPacks) => this.setState({ studyPacks });
  updateStudyPackQuizes = (quizzes) => this.setState({ quizzes });

  updateUser = (user) => {
    this.setState({ user });
  };

  updateUserCourses = (userCourses) => {
    let subjectObj = this.state.subjects.reduce((pay, subj) => {
      pay[subj.id] = subj;
      return pay;
    }, {});
    let fullCourses = userCourses.map((course) => {
      return {
        ...course,
        name: subjectObj[course.learnCourseId]
          ? subjectObj[course.learnCourseId].name
          : "",
      };
    });
    console.log({ subjectObj, fullCourses });
    this.setState({
      userCourses: fullCourses,
    });
  };

  updateAwards = (awards) => this.setState({ awards });

  updateUserAwards = (userAwards) => this.setState({ userAwards });

  updateSubjects = (subjects) => {
    this.setState({ subjects });
  };

  logout = () => {
    this.setState({
      loading: false,
      isLoggedIn: false,
      user: {
        email: "",
        firstName: "",
        id: null,
        image: null,
        phoneNumber: "",
        role: 0,
        sex: 0,
        surName: "",
        level: null,
      },
      selectedSubject: {},
      subjects: [],
      userCourses: [],
      awards: [],
      userAwards: [],
      token: "",
      studyPacks: [],
      quizzes: [],
    });
    setTimeout(() => {
      localStorage.clear();
      sessionStorage.clear();
    }, 1000);
  };

  render() {
    const {
      updateUser,
      updateLoader,
      updateLoggedInStatus,
      logout,
      updateSubjects,
      saveSelectedSubject,
      updateUserCourses,
      updateAwards,
      updateUserAwards,
      updateStudyPack,
      updateStudyPackQuizes,
    } = this;
    return (
      <userContext.Provider
        value={{
          ...this.state,
          updateLoader,
          updateUser,
          updateLoggedInStatus,
          logout,
          updateSubjects,
          updateUserCourses,
          saveSelectedSubject,
          updateAwards,
          updateUserAwards,
          updateStudyPack,
          updateStudyPackQuizes,
        }}
      >
        {this.props.children}
      </userContext.Provider>
    );
  }
}
