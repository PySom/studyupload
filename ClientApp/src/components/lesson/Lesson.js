import React, { useState, useContext, useEffect, createRef } from "react";
import "./Lesson.scss";

import play from "../../assets/images/play.svg";
import playIcn from "../../assets/images/play-button.svg";
import close from "../../assets/images/close.svg";
import Modal from "react-responsive-modal";
import { withRouter } from "react-router-dom";

import icn from "../../assets/images/dashboard.svg";
import clock from "../../assets/images/clock.svg";
import listener from "../../assets/images/Nice-listener.svg";
import streak from "../../assets/images/streak.svg";
import { appUrl, videoUrl, fakeVideoUrl } from "../../services/urls";
import { userContext } from "../../store/UserContext";
import authServices from "../../services/authServices";

export default withRouter(function Lesson({
  disableClick,
  history,
  video,
  grade,
  ...props
}) {
  console.log({abc: props.usertests, def: props.uservideos})
  const vidRef = createRef()
  const context = useContext(userContext);
  const { selectedSubject } = context;
  const videoLevel =
    grade === 0 ? "Beginner" : grade === 1 ? "Intermediate" : "Advanced";
  let modalWidth = window.innerWidth > 1024 ? 96 : 100;



  useEffect(() => {
    
    let data = [];
    let subjectVideos = selectedSubject.tests.reduce((agg, curr) => {
      return agg.concat(curr.videos);
    }, []);
    subjectVideos
      .filter((v) => v.level === grade)
      .forEach((vid, i) => {
        if (vid.id === video.id) {
          setactiveVideo(i);
        }
        data.push({
          ...vid,
          text: vid.name,
          selected: vid.id === video.id,
        });
      });
    setVideos(data);
  }, []);
  const [open, setopen] = useState(false);
  const [showStreak, setshowStreak] = useState(false);
  const [url, seturl] = useState("");
  const [videos, setVideos] = useState([]);
  const [userVideo, setUserVideo] = useState([])
  const [render, setRender] = useState(false)
  const [initVideo, setInitVideo] = useState(false)
  const [activeVideo, setactiveVideo] = useState(null);

  const oncontextmenu = (e) => e.preventDefault();
  const duration = videos.reduce((agg, vid) => {
    return agg + vid.duration;
  }, 0);

  const timeFrame = `${Math.floor(duration / 60)}hrs ${duration % 60}mins`;

  const sendUserVideoToStore = (i) => {
    //add a user's test and then add the courses
    //check if user answered
    console.log("quiz commenced adding...");
    console.log("testid", props.usertests)
      let userTestId = 0;
      if (props.usertests && props.usertests.length) {
        userTestId = props.usertests[0].id;
      }
      if (userTestId) {
        const currentVid = videos[i]
        const vid = vidRef.current;
        const vidData = {
          duration: vid && vid.currentTime,
          videoId: currentVid.id,
          userTestId
        }
        console.log({one: vidData})
        sendUserVideoToServer(vidData);
      } else {
        const data = {
          userCourseId: props.usercourseid,
          testId: video.testId,
        };
        console.log({data})
        sendUserTestToServer(data, i);
      }
  };
  const sendUserTestToServer = (data, i) => {
    authServices
      .addUserTest(data)
      .then((res) => {
        const currentVid = videos[i]
        const vid = vidRef.current;
        const vidData = {
          duration: vid && vid.currentTime,
          videoId: currentVid.id,
          userTestId: res.data.id,
        }
        console.log({vidData, current: currentVid})
        sendUserVideoToServer(vidData);
      })
      .catch((err) => {
        console.log({ err });
      });
  };

  const sendUserVideoToServer = (data) => {
    authServices
      .addUserVideo(data)
      .then((res) => {
        console.log({ uservideo: res.data });
        setUserVideo(res.data)
      })
      .catch((err) => {
        console.log({ err });
      });
  };
  const selectVideo = (i) => {
    sendUserVideoToStore(i);
    if (i >= videos.length) return;
    setactiveVideo(i);
    seturl(videos[i].url);
    setVideos(
      videos.map((v, j) => {
        return i === j
          ? {
              ...v,
              selected: true,
            }
          : {
              ...v,
              selected: false,
            };
      })
    );
    const url = `${videoUrl}${videos[i].url}`;
    prepVideo(url, videos[i]);

    
  };

  const prepVideo = (url, video) => {
    if(url){
      url = url.replace('assets', 'video');
      const videoPlayer = vidRef.current;
      videoPlayer.oncontextmenu = () => false;
      console.log({url})
      videoPlayer.src = url.replace('assets', 'video');
      console.log("oncanplay", {videoPlayer})
      if(videoPlayer && video && (props.uservideos || userVideo)){
        const useVideos = userVideo && userVideo.length ? userVideo : props.uservideos
        const previousVideo = useVideos.find(uv => uv.videoId === video.id);
        console.log("oncanplay", {video, userVideo, useVideos, previousVideo})
        if(previousVideo){
          videoPlayer.currentTime = previousVideo.duration;
        }
      }
      //videoPlayer.play();
    }
  }

  const openModal = (url) => {
    if (disableClick) {
      history.push("/subject/mathematics");
    } else {
      setopen(!open);
      let turl = url;
      if(video && (props.uservideos || userVideo)){
        const useVideos = userVideo && userVideo.length ? userVideo : props.uservideos
        const previousVideo = useVideos.find(uv => uv.videoId === video.id);
        if(previousVideo){
          turl += "#t="+ previousVideo.duration;
          console.log({turl})
        }
        setUserVideo(props.uservideos);
      }
      seturl(turl);
    }
  };

  return (
    <div className="lesson-wrap">
      <div className="lesson" onClick={() => openModal(video.url)}>
        <p>
          <img src={play} width="20" className="mr15 mt5" alt="" />
          <span style={{ fontSize: 12 }}>{video.name}</span>
        </p>
      </div>

      <Modal
        open={open}
        onClose={() => setopen(!open)}
        center
        showCloseIcon={false}
        styles={{ modal: { width: `${modalWidth}%` } }}
        closeOnOverlayClick={false}
      >
        <div className="video-modal">
          {showStreak && (
            <div className="streak">
              <img src={streak} alt="" />
              <div className="streak-text">
                <p className="top">Great Listener</p>
                <p style={{ fontSize: 12 }}>
                  You have watched 30 minutes of video in a single topic
                </p>
              </div>
            </div>
          )}
          <div className="videos">
            <div
              style={{ fontWeight: 600, fontSize: 20, padding: "30px 40px" }}
            >
              {videoLevel}
            </div>
            <div
              style={{ padding: "0 40px 30px", borderBottom: "1px solid #eee" }}
            >
              <div
                className="tag"
                style={{
                  display: "inline-block",
                  width: "50%",
                  color: "#717171",
                  fontSize: 14,
                }}
              >
                <img
                  src={playIcn}
                  width="15"
                  alt=""
                  style={{ marginRight: 4, position: "relative", top: 3 }}
                />{" "}
                {videos.length} videos
              </div>
              {/* <div
                className="tag"
                style={{
                  display: "inline-block",
                  width: "33%",
                  color: "#717171",
                  fontSize: 12,
                }}
              >
                <img src={icn} width="12" alt="" /> 18 Topics
              </div> */}
              <div
                className="tag"
                style={{
                  display: "inline-block",
                  width: "50%",
                  color: "#717171",
                  fontSize: 12,
                }}
              >
                <img
                  src={clock}
                  width="15"
                  alt=""
                  style={{ marginRight: 4, position: "relative", top: 3 }}
                />{" "}
                {timeFrame}
              </div>
            </div>

            <ul
              style={{
                padding: 0,
                overflow: "auto",
                height: "calc(90vh - 140px)",
                margin: 0,
              }}
            >
              {videos.map((video, i) => {
                return (
                  <li
                    onClick={() => selectVideo(i)}
                    key={video.text}
                    style={{
                      padding: "15px 40px",
                      cursor: "pointer",
                      fontSize: 14,
                      listStyle: "none",
                      color: video.selected ? "#fff" : "#000",
                      backgroundColor: video.selected ? "#10A3FF" : "#fff",
                      whiteSpace: "nowrap",
                      overflow: "hidden",
                      textOverflow: "ellipsis",
                    }}
                  >
                    <img
                      src={play}
                      style={{
                        width: 22,
                        marginRight: 12,
                        position: "relative",
                        top: "3px",
                      }}
                      alt=""
                    />
                    <span
                      style={{
                        position: "relative",
                        top: "-5px",
                        fontSize: 12,
                      }}
                      title={video.text}
                    >
                      {video.text}
                    </span>
                  </li>
                );
              })}
            </ul>
          </div>
          <div className="frame">
            <video
              style={{ width: "100%", height: "calc(90vh - 90px)" }}
              controls
              ref={vidRef}
              onContextMenu={oncontextmenu}
              autoPlay={true}
              src={`${videoUrl}${url ? url.replace("assets", "video"): url}`}
              controlsList="nodownload"
              id="homevideo"
            >
              {/* <source
                src={`${videoUrl}${url ? url.replace("assets", "video"): url}`}
                type="video/mp4"
                frameBorder="0"
                allow="autoplay; encrypted-media"
                allowFullScreen
                title="video"
              ></source> */}
            </video>
            <div
              className="info"
              style={{ padding: 20, width: "100%", height: 80 }}
            >
              <span
                className="blue--text mt10 video-name"
                style={{ fontWeight: 600, position: "relative", top: "5px" }}
              >
                {videos[activeVideo] && videos[activeVideo].text}
              </span>
              <button
                className="f-right tw-btn"
                style={{ padding: "12px 30px" }}
                onClick={() => selectVideo(activeVideo + 1)}
                disabled={activeVideo >= videos.length - 1}
              >
                Next video
              </button>
            </div>
            <span className="close">
              <img
                src={close}
                alt=""
                onClick={() => setopen(false)}
                style={{
                  width: "20px",
                  float: "right",
                  marginTop: "15px",
                  marginRight: 20,
                  cursor: "pointer",
                  position: "absolute",
                  top: 12,
                  right: 2,
                }}
              />
            </span>
          </div>
        </div>
      </Modal>
    </div>
  );
});
