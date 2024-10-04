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
using System.Runtime.InteropServices;

namespace ASPTestRustami.Controllers
{
    public class MainPageController : Controller
    {
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
            return View();
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
            TempData["rowsAdded"] = rowsAdded;

            return RedirectToAction("Index");
        }

        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            DatabaseAccess dbA = new DatabaseAccess();
            var employeeList = dbA.ReadEmployess();            

            IEnumerable DataSource = employeeList;
            DataOperations operation = new DataOperations();
            int count = employeeList.Count;

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            else
            {
                employeeList.Sort((x, y) => x.Surname.CompareTo(y.Surname));
                DataSource = employeeList;
            }
            if (dm.Where != null && dm.Where.Count > 0)
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, "and");
            }
            
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
        
        public ActionResult Update(Employee value)
        {            
            DatabaseAccess dbA = new DatabaseAccess();
            dbA.UpdateEmployee(value);

            return Json(value);
        }
    }

}