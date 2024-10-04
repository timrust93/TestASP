using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPTestRustami.Models
{
    public class MainPageModel
    {
        private List<Employee> _emplyeeList;

        public List<Employee> EmployeeList
        {
            get
            {
                return _emplyeeList;
            }
            set
            {
                _emplyeeList = value;
                _emplyeeList.Sort((x, y) => x.Surname.CompareTo(y.Surname));
            }
        }
    }
}