using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionsController : ControllerBase
    {
        private readonly IModelManager<Option> _repo;
        private readonly IModelManager<Quiz> _quiz;
        private readonly IMapper _mapper;
        public OptionsController(IModelManager<Option> repo,
            IModelManager<Quiz> quiz,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _quiz = quiz;
        }

        [HttpGet]
        public async ValueTask<IActionResult> Get()
        {
            ICollection<Option> options = await _repo
                                            .Item()
                                            .ToListAsync();
            return Ok(options);

        }

        [HttpGet("{id:int}")]
        public async ValueTask<IActionResult> Get(int id)
        {
            Option model = await _repo
                                .Item()
                                .Where(c => c.Id == id)
                                .FirstOrDefaultAsync();
            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }


        [HttpPost]
        public async ValueTask<IActionResult> Post([FromBody] Option model, bool isCorrect)
        {
            if (ModelState.IsValid)
            {
                (bool succeeded, Option option, string error) = await _repo.Add(model);
                if (succeeded)
                {
                    if (isCorrect)
                    {
                        
                        try
                        {
                            var quiz = await _quiz.FindOne(c => c.Id == model.PracticeId && !c.Deleted);
                            quiz.AnswerId = option.Id;
                            var _ = await _quiz.Update(quiz);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    return Ok(option);
                }
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }


        [HttpPut]
        public async ValueTask<IActionResult> Put([FromBody] Option model)
        {
            if (ModelState.IsValid)
            {
                (bool succeeded, Option option, string error) = await _repo.Update(model);
                if (succeeded) return Ok(option);
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }

        [HttpDelete("{id}")]
        public async ValueTask<IActionResult> Delete(int id)
        {
            Option option = new Option { Id = id };
            string message;
            try
            {
                (bool succeeded, string error) = await _repo.Delete(option);
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