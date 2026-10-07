namespace QuanLySinhVien.Middlewares;

public class RequestCounterMiddleware
{
    // Biến static — dùng chung cho mọi request, tồn tại suốt vòng đời app
    private static int _requestCount = 0;

    private readonly RequestDelegate _next;

    public RequestCounterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Tăng biến đếm — an toàn với đa luồng
        Interlocked.Increment(ref _requestCount);

        // Cho request đi tiếp
        await _next(context);
    }

    // Property public để endpoint /thong-ke đọc giá trị
    public static int RequestCount => _requestCount;
}

// Extension method đăng ký
public static class RequestCounterMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCounter(this IApplicationBuilder app)
        => app.UseMiddleware<RequestCounterMiddleware>();
}