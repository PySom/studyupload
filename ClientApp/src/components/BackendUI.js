import React, { useState, useEffect } from 'react';
import { Layout } from './Layout';
import './BackendUI.css';
import MathJax from 'react-mathjax-preview'
import parse from "html-react-parser";
import api from '../api/api';
import { useLocation } from 'react-router-dom';
import TextEditor from './TextEditor';




String.prototype.Insert = function (word_before, word_to_add) {
    if (word_to_add) {
        const indexOfWordBefore = this.indexOf(word_before);
        const wordsBefore = this.substring(0, indexOfWordBefore);
        const wordsAfter = word_to_add + " " + this.substring(indexOfWordBefore);
        return wordsBefore + wordsAfter;
    }
    return this;
}

String.prototype.Remove = function (word_to_remove) {
    if (word_to_remove) {
        const indexOfWordBefore = this.indexOf(word_to_remove);
        if (indexOfWordBefore > 0) {
            const wordsBefore = this.substring(0, indexOfWordBefore);
            const wordsAfter = this.substring(indexOfWordBefore + word_to_remove.length);
            return wordsBefore + wordsAfter;
        }
    }
    return this;
}

const BackendUI = () => {
    
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
                alert(err.message)
                console.log(err.message)
                setLoader(() => false)
            })
    }

    const handleUpdateAlpha = (event) => {
        setLoader(() => true)
        event.preventDefault();
        api.update('courses/updatealpha/30')
            .then(() => {
                setLoader(() => false)
                alert("Successfully updated")

            }).catch(err => {
                alert(err.message)
                console.log(err.message)
                setLoader(() => false)
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
                alert(err.message)
                console.log(err.message)
                setLoader(() => false)
            })
    }

    const handleMusicUpdate = (event) => {
        setLoader(() => true)
        event.preventDefault();
        api.create('courses/updateaudio')
            .then(() => {
                setLoader(() => false)
                alert("Successfully updated")

            }).catch(err => {
                alert(err.message)
                console.log(err.message)
                setLoader(() => false)
            })
    }

    const handleQuestionNumber = (event) => {
        setLoader(() => true)
        event.preventDefault();
        api.create('courses/updatequestionnumber')
            .then(() => {
                setLoader(() => false)
                alert("Successfully updated")

            }).catch(err => {
                alert(err.message)
                console.log(err.message)
                setLoader(() => false)
            })
    }

    const handleModClick = (event) => {
        setLoader(() => true)
        event.preventDefault();
        let fileupload = new FormData()
        fileupload.append('file', file)
        api.create('files/editpassage', fileupload)
            .then(() => {
                setLoader(() => false)
                alert("Successfully uploaded")

            }).catch(err => {
                alert(err.message)
                console.log(err.message)
                setLoader(() => false)
            })
    }

    return (
        <Layout>
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
                    <label htmlFor="mod_upload">Modify passage and/ or section</label>
                </div>
                <input type="file" id="mod_upload" onChange={({ target }) => setFile(target.files[0])} />
                {loader && <p>Uploading...</p>}
                {!loader && <button type="submit" onClick={handleModClick}>Upload modification file</button>}
            </form>

            <form className="p-10">
                <div>
                    <label htmlFor="mod_upload">Upload music file (.txt)</label>
                </div>
                <input type="file" id="mod_upload" onChange={({ target }) => setFile(target.files[0])} />
                {loader && <p>Uploading...</p>}
                {!loader && <button type="submit" onClick={handleMusicClick}>Upload music file</button>}
            </form>
            <form className="p-10">
                <p className="text-danger">Please be aware this is an expensive process, use coordinatively</p>
                {loader && <p>Uploading...</p>}
                {!loader && <button type="submit" onClick={handleMusicUpdate}>Update audio</button>}
                <p>Update options with A., B. C. D. for Economics</p>
                {loader && <p>Uploading...</p>}
                {!loader && <button type="submit" onClick={handleUpdateAlpha}>Update Alphabet</button>}
            </form>
            <div className="p-10">
                {!loader && <button type="button" onClick={handleQuestionNumber}>Update Question Number</button>}
            </div>
        </Layout>
        
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

    const handleCourseDelete = (e, id) => {
        e.preventDefault()
        if (window.confirm("Confirm delete!")) {
            api.remove(`courses/${id}`)
                .then(_ => {
                    console.log("deleted")
                    alert("deleted")
                    setCourses(() => courses.filter(course => course.id !== id))

                })
                .catch(err => console.log(err))
        }
        

    }

    return ( <ul>
            {courses.map(item =>
                (
                    <li key={item.id}>
                        <Course data={item} handleCourseDelete={handleCourseDelete} />
                    </li>
                ))
            }
        </ul>)
}

