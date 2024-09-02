using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_com.Models
{
    public class User
    {
       // This specifies that the database should generate a value when a row is inserted
        public int UserId { get; set; }

        
        public string? Name { get; set; }


        public string? Email { get; set; }

        public string? Phone { get; set; }

     
        public string? Address { get; set; }

        
        public string? Password { get; set; }

        
        public DateTime Created_Time { get; set; } = DateTime.Now;

        public int? Created_By { get; set; }

        public DateTime? Modified_Time { get; set; }

        public int? Modified_By { get; set; }
    }
}
