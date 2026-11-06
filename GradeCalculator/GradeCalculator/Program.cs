using System.Globalization;
using System.Net;
using System.Text;

const double QuizW = 0.20, AssignW = 0.20, MidW = 0.25, FinalW = 0.35;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();   
app.UseStaticFiles();    


app.MapPost("/calculate", async (HttpContext ctx) =>
{
    var form = await ctx.Request.ReadFormAsync();

    string name = form["name"].ToString().Trim();
    string[] labels = { "Quiz", "Assignment", "Midterm Exam", "Final Exam" };
    string[] keys   = { "quiz", "assignment", "midterm", "final" };
    double[] scores = new double[4];
    var errors = new List<string>();

    
    if (name.Length == 0)
        errors.Add("Student name is required.");

    for (int i = 0; i < 4; i++)
    {
        bool ok = double.TryParse(form[keys[i]], NumberStyles.Float,
                                  CultureInfo.InvariantCulture, out scores[i]);
        if (!ok || !double.IsFinite(scores[i]) || scores[i] < 0 || scores[i] > 100)
            errors.Add($"{labels[i]} score must be a number between 0 and 100.");
    }

    if (errors.Count > 0)
    {
        var sb = new StringBuilder("<h2 class=\"error-title\">Please fix the following:</h2><ul class=\"errors\">");
        foreach (var e in errors) sb.Append($"<li>{WebUtility.HtmlEncode(e)}</li>");
        sb.Append("</ul><a class=\"btn\" href=\"/\">&larr; Go back</a>");
        return Results.Content(Page(sb.ToString()), "text/html");
    }

   
    double finalGrade = scores[0] * QuizW + scores[1] * AssignW
                      + scores[2] * MidW  + scores[3] * FinalW;

    
    char letter = GetLetter(finalGrade);
    bool passed = finalGrade >= 60;
    string remark = passed ? "Passed" : "Failed";
    string cls = passed ? "passed" : "failed";

  
    string F(double d) => d.ToString("F2", CultureInfo.InvariantCulture);
    string body = $@"
<h2>Result</h2>
<table>
  <tr><th>Student Name</th><td>{WebUtility.HtmlEncode(name)}</td></tr>
  <tr><th>Quiz (20%)</th><td>{F(scores[0])}</td></tr>
  <tr><th>Assignment (20%)</th><td>{F(scores[1])}</td></tr>
  <tr><th>Midterm Exam (25%)</th><td>{F(scores[2])}</td></tr>
  <tr><th>Final Exam (35%)</th><td>{F(scores[3])}</td></tr>
  <tr class=""total""><th>Final Grade</th><td>{F(finalGrade)}</td></tr>
  <tr><th>Letter Grade</th><td>{letter}</td></tr>
  <tr><th>Remark</th><td><span class=""badge {cls}"">{remark}</span></td></tr>
</table>
<a class=""btn"" href=""/"">Compute another student</a>";

    return Results.Content(Page(body), "text/html");
});

app.Run();

static char GetLetter(double grade)
{
    if (grade >= 90) return 'A';
    else if (grade >= 80) return 'B';
    else if (grade >= 70) return 'C';
    else if (grade >= 60) return 'D';
    else return 'F';
}


static string Page(string content) => $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <title>Student Grade Calculator</title>
  <link rel=""stylesheet"" href=""/style.css"">
</head>
<body>
  <main class=""card"">
    <h1>Student Grade Calculator</h1>
    {content}
  </main>
</body>
</html>";
