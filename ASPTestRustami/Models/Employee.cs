using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPTestRustami.Models
{
    // DO NOT!!!!! change the property names as they are used for database

    public class Employee
    {
        public int Id { get; set; }
        public string PayrollNumber { get; set; }
        public string Forenames { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Adress { get; set; }
        public string Adress2 { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
    }
}