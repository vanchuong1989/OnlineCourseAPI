using Microsoft.EntityFrameworkCore;
using OnlineCourse.Core.Entities;
using OnlineCourse.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Data
{
    //this is primary constructor and availabel from .net 8


    //public class CourseCategoryRepository(OnlineCourseDbContext dbContext) : ICourseCategoryRepository
    //{
    //    private readonly OnlineCourseDbContext dbContext= dbContext;


    //    /*
    //     * these are sync methods
    //    public CourseCategory? GetById(int id)
    //    {
    //        var data = dbContext.CourseCategories.Find(id);//find is the sync method will try to find record by primary key
    //        return data;
    //    }

    //    public List<CourseCategory> GetCourseCategories()
    //    {
    //        var data = dbContext.CourseCategories.ToList();
    //        return data;
    //    }
    //    */

    //    //implement async 

    //}

    public class CourseCategoryRepository : ICourseCategoryRepository
    {
        private readonly OnlineCourseDbContext dbContext;

        public CourseCategoryRepository(OnlineCourseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Task<CourseCategory?> GetByIdAsync(int id)
        {
            var data = dbContext.CourseCategories.FindAsync(id).AsTask();
            return data;
        }

        public Task<List<CourseCategory>> GetCourseCategoriesAsync()
        {
            var data = dbContext.CourseCategories.ToListAsync();
            return data;
        }
    }
}
