import React from 'react';
import "../js/global";
import ReactSummernote from 'react-summernote';
import 'react-summernote/dist/react-summernote.css';

// Import bootstrap(v3 or v4) dependencies
import "bootstrap/js/dist/dropdown";
import "bootstrap/js/dist/tooltip";
import "bootstrap/js/dist/modal";

import "bootstrap/dist/css/bootstrap.css";
import api from '../api/api';


class TextEditor extends React.Component {
    constructor(props) {
        super(props)
        this.state = { showCode: false }
        this.onChange = this.onChange.bind(this);
        this.onImageUpload = this.onImageUpload.bind(this);
    }

    onChange(content) {
        this.props.handleChange(content)
    }
    
    onImageUpload = (fileList) => {
        this.setState({ showCode: true });
        const file = fileList[0];
        let fileupload = new FormData()
        fileupload.append('file', file)
        console.log("about to push")
        api.create('files/upload', fileupload)
            .then( url => {
                console.log("image url", url)
                let htmlTag = ` <img src='${url.name}' class="img-responsive custom-img ${this.props.id}" alt="uploaded question helper visual" />`;
                this.props.handleChange(`${this.props.value} ${htmlTag}`)
                this.setState({ showCode: false })
                
            }).catch(err => {
                alert(err)
                console.log(err)
            })
    }

    render() {
        console.log("value is", this.state.value)
        return (
            <ReactSummernote
                value={this.props.value}
                codeView={this.state.showCode}
                options={{
                    height: 100,
                    dialogsInBody: false,
                    toolbar: [
                        ['style', ['style']],
                        ['font', ['bold', 'underline', 'clear']],
                        ['fontname', ['fontname']],
                        ['para', ['ul', 'ol', 'paragraph']],
                        ['table', ['table']],
                        ['insert', ['link', 'picture', 'video']],
                        ['view', ['fullscreen', 'codeview']]
                    ]
                }}
                onChange={this.onChange}
                onImageUpload={this.onImageUpload}
            />
        );
    }
}

export default TextEditor;
