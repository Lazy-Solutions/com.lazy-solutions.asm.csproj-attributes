using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AdvancedSceneManager.CsProjAttributes
{

    class DocumentationProcessor : UnityEditor.AssetPostprocessor
    {

        static string OnGeneratedCSProject(string path, string content)
        {

            var projectName = Path.GetFileNameWithoutExtension(path);
            var assembly = FindAssembly(projectName);
            if (assembly == null)
                return content;

            Apply<DocumentationFileAttribute>();
            Apply<SuppressWarningAttribute>();
            Apply<IncludeAttribute>();
            Apply<NestFilesAttribute>();

            void Apply<T>() where T : CsProjAttribute
            {

                var attributes = assembly.GetCustomAttributes<T>().ToArray();

                if (attributes.Length == 0)
                    return;

                var fragment = attributes[0].GetCsProjFragment(attributes);

                if (!string.IsNullOrEmpty(fragment))
                {
                    var index = content.LastIndexOf("</Project>", StringComparison.Ordinal);
                    if (index >= 0)
                        content = content.Insert(index, fragment + Environment.NewLine);
                }

            }

            return content;

        }

        static Assembly FindAssembly(string projectName) =>
               AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == projectName);

    }

}
