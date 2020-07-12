import React, {useState} from 'react';
import useTest from './usetest';

export default function useLearn(previousData, quizId){
    const {
        answerIsCorrect,
        userScore,
        answer,
        answers,
        setThisAnswer,
        canTryAgain,
        optionIsCorrect,
        setAnswers,
        updateAnswers,
        userAnswersCount,
        showExplanation,
        lockAllAnswers,
        willTryAgain,
        willShowAlert,
        willSubmit,
        willMoveNext,
        lockState,
        correctClassName,
        userAnswersToAdd,
        userAnswersToUpdate,
        wrongClassName,
        lockThisAnswer,
        ...rest} = useTest(previousData, quizId)

        const updateThisAnswer = (extras) => {
            const thisanswer = {...answer, ...extras, attempts: answer.attempts + 1};
            rest.updateThisAnswer(thisanswer);
        }

        const onTryAgain = (options) => {
            const thisanswer = {...answer, ...options};
            rest.updateThisAnswer(thisanswer);
        }

        const setShowAlert = () => {
            const newAnswer = {...answer, alert: true}
            console.log({newAnswer})
            rest.updateThisAnswer(newAnswer)
        }

        const hideShowAlert = () => {
            const newAnswer = {...answer, alert: false}
            console.log({newAnswer})
            rest.updateThisAnswer(newAnswer)
        }

    return {
        answer,
        updateAnswers,
        setThisAnswer,
        userScore,
        setAnswers,
        userAnswersCount,
        showExplanation,
        optionIsCorrect,
        answerIsCorrect,
        lockAllAnswers,
        willTryAgain,
        willSubmit,
        setShowAlert,
        lockState,
        hideShowAlert,
        willShowAlert,
        userAnswersToAdd,
        userAnswersToUpdate,
        willMoveNext,
        lockThisAnswer,
        canTryAgain,
        onTryAgain,
        correctClassName,
        wrongClassName,
        updateThisAnswer,
        userAnswers: answers,
    }
}

