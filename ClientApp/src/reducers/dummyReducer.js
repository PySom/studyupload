const dummyReducer = (state = ["Hello World", "Good boy"], action) => {
    switch (action.type) {
        case "HELLO":
            return state.concat(action.data);
        default:
            return state;
    }
}

export default dummyReducer;