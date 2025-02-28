﻿using OnlineCourse.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Data
{
    public interface ICourseRepository
    {
        Task<List<CourseModel>>GetAllCoursesAsync(int? categorryId=null);
        Task<CourseDetailModel>GetCourseDetailAsync(int courseId);
        
    }
}
