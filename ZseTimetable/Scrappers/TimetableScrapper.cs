using System;
using System.IO;
using System.Threading.Tasks;
using TimetableLib.Timetables;

namespace ZseTimetable
{
    public class TimetableScrapper
    {
        public async Task<ClassTimetable> Scrapper(Stream RawTimetable)
        {
            var stmReader = new StreamReader(RawTimetable);

            var rawHtml = (await stmReader.ReadToEndAsync()).ToLower();
            var tbody = rawHtml[rawHtml.IndexOf("<table")..rawHtml.LastIndexOf("</table>")].Replace("\r\n", String.Empty);
            var timetableRaw = tbody[tbody.LastIndexOf("<table")..];
            var classnameRaw = tbody[tbody.IndexOf("<span")..tbody.IndexOf("</span>")];


            return new ClassTimetable();
        }
    }
}