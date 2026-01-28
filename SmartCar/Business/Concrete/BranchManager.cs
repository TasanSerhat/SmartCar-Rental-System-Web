using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class BranchManager : IBranchService
    {
        IBranchDal _branchDal;

        public BranchManager(IBranchDal branchDal)
        {
            _branchDal = branchDal;
        }

        public IResult Add(Branch branch)
        {
            _branchDal.Add(branch);
            return new SuccessResult("Branch added successfully");
        }

        public IResult Delete(Branch branch)
        {
            _branchDal.Delete(branch);
            return new SuccessResult("Branch deleted successfully");
        }

        public IDataResult<List<Branch>> GetAll()
        {
            return new SuccessDataResult<List<Branch>>(_branchDal.GetAll());
        }

        public IDataResult<Branch> GetById(int branchId)
        {
            return new SuccessDataResult<Branch>(_branchDal.Get(b => b.BranchId == branchId));
        }

        public IResult Update(Branch branch)
        {
            _branchDal.Update(branch);
            return new SuccessResult("Branch updated successfully");
        }
    }
}
