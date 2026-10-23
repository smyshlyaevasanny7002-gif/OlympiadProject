using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace OlympiadProject
{
    public class StudentResult
    {
        public string FullName { get; set; }
        public string GroupName { get; set; }
        public double AverageScore { get; set; }
        public int ProfileSubjectsCount { get; set; }
    }

    public class OlympiadService
    {
        public List<StudentResult> GetSelectedStudents(double threshold, int olympiadId, int? subjectId = null)
        {
            using (var db = new AppDbContext())
            {
                var students = db.Students.ToList();
                var results = new List<StudentResult>();

                var profileSubjectQuery = db.OlympiadSubjects
                    .Where(os => os.OlympiadId == olympiadId && os.IsProfile);

                if (subjectId.HasValue && subjectId.Value > 0)
                {
                    profileSubjectQuery = profileSubjectQuery.Where(os => os.SubjectId == subjectId.Value);
                }

                var profileSubjectIds = profileSubjectQuery.Select(os => os.SubjectId).ToList();

                foreach (var student in students)
                {
                    var profileGrades = db.Grades
                        .Where(g => g.StudentId == student.Id && profileSubjectIds.Contains(g.SubjectId))
                        .Select(g => g.Score)
                        .ToList();

                    if (profileGrades.Any())
                    {
                        double avg = profileGrades.Average();
                        if (avg >= threshold)
                        {
                            results.Add(new StudentResult
                            {
                                FullName = student.FullName,
                                GroupName = student.GroupName,
                                AverageScore = System.Math.Round(avg, 2),
                                ProfileSubjectsCount = profileGrades.Count
                            });
                        }
                    }
                }
                return results.OrderByDescending(r => r.AverageScore).ToList();
            }
        }
    }
}