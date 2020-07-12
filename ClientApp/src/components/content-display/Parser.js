import React from "react";
import MathParser from "./MathParser";
import HtmlParser from "./HtmlParser";
import { appUrl } from "../../services/urls";

const setImageUrl = (str) => {
    return str
      .replace(`<img src='assets`, `<img src='${appUrl}/assets`)
      .replace(`<img src="assets`, `<img src="${appUrl}/assets`)
      .replace("assets\\", "assets/");
  };

const Parser = ({className, question, isMathJax, id, inline}) => {
    const props = {className, id}
    return (
        <React.Fragment>
            { isMathJax 
                ? <MathParser {...props} inline={inline} question={question ? setImageUrl(question) : ""} />
                : <HtmlParser {...props} inline={inline} question={question ? setImageUrl(question) : ""} />      
            }
        </React.Fragment>
    )
}

export default Parser