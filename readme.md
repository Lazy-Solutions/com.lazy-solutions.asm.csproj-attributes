# ASM csproj attributes

This package provides a small set of **assembly-level attributes** that can be used to inject fragments into the **generated `.csproj` files** in Unity.

It is intended for **build- and tooling-related metadata**, not runtime behavior.

## What this is for

Unity generates `.csproj` files automatically. In some cases, it is useful to influence that generation, for example to:

- Enable XML documentation output
- Suppress specific compiler warnings
- Include non-code files in the project
- Nest related source files in IDEs

This package allows you to do that **declaratively**, using attributes on an assembly.

## How it works

At editor time, the package hooks into Unity’s csproj generation process and:

1. Identifies the assembly associated with the generated project
2. Reads supported attributes applied to that assembly
3. Injects the corresponding MSBuild fragments into the `.csproj`

If no supported attributes are present, nothing is modified.

## Available attributes
```csharp
// Enables XML documentation output for the assembly. Note that this isn't enough, more info below.
[assembly: DocumentationFile("Library/ScriptAssemblies/MyAssembly.xml")]

// Suppresses warnings about missing XML documentation on public members
[assembly: SuppressWarning("1591")] // Missing XML comment for publicly visible member

// Suppresses warnings about usage of obsolete members
[assembly: SuppressWarning("0618")] // 'member' is obsolete

// Includes matching non-code files in the generated project (e.g. for IDE visibility)
[assembly: Include("Packages/com.example.my-package/**/*.md")]

// Nests matching files under a parent file in supported IDEs
[assembly: NestFiles("MyType.cs", "MyType.*.cs")]

// Explicitly nests specific files under a parent file
[assembly: NestFiles("MyType.cs", "MyType.Editor.cs", "MyType.Tests.cs")]
```

## DocumentationFile

Unity does not perform a full MSBuild compilation when compiling projects.
As a result, enabling `<DocumentationFile>` alone is often **not enough** to produce
the XML documentation file.

In practice, a **manual build of the generated project** is required for the XML file
to be emitted.

Below is an example of how ASM triggers such a build using `dotnet build`:

```csharp
static async Task BuildCsProj()
{

    try
    {

        var psi = new ProcessStartInfo("dotnet", "build AdvancedSceneManager.csproj")
        {
            WorkingDirectory = Path.Combine(Application.dataPath, ".."),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var p = new Process { StartInfo = psi };

        var output = new List<string>();
        p.OutputDataReceived += (s, e) => { if (e.Data != null) output.Add(e.Data); };
        p.ErrorDataReceived += (s, e) => { if (e.Data != null) output.Add("[ERR] " + e.Data); };

        p.Start();
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();

        // wait with timeout
        var exited = await Task.Run(() => p.WaitForExit((int)TimeSpan.FromSeconds(30).TotalMilliseconds));

        if (!exited)
        {
            try { p.Kill(); } catch { /* ignore */ }
            p.CancelOutputRead();
            p.CancelErrorRead();
            Debug.LogError($"dotnet build timed out after 30s and was killed.\n{string.Join("\n", output)}");
            return;
        }

        if (p.ExitCode != 0)
        {
            Debug.LogError($"dotnet build failed (exit {p.ExitCode}).\n{string.Join("\n", output)}");
            return;
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"dotnet build failed: {ex}");
    }

}
```
