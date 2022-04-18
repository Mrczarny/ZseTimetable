using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableLib;
using TimetableLib.Changes;

namespace ZseTimetable
{
    public class ChangesScrapper
    {
        private bool Contains(string s) => s.Contains("");

        public async Task<IEnumerable<TeacherReplacement>> Scrapper(Stream RawChanges)
        {
            int tHeaderCellsNumber = 5;
            var stmReader = new StreamReader(RawChanges, Encoding.GetEncoding("iso-8859-2"));

            var rawHtml = (await stmReader.ReadToEndAsync()).ToLower();
            var body = rawHtml[rawHtml.IndexOf("<body>")..rawHtml.IndexOf("</body>")].Replace("\r\n", String.Empty);
            var tables = body.Split("<td nowrap class=st1 ")
                .ToList();

           List<TeacherReplacement> teachersReplacements = new List<TeacherReplacement>();
           for (int i = 1; i < tables.Count; i++)
           {
               var whiteBox = tables[i].IndexOf("<td nowrap class=st15");
               if (whiteBox != -1)
               {
                   tables[i] = tables[i][..whiteBox];
               }
               var tds = tables[i].Split("</td>");
               List<string[]> rows = new List<string[]>();
               for (int j = 0; j < (tds.Length-tHeaderCellsNumber)/4; j++)
               {
                   string[] row = new string[4];
                   for (int k = 0; k < 4; k++)
                   {    
                       row[k] = tds[tHeaderCellsNumber+(4*j)+k];
                   }
                   rows.Add(row);
                }

                //construct TeacherChange
                TeacherReplacement replacement = new TeacherReplacement()
                {
                    Id = null,
                    Teacher = new Teacher()
                    {
                        Id = null,
                        Name = tds[0][(tds[0].LastIndexOf(">")+1)..]
                    },
                    ClassReplacements = new List<ClassReplacement>()
                };
                foreach (var row in rows)
                {
                    ClassReplacement cr = new ClassReplacement();
                    cr.Id = null;
                    cr.LessonNumber = row[0][(row[0].LastIndexOf(">")+1)..];
                    cr.Description = row[1][(row[1].LastIndexOf(">")+1)..];
                    cr.Sub = row[2][(row[2].LastIndexOf(">")+1)..];
                    cr.Note = row[3][(row[3].LastIndexOf(">")+1)..];
                    replacement.ClassReplacements.Add(cr);
                }
                teachersReplacements.Add(replacement);
            }

           return teachersReplacements;
        }
    }
}
