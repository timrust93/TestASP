using ASPTestRustami.Models;
using Syncfusion.EJ2.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASPTestRustami.Models;

namespace ASPTestRustami.Controllers
{
    public class MainPageController : Controller
    {
        private MainPageModel _model;
        private MainPageModel Model
        {
            get
            {
                return _model == null ? new MainPageModel() : _model;
            }
            set
            {
                _model = value;
            }
        }


        // GET: MainPage
        public ActionResult Index()
        {
            // If user added some rows or not. The page will depend on it
            if (TempData["rowsAdded"] != null)
            {
                int rowsAdded = (int)TempData["rowsAdded"];
                if (rowsAdded > 0)
                {
                    ViewBag.IsRowsAdded = true;
                    ViewBag.Message = $"{rowsAdded} rows added to database";
                }
                else
                {
                    ViewBag.IsRowsAdded = false;
                    ViewBag.Message = "No rows added. Maybe file issue";
                }
            }

            // Load data to show it on a page. Model for page will store
            //DatabaseAccess dbAccess = new DatabaseAccess();
            //List<Employee> employeeList = dbAccess.ReadEmployess();            
            //var model = Model;            
            //model.EmployeeList = employeeList;

            return View(Model);
        }

        // This method expects .csv file to upload to database
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            int rowsAdded = 0;
            DatabaseAccess dbAccess = new DatabaseAccess();

            if (file != null && file.ContentLength > 0)
            {
                // Read the file and pass it to database
                using (BinaryReader b = new BinaryReader(file.InputStream))
                {
                    byte[] binData = b.ReadBytes(file.ContentLength);
                    rowsAdded = dbAccess.AddCSVForEmployees(binData);
                }
            }

            // Data to display when the page reloads after uploading
            ViewBag.Message = rowsAdded > 0 ? $"{rowsAdded} rows were successfully added." : "No rows were added.";
            ViewBag.Model = Model;
            TempData["rowsAdded"] = rowsAdded;

            return RedirectToAction("Index");
        }

        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            DatabaseAccess dbA = new DatabaseAccess();
            var employeeList = dbA.ReadEmployess();
            //employeeList.Sort((x, y) => x.Surname.CompareTo(y.Surname));

            IEnumerable DataSource = employeeList;
            DataOperations operation = new DataOperations();
            int count = employeeList.Count;
            
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        //public ActionResult Update(ICRUDModel<Employee> value)
        public ActionResult Update(ICRUDModel<Employee> value)
        {
            using (StreamReader reader = new StreamReader(Request.InputStream))
            {
                string jsonData = reader.ReadToEnd();
                //ICRUDModel<Employee> value = JsonConvert.DeserializeObject<ICRUDModel<Employee>>(jsonData);
                Debug.WriteLine(jsonData);
                // Now you can work with the "value" object
                //return Json(value.value);
            }
            //Employee employee = value.value;
            //DatabaseAccess dbA = new DatabaseAccess();
            //dbA.UpdateEmployee(employee);

            return Json(value.value);
        }

        public class ICRUDModel<T> where T : class
        {
            public string action { get; set; }

            public string table { get; set; }

            public string keyColumn { get; set; }

            public object key { get; set; }

            public T value { get; set; }

            public List<T> added { get; set; }

            public List<T> changed { get; set; }

            public List<T> deleted { get; set; }

            public IDictionary<string, object> @params { get; set; }
        }
    }

}