using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class GenericController<T, TV, TD> : ControllerBase 
        where T : class, IModel, new()
        where TV : class, new()
        where TD : class, new()
    {
        protected readonly IModelManager<T> _repo;
        protected readonly IMapper _mapper;
        public GenericController(IModelManager<T> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        [HttpGet]
        public virtual async ValueTask<IActionResult> GetAll(int page = 0)
        {
            int itemsPerPage = 20;
            ICollection<T> options = new List<T>();
            if(page == 0)
            {
                options = await _repo
                                .Item()
                                .ToListAsync();
            }
            else
            {
                options = await _repo
                                .Item()
                                .Skip(page * itemsPerPage - itemsPerPage)
                                .Take(itemsPerPage)
                                .ToListAsync();
            }
                
            return Ok(_mapper.Map<ICollection<T>, ICollection<TD>>(options));

        }


        [HttpGet("{id:int}")]
        public virtual async ValueTask<IActionResult> Get(int id)
        {
            T model = await _repo
                                .Item()
                                .Where(c => c.Id == id)
                                .FirstOrDefaultAsync();
            if (model != null)
            {
                return Ok(_mapper.Map<T, TD>(model));
            }
            return NotFound();
        }


        [HttpPost]
        public virtual async ValueTask<IActionResult> Post([FromBody] TV model)
        {
            if (ModelState.IsValid)
            {
                (bool succeeded, T t, string error) = await _repo.Add(_mapper.Map<TV, T>(model));
                if (succeeded) return Ok(_mapper.Map<T, TD>(t));
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }

        [HttpPost("multiple")]
        public virtual async ValueTask<IActionResult> Post([FromBody] ICollection<TV> models)
        {
            if (ModelState.IsValid)
            {
                (bool succeeded, ICollection<T> t, string error) = await _repo.Add(_mapper.Map<ICollection<TV>, ICollection<T>>(models));
                if (succeeded) return Ok(_mapper.Map<ICollection<T>, ICollection<TD>>(t));
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }


        [HttpPut("{id:int}")]
        public virtual async ValueTask<IActionResult> Put(int id, [FromBody] TV model)
        {
            if (ModelState.IsValid)
            {
                var newModel = _mapper.Map<TV, T>(model);
                newModel.Id = id;
                (bool succeeded, T t, string error) = await _repo.Update(newModel);
                if (succeeded) return Ok(_mapper.Map<T, TD>(t));
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }

        [HttpPut]
        public virtual async ValueTask<IActionResult> Put(T model)
        {
            if (ModelState.IsValid)
            {
                (bool succeeded, T t, string error) = await _repo.Update(model);
                if (succeeded) return Ok(_mapper.Map<T, TD>(t));
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }

        [HttpPut("multiple")]
        public IActionResult Put([FromBody]ICollection<T> models)
        {
            if (ModelState.IsValid)
            {
                var (succeeded, t, error) = _repo.Update(models);
                if (succeeded) return Ok(t);
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }

        [HttpPatch("{id:int}")]
        public virtual async ValueTask<IActionResult> Put([FromBody]JsonPatchDocument<T> patchDoc, int id)
        {
            var model = await _repo.Item().FindAsync(id);
            if (model != null)
            {
                patchDoc.ApplyTo(model, ModelState);
                if (ModelState.IsValid)
                {
                    (bool succeeded, T t, string error) = await _repo.Update(model);
                    if (succeeded) return Ok(_mapper.Map<T, TD>(t));
                    return BadRequest(new { Message = error });
                }
                return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
            }
            return BadRequest(new { Message = "No such item" });


        }

        [HttpDelete("{id}")]
        public virtual async ValueTask<IActionResult> Delete(int id)
        {
            T t = new T { Id = id };
            string message;
            try
            {
                (bool succeeded, string error) = await _repo.Delete(t);
                message = error;
                if (succeeded) return NoContent();
                return BadRequest(new { message });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                message = ex.Message + " " + ex.InnerException.Message;
            }
            return BadRequest(new { Message = message });
        }


        [HttpDelete]
        public async ValueTask<IActionResult> Delete(T model)
        {
            if (model == null) return NotFound();
            string message;
            try
            {
                (bool succeeded, string error) = await _repo.Delete(model);
                message = error;
                if (succeeded) return NoContent();
                return BadRequest(new { message });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                message = ex.Message + " " + ex.InnerException.Message;
            }
            return BadRequest(new { Message = message });
        }

        protected async ValueTask<IActionResult> Hide(T model)
        {
            if (model == null) return NotFound();
            string message;
            try
            {
                (bool succeeded, string error) = await _repo.Hide(model);
                message = error;
                if (succeeded) return NoContent();
                return BadRequest(new { message });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                message = ex.Message + " " + ex.InnerException.Message;
            }
            return BadRequest(new { Message = message });
        }
    }
}
