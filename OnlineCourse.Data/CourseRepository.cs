using Microsoft.EntityFrameworkCore;
using OnlineCourse.Core.Models;
using OnlineCourse.Data.Entities;


namespace OnlineCourse.Data
{
    public class CourseRepository : ICourseRepository
    {
        private readonly OnlineCourseDbContext dbContext;

        public CourseRepository(OnlineCourseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<CourseModel>> GetAllCoursesAsync(int? categorryId = null)
        {
            //we first build a query to dynamic apply categoryId filter if it was sent
            //this query called differed execution, it wont run until we await or use sync methods like .ToList()
            var query = dbContext.Courses
                                 .Include(c => c.Category)
                                 .AsQueryable();
            //Apply the filter if categoryId is provided
            if (categorryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categorryId.Value);
            }

            var courses = await query
                .Select(c => new CourseModel()
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price,
                    CourseType = c.CourseType,
                    SeatsAvailable = c.SeatsAvailable,
                    Duration = c.Duration,
                    CategoryId = c.CategoryId,
                    InstructorId = c.InstructorId,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,

                    Category = new CourseCategoryModel()
                    {
                        CategoryId = c.Category.CategoryId,
                        CategoryName = c.Category.CategoryName,
                        Description = c.Category.Description,
                    },

                    UserRating = new UserRatingModel
                    {
                        CourseId = c.CourseId,
                        AverageRating = c.Reviews.Any() ? Convert.ToDecimal(c.Reviews.Average(r => r.Rating)) : 0,
                        TotalRating = c.Reviews.Count
                    }

                }).ToListAsync();

            return courses;
        }

        public async Task<CourseDetailModel> GetCourseDetailAsync(int courseId)
        {
            /*
             * //this means to pull all the associted records for a given course
             * Also, it is possible only when tables are related. parent/child (primary key/foreign key)
             
             */
            var course = await dbContext.Courses
                .Include(c => c.Category)
                .Include(r => r.Reviews)
                .Include(s => s.SessionDetails)
                .Where(c => c.CourseId == courseId)
                //to convert the data we have to a particular return model we use Select(linq) in fact all of these are linq
                .Select(c => new CourseDetailModel()
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price,
                    CourseType = c.CourseType,
                    SeatsAvailable = c.SeatsAvailable,
                    Duration = c.Duration,
                    CategoryId = c.CategoryId,
                    InstructorId = c.InstructorId, 
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,

                    Category = new CourseCategoryModel()
                    {
                        CategoryId = c.Category.CategoryId,
                        CategoryName = c.Category.CategoryName,
                        Description = c.Category.Description,
                    },

                    Reviews = c.Reviews.Select(r=>new UserReviewModel()
                    {
                        CourseId=r.CourseId,
                        UserName=r.User.DisplayName,
                        Rating=r.Rating,
                        Comments=r.Comments,
                        ReviewDate=r.ReviewDate,
                    }).OrderByDescending(o=>o.Rating).Take(10).ToList(),//want to sort and take only top 10

                    SessionDetails = c.SessionDetails.Select(sd=>new SessionDetailModel
                    {
                        SessionId=sd.SessionId,
                        CourseId = sd.CourseId,
                        Title = sd.Title,
                        Description = sd.Description,
                        VideoUrl = sd.VideoUrl,
                        VideoOrder = sd.VideoOrder,
                    }).OrderBy(o=>o.VideoOrder).ToList(), //order the course session by its proper order

                    UserRating = new UserRatingModel
                    {
                        CourseId=c.CourseId,
                        AverageRating=c.Reviews.Any()?Convert.ToDecimal(c.Reviews.Average(r=>r.Rating)):0,
                        TotalRating=c.Reviews.Count
                    }
                }).FirstOrDefaultAsync();

            return course;

        }
    }
}
