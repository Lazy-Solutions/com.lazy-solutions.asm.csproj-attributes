using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AdvancedSceneManager.CsProjAttributes
{

    /// <summary>Provides support for specifying <see langword="&lt;Compile Update=&quot;*&quot; DependsUpon=&quot;*&quot; /&gt;"/> in the generated csproj for a project.</summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class NestFilesAttribute : CsProjItemAttribute
    {

        public string parent { get; }
        public string pattern { get; }
        public string[] files { get; }

        // Pattern-based
        public NestFilesAttribute(string parent, string pattern)
        {
            this.parent = parent;
            this.pattern = pattern;
            this.files = Array.Empty<string>();
        }

        // Explicit
        public NestFilesAttribute(string parent, params string[] files)
        {
            this.parent = parent;
            this.files = files ?? Array.Empty<string>();
        }

        protected override void GenerateCsProjFragment(CsProjAttribute[] attributes)
        {
            foreach (var attr in attributes.OfType<NestFilesAttribute>())
            {

                var children = attr.files;

                if (!string.IsNullOrEmpty(attr.pattern))
                    children = FindMatchingFiles(attr.pattern);

                foreach (var child in children)
                    Insert("Compile", ("Update", child), ("DependentUpon", attr.parent));

            }
        }

        static string[] FindMatchingFiles(string pattern)
        {
            var root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

            return Directory
                .GetFiles(root, pattern, SearchOption.AllDirectories)
                .Select(NormalizeToProjectRelativePath)
                .ToArray();
        }

        static string NormalizeToProjectRelativePath(string fullPath)
        {
            var root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."))
                .Replace('\\', '/');

            var path = fullPath.Replace('\\', '/');

            if (path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                return path.Substring(root.Length + 1);

            return path;
        }

    }

}
