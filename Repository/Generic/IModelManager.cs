using StudyMATEUpload.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace StudyMATEUpload.Repository.Generics
{
    public interface IModelManager<TModel> 
        where TModel: class, IModel
    {
        ValueTask<(bool, TModel, string)> Add(TModel model);
        ValueTask<(bool, ICollection<TModel>, string)> Add(IEnumerable<TModel> models);
        ValueTask<(bool, TModel, string)> Update(TModel model);
        public (bool, ICollection<TModel>, string) Update(ICollection<TModel> models);
        void UpdatePartly(TModel model);
        Task<(bool, string)> SaveChangesAsync();

        ValueTask<(bool, string)> Delete(TModel model);
        ValueTask<(bool, string)> Delete(IEnumerable<TModel> model);
        ValueTask<(bool, string)> Hide(IEnumerable<TModel> models);
        ValueTask<(bool, string)> Hide(TModel model);

        ValueTask<TModel> FindOne(Expression<Func<TModel, bool>> query);
        ValueTask<ICollection<TModel>> FindMany(Expression<Func<TModel, bool>> query);

        DbSet<TModel> Item();
    }
}
