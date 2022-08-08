using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimetableLib.Models;
using TimetableLib.Timetables;

namespace ZseTimetable
{
    public static class TimetableScrapper
    {
        private static IEnumerable<ClassLesson>? ScrapClassLesson(string rawLesson, int lessonNumber)
        {
            Regex LessonNameRx = new Regex(@"<.*?>((((?<lessonName>[^-<>\n]+).*?(?<GroupName>(?<=-)[^<> ]+))|(?<lessonName>[^<>\n]+)).*?""((?<teacherLink>(?<="").*?\.html)|).*?>(?<teacherName>[^<>\n\s]+)<.*?""((?<classroomLink>(?<="").*?\.html)|).*?>(?<classroomName>(?<!</a>|</span>)[^<>\n]+)<.*?)(<br>|</td>|)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            MatchCollection LessonMatches = LessonNameRx.Matches(rawLesson);

            foreach (Match match in LessonMatches)
            {
                ClassLesson Lesson = new ClassLesson
                {
                    LessonNumber = lessonNumber,
                    LessonName = match.Groups["lessonName"].Value,
                    TeacherName = match.Groups["teacherName"].Value,
                    TeacherLink = match.Groups["teacherLink"].Value,
                    Classroom = match.Groups["classroomName"].Value,
                    ClassroomLink = match.Groups["classroomLink"].Value,
                    Group = match.Groups["GroupName"].Value
                };
                yield return Lesson;
            }
        }
        private static IList<ClassDay> ScrapRawTable(string rawTable)
        {
            var rawLessonsMatches =
                new Regex(@"<tr>.*?nr"">(?<lessonNumber>\d+).*?g"">(?<lessonHours>.*?)<.*?(?<lessons><td.*?)</tr>",
                    RegexOptions.Compiled | RegexOptions.Singleline).Matches(rawTable);
            
            var classDays = new List<ClassDay>();
            var tds = rawLessonsMatches[0].Groups["lessons"].Value.Split("</td>");
            for (int i = 0; i < tds.Length-1; i++)
            {
                var day = new ClassDay();
                day.Lessons = new List<ClassLesson>();
                day.Day = (DayOfWeek)i;
                classDays.Add(day);
            }

            foreach (Match lessonsMatch in rawLessonsMatches)
            {
                var lessonNumber = int.Parse(lessonsMatch.Groups["lessonNumber"].Value);
                var lessonHours = lessonsMatch.Groups["lessonHours"].Value;
                var rawLessons = lessonsMatch.Groups["lessons"].Value;
                tds = rawLessons.Split("</td>");
                for (int i = 0; i < tds.Length - 1; i++)
                {
                    var lessons = ScrapClassLesson(tds[i], lessonNumber);
                    classDays[i].Lessons.AddRange(lessons);
                }
            }

            return classDays;
        }


        public static async Task<ClassTimetable> Scrap(string rawHtml)
        {
            var rawBodyMatch = new Regex(@"<body>.*?tytulnapis"">(?<ClassName>.+?)<.+?(?<Table><table.+?</table>).*?obowiązuje od: (?<startDate>\d{2}\.\d{2}\.\d{4})(.*? do (?<endDate>\d{2}\.\d{2}\.\d{4}).*?|.*?)</body>",
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture)
                .Match(rawHtml);

             return new ClassTimetable
            {
                ClassName = rawBodyMatch.Groups["ClassName"].Value,
                StartDate = DateTime.Parse(rawBodyMatch.Groups["startDate"].Value).Date,
                EndDate = DateTime.TryParse(rawBodyMatch.Groups["endDate"].Value, out DateTime date) ? date.Date : (DateTime?)null,
                Days = ScrapRawTable(rawBodyMatch.Groups["Table"].Value)
            };

        }
    }
}