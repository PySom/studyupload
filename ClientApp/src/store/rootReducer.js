import { combineReducers } from 'redux';
import dummyReducer from "../reducers/dummyReducer";

const rootReducer = combineReducers({
    dummy: dummyReducer
})

export default rootReducer;