import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import api from './api/api';

const App = ({ dummy }) => {
    const [file, setFile] = useState();
    const [loader, setLoader] = useState();

    const handleClick = (event) => {
        setFile(() => setLoader(() => true))
        event.preventDefault();
        let fileupload = new FormData()
        fileupload.append('file', file)
        //console.log(fileupload);
        api.create('files/docupload', fileupload)
            .then(() => {
                //console.log(res)
                setLoader(() => false)
                alert("Successfully uploaded")

            }).catch(err => console.log(err))
    }
    return (
        <form>
            <div>
                <label for="file_upload">Upload File</label>
            </div>
            <input type="file" id="file_upload" onChange={({ target }) => setFile(target.files[0])} />
            {loader && <p>Uploading...</p>}
            {!loader && <button type="submit" onClick={handleClick}>Upload</button>}
            <CourseList />
        </form>
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
    const [visiblility, setVisibility] = useState(false)
    const handleClick = (e) => {
        e.preventDefault()
        setVisibility(() => !visiblility)
    }
    return (
        <div>
            <button type="button" onClick={handleClick}>{data.name}</button>
            {visiblility && 
                data.tests.map(test => (
                    <div>
                        <h4>Year: {test.year}</h4>
                        <p>Text: {test.text || "Nothing yet"}</p>
                        <p>Total questions: {test.questionNo}</p>
                        <p>Description: {test.shortDescription || "nothing yet"}</p>
                        <p>Duration: {test.duration}</p>
                        <br />
                        {test.quizes.map((quiz, index) => (
                            <div>
                                <h4><b>Question {++index}:</b> {quiz.question}</h4>
                                <p><b>Passage:</b> {quiz.passage || "No passage"}</p>
                                <p>--------------options (green is correct)----------------</p>
                                {quiz.options.map(option => (
                                    <p style={{ backgroundColor: option.id === quiz.answerId && "green", color: option.id === quiz.answerId && "white", }}>{option.content}</p>
                                ))}
                                <p>--------------end of options----------------</p>
                                <p><b>Answer description: </b> {quiz.answerUrl}</p>
                                <p>Total questions: {test.questionNo}</p>
                                <p>Description: {test.shortDescription || "nothing yet"}</p>
                                <p>Duration: {test.duration}</p>
                                <br />
                                {test.quizes.map}
                            </div>))}
                    </div>
                ))
            }
        </div>
        
        )
}
const ConnectedApp = connect(({ dummy }) => ({ dummy }))(App)
export default ConnectedApp;