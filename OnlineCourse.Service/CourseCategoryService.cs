using OnlineCourse.Core.Entities;
using OnlineCourse.Core.Models;
using OnlineCourse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Service
{
    public class CourseCategoryService : ICourseCategoryService
    {
        private readonly ICourseCategoryRepository categoryRepository;

        public CourseCategoryService(ICourseCategoryRepository categoryRepository) 
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<CourseCategoryModel?> GetByIdAsync(int id)
        {
            //this await keyword tells compiler to pause the execution until data is retrieved
            var data = await categoryRepository.GetByIdAsync(id);
            return data==null?null : new CourseCategoryModel()
            {
                CategoryId = data.CategoryId,
                CategoryName = data.CategoryName,
                Description = data.Description
            };
        }

        public async Task<List<CourseCategoryModel>> GetCourseCategoriesAsync()
        {
            var data = await categoryRepository.GetCourseCategoriesAsync();
            var modelData = data.Select(c => new CourseCategoryModel()
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description
            }).ToList();

            return modelData;
        }
    }
}
