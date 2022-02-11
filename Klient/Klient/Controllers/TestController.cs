#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Client.Data;
using Client.Models;
using System.Text;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace Clients.Controllers
{
    public class TestController : Controller
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Klient.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var klient = await _context.Klient
                .FirstOrDefaultAsync(m => m.Id == id);
            if (klient == null)
            {
                return NotFound();
            }

            return View(klient);
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Import()
        {
            return View();
        }
        public IActionResult ExportData(string file_type)
        {
            if (file_type == "csv")
            {
                 
                return ExportToCSV();
            }
            else if(file_type == "xmlx")
            {
                return ExportToXLSX();
            }
            return View();
        }
        private  IActionResult ExportToCSV()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Id,Name,Surname,PESEL,BirthYear,Sex");
            var clients =  _context.Klient.ToList();
            foreach (var client in clients)
            {
                builder.AppendLine($"{client.Id},{client.Name},{client.Surname},{client.PESEL},{client.BirthYear},{client.Sex}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv","klienci.csv");
        }

        public IActionResult ExportToXLSX()
        {
            using(var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Clients");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "Surname";
                worksheet.Cell(currentRow, 4).Value = "PESEL";
                worksheet.Cell(currentRow, 5).Value = "BirthYear";
                worksheet.Cell(currentRow, 6).Value = "Sex";
                var clients = _context.Klient.ToList();
                foreach (var client in clients)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = client.Id;
                    worksheet.Cell(currentRow, 2).Value = client.Name;
                    worksheet.Cell(currentRow, 3).Value = client.Surname;
                    worksheet.Cell(currentRow, 4).Value = client.PESEL;
                    worksheet.Cell(currentRow, 5).Value = client.BirthYear;
                    worksheet.Cell(currentRow, 6).Value = client.Sex;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnc.openxmlformats-officedocument.spreadsheetml.sheet", "clients.xlsx");
                }

            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,PESEL,BirthYear,Sex")] Klient klient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(klient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(klient);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var klient = await _context.Klient.FindAsync(id);
            if (klient == null)
            {
                return NotFound();
            }
            return View(klient);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,PESEL,BirthYear,Sex")] Klient klient)
        {
            if (id != klient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(klient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KlientExists(klient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(klient);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var klient = await _context.Klient
                .FirstOrDefaultAsync(m => m.Id == id);
            if (klient == null)
            {
                return NotFound();
            }

            return View(klient);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var klient = await _context.Klient.FindAsync(id);
            _context.Klient.Remove(klient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<List<Klient>> ImportFromXMLX(IFormFile file)
        {
            var list = new List<Klient>();
            using(var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream); ;
                using(var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;
                    for (int  row = 2; row <= rowcount; row++ )
                    {
                        list.Add(new Klient
                        {
                            Id = Convert.ToInt32(worksheet.Cells[row, 1].Value.ToString().Trim()),
                            Name = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            Surname = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            PESEL = worksheet.Cells[row, 4].Value.ToString().Trim(),
                            BirthYear = Convert.ToInt32(worksheet.Cells[row, 5].Value.ToString().Trim()),
                            Sex = Convert.ToInt32(worksheet.Cells[row, 6].Value.ToString().Trim())
                        });
                    }
                }
            }
            _context.Klient.AddRange(list);
            await _context.SaveChangesAsync();
            return list;
        }
        [HttpPost]
        public async Task<List<Klient>> ImportFromCSV(IFormFile file)
        {
            List<Klient> list = new List<Klient>();
            if(file.FileName.EndsWith(".csv"))
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string[] headers = reader.ReadLine().Split(',');
                    while(!reader.EndOfStream)
                    {
                        string[] row = reader.ReadLine().Split(',');
                        list.Add(new Klient
                        { 
                            Id = Convert.ToInt32(row[0]),
                            Name = row[1].ToString(),
                            Surname = row[2].ToString(),
                            PESEL = row[3].ToString(),
                            BirthYear = Convert.ToInt32(row[4]),
                            Sex = Convert.ToInt32(row[5])
                        });
                    }
                }
            }
            _context.Klient.AddRange(list);
            await _context.SaveChangesAsync();
            return list;
        }
        private bool KlientExists(int id)
        {
            return _context.Klient.Any(e => e.Id == id);
        }
    }
}
