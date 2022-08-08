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

namespace ZseTimetable
{
    public static class ChangesScrapper
    {

        private static IEnumerable<ClassReplacement> ScrapClassReplacements(string rawClassReplacements)
        {
            var replacementsMatches = new Regex(
                @"<tr>.*?>\s*?(?<lessonNumber>\d+)\s*?<.*?<td.*?>\s*?(?<Description>\S[^<>]+?)\s*?<.*?<td.*?>\s*?(?<Sub>\S[^<>]+?)\s*?<.*?<td.*?>\s*?(?<Note>\S[^<>]+?)\s*?<.*?</tr>",
                RegexOptions.Compiled | RegexOptions.Singleline).
                Matches(rawClassReplacements);
            foreach (Match replacementsMatch in replacementsMatches)
            {
                yield return new ClassReplacement()
                {
                    LessonNumber = replacementsMatch.Groups["lessonNumber"].Value,
                    Description = replacementsMatch.Groups["Description"].Value == "&nbsp;" ? null : replacementsMatch.Groups["Description"].Value,
                    Sub = replacementsMatch.Groups["Sub"].Value == "&nbsp;" ? null : replacementsMatch.Groups["Sub"].Value,
                    Note = replacementsMatch.Groups["Note"].Value == "&nbsp;" ? null : replacementsMatch.Groups["Note"].Value
                };
            }
        }

        private static IEnumerable<TeacherReplacement>? ScrapTeacherReplacements(string rawTeacherReplacements)
        {
            var tReplacementsRegex = new Regex(
                @"<tr>[^<]*?<td[^<]*?(?<TeacherName>\w+( \w+)+)[^<]*?</td>[^<]*?</tr>(?<rawClassReplacements>.*?)",
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.RightToLeft | RegexOptions.ExplicitCapture);
            var tReplacementMatches = tReplacementsRegex.Matches(rawTeacherReplacements);
            foreach (Match replacementMatch in tReplacementMatches)
            {
                yield return new TeacherReplacement()
                {
                    Teacher = new Teacher()
                    {
                        Name = replacementMatch.Groups["TeacherName"].Value
                    },
                    ClassReplacements = ScrapClassReplacements(replacementMatch.Groups["rawClassReplacements"].Value)
                };
            }

        }
        
         public static DayReplacement Scrap(string rawHtml)
        {
            var trMatch = new Regex(@"<nobr>(?<replacementHeader>.*?(?<replacementDate>\d{1,2}\.\d{1,2}\.\d{4}).*?)</nobr>.*?(?<replacements><tr>.*</tr>)", RegexOptions.Compiled | RegexOptions.Singleline).Match(rawHtml);

            return new DayReplacement()
            {
                Date = DateTime.TryParse(trMatch.Groups["replacementDate"].Value, out DateTime date) ? date.Date : (DateTime?)null,
                Replacements = ScrapTeacherReplacements(trMatch.Groups["replacements"].Value)
            };

        }
    }
}
