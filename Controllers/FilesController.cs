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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore;

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
        private readonly IWebHostEnvironment _env;

        public FilesController(
            IImageService image,
            IModelManager<Course> course,
            IModelManager<Test> test,
            IModelManager<Quiz> quiz,
            IModelManager<Option> option,
            IWebHostEnvironment env)
        {
            _img = image;
            _course = course;
            _test = test;
            _quiz = quiz;
            _option = option;
            _env = env;
        }

        private (string, string) GetCourseAndYear(string fileName, bool isZipped = false)
        {
            var courseAndYear = isZipped ? fileName.Substring(0, fileName.IndexOf(".zip")).Split(" ") : fileName.Substring(0, fileName.IndexOf(".doc")).Split(" ");
            var courseName = string.Join(' ', courseAndYear[..^1]);
            var year = courseAndYear[^1];
            return (courseName, year);
        }

        [HttpPost("docupload")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file != null)
            {
                string fileName = file.FileName;
                (string courseName, string year) = GetCourseAndYear(fileName);
                Course course = null;
                if (!string.IsNullOrEmpty(courseName))
                {
                    //Handling courses
                    course = _course.Item().Where(c => c.Name.ToLower() == courseName.ToLower()).FirstOrDefault();
                    if (course == null)
                    {
                        (bool succeded, Course cc, string error) = await _course.Add(new Course { Name = courseName.ToUpper() });
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
                int totalQuestions = 0;
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
                                    if (currentRep == Rep.Correct)
                                    {
                                        quiz.AnswerId = op.Id;
                                        await _quiz.Update(quiz);
                                    }
                                    break;
                                case Rep.Question:
                                    quiz = new Quiz { Question = mainText, TestId = test.Id, IsQuestionMathJax = HasMathJax(mainText) };
                                    (bool quizSucceeded, Quiz newQuiz, string quizError) = await _quiz.Add(quiz);
                                    if (quizSucceeded)
                                    {
                                        quiz = newQuiz;
                                        totalQuestions += 1;
                                    }
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
                test.QuestionNo = totalQuestions;
                test.Duration = totalQuestions * 2;
                await _quiz.Update(quiz);
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


        [HttpPost("musicupload")]
        public async Task<IActionResult> PostMusicAsync(IFormFile file)
        {
            if (ModelState.IsValid && file != null)
            {
                if (_img.Create(file, out string path))
                {
                    var zipPath = Path.Combine(_env.WebRootPath, path);
                    using ZipArchive archive = ZipFile.OpenRead(zipPath);
                    var fileName = file.FileName;
                    (string courseName, string year) = GetCourseAndYear(fileName, isZipped: true);
                    if (!string.IsNullOrEmpty(courseName) && !string.IsNullOrEmpty(year))
                    {
                        var course = _quiz.Item()
                                        .Include(t => t.Test)
                                            .ThenInclude(c => c.Course)
                                        .Where(q =>
                                                q.Test.Year.ToLower() == year.ToLower()
                                                && q.Test.Course.Name.ToLower() == courseName);
                        if (course != null)
                        {
                            if (course.Count() == archive.Entries.Count)
                            {
                                for (int i = 0; i < archive.Entries.Count; i++)
                                {
                                    var updatedCourse = course.ElementAt(i);
                                    updatedCourse.AudioUrl = archive.Entries.ElementAt(i).Name;
                                    _quiz.UpdatePartly(updatedCourse);
                                }
                                (bool succeeded, string error) = await _quiz.SaveChangesAsync();
                                if (succeeded) return NoContent();
                                return BadRequest(new { Message = error });
                            }
                            return BadRequest((new { Message = "The total files do not match with the total questions for this course" }));
                        }
                    }
                    
                    return BadRequest((new { Message = "We could not locate this question in the db. Ensure we have this or rename the zip file accordingly" }));

                }
                return BadRequest(new { Message = "We could not add this resource. Please try again" });
            }
            return BadRequest(new { Message = "Your data is bad" });
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


