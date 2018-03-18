using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using videozid.Models;
using System.IO;

namespace videozid.Controllers
{
    public class LogController : Controller
    {
        private readonly RPPP15Context _context;

        public LogController(RPPP15Context context)
        {
            _context = context;    
        }


        public async Task<IActionResult> Show(DateTime dan, int option)
        {
            string day = dan.ToString("yyyy-MM-dd");

            var logs = _context.Log.ToList();
            if (dan.Equals("0001-01-01")) return View(logs);

            List<Log> filteredLogs = new List<Log>();
            
            foreach (var log in logs)
            {
                string logDay = log.Logged.ToString("yyyy-MM-dd");
                if (day.Equals(logDay)) filteredLogs.Add(log);
            }

            return View(filteredLogs);
        }

        // GET: LogEntries
        public async Task<IActionResult> Index()
        {
            return View();
        }

        //public async Task<IActionResult> Show(DateTime dan, int option)
        //{
        //    ViewBag.Dan = dan;
        //    List<Log> list = new List<Log>();


        //    string format = dan.ToString("yyyy-MM-dd");
        //    string filename = createFileName(option, format);

        //    if (System.IO.File.Exists(filename))
        //    {
        //        String previousEntry = string.Empty;
        //        using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
        //        {
        //            using (StreamReader reader = new StreamReader(fileStream))
        //            {
        //                string line;
        //                while ((line = await reader.ReadLineAsync()) != null)
        //                {
        //                    if (line.StartsWith(format))
        //                    {
        //                        //počinje novi zapis, starog dodaj u listu
        //                        if (previousEntry != string.Empty)
        //                        {
        //                            Log logEntry = Log.FromString(previousEntry);
        //                            list.Add(logEntry);
        //                        }
        //                        previousEntry = line;
        //                    }
        //                    else
        //                    {
        //                        previousEntry += line;
        //                    }
        //                }
        //            }
        //        }
        //        //dodaj zadnji

        //        if (previousEntry != string.Empty)
        //        {
        //            Log logEntry = Log.FromString(previousEntry);
        //            list.Add(logEntry);
        //        }

        //        var oldLogs = _context.LogEntry.ToList();
        //        foreach (var log in oldLogs)
        //        {
        //            _context.Remove(log);
        //        }
        //        _context.SaveChanges();

        //        foreach (var log in list)
        //        {
        //            log.Id = 0;
        //            _context.LogEntry.Add(log);
        //        }
        //        _context.SaveChanges();

        //        if (option == 3)
        //        {
        //            var Oldlist = _context.LogEntry.ToList();
        //            list = filterDates(Oldlist, format);
        //        }
        //    }




        //    return View(list);
        //}

        //private string createFileName(int option, string format)
        //{
        //    if (option == 1) return $"logs/nlog-trace-{format}.log";
        //    else if (option == 2) return $"logs/nlog-all-{format}.log";
        //    else return $"logs/nlog-own-{format}.log";
        //}

        //private List<Log> filterDates(List<Log> oldList, string date)
        //{
        //    List<Log> newList = new List<Log>();
        //    foreach (var log in oldList)
        //    {
        //        if (log.Time.ToString("yyyy-MM-dd").Equals(date))
        //        {
        //            newList.Add(log);
        //        }
        //    }

        //    return newList;
        //}

    }
}
