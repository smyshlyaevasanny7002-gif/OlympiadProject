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
    }

    public class OlympiadService
    {
        public List<StudentResult> GetSelectedStudents(double threshold)
        {
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated(); // Создаст БД при первом запуске
                var students = db.Students.ToList();
                var results = new List<StudentResult>();

                foreach (var student in students)
                {
                    var profileGrades = db.Grades
                        .Include(g => g.Subject)
                        .Where(g => g.StudentId == student.Id && g.Subject.IsProfile)
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
                                AverageScore = System.Math.Round(avg, 2)
                            });
                        }
                    }
                }
                return results.OrderByDescending(r => r.AverageScore).ToList();
            }
        }
    }
}