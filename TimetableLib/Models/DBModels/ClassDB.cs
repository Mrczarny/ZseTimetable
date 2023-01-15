using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TimetableLib.Models.DBModels.DBAttributes;
using TimetableLib.Models.DTOs;
using TimetableLib.Models.ScrapperModels;

namespace TimetableLib.Models.DBModels
{
    public class ClassDB :  ITimetables, IDisposable
    {
        public ClassDB(){}
        public ClassDB(Class cl)
        {
            Name = cl.Name;
            Timetable = new TimetableDB(cl.Timetable);
            Link = cl.Link;
            
        }

        [Identity]
        [SqlType(SqlDbType.BigInt)]
        public long? Id { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Name { get; set; }

        [SqlType(SqlDbType.BigInt)]
        public long TimetableId { get; set; }

        [SqlType(SqlDbType.NVarChar)]
        public string Link { get; set; }
        
        public void SetLessonId(LessonDB ls)
        {
            ls.ClassId = Id;
        }

        public void SetLessonName(LessonDB ls)
        {
            ls.ClassName = Name;
        }

        public string? GetLessonName(LessonDB ls) => ls?.ClassName;
        public string? GetLessonLink(LessonDB ls) => ls?.ClassLink;

        public TimetableDB Timetable { get; set; }

        public string TimetableLink { get; set; }

        public void Dispose()
        {
            Timetable = null;
            Id = null;
            Link = null;
            Name = null;
            TimetableLink = null;

        }
    }
}
