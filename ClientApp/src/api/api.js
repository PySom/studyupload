import axios from 'axios';

const baseUrl = "/api/";
const all = (uri) =>
    axios
        .get(baseUrl + uri)
        .then(response => response.data)
        .catch(err => { throw new Error(err.response.data) })


const create = (uri, personObject) =>
    axios
        .post(baseUrl + uri, personObject)
        .then(response => response.data)
        .catch(err => { throw new Error(err.response.data) })

const get = (url) =>
            axios
                .get(baseUrl + url)
                .then(response => response.data)
        .catch(err => { throw new Error(err.response.data) })

const remove = (url) =>
    axios
        .delete(baseUrl + url)
        .then()
        .catch()


const update = (url, personObject) =>
    axios
        .put(baseUrl + url, personObject)
        .then(response => response.data)
        .catch(err => { throw new Error(err.response.data) })


export default
    {
        all,
        create,
        get,
        remove,
        update
    }

