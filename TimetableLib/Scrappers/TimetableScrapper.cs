using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimetableLib.Models.ScrapperModels;
using TimetableLib.Timetables;

namespace ZseTimetable
{
    /// <summary>
    ///     Class <c>TimetableScrapper</c> is responsible for scrapping data about one timetable from plain html file
    /// </summary>
    public class TimetableScrapper : IAsyncDisposable
    {
        private IDictionary<string, Regex> _dic;

        /// <summary>
        ///     <c>TimetableScrapper</c> constructor with enumerable of its options
        /// </summary>
        /// <param name="options">
        ///     Options for this scrapper such as regexs and theirs options,
        ///     position of those in <c>appsettings.json</c>
        /// </param>
        public TimetableScrapper(IEnumerable<ScrapperOption> options)
        {
            _dic = options.ToDictionary(x => x.Name, x => new Regex(
                x.Pattern,
                (RegexOptions) x.RegexOptions
            ));
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
        }

        private IEnumerable<Lesson>? ScrapLesson<T>(string rawLesson, int lessonNumber)
        {
            var LessonNameRx = _dic[typeof(T).Name];
            // new Regex(@"<.*?>((((?<lessonName>[^-<>\n]+).*?(?<GroupName>(?<=-)[^<> ]+))|(?<lessonName>[^<>\n]+)).*?""((?<teacherLink>(?<="").*?\.html)|).*?>(?<teacherName>[^<>\n\s]+)<.*?""((?<classroomLink>(?<="").*?\.html)|).*?>(?<classroomName>(?<!</a>|</span>)[^<>\n]+)<.*?)(<br>|</td>|)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            var LessonMatches = LessonNameRx.Matches(rawLesson);

            foreach (Match match in LessonMatches)
            {
                var Lesson = new Lesson
                {
                    LessonNumber = lessonNumber,
                    LessonName = match.Groups["lessonName"].Value,
                    TeacherName = match.Groups["teacherName"].Value,
                    TeacherLink = match.Groups["teacherLink"].Value,
                    ClassroomName = match.Groups["classroomName"].Value,
                    ClassroomLink = match.Groups["classroomLink"].Value,
                    ClassName = match.Groups["className"].Value,
                    ClassLink = match.Groups["classLink"].Value,
                    Group = match.Groups["groupName"].Value
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
            for (var i = 0; i < tds.Length - 1; i++)
            {
                var day = new TimetableDay();
                day.Lessons = new List<Lesson>();
                day.Day = (DayOfWeek) i;
                classDays.Add(day);
            }

            foreach (Match lessonsMatch in rawLessonsMatches)
            {
                var lessonNumber = int.Parse(lessonsMatch.Groups["lessonNumber"].Value);
                var lessonHours = lessonsMatch.Groups["lessonHours"].Value;
                var rawLessons = lessonsMatch.Groups["lessons"].Value;
                tds = rawLessons.Split("</td>");
                for (var i = 0; i < tds.Length - 1; i++)
                {
                    var d = tds[i].Split(',');
                    if (d.Length > 1)
                    {
                        //TODO - Lessons can have more then one class 
                        var t = tds[i].Split(" <");
                        if (t[0].Length - 1 < d[0].Length)
                        {
                            d[0] = d[0].Substring(t[0].Length - 1);
                            d[^1] = d[^1][..(d[^1].Length - t[^1].Length)];
                            foreach (var s in d)
                            {
                                var lessons = ScrapLesson<T>(t[0] + s + t[^1], lessonNumber);
                                classDays[i].Lessons.AddRange(lessons);
                            }
                        }
                    }
                    else
                    {
                        var lessons = ScrapLesson<T>(tds[i], lessonNumber);
                        classDays[i].Lessons.AddRange(lessons);
                    }
                }
            }

            return classDays;
        }


        public async Task<Timetable> Scrap<T>(string rawHtml) //TODO: should return T
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
                EndDate = DateTime.TryParse(rawBodyMatch.Groups["endDate"].Value, out var date)
                    ? date.Date
                    : (DateTime?) null,
                Days = ScrapRawTable<T>(rawBodyMatch.Groups["Table"].Value)
            };
        }

        public void Dispose()
        {
            _dic = null;
        }

        protected virtual ValueTask DisposeAsyncCore()
        {
            Dispose();
            return default;
        }
    }
}