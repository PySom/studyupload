import React from 'react';
import useTest from './usetest';
import useLearn from './uselearn';

export default function useStudy(previousData, quizId, mode){
    const test = useTest(previousData, quizId)
    const learn = useLearn(previousData, quizId)
    if(mode === "Time Mode" || mode === "Free Form Mode"){
        return test;
    }
    return learn;
}

