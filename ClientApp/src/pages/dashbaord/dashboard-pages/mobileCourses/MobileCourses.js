import React, { useState } from "react";
import Courses from "../../../../components/courses/Courses";
import { motion } from "framer-motion";
import AddCourses from "../../../../components/add-courses/AddCourses";

export default function MobileCourses() {
  const [open, setopen] = useState(false);
  const onCloseModal = () => setopen(false);
  const onOpenModal = () => {
    setopen(true);
  };
  return (
    <motion.div className="mobile-courses" style={{ padding: "30px 20px" }}>
      <Courses onOpenModal={onOpenModal} moveToCourse={true} />
      <AddCourses open={open} onCloseModal={onCloseModal} />
    </motion.div>
  );
}
