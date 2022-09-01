using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimetableLib.Models;
using TimetableLib.Models.ScrapperModels;
using TimetableLib.Timetables;

namespace ZseTimetable
{
    public class TimetableScrapper
    {
        private readonly IReadOnlyDictionary<string,Regex> _dic;
        public TimetableScrapper(IEnumerable<ScrapperOption> options)
        {
            _dic = options.ToDictionary(x => x.Name, x => new Regex(
                x.Pattern,
                (RegexOptions)x.RegexOptions
            ));
        }


        private IEnumerable<Lesson>? ScrapLesson<T>(string rawLesson, int lessonNumber)
        {
            
            Regex LessonNameRx = _dic[typeof(T).Name];
            // new Regex(@"<.*?>((((?<lessonName>[^-<>\n]+).*?(?<GroupName>(?<=-)[^<> ]+))|(?<lessonName>[^<>\n]+)).*?""((?<teacherLink>(?<="").*?\.html)|).*?>(?<teacherName>[^<>\n\s]+)<.*?""((?<classroomLink>(?<="").*?\.html)|).*?>(?<classroomName>(?<!</a>|</span>)[^<>\n]+)<.*?)(<br>|</td>|)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            MatchCollection LessonMatches = LessonNameRx.Matches(rawLesson);

            foreach (Match match in LessonMatches)
            {
                Lesson Lesson = new Lesson
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

        

        private IList<TimetableDay> ScrapRawTable<T>(string rawTable)
        {
            var rawLessonsMatches = _dic[nameof(ScrapRawTable)].Matches(rawTable);
            //new Regex(@"<tr>.*?nr"">(?<lessonNumber>\d+).*?g"">(?<lessonHours>.*?)<.*?(?<lessons><td.*?)</tr>",
            //        RegexOptions.Compiled | RegexOptions.Singleline).Matches(rawTable);
            
            
            var classDays = new List<TimetableDay>();
            var tds = rawLessonsMatches[0].Groups["lessons"].Value.Split("</td>");
            for (int i = 0; i < tds.Length-1; i++)
            {
                var day = new TimetableDay();
                day.Lessons = new List<Lesson>();
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
                    var lessons = ScrapLesson<T>(tds[i], lessonNumber);
                    classDays[i].Lessons.AddRange(lessons);
                }
            }

            return classDays;
        }


        public async Task<Timetable> Scrap<T>(string rawHtml)
        {
            
            var rawBodyMatch = _dic[nameof(Scrap)].Match(rawHtml); 
                //new Regex(
                //@"<body>.*?tytulnapis"">(?<Title>.+?)<.+?(?<Table><table.+?</table>).*?obowiązuje od: (?<startDate>\d{2}\.\d{2}\.\d{4})(.*? do (?<endDate>\d{2}\.\d{2}\.\d{4}).*?|.*?)</body>",
                //RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase |
                //RegexOptions.ExplicitCapture);

                return new Timetable
            {
                Title = rawBodyMatch.Groups["Title"].Value,
                StartDate = DateTime.Parse(rawBodyMatch.Groups["startDate"].Value).Date,
                EndDate = DateTime.TryParse(rawBodyMatch.Groups["endDate"].Value, out DateTime date) ? date.Date : (DateTime?)null,
                Days = ScrapRawTable<T>(rawBodyMatch.Groups["Table"].Value)
            };

        }
    }
}