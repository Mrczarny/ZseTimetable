using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimetableLib.Changes;
using TimetableLib.Models.Replacements;
using TimetableLib.Models.ScrapperModels;

namespace ZseTimetable
{
    /// <summary>
    ///     Class <c>ChangesScrapper</c> is responsible for scrapping data about timetable's changes from plain html file
    /// </summary>
    public class ChangesScrapper : IAsyncDisposable
    {
        /// <summary>
        ///     This dictionary contains names of methods and theirs regexs
        /// </summary>
        private IReadOnlyDictionary<string, Regex> _dic;

        /// <summary>
        ///     <c>ChangesScrapper</c> constructor with enumerable of its options
        /// </summary>
        /// <param name="options">
        ///     Options for this scrapper such as regexs and theirs options,
        ///     position of those in <c>appsettings.json</c>
        /// </param>
        public ChangesScrapper(IEnumerable<ScrapperOption> options)
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

        private IEnumerable<LessonReplacement> ScrapClassReplacements(string rawClassReplacements, string TeacherName)
        {
            var replacementsMatches = _dic[nameof(ScrapClassReplacements)].Matches(rawClassReplacements);
            //new Regex(
            //    @"<tr>.*?>\s*?(?<lessonNumber>\d+)\s*?<.*?<td.*?>\s*?(?<Description>\S[^<>]+?)\s*?<.*?<td.*?>\s*?(?<Sub>\S[^<>]+?)\s*?<.*?<td.*?>\s*?(?<Note>\S[^<>]+?)\s*?<.*?</tr>",
            //    RegexOptions.Compiled | RegexOptions.Singleline)

            foreach (Match replacementsMatch in replacementsMatches)
                yield return new LessonReplacement
                {
                    LessonNumber = byte.Parse(replacementsMatch.Groups["lessonNumber"].Value),
                    ClassName = replacementsMatch.Groups["className"].Value.Replace(" ", string.Empty),
                    Group = replacementsMatch.Groups["groupName"].Value,
                    ClassroomName = replacementsMatch.Groups["classroomName"].Value,
                    Description = replacementsMatch.Groups["description"].Value == "&nbsp;"
                        ? null
                        : replacementsMatch.Groups["Description"].Value,
                    Sub = replacementsMatch.Groups["sub"].Value == "&nbsp;"
                        ? null
                        : replacementsMatch.Groups["Sub"].Value,
                    Note = replacementsMatch.Groups["note"].Value == "&nbsp;"
                        ? null
                        : replacementsMatch.Groups["Note"].Value,
                    OriginalTeacher = TeacherName
                };
        }

        private IEnumerable<TeacherReplacements>? ScrapTeacherReplacements(string rawTeacherReplacements)
        {
            var tReplacementsRegex = _dic[nameof(ScrapTeacherReplacements)];
            //new Regex(
            //    @"<tr>[^<]*?<td[^<]*?(?<TeacherName>\w+( \w+)+)[^<]*?</td>[^<]*?</tr>(?<rawClassReplacements>.*?)",
            //    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.RightToLeft | RegexOptions.ExplicitCapture)
            var tReplacementMatches = tReplacementsRegex.Matches(rawTeacherReplacements);
            foreach (Match replacementMatch in tReplacementMatches)
                yield return new TeacherReplacements
                {
                    Teacher = new Teacher
                    {
                        Name = replacementMatch.Groups["TeacherName"].Value
                    },
                    ClassReplacements = ScrapClassReplacements(replacementMatch.Groups["rawClassReplacements"].Value,
                        replacementMatch.Groups["TeacherName"].Value)
                };
        }

        public DayReplacements Scrap(string rawHtml)
        {
            var trMatch = _dic[nameof(Scrap)].Match(rawHtml);
            // new Regex(@"<nobr>(?<replacementHeader>.*?(?<replacementDate>\d{1,2}\.\d{1,2}\.\d{4}).*?)</nobr>.*?(?<replacements><tr>.*</tr>)", RegexOptions.Compiled | RegexOptions.Singleline)
            var d = new DayReplacements
            {
                Date = DateTime.TryParse(trMatch.Groups["replacementDate"].Value, out var date)
                    ? date.Date
                    : (DateTime?) null,
                Replacements = ScrapTeacherReplacements(trMatch.Groups["replacements"].Value)
            };
            return d;
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