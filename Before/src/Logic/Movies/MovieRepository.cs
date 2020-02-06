using System.Collections.Generic;
using System.Linq;
using Logic.Utils;
using Logic.Common;

namespace Logic.Movies
{
    public class MovieRepository : Repository<Movie>
    {
        public MovieRepository(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IReadOnlyList<Movie> GetList()
        {
            return _unitOfWork.Query<Movie>().ToList();
        }
    }
}
