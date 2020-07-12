import React, { useContext, useEffect, useState } from "react";

import userIcon from "../../assets/images/user.svg";
import editIcon from "../../assets/images/edit.svg";
import authServices from "../../services/authServices";
import { userContext } from "../../store/UserContext";
import Loader from "../../components/loader/Loader";
import { appUrl } from "../../services/urls";
import { useSnackbar } from "react-simple-snackbar";

export default function UserPhoto() {
  const context = useContext(userContext);
  const { updateLoader, loading, user, updateUser } = context;

  const [userPic, setuserPic] = useState("");
  const options = {
    position: "top-right",
  };
  const [openSnackbar, closeSnackbar] = useSnackbar(options);

  useEffect(() => {
    updateLoader(false);
    setuserPic(user.image ? user.image.replace("\\", "/") : "");
  }, []);

  const getCurrentUser = () => {
    authServices
      .getCurrentUser()
      .then((res) => {
        let user = res.data;
        updateUser(user);
      })
      .catch((err) => {
        console.log(err);
      });
  };

  const toProfile = (e) => {
    updateLoader(true);
    const selectedFile = e.target.files[0];
    var f = new FormData();
    f.append("file", selectedFile);
    authServices
      .uploadFile(f)
      .then((res) => {
        let url = res.data.name;
        updateUserPhoto(url);
        openSnackbar("Profile photo updated sucessfully", 5000);
        getCurrentUser();
      })
      .catch((err) => {
        updateLoader(false);
        console.log({ err });
      });
  };

  const updateUserPhoto = (value) => {
    let data = [
      {
        value: value,
        op: "add",
        path: "/image",
      },
    ];
    authServices
      .updateUserData(data, user.id)
      .then((res) => {
        updateLoader(false);
        let url = res.data.image.replace("\\", "/");
        setuserPic(url);
        openSnackbar("Profile photo updated sucessfully", 5000);
        getCurrentUser();
      })
      .catch((err) => {
        updateLoader(false);
      });
  };

  return (
    <>
      {/* {loading && <Loader />} */}
      <div
        className="user-photo"
        style={{
          position: "relative",
          backgroundImage: `url("${appUrl}${userPic}")`,
          backgroundSize: "cover",
          backgroundPosition: "center",
        }}
        title="change profile photo"
      >
        {!userPic && (
          <img
            src={userIcon}
            style={{ height: "85%", margin: " 12px auto" }}
            alt=""
          />
        )}
        <div className="edit_" onClick={toProfile}>
          <img
            src={editIcon}
            style={{ height: "20px", margin: "10px auto" }}
            alt=""
          />
        </div>
        <input
          type="file"
          style={{
            position: "absolute",
            top: 0,
            opacity: 0,
            left: 0,
            width: "100%",
            height: "100%",
            cursor: "pointer",
          }}
          onChange={(e) => toProfile(e)}
        />
      </div>
    </>
  );
}
