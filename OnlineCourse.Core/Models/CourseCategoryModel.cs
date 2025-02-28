using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Core.Models
{
    //this is the model class exposed to customer
    public class CourseCategoryModel
    {
        
        public int CategoryId { get; set; }

       
        public string CategoryName { get; set; } = null!;

        
        public string? Description { get; set; }
        
    }
}
