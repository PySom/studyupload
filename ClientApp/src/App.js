import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import api from './api/api';
import './App.css';

const App = ({ dummy }) => {
    const [file, setFile] = useState();
    const [loader, setLoader] = useState();

    const handleClick = (event) => {
        setLoader(() => true)
        event.preventDefault();
        let fileupload = new FormData()
        fileupload.append('file', file)
        api.create('files/docupload', fileupload)
            .then(() => {
                setLoader(() => false)
                alert("Successfully uploaded")

            }).catch(err => {
                alert(err)
                console.log(err)
            })
    }

    const handleMusicClick = (event) => {
        setLoader(() => true)
        event.preventDefault();
        let fileupload = new FormData()
        fileupload.append('file', file)
        api.create('files/musicupload', fileupload)
            .then(() => {
                setLoader(() => false)
                alert("Successfully uploaded")

            }).catch(err => {
                alert(err)
                console.log(err)
            })
    }

    return (
        <div>
            <form className="p-10">
                <div>
                    <label htmlFor="file_upload">Upload File</label>
                </div>
                <input type="file" id="file_upload" onChange={({ target }) => setFile(target.files[0])} />
                {loader && <p>Uploading...</p>}
                {!loader && <button type="submit" onClick={handleClick}>Upload</button>}
                <CourseList />
            </form>
            <form className="p-10">
                <div>
                    <label htmlFor="music_upload">Upload music files</label>
                </div>
                <input type="file" id="music_upload" onChange={({ target }) => setFile(target.files[0])} />
                {loader && <p>Uploading...</p>}
                {!loader && <button type="submit" onClick={handleMusicClick}>Upload music</button>}
            </form>
        </div>
        
    )
}

const CourseList = () => {
    const [courses, setCourses] = useState([])
    useEffect(() => {
        api.get("courses")
            .then(data => {
                setCourses(() => data)
            })
            .catch(err => console.log(err))
            
    }, [])

    return ( <ul>
            {courses.map(item =>
                (
                    <li key={item.id}>
                        <Course data={item} />
                    </li>
                ))
            }
        </ul>)
    }
        
const Course = ({ data }) => {
    const [testVisiblility, setTestVisibility] = useState(false)
    const [quizVisiblility, setQuizVisibility] = useState(false)
    const [quizId, setQuizId] = useState()

    const handleTestClick = (e) => {
        e.preventDefault()
        setTestVisibility(() => !testVisiblility)
    }

    const handleQuizClick = (e, id) => {
        e.preventDefault()
        setQuizVisibility(() => !quizVisiblility)
        setQuizId(() => id)
    }
    return (
        <div>
            <button type="button" onClick={handleTestClick}>{data.name}</button>
            {testVisiblility && 
                data.tests.map(test => (
                    <>
                        <div className="p-10">
                            <button type="button" onClick={(event) => handleQuizClick(event, test.id)}>{test.year}</button>
                        </div>
                        {quizVisiblility && test.id === quizId &&
                            <div>
                                <h4>Year: {test.year}</h4>
                                <p>Text: {test.text || "Nothing yet"}</p>
                                <p>Total questions: {test.questionNo}</p>
                                <p>Description: {test.shortDescription || "nothing yet"}</p>
                                <p>Duration: {test.duration}</p>
                                <p>Audio url: {test.audioUrl || "no upload yet"}</p>
                                <br />
                                {test.quizes.map((quiz, index) => (
                                    <div>
                                        <h4><b>Question {++index}:</b> {quiz.question}</h4>
                                        <p><b>Passage:</b> {quiz.passage || "No passage"}</p>
                                        <p>--------------options (green is correct)----------------</p>
                                        {quiz.options.map(option => (
                                            <p style={
                                                {
                                                    backgroundColor: option.id === quiz.answerId && "green",
                                                    color: option.id === quiz.answerId && "white",
                                                }
                                            }>{option.content}</p>
                                        ))}
                                        <p>--------------end of options----------------</p>
                                        <p><b>Answer description: </b> {quiz.answerUrl}</p>
                                        <br />
                                    </div>))}
                            </div>
                        }
                        
                    </>
                ))
            }
        </div>
        
        )
}
const ConnectedApp = connect(({ dummy }) => ({ dummy }))(App)
export default ConnectedApp;