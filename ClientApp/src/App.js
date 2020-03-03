import React from 'react';
import { connect } from 'react-redux';
import { api } from './api/api';

const App = ({ dummy }) => {
    const handleClick = (event) => {
        event.preventDefault();
        let fileupload = new FormData()
        fileupload.append('file', this.state.fileObj)
        //console.log(fileupload);
        api.create('files/upload', fileupload)
            .then(res => {
                //console.log(res)
                this.setState({ fileResp: res.name, imageUploadView: 'd-block' })
            }).catch(err => console.log(err))
    }
    return (
        <form>
            <label for="file_upload">Upload File</label>
            <input type="file" id="file_upload" />
            <button type="submit" onClick={handleClick}>Upload</button>
        </form>
    )
}


const ConnectedApp = connect(({ dummy }) => ({ dummy }))(App)
export default ConnectedApp;