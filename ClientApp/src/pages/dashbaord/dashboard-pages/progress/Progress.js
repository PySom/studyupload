import React, { useState, useContext, useEffect } from "react";
import Select from "react-select";
import { motion } from "framer-motion";

import "./Progress.scss";
import Statistics from "../../../../components/statistics/Statistics";

import play from "../../../../assets/images/play.svg";
import Badges from "../../../../components/badges/Badges";
import Links from "../../../../components/sidebar/Links";
import Days from "../../../../components/days/Days";
import { userContext } from "../../../../store/UserContext";
import authServices from "../../../../services/authServices";
import Loader from "../../../../components/loader/Loader";

export default function Progress() {
  const context = useContext(userContext);
  const { userCourses, user, loading, updateLoader } = context;
  console.log(userCourses)
  
  const courses_ = userCourses.filter(c => c.course.studyLevel === user.level).map((c) => {
    return {
      ...c,
      label: c.name,
      value: c.name,
    };
  });
  const options = [
    { value: "", label: "Activities" },
    { value: "Video", label: "Video" },
    { value: "Quiz", label: "Quiz" },
  ];
  const days = [
    { value: "", label: "Last Seven Days", rep: ""},
    { value: "Monday", label: "Monday", rep: 1 },
    { value: "Tuesday", label: "Tuesday", rep: 2 },
    { value: "Wednesday", label: "Wednesday", rep: 3 },
    { value: "Thursday", label: "Thursday", rep: 4 },
    { value: "Friday", label: "Friday", rep: 5 },
    { value: "Saturday", label: "Saturday", rep: 6 },
    { value: "Sunday", label: "Sunday", rep: 0 },
  ];
  const [selectedOption, setselectedOption] = useState("");
  const [selectedOption_, setselectedOption_] = useState("");
  const [statistics, setStatistics] = useState([]);
  const [currentStat, setCurrentStat] = useState();
  const [filteredStat, setFilteredStat] = useState();
  const [pageLoaded, setPageLoaded] = useState(false);
  const [course, setCourse] = useState();

  const onCourseChange = (value) => {
    if(value){
      setCourse(value);
      const thisCourse = userCourses.find(uc => uc.id === Number(value));
      console.log(thisCourse, value, userCourses)
      if(thisCourse){
        const statAvailable = statistics.find(s => s.name === thisCourse.course.name);
        console.log({statAvailable})
        if(statAvailable){
          setCurrentStat(statAvailable);
          setFilteredStat(statAvailable)
        }
        else{
          getStatistics(thisCourse)
        }
      }
    }
    
  }

  useEffect(() => {
    if(!pageLoaded){
      if(userCourses && userCourses.length){
        const first = userCourses[0];
        getStatistics(first)
        setCourse(first.id)
      }
      setPageLoaded(true);
    }
  }, [pageLoaded])

  const getStatistics = (userCourse) => {
    updateLoader(true);
    authServices
      .getStatistics(userCourse.id)
      .then((res) => {
        console.log({ stats: res.data });
        let newStat = [];
        if(statistics.some(c => c.name == userCourse.course.name)){
          newStat = statistics.map(s => s.name === userCourse.course.name ? {name: userCourse.course.name, ...res.data} : s);
        }
        else{
          newStat = statistics.concat({name: userCourse.course.name, ...res.data});
        }
        setStatistics(newStat);
        setCurrentStat({name: userCourse.course.name, ...res.data})
        setFilteredStat({name: userCourse.course.name, ...res.data})
        updateLoader(false);
      })
      .catch((err) => {
        console.log({ err });
        updateLoader(false);
      });
  };

  const handleChange = (selectedOption) => {
    console.log({selectedOption})
    setselectedOption(selectedOption);
  };
  const handleChange_ = (selectedOption) => {
    console.log({selectedOption})
    setselectedOption_(selectedOption);
  };

  const handleSearch = (e) => {
    if(selectedOption || selectedOption_){
      const selectedType = selectedOption && selectedOption.value;
      const selectedDay = selectedOption_ && selectedOption_.rep;
      console.log(selectedOption, selectedOption_, selectedDay, selectedType)
      const params = {day: selectedDay, type: selectedType};
      setFilteredStat(filtered(params))


    }

  }

  const filtered = (params) => {
    console.log({params})
    if(!params) return currentStat;
    else{
      if(currentStat){
        console.log({params})
        if((params.day || params.day === 0) && params.type){
          const d = currentStat.activities.filter(activity => activity.dayOfWeek === params.day && activity.testType === params.type)
          return {...currentStat, activities: d}
        }
        else if(params.day || params.day === 0){
          const d = currentStat.activities.filter(activity => activity.dayOfWeek === params.day)
          return {...currentStat, activities: d}
        
        }
        else if(params.type){
          console.log("type", currentStat.activities)
          const d = currentStat.activities.filter(activity => activity.testType === params.type)
          return {...currentStat, activities: d}
        }
        else return currentStat
      }
    }
  }
  return (
    <>
    {loading && <Loader />}
    <motion.div
      className="progress"
      initial={{ opacity: 0, x: "-5vw" }}
      animate={{ opacity: 1, x: "0vw" }}
      exit={{ opacity: 0, x: 0 }}
      transition={{ duration: 0.6 }}
    >
      <div className="wide-side">
        <p className="heading">Learning Statistics</p>
        <div className="stats">
          <p className="sub-heading">
            Latest activity may take 10 mins to show below.
          </p>
          <div className="filters">
            <div className="selects">
              <Select
                placeholder="Last 7 days"
                value={selectedOption_}
                onChange={handleChange_}
                options={days}
              />
            </div>
            <div className="selects">
              <Select
                placeholder="Activities"
                value={selectedOption}
                onChange={handleChange}
                options={options}
                styles={{ width: 120, marginTop: "-10px" }}
              />
            </div>
            <button className="tw-btn" type="button" onClick={handleSearch}>Search</button>
          </div>

          <div className="fixed-table mt40">
            <div className="tr thead">
              <div className="th activity">Activity</div>
              <div className="th">Date</div>
              <div className="th">Level</div>
              <div className="th points">Points aquired</div>
              <div className="th">Taken/Module</div>
            </div>
            <div className="tbody">
              {
                filteredStat && filteredStat.activities && filteredStat.activities.length 
                ? filteredStat.activities.map((activity, index) => (
                  <div className="tr" key={index}>
                    <div className="td activity">{currentStat.name} {activity.testType}</div>
                    <div className="td">{activity.date}</div>
                    <div className="td">Level {activity.level + 1}</div>
                    <div className="td points">{activity.testType === "Video" ? "N/A" : activity.point}</div>
                    <div className="td">{activity.overall}</div>
                  </div>
                ))
                : <span>No statistics yet</span>
              }
              
              
            </div>
            
          </div>
        </div>
      </div>
      <div className="narrow-side">
        <div className="bg_white">
          <p className="heading">
            Overall Statistics
            <span className="f-right">
              <select
                onChange={({target: {value}}) => onCourseChange(value)}
                value={course}
                style={{
                  width: "100px",
                  height: "30px",
                  border: "1px solid #eee",
                  marginTop: "-10px",
                }}
              >
                {courses_.map((course) => (
                  <option style={{textTransform: "uppercase"}} key={course.id} value={course.id}>
                    {course.course.name}
                  </option>
                ))}
              </select>
            </span>
          </p>
          <Statistics 
            first={currentStat && currentStat.percentVideo}
            second={currentStat && currentStat.percentQuiz}
            third={currentStat && currentStat.percentMastery} />
        </div>
        <div className="bg_white mt30">
          <p className="heading">Days learnt</p>
          <Days days={filteredStat && filteredStat.activities && filteredStat.activities.length && filteredStat.activities.map(a => a.dayOfWeek)} />
        </div>
      </div>
    </motion.div>
    </>
    
  );
}
