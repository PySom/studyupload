using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Services;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models;
using System.Threading.Tasks;
using System.Text;

namespace LAAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IImageService _img;

        private readonly IModelManager<Course> _course;
        private readonly IModelManager<Test> _test;
        private readonly IModelManager<Quiz> _quiz;
        private readonly IModelManager<Option> _option;

        public FilesController(
            IImageService image,
            IModelManager<Course> course,
            IModelManager<Test> test,
            IModelManager<Quiz> quiz,
            IModelManager<Option> option)
        {
            _img = image;
            _course = course;
            _test = test;
            _quiz = quiz;
            _option = option;
        }

        [HttpPost("docupload")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file != null)
            {
                string fileName = file.FileName;
                var courseAndYear = fileName.Substring(0, fileName.IndexOf(".doc")).Split(" ");
                var courseName = courseAndYear[0];
                var year = courseAndYear[1];
                Course course = null;
                if (!string.IsNullOrEmpty(courseName))
                {
                    //Handling courses
                    course = _course.Item().Where(c => c.Name.ToLower() == courseName.ToLower()).FirstOrDefault();
                    if (course == null)
                    {
                        (bool succeded, Course cc, string error) = await _course.Add(new Course { Name = courseName });
                        if (succeded) course = cc;
                        else return BadRequest(new { Message = error });
                    }
                }
                
                Test test = null;
                if (!string.IsNullOrEmpty(year) && course != null)
                {
                    //Handling tests
                    (bool succeded, Test tt, string error) = await _test.Add(new Test { Year = year, CourseId = course.Id });
                    if (succeded) test = tt;
                    else return BadRequest(new { Message = error });
                }

                using WordprocessingDocument wordDocument = WordprocessingDocument.Open(file.OpenReadStream(), false);
                ICollection<Paragraph> paragraphs = wordDocument.MainDocumentPart.Document.Body.Elements<Paragraph>().ToList();
                Quiz quiz = null;
                Rep currentRep = Rep.None;
                StringBuilder sb = new StringBuilder(100);
                foreach (Paragraph paragraph in paragraphs)
                {
                    (string editedText, Rep rep, bool isNewRep) = HackLine(paragraph.InnerText, sb.ToString(), out bool status);

                    if (status)
                    {
                        if (isNewRep && string.IsNullOrEmpty(sb.ToString()) || rep == Rep.Continue)
                        {
                            if (string.IsNullOrEmpty(sb.ToString()))
                            {
                                currentRep = rep;
                            }
                            sb.Append(editedText + "\n");
                        }
                        else
                        {
                            string mainText = sb.ToString().Trim();
                            switch (currentRep)
                            {
                                case Rep.Answer:
                                    quiz.AnswerUrl = mainText;
                                    (bool succeded, Quiz qq, string error) = await _quiz.Update(quiz);
                                    quiz = qq;
                                    break;
                                case Rep.Option:
                                case Rep.Correct:
                                    Option option = new Option { IsMathJax = HasMathJax(mainText), Content = mainText, PracticeId = quiz.Id };
                                    (bool optionSucceeded, Option op, string optionError) = await _option.Add(option);
                                    if (rep == Rep.Correct)
                                    {
                                        quiz.AnswerId = op.Id;
                                        await _quiz.Update(quiz);
                                    }
                                    break;
                                case Rep.Question:
                                    quiz = new Quiz { Question = mainText, TestId = test.Id, IsQuestionMathJax = HasMathJax(mainText) };
                                    (bool quizSucceeded, Quiz newQuiz, string quizError) = await _quiz.Add(quiz);
                                    if (quizSucceeded) quiz = newQuiz;
                                    else return BadRequest(new { Message = quizError });
                                    break;
                                case Rep.None:
                                    break;

                            }
                            sb = new StringBuilder(100);
                            sb.Append(editedText + "\n");
                            currentRep = rep;
                        }
                        
                    }
                }
                return NoContent();
            }
            return BadRequest(new { Message = "Supply a file please" });
        }

        private bool HasMathJax(string text) => text.Contains('\\');

        private (string, Rep, bool) HackLine(string line, string previousText, out bool status)
        {
            int startIndex;
            line = line?.Trim();
            if (line.StartsWith('?') || (line.IndexOf('?') >= 0 && line.IndexOf('?') <= 10))
            {
                status = true;
                startIndex = line.IndexOf('?');
                return (line.Substring(startIndex+1).Trim(), Rep.Question, true);
            }
            else if (line.StartsWith('#') || (line.IndexOf('#') >= 0 && line.IndexOf('#') <= 10))
            {
                status = true;
                startIndex = line.IndexOf('#');
                return (line.Substring(startIndex+1).Trim(), Rep.Option, true);
            }
            else if (line.StartsWith('/') || (line.IndexOf('/') >= 0 && line.IndexOf('/') <= 10))
            {
                status = true;
                startIndex = line.IndexOf('/');
                return (line.Substring(startIndex+1).Trim(), Rep.Correct, true);
            }
            else if (line.StartsWith('@') || (line.IndexOf('@') >= 0 && line.IndexOf('@') <= 10))
            {
                status = true;
                startIndex = line.IndexOf('@');
                return (line.Substring(startIndex+1).Trim(), Rep.Answer, true);
            }
            else if (!string.IsNullOrEmpty(previousText))
            {
                status = true;
                return (line.Trim(), Rep.Continue, false);
            }
            else
            {
                status = false;
                return (null, Rep.None, false);
            }
        }



        [HttpPost("upload")]
        public IActionResult Post([FromForm]FileViewModel model)
        {
            if(ModelState.IsValid && model.File != null)
            {
                if(_img.Create(model.File, out string path))
                {
                    return Ok(new { Name = path });
                }
                return BadRequest(new { Message = "We could not add this resource. Please try again" });
            }
            return BadRequest(new { Message = "Your data is bad" });
        }

        [HttpPut("edit")]
        public IActionResult Put([FromForm]FileEditViewModel model)
        {
            if (ModelState.IsValid && model.File != null)
            {
                if (_img.Edit(model.File, model.OldImage, out string path))
                {
                    return Ok(new { Name = path });
                }
                return BadRequest(new { Message = "We could not add this resource. Please try again" });
            }
            return BadRequest(new { Message = "Your data is bad" });
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromBody]FileDeleteViewModel model)
        {
            if (ModelState.IsValid)
            {
                _img.Delete(model.Image);
                return NoContent();
            }
            return BadRequest(new { Message = "you need to supply an image to remove" });
        }
    }

    public enum Rep : byte
    {
        Question, Option, Correct, Answer, Continue, None
    }
}


