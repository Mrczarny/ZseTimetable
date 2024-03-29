﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TimetableLib.Models.ScrapperModels;
using TimetableLib.Timetables;

namespace TimetableLib.Scrappers
{
    /// <summary>
    ///     Class <c>TimetableScrapper</c> is responsible for scrapping data about one timetable from plain html file
    /// </summary>
    public class TimetableScrapper
    {
        private readonly IDictionary<string, Regex> _dic;

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
                    var d = _dic["tds"].Matches(tds[i]).ToList();
                    if (d.Count > 3)
                    {
                        //Lessons can have more then one class 
                        if (d.Count % 3 != 0)
                        {
                            var oddOnesE = d.Where(x => d.Count(y => y.Groups["type"]?.Value == x.Groups["type"]?.Value) > 1);
                            var oddOnes = new List<Match>(oddOnesE);
                            foreach (var m in oddOnesE)
                            {
                                oddOnes.Remove(m);
                                var str = String.Empty;
                                foreach (var normal in d.Where(x => !oddOnes.Contains(x)))
                                {
                                    str += normal.ToString();
                                }
                                oddOnes.Add(m);
                                str += "<";
                                var lessons = ScrapLesson<T>(str, lessonNumber);
                                classDays[i].Lessons.AddRange(lessons);
                            }
                        }
                        else
                        {
                            var chunked = new List<List<Match>>();
                            for (int j = 0; j < d.Count; j += 3)
                            {
                                chunked.Add(d.GetRange(j, Math.Min(3, d.Count - j)));
                            }
                            
                            foreach (var ch in chunked)
                            {
                                var str = String.Empty;
                                foreach (var m in ch)
                                {
                                    str += m.ToString();
                                }
                                str += "<";
                                var lessons = ScrapLesson<T>(str, lessonNumber);
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


        public Timetable Scrap<T>(string rawHtml) //TODO: should return T
        {
            var rawBodyMatch = _dic[nameof(Scrap)].Match(rawHtml);
            //new Regex(
            //@"<body>.*?tytulnapis"">(?<Title>.+?)<.+?(?<Table><table.+?</table>).*?obowiązuje od: (?<startDate>\d{2}\.\d{2}\.\d{4})(.*? do (?<endDate>\d{2}\.\d{2}\.\d{4}).*?|.*?)</body>",
            //RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase |
            //RegexOptions.ExplicitCapture);

            return new Timetable
            {
                Title = rawBodyMatch.Groups["Title"].Value,
                StartDate = DateTime.Parse(rawBodyMatch.Groups["startDate"].Value, CultureInfo.GetCultureInfo("pl")).Date,
                EndDate = DateTime.TryParse(rawBodyMatch.Groups["endDate"].Value,CultureInfo.GetCultureInfo("pl"),DateTimeStyles.None, out var date)
                    ? date.Date
                    : (DateTime?) null,
                Days = ScrapRawTable<T>(rawBodyMatch.Groups["Table"].Value)
            };
        }

    }
}