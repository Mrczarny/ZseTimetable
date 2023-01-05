using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimetableLib;
using TimetableLib.Models;
using TimetableLib.Changes;
using TimetableLib.Models.Replacements;
using TimetableLib.Models.ScrapperModels;

namespace ZseTimetable
{
    /// <summary>
    /// Class <c>ChangesScrapper</c> is responsible for scrapping data about timetable's changes from plain html file
    /// </summary>
    public class ChangesScrapper
    {
        /// <summary>
        /// This dictionary contains names of methods and theirs regexs
        /// </summary>
        private readonly IReadOnlyDictionary<string, Regex> _dic;

        /// <summary>
        /// <c>ChangesScrapper</c> constructor with enumerable of its options
        /// </summary>
        /// <param name="options">Options for this scrapper such as regexs and theirs options,
        /// position of those in <c>appsettings.json</c> </param>
        public ChangesScrapper(IEnumerable<ScrapperOption> options)
        {
            _dic = options.ToDictionary(x => x.Name, x => new Regex(
                x.Pattern,
                (RegexOptions)x.RegexOptions
            ));
        }

        private IEnumerable<LessonReplacement> ScrapClassReplacements(string rawClassReplacements)
        {
            var replacementsMatches = _dic[nameof(ScrapClassReplacements)].
                Matches(rawClassReplacements);
            //new Regex(
            //    @"<tr>.*?>\s*?(?<lessonNumber>\d+)\s*?<.*?<td.*?>\s*?(?<Description>\S[^<>]+?)\s*?<.*?<td.*?>\s*?(?<Sub>\S[^<>]+?)\s*?<.*?<td.*?>\s*?(?<Note>\S[^<>]+?)\s*?<.*?</tr>",
            //    RegexOptions.Compiled | RegexOptions.Singleline)
            
            foreach (Match replacementsMatch in replacementsMatches)
            {
                yield return new LessonReplacement()
                {
                    LessonNumber = replacementsMatch.Groups["lessonNumber"].Value,
                    Description = replacementsMatch.Groups["Description"].Value == "&nbsp;" ? null : replacementsMatch.Groups["Description"].Value,
                    Sub = replacementsMatch.Groups["Sub"].Value == "&nbsp;" ? null : replacementsMatch.Groups["Sub"].Value,
                    Note = replacementsMatch.Groups["Note"].Value == "&nbsp;" ? null : replacementsMatch.Groups["Note"].Value
                };
            }
        }

        private IEnumerable<TeacherReplacements>? ScrapTeacherReplacements(string rawTeacherReplacements)
        {
            var tReplacementsRegex = _dic[nameof(ScrapTeacherReplacements)];
            //new Regex(
            //    @"<tr>[^<]*?<td[^<]*?(?<TeacherName>\w+( \w+)+)[^<]*?</td>[^<]*?</tr>(?<rawClassReplacements>.*?)",
            //    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.RightToLeft | RegexOptions.ExplicitCapture)
            var tReplacementMatches = tReplacementsRegex.Matches(rawTeacherReplacements);
            foreach (Match replacementMatch in tReplacementMatches)
            {
                yield return new TeacherReplacements()
                {
                    Teacher = new Teacher()
                    {
                        Name = replacementMatch.Groups["TeacherName"].Value
                    },
                    ClassReplacements = ScrapClassReplacements(replacementMatch.Groups["rawClassReplacements"].Value)
                };
            }

        }
        
         public DayReplacements Scrap(string rawHtml)
        {
            var trMatch = _dic[nameof(Scrap)].Match(rawHtml);
            // new Regex(@"<nobr>(?<replacementHeader>.*?(?<replacementDate>\d{1,2}\.\d{1,2}\.\d{4}).*?)</nobr>.*?(?<replacements><tr>.*</tr>)", RegexOptions.Compiled | RegexOptions.Singleline)

            return new DayReplacements()
            {
                Date = DateTime.TryParse(trMatch.Groups["replacementDate"].Value, out DateTime date) ? date.Date : (DateTime?)null,
                Replacements = ScrapTeacherReplacements(trMatch.Groups["replacements"].Value)
            };

        }
    }
}
