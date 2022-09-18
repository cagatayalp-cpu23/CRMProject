using CRMProject.Data;
using CRMProject.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CRMProject.ViewModels.Common;
using LinqKit;

namespace CRMProject.Service.Admin
{


    public class ProblemService
    {
        private readonly CrmDbTestEntities _context;

        public ProblemService(CrmDbTestEntities context)
        {
            _context = context;
        }

        public List<ProblemViewModel> Get()
        {
            var reference = (from b in _context.ProblemSet
                select new ProblemViewModel()
                {
                    Id = b.Id,
                    Name = b.Name

                }).ToList();
            return reference;
        }
        public ProblemViewModel Get2()
        {
            var reference = new ProblemViewModel();
            return reference;
        }
        public async Task<ServiceCallResult> Get3(ProblemViewModel problemViewModel)
        {
            var callResult = new ServiceCallResult() { Success = false };
            var problem = new ProblemSet()
            {
                Id = problemViewModel.Id,
                Name = problemViewModel.Name
            };
            _context.ProblemSet.Add(problem);
            using (var dbTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbTransaction.Commit();
                    callResult.Success = true;
                    callResult.Item = problem.Id;
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }
        }
        public  async Task<ProblemViewModel> GetProblemListViewAsync(int problemId)
        {
            var predicate = PredicateBuilder.New<Data.ProblemSet>(true);/*AND*/
            predicate.And(a => a.Id == problemId);
            return await _getProblemListIQueryable(predicate).FirstOrDefaultAsync().ConfigureAwait(false);
        }
        private IQueryable<ProblemViewModel> _getProblemListIQueryable(Expression<Func<Data.ProblemSet, bool>> expr)
        {
            return (from b in _context.ProblemSet.AsExpandable().Where(expr)
                select new ProblemViewModel()
                {
                    
                    Name =b.Name

                });
        }
        public async Task<ServiceCallResult> DeleteAsync(int problemId)
        {
            //var model=_context.TaskSet.FirstOrDefault(x=>x.Id==id);   
            var callResult = new ServiceCallResult() { Success = false };
            var problem = await _context.ProblemSet.FirstOrDefaultAsync(a => a.Id == problemId).ConfigureAwait(false);
            if (problem == null)
            {
                callResult.ErrorMessages.Add("Böyle bir problem bulunamadı.");
                return callResult;
            }
            _context.ProblemSet.Remove(problem);
            callResult.Success = true;
           await _context.SaveChangesAsync().ConfigureAwait(false);
            return callResult;
        }
    }
}
