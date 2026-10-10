namespace CourseManager.Models;

public static class CourseRepository
{
    public static List<Course> Courses { get; } = new()
    {
        new Course { Id = 1, Code="KH-001", Title = "ASP.NET Core MVC",        Category = "backend",  Price = 1200000, Duration = 40, IsPublished = true  },
        new Course { Id = 2, Code="KH-002", Title = "Lập trình C# cơ bản",      Category = "backend",  Price = 800000,  Duration = 30, IsPublished = true  },
        new Course { Id = 3, Code="KH-003", Title = "SQL Server cho lập trình", Category = "backend",  Price = 900000,  Duration = 25, IsPublished = false },
        new Course { Id = 4, Code="KH-004", Title = "HTML5 & CSS3",             Category = "frontend", Price = 600000,  Duration = 20, IsPublished = true  },
        new Course { Id = 5, Code="KH-005", Title = "JavaScript nâng cao",      Category = "frontend", Price = 750000,  Duration = 28, IsPublished = true  },
        new Course { Id = 6, Code="KH-006", Title = "ReactJS thực chiến",       Category = "frontend", Price = 1100000, Duration = 35, IsPublished = false },
        new Course { Id = 7, Code="KH-007", Title = "Flutter cơ bản",           Category = "mobile",   Price = 1000000, Duration = 32, IsPublished = true  },
        new Course { Id = 8, Code="KH-008", Title = "React Native",             Category = "mobile",   Price = 950000,  Duration = 30, IsPublished = true  },
    };
}