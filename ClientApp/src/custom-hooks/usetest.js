import React, { useState, useEffect } from "react";

const answerBuilder = (quizId, userOptionId, correctOptionId, mode, attempts = 0, showExplanation = false, id = 0, dateTaken) => ({
  quizId,
  userOptionId,
  correctOptionId,
  mode,
  attempts: attempts,
  showExplanation: showExplanation,
  alert: false,
  id,
  dateTaken
});

export default function useTest(previousData, quizId) {
  const [answers, setAnswers] = useState([]);
  const [answer, setAnswer] = useState();
  const [answersToUpdate, setanswersToUpdate] = useState([]);
  const [answersToAdd, setanswersToAdd] = useState([]);
  const [componentLoaded, setcomponentLoaded] = useState(false);

  useEffect(() => {
    // console.log({quizId})
    setThisAnswer(quizId);
  }, [quizId]);

  useEffect(() => {
    // console.log(previousData)
    if(previousData && previousData.length){
      const prepareUserData =
      (previousData && previousData.length) &&
      previousData.reduce((aggregate, current) => {
        const state = current.userOption === current.correctOption;
        const build = answerBuilder(
          current.quizId,
          current.userOption,
          current.correctOption,
          current.mode,
          2,
          true,
          current.id,
          current.dateTaken
        );
        aggregate = aggregate.concat(build);
        return aggregate;
      }, []);
    setcomponentLoaded(true);
    console.log({prepareUserData, previousData})
    setAnswers(prepareUserData);
    setAnswer(prepareUserData[0])
    }
    
  }, [componentLoaded]);

  const upsertAnswer = (currentAnswer) => {
    let updatedAnswers = null;
    if (answers.some((ans) => ans.quizId === currentAnswer.quizId)) {
      updatedAnswers = answers.map((ans) =>
        ans.quizId === currentAnswer.quizId ? currentAnswer : ans
      );
    } else {
      updatedAnswers = answers.concat(currentAnswer);
    }
    return updatedAnswers;
  };

  const updateAnswers = () => {
    if(!answer.userOptionId) return;
    const currentAnswer = { ...answer, attempts: 2 };
    console.log("user", {previousData, currentAnswer})
    if (previousData.some((ans) => ans.quizId === currentAnswer.quizId && ans.mode === currentAnswer.mode)) {
      console.log("user to be updated")
      if(answersToUpdate.some((ans) => ans.quizId === currentAnswer.quizId)){
        setanswersToUpdate(
          answersToUpdate.map((ans) =>
            ans.quizId === currentAnswer.quizId ? currentAnswer : ans
          )
        );
      }
      else{
        setanswersToUpdate(answersToUpdate.concat(currentAnswer));
      }
      
    } else {
      if(answersToAdd.some((ans) => ans.quizId === currentAnswer.quizId)){
        setanswersToAdd(
          answersToAdd.map((ans) =>
            ans.quizId === currentAnswer.quizId ? currentAnswer : ans
          )
        );
      }
      else{
        setanswersToAdd(answersToAdd.concat(currentAnswer));
      }
    }
    setAnswers(upsertAnswer(currentAnswer));
  };

  const userAnswersToAdd = answersToAdd.filter(
    (answer) => answer.userOptionId !== 0
  );
  const userAnswersToUpdate = answersToUpdate.filter(
    (answer) => answer.userOptionId !== 0
  );

  const userScore = answers.reduce((aggregate, current) => {
    // console.log({ current, aggregate });
    if (
      current.userOptionId === 0 ||
      current.userOptionId !== current.correctOptionId
    )
      return aggregate;
    return aggregate + 1;
  }, 0);

  const setThisAnswer = (quizId) => {
    let thisanswer = answers.find((answer) => answer.quizId === quizId);
    if (thisanswer) {
      setAnswer(thisanswer);
      return;
    }
    thisanswer = answerBuilder(quizId, 0, 0);
    // console.log({thisanswer})
    setAnswer(thisanswer);
  };

  const updateThisAnswer = (extras) => {
    const thisanswer = { ...answer, ...extras };
    setAnswer(thisanswer);
  };

  const answerIsCorrect =
    answer &&
    answer.userOptionId === answer.correctOptionId &&
    answer.userOptionId !== 0;
  const optionIsCorrect = (optionId) => answer.correctOptionId === optionId;
  const lockState = answer && (answer.attempts === 2 || answer.showExplanation);

  const showExplanation = () => {
    const update = { ...answer, showExplanation: true, attempts: 2 };
    setAnswer(update);
  };

  const correctClassName = (optionId) => {
    return (
      (answer.attempts === 1 &&
        answer.userOptionId === optionId &&
        answerIsCorrect) ||
      (answer.showExplanation && answer.correctOptionId === optionId) ||
      (answer.attempts === 2 && answer.correctOptionId === optionId)
    );
  };

  const wrongClassName = (optionId) => {
    return (
      answer.attempts >= 1 &&
      answer.userOptionId === optionId &&
      !answerIsCorrect
    );
  };

  const lockAllAnswers = () => {
    const newAnswers = answers.map((answer) => ({ ...answer, attempts: 2 }));
    setAnswers(newAnswers);
  };

  const lockThisAnswer = () => {
    const thisanswer = { ...answer, attempts: 2 };
    setAnswer(thisanswer);
  };

  const canTryAgain = answer && !answer.showExplanation && !answerIsCorrect;

  const willTryAgain = answer && answer.attempts === 1 && canTryAgain;

  const willSubmit = answer && answer.attempts === 0;

  const willMoveNext =
    answer &&
    (answer.attempts === 2 || answer.showExplanation || answerIsCorrect);

  const willShowAlert = answer && answer.attempts <= 2;

  const userAnswersCount = answer && answers.length;

  return {
    updateAnswers,
    userScore,
    updateThisAnswer,
    userAnswersToAdd,
    userAnswersToUpdate,
    answer,
    setThisAnswer,
    optionIsCorrect,
    correctClassName,
    wrongClassName,
    answers,
    userAnswers: answers,
    willTryAgain,
    willSubmit,
    willMoveNext,
    willShowAlert,
    setAnswers,
    answerIsCorrect,
    lockState,
    userAnswersCount,
    showExplanation,
    lockAllAnswers,
    lockThisAnswer,
  };
}
