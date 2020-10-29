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
using StudyMATEUpload.Enums;
using System;

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

        private (string, string) GetCourseAndYear(string fileName, string ext)
        {
            var courseAndYear = fileName.Substring(0, fileName.IndexOf($".{ext}")).Split(" ");
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
                (string courseName, string year) = GetCourseAndYear(fileName, "docx");
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
                string section = null;
                string passage = null;
                Rep repForPassage = Rep.None;
                Rep repForSection = Rep.None;
                int countPassge = 0;
                int countSection = 0;
                int currentLoopPC = 0, currentLoopSC = 0;
                int previousCountPassge = 0, totalLoop = 0, previousCountSection = 0, totalLoopSection = 0;
                Rep currentRep = Rep.None;
                bool newPassage = false, newSection = false;
                StringBuilder sb = new StringBuilder(100);

                string GetLoopCount(string line, bool isPassage = true)
                {
                    int indexOfFirst = line.IndexOf('[');
                    int indexOfSecond = line.IndexOf(']');
                    var substringLoop = line[++indexOfFirst..indexOfSecond];
                    if (isPassage) int.TryParse(substringLoop, out currentLoopPC);
                    else int.TryParse(substringLoop, out currentLoopSC);

                    return line[..--indexOfFirst];
                }
                foreach (Paragraph paragraph in paragraphs)
                {
                    (string editedText, Rep rep, bool isNewRep) = HackLine(paragraph.InnerText, sb.ToString(), out bool status);

                    if (status)
                    {
                        if (rep == Rep.Passage)
                        {
                            repForPassage = Rep.Passage;
                        }
                        else if(rep == Rep.Section)
                        {
                            repForSection = Rep.Section;
                        }
                        if (editedText.EndsWith(']'))
                        {
                            if(repForPassage == Rep.Passage)
                            {
                                editedText = GetLoopCount(editedText);
                            }
                            else if(repForSection == Rep.Section)
                            {
                                editedText = GetLoopCount(editedText, isPassage: false);
                            }
                        }
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
                                    bool quizChanged = false;
                                    if((countPassge != previousCountPassge && previousCountPassge < totalLoop) || newPassage)
                                    {
                                        if (newPassage)
                                        {
                                            newPassage = false;
                                        }
                                        previousCountPassge = countPassge;
                                        if (!string.IsNullOrEmpty(passage))
                                        {
                                            if (countPassge == 1)
                                            {
                                                quiz.IsFirstPassage = true;
                                            }
                                            quiz.Passage = passage;
                                            quizChanged = true;
                                        }
                                    }
                                    
                                    if ((countSection != previousCountSection && previousCountSection < totalLoopSection) || newSection)
                                    {
                                        if (newSection)
                                        {
                                            newSection = false;
                                        }
                                        previousCountSection = countSection;
                                        if (!string.IsNullOrEmpty(section))
                                        {
                                            if (countSection == 1)
                                            {
                                                quiz.IsFirstSection = true;
                                            }
                                            quiz.Section = section;
                                            quizChanged = true;
                                        }
                                    }
                                    
                                    
                                    if (currentRep == Rep.Correct)
                                    {
                                        quiz.AnswerId = op.Id;
                                        quizChanged = true;

                                    }
                                    if (quizChanged)
                                    {
                                        await _quiz.Update(quiz);
                                    }
                                    break;
                                case Rep.Question:
                                    
                                    quiz = new Quiz { Question = mainText, TestId = test.Id, QuestionNumber = totalQuestions+1, IncludeThis = true, IsQuestionMathJax = HasMathJax(mainText) };
                                    (bool quizSucceeded, Quiz newQuiz, string quizError) = await _quiz.Add(quiz);
                                    if (quizSucceeded)
                                    {
                                        quiz = newQuiz;
                                        totalQuestions += 1;
                                        
                                        if (countPassge == totalLoop)
                                        {
                                            if (newPassage)
                                            {
                                                countPassge = 1;
                                                previousCountPassge = 0;
                                            }
                                            
                                        }
                                        else
                                        {
                                            countPassge += 1;
                                        }
                                        if (countSection == totalLoopSection)
                                        {
                                            if (newSection)
                                            {
                                                countSection = 1;
                                                previousCountSection = 0;
                                            }
                                           
                                        }
                                        else
                                        {
                                            countSection += 1;
                                        }
                                        if (newPassage)
                                        {
                                            totalLoop = currentLoopPC;
                                        }
                                        if (newSection)
                                        {
                                            totalLoopSection = currentLoopSC;
                                        }
                                    }
                                    else return BadRequest(new { Message = quizError });
                                    break;
                                case Rep.Passage:
                                    passage = mainText;
                                    repForPassage = Rep.None;
                                    newPassage = true;
                                    break;
                                case Rep.Section:
                                    section = mainText;
                                    repForSection = Rep.None;
                                    newSection = true;
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

        [HttpPost("specificdocupload")]
        public async Task<IActionResult> Post(IFormFile file, int testId)
        {
            if (file != null)
            {


                Test test = new Test { Id = testId };

                using WordprocessingDocument wordDocument = WordprocessingDocument.Open(file.OpenReadStream(), false);
                ICollection<Paragraph> paragraphs = wordDocument.MainDocumentPart.Document.Body.Elements<Paragraph>().ToList();
                Quiz quiz = null;
                int totalQuestions = 0;
                StudyMATEUpload.Enums.Level level = default;
                string section = null;
                string passage = null;
                Rep repForPassage = Rep.None;
                Rep repForSection = Rep.None;
                int countPassge = 0;
                int countSection = 0;
                int currentLoopPC = 0, currentLoopSC = 0;
                int previousCountPassge = 0, totalLoop = 0, previousCountSection = 0, totalLoopSection = 0;
                Rep currentRep = Rep.None;
                bool newPassage = false, newSection = false;
                StringBuilder sb = new StringBuilder(100);

                string GetLoopCount(string line, bool isPassage = true)
                {
                    int indexOfFirst = line.IndexOf('[');
                    int indexOfSecond = line.IndexOf(']');
                    var substringLoop = line[++indexOfFirst..indexOfSecond];
                    if (isPassage) int.TryParse(substringLoop, out currentLoopPC);
                    else int.TryParse(substringLoop, out currentLoopSC);

                    return line[..--indexOfFirst];
                }
                foreach (Paragraph paragraph in paragraphs)
                {
                    var lev = paragraph.InnerText;
                    if(!string.IsNullOrEmpty(lev) && lev.Trim().ToLower() != "beginner" && lev.Trim().ToLower() != "intermediate" && lev.Trim().ToLower() != "advanced")
                    {
                        if (!string.IsNullOrEmpty(lev) && lev[0] == '*')
                        {
                            lev = lev.Substring(1).Trim();
                            level = (StudyMATEUpload.Enums.Level)Enum.Parse(typeof(StudyMATEUpload.Enums.Level), lev);
                        }
                        else
                        {
                            (string editedText, Rep rep, bool isNewRep) = HackLine(paragraph.InnerText, sb.ToString(), out bool status);

                            if (status)
                            {
                                if (rep == Rep.Passage)
                                {
                                    repForPassage = Rep.Passage;
                                }
                                else if (rep == Rep.Section)
                                {
                                    repForSection = Rep.Section;
                                }
                                if (editedText.EndsWith(']'))
                                {
                                    if (repForPassage == Rep.Passage)
                                    {
                                        editedText = GetLoopCount(editedText);
                                    }
                                    else if (repForSection == Rep.Section)
                                    {
                                        editedText = GetLoopCount(editedText, isPassage: false);
                                    }
                                }
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
                                            bool quizChanged = false;
                                            if ((countPassge != previousCountPassge && previousCountPassge < totalLoop) || newPassage)
                                            {
                                                if (newPassage)
                                                {
                                                    newPassage = false;
                                                }
                                                previousCountPassge = countPassge;
                                                if (!string.IsNullOrEmpty(passage))
                                                {
                                                    if (countPassge == 1)
                                                    {
                                                        quiz.IsFirstPassage = true;
                                                    }
                                                    quiz.Passage = passage;
                                                    quizChanged = true;
                                                }
                                            }

                                            if ((countSection != previousCountSection && previousCountSection < totalLoopSection) || newSection)
                                            {
                                                if (newSection)
                                                {
                                                    newSection = false;
                                                }
                                                previousCountSection = countSection;
                                                if (!string.IsNullOrEmpty(section))
                                                {
                                                    if (countSection == 1)
                                                    {
                                                        quiz.IsFirstSection = true;
                                                    }
                                                    quiz.Section = section;
                                                    quizChanged = true;
                                                }
                                            }


                                            if (currentRep == Rep.Correct)
                                            {
                                                quiz.AnswerId = op.Id;
                                                quizChanged = true;

                                            }
                                            if (quizChanged)
                                            {
                                                await _quiz.Update(quiz);
                                            }
                                            break;
                                        case Rep.Question:

                                            quiz = new Quiz { Question = mainText, TestId = test.Id, Level = level, QuestionNumber = totalQuestions + 1, IncludeThis = true, IsQuestionMathJax = HasMathJax(mainText) };
                                            (bool quizSucceeded, Quiz newQuiz, string quizError) = await _quiz.Add(quiz);
                                            if (quizSucceeded)
                                            {
                                                quiz = newQuiz;
                                                totalQuestions += 1;

                                                if (countPassge == totalLoop)
                                                {
                                                    if (newPassage)
                                                    {
                                                        countPassge = 1;
                                                        previousCountPassge = 0;
                                                    }

                                                }
                                                else
                                                {
                                                    countPassge += 1;
                                                }
                                                if (countSection == totalLoopSection)
                                                {
                                                    if (newSection)
                                                    {
                                                        countSection = 1;
                                                        previousCountSection = 0;
                                                    }

                                                }
                                                else
                                                {
                                                    countSection += 1;
                                                }
                                                if (newPassage)
                                                {
                                                    totalLoop = currentLoopPC;
                                                }
                                                if (newSection)
                                                {
                                                    totalLoopSection = currentLoopSC;
                                                }
                                            }
                                            else return BadRequest(new { Message = quizError });
                                            break;
                                        case Rep.Passage:
                                            passage = mainText;
                                            repForPassage = Rep.None;
                                            newPassage = true;
                                            break;
                                        case Rep.Section:
                                            section = mainText;
                                            repForSection = Rep.None;
                                            newSection = true;
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
            if (int.TryParse(line, out var _))
            {
                status = false;
                return (null, Rep.None, false);
            }
            if (line.StartsWith('+'))
            {
                status = true;
                startIndex = line.IndexOf('+');
                return (line.Substring(startIndex + 1).Trim(), Rep.Passage, true);
            }
            else if (line.StartsWith('='))
            {
                status = true;
                startIndex = line.IndexOf('=');
                return (line.Substring(startIndex + 1).Trim(), Rep.Section, true);
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
            if (line.StartsWith('?') || (line.IndexOf('?') >= 0 && line.IndexOf('?') <= 10))
            {
                status = true;
                startIndex = line.IndexOf('?');
                return (line.Substring(startIndex + 1).Trim(), Rep.Question, true);
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
        [DisableRequestSizeLimit]
        public async Task<IActionResult> PostMusicAsync(IFormFile file)
        {
            if (ModelState.IsValid && file != null)
            {
                //if (_img.Create(file, out string path))
                //{
                //    var zipPath = Path.Combine(_env.WebRootPath, path);
                //    using ZipArchive archive = ZipFile.OpenRead(zipPath);
                //    var fileName = file.FileName;
                //    (string courseName, string year) = GetCourseAndYear(fileName, isZipped: true);
                //    if (!string.IsNullOrEmpty(courseName) && !string.IsNullOrEmpty(year))
                //    {
                //        var course = await _quiz.Item()
                //                        .Include(t => t.Test)
                //                            .ThenInclude(c => c.Course)
                //                        .Where(q =>
                //                                q.Test.Year.ToLower() == year.ToLower()
                //                                && q.Test.Course.Name.ToLower() == courseName).ToListAsync();
                //        if (course != null)
                //        {
                //            if (course.Count() == archive.Entries.Count)
                //            {
                //                for (int i = 0; i < archive.Entries.Count; i++)
                //                {
                //                    var updatedCourse = course.ElementAt(i);
                //                    updatedCourse.AudioUrl = archive.Entries.ElementAt(i).Name;
                //                    _quiz.UpdatePartly(updatedCourse);
                //                }
                //                (bool succeeded, string error) = await _quiz.SaveChangesAsync();
                //                if (succeeded) return NoContent();
                //                return BadRequest(new { Message = error });
                //            }
                //            return BadRequest((new { Message = "The total files do not match with the total questions for this course" }));
                //        }
                //    }

                //    return BadRequest((new { Message = "We could not locate this question in the db. Ensure we have this or rename the zip file accordingly" }));

                //}

                var fileName = file.FileName;
                (string courseName, string year) = GetCourseAndYear(fileName, "txt");
                using var reader = new StreamReader(file.OpenReadStream());
                var contents = reader.ReadToEnd().Trim().Split(" ");
                var course = await _quiz.Item()
                                        .Include(t => t.Test)
                                            .ThenInclude(c => c.Course)
                                        .Where(q =>
                                                q.Test.Year.ToLower() == year.ToLower()
                                                && q.Test.Course.Name.ToLower() == courseName.ToLower()).ToListAsync();
                if (course != null)
                {
                    if (course.Count() == contents.Length)
                    {
                        for (int i = 0; i < contents.Length; i++)
                        {
                            var updatedCourse = course.ElementAt(i);
                            updatedCourse.AudioUrl = $"audios/{courseName}/{year}/{contents[i]}";
                            _quiz.UpdatePartly(updatedCourse);
                        }
                        (bool succeeded, string error) = await _quiz.SaveChangesAsync();
                        if (succeeded) return NoContent();
                        return BadRequest(new { Message = error });
                    }
                    return BadRequest(new { Message = "We could not add this resource. Numbers do not match. Please try again" });
                }
            }
            return BadRequest(new { Message = "Your data is bad" });
        }

        [HttpPost("editpassage")]
        public async Task<IActionResult> AddMusic(IFormFile file)
        {
            //int[] allowable = { 2000, 2001, 2002, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017 };
            var fileName = file.FileName;
            var courseName = fileName.Substring(0, fileName.IndexOf(".txt"));
            using var reader = new StreamReader(file.OpenReadStream());
            int yearInt = 0;
            string year = null;
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    if (int.TryParse(line, out yearInt))
                    {
                        year = yearInt.ToString();
                    }
                    if (line.StartsWith("Passage") || line.StartsWith("Section"))
                    {
                        var quiz = await _quiz.Item().Include(q => q.Test)
                                                        .ThenInclude(t => t.Course)
                                                     .Where(q => q.Test.Course.Name.ToLower() == courseName.ToLower()
                                                            && q.Test.Year == year).ToArrayAsync();
                        if (line.StartsWith("Passage"))
                        {
                            var indexOfP = line.IndexOf(':') + 1;
                            var remaining = line[indexOfP..].Trim().Split(',');
                            for (int i = 0; i < quiz.Count(); i++)
                            {

                                if (!remaining.Contains((i + 1).ToString()))
                                {
                                    var editPass = quiz.ElementAt(i);
                                    editPass.Passage = null;
                                    await _quiz.Update(editPass);
                                }
                            }
                        }
                        if (line.StartsWith("Section"))
                        {
                            var indexOfP = line.IndexOf(':') + 1;
                            var remaining = line[indexOfP..].Trim().Split(',');
                            for (int i = 0; i < quiz.Count(); i++)
                            {

                                if (!remaining.Contains((i + 1).ToString()))
                                {
                                    var editPass = quiz.ElementAt(i);
                                    editPass.Section = null;
                                    await _quiz.Update(editPass);
                                }
                            }
                        }
                    }
                }
                
                
            }

            return NoContent();
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
        Question, Option, Correct, Answer, Continue, Passage, Section, None
    }
}


