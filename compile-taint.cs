using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace MvcMovie.Controllers;

public class HomeCtrl : Controller
{

    private readonly YourDbContext _context;

    public HomeCtrl(YourDbContext context)
    {
        _context = context;
    }

    public IActionResult Test1(string inputVal)
    {
        string r;
        try{
            // ruleid: compile-taint
            r = CSharpScript.EvaluateAsync("System.Math.Pow(2, " + inputVal + ")")?.Result?.ToString();
        } catch (Exception e) {
            r = e.ToString();
        }
        return View(doSmth(r));
    }

    public IActionResult OkTest1(int inputVal)
    {
        string r;
        try{
            // ok: compile-taint
            r = CSharpScript.EvaluateAsync("System.Math.Pow(2, " + inputVal + ")")?.Result?.ToString();
        } catch (Exception e) {
            r = e.ToString();
        }
        return View(doSmth(r));
    }

    public IActionResult Test2([FromBody] CmdBody body)
    {
        // ruleid: compile-taint
        var users = CSharpScript.EvaluateAsync<Type>($"System.Math.Pow(2, {body.val})")?.Result?.ToString();
        return View(users);
    }

    [NonAction]
    public IActionResult Test3(CmdForm form)
    {
        // ok: compile-taint
        var users = CSharpScript.EvaluateAsync(string.Format("System.Math.Pow(2, {0})", form.name))?.Result?.ToString();
        return View(users);
    }

}

[Controller]
public class ThisIsCtrller
{
    public IActionResult Test6(string inputVal)
    {
        string r;
        try{
            // ruleid: compile-taint
            r = CSharpScript.RunAsync("System.Math.Pow(2, " + inputVal + ")")?.Result?.ToString();
        } catch (Exception e) {
            r = e.ToString();
        }
        return View(r);
    }

    private IActionResult _doSmth(string inputVal)
    {
        // ok: compile-taint
        var users = CSharpScript.EvaluateAsync<Type>($"System.Math.Pow(2, {inputVal})")?.Result?.ToString();
        return View();
    }

}

[NonController]
public class NotController
{
    public IActionResult OkTest2(string inputVal)
    {
        // ok: compile-taint
        var users = CSharpScript.RunAsync<Type>($"System.Math.Pow(2, {inputVal})")?.Result?.ToString();
        return View(users);
    }

}

public class ApiConroller : ControllerBase
{
    public IActionResult Test7(string inputVal)
    {
        StringBuilder sb = new StringBuilder("System.Math.Pow(2, ");
        sb.Append(inputVal);
        sb.Append(")");
        // ruleid: compile-taint
        var users = CSharpScript.RunAsync<Type>(sb)?.Result?.ToString();
        return View(users);
    }

    private Task<bool> runCmd(HttpContext httpContext)
    {
        var name = httpContext.Request.Query["name"].ToString();
        // ruleid: compile-taint
        var users = CSharpScript.Create(string.Format("System.Math.Pow(2, {0})", name))?.Result?.ToString();
        return true;
    }

    public IActionResult OkTest4(string inputVal)
    {
        int val = calcInt(inputVal);
        // ok: compile-taint
        var users = CSharpScript.Create($"System.Math.Pow(2, {val})")?.Result?.ToString();
        return View();
    }


    private string somethingElse(string val)
    {
        // ok: compile-taint
        var users = CSharpScript.EvaluateAsync($"System.Math.Pow(2, {val})")?.Result?.ToString();
        return View();
    }

}
