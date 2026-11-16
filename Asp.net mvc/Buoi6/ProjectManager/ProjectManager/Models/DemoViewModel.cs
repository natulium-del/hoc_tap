using System.Collections.Generic;

namespace ProjectManager.Models;

public record CustomerSummary(string Name, int ProjectCount, decimal TotalBudget);
public record ResourceRow(string FullName, string Type, string Skill);
public record ProfileRow(string FullName, string? Phone);
public record ProjectRow(int Id, string Name, string Tags, string Members);

public class DemoViewModel
{
    public List<CustomerSummary> Customers { get; set; } = [];
    public List<ResourceRow> Resources { get; set; } = [];
    public List<string> Developers { get; set; } = [];
    public List<ProfileRow> Profiles { get; set; } = [];
    public List<ProjectRow> Projects { get; set; } = [];
}