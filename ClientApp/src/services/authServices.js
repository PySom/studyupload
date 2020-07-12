import axios from "axios";
import * as urls from "./urls";

let headers = () => {
  let userToken = localStorage.getItem("studymate-token");
  return { Authorization: `Bearer ${userToken}` } || {};
};

let signup = (data) => {
  return axios({
    method: "post",
    url: urls.registerUrl,
    data,
  });
};

let login = (data) => {
  return axios({
    method: "post",
    url: urls.authUrl,
    data,
  });
};

let generateCode = (data) => {
  return axios({
    method: "post",
    url: urls.getCodeUrl + "?email=" + data.email + "&validate=true",
    headers: headers(),
  });
};

let verifyEmail = (data) => {
  return axios({
    method: "put",
    url: urls.verifyEmailUrl + "?code=" + data.code,
    data,
  });
};

let googleLogin = (data) => {
  return axios({
    method: "post",
    url: urls.googleAuthUrl,
    data,
  });
};

let getCurrentUser = (token) => {
  return axios({
    method: "get",
    url: urls.currentUserUrl,
    headers: token ? { Authorization: `Bearer ${token}` } : headers(),
  });
};

let getUserCourses = (token, id) => {
  return axios({
    method: "get",
    // url: urls.userCourseUrl,
    url: urls.getUserLearnCourseUrl,
    params: {
      id
    },
    headers: token ? { Authorization: `Bearer ${token}` } : headers(),
  });
};

let getCourseByName = (name) => {
  return axios({
    method: "get",
    url: urls.courseUrl + "/byname",
    params: {
      name,
    },
  });
};

let updateUserCourses = (data) => {
  return axios({
    method: "post",
    url: urls.userLearnCourseUrl,
    data,
  });
};

let deleteUserCourse = (cid,uid) => {
  return axios({
    method: "delete",
    url: urls.userLearnCourseUrl + "/course/" + cid + "/user/" + uid,
  });
};

let getAllCourses = (token) => {
  return axios({
    method: "get",
    url: urls.learnCourseUrl,
    headers: token ? { Authorization: `Bearer ${token}` } : headers(),
  });
};

let updateUserData = (data, id) => {
  return axios({
    method: "patch",
    url: urls.userUrl + "/" + id,
    data,
    headers: headers(),
  });
};

let getSubjectQuiz = (id) => {
  return axios({
    method: "get",
    url: urls.courseUrl + "/" + id,
  });
};

let getSubjectVideos = (id) => {
  return axios({
    method: "get",
    url: urls.videosUrl + "/" + id,
  });
};

let getAwards = () => {
  return axios({
    method: "get",
    url: urls.awardsUrl,
    headers: headers(),
  });
};

let getUserAward = (id) => {
  return axios({
    method: "get",
    url: urls.userAwardsUrl,
    headers: headers(),
    params: {
      id,
    },
  });
};

let getUcourseWithTests = (uid, courseId) => {
  return axios({
    method: "get",
    url: `${urls.userCourseWithTestsUrl}/${courseId}/user/${uid}`,
    headers: headers(),
  });
};

let getAwardboard = (userID, quizID) => {
  return axios({
    method: "get",
    url: urls.userAwardsUrl + "/leaders/" + userID + "?id=" + quizID,
    headers: headers(),
  });
};

let addUserTest = (data) => {
  return axios({
    method: "post",
    url: urls.userTestUrl,
    data,
  });
};

let addMultipleUserQuizzes = (data) => {
  return axios({
    method: "post",
    url: urls.addMultipleUserQuizesUrl,
    data,
  });
};

let addUserVideo = (data) => {
  return axios({
    method: "post",
    url: urls.userVideosUrl,
    data,
  });
};

let getLeaderboard = (userID) => {
  return axios({
    method: "get",
    url: urls.leadersUrl + userID,
    headers: headers(),
  });
};

let getStatistics = (usercourseid) => {
  return axios({
    method: "get",
    url: urls.statisticsUrl + "/" + usercourseid,
    headers: headers(),
  });
};

let getStudypackData = (quizID) => {
  return axios({
    method: "get",
    url: `${urls.testWithQuizzes}${quizID}`,
    headers: headers(),
  });
};

let getQuizCount = (courseId) => {
  return axios({
    method: "get",
    url: `${urls.quizCountUrl}${courseId}`,
    headers: headers(),
  });
};


let updateUserTestData = (data, id) => {
  return axios({
    method: "patch",
    url: urls.userTestUrl + "/" + id,
    data,
    headers: headers(),
  });
};

let updateMultipleUserQuiz = (data) => {
  return axios({
    method: "put",
    url: urls.updateMultipleUserQuizesUrl,
    data,
  });
};

let uploadFile = (data) => {
  return axios({
    method: "post",
    url: urls.filesUrl,
    data,
  });
};

export default {
  signup,
  login,
  generateCode,
  verifyEmail,
  googleLogin,
  updateUserData,
  getCurrentUser,
  getUserCourses,
  getCourseByName,
  updateUserCourses,
  deleteUserCourse,
  getAllCourses,
  getAwards,
  getSubjectQuiz,
  getSubjectVideos,
  getUserAward,
  getLeaderboard,
  getStudypackData,
  uploadFile,
  addUserTest,
  getUcourseWithTests,
  addMultipleUserQuizzes,
  addUserVideo,
  getStatistics,
  updateUserTestData,
  updateMultipleUserQuiz,
  getQuizCount
};