const Course = ({ data, ...props }) => {
    const [testVisiblility, setTestVisibility] = useState(false)
    const [quizVisiblility, setQuizVisibility] = useState(false)
    const [quizId, setQuizId] = useState()
    const [tests, setTests] = useState([]);
    const [quizes, setQuizes] = useState([]);
    const [answer, setAnswer] = useState();
    const [question, setQuestion] = useState();
    const [passage, setPassage] = useState();
    const [includeQuiz, setIncludeQuiz] = useState();
    const [section, setSection] = useState();
    const [options, setOptions] = useState([]);
    const [showList, setShowList] = useState("");
    const [classNameQuestion, setClassNameQuestion] = useState("");
    const [removeClassNameQuestion, setRemoveClassNameQuestion] = useState("");
    const [id, setId] = useState()
    const [optionToAdd, setOptionToAdd] = useState("");
    const [optionIsCorrect, setOptionIsCorrect] = useState(false);


    const [newQuestion, setNewQuestion] = useState("");
    const [newSection, setNewSection] = useState("");
    const [newPassage, setNewPassage] = useState("");
    const [newAnswer, setNewAnswer] = useState("");
    const [newAudio, setNewAudio] = useState("");


    const [questionNumber, setQuestionNumber] = useState(0);



    useEffect(() => {
        const idl = quizes.length > 0 ? quizes[0].id : 0;
        setId(() => idl)
    }, [quizes])

    useEffect(() => {
        const quiz = quizes.find(q => q.id === id)
        if (quiz) {
            setAnswer(() => quiz.answerUrl);
            setQuestion(() => quiz.question);
            setPassage(() => quiz.passage);
            setSection(() => quiz.section);
            setIncludeQuiz(() => quiz.includeThis);
            if (quiz.options && quiz.options.length > 0) {
                let build = []
                quiz.options.forEach(option => {
                    build = build.concat(option.content)
                })
                setOptions(build);
            }
        }
        
    }, [id, quizes])

    const handleTestClick = (e, id) => {
        e.preventDefault()
        api.get(`tests/${id}?parent=true`)
            .then(data => {
                setTests(() => data)
                setTestVisibility(() => !testVisiblility)
            })
            .catch(err => console.log(err))

    }

    const handleChange = (content, idx) => {
        setOptions(options.map((option, index) => index === idx ? content : option))
    }

    const handleAnswerChange = (content) => {
        setAnswer(() => content)
        console.log("from app", content);
    }

    const handleQuestionChange = (content) => {
        setQuestion(() => content)
        console.log("from app", content);
    }

    const handlePassageChange = (content) => {
        setPassage(() => content)
        console.log("from app", content);
    }

    const handleSectionChange = (content) => {
        setSection(() => content)
        console.log("from app", content);
    }

    const handleQuizClick = (e, id) => {
        e.preventDefault()
        if (quizId === id) {
            setQuizVisibility(() => !quizVisiblility)
            return
        }
        setQuizId(() => id)
        api.get(`quizes/${id}?child=true&study=studymate`)
            .then(data => {
                console.log("loaded")
                setQuizes(() => data)
                setQuizVisibility(() => true)

            })
            .catch(err => console.log(err))

    }

    const handleCourseDelete = (e, id) => {
        props.handleCourseDelete(e, id);
    }

    const handleTestDelete = (e, id) => {
        if (window.confirm("Confirm delete!")) {
            e.preventDefault()
            api.remove(`tests/${id}`)
                .then(_ => {
                    console.log("deleted")
                    alert("deleted")
                    setTests(() => tests.filter(test => test.id !== id))
                })
                .catch(err => console.log(err))
        }
        

    }

    const addOption = (qid) => {
        console.log(qid, optionToAdd)
        if (optionToAdd) {
            api.create(`options?iscorrect=${optionIsCorrect}`, { practiceId: qid, content: optionToAdd, isMathJax: optionToAdd.includes('\\(') })
                .then(_ => {
                    console.log("added")
                    alert("added. Please refesh if you want to see this")
                })
                .catch(err => console.log(err.message))
        }
    }

    const updateQuestionNumber = (e, testId, id) => {
        e.preventDefault();
        if (questionNumber) {
            api.update(`quizes/updatenumber`, { id, testId, questionNumber })
                .then(quiz => {
                    console.log("updated")
                    alert("added successfully")
                })
                .catch(err => console.log(err.message))
        }
    }

    const addNewQuestion = (id) => {
        const qToAdd = {
            question: newQuestion,
            section: newSection,
            passage: newPassage,
            isSectioned: !!newSection,
            hasPassage: !!newPassage,
            answerUrl: newAnswer,
            questionNumber: quizes.length + 1,
            isQuestionMathJax: newQuestion.includes('\\('),
            testId: id
        }
        api.create(`quizes`, qToAdd)
            .then(quiz => {
                console.log("added")
                alert("added successfully")
                setAnswer(() => quiz.answerUrl);
                setQuestion(() => quiz.question);
                setPassage(() => quiz.passage);
                setSection(() => quiz.section);
                if (quiz.options && quiz.options.length > 0) {
                    let build = []
                    quiz.options.forEach(option => {
                        build = build.concat(option.content)
                    })
                    setOptions(build);
                }
                else {
                    setOptions([]);
                }
            })
            .catch(err => console.log(err.message))
    }

    const removeOption = (oid) => {
        if (window.confirm("Confirm delete!")) {
            api.remove(`options/${oid}`)
                .then(_ => {
                    console.log("removed")
                    alert("removed. Please refesh if you want to see this")
                })
                .catch(err => console.log(err.message))
        }
    }
    const previousQuiz = (e, id) => {
        const sq = quizes.findIndex(q => q.id === id);
        console.log(sq)
        if (sq !== 0) setId(() => quizes[sq - 1].id )


    }

    const nextQuiz = (e, id) => {
        const sq = quizes.findIndex(q => q.id === id);
        if (sq !== quizes.length - 1) setId(() => quizes[sq + 1].id)
    }

    const pagination = (id) => {
        const sq = quizes.findIndex(q => q.id === id);
        setId(() => quizes[sq].id)
    }

    const editQuiz = (e, quizData) => {
        if (quizData) {
            delete quizData.options
            quizData = { ...quizData, answerUrl: answer, isQuestionMathJax: question.includes("\\("), question, passage, section, includeThis: includeQuiz }
            api.update(`quizes/${quizData.id}`, quizData)
                .then(data => {
                    alert("updated")
                })
                .catch(err => console.err(err))
        }
    }

    const editOption = (e, idx, optionData) => {
        if (optionData) {
            optionData = { ...optionData, isMathJax: options[idx].includes("\\("), content: options[idx] }
            api.update("options", optionData)
                .then(data => {
                    alert("updated")
                })
                .catch(err => console.err(err))
        }
    }

    const addQuestionNumber = (no) => {
        try {
            const data = Number(no);
            setQuestionNumber(data);
        }
        catch{
            console.log("STOP IT!!!");
        }
    }



    return (
        <div>
            <button type="button" onClick={(event) => handleTestClick(event, data.id)}>{data.name}</button>
            <button type="button" onClick={(event) => handleCourseDelete(event, data.id)}>Delete</button>
            {testVisiblility &&
                tests.map(test => (
                    <React.Fragment key={test.id}>
                        <div className="p-10">
                            <button type="button" onClick={(event) => handleQuizClick(event, test.id)}>{test.year}</button>
                            <button type="button" onClick={(event) => handleTestDelete(event, test.id)}>Delete</button>
                        </div>
                        {quizVisiblility && test.id === quizId && <>
                            <button type="button" onClick={() => setShowList(() => "list")}>List View</button>
                            <button type="button" onClick={() => setShowList(() => "single")}>Single View</button>
                        </>}
                        
                        {showList === "single"
                            ? 
                            quizVisiblility && test.id === quizId &&
                            <div>
                                <h4>Year: {test.year}</h4>
                                <p>Text: {test.text || "Nothing yet"}</p>
                                <p>Total questions: {test.questionNo}</p>
                                <p>Description: {test.shortDescription || "nothing yet"}</p>
                                <p>Duration: {test.duration}</p>
                                <br />
                                {quizes.map((quiz, index) => (
                                    id === quiz.id &&
                                    <div key={quiz.id}>
                                        <p>Question {quiz.questionNumber}</p>
                                        <p>Audio url: {quiz.audioUrl || "no upload yet"}
                                            <audio src={"https://content-qc.studymate.ng/" + quiz.audioUrl} controls></audio>
                                        </p>
                                        <p>
                                            <input type="text" value={questionNumber} onChange={({ target: { value } }) => addQuestionNumber(value)} />
                                            <button type="button" onClick={(e) => updateQuestionNumber(e, test.id, quiz.id)}>Change this question number</button>
                                        </p>
                                        <p>Quiz Id: {quiz.id}</p>
                                        <div><b>Question {++index}:</b>
                                            <ParserCondition content={question} isMathJax={quiz.isQuestionMathJax} />
                                        </div>
                                        <TextEditor value={question}
                                            id={`${data.name.toLowerCase().replace(' ', '-')}-q-${test.year}-${quiz.id}`}
                                            handleChange={(content) => handleQuestionChange(content)} />

                                        <div>
                                            <input value={classNameQuestion} onChange={({ target: { value } }) => setClassNameQuestion(value)} />
                                            <button type="button" onClick={() => handleQuestionChange(question.Insert("img-responsive", classNameQuestion))}>Add Class</button>
                                        </div>
                                        <div>
                                            <input value={removeClassNameQuestion} onChange={({ target: { value } }) => setRemoveClassNameQuestion(value)} />
                                            <button type="button" onClick={() => handleQuestionChange(question.Remove(removeClassNameQuestion))}>Remove Class</button>
                                        </div>
                                        <p><b>Passage:</b> {passage || "No passage"}</p>
                                        <TextEditor value={passage} id={`passage-o-${test.year}-${quiz.id}`} handleChange={(content) => handlePassageChange(content)} />

                                        <p><b>Section:</b> {section || "No section"}</p>
                                        <TextEditor value={section} id={`passage-o-${test.year}-${quiz.id}`} handleChange={(content) => handleSectionChange(content)} />


                                        <p><b>IsFirstPassge:</b> {quiz.isFirstPassage.toString()}</p>
                                        <p><b>IsFirstSection:</b> {quiz.isFirstSection.toString()}</p>
                                        <p>--------------options (green is correct)----------------</p>
                                        {quiz.options.map((option, index) => (
                                            <div key={option.id} style={
                                                {
                                                    backgroundColor: option.id === quiz.answerId && "green",
                                                    color: option.id === quiz.answerId && "white",
                                                }
                                            }>
                                                <ParserCondition content={options[index]} isMathJax={option.isMathJax} />
                                                {console.log("current option",options[index])}
                                                <TextEditor value={options[index]} id={`${data.name.toLowerCase().replace(' ', '-')}-o-${test.year}-${quiz.id}-${option.id}`} handleChange={(content) => handleChange(content, index)} />

                                                <button className="primary btn-primary"
                                                    onClick={(e) => editOption(e, index, { ...option })}
                                                    type="button">Edit Option</button>
                                                <button className="primary btn-primary"
                                                    onClick={(e) => removeOption(option.id)}
                                                    type="button">Remove Option</button>
                                            </div>
                                        ))}
                                        <br />
                                        <p>{quiz.includeThis ? "Included" : "Not included"}</p>
                                        <label>
                                            <input type="checkbox" value={includeQuiz} checked={includeQuiz ? "checked" : ""} onChange={() => setIncludeQuiz(!includeQuiz)} />
                                            Include this quiz
                                        </label>
                                        <br/>

                                        <p>--------------end of options----------------</p>
                                        <p className="text-success">-------------are you adding a new option?-----------</p>
                                        <TextEditor value={optionToAdd} id={`${data.name.toLowerCase().replace(' ', '-')}-o-${test.year}-${quiz.id}-custom`} handleChange={(content) => setOptionToAdd(content)} />

                                        <input type="checkbox" value={optionIsCorrect} onChange={({ target: { checked } }) => setOptionIsCorrect(checked)} /> <span>Is this the correct option? </span>
                                        <button className="primary btn-primary"
                                            onClick={(e) => addOption(quiz.id)}
                                            type="button">Add this option</button>

                                        <p>-----------------------Add a new question---------------------</p>
                                        <p className="text-success">-------------are you adding a new question?-----------</p>
                                        <p>Question: </p>
                                        <p><ParserCondition content={newQuestion} isMathJax={newQuestion.includes('\\(')} /></p>
                                        <TextEditor value={newQuestion} id={`${data.name.toLowerCase().replace(' ', '-')}-o-${test.year}-${quiz.id}-custom`} handleChange={(content) => setNewQuestion(content)} />

                                        <p>Passage: </p>
                                        <p><ParserCondition content={newPassage} isMathJax={newPassage.includes('\\(')} /></p>
                                        <TextEditor value={newPassage} id={`${data.name.toLowerCase().replace(' ', '-')}-o-${test.year}-${quiz.id}-custom`} handleChange={(content) => setNewPassage(content)} />

                                        <p>Section: </p>
                                        <p><ParserCondition content={newSection} isMathJax={newSection.includes('\\(')} /></p>
                                        <TextEditor value={newSection} id={`${data.name.toLowerCase().replace(' ', '-')}-o-${test.year}-${quiz.id}-custom`} handleChange={(content) => setNewSection(content)} />

                                        <p>Answer to this question: </p>
                                        <p><ParserCondition content={newAnswer} isMathJax={newAnswer.includes('\\(')} /></p>
                                        <TextEditor value={newAnswer} id={`${data.name.toLowerCase().replace(' ', '-')}-o-${test.year}-${quiz.id}-custom`} handleChange={(content) => setNewAnswer(content)} />

                                        <p>Audio URL: </p>
                                        <p>{newAudio}</p>
                                        <input type="text" value={newAudio} onChange={({ target: value }) => setNewAudio(value)} />
                                        
                                        <button className="primary btn-primary"
                                            onClick={(e) => addNewQuestion(quiz.testId)}
                                            type="button">Add this question</button>
                                        <p>-------------------------------End of adding new question----------------------------------------------</p>
                                        <p><b>Answer description: </b></p>
                                        <ParserCondition content={answer} isMathJax={quiz.answerUrl ? quiz.answerUrl.includes("\\(") : false} />
                                        <TextEditor value={answer} id={`${data.name.toLowerCase().replace(' ', '-')}-a-${test.year}-${quiz.id}`} handleChange={(content) => handleAnswerChange(content)} />

                                        <br />
                                        <button type="button" onClick={(e) => previousQuiz(e, quiz.id)}>Previous</button>
                                        <button type="button" onClick={(e) => nextQuiz(e, quiz.id)}>Next</button>
                                        <br />
                                        <button className="primary btn-primary"
                                            onClick={(e) => editQuiz(e, { ...quiz })}
                                            type="button">Edit Quiz</button>
                                    </div>
                                ))}
                                {quizes.map((quiz, index) => (<button key={quiz.id} type="button" onClick={() => pagination(quiz.id)}>{quiz.questionNumber}</button>))}
                            </div>
                            :
                            showList === "list" &&
                            quizVisiblility && test.id === quizId &&
                            <div>
                                <h4>Year: {test.year}</h4>
                                <p>Text: {test.text || "Nothing yet"}</p>
                                <p>Total questions: {test.questionNo}</p>
                                <p>Description: {test.shortDescription || "nothing yet"}</p>
                                <p>Duration: {test.duration}</p>
                                <br />
                                {quizes.map((quiz, index) => (
                                    <div key={quiz.id}>
                                        <p>Audio url: {quiz.audioUrl || "no upload yet"}</p>
                                        <p>Quiz Id: {quiz.id}</p>
                                        <div><b>Question {quiz.questionNumber}:</b>
                                            <ParserCondition content={quiz.question} isMathJax={quiz.isQuestionMathJax} />
                                        </div>
                                        <p><b>Passage:</b> {quiz.passage || "No passage"}</p>
                                        <p><b>Section:</b> {quiz.section || "No section"}</p>
                                        <p><b>IsFirstPassge:</b> {quiz.isFirstPassage.toString()}</p>
                                        <p><b>IsFirstSection:</b> {quiz.isFirstSection.toString()}</p>
                                        <p>--------------options (green is correct)----------------</p>
                                        {quiz.options.map((option, index) => (
                                            <div key={option.id} style={
                                                {
                                                    backgroundColor: option.id === quiz.answerId && "green",
                                                    color: option.id === quiz.answerId && "white",
                                                }
                                            }>
                                                <ParserCondition content={option.content} isMathJax={option.isMathJax} />
                                            </div>
                                        ))}

                                        <p>--------------end of options----------------</p>
                                        <p><b>Answer description: </b></p>
                                        <ParserCondition content={quiz.answerUrl} isMathJax={quiz.answerUrl ? quiz.answerUrl.includes("\\(") : false} />

                                    </div>
                                ))}
                            </div>
                        }
                        

                    </React.Fragment>
                ))
            }
        </div>

    )
}
const HtmlParse = ({ question }) => {
    return (
        <p>{parse(question)}</p>
    )
}

const MathJaxParse = ({ question }) => {
    console.log(question)
    return (
        <MathJax math={question} />
    )
}

const ParserCondition = (props) => {
    console.log(props)
    return (
        <React.Fragment>
            {props.isMathJax ?
                <div><MathJaxParse question={props.content || ""} /></div>
                : <div><HtmlParse question={props.content || ""} /></div>
            }
        </React.Fragment>
    )
}

export default BackendUI;