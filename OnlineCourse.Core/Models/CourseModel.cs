﻿using OnlineCourse.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Core.Models
{
    //we need to build model for our course related APIs

    public class CourseDetailModel:CourseModel
    {
        public List<UserReviewModel> Reviews { get; set; } = new List<UserReviewModel>();
        public List<SessionDetailModel> SessionDetails { get; set; } = new List<SessionDetailModel>();
    }

    public class UserReviewModel
    {
        public int ReviewId { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
         
        public int Rating { get; set; }

        public string? Comments { get; set; }

        public DateTime ReviewDate { get; set; }
    }

    public class SessionDetailModel
    {
        public int SessionId { get; set; }

        public int CourseId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? VideoUrl { get; set; }

        public int VideoOrder { get; set; }

    }

    public class CourseModel
    {
        
        public int CourseId { get; set; }

        
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        
        public decimal Price { get; set; }

        
        public string CourseType { get; set; } = null!;

        public int? SeatsAvailable { get; set; }

        
        public decimal Duration { get; set; }

        public int CategoryId { get; set; }

        public int InstructorId { get; set; }

        
        public DateTime? StartDate { get; set; }

        
        public DateTime? EndDate { get; set; }

        
        public string? Thumbnail { get; set; }

        
        public CourseCategoryModel Category { get; set; }=null!; //show category for each course

        public UserRatingModel UserRating { get; set; } //gather average rating for a course


    }

    public class UserRatingModel
    {
        public int CourseId { get; set; }
        public decimal AverageRating  { get; set; }
        public int TotalRating  { get; set; }
    }
}
