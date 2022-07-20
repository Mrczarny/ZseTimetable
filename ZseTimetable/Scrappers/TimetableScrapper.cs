using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimetableLib.Timetables;

namespace ZseTimetable
{
    public class TimetableScrapper
    {

        private ClassDay ClasDayScrapper(string[] column)
        {
            ClassDay classDay = new ClassDay { Lessons = new List<ClassLesson>() };
            classDay.DayOfWeek = (DayOfWeek)Array.IndexOf(new CultureInfo("pl-PL").DateTimeFormat.DayNames, column[0]); //TODO - Move CultureInfo to global 
            Regex LessonNameRx = new Regex(@"p\"">(?<LessonName>.+?)<.+?n\"">(?<TeacherName>.+?)<.+?s\"">(?<Classroom>.+?)<", RegexOptions.Compiled);
            for (int i = 1; i < column.Length; i++)
            {
                MatchCollection LessonMatches = LessonNameRx.Matches(column[i]);
                foreach (Match match in LessonMatches)
                {
                    GroupCollection groups = match.Groups;
                    ClassLesson Lesson = new ClassLesson
                    {
                        Classroom = groups["Classroom"].Value,
                        LessonName = groups["LessonName"].Value,
                        TeacherName = groups["TeacherName"].Value
                    };
                    classDay.Lessons.Add(Lesson);
                }
            }
            return classDay;
        }
        public async Task<ClassTimetable> Scrapper(Stream RawTimetable)
        {
            var stmReader = new StreamReader(RawTimetable);

            var rawHtml = (await stmReader.ReadToEndAsync()).ToLower();
            var body = rawHtml[rawHtml.IndexOf("<body")..rawHtml.LastIndexOf("</body>")].Replace("\r\n", String.Empty);
            var timetableRaw = body[body.IndexOf("class=\"tabela\"")..].Split("</table>")[0];                                  //It's painful just to look at it
            var classnameRaw = body[body.IndexOf("<table")..body.IndexOf("</table>")];                                                         //but it works, so it's good for now
                                                                                                                                //TODO - do it with regex

            var trs = timetableRaw.Split("<tr>")[1..];
            var rows = trs[0].Split("</th>")[..^1];
            for (int i = 1; i < trs.Length; i++)
            {
                var tds = trs[i].Split("</td>")[..^1];
                for (int j = 0; j < rows.Length; j++)
                {
                    //var cell = tds[j].Substring(tds[j].IndexOf('>')+1);
                    rows[j] += "," + tds[j];
                }
            }

            List<string[]> columns = new List<string[]>();                                  //some extensive number of fors
            for (int i = 0; i < rows.Length; i++)
            {
                var cells = rows[i].Split(',');
                for (int j = 0; j < cells.Length; j++)
                {
                    cells[j] = cells[j].Substring(cells[j].IndexOf('>') + 1);
                }
                columns.Add(cells);
            }
            var clasDay = ClasDayScrapper(columns[2]);




            return new ClassTimetable();
        }
    }
}