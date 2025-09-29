using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

public class Simulator : BackgroundService
{
    private readonly IHubContext<DrawHub> _hub;
    private int _x = 50;
    private int _y = 100;

    public Simulator(IHubContext<DrawHub> hub)
    {
        _hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var frame = new
            {
                commands = new object[]
                {
                    new { type = "clear" },
                    new { type = "circle", x = _x, y = _y, radius = 30, color = "lime" },
                    new { type = "line", x1 = _x, y1 = _y,
                                     x2 = _x + 30, y2 = _y, color = "red", width = 3 },
                    new { type = "rect", x = 20, y = 200, width = 100, height = 40, color = "orange", fill = true }
                }
            };

            var json = JsonSerializer.Serialize(frame);

            await _hub.Clients.All.SendAsync("DrawCommands", json, cancellationToken: stoppingToken);

            // Move robot right
            _x += 5;
            if (_x > 350) _x = 50;

            await Task.Delay(200, stoppingToken); // 5 FPS
        }
    }
}

