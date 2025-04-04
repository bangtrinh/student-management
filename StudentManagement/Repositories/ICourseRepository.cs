using StudentManagement.Models;

namespace StudentManagement.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        IEnumerable<Course> GetCoursesByMajor(string majorId);
        IEnumerable<Course> GetCoursesByTeacher(string teacherID);

        Course GetCourseDetails(string courseId);
    }
}