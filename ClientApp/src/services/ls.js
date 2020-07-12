export const saveItem = (name, data) => {
    if(localStorage){
        localStorage.setItem(name, JSON.stringify(data));
    }
}

export const retrieveItem = (name) => {
    if(localStorage){
        const data = localStorage.getItem(name);
        if(name){
            return JSON.parse(data)
        }
    }
}