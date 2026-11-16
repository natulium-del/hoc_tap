namespace QuanLySinhVien.Middlewares;

public class BlockBotMiddleware
{
    private readonly RequestDelegate _next;

    public BlockBotMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Lấy giá trị header User-Agent
        var userAgent = context.Request.Headers.UserAgent.ToString();

        // Kiểm tra chứa "bot" hoặc "crawler" (không phân biệt hoa thường)
        if (userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("crawler", StringComparison.OrdinalIgnoreCase))
        {
            // Chặn — trả về 403, không gọi next()
            context.Response.StatusCode = 403;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync("Truy cập bị từ chối.");
            return;   // ← Short-circuit
        }

        // Không phải bot → cho đi tiếp
        await _next(context);
    }
}

// Extension method đăng ký gọn
public static class BlockBotMiddlewareExtensions
{
    public static IApplicationBuilder UseBlockBot(this IApplicationBuilder app)
        => app.UseMiddleware<BlockBotMiddleware>();
}