using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyMATEUpload.Models.ViewModels;
using System;
using Microsoft.AspNetCore.JsonPatch;
using StudyMATEUpload.Models.DTOs;
using StudyMATEUpload.Enums;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizesController : ControllerBase
    {
        private readonly IModelManager<Quiz> _repo;
        private readonly IMapper _mapper;
        public QuizesController(IModelManager<Quiz> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async ValueTask<IActionResult> Get()
        {
            ICollection<Quiz> quizes = await _repo
                                            .Item()
                                            .OrderBy(q => q.QuestionNumber)
                                            .ToListAsync();
            return Ok(quizes);

        }

        [HttpGet("count/{courseId:int}")]
        public IActionResult GetCount(int courseId, StudyType stype = StudyType.StudyMate)
        {
            var total = _repo
                            .Item()
                            .Where(q => q.Test.CourseId == courseId && q.IncludeThis && q.Test.StudyType == stype)
                            .Count();
            return Ok(new { Score = total });

        }

        [HttpGet("{id:int}")]
        public async ValueTask<IActionResult> Get(int id, bool child = false, StudyType study = StudyType.Main)
        {
            if (child)
            {
                if(study == StudyType.StudyMate)
                {
                    return Ok(await _repo.Item().Where(q => q.TestId == id)
                    .Include(q => q.Options)
                    .OrderBy(q => q.QuestionNumber)
                    .ToListAsync());
                }
                return Ok(await _repo.Item().Where(q => q.TestId == id && q.IncludeThis)
                    .Include(q => q.Options)
                    .OrderBy(q => q.QuestionNumber)
                    .ToListAsync());
            }
            Quiz model = await _repo.Item().Where(c => c.Id == id).FirstOrDefaultAsync();
            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }



        [HttpPost]
        public async ValueTask<IActionResult> Post([FromBody] QuizViewModel model)
        {
            if (ModelState.IsValid)
            {
                (bool succeeded, Quiz quiz, string error) = await _repo.Add(_mapper.Map<QuizViewModel, Quiz>(model));
                if (succeeded) return Ok(quiz);
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }

        [HttpPut("{id:int}")]
        public async ValueTask<IActionResult> Put(int id, [FromBody] QuizViewModel model)
        {
            var newModel = _mapper.Map<QuizViewModel, Quiz>(model);
            newModel.Id = id;
            if (ModelState.IsValid)
            {
                (bool succeeded, Quiz quiz, string error) = await _repo.Update(newModel);
                if (succeeded) return Ok(quiz);
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }


        [HttpPatch("{id:int}")]
        public virtual async ValueTask<IActionResult> Put([FromBody]JsonPatchDocument<Quiz> patchDoc, int id)
        {
            var model = await _repo.Item().FindAsync(id);
            if (model != null)
            {
                patchDoc.ApplyTo(model, ModelState);
                if (ModelState.IsValid)
                {
                    (bool succeeded, Quiz t, string error) = await _repo.Update(model);
                    if (succeeded) return Ok(_mapper.Map<Quiz, QuizDTO>(t));
                    return BadRequest(new { Message = error });
                }
                return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
            }
            return BadRequest(new { Message = "No such item" });


        }


        [HttpPut("updatenumber")]
        public async ValueTask<IActionResult> UpdateNumber([FromBody] UpdateQuizNumberDTO model)
        {
            if (ModelState.IsValid)
            {
                var models = new List<Quiz>();
                try
                {
                    models = await _repo.Item().Where(m => m.TestId == model.TestId).ToListAsync();

                }
                catch(Exception ex)
                {
                    return BadRequest(new { Error = ex.Message });
                }
                var item = models.Where(m => m.Id == model.Id).FirstOrDefault();
                if (item != null)
                {
                    var listOfNumbers = models.Select(m => m.QuestionNumber);
                    if (listOfNumbers.Contains(model.QuestionNumber))
                    {
                        var indexOfCurrentOwner = models.IndexOf(models.Where(m => m.QuestionNumber == model.QuestionNumber).First());
                        int inc = models[indexOfCurrentOwner].QuestionNumber + 1;
                        for (int i = indexOfCurrentOwner; i < models.Count; i++)
                        {
                            if(models[i].Id != model.Id)
                            {
                                models[i].QuestionNumber = inc;
                                await _repo.Update(models[i]);
                                inc += 1;
                            }
                            
                        }
                    }
                    item.QuestionNumber = model.QuestionNumber;
                    await _repo.Update(item);
                    return NoContent();
                }
                return NotFound();
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }

        [HttpPut("updatenumberaudio")]
        public async ValueTask<IActionResult> UpdateNumberAudio(int testId, int number)
        {
            if (ModelState.IsValid)
            {
                var models = new List<Quiz>();
                try
                {
                    models = await _repo.Item().Where(m => m.TestId == testId).OrderBy(q => q.QuestionNumber).ToListAsync();
                    models = models.Skip(number - 1).ToList();

                }
                catch(Exception ex)
                {
                    return BadRequest(new { Error = ex.Message });
                }
                //var item = models.Where(m => m.QuestionNumber == number).FirstOrDefault();
                var audio = models.Where(m => !string.IsNullOrEmpty(m.AudioUrl)).Select(m => m.AudioUrl).FirstOrDefault();
                var t = audio.Split('Q');
                var m = t[0];
                var pos = number;
                var re = new List<Quiz>();
                foreach (var item in models)
                {
                    item.AudioUrl = m + "Q" + pos + ".mp3";
                    re.Add(item);
                    pos += 1;
                }
                if(re.Count > 1)
                {
                    var (success, mods, error) = _repo.Update(re);
                    if (success) return NoContent();
                }
                return NotFound();
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }


        [HttpDelete("{id}")]
        public async ValueTask<IActionResult> Delete(int id)
        {
            Quiz quiz = new Quiz { Id = id };
            string message;
            try
            {
                (bool succeeded, string error) = await _repo.Delete(quiz);
                message = error;
                if (succeeded) return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                message = ex.Message;
            }
            return NotFound(new { Message = message });
        }
    }
}